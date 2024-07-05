using SP.Tools.Unity;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.ExtremeWeatherGuardSand)]
    public class ExtremeWeatherGuardSand : Bullet
    {
        public override void Initialize()
        {
            base.Initialize();

            rb.gravityScale = 0;
            damage = 20;

            AddSpriteRenderer("ori:desert_guard_sand");
        }
    }
}