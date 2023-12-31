using UnityEngine;
using static GameCore.PlayerUI;

namespace GameCore
{
    public class ClayFurnace : StorageBlock
    {
        #region 存储

        public const int defaultItemCountConst = 4 * 2;
        public override int defaultItemCount { get; set; } = defaultItemCountConst;
        public override string sidebarId { get; set; } = "ori:clay_furnace";
        public static ScrollViewIdentity itemView;
        public static InventorySlotUI[] slotUIs = new InventorySlotUI[defaultItemCountConst];

        public static ScrollViewIdentity GenerateItemView()
        {
            if (!itemView)
            {
                //物品视图
                Player.local.pui.GenerateSidebar(SidebarType.Left, "ori:sw.clay_furnace_items", 52.5f, 210, Vector2.zero, "ori:crafting_result", "ori:sidebar_sign.clay_furnace", out itemView, out _, out _);

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
            for (int i = 0; i < slotUIs.Length; i++)
            {
                slotUIs[i].Refresh(this, i.ToString());
            }
        }

        #endregion

        public string smeltingResult;

        public override void DoStart()
        {
            base.DoStart();

            GenerateItemView().gameObject.SetActive(false);
        }
    }
}