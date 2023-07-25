using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.Snail)]
    public class Snail : Entity
    {
        public float snailSpeed = 0.1f;
        public float changeDirectionTimer;
        public bool orientation = false;

        protected override void Start()
        {
            base.Start();

            SetOrientation(orientation);
        }
        protected override void Update()
        {
            base.Update();

            if (Tools.time >= changeDirectionTimer)
            {
                if (orientation)
                {
                    Vector2 velocity = new(snailSpeed, 0);
                    rb.velocity = velocity;
                }
                else
                {
                    Vector2 velocity = new(-snailSpeed, 0);
                    rb.velocity = velocity;
                }

                SetOrientation(orientation);
                orientation = !orientation;

                changeDirectionTimer = Tools.time + Random.Range(5, 11);
            }
        }
    }
}