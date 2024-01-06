using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.Longicorn)]
    public class Longicorn : TinyAnimal
    {
        public override string Texture() => "ori:longicorn_0";
    }
}