using SP.Tools.Unity;
using UnityEngine;

namespace GameCore
{
    public static class EnemyJumpToTargetBehaviour
    {
        public static Vector2 GetMovementVelocity<T>(T enemy) where T : Enemy, IEnemyJumpToTarget
        {
            Vector2 result;
            enemy.isPursuing = enemy.targetEntity;

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

                result = Vector2.zero;
            }

            enemy.isPursuingLastFrame = enemy.isPursuing;
            return result;
        }

        public static Vector2 Pursuit<T>(T enemy) where T : Enemy, IEnemyJumpToTarget
        {
            /* --------------------------------- 声明移动速度 --------------------------------- */
            float xVelo = 0;
            float yVelo = 0;

            /* ----------------------------------- 跳跃 ----------------------------------- */
            if (Tools.time >= enemy.jumpTimer)
            {
                if (enemy.isOnGround)
                {
                    var enemyPosition = enemy.transform.position;
                    var targetPosition = enemy.targetEntity.transform.position;


                    bool isTargetLeft = targetPosition.x < enemyPosition.x;
                    float errorValue = 0.1f;

                    // 目标右向右
                    // 靠的很近就设为 0, 否则会鬼畜
                    xVelo = !isTargetLeft ? (targetPosition.x - enemyPosition.x < errorValue ? 0 : 13) : (targetPosition.x - enemyPosition.x > -errorValue ? 0 : -13);

                    yVelo = enemy.GetJumpVelocity(60);


                    //起跳的时候改变面朝的方向
                    if (isTargetLeft)
                        enemy.transform.SetScaleXNegativeAbs();
                    else
                        enemy.transform.SetScaleXAbs();
                }
            }


            /* ---------------------------------- 应用速度 ---------------------------------- */

            //设置 RB 的速度
            return new(xVelo, yVelo);
        }
    }

    public interface IEnemyJumpToTarget : IEnemyMovement
    {
        float jumpForce { get; }
    }
}