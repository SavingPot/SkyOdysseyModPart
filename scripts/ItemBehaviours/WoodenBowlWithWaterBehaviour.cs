using UnityEngine;
using static GameCore.Player;

namespace GameCore
{
    [ItemBinding(ItemID.WoodenBowlWithWater)]
    public class WoodenBowlWithWaterBehaviour : ItemBehaviour//StorageItemBehaviour
    {
        // public const int defaultItemCountConst = 10;
        // public override int defaultItemCount { get; set; } = defaultItemCountConst;
        // public override string sidebarId { get; set; } = "ori:wooden_bowl_with_water";
        // public static ScrollViewIdentity itemView;
        // public static InventorySlotUI[] slotUIs = new InventorySlotUI[defaultItemCountConst];


        public WoodenBowlWithWaterBehaviour(IInventoryOwner owner, Item instance, string inventoryIndex) : base(owner, instance, inventoryIndex)
        {

        }
    }
}
