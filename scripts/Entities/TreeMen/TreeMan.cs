using DG.Tweening;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace GameCore
{
    public class TreeMan: Enemy, IEnemyWalkToTarget
    {
        public string BodyTexture;
        public string HeadTexture;
        public string RightArmTexture;
        public string LeftArmTexture;
        public string RightLegTexture;
        public string LeftLegTexture;

        public bool isPursuing { get; set; }
        public bool isPursuingLastFrame { get; set; }
        public float jumpForce => 0;








        public override void Initialize()
        {
            base.Initialize();

            MethodAgent.DebugRun(() =>
            {
                //添加身体部分
                CreateModel();
                body = AddBodyPart("body", ModFactory.CompareTexture(BodyTexture).sprite, Vector2.zero, 3, model.transform, BodyPartType.Body);
                head = AddBodyPart("head", ModFactory.CompareTexture(HeadTexture).sprite, new Vector2(0, -0.15f), 6, body, BodyPartType.Head);
                rightArm = AddBodyPart("rightarm", ModFactory.CompareTexture(RightArmTexture).sprite, new Vector2(-0.03f, 0), 4, body, BodyPartType.RightArm);
                leftArm = AddBodyPart("leftarm", ModFactory.CompareTexture(LeftArmTexture).sprite, new Vector2(0.03f, 0), 2, body, BodyPartType.LeftArm);
                rightLeg = AddBodyPart("rightleg", ModFactory.CompareTexture(RightLegTexture).sprite, Vector2.zero, 2, body, BodyPartType.RightLeg);
                leftLeg = AddBodyPart("leftleg", ModFactory.CompareTexture(LeftLegTexture).sprite, Vector2.zero, 1, body, BodyPartType.LeftLeg);
            });


            Creature.BindHumanAnimations(this);
        }

        public override Vector2 GetMovementDirection()
        {
            if (isDead)
                return Vector2.zero;

            return EnemyWalkToTargetBehaviour.GetMovementVelocity(this);
        }

        public void WhenStroll()
        {
            GAudio.Play(AudioID.ZombieSpare, true);
        }
    }
}
