using UnityEngine;

namespace GameCore
{
    public class PigProperties : LivestockProperties<PigProperties>
    {
        public override string Texture() => "ori:pig_pink";
    }

    [EntityBinding(EntityID.Pig)]
    public class Pig : Livestock<PigProperties>
    {

    }
}

