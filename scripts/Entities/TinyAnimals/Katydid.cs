using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.Katydid)]
    public class Katydid : TinyAnimal
    {
        public override string Texture() => "ori:katydid_0";
    }
}