using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.Pig)]
    public class Pig : Livestock
    {
        protected override void Awake()
        {
            base.Awake();

            textureId = "ori:pig_pink";
        }
    }
}

