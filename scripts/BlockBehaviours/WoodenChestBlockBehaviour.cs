using UnityEngine;
using static GameCore.PlayerUI;

namespace GameCore
{
    public class WoodenChestBlockBehaviour : StorageBlockBehaviour
    {
        public const int defaultItemCountConst = 3 * 7;
        public override int defaultItemCount { get; set; } = defaultItemCountConst;
        public override string sidebarId { get; set; } = "ori:wooden_chest";
        public static ScrollViewIdMessage itemView;
        public static InventorySlotUI[] slotUIs = new InventorySlotUI[defaultItemCountConst];

        public static ScrollViewIdMessage GenerateItemView()
        {
            if (!itemView)
            {
                //桶的物品视图
                Player.local.pui.GenerateSidebar(SidebarType.Left, "ori:sw.wooden_chest_items", 52.5f, 210, Vector2.zero, "ori:crafting_result", "ori:sidebar_sign.wooden_chest", out itemView, out _, out _);

                //初始化所有UI
                for (int i = 0; i < slotUIs.Length; i++)
                {
                    itemView.AddChild((slotUIs[i] = InventorySlotUI.Generate($"ori:button.wooden_chest_item_{i}", $"ori:image.wooden_chest_item_{i}", itemView.gridLayoutGroup.cellSize)).button);
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