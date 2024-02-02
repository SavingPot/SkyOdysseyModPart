using SP.Tools.Unity;
using UnityEngine;
using static GameCore.PlayerUI;

namespace GameCore
{
    public class ClayFurnace : StorageBlock
    {
        #region 存储

        public const int defaultItemCountConst = 4 * 2;
        public override int defaultItemCount { get; set; } = defaultItemCountConst;
        public override string backpackPanelId { get; set; } = "ori:clay_furnace";
        public static BackpackPanel itemPanel;
        public static ScrollViewIdentity itemView;
        public static InventorySlotUI[] slotUIs = new InventorySlotUI[defaultItemCountConst];

        public static ScrollViewIdentity GenerateItemView()
        {
            if (!itemView)
            {
                //物品视图
                (itemPanel, itemView) = Player.local.pui.GenerateItemViewBackpackPanel("ori:clay_furnace", "ori:switch_button.clay_furnace", 80, Vector2.zero, Vector2.zero, () => itemView.gameObject.SetActive(true));

                //初始化所有UI
                for (int i = 0; i < slotUIs.Length; i++)
                {
                    itemView.AddChild((slotUIs[i] = InventorySlotUI.Generate($"ori:button.clay_furnace_item_{i}", $"ori:image.clay_furnace_item_{i}", itemView.gridLayoutGroup.cellSize)).button);
                }
            }

            return itemView;
        }

        public override void RefreshItemView()
        {
            GenerateItemView();

            for (int i = 0; i < slotUIs.Length; i++)
            {
                slotUIs[i].Refresh(this, i.ToString());
            }
        }

        #endregion

        public string smeltingResult;
    }
}