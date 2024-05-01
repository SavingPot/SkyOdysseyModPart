using GameCore.High;
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

    public interface ICrop
    {
        void Grow();
        HarvestResult[] HarvestResults(Vector3 pos);
    }

    public abstract class CropDecorator : ICrop
    {
        protected readonly ICrop crop;

        public CropDecorator(ICrop crop)
        {
            this.crop = crop;
        }

        public abstract void Grow();
        public abstract HarvestResult[] HarvestResults(Vector3 pos);
    }

    public struct HarvestResult
    {
        public string id;
        public ushort count;
        public string customData;

        public HarvestResult(string id, ushort count, string customData)
        {
            this.id = id;
            this.count = count;
            this.customData = customData;
        }
    }

    public class NormalCrop : ICrop
    {
        public CropBlock block;

        public void Grow()
        {
            block.Grow();
        }

        public HarvestResult[] HarvestResults(Vector3 pos)
        {
            List<HarvestResult> items = new();

            //达到最大值, 即已成熟
            if (block.cropIndex >= block.cropDatum.processes.Count - 1)
            {
                //生成掉落物
                block.cropDatum.harvests.For(a =>
                {
                    items.Add(new(a.id, a.count, null));
                });
            }
            else
            {
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
            }

            return items.ToArray();
        }



        public NormalCrop(CropBlock block)
        {
            this.block = block;
        }
    }

    public class CropBlock : Block
    {
        public static readonly Dictionary<string, CropBlockDatum> dataTemps = new();
        public CropBlockDatum cropDatum;
        public int cropIndex;
        public string randomUpdateID;
        public bool hasBindGrowMethod;

        public static Func<CropBlock, ICrop> GetCrop = (block) =>
        {
            ICrop result = new NormalCrop(block);

            if (Player.TryGetLocal(out var player))
            {
                if (player.unlockedSkills.Any(p => p.unlocked && p.id == SkillID.Agriculture_Harvest))
                {
                    result = new DoubleHarvestDecorator(result);
                }
            }

            return result;
        };
        public ICrop crop;



        public override void DoStart()
        {
            base.DoStart();

            randomUpdateID = $"ori:crop_blocks_{gameObject.GetInstanceID()}";
            cropDatum = GetDatum();
            crop = GetCrop(this);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (!chunk.map.TryGetBlock(new(pos.x, pos.y - 1), isBackground, out var block))
            {
                Destroy();
                return;
            }
            else
            {
                UnbindGrowingMethod(this);

                switch (block.data.id)
                {
                    case BlockID.GrassBlock:
                        RandomUpdater.Bind(randomUpdateID, cropDatum.speed * 2, Grow);
                        hasBindGrowMethod = true;
                        break;

                    case BlockID.Dirt:
                        RandomUpdater.Bind(randomUpdateID, cropDatum.speed * 4, Grow);
                        hasBindGrowMethod = true;
                        break;

                    case BlockID.Farmland:
                        RandomUpdater.Bind(randomUpdateID, cropDatum.speed * 6, Grow);
                        hasBindGrowMethod = true;
                        break;

                    default:
                        hasBindGrowMethod = false;
                        break;
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
            }
        }

        public void Grow()
        {
            //如果已经达到最大值, 则不再生长
            if (data.jo == null || cropIndex + 1 < cropDatum.processes.Count)
                return;

            //增加生长进度
            cropIndex++;

            //刷新贴图
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

        public override void OutputDrops(Vector3 pos)
        {
            var results = crop.HarvestResults(pos);

            foreach (var item in results)
            {
                GM.instance.SummonDrop(pos, item.id, item.count, item.customData);
            }
        }
    }
}
