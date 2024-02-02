using SP.Tools.Unity;
using UnityEngine;
using static GameCore.PlayerUI;

namespace GameCore
{
    public class Barrel : StorageBlock
    {
        public const int defaultItemCountConst = 4 * 7;
        public override int defaultItemCount { get; set; } = defaultItemCountConst;
        public override string backpackPanelId { get; set; } = "ori:barrel";
        public static BackpackPanel itemPanel;
        public static ScrollViewIdentity itemView;
        public static InventorySlotUI[] slotUIs = new InventorySlotUI[defaultItemCountConst];

        public static ScrollViewIdentity GenerateItemView()
        {
            if (!itemView)
            {
                //物品视图
                (itemPanel, itemView) = Player.local.pui.GenerateItemViewBackpackPanel("ori:barrel", "ori:switch_button.barrel", 80, Vector2.zero, Vector2.zero, () => itemView.gameObject.SetActive(true));

                //初始化所有UI
                for (int i = 0; i < slotUIs.Length; i++)
                {
                    itemView.AddChild((slotUIs[i] = InventorySlotUI.Generate($"ori:button.barrel_item_{i}", $"ori:image.barrel_item_{i}", itemView.gridLayoutGroup.cellSize)).button);
                }
            }

            return itemView;
        }

        public override void DoStart()
        {
            base.DoStart();

            GenerateItemView().gameObject.SetActive(false);
        }

        public override void RefreshItemView()
        {
            for (int i = 0; i < slotUIs.Length; i++)
            {
                slotUIs[i].Refresh(this, i.ToString());
            }
        }
    }
}