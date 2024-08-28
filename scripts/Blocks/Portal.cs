using GameCore.UI;

namespace GameCore
{
    public class Portal : Block
    {
        public override bool PlayerInteraction(Player player)
        {
            GameUI.SetPage(player.pui.mapPanel);
            player.ServerRefreshTeleportPoints();

            return true;
        }
    }
}