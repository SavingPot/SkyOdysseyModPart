using System;
using System.Collections;
using SP.Tools.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    public static class EnemyWalkToTargetBehaviour
    {
        public static Vector2 GetMovementVelocity<T>(T enemy) where T : Enemy, IEnemyWalkToTarget
        {
            Vector2 result;
            enemy.isPursuing = enemy.targetEntity != null;

            if (enemy.isPursuing)
            {
                //刚开始移动
                if (!enemy.isPursuingLastFrame)
                {
                    enemy.isMoving = true;
                }

                result = Pursuit(enemy);
            }
            else
            {
                //刚停止移动
                if (enemy.isPursuingLastFrame)
                {
                    enemy.isMoving = false;
                }

                result = Stroll(enemy);
            }

            enemy.isPursuingLastFrame = enemy.isPursuing;
            return result;
        }

        public static Vector2 Stroll<T>(T enemy) where T : Enemy, IEnemyWalkToTarget
        {
            return Vector2.zero;
        }

        public static Vector2 Pursuit<T>(T enemy) where T : Enemy, IEnemyWalkToTarget
        {
            if (!enemy.targetEntity)
            {
                Debug.LogError("请确保敌人有追击目标");
                return Vector2.zero;
            }

            var targetPosition = enemy.targetEntity.transform.position;

            /* ---------------------------------- 声明方向 ---------------------------------- */
            bool isTargetLeft = targetPosition.x < enemy.transform.position.x;
            float errorValue = 0.1f;

            /* --------------------------------- 声明移动速度 --------------------------------- */
            float yVelo = 0;

            // 目标右向右
            // 靠的很近就设为 0, 否则会鬼畜
            int xVelo = !isTargetLeft ? (targetPosition.x - enemy.transform.position.x < errorValue ? 0 : 1) : (targetPosition.x - enemy.transform.position.x > -errorValue ? 0 : -1);

            /* --------------------------------- 决定是否跳跃 --------------------------------- */
            if (enemy.jumpForce != 0)
            {
                if (enemy.isOnGround)
                {
                    //如果玩家所处高度比自己高
                    if (targetPosition.y - enemy.transform.position.y > 2)
                    {
                        Jump();
                        goto set;
                    }

                    //如果玩家所处高度比自己高
                    Vector2 stemCenter = isTargetLeft ? enemy.mainCollider.LeftPoint() : enemy.mainCollider.RightPoint();
                    Vector2 stemSize = new(1, enemy.mainCollider.size.y);
                    float angle = isTargetLeft ? 180 : 0;

                    //检测阻挡
                    if (RayTools.TryOverlapBox(stemCenter, stemSize, angle, Block.blockLayerMask, out _))
                    {
                        Jump();
                        goto set;
                    }

                    Vector2 airCenter = isTargetLeft ? enemy.mainCollider.LeftDownPoint() + new Vector2(0.6f, -0.5f) : enemy.mainCollider.RightDownPoint() + new Vector2(0.6f, -0.5f);
                    Vector2 airSize = new(0.5f, 0.5f);

                    //检测无地面
                    if (!RayTools.TryOverlapBox(airCenter, airSize, angle, Block.blockLayerMask, out _))
                    {
                        Jump();
                        goto set;
                    }
                }
            }

            void Jump()
            {
                yVelo = enemy.GetJumpVelocity(enemy.jumpForce);
            }



        /* ---------------------------------- 应用速度 ---------------------------------- */
        set:
            //设置 RB 的速度
            if (isTargetLeft)
                enemy.transform.SetScaleXNegativeAbs();
            else
                enemy.transform.SetScaleXAbs();

            return new(xVelo, yVelo);
        }
    }

    public interface IEnemyWalkToTarget : IEnemyMovement
    {
        float jumpForce { get; }

        void WhenStroll();
    }
}