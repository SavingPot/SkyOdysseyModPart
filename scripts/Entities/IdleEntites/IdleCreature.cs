using UnityEngine;

namespace GameCore
{
    public class IdleCreature : Creature
    {
        public float moveTimer;
        public bool orientation = false;
        public float intervalTime;

        protected override void Start()
        {
            base.Start();

            SetOrientation(orientation);
        }
        protected override async void Update()
        {
            base.Update();

            if (Tools.time >= moveTimer)
            {
                Vector2 velocity;

                switch (Random.Range(-1, 2))
                {
                    case -1:
                        velocity = GetMovementVelocity(Vector2.left);
                        SetOrientation(false);
                        break;
                    
                    case 0:
                        velocity = GetMovementVelocity(Vector2.zero);
                        break;
                    
                    case 1:
                        velocity = GetMovementVelocity(Vector2.right);
                        SetOrientation(true);
                        break;

                    default:
                        throw new();
                }

                rb.velocity = velocity;

                moveTimer = Tools.time + intervalTime;
            }
        }
    }
}