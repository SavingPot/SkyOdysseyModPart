using GameCore.UI;

namespace GameCore
{
    public class FryingPan : CookingStorageBlock
    {
        #region 烹饪

        public override string uncookedTexture => "ori:frying_pan";
        public override string cookedTexture => "ori:frying_pan_filled";

        #endregion

        #region 存储

        public static BackpackPanel staticItemPanel;
        public static ScrollViewIdentity staticItemView;
        public static InventorySlotUI[] staticSlotUIs = new InventorySlotUI[defaultItemCountConst];
        public const int defaultItemCountConst = 4 * 1;
        public override int itemCount => defaultItemCountConst;
        public override string backpackPanelId => "ori:frying_pan";
        public override BackpackPanel itemPanel { get => staticItemPanel; set => staticItemPanel = value; }
        public override ScrollViewIdentity itemView { get => staticItemView; set => staticItemView = value; }
        public override InventorySlotUI[] slotUIs { get => staticSlotUIs; set => staticSlotUIs = value; }

        #endregion
    }
}