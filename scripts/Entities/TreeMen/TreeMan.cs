using DG.Tweening;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;
using static GameCore.UniversalEntityBehaviour;
using Random = UnityEngine.Random;

namespace GameCore
{
    public class TreeMan: Enemy
    {
        public EnemyMoveToTarget ai;
        public bool isPursuing;
        public bool isPursuingLastFrame;
        public string BodyTexture;
        public string HeadTexture;
        public string RightArmTexture;
        public string LeftArmTexture;
        public string RightLegTexture;
        public string LeftLegTexture;


        protected override void Awake()
        {
            base.Awake();

            ai = new(this, 35);
        }

        public override void Initialize()
        {
            base.Initialize();

            MethodAgent.TryRun(() =>
            {
                //添加身体部分
                CreateModel();
                body = AddBodyPart("body", ModFactory.CompareTexture(BodyTexture).sprite, Vector2.zero, 3, model.transform, BodyPartType.Body);
                head = AddBodyPart("head", ModFactory.CompareTexture(HeadTexture).sprite, new Vector2(0, -0.15f), 6, body, BodyPartType.Head);
                rightArm = AddBodyPart("rightarm", ModFactory.CompareTexture(RightArmTexture).sprite, new Vector2(-0.03f, 0), 4, body, BodyPartType.RightArm);
                leftArm = AddBodyPart("leftarm", ModFactory.CompareTexture(LeftArmTexture).sprite, new Vector2(0.03f, 0), 2, body, BodyPartType.LeftArm);
                rightLeg = AddBodyPart("rightleg", ModFactory.CompareTexture(RightLegTexture).sprite, Vector2.zero, 2, body, BodyPartType.RightLeg);
                leftLeg = AddBodyPart("leftleg", ModFactory.CompareTexture(LeftLegTexture).sprite, Vector2.zero, 1, body, BodyPartType.LeftLeg);
            }, true);


            Creature.BindHumanAnimations(this);
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

                ai.Pursuit();
            }
            else
            {
                if (isPursuingLastFrame)
                {
                    ServerOnStopMovement();

                    rb.velocity = Vector2.zero;
                }

                ai.Stroll();
            }

            isPursuingLastFrame = isPursuing;
        }
    }
}
