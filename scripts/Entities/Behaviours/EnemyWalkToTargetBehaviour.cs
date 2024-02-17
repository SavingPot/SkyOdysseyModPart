using System;
using System.Collections;
using SP.Tools.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    public static class EnemyWalkToTargetBehaviour
    {
        public static void OnMovement<T>(T enemy) where T : Enemy, IEnemyWalkToTarget
        {
            enemy.isPursuing = enemy.targetTransform;

            if (enemy.isPursuing)
            {
                if (!enemy.isPursuingLastFrame)
                {
                    enemy.isMoving = true;
                }

                Pursuit(enemy);
            }
            else
            {
                if (enemy.isPursuingLastFrame)
                {
                    enemy.isMoving = false;

                    enemy.rb.velocity = Vector2.zero;
                }

                Stroll(enemy);
            }

            enemy.isPursuingLastFrame = enemy.isPursuing;
        }

        public static void Stroll<T>(T enemy) where T : Enemy, IEnemyWalkToTarget
        {
            //12.5 为倍数, 每秒有 (moveRandomize / deltaTime)% 的几率触发移动
            float moveRandomize = Tools.deltaTime * 2f;

            if (Tools.Prob100(moveRandomize, Tools.staticRandom))
            {
                // -1 to 1
                float horizontal = Random.Range(-1, 2) * 1.75f;
                float vertical = enemy.rb.velocity.y;

                enemy.rb.SetVelocity(horizontal, vertical);
                enemy.StartCoroutine(IEStrollResumeVelocity(enemy, 1));
                enemy.WhenStroll();
            }
        }

        static IEnumerator IEStrollResumeVelocity<T>(T enemy, float time) where T : Enemy, IEnemyWalkToTarget
        {
            yield return new WaitForSeconds(time);

            enemy.rb.SetVelocity(Vector2.zero);
        }

        public static void Pursuit<T>(T enemy) where T : Enemy, IEnemyWalkToTarget
        {
            if(!enemy.targetTransform)
            {
                Debug.LogError("请确保敌人有追击目标");
                return;
            }

            var target = enemy.targetTransform.position;

            /* ---------------------------------- 声明方向 ---------------------------------- */
            bool isTargetLeft = target.x < enemy.transform.position.x;
            float errorValue = 0.1f;

            /* --------------------------------- 声明移动速度 --------------------------------- */
            float yVelo = 0;

            // 目标右向右
            // 靠的很近就设为 0, 否则会鬼畜
            int xVelo = !isTargetLeft ? (target.x - enemy.transform.position.x < errorValue ? 0 : 1) : (target.x - enemy.transform.position.x > -errorValue ? 0 : -1);

            /* --------------------------------- 决定是否跳跃 --------------------------------- */
            if (enemy.jumpForce != 0)
            {
                if (enemy.isOnGround)
                {
                    //如果玩家所处高度比自己高
                    if (target.y - enemy.transform.position.y > 2)
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

            enemy.rb.velocity = enemy.GetMovementVelocity(new(xVelo, yVelo));
        }
    }

    public interface IEnemyWalkToTarget: IEnemyMovement
    {
        float jumpForce { get; }

        void WhenStroll();
    }
}