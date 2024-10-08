using GameCore.High;
using SP.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    public class HoeBehaviour : ItemBehaviour
    {
        public static readonly Dictionary<string, string> blockPairs = new()
        {
            { BlockID.GrassBlock, BlockID.Dirt },
            { BlockID.Dirt, BlockID.Farmland },
        };

        public override bool Use(Vector2 point)
        {
            bool baseUse = base.Use(point);

            if (baseUse)
                return baseUse;


            if (owner is Player player)
            {
                if (player.IsPointInteractable(point) && Map.instance.TryGetBlock(PosConvert.WorldToMapPos(point), player.isControllingBackground, out Block block) && blockPairs.TryGetValue(block.data.id, out var result))
                {
                    BlockData blockDatum = ModFactory.CompareBlockData(result); if (blockDatum == null) return false;
                    Vector2Int pos = block.pos;
                    bool isBackground = block.isBackground;
                    Chunk chunk = block.chunk;

                    //Remove 不执行生命周期，等到 Add 再更新，以节约性能
                    chunk.RemoveBlock(pos, isBackground, true, false);
                    chunk.AddBlock(pos, isBackground, BlockStatus.Normal, blockDatum, null, true, true);

                    if (GControls.mode == ControlMode.Gamepad)
                        GControls.GamepadVibrationSlightMedium();

                    GAudio.Play(AudioID.Hoe, block.pos);

                    return true;
                }
            }

            return false;
        }

        public HoeBehaviour(IInventoryOwner owner, Item data, string inventoryIndex) : base(owner, data, inventoryIndex)
        {

        }
    }
}
