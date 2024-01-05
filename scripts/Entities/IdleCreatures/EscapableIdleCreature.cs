using Mirror;
using UnityEngine;

namespace GameCore
{
    public class EscapableIdleCreature : IdleCreature
    {
        public float escapeTimer;
        public float escapeTime = 4;

        public override void OnGetHurtClient(float damage, float invincibleTime, Vector2 damageOriginPos, Vector2 impactForce, NetworkConnection caller)
        {
            base.OnGetHurtClient(damage, invincibleTime, damageOriginPos, impactForce, caller);

            escapeTimer = Tools.time + escapeTime;
        }

        protected override void Update()
        {
            base.Update();

            if (Tools.time < escapeTimer)
            {
                Vector2 velocity = Random.Range(-1, 2) switch
                {
                    -1 => TurnLeft(),
                    0 => GetMovementVelocity(rb.velocity),
                    1 => TurnRight(),
                    _ => throw new()
                };
                rb.velocity = velocity;
            }
        }
    }
}