using GameCore.High;
using Newtonsoft.Json.Linq;
using SP.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    public class CropBlockDatum
    {
        public int threadCount;
        public string seed;
        public float speed;
        public List<CraftingRecipe_Item> matureCrops = new();
        public List<CropBlockDatum_Texture> textures = new();
    }

    public class CropBlockDatum_Texture
    {
        public int index;
        public TextureData texture;
    }

    public class CropBlockBehaviour : Block
    {
        public static Dictionary<string, CropBlockDatum> dataTemps = new();
        public CropBlockDatum cropDatum;
        public int cropIndex;
        public string randomUpdateID;


        public override void DoStart()
        {
            base.DoStart();

            randomUpdateID = $"ori:crop_blocks_{GetInstanceID()}";
            cropDatum = GetDatum();

            RandomUpdater.Bind(randomUpdateID, cropDatum.speed, Grow);
        }

        public override void OnRecovered()
        {
            base.OnRecovered();

            RandomUpdater.Unbind(randomUpdateID);
        }

        void Grow()
        {
            if (data.jo == null)
                return;

            //增加生长进度
            if (cropIndex + 1 < cropDatum.threadCount)
            {
                //增加 index
                cropIndex++;

                //刷新贴图
                if (sr)
                {
                    foreach (var texture in cropDatum.textures)
                    {
                        if (texture.index == cropIndex)
                        {
                            sr.sprite = texture.texture.sprite;
                            return;
                        }
                    }

                    sr.sprite = GInit.instance.spriteUnknown;
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

            if (GameTools.CompareVersions(data.jsonFormat, "0.6.2", Operators.thanOrEqual))
            {
                JToken cropJT = data.jo?["ori:corp_block"];
                JToken cropDip = cropJT?["display"];
                JToken cropProp = cropJT?["property"];

                cropDatum.threadCount = cropProp?["thread_count"]?.ToString()?.ToInt() ?? 3;
                cropDatum.seed = cropProp?["seed"]?.ToString();
                cropDatum.speed = 0.5f * (cropProp?["speed"]?.ToString()?.ToFloat() ?? 1);
                cropDip?["textures"]?.For(j =>
                {
                    cropDatum.textures.Add(new()
                    {
                        index = j?["index"]?.ToString()?.ToInt() ?? 0,
                        texture = ModFactory.CompareTexture(j?["texture"]?.ToString())
                    }); ;
                });
                cropProp?["mature_crops"]?.For(j =>
                {
                    cropDatum.matureCrops.Add(new(j?["id"]?.ToString(), (j?["count"]?.ToString()?.ToInt() ?? 1).ToUShort(), new List<string>()));
                });
            }

            dataTemps.Add(data.id, cropDatum);
            return cropDatum;
        }

        public override void OutputDrops(Vector3 pos)
        {
            //达到最大值, 即已成熟
            if (cropIndex >= cropDatum.threadCount - 1)
            {
                //生成掉落物
                cropDatum.matureCrops.For(a =>
                {
                    GM.instance.SummonItem(pos, a.id, a.count);
                });
            }
            else
            {
                //未设定种子则直接生成
                if (cropDatum.seed.IsNullOrWhiteSpace())
                    base.OutputDrops(pos);
                else
                    GM.instance.SummonItem(pos, cropDatum.seed);
            }
        }
    }
}
