using GameCore.High;
using GameCore.Network;
using Newtonsoft.Json.Linq;
using SP.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameCore
{
    public class CropBlockDatum
    {
        public string seed;
        public float speed;
        public List<CraftingRecipe_Item> harvests = new();
        public List<CropBlockDatum_Process> processes = new();
    }

    public class CropBlockDatum_Process
    {
        public TextureData texture;
    }

    public struct DropResult
    {
        public string id;
        public ushort count;
        public string customData;

        public DropResult(string id, ushort count, string customData)
        {
            this.id = id;
            this.count = count;
            this.customData = customData;
        }
    }

    public static class DefaultCropDelegates
    {
        public static float DecideGrowProbability(CropBlock block, Block underBlock) => underBlock.data.id switch
        {
            BlockID.GrassBlock => block.cropDatum.speed * 2,
            BlockID.Dirt => block.cropDatum.speed * 4,
            BlockID.Farmland => block.cropDatum.speed * 6,
            _ => 0,
        };


        public static void CropGrow(CropBlock block)
        {
            block.Grow();
        }

        public static DropResult[] CutResults(CropBlock block, Vector3 pos)
        {
            List<DropResult> items = new();

            //未设定种子则直接掉落方块本身
            if (block.cropDatum.seed.IsNullOrWhiteSpace())
            {
                items.Add(new(block.data.id, 1, block.customData?.ToString()));
            }
            //设定了种子则掉落设定的种子
            else
            {
                items.Add(new(block.cropDatum.seed, 1, null));
            }

            return items.ToArray();
        }

        public static DropResult[] HarvestResults(CropBlock block, Vector3 pos)
        {
            List<DropResult> items = new();

            //生成掉落物
            block.cropDatum.harvests.For(a =>
            {
                items.Add(new(a.id, a.count, null));
            });

            return items.ToArray();
        }
    }

    public delegate float DecideGrowProbabilityDelegate(CropBlock block, Block underBlock);
    public delegate void CropGrowDelegate(CropBlock block);
    public delegate DropResult[] CutResultsDelegate(CropBlock block, Vector3 pos);
    public delegate DropResult[] HarvestResultsDelegate(CropBlock block, Vector3 pos);

    public class CropBlock : Plant
    {
        public static readonly Dictionary<string, CropBlockDatum> dataTemps = new();
        public CropBlockDatum cropDatum;
        public int cropIndex;
        public string randomUpdateID;
        public bool hasBindGrowMethod;

        public static DecideGrowProbabilityDelegate DecideGrowProbability;
        public static CropGrowDelegate CropGrow;
        public static CutResultsDelegate CutResults;
        public static HarvestResultsDelegate HarvestResults;

        public static Action GetCrop = () =>
        {
            DecideGrowProbability = DefaultCropDelegates.DecideGrowProbability;
            CropGrow = DefaultCropDelegates.CropGrow;
            CutResults = DefaultCropDelegates.CutResults;
            HarvestResults = DefaultCropDelegates.HarvestResults;

            if (Player.TryGetLocal(out var player))
            {
                //TODO: 通用化
                if (player.unlockedSkills.Any(p => p.unlocked && p.id == SkillID.Agriculture_Quick))
                {
                    var OldDecideGrowProbability = DecideGrowProbability;
                    DecideGrowProbability = (block, underBlock) => OldDecideGrowProbability(block, underBlock) * 1.15f;
                }
                if (player.unlockedSkills.Any(p => p.unlocked && p.id == SkillID.Agriculture_Coin))
                {
                    var OldHarvestResults = HarvestResults;
                    HarvestResults = (block, pos) =>
                    {
                        //20% 的几率掉落 1 个硬币
                        if (Tools.Prob100(20))
                            GM.instance.SummonCoinEntity(pos, 1);

                        return OldHarvestResults(block, pos);
                    };
                }
                if (player.unlockedSkills.Any(p => p.unlocked && p.id == SkillID.Agriculture_Harvest))
                {
                    var OldHarvestResults = HarvestResults;
                    HarvestResults = (block, pos) =>
                    {
                        DropResult[] results = OldHarvestResults(block, pos);

                        //有 10% 的几率使结果数量翻倍
                        if (Tools.Prob100(10))
                        {
                            for (int i = 0; i < results.Length; i++)
                            {
                                var result = results[i];

                                //数量翻倍
                                result.count = (ushort)Mathf.Min(result.count * 2, ModFactory.CompareItem(result.id).maxCount);

                                results[i] = result;
                            }
                        }

                        return results;
                    };
                }
            }
        };





        public override void DoStart()
        {
            base.DoStart();

            //TODO: 解决客户端同步问题
            if (Server.isServer)
            {
                //决定生长
                randomUpdateID = $"ori:crop_blocks_{gameObject.GetInstanceID()}";
                cropDatum = GetDatum();
                GetCrop();

                //加载生长进度
                customData ??= new();
                var cropJT = customData["ori:crop"];
                if (cropJT == null)
                {
                    customData.AddObject("ori:crop", new JProperty("crop_index", 0));
                    WriteCustomDataToSave();
                }
                else
                {
                    cropIndex = cropJT["crop_index"].ToInt();
                }
                RefreshTextureByCropIndex();
            }
        }

        public void WriteCropIndexToSave()
        {
            customData["ori:crop"]["crop_index"] = cropIndex;
            WriteCustomDataToSave();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (!chunk.map.TryGetBlock(new(pos.x, pos.y - 1), isBackground, out var underBlock))
            {
                Destroy();
                return;
            }
            else
            {
                UnbindGrowingMethod(this);

                var probability = DecideGrowProbability(this, underBlock);

                if (probability != 0)
                {
                    RandomUpdater.Bind(randomUpdateID, probability, Grow);
                    hasBindGrowMethod = true;
                }
            }
        }

        public override void OnRecovered()
        {
            base.OnRecovered();

            UnbindGrowingMethod(this);
        }

        public static void UnbindGrowingMethod(CropBlock block)
        {
            if (block.hasBindGrowMethod)
            {
                RandomUpdater.Unbind(block.randomUpdateID);
                block.hasBindGrowMethod = false;
            }
        }

        public void Grow()
        {
            //如果已经达到最大值, 则不再生长
            if (data.jo == null || cropIndex >= cropDatum.processes.Count - 1)
                return;

            //增加生长进度
            cropIndex++;
            WriteCropIndexToSave();

            //刷新贴图
            RefreshTextureByCropIndex();
        }

        void RefreshTextureByCropIndex()
        {
            if (sr)
            {
                try
                {
                    sr.sprite = cropDatum.processes[cropIndex].texture.sprite;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"刷新作物贴图失败, 异常如下: {ex}");
                    sr.sprite = GInit.instance.spriteUnknown;
                    throw;
                }
            }
        }

        public override void OutputDrops(Vector3 pos)
        {
            //达到最大值, 即已成熟
            var results = (cropIndex >= cropDatum.processes.Count - 1) ? HarvestResults(this, pos) : CutResults(this, pos);

            foreach (var item in results)
            {
                GM.instance.SummonDrop(pos, item.id, item.count, item.customData);
            }
        }





        public CropBlockDatum GetDatum()
        {
            if (dataTemps.TryGetValue(data.id, out CropBlockDatum value))
            {
                return value;
            }

            //添加默认值
            CropBlockDatum cropDatum = new();

            if (GameTools.CompareVersions(data.jsonFormat, "0.7.8", Operators.thanOrEqual))
            {
                JToken cropJT = data.jo?["ori:crop_block"];

                if (cropJT != null && cropJT.Type == JTokenType.Object)
                {
                    var processes = cropJT["processes"];

                    cropDatum.seed = cropJT["seed"]?.ToString();
                    cropDatum.speed = 0.3f * (cropJT["speed"]?.ToString()?.ToFloat() ?? 1);
                    cropJT["harvests"]?.For(j =>
                    {
                        cropDatum.harvests.Add(new(j?["id"]?.ToString(), (j?["count"]?.ToString()?.ToInt() ?? 1).ToUShort(), new List<string>()));
                    });

                    if (processes == null || processes.Type != JTokenType.Array || processes.Count() == 0)
                    {
                        Debug.LogError($"加载作物失败: 作物方块 {data.id} 的 json 文件中不包含 ori:crop_block/processes 或者 processes 不是数组!");
                        return null;
                    }
                    else
                    {
                        processes.For(j =>
                        {
                            cropDatum.processes.Add(new()
                            {
                                texture = ModFactory.CompareTexture(j["texture"]?.ToString())
                            }); ;
                        });
                    }
                }
                else
                {
                    Debug.LogError($"加载作物失败: 作物方块 {data.id} 的 json 文件中不包含 ori:crop_block 或者 ori:crop_block 不是对象!");
                    return null;
                }
            }
            else
            {
                Debug.LogError($"加载作物失败: 方块 {data.id} 的 json 格式版本 {data.jsonFormat} 不受支持!");
                return null;
            }

            dataTemps.Add(data.id, cropDatum);
            return cropDatum;
        }
    }
}
