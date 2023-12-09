using DG.Tweening;
using SP.Tools.Unity;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    public class AncientBoxerProperties : CoreEnemyProperties<AncientBoxerProperties>
    {
        public override ushort SearchRadius() => 20;

        public const float attackCD = 2;
    }


    [EntityBinding(EntityID.AncientBoxer)]
    public class AncientBoxer : CoreEnemy<AncientBoxerProperties>
    {
        public DefaultSpriteAnimSequence sequence;





        protected override void Update()
        {
            base.Update();

            onGround = RayTools.TryOverlapCircle(mainCollider.DownPoint(), 0.3f, Block.blockLayerMask, out _);

            if (!isDead && targetTransform && isServer)
            {
                TryAttack();
            }

            BasicEnemyState stateTemp = state;

            if (stateLastFrame != stateTemp)
            {
                //当进入时
                switch (stateTemp)
                {
                    case BasicEnemyState.Idle:
                        {
                            rb.velocity = Vector2.zero;
                            anim.ResetAnimations();
                            anim.SetAnim("idle");

                            break;
                        }

                    case BasicEnemyState.Movement:
                        {
                            anim.ResetAnimations();

                            anim.SetAnim("run");

                            break;
                        }
                }

                //当离开时
                switch (stateLastFrame)
                {
                    case BasicEnemyState.Idle:
                        {

                            break;
                        }

                    case BasicEnemyState.Movement:
                        {

                            break;
                        }
                }
            }

            //当停留时
            switch (stateTemp)
            {
                case BasicEnemyState.Idle:
                    {
                        MoveWithoutTarget();

                        break;
                    }

                case BasicEnemyState.Movement:
                    {
                        if (Tools.Prob100(35f * Tools.deltaTime))
                        {
                            GAudio.Play(AudioID.AncientBoxerAttack, true);
                        }

                        MoveWithTarget();

                        break;
                    }
            }

            stateLastFrame = stateTemp;
        }

        void MoveWithTarget()
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

            /* --------------------------------- 决定是否跳跃 --------------------------------- */
            if (onGround)
            {
                //如果玩家所处高度比自己高
                if (targetTransform.position.y - transform.position.y > 2)
                {
                    Jump();
                    goto set;
                }

                //如果玩家所处高度比自己高
                Vector2 stemCenter = tL ? mainCollider.LeftPoint() : mainCollider.RightPoint();
                Vector2 stemSize = new Vector2(1, mainCollider.size.y);
                float angle = tL ? 180 : 0;

                //检测阻挡
                if (RayTools.TryOverlapBox(stemCenter, stemSize, angle, Block.blockLayerMask, out _))
                {
                    Jump();
                    goto set;
                }

                Vector2 airCenter = tL ? mainCollider.LeftDownPoint() + new Vector2(0.6f, -0.5f) : mainCollider.RightDownPoint() + new Vector2(0.6f, -0.5f);
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
                yVelo = GetJumpVelocity(20);
            }



        /* ---------------------------------- 应用速度 ---------------------------------- */
        set:
            //设置 RB 的速度
            if (tL)
                transform.SetScaleXNegativeAbs();
            else
                transform.SetScaleXAbs();

            rb.velocity = GetMovementVelocity(new(xVelo, yVelo));
        }

        IEnumerator IEWaitAndSetVelocity(float time)
        {
            yield return new WaitForSeconds(time);

            rb.SetVelocity(Vector2.zero);
        }

        void MoveWithoutTarget()
        {
            if (!isServer)
                return;

            //12.5 为倍数, 每秒有 (moveRandomize / deltaTime)% 的几率触发移动
            float moveRandomize = Tools.deltaTime * 2f;

            if (Tools.Prob100(moveRandomize, Tools.staticRandom))
            {
                // -1 to 1
                float horizontal = Random.Range(-1, 2) * 1.75f;
                float vertical = rb.velocity.y;

                rb.SetVelocity(horizontal, vertical);
                StartCoroutine(IEWaitAndSetVelocity(1));
                GAudio.Play(AudioID.AncientBoxerSpare, true);
            }
        }

        protected override void Start()
        {
            base.Start();

            /* -------------------------------------------------------------------------- */
            /*                                     贴图                                     */
            /* -------------------------------------------------------------------------- */
            var sr = AddSpriteRenderer("ori:ancient_boxer.idle_0");



            /* -------------------------------------------------------------------------- */
            /*                                     动画                                     */
            /* -------------------------------------------------------------------------- */
            sequence = new();
            anim.sequences.Add(sequence);
            attackAnimations = new[] { "attack" };

            /* ----------------------------------- 战立 ----------------------------------- */
            anim.AddAnim("idle", () =>
            {
                anim.ResetAnimations("idle");
            }, () => new Tween[]
            {
                AnimCenter.PlaySprites(1.5f, new[]
                {
                    ModFactory.CompareTexture("ori:ancient_boxer.idle_0").sprite,
                    ModFactory.CompareTexture("ori:ancient_boxer.idle_1").sprite
                }, value => { if (sr) sr.sprite = value; return sr; }, 1)
            }, () => sequence.sequence);

            /* ----------------------------------- 跑步 ----------------------------------- */
            anim.AddAnim("run", () =>
            {
                anim.ResetAnimations("run");
            }, () => new Tween[]
            {
                AnimCenter.PlaySprites(0.7f, new[]
                {
                    ModFactory.CompareTexture("ori:ancient_boxer.run_0").sprite,
                    ModFactory.CompareTexture("ori:ancient_boxer.run_1").sprite,
                    ModFactory.CompareTexture("ori:ancient_boxer.run_2").sprite,
                    ModFactory.CompareTexture("ori:ancient_boxer.run_3").sprite,
                }, value => { if (sr) sr.sprite = value; return sr; }, 1)
            }, () => sequence.sequence);

            /* ----------------------------------- 攻击 ----------------------------------- */
            anim.AddAnim("attack", () =>
            {
                anim.ResetAnimations("attack");

                sequence.sequence.OnStepComplete(() =>
                {
                    anim.SetAnim("attack", false);

                    if (isMoving)
                    {
                        //重播放移动动画
                        OnStartMovement();
                    }
                    else
                    {
                        //重播放待机动画
                        OnStopMovement();
                    }
                });
            }, () => new Tween[]
            {
                AnimCenter.PlaySprites(0.4f, new[]
                {
                    ModFactory.CompareTexture("ori:ancient_boxer.attack_0").sprite,
                    ModFactory.CompareTexture("ori:ancient_boxer.attack_1").sprite,
                    ModFactory.CompareTexture("ori:ancient_boxer.attack_2").sprite,
                    ModFactory.CompareTexture("ori:ancient_boxer.attack_3").sprite,
                }, value => { if (sr) sr.sprite = value; return sr; }, 1)
            }, () => sequence.sequence);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            sequence?.sequence?.Kill();
        }

        public override void Movement()
        {
            base.Movement();

            if (!isServer || isDead)
                return;

            //如果目标超出范围
            CheckEnemyTarget();

            if (!targetTransform)
            {
                state = BasicEnemyState.Idle;
            }
            else
            {
                state = BasicEnemyState.Movement;
            }
        }
    }
}
