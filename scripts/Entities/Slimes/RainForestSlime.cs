using DG.Tweening;
using GameCore.High;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    public class RainForestSlimeProperties : SlimeProperties<RainForestSlimeProperties>
    {
        public override string Texture() => "ori:rain_forest_slime.idle";
    }


    [EntityBinding(EntityID.RainForestSlime)]
    public class RainForestSlime : Slime<RainForestSlimeProperties>
    {

    }
}
