using SP.Tools.Unity;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.ExtremeWeatherRegionGuardSnowball)]
    public class ExtremeWeatherRegionGuardSnowball : Bullet
    {
        public override void Initialize()
        {
            base.Initialize();
            damage = 15;

            rb.velocity = customData["ori:bullet"]["velocity"].ToVector2();
        }
    }
}