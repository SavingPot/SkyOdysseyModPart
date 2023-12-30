using UnityEngine;

namespace GameCore
{

    [EntityBinding(EntityID.Chicken)]
    public class Chicken : Poultry
    {
        public bool hasCrowedToday;

        protected override void Awake()
        {
            base.Awake();

            eggId = "ori:chicken_egg";
            textureId = "ori:chicken_white_leghorn";
        }

        protected override void Update()
        {
            base.Update();

            //打鸣
            if (!hasCrowedToday && GTime.IsInTime(GTime.time24Format, 4.95f, 5.05f) && Tools.Prob100(8 * Tools.deltaTime))
            {
                GAudio.Play(AudioID.Rooster);

                hasCrowedToday = true;
            }
        }
    }
}

