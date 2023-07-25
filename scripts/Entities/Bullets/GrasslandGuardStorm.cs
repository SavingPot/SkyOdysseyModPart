using SP.Tools.Unity;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.GrasslandGuardStorm)]
    public class GrasslandGuardStorm : Bullet
    {
        protected override void Start()
        {
            base.Start();

            rb.gravityScale = 0;
            damage = 10;

            WhenCorrectedSyncVars(() =>
            {
                rb.velocity = customData["ori:bullet"]["velocity"].ToVector2();
            });

            AddSpriteRenderer("ori:grassland_guard_storm");
        }
    }
}