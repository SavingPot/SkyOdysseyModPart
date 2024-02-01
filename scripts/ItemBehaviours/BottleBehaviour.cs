using UnityEngine;

namespace GameCore
{
    [ItemBinding(ItemID.Bottle)]
    public class BottleBehaviour : ItemBehaviour
    {
        public override bool Use(Vector2 point)
        {
            bool baseUse = base.Use(point);

            if (baseUse)
                return baseUse;

            if (owner is Player player)
            {
                if (player.InUseRadius(point) && player.map.TryGetBlock(PosConvert.WorldToMapPos(point), player.isControllingBackground, out Block block))
                {
                    if (block.data.id != BlockID.Water)
                        return false;

                    ItemData target = ModFactory.CompareItem(ItemID.WaterBottle);

                    if (target == null)
                        return false;

                    if (GControls.mode == ControlMode.Gamepad)
                        GControls.GamepadVibrationSlightMedium();

                    player.ServerAddItem(target.DataToItem());
                    player.ServerReduceItemCount(inventoryIndex, 1);

                    GAudio.Play(AudioID.FillingWaterBottle);

                    return true;
                }
            }

            return false;
        }

        public BottleBehaviour(IInventoryOwner owner, Item data, string inventoryIndex) : base(owner, data, inventoryIndex)
        {

        }
    }
}
