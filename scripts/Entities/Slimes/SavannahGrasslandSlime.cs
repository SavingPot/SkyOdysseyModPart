using DG.Tweening;
using GameCore.High;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    public class SavannahGrasslandSlimeProperties : SlimeProperties<SavannahGrasslandSlimeProperties>
    {
        public override string Texture() => "ori:savannah_grassland_slime.idle";
    }


    [EntityBinding(EntityID.SavannahGrasslandSlime)]
    public class SavannahGrasslandSlime : Slime<SavannahGrasslandSlimeProperties>
    {

    }
}
