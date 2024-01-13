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

        public override bool Use()
        {
            bool baseUse = base.Use();

            if (baseUse)
                return baseUse;


            if (owner is Player player)
            {
                if (player.InUseRadius() && player.map.TryGetBlock(PosConvert.WorldToMapPos(player.cursorWorldPos), player.isControllingBackground, out Block block) && blockPairs.TryGetValue(block.data.id, out var result))
                {
                    BlockData blockDatum = ModFactory.CompareBlockData(result); if (blockDatum == null) return false;
                    Vector2Int pos = block.pos;
                    bool isBackground = block.isBackground;
                    Chunk chunk = block.chunk;

                    chunk.RemoveBlock(pos, isBackground, true);
                    chunk.AddBlock(pos, isBackground, blockDatum, null, true);

                    if (GControls.mode == ControlMode.Gamepad)
                        GControls.GamepadVibrationSlightMedium();

                    GAudio.Play(AudioID.Hoe);

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
