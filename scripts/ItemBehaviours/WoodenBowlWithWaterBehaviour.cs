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
        // public static ScrollViewIdMessage itemView;
        // public static InventorySlotUI[] slotUIs = new InventorySlotUI[defaultItemCountConst];


        // public static ScrollViewIdMessage GenerateItemView()
        // {
        //     if (!itemView)
        //     {
        //         //碗的物品视图
        //         Player.local.GenerateSidebar(SidebarType.Left, "ori:sw.wooden_bowl_with_water_items", 55, 220, Vector2.zero, out itemView);

        //         //初始化所有UI
        //         for (int i = 0; i < slotUIs.Length; i++)
        //         {
        //             itemView.AddChild((slotUIs[i] = InventorySlotUI.Generate($"ori:button.wooden_bowl_with_water_item_{i}", $"ori:image.wooden_bowl_with_water_item_{i}", itemView.gridLayoutGroup.cellSize)).button);
        //         }
        //     }

        //     return itemView;
        // }

        // public override void RefreshItemView()
        // {
        //     for (int i = 0; i < slotUIs.Length; i++)
        //     {
        //         slotUIs[i].Refresh(this, i.ToString());
        //     }
        // }


        public WoodenBowlWithWaterBehaviour(IInventoryOwner owner, Item instance, string inventoryIndex) : base(owner, instance, inventoryIndex)
        {

        }
    }
}
