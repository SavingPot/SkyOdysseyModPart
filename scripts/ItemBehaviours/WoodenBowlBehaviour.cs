using UnityEngine;

namespace GameCore
{
    [ItemBinding(ItemID.WoodenBowl)]
    public class WoodenBowlBehaviour : ItemBehaviour
    {
        public override bool Use(Vector2 point)
        {
            bool baseUse = base.Use(point);

            if (baseUse)
                return baseUse;

            if (owner is Player player)
            {
                if (player.IsPointInteractable(point) && player.map.TryGetBlock(PosConvert.WorldToMapPos(point), player.isControllingBackground, out Block block))
                {
                    switch (block.data.id)
                    {
                        case BlockID.Water:
                            {
                                ItemData target = ModFactory.CompareItem(ItemID.WoodenBowlWithWater);

                                if (target == null)
                                    return false;

                                if (GControls.mode == ControlMode.Gamepad)
                                    GControls.GamepadVibrationSlightMedium();

                                player.ServerAddItem(target.DataToItem());
                                player.ServerReduceItemCount(inventoryIndex, 1);

                                GAudio.Play(AudioID.FillingWaterBowl);

                                return true;
                            }

                        default:
                            {
                                return false;
                            }
                    }
                }
            }

            return false;
        }

        public WoodenBowlBehaviour(IInventoryOwner owner, Item instance, string inventoryIndex) : base(owner, instance, inventoryIndex)
        {

        }
    }
}
