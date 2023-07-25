using UnityEngine;

namespace GameCore
{
    public class DuckProperties : PoultryProperties<DuckProperties>
    {
        public override string EggID() => "ori:duck_egg";
        public override string Texture() => "ori:duck_cute";
    }

    [EntityBinding(EntityID.Duck)]
    public class Duck : Poultry<DuckProperties>
    {

    }
}

