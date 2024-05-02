using GameCore.UI;

namespace GameCore
{
    public class WoodenChest : StorageBlock
    {
        public static BackpackPanel staticItemPanel;
        public static ScrollViewIdentity staticItemView;
        public static InventorySlotUI[] staticSlotUIs = new InventorySlotUI[defaultItemCountConst];
        public const int defaultItemCountConst = 3 * 7;
        public override int itemCount => defaultItemCountConst;
        public override string backpackPanelId => "ori:wooden_chest";
        public override BackpackPanel itemPanel { get => staticItemPanel; set => staticItemPanel = value; }
        public override ScrollViewIdentity itemView { get => staticItemView; set => staticItemView = value; }
        public override InventorySlotUI[] slotUIs { get => staticSlotUIs; set => staticSlotUIs = value; }
    }
}