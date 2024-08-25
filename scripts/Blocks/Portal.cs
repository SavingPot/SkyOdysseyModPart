namespace GameCore
{
    public class Portal : Block
    {
        public bool hasSummonedGuard;



        //TODO: 把方块变成 ActivatedPortal
        public override bool PlayerInteraction(Player player)
        {
            if (chunk.regionIndex.x == 0)
                return false;

            if (!hasSummonedGuard)
            {
                GM.instance.SummonEntity(new(pos.x, pos.y + 1), $"{GFiles.world.GetOrAddRegion(chunk.regionIndex).biomeId}_biome_guard");
                hasSummonedGuard = true;
                return true;
            }
            else
            {
                return base.PlayerInteraction(player);
            }
        }
    }
}