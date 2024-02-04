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
    [EntityBinding(EntityID.HostileFarmer)]
    public class HostileFarmer : Enemy, IInventoryOwner
    {
        public Inventory inventory;
        public SpriteRenderer usingItemRenderer { get; set; }
        public EnemyMoveToTarget ai;
        public bool isPursuing;
        public bool isPursuingLastFrame;





        protected override void Awake()
        {
            base.Awake();

            ai = new(this, 30);
        }

        public override void Initialize()
        {
            base.Initialize();

            MethodAgent.TryRun(() =>
            {
                //添加身体部分
                CreateModel();
                body = AddBodyPart("body", ModFactory.CompareTexture("ori:hostile_farmer_body").sprite, Vector2.zero, 5, model.transform, BodyPartType.Body);
                head = AddBodyPart("head", ModFactory.CompareTexture("ori:hostile_farmer_head").sprite, new(-0.02f, -0.03f), 10, body, BodyPartType.Head, new(-0.03f, -0.04f));
                rightArm = AddBodyPart("rightArm", ModFactory.CompareTexture("ori:hostile_farmer_right_arm").sprite, new(0, 0.03f), 8, body, BodyPartType.RightArm);
                leftArm = AddBodyPart("leftArm", ModFactory.CompareTexture("ori:hostile_farmer_left_arm").sprite, new(0, 0.03f), 3, body, BodyPartType.LeftArm);
                rightLeg = AddBodyPart("rightLeg", ModFactory.CompareTexture("ori:hostile_farmer_right_leg").sprite, new(0.02f, 0.04f), 3, body, BodyPartType.RightLeg);
                leftLeg = AddBodyPart("leftLeg", ModFactory.CompareTexture("ori:hostile_farmer_left_leg").sprite, new(-0.02f, 0.04f), 1, body, BodyPartType.LeftLeg);
                rightFoot = AddBodyPart("rightFoot", ModFactory.CompareTexture("ori:hostile_farmer_right_foot").sprite, Vector2.zero, 3, rightLeg, BodyPartType.RightFoot);
                leftFoot = AddBodyPart("leftFoot", ModFactory.CompareTexture("ori:hostile_farmer_left_foot").sprite, Vector2.zero, 1, leftLeg, BodyPartType.LeftFoot);

                //添加双手物品的显示
                usingItemRenderer = ObjectTools.CreateSpriteObject(rightArm.transform, "item");

                usingItemRenderer.sortingOrder = 9;

                SetUsingItemRendererLocalPositionAndScale(Vector2.zero, Vector2.one);
            }, true);


            Creature.BindHumanAnimations(this);

            this.LoadInventoryFromCustomData(1);

            //没有物品就设置
            if (Item.Null(inventory.GetItem(0)))
            {
                Item item = Random.Range(0, 5) switch
                {
                    1 => ModFactory.CompareItem("ori:straw_hat").DataToItem(),
                    2 => ModFactory.CompareItem("ori:straw_hat").DataToItem(),
                    3 => ModFactory.CompareItem("ori:straw_hat").DataToItem(),
                    4 => ModFactory.CompareItem("ori:straw_hat").DataToItem(),
                    _ => ModFactory.CompareItem("ori:flint_hoe").DataToItem()
                };

                inventory.SetItem(0, item);
            }

            OnInventoryItemChange(inventory, "0");
        }

        public void SetUsingItemRendererLocalPositionAndScale(Vector2 localPosition, Vector2 localScale)
        {
            usingItemRenderer.transform.localPosition = new(0.1f + localPosition.x, -0.5f + localPosition.y);
            usingItemRenderer.transform.SetScale(0.6f * localScale.x, 0.6f * localScale.y);
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










        public Item[] items { get => inventory.slots; set => inventory.slots = value; }


        public void OnInventoryItemChange(Inventory newValue, string index)
        {
            Item item = newValue.GetItem(index);

            usingItemRenderer.sprite = item.data.texture.sprite;

            inventory = newValue;
        }


        public Inventory GetInventory() => inventory;
        public void SetInventory(Inventory value) => inventory = value;

        public Item GetItem(string index) => inventory.GetItem(index);
        public void SetItem(string index, Item value) => inventory.SetItem(index, value);
    }
}
