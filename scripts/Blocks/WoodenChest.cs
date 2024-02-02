using SP.Tools.Unity;
using UnityEngine;
using static GameCore.PlayerUI;

namespace GameCore
{
    public class WoodenChest : StorageBlock
    {
        public const int defaultItemCountConst = 3 * 7;
        public override int defaultItemCount { get; set; } = defaultItemCountConst;
        public override string backpackPanelId { get; set; } = "ori:wooden_chest";
        public static BackpackPanel itemPanel;
        public static ScrollViewIdentity itemView;
        public static InventorySlotUI[] slotUIs = new InventorySlotUI[defaultItemCountConst];

        public static ScrollViewIdentity GenerateItemView()
        {
            if (!itemView)
            {
                //桶的物品视图
                (itemPanel, itemView) = Player.local.pui.GenerateItemViewBackpackPanel("ori:wooden_chest", "ori:switch_button.wooden_chest", 80, Vector2.zero, Vector2.zero, () => itemView.gameObject.SetActive(true));
                
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