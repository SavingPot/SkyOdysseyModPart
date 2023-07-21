using DG.Tweening;
using GameCore.High;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    public class SnowfieldSlimeProperties : SlimeProperties<SnowfieldSlimeProperties>
    {
        public override string Texture() => "ori:snowfield_slime.idle";
    }


    [EntityBinding(EntityID.SnowfieldSlime)]
    public class SnowfieldSlime : Slime<SnowfieldSlimeProperties>
    {

    }
}
