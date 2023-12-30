using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.Duck)]
    public class Duck : Poultry
    {
        protected override void Awake()
        {
            base.Awake();

            eggId = "ori:duck_egg";
            textureId = "ori:duck_cute";
        }
    }
}

