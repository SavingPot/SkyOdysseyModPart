namespace GameCore
{
    [ItemBinding(ItemID.Bottle)]
    public class BottleBehaviour : ItemBehaviour
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
                    if (block.data.id != BlockID.Water)
                        return false;

                    ItemData target = ModFactory.CompareItem(ItemID.WaterBottle);

                    if (target == null)
                        return false;

                    if (GControls.mode == ControlMode.Gamepad)
                        GControls.GamepadVibrationSlightMedium();

                    player.ServerAddItem(target.ToExtended());
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
