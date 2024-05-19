using SP.Tools.Unity;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.ExtremeWeatherGuardSnowball)]
    public class ExtremeWeatherGuardSnowball : Bullet
    {
        public override void Initialize()
        {
            base.Initialize();
            damage = 15;

            rb.velocity = customData["ori:bullet"]["velocity"].ToVector2();
        }
    }
}