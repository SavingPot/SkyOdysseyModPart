using SP.Tools.Unity;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.ExtremeWeatherRegionGuardSnowball)]
    public class ExtremeWeatherRegionGuardSnowball : Bullet
    {
        protected override void Start()
        {
            base.Start();
            damage = 15;

            rb.velocity = customData["ori:bullet"]["velocity"].ToVector2();
        }
    }
}