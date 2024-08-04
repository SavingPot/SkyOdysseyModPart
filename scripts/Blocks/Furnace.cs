using GameCore.UI;

namespace GameCore
{
    public class Furnace : Block
    {
        public string smeltingResult;

        public override bool PlayerInteraction(Player player)
        {
            player.pui.SetCraftingFacility(data.id);
            player.pui.ShowOrHideBackpackAndSetPanelToCrafting();
            player.pui.RefreshBackpackPanel("ori:crafting");
            return true;
        }
    }
}