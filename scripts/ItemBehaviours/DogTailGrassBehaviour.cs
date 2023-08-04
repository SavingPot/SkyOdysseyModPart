using GameCore.High;

namespace GameCore
{
    [ItemBinding(ItemID.DogTailGrass)]
    public class DogTailGrassBehaviour : ItemBehaviour
    {
        public override bool Use()
        {
            bool baseUse = base.Use();

            if (baseUse)
                return baseUse;

            if (owner is Player)
            {
                Player player = (Player)owner;

                if (player.InUseRadius() && player.map.TryGetBlock(PosConvert.WorldToMapPos(player.cursorWorldPos), player.controllingLayer, out Block block))
                {
                    if (block.data.id == BlockID.Water)
                        return false;

                    if (GControls.mode == ControlMode.Gamepad)
                        GControls.GamepadVibrationSlightMedium();

                    GM.instance.SummonItem(block.transform.position, ItemID.ShelledDogTailGrass);
                    player.ServerReduceItemCount(inventoryIndex, 1);

                    GAudio.Play(AudioID.PickBerryBush);

                    return true;
                }
            }


            return false;
        }

        public DogTailGrassBehaviour(IInventoryOwner owner, Item instance, string inventoryIndex) : base(owner, instance, inventoryIndex)
        {

        }
    }
}
