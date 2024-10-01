using DG.Tweening;
using GameCore.High;
using SP.Tools;
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
        public SpriteRenderer usingShieldRenderer { get; set; }
        public SpriteRenderer usingItemRenderer { get; set; }
        public BoxCollider2D usingItemCollider { get; set; }
        public InventoryItemRendererCollision usingItemCollisionComponent { get; set; }
        public bool isPursuing { get; set; }
        public bool isPursuingLastFrame { get; set; }
        public float jumpForce => 25;
        public bool isAttacking { get; private set; }
        string[] seedsToBeDropped = new[] { BlockID.OnionCrop, BlockID.CarrotCrop, BlockID.TomatoCrop, BlockID.PotatoCrop, BlockID.PumpkinCrop, BlockID.WatermelonCrop };






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
                EntityInventoryOwnerBehaviour.CreateItemRenderers(this, leftArm.transform, rightArm.transform, 9, 9);
            });

            BindHumanAnimations(this);






            EntityInventoryOwnerBehaviour.LoadInventoryFromCustomData(this);
            EntityInventoryOwnerBehaviour.RefreshInventory(this);
        }

        protected override void SummonDrops()
        {
            base.SummonDrops();

            //把装备栏的第一个物品爆出
            if (inventory != null)
            {
                var item = inventory.GetItemChecked(0);
                if (!Item.Null(item))
                {
                    GM.instance.SummonDrop(transform.position, item.data.id, item.count, item.customData?.ToString(Newtonsoft.Json.Formatting.None));
                }
            }

            //掉落种子
            var id = seedsToBeDropped.Extract(Tools.staticRandom);
            var count = (ushort)Random.Range(1, 4);
            GM.instance.SummonDrop(transform.position, id, count);
        }


        public void ModifyUsingShieldRendererTransform()
        {
            usingShieldRenderer.transform.localPosition = new(0.04f, -0.7f);
            usingShieldRenderer.transform.localScale = new(0.5f, 0.5f);
        }

        public void ModifyUsingItemRendererTransform(Vector2 localPosition, Vector2 localScale, int localRotation)
        {
            localScale *= 0.5f;
            localPosition += new Vector2(0.1f, -0.5f);

            usingItemRenderer.transform.SetLocalPositionAndRotation(localPosition, Quaternion.Euler(0, 0, localRotation));
            usingItemRenderer.transform.SetScale(localScale);
        }

        public override Vector2 GetMovementDirection()
        {
            if (isDead)
                return Vector2.zero;

            return EnemyWalkToTargetBehaviour.GetMovementVelocity(this);
        }

        public void WhenStroll()
        {

        }










        public Item[] items { get => inventory.slots; set => inventory.slots = value; }
        public int inventorySlotCount => 1;
        public int usingItemIndex { get; set; } = 0;

        Inventory IInventoryOwner.DefaultInventory()
        {
            Inventory inventory = new(inventorySlotCount, this);

            string item = Random.Range(0, 3) switch
            {
                0 => "ori:iron_hoe",
                1 => "ori:flint_hoe",
                2 => "ori:flint_hoe",
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

        public int TotalDefense
        {
            get
            {
                int totalDefense = 0;
                totalDefense += inventory.helmet?.data?.Helmet?.defense ?? 0;
                totalDefense += inventory.breastplate?.data?.Breastplate?.defense ?? 0;
                totalDefense += inventory.legging?.data?.Legging?.defense ?? 0;
                totalDefense += inventory.boots?.data?.Boots?.defense ?? 0;
                return totalDefense;
            }
        }
    }
}
