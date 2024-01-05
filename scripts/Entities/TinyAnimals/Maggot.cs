using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.Maggot)]
    public class Maggot : TinyAnimal
    {
        public override string Texture() => "ori:maggot_0";
    }
}