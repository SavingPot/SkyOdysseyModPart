using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.Fly)]
    public class Fly : TinyAnimal
    {
        public override string Texture() => "ori:fly_0";
    }
}