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

        public override Vector2 GetMovementDirection() => Vector2.zero;

        public override void Initialize()
        {
            base.Initialize();


            //添加身体部分
            CreateModel();
            body = AddBodyPart("body", ModFactory.CompareTexture("ori:giant_scorpion_body").sprite, Vector2.zero, 5, model.transform, BodyPartType.Body);
            upperPincher = AddBodyPart("upper_pincher", ModFactory.CompareTexture("ori:giant_scorpion_upper_pincher").sprite, new(0, 0f), 9, body, BodyPartType.RightArm);
            backPincher = AddBodyPart("back_pincher", ModFactory.CompareTexture("ori:giant_scorpion_back_pincher").sprite, new(2.3f, -0.61f), 8, body, BodyPartType.LeftArm);
            upperLegs0 = AddBodyPart("upper_legs_0", ModFactory.CompareTexture("ori:giant_scorpion_upper_legs_0").sprite, new(-1.643f, -0.958f), 7, body, BodyPartType.RightLeg);
            upperLegs1 = AddBodyPart("upper_legs_1", ModFactory.CompareTexture("ori:giant_scorpion_upper_legs_1").sprite, new(-0.516f, -1.214f), 7, body, BodyPartType.LeftLeg);
            backLegs0 = AddBodyPart("back_legs_0", ModFactory.CompareTexture("ori:giant_scorpion_back_legs_0").sprite, new(0.054f, -0.338f), 4, body, BodyPartType.RightFoot);
            backLegs1 = AddBodyPart("back_legs_1", ModFactory.CompareTexture("ori:giant_scorpion_back_legs_1").sprite, new(-0f, -0f), 4, body, BodyPartType.LeftFoot);


            animWeb = new AnimWeb();
            var duration = 0.25f;
            animWeb.AddAnim("walk_0", -1, new AnimFragment[4]
            {
                new LocalRotationZAnimFragment(upperLegs0.transform, -4.5f, duration),
                new LocalRotationZAnimFragment(backLegs0.transform, 4.5f, duration),
                new LocalRotationZAnimFragment(upperLegs0.transform, 4.5f, duration),
                new LocalRotationZAnimFragment(backLegs0.transform, -4.5f, duration)
            }, 0);
            animWeb.AddAnim("walk_1", -1, new AnimFragment[4]
            {
                new LocalRotationZAnimFragment(backLegs1.transform, -4.5f, duration),
                new LocalRotationZAnimFragment(upperLegs1.transform, 4.5f, duration),
                new LocalRotationZAnimFragment(backLegs1.transform, 4.5f, duration),
                new LocalRotationZAnimFragment(upperLegs1.transform,-4.5f, duration)
            }, 0);

            animWeb.GroupAnim(0, "walk", "walk_0", "walk_1");
            animWeb.SwitchPlayingTo("walk");


            jumpCD = 2;
            fallenDamageHeight = float.MaxValue; //免疫摔落伤害
        }
    }
}
