using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    public abstract class TinyAnimal : Entity
    {
        public abstract string Texture();
        public float movementTimer;
        public float movementInterval = 5;

        protected override void Update()
        {
            base.Update();

            if (Tools.time >= movementTimer)
            {
                float velocityX = Random.Range(-2, 3) switch
                {
                    -2 => TurnLeft(),
                    > -2 and < 2 => 0,
                    2 => TurnRight(),
                    _ => throw new(),
                };
                rb.velocity = new(velocityX, rb.velocity.y);

                movementTimer = Tools.time + movementInterval;
            }
        }

        protected virtual float TurnLeft()
        {
            SetOrientation(false);

            return -1;
        }

        protected virtual float TurnRight()
        {
            SetOrientation(true);

            return 1;
        }

        public override void Initialize()
        {
            base.Initialize();

            /* ---------------------------------- 设置贴图 ---------------------------------- */
            AddSpriteRenderer(Texture());
        }
    }
}