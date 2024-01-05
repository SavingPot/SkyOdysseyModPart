using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.Snail)]
    public class Snail : TinyAnimal
    {
        public override string Texture()=> "ori:snail_0";
    }
}