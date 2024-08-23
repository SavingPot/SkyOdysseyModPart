using GameCore.UI;
using UnityEngine;

namespace GameCore
{
    [ItemBinding(ItemID.WashedShelledDogTailGrass)]
    public class WashedShelledDogTailGrassBehaviour : ItemBehaviour
    {
        public static ushort powerNeed = 3;

        public override bool Use(Vector2 point)
        {
            bool baseUse = base.Use(point);

            if (baseUse)
                return baseUse;

            if (owner is Player player)
            {
                if (instance.count < powerNeed)
                {
                    InternalUIAdder.instance.SetStatusText($"你需要至少 {powerNeed}个 水洗去壳狗尾草才可以做成粉末");
                    return false;
                }

                //TODO: 改为用石头敲打, 检测地上是否有水洗去壳狗尾草
                if (Map.instance.GetBlock(PosConvert.WorldToMapPos(point), player.isControllingBackground)?.data?.id == BlockID.Stone)
                {
                    for (int i = 0; i < player.inventory.slots.Length; i++)
                    {
                        var slot = player.inventory.slots[i];

                        if (slot?.data?.id == ItemID.Bottle && slot.count >= 1)
                        {
                            ItemData powder = ModFactory.CompareItem(ItemID.DogTailGrassPowder);

                            if (powder == null)
                                return false;

                            if (GControls.mode == ControlMode.Gamepad)
                                GControls.GamepadVibrationSlightMedium();

                            player.ServerAddItem(powder.DataToItem());
                            player.ServerReduceItemCount(i.ToString(), 1);
                            player.ServerReduceItemCount(inventoryIndex, powerNeed);

                            GAudio.Play(AudioID.PickBerryBush, point);
                            return true;
                        }
                    }

                    InternalUIAdder.instance.SetStatusText("背包中需要一个瓶子");
                }
                else
                {
                    InternalUIAdder.instance.SetStatusText("请对着石头方块使用");
                }
            }

            return false;
        }

        public WashedShelledDogTailGrassBehaviour(IInventoryOwner owner, Item instance, string inventoryIndex) : base(owner, instance, inventoryIndex)
        {

        }
    }
}
