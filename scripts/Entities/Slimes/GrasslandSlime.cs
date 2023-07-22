using DG.Tweening;
using GameCore.High;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    public class GrasslandSlimeProperties : SlimeProperties<GrasslandSlimeProperties>
    {
        public override string Texture() => "ori:grassland_slime.idle";
    }


    [EntityBinding(EntityID.GrasslandSlime)]
    public class GrasslandSlime : Slime<GrasslandSlimeProperties>
    {

    }
}
