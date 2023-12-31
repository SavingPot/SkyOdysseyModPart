using UnityEngine;
using static GameCore.PlayerUI;

namespace GameCore
{
    public class SoftClayFurnace : Block
    {
        public Vector2Int posDown;

        public override void DoStart()
        {
            base.DoStart();

            posDown = new(pos.x, pos.y - 1);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (Map.instance.TryGetBlock(posDown, isBackground, out Block b) && b.data.id == BlockID.Campfire)
            {
                Map.instance.SetBlockNet(pos, isBackground, BlockID.ClayFurnace, null);
            }
        }
    }
}