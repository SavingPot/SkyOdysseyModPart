using DG.Tweening;
using GameCore.High;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    [EntityBinding(EntityID.DirtSlime)]
    public class DirtSlime : Slime
    {
        protected override void Awake()
        {
            base.Awake();

            Texture = "ori:dirt_slime.idle";
        }
    }
}
