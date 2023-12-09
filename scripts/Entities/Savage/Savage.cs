using System.Collections;
using SP.Tools.Unity;
using UnityEngine;

namespace GameCore
{
    public class SavageProperties : CoreEnemyProperties<SavageProperties>
    {
        public override ushort SearchRadius() => 35;
        public override float NormalAttackDamage() => 12;

        public const float attackCD = 2;
    }
    [EntityBinding(EntityID.Savage)]
    public class Savage : CoreEnemy<SavageProperties>
    {
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
                        if (Tools.Prob100(35f * Tools.deltaTime))
                        {
                            GAudio.Play(AudioID.ZombieAttack, true);
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

            /* ----------------------------------- 跳跃 ----------------------------------- */
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
                yVelo = GetJumpVelocity(45);
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
            float moveRandomize = Tools.deltaTime * 2f;

            if (Tools.Prob100(moveRandomize, Tools.staticRandom))
            {
                // -1 to 1
                float horizontal = Random.Range(-1, 2) * 1.75f;
                float vertical = rb.velocity.y;

                rb.SetVelocity(horizontal, vertical);
                StartCoroutine(IEWaitAndSetVelo(1));
                GAudio.Play(AudioID.ZombieSpare, true);
            }
        }

        protected override void Start()
        {
            base.Start();

            MethodAgent.TryRun(() =>
            {
                //添加身体部分
                body = AddBodyPart("body", ModFactory.CompareTexture("ori:zombie_body").sprite, Vector2.zero, 5, model.transform, BodyPartType.Body);
                head = AddBodyPart("head", ModFactory.CompareTexture("ori:zombie_head").sprite, new(-0.02f, -0.03f), 10, body, BodyPartType.Head, new(-0.03f, -0.04f));
                rightArm = AddBodyPart("rightArm", ModFactory.CompareTexture("ori:zombie_right_arm").sprite, new(0, 0.03f), 8, body, BodyPartType.RightArm);
                leftArm = AddBodyPart("leftArm", ModFactory.CompareTexture("ori:zombie_left_arm").sprite, new(0, 0.03f), 3, body, BodyPartType.LeftArm);
                rightLeg = AddBodyPart("rightLeg", ModFactory.CompareTexture("ori:zombie_right_leg").sprite, new(0.02f, 0.04f), 3, body, BodyPartType.RightLeg);
                leftLeg = AddBodyPart("leftLeg", ModFactory.CompareTexture("ori:zombie_left_leg").sprite, new(-0.02f, 0.04f), 1, body, BodyPartType.LeftLeg);
                rightFoot = AddBodyPart("rightFoot", ModFactory.CompareTexture("ori:zombie_right_foot").sprite, Vector2.zero, 3, rightLeg, BodyPartType.RightFoot);
                leftFoot = AddBodyPart("leftFoot", ModFactory.CompareTexture("ori:zombie_left_foot").sprite, Vector2.zero, 1, leftLeg, BodyPartType.LeftFoot);
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