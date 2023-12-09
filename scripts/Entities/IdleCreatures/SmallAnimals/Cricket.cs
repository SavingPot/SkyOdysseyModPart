using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.Cricket)]
    public class Cricket : SmallAnimal
    {
        public override string Texture()
        {
            return "ori:cricket_0";
        }
    }
}