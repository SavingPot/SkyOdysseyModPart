using UnityEngine;

namespace GameCore
{
    public class GrassBlock : Block
    {
        public string randomUpdateID;

        public override void DoStart()
        {
            base.DoStart();

            randomUpdateID = $"ori:grass_block_{gameObject.GetInstanceID()}";

            RandomUpdater.Bind(randomUpdateID, 0.05f, GrowGrass);
        }

        public override void OnRecovered()
        {
            base.OnRecovered();

            RandomUpdater.Unbind(randomUpdateID);
        }

        void GrowGrass()
        {
            Vector2Int targetPos = pos + new Vector2Int(0, 1);

            if (!Map.instance.HasBlock(targetPos, isBackground))
            {
                //TODO: 长任意一种植物、草蔓延
                BlockData grassData = ModFactory.CompareBlockData(BlockID.Grass);

                if (grassData == null)
                    return;

                Map.instance.SetBlockNet(targetPos, isBackground, BlockStatus.Normal, grassData.id, null);
            }
        }
    }
}
