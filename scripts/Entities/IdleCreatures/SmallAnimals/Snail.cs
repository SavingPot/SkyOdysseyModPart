using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.Snail)]
    public class Snail : SmallAnimal
    {
        public override string Texture()
        {
            return "ori:snail_0";
        }
    }
}