using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.Cicada)]
    public class Cicada : TinyAnimal
    {
        public override string Texture() => "ori:cicada_0";
    }
}