using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.Earthworm)]
    public class Earthworm : TinyAnimal
    {
        public override string Texture() => "ori:earthworm_0";
    }
}