using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.Cricket)]
    public class Cricket : TinyAnimal
    {
        public override string Texture()
        {
            return "ori:cricket_0";
        }
    }
}