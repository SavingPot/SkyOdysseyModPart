using DG.Tweening;
using GameCore.High;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    public class BambooGroveSlimeProperties : SlimeProperties<BambooGroveSlimeProperties>
    {
        public override string Texture() => "ori:bamboo_grove_slime.idle";
    }


    [EntityBinding(EntityID.BambooGroveSlime)]
    public class BambooGroveSlime : Slime<BambooGroveSlimeProperties>
    {

    }
}
