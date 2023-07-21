using DG.Tweening;
using GameCore.High;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    public class DesertSlimeProperties : SlimeProperties<DesertSlimeProperties>
    {
        public override string Texture() => "ori:desert_slime.idle";
    }


    [EntityBinding(EntityID.DesertSlime)]
    public class DesertSlime : Slime<DesertSlimeProperties>
    {

    }
}
