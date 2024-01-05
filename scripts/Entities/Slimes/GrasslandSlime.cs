using DG.Tweening;
using GameCore.High;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    [EntityBinding(EntityID.GrasslandSlime)]
    public class GrasslandSlime : Slime
    {
        protected override void Awake()
        {
            base.Awake();

            Texture = "ori:grassland_slime.idle";
        }
    }
}
