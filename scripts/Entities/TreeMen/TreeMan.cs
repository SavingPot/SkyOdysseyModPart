using DG.Tweening;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    public abstract class TreeManProperties<T> : CoreEnemyProperties<T> where T : TreeManProperties<T>, new()
    {
        public override ushort SearchRadius() => 25;
        public override float AttackRadius() => 2.75f;

        public abstract string BodyTexture();
        public abstract string HeadTexture();
        public abstract string RightArmTexture();
        public abstract string LeftArmTexture();
        public abstract string RightLegTexture();
        public abstract string LeftLegTexture();
    }


    public class TreeMan<PropertyT> : CoreEnemy<PropertyT>
        where PropertyT : TreeManProperties<PropertyT>, new()
    {
        protected override void Update()
        {
            base.Update();

            onGround = RayTools.TryOverlapCircle(mainCollider.DownPoint(), 0.3f, Block.blockLayerMask, out _);

            if (correctedSyncVars)
            {
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
                                anim.SetAnim("idle_head");

                                break;
                            }

                        case BasicEnemyState.Movement:
                            {
                                OnStartMovementAction();
                                anim.ResetAnimations();

                                anim.SetAnim("run_rightarm");
                                anim.SetAnim("run_leftarm");
                                anim.SetAnim("run_rightleg");
                                anim.SetAnim("run_leftleg");
                                anim.SetAnim("run_head");
                                anim.SetAnim("run_body");

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
                                OnStopMovementAction();

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
                            MoveWithTarget();

                            break;
                        }
                }

                stateLastFrame = stateTemp;
            }
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


            /* ---------------------------------- 应用速度 ---------------------------------- */
            //设置 RB 的速度
            if (tL)
                transform.SetScaleXNegativeAbs();
            else
                transform.SetScaleXAbs();

            rb.velocity = GetMovementVelocity(new(xVelo, yVelo));
        }

        IEnumerator IEWaitAndSetVelo(float time)
        {
            yield return new WaitForSeconds(time);

            rb.SetVelocity(Vector2.zero);
        }

        void MoveWithoutTarget()
        {
            if (!isServer)
                return;

            //12.5 为倍数, 每秒有 (moveRandomize / deltaTime)% 的几率触发移动
            float moveRandomize = Tools.deltaTime * 1f;

            if (Tools.Prob100(moveRandomize, Tools.staticRandom))
            {
                // -1 to 1
                float horizontal = Random.Range(-1, 2) * 1.75f;
                float vertical = rb.velocity.y;

                rb.SetVelocity(horizontal, vertical);
                StartCoroutine(IEWaitAndSetVelo(1));
            }
        }

        protected override void Start()
        {
            base.Start();

            MethodAgent.TryRun(() =>
            {
                //添加身体部分
                body = AddBodyPart("body", ModFactory.CompareTexture(TreeManProperties<PropertyT>.instance.BodyTexture()).sprite, Vector2.zero, 3, model.transform, BodyPartType.Body);
                head = AddBodyPart("head", ModFactory.CompareTexture(TreeManProperties<PropertyT>.instance.HeadTexture()).sprite, new Vector2(0, -0.15f), 6, body, BodyPartType.Head);
                rightArm = AddBodyPart("rightarm", ModFactory.CompareTexture(TreeManProperties<PropertyT>.instance.RightArmTexture()).sprite, new Vector2(-0.03f, 0), 4, body, BodyPartType.RightArm);
                leftArm = AddBodyPart("leftarm", ModFactory.CompareTexture(TreeManProperties<PropertyT>.instance.LeftArmTexture()).sprite, new Vector2(0.03f, 0), 2, body, BodyPartType.LeftArm);
                rightLeg = AddBodyPart("rightleg", ModFactory.CompareTexture(TreeManProperties<PropertyT>.instance.RightLegTexture()).sprite, Vector2.zero, 2, body, BodyPartType.RightLeg);
                leftLeg = AddBodyPart("leftleg", ModFactory.CompareTexture(TreeManProperties<PropertyT>.instance.LeftLegTexture()).sprite, Vector2.zero, 1, body, BodyPartType.LeftLeg);
            }, true);


            Creature.BindHumanAnimations(this);
        }

        public override void Movement()
        {
            base.Movement();

            if (!isServer || isDead)
                return;

            //如果目标超出范围
            CheckEnemyTarget();

            if (correctedSyncVars)
            {
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
}
