using SP.Tools.Unity;
using UnityEngine;

namespace GameCore
{
    public class UniversalEntityBehaviour
    {
        public class EnemyMoveToTarget
        {
            public Enemy enemy;
            public float jumpForce;

            public void MoveWithTarget()
            {
                if (!enemy.isServer || !enemy.targetTransform)
                    return;

                var target = enemy.targetTransform.position;

                /* ---------------------------------- 声明方向 ---------------------------------- */
                bool tL = target.x < enemy.transform.position.x;
                float errorValue = 0.1f;

                /* --------------------------------- 声明移动速度 --------------------------------- */
                float yVelo = 0;

                // 目标右向右
                // 靠的很近就设为 0, 否则会鬼畜
                int xVelo = !tL ? (target.x - enemy.transform.position.x < errorValue ? 0 : 1) : (target.x - enemy.transform.position.x > -errorValue ? 0 : -1);

                /* --------------------------------- 决定是否跳跃 --------------------------------- */
                if (jumpForce != 0 && enemy.onGround)
                {
                    //如果玩家所处高度比自己高
                    if (target.y - enemy.transform.position.y > 2)
                    {
                        Jump();
                        goto set;
                    }

                    //如果玩家所处高度比自己高
                    Vector2 stemCenter = tL ? enemy.mainCollider.LeftPoint() : enemy.mainCollider.RightPoint();
                    Vector2 stemSize = new(1, enemy.mainCollider.size.y);
                    float angle = tL ? 180 : 0;

                    //检测阻挡
                    if (RayTools.TryOverlapBox(stemCenter, stemSize, angle, Block.blockLayerMask, out _))
                    {
                        Jump();
                        goto set;
                    }

                    Vector2 airCenter = tL ? enemy.mainCollider.LeftDownPoint() + new Vector2(0.6f, -0.5f) : enemy.mainCollider.RightDownPoint() + new Vector2(0.6f, -0.5f);
                    Vector2 airSize = new(0.5f, 0.5f);

                    //检测无地面
                    if (!RayTools.TryOverlapBox(airCenter, airSize, angle, Block.blockLayerMask, out _))
                    {
                        Jump();
                        goto set;
                    }
                }

                void Jump()
                {
                    yVelo = enemy.GetJumpVelocity(jumpForce);
                }



            /* ---------------------------------- 应用速度 ---------------------------------- */
            set:
                //设置 RB 的速度
                if (tL)
                    enemy.transform.SetScaleXNegativeAbs();
                else
                    enemy.transform.SetScaleXAbs();

                enemy.rb.velocity = enemy.GetMovementVelocity(new(xVelo, yVelo));
            }

            public EnemyMoveToTarget(Enemy enemy, float jumpForce)
            {
                this.enemy = enemy;
                this.jumpForce = jumpForce;
            }
        }
    }
}