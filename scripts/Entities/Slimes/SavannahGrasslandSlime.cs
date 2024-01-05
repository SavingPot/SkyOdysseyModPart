using DG.Tweening;
using GameCore.High;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    [EntityBinding(EntityID.SavannahGrasslandSlime)]
    public class SavannahGrasslandSlime : Slime
    {
        protected override void Awake()
        {
            base.Awake();

            Texture = "ori:savannah_grassland_slime.idle";
        }
    }
}
