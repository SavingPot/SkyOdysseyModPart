using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.Caterpillar)]
    public class Caterpillar : TinyAnimal
    {
        public override string Texture() => "ori:caterpillar_0";
    }
}