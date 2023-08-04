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


            if (owner is Player)
            {
                Player player = (Player)owner;

                if (player.InUseRadius() && player.map.TryGetBlock(PosConvert.WorldToMapPos(player.cursorWorldPos), player.controllingLayer, out Block block) && blockPairs.TryGetValue(block.data.id, out var result))
                {
                    BlockData blockDatum = ModFactory.CompareBlockDatum(result); if (blockDatum == null) return false;
                    Vector2Int pos = block.pos;
                    BlockLayer layer = block.layer;
                    Chunk chunk = block.chunk;

                    chunk.RemoveBlock(pos, layer, true);
                    chunk.AddBlock(pos, layer, blockDatum, null, true);

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
