using UnityEngine;

namespace GameCore
{
    public class ChickenProperties : PoultryProperties<ChickenProperties>
    {
        public override string EggID() => "ori:chicken_egg";
        public override string Texture() => "ori:chicken_white_leghorn";
    }

    [EntityBinding(EntityID.Chicken)]
    public class Chicken : Poultry<ChickenProperties>
    {
        public bool hasCrowedToday;

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

