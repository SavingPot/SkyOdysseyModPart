using DG.Tweening;
using GameCore.High;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    public class Slime : Enemy
    {
        public bool isPursuing;
        public bool isPursuingLastFrame;
        public string Texture;




        void Pursuit()
        {
            if (!isServer || !targetTransform)
                return;

            /* --------------------------------- 声明移动速度 --------------------------------- */
            float xVelo = 0;
            float yVelo = 0;


            /* ----------------------------------- 跳跃 ----------------------------------- */
            if (Tools.time >= jumpTimer)
            {
                bool onGround = RayTools.TryOverlapCircle(mainCollider.DownPoint(), 0.3f, Block.blockLayerMask, out _);

                if (onGround)
                {
                    bool isTargetLeft = targetTransform.position.x < transform.position.x;
                    float errorValue = 0.1f;

                    // 目标右向右
                    // 靠的很近就设为 0, 否则会鬼畜
                    xVelo = !isTargetLeft ? (targetTransform.position.x - transform.position.x < errorValue ? 0 : 5) : (targetTransform.position.x - transform.position.x > -errorValue ? 0 : -5);

                    yVelo = GetJumpVelocity(75);


                    //起跳的时候改变面朝的方向
                    if (isTargetLeft)
                        transform.SetScaleXNegativeAbs();
                    else
                        transform.SetScaleXAbs();
                }
            }


            /* ---------------------------------- 应用速度 ---------------------------------- */

            //设置 RB 的速度
            rb.velocity = GetMovementVelocity(new(xVelo, yVelo));
        }

        protected override void Start()
        {
            base.Start();

            /* ---------------------------------- 设置贴图 ---------------------------------- */
            AddSpriteRenderer(Texture);

            jumpCD = 2;
        }

        public override void Movement()
        {
            base.Movement();

            if (!isServer || isDead)
                return;

            isPursuing = targetTransform;

            if (isPursuing)
            {
                if (!isPursuingLastFrame)
                {
                    ServerOnStartMovement();
                }

                Pursuit();
            }
            else
            {
                if (isPursuingLastFrame)
                {
                    ServerOnStopMovement();

                    rb.velocity = Vector2.zero;
                }
            }

            isPursuingLastFrame = isPursuing;
        }
    }
}
