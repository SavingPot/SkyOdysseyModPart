using GameCore.High;

namespace GameCore
{
    [ItemBinding(ItemID.DogTailGrassPowder)]
    public class DogTailGrassPowderBehaviour : ItemBehaviour
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

                    if (GControls.mode == ControlMode.Gamepad)
                        GControls.GamepadVibrationSlightMedium();

                    GM.instance.SummonDrop(block.transform.position, ItemID.DogTailGrassDough);
                    GM.instance.SummonDrop(block.transform.position, ItemID.Bottle);
                    player.ServerReduceItemCount(inventoryIndex, 1);

                    GAudio.Play(AudioID.PickBerryBush);

                    return true;
                }
            }

            return false;
        }

        public DogTailGrassPowderBehaviour(IInventoryOwner owner, Item instance, string inventoryIndex) : base(owner, instance, inventoryIndex)
        {

        }
    }
}
