using DG.Tweening;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;
using static GameCore.UniversalEntityBehaviour;
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
        public EnemyMoveToTarget ai;
        public bool isPursuing;
        public bool isPursuingLastFrame;



        protected override void Awake()
        {
            base.Awake();

            ai = new(this, 0);
        }

        protected override void Update()
        {
            base.Update();

            onGround = RayTools.TryOverlapCircle(mainCollider.DownPoint(), 0.3f, Block.blockLayerMask, out _);

            if (!isDead && targetTransform && isServer)
            {
                TryAttack();
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

            isPursuing = targetTransform;

            if (isPursuing)
            {
                if (!isPursuingLastFrame)
                {
                    OnStartMovementAction();
                    anim.ResetAnimations();

                    anim.SetAnim("run_rightarm");
                    anim.SetAnim("run_leftarm");
                    anim.SetAnim("run_rightleg");
                    anim.SetAnim("run_leftleg");
                    anim.SetAnim("run_head");
                    anim.SetAnim("run_body");
                }

                ai.Pursuit();
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

                ai.Stroll();
            }

            isPursuingLastFrame = isPursuing;
        }
    }
}
