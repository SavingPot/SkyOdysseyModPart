using DG.Tweening;
using GameCore.High;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    [EntityBinding(EntityID.GiantScorpion), NotSummonable]
    public class GiantScorpion : Enemy
    {
        CreatureBodyPart upperPincher;
        CreatureBodyPart backPincher;
        CreatureBodyPart upperLegs0;
        CreatureBodyPart upperLegs1;
        CreatureBodyPart backLegs0;
        CreatureBodyPart backLegs1;
        Vector2 localPosOfTail;
        LineRenderer lineRenderer;




        public override Vector2 GetMovementDirection() => Vector2.zero;

        public override void Initialize()
        {
            base.Initialize();



            #region 躯体动画

            //添加身体部分
            CreateModel();
            body = AddBodyPart("body", ModFactory.CompareTexture("ori:giant_scorpion_body").sprite, Vector2.zero, 6, model.transform);
            upperPincher = AddBodyPart("upper_pincher", ModFactory.CompareTexture("ori:giant_scorpion_upper_pincher").sprite, new(2.84f, -1.93f), 7, body);
            backPincher = AddBodyPart("back_pincher", ModFactory.CompareTexture("ori:giant_scorpion_back_pincher").sprite, new(2.85f, -1.08f), 5, body);
            upperLegs0 = AddBodyPart("upper_legs_0", ModFactory.CompareTexture("ori:giant_scorpion_upper_legs_0").sprite, new(-0.721f, -1.686f), 7, body);
            upperLegs1 = AddBodyPart("upper_legs_1", ModFactory.CompareTexture("ori:giant_scorpion_upper_legs_1").sprite, new(0.364f, -1.906f), 7, body);
            backLegs0 = AddBodyPart("back_legs_0", ModFactory.CompareTexture("ori:giant_scorpion_back_legs_0").sprite, new(-0.196f, -0.925f), 4, body);
            backLegs1 = AddBodyPart("back_legs_1", ModFactory.CompareTexture("ori:giant_scorpion_back_legs_1").sprite, new(0.579f, -1.065f), 4, body);


            float attackAnimFirstTime = 0.15f;
            float attackAnimSecondTime = 2f;
            animWeb = new AnimWeb();

            {
                var duration = 0.1f;
                animWeb.AddAnim("walk_0", -1, new AnimFragment[]
                {
                    new LocalRotationZAnimFragment(upperLegs0.transform, -4.5f, duration, Ease.Linear),
                    new LocalRotationZAnimFragment(backLegs0.transform, 4.5f, duration , Ease.Linear),
                    new LocalRotationZAnimFragment(upperLegs0.transform, 4.5f, duration, Ease.Linear),
                    new LocalRotationZAnimFragment(backLegs0.transform, -4.5f, duration, Ease.Linear)
                }, 0);
                animWeb.AddAnim("walk_1", -1, new AnimFragment[]
                {
                    new LocalRotationZAnimFragment(backLegs1.transform, -4.5f, duration, Ease.Linear),
                    new LocalRotationZAnimFragment(upperLegs1.transform, 4.5f, duration, Ease.Linear),
                    new LocalRotationZAnimFragment(backLegs1.transform, 4.5f, duration , Ease.Linear),
                    new LocalRotationZAnimFragment(upperLegs1.transform,-4.5f, duration, Ease.Linear)
                }, 0);
            }
            {
                animWeb.AddAnim("attack_upper", -1, new AnimFragment[]
                {
                    new LocalRotationZAnimFragment(upperPincher.transform, 14f, attackAnimFirstTime, Ease.Linear),
                    new LocalRotationZAnimFragment(upperPincher.transform, 0f, attackAnimSecondTime, Ease.Linear),
                }, 0);
                animWeb.AddAnim("attack_back", -1, new AnimFragment[]
                {
                    new LocalRotationZAnimFragment(backPincher.transform, -14f, attackAnimFirstTime, Ease.Linear),
                    new LocalRotationZAnimFragment(backPincher.transform, 0f, attackAnimSecondTime, Ease.Linear),
                }, 0);
            }


            attackAnimations = new[] { "attack" };


            animWeb.GroupAnim(0, "walk", "walk_0", "walk_1");
            animWeb.GroupAnim(0, "attack", "attack_upper", "attack_back");
            animWeb.CreateConnectionFromTo("attack", "walk", () => true, attackAnimFirstTime + attackAnimSecondTime, 0);

            animWeb.SwitchPlayingTo("walk");

            #endregion



            //初始化 LineRenderer
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = GInit.instance.spriteDefaultMat;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;


            jumpCD = 2;
            fallenDamageHeight = float.MaxValue; //免疫摔落伤害
            localPosOfTail = new(body.transform.localPosition.x - 7 / body.sr.sprite.pixelsPerUnit, body.transform.localPosition.y + 17 / body.sr.sprite.pixelsPerUnit);
        }

        protected override void Update()
        {
            base.Update();

            if (targetEntity)
            {
                //面向玩家
                SetOrientation(targetEntity.transform.position.x > transform.position.x);

                var worldPosOfTail = body.transform.TransformPoint(localPosOfTail);
                lineRenderer.SetPosition(0, worldPosOfTail);
                lineRenderer.SetPosition(1, targetEntity.transform.position);
            }
        }
    }
}
