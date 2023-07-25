using UnityEngine;

namespace GameCore
{
    public class IdleCreature : Creature
    {
        public float moveTimer;
        public float intervalTime = 5;

        protected override void Update()
        {
            base.Update();

            if (Tools.time >= moveTimer)
            {
                Vector2 velocity = Random.Range(-2, 3) switch
                {
                    -2 => TurnLeft(),
                    > -2 and < 2 => GetMovementVelocity(Vector2.zero),
                    2 => TurnRight(),
                    _ => throw new(),
                };

                rb.velocity = velocity;
                moveTimer = Tools.time + intervalTime;
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