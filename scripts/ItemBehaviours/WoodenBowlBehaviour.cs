namespace GameCore
{
    [ItemBinding(ItemID.WoodenBowl)]
    public class WoodenBowlBehaviour : ItemBehaviour
    {
        public override bool Use()
        {
            bool baseUse = base.Use();

            if (baseUse)
                return baseUse;

            if (owner is Player player)
            {
                if (player.InUseRadius() && player.map.TryGetBlock(PosConvert.WorldToMapPos(player.cursorWorldPos), player.isControllingBackground, out Block block))
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
