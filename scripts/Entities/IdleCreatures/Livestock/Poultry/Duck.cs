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
            textureId = Random.value switch
            {
                < 0.5f => "ori:duck_cute",
                _ => "ori:duck_river"
            };
        }
    }
}

