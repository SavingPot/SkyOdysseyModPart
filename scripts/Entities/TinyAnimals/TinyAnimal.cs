using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    public abstract class TinyAnimal : Entity, IInteractableEntity
    {
        public abstract string Texture();
        public float movementTimer;
        public float movementInterval = 5;
        public virtual Vector2 interactionSize { get; } = new(2, 2f);

        public override void Initialize()
        {
            base.Initialize();

            /* ---------------------------------- 设置贴图 ---------------------------------- */
            AddSpriteRenderer(Texture());
        }

        public virtual bool PlayerInteraction(Player caller)
        {
            //获取物品并检查
            var itemData = ModFactory.CompareItem(data.id);
            if (itemData == null)
            {
                Debug.LogError("Item not found");
                return false;
            }

            //给予玩家物品
            caller.ServerAddItem(itemData.DataToItem());

            //死亡
            Death();

            return true;
        }

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
    }
}