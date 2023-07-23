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
            damage = 20;

            WhenCorrectedSyncVars(() =>
            {
                rb.velocity = customData["ori:bullet"]["velocity"].ToVector2();
            });
        }
    }
}