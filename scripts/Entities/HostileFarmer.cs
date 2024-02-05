using DG.Tweening;
using GameCore.High;
using SP.Tools.Unity;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    [EntityBinding(EntityID.HostileFarmer)]
    public class HostileFarmer : Enemy, IInventoryOwner, IEnemyWalkToTarget
    {
        public Inventory inventory;
        public SpriteRenderer usingItemRenderer { get; set; }
        public bool isPursuing { get; set; }
        public bool isPursuingLastFrame { get; set; }
        public float jumpForce => 25;






        public override void Initialize()
        {
            base.Initialize();

            MethodAgent.DebugRun(() =>
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

                //添加手持物品的显示
                EntityInventoryOwnerBehaviour.CreateUsingItemRenderer(this, rightArm.transform, 9);
            });

            BindHumanAnimations(this);






            EntityInventoryOwnerBehaviour.LoadInventoryFromCustomData(this);
            EntityInventoryOwnerBehaviour.RefreshInventory(this);
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

            EnemyWalkToTargetBehaviour.OnMovement(this);
        }

        public void WhenStroll()
        {

        }










        public Item[] items { get => inventory.slots; set => inventory.slots = value; }
        public int inventorySlotCount => 1;
        public int usingItemIndex => 0;

        Inventory IInventoryOwner.DefaultInventory()
        {
            Inventory inventory = new(inventorySlotCount, this);

            string item = Random.Range(0, 5) switch
            {
                2 => "ori:iron_hoe",
                0 => "ori:flint_hoe",
                1 => "ori:straw_hat",
                3 => "ori:straw_hat",
                4 => "ori:straw_hat",
                _ => throw new NotImplementedException()
            };

            inventory.SetItem(0, ModFactory.CompareItem(item).DataToItem());

            return inventory;
        }


        public void OnInventoryItemChange(Inventory newValue, string index)
        {
            if (inventory != null)
                EntityInventoryOwnerBehaviour.RefreshInventory(this);
        }


        public Inventory GetInventory() => inventory;
        public void SetInventory(Inventory value) => inventory = value;

        public Item GetItem(string index) => inventory.GetItem(index);
        public void SetItem(string index, Item value) => inventory.SetItem(index, value);
    }
}
