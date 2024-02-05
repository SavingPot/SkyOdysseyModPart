using DG.Tweening;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.Zombie)]
    public class Zombie : Enemy, IEnemyWalkToTarget
    {
        public bool isPursuing { get; set; }
        public bool isPursuingLastFrame { get; set; }
        public float jumpForce => 46;






        public override void Initialize()
        {
            base.Initialize();

            MethodAgent.DebugRun(() =>
            {
                //添加身体部分
                CreateModel();
                body = AddBodyPart("body", ModFactory.CompareTexture("ori:zombie_body").sprite, Vector2.zero, 5, model.transform, BodyPartType.Body);
                head = AddBodyPart("head", ModFactory.CompareTexture("ori:zombie_head").sprite, new(-0.02f, -0.03f), 10, body, BodyPartType.Head, new(-0.03f, -0.04f));
                rightArm = AddBodyPart("rightArm", ModFactory.CompareTexture("ori:zombie_right_arm").sprite, new(0, 0.03f), 8, body, BodyPartType.RightArm);
                leftArm = AddBodyPart("leftArm", ModFactory.CompareTexture("ori:zombie_left_arm").sprite, new(0, 0.03f), 3, body, BodyPartType.LeftArm);
                rightLeg = AddBodyPart("rightLeg", ModFactory.CompareTexture("ori:zombie_right_leg").sprite, new(0.02f, 0.04f), 3, body, BodyPartType.RightLeg);
                leftLeg = AddBodyPart("leftLeg", ModFactory.CompareTexture("ori:zombie_left_leg").sprite, new(-0.02f, 0.04f), 1, body, BodyPartType.LeftLeg);
                rightFoot = AddBodyPart("rightFoot", ModFactory.CompareTexture("ori:zombie_right_foot").sprite, Vector2.zero, 3, rightLeg, BodyPartType.RightFoot);
                leftFoot = AddBodyPart("leftFoot", ModFactory.CompareTexture("ori:zombie_left_foot").sprite, Vector2.zero, 1, leftLeg, BodyPartType.LeftFoot);
            });


            Creature.BindHumanAnimations(this);
        }

        public override void Movement()
        {
            base.Movement();

            if (!isServer || isDead)
                return;

            EnemyWalkToTargetBehaviour.OnMovement(this);
        }

        public void WhenStroll()
        {
            GAudio.Play(AudioID.ZombieSpare, true);
        }
    }
}
