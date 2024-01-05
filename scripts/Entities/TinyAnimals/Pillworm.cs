using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.Pillworm)]
    public class Pillworm : TinyAnimal
    {
        public override string Texture() => "ori:pillworm_0";
    }
}