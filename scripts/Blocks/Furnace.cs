using GameCore.UI;

namespace GameCore
{
    public class Furnace : Block
    {
        public string smeltingResult;

        public override bool PlayerInteraction(Player player)
        {
            player.pui.Backpack.SetCraftingFacility(data.id);
            player.pui.Backpack.ShowOrHideBackpackAndSetPanelToCrafting();
            player.pui.Backpack.RefreshBackpackPanel("ori:crafting");
            return true;
        }
    }
}