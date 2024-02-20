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
        public float escapeInterval = 0.5f;





        public override void OnGetHurtClient(float damage, float invincibleTime, Vector2 damageOriginPos, Vector2 impactForce, NetworkConnection caller)
        {
            base.OnGetHurtClient(damage, invincibleTime, damageOriginPos, impactForce, caller);

            escapeEndTime = Tools.time + escapeLastTime;
        }

        public override Vector2 GetMovementDirection()
        {
            /* ----------------------------------- 逃脱 ----------------------------------- */
            if (Tools.time < escapeEndTime && Tools.time >= escapeNextStartTime)
            {
                escapeNextStartTime = Tools.time + escapeInterval;

                return Random.Range(-2, 3) switch
                {
                    -2 => TurnLeft(),
                    > -2 and < 2 => new(0, rb.velocity.y),
                    2 => TurnRight(),
                    _ => throw new()
                };
            }
            /* ----------------------------------- 闲逛 ----------------------------------- */
            else if (Tools.time >= movementTimer)
            {
                movementTimer = Tools.time + movementInterval;

                return Random.Range(-2, 3) switch
                {
                    -2 => TurnLeft(),
                    > -2 and < 2 => new(0, rb.velocity.y),
                    2 => TurnRight(),
                    _ => throw new(),
                };
            }

            return Vector2.zero;
        }

        protected virtual Vector2 TurnLeft()
        {
            SetOrientation(false);

            return Vector2.left;
        }

        protected virtual Vector2 TurnRight()
        {
            SetOrientation(true);

            return Vector2.right;
        }
    }
}