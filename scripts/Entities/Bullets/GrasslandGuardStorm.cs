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
            damage = 15;

            WhenCorrectedSyncVars(() =>
            {
                rb.velocity = customData["ori:bullet"]["velocity"].ToVector2();
            });
        }
    }
}