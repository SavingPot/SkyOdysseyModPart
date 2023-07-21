using DG.Tweening;
using GameCore.High;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    public class OakForestSlimeProperties : SlimeProperties<OakForestSlimeProperties>
    {
        public override string Texture() => "ori:oak_forest_slime.idle";
    }


    [EntityBinding(EntityID.OakForestSlime)]
    public class OakForestSlime : Slime<OakForestSlimeProperties>
    {

    }
}
