namespace GameCore
{
    public class ResurrectionAltar : Block
    {
        public override bool PlayerInteraction(Player player)
        {
            foreach (var item in PlayerCenter.all)
            {
                if (item.isDead)
                {
                    item.Respawn(100, null);
                }
            }

            return base.PlayerInteraction(player);
        }
    }
}