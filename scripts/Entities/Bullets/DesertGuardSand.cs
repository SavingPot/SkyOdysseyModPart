using SP.Tools.Unity;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.DesertGuardSand)]
    public class DesertGuardSand : Bullet
    {
        protected override void Start()
        {
            base.Start();

            rb.gravityScale = 0;
            damage = 20;
            livingTime = 4;

            rb.velocity = customData["ori:bullet"]["velocity"].ToVector2();

            AddSpriteRenderer("ori:desert_guard_sand");
        }
    }
}