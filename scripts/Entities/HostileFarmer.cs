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
        //TODO: 变成同步变量
        public Inventory inventory;
        public SpriteRenderer rightHandItem { get; set; }
        public SpriteRenderer leftHandItem { get; set; }
        public EnemyMoveToTarget ai;
        public bool isPursuing;
        public bool isPursuingLastFrame;




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

            OnInventoryItemChange(inventory, "0");
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
                }

                ai.Pursuit();
            }
            else
            {
                if (isPursuingLastFrame)
                {
                    OnStopMovementAction();

                    rb.velocity = Vector2.zero;
                }

                ai.Stroll();
            }

            isPursuingLastFrame = isPursuing;
        }

        public void OnInventoryItemChange(Inventory newValue, string index)
        {
            Item item = newValue.GetItem(index);

            rightHandItem.sprite = item.data.texture.sprite;

            inventory = newValue;
        }

        public Inventory GetInventory() => inventory;

        public void SetInventory(Inventory value) => inventory = value;
    }
}
