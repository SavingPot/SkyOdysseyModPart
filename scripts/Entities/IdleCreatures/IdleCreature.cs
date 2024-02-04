using Mirror;
using UnityEngine;

namespace GameCore
{
    public class IdleCreature : Creature
    {
        public float movementTimer;
        public float movementInterval = 5;
        public float escapeEndTime;
        public float escapeLastTime = 4;
        public float escapeNextStartTime;
        public float escapeInterval = 1;

        public override void OnGetHurtClient(float damage, float invincibleTime, Vector2 damageOriginPos, Vector2 impactForce, NetworkConnection caller)
        {
            base.OnGetHurtClient(damage, invincibleTime, damageOriginPos, impactForce, caller);

            escapeEndTime = Tools.time + escapeLastTime;
        }

        protected override void Update()
        {
            base.Update();

            /* ----------------------------------- 逃脱 ----------------------------------- */
            if (Tools.time < escapeEndTime && Tools.time >= escapeNextStartTime)
            {
                Vector2 velocity = Random.Range(-1, 2) switch
                {
                    -1 => TurnLeft(),
                    0 => new(0, rb.velocity.y),
                    1 => TurnRight(),
                    _ => throw new()
                };
                rb.velocity = velocity;
                
                escapeNextStartTime = Tools.time + escapeInterval;
            }
            /* ----------------------------------- 闲逛 ----------------------------------- */
            else if (Tools.time >= movementTimer)
            {
                Vector2 velocity = Random.Range(-2, 3) switch
                {
                    -2 => TurnLeft(),
                    > -2 and < 2 => new(0, rb.velocity.y),
                    2 => TurnRight(),
                    _ => throw new(),
                };
                rb.velocity = velocity;

                movementTimer = Tools.time + movementInterval;
            }
        }

        protected virtual Vector2 TurnLeft()
        {
            Vector2 velocity = GetMovementVelocity(Vector2.left);
            SetOrientation(false);

            return velocity;
        }

        protected virtual Vector2 TurnRight()
        {
            Vector2 velocity = GetMovementVelocity(Vector2.right);
            SetOrientation(true);

            return velocity;
        }
    }
}