using UnityEngine;

namespace GameCore
{
    public class GrassBlockBehaviour : Block
    {
        public string randomUpdateID;

        public override void DoStart()
        {
            base.DoStart();

            randomUpdateID = $"ori:grass_block_{GetInstanceID()}";

            RandomUpdater.Bind(randomUpdateID, 0.075f, GrowGrass);
        }

        public override void OnRecovered()
        {
            base.OnRecovered();

            RandomUpdater.Unbind(randomUpdateID);
        }

        void GrowGrass()
        {
            Vector2Int targetPos = pos + new Vector2Int(0, 1);

            if (!Map.instance.HasBlock(targetPos, layer))
            {
                //TODO: 长任意一种植物
                BlockData grassData = ModFactory.CompareBlockDatum(BlockID.Grass);

                if (grassData == null)
                    return;

                Map.instance.SetBlockNet(targetPos, layer, grassData.id, null);
            }
        }
    }
}
