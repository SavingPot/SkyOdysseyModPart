using DG.Tweening.Core;
using UnityEngine;
using System.Collections.Generic;
using SP.Tools;
using Newtonsoft.Json.Linq;
using DG.Tweening;

namespace GameCore
{
    public class AnimationBlockBehaviour : Block
    {
        public class AnimationBlockDatum
        {
            public Sprite[] sprites;
            public float time;
        }


        public static Dictionary<string, AnimationBlockDatum> dataTemps = new();
        public AnimationBlockDatum animationDatum;
        public TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> anim;


        public override void DoStart()
        {
            base.DoStart();

            animationDatum = GetDatum(this);
            anim = EasyAnim.PlaySprites(animationDatum.time, animationDatum.sprites, sprite => { sr.sprite = sprite; return true; });
        }

        public override void OnRecovered()
        {
            base.OnRecovered();

            anim.Kill();
        }


        public static AnimationBlockDatum GetDatum(Block block)
        {
            var data = block.data;

            if (dataTemps.TryGetValue(data.id, out AnimationBlockDatum value))
            {
                return value;
            }

            //添加默认值
            AnimationBlockDatum animDatum = new();

            if (true)//(GameTools.CompareVersions(datum.jsonFormat, "0.6.2", Operators.thanOrEqual))
            {
                JToken animations = data.jo["ori:animations"];
                JToken textures = animations?["textures"];
                JToken time = animations?["time"];

                if (animations != null && textures != null && time != null)
                {
                    List<Sprite> tempList = new();

                    foreach (var item in textures)
                    {
                        tempList.Add(ModFactory.CompareTexture(item.ToString()).sprite);
                    }

                    animDatum.sprites = tempList.ToArray();
                    animDatum.time = int.TryParse(time.ToString(), out int result) ? result : 1;
                }
            }

            dataTemps.Add(data.id, animDatum);
            return animDatum;
        }
    }
}