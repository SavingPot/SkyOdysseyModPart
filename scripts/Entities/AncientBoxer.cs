using DG.Tweening;
using SP.Tools.Unity;
using System.Collections;
using UnityEngine;
using static GameCore.UniversalEntityBehaviour;
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
        public EnemyMoveToTarget ai;



        protected override void Awake()
        {
            base.Awake();

            ai = new(this, 20);
        }

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

                        ai.MoveWithTarget();

                        break;
                    }
            }

            stateLastFrame = stateTemp;
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
