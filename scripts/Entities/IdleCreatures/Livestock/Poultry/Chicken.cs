using UnityEngine;

namespace GameCore
{

    [EntityBinding(EntityID.Chicken)]
    public class Chicken : Poultry
    {
        protected override void Awake()
        {
            base.Awake();

            eggId = "ori:chicken_egg";
            textureId = Random.value switch
            {
                < 0.5f => "ori:chicken_white_leghorn",
                _ => "ori:chicken_china_local"
            };

            //打鸣
            if (Tools.Prob100(50))
            {
                GTime.BindTimeEvent(5, true, () =>
                {
                    GAudio.Play(AudioID.Rooster, transform.position);
                });
            }
        }
    }
}

