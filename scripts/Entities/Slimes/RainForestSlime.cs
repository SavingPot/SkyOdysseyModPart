using DG.Tweening;
using GameCore.High;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    [EntityBinding(EntityID.RainForestSlime)]
    public class RainForestSlime : Slime
    {
        protected override void Awake()
        {
            base.Awake();

            Texture = "ori:rain_forest_slime.idle";
        }
    }
}
