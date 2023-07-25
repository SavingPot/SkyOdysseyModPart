using SP.Tools.Unity;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.SnowfieldGuardSnowball)]
    public class SnowfieldGuardSnowball : Bullet
    {
        protected override void Start()
        {
            base.Start();
            damage = 15;

            WhenCorrectedSyncVars(() =>
            {
                rb.velocity = customData["ori:bullet"]["velocity"].ToVector2();
            });
        }
    }
}