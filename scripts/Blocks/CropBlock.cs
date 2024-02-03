using GameCore.High;
using Newtonsoft.Json.Linq;
using SP.Tools;
using System.Collections.Generic;
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

    public class CropBlock : Block
    {
        public static Dictionary<string, CropBlockDatum> dataTemps = new();
        public CropBlockDatum cropDatum;
        public int cropIndex;
        public string randomUpdateID;
        public bool hasBindGrowMethod;


        public override void DoStart()
        {
            base.DoStart();

            randomUpdateID = $"ori:crop_blocks_{gameObject.GetInstanceID()}";
            cropDatum = GetDatum();
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
                    case BlockID.Dirt:
                    case BlockID.GrassBlock:
                        RandomUpdater.Bind(randomUpdateID, cropDatum.speed, Grow);
                        hasBindGrowMethod = true;
                        break;

                    case BlockID.Farmland:
                        RandomUpdater.Bind(randomUpdateID, cropDatum.speed * 2, Grow);
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

        void Grow()
        {
            if (data.jo == null)
                return;

            //增加生长进度
            if (cropIndex + 1 < cropDatum.processes.Count)
            {
                //增加 index
                cropIndex++;

                //刷新贴图
                if (sr)
                {
                    try
                    {
                        sr.sprite = cropDatum.processes[cropIndex].texture.sprite;
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"刷新作物贴图失败, 异常如下: {ex}");
                        sr.sprite = GInit.instance.spriteUnknown;
                        throw;
                    }
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

            if (GameTools.CompareVersions(data.jsonFormat, "0.6.2", Operators.lessOrEqual))
            {
                JToken cropJT = data.jo?["ori:crop_block"];

                if (cropJT != null)
                {
                    cropDatum.seed = cropJT["seed"]?.ToString();
                    cropDatum.speed = 0.3f * (cropJT["speed"]?.ToString()?.ToFloat() ?? 1);
                    cropJT["harvests"]?.For(j =>
                    {
                        cropDatum.harvests.Add(new(j?["id"]?.ToString(), (j?["count"]?.ToString()?.ToInt() ?? 1).ToUShort(), new List<string>()));
                    });
                    cropJT?["textures"]?.For(j =>
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

            }

            dataTemps.Add(data.id, cropDatum);
            return cropDatum;
        }

        public override void OutputDrops(Vector3 pos)
        {
            //达到最大值, 即已成熟
            if (cropIndex >= cropDatum.processes.Count - 1)
            {
                //生成掉落物
                cropDatum.harvests.For(a =>
                {
                    GM.instance.SummonDrop(pos, a.id, a.count);
                });
            }
            else
            {
                //未设定种子则直接生成
                if (cropDatum.seed.IsNullOrWhiteSpace())
                    base.OutputDrops(pos);
                else
                    GM.instance.SummonDrop(pos, cropDatum.seed);
            }
        }
    }
}
