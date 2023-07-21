using GameCore.High;

namespace GameCore
{
    [ItemBinding(ItemID.ShelledDogTailGrass)]
    public class ShelledDogTailGrassBehaviour : ItemBehaviour
    {
        public override bool Use()
        {
            bool baseUse = base.Use();

            if (baseUse)
                return baseUse;

            if (owner is Player)
            {
                Player player = (Player)owner;

                if (player.InUseRadius() && player.blockmap.TryGetBlock(PosConvert.WorldToMapPos(player.cursorWorldPos), player.controllingLayer, out Block block))
                {
                    if (block.data.id != BlockID.Water)
                        return false;

                    if (GControls.mode == ControlMode.Gamepad)
                        GControls.GamepadVibrationSlightMedium();

                    GM.instance.SummonItem(block.transform.position, ItemID.WashedShelledDogTailGrass);
                    player.ServerReduceItemCount(inventoryIndex, 1);

                    GAudio.Play(AudioID.PickBerryBush);

                    return true;
                }
            }

            return false;
        }

        public ShelledDogTailGrassBehaviour(IInventoryOwner owner, Item instance, string inventoryIndex) : base(owner, instance, inventoryIndex)
        {

        }
    }
}
