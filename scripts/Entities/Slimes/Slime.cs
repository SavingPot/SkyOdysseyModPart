using DG.Tweening;
using GameCore.High;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    public abstract class SlimeProperties<T> : CoreEnemyProperties<T> where T : SlimeProperties<T>, new()
    {
        public abstract string Texture();
        public override float AttackRadius() => 1f;
        public override float NormalAttackDamage() => 10;
    }


    public class Slime<PropertyT> : CoreEnemy<PropertyT>
        where PropertyT : SlimeProperties<PropertyT>, new()
    {
        public bool isPursuing;
        public bool isPursuingLastFrame;


        
        protected override void Update()
        {
            base.Update();

            onGround = RayTools.TryOverlapCircle(mainCollider.DownPoint(), 0.3f, Block.blockLayerMask, out _);

            if (!isDead && targetTransform && isServer)
            {
                TryAttack();
            }
        }

        void Pursuit()
        {
            if (!isServer || !targetTransform)
                return;

            /* ---------------------------------- 声明方向 ---------------------------------- */
            bool tL = targetTransform.position.x < transform.position.x;
            float errorValue = 0.1f;

            /* --------------------------------- 声明移动速度 --------------------------------- */
            float yVelo = 0;

            // 目标右向右
            // 靠的很近就设为 0, 否则会鬼畜
            int xVelo = !tL ? (targetTransform.position.x - transform.position.x < errorValue ? 0 : 1) : (targetTransform.position.x - transform.position.x > -errorValue ? 0 : -1);

            /* ----------------------------------- 跳跃 ----------------------------------- */
            if (onGround)
            {
                //TODO: 在地上的时间超过一秒后再跳, 并且在跳跃的时候才设置x轴速度
                yVelo = GetJumpVelocity(50);
            }


            /* ---------------------------------- 应用速度 ---------------------------------- */

            //设置 RB 的速度
            if (tL)
                transform.SetScaleXNegativeAbs();
            else
                transform.SetScaleXAbs();

            rb.velocity = GetMovementVelocity(new(xVelo, yVelo));
        }

        protected override void Start()
        {
            base.Start();

            /* ---------------------------------- 设置贴图 ---------------------------------- */
            AddSpriteRenderer(SlimeProperties<PropertyT>.instance.Texture());
        }

        public override void Movement()
        {
            base.Movement();

            if (!isServer || isDead)
                return;

            //如果目标超出范围
            CheckEnemyTarget();

            isPursuing = targetTransform;

            if (isPursuing)
            {
                if (!isPursuingLastFrame)
                {
                    OnStartMovementAction();
                }

                Pursuit();
            }
            else
            {
                if (isPursuingLastFrame)
                {
                    OnStopMovementAction();

                    rb.velocity = Vector2.zero;
                    anim.ResetAnimations();
                    anim.SetAnim("idle_head");
                }
            }

            isPursuingLastFrame = isPursuing;
        }
    }
}
