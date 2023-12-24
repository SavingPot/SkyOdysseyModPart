using SP.Tools.Unity;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.ExtremeWeatherRegionGuardSand)]
    public class ExtremeWeatherRegionGuardSand : Bullet
    {
        protected override void Start()
        {
            base.Start();

            rb.gravityScale = 0;
            damage = 20;

            rb.velocity = customData["ori:bullet"]["velocity"].ToVector2();

            AddSpriteRenderer("ori:desert_guard_sand");
        }
    }
}