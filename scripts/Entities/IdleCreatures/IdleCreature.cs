using UnityEngine;

namespace GameCore
{
    public class IdleCreature : Creature
    {
        public float movementTimer;
        public float movementInterval = 5;

        protected override void Update()
        {
            base.Update();

            if (Tools.time >= movementTimer)
            {
                Vector2 velocity = Random.Range(-2, 3) switch
                {
                    -2 => TurnLeft(),
                    > -2 and < 2 => Vector2.zero,
                    2 => TurnRight(),
                    _ => throw new(),
                };
                rb.velocity = velocity == Vector2.zero ? GetMovementVelocity(rb.velocity) : velocity;

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