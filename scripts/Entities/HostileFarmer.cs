using DG.Tweening;
using GameCore.High;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;
using static GameCore.UniversalEntityBehaviour;
using Random = UnityEngine.Random;

namespace GameCore
{
    public class HostileFarmerProperties : CoreEnemyProperties<HostileFarmerProperties>
    {
        public override ushort SearchRadius() => 35;
        public override float NormalAttackDamage() => 12;

        public const float attackCD = 2;
    }


    [EntityBinding(EntityID.HostileFarmer)]
    public class HostileFarmer : CoreEnemy<HostileFarmerProperties>, IInventoryOwner
    {
        public Inventory inventory;
        public SpriteRenderer rightHandItem { get; set; }
        public SpriteRenderer leftHandItem { get; set; }
        public EnemyMoveToTarget ai;




        protected override void Awake()
        {
            base.Awake();

            ai = new(this, 30);
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
                        ai.Stroll();

                        break;
                    }

                case BasicEnemyState.Movement:
                    {
                        ai.Pursuit();

                        break;
                    }
            }

            stateLastFrame = stateTemp;
        }

        protected override void Start()
        {
            base.Start();

            MethodAgent.TryRun(() =>
            {
                //添加身体部分
                body = AddBodyPart("body", ModFactory.CompareTexture("ori:hostile_farmer_body").sprite, Vector2.zero, 5, model.transform, BodyPartType.Body);
                head = AddBodyPart("head", ModFactory.CompareTexture("ori:hostile_farmer_head").sprite, new(-0.02f, -0.03f), 10, body, BodyPartType.Head, new(-0.03f, -0.04f));
                rightArm = AddBodyPart("rightArm", ModFactory.CompareTexture("ori:hostile_farmer_right_arm").sprite, new(0, 0.03f), 8, body, BodyPartType.RightArm);
                leftArm = AddBodyPart("leftArm", ModFactory.CompareTexture("ori:hostile_farmer_left_arm").sprite, new(0, 0.03f), 3, body, BodyPartType.LeftArm);
                rightLeg = AddBodyPart("rightLeg", ModFactory.CompareTexture("ori:hostile_farmer_right_leg").sprite, new(0.02f, 0.04f), 3, body, BodyPartType.RightLeg);
                leftLeg = AddBodyPart("leftLeg", ModFactory.CompareTexture("ori:hostile_farmer_left_leg").sprite, new(-0.02f, 0.04f), 1, body, BodyPartType.LeftLeg);
                rightFoot = AddBodyPart("rightFoot", ModFactory.CompareTexture("ori:hostile_farmer_right_foot").sprite, Vector2.zero, 3, rightLeg, BodyPartType.RightFoot);
                leftFoot = AddBodyPart("leftFoot", ModFactory.CompareTexture("ori:hostile_farmer_left_foot").sprite, Vector2.zero, 1, leftLeg, BodyPartType.LeftFoot);

                //添加双手物品的显示
                leftHandItem = ObjectTools.CreateSpriteObject(leftArm.transform, "item");
                rightHandItem = ObjectTools.CreateSpriteObject(rightArm.transform, "item");

                leftHandItem.sortingOrder = 7;
                rightHandItem.sortingOrder = 9;

                leftHandItem.transform.localPosition = new(0.1f, -0.5f);
                rightHandItem.transform.localPosition = new(0.1f, -0.5f);
                leftHandItem.transform.SetScale(0.6f, 0.6f);
                rightHandItem.transform.SetScale(0.6f, 0.6f);
            }, true);


            Creature.BindHumanAnimations(this);


            this.LoadInventoryFromCustomData(1);

            //没有物品就设置
            if (Item.Null(inventory.GetItem(0)))
            {
                Item item = Random.Range(0, 5) switch
                {
                    1 => ModFactory.CompareItem("ori:straw_hat").ToExtended(),
                    2 => ModFactory.CompareItem("ori:straw_hat").ToExtended(),
                    3 => ModFactory.CompareItem("ori:straw_hat").ToExtended(),
                    4 => ModFactory.CompareItem("ori:straw_hat").ToExtended(),
                    _ => ModFactory.CompareItem("ori:flint_hoe").ToExtended()
                };

                inventory.SetItem(0, item);
            }

            OnInventoryItemChange("0");
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

        public void OnInventoryItemChange(string index)
        {
            Item item = inventory.GetItem(index);

            rightHandItem.sprite = item.data.texture.sprite;
        }

        public Inventory GetInventory() => inventory;

        public void SetInventory(Inventory value) => inventory = value;
    }
}
