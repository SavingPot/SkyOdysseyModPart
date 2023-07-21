using UnityEngine;
using static GameCore.PlayerUI;

namespace GameCore
{
    public class BarrelBlockBehaviour : StorageBlockBehaviour
    {
        public const int defaultItemCountConst = 4 * 7;
        public override int defaultItemCount { get; set; } = defaultItemCountConst;
        public override string sidebarId { get; set; } = "ori:barrel";
        public static ScrollViewIdMessage itemView;
        public static InventorySlotUI[] slotUIs = new InventorySlotUI[defaultItemCountConst];

        public static ScrollViewIdMessage GenerateItemView()
        {
            if (!itemView)
            {
                //桶的物品视图
                Player.local.pui.GenerateSidebar(SidebarType.Left, "ori:sw.barrel_items", 52.5f, 210, Vector2.zero, "ori:crafting_result", "ori:sidebar_sign.barrel", out itemView, out _, out _);

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