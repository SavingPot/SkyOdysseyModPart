using UnityEngine;

namespace GameCore
{
    [ItemBinding(ItemID.DogTailGrass)]
    public class DogTailGrassBehaviour : ItemBehaviour
    {
        public override bool Use(Vector2 point)
        {
            bool baseUse = base.Use(point);

            if (baseUse)
                return baseUse;

            if (owner is Player player)
            {
                if (player.IsPointInteractable(point) && Map.instance.TryGetBlock(PosConvert.WorldToMapPos(point), player.isControllingBackground, out Block block))
                {
                    if (block.data.id == BlockID.Water)
                        return false;

                    if (GControls.mode == ControlMode.Gamepad)
                        GControls.GamepadVibrationSlightMedium();

                    GM.instance.SummonDrop(block.transform.position, ItemID.ShelledDogTailGrass);
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
