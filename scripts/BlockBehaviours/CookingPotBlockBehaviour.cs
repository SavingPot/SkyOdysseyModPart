using UnityEngine;
using static GameCore.PlayerUI;

namespace GameCore
{
    public class CookingPotBlockBehaviour : StorageBlockBehaviour
    {
        #region 存储

        public const int defaultItemCountConst = 4 * 2;
        public override int defaultItemCount { get; set; } = defaultItemCountConst;
        public override string sidebarId { get; set; } = "ori:cooking_pot";
        public static ScrollViewIdMessage itemView;
        public static InventorySlotUI[] slotUIs = new InventorySlotUI[defaultItemCountConst];

        public static ScrollViewIdMessage GenerateItemView()
        {
            if (!itemView)
            {
                //物品视图
                Player.local.pui.GenerateSidebar(SidebarType.Left, "ori:sw.cooking_pot_items", 52.5f, 210, Vector2.zero, "ori:crafting_result", "ori:sidebar_sign.cooking_pot", out itemView, out _, out _);

                //初始化所有UI
                for (int i = 0; i < slotUIs.Length; i++)
                {
                    itemView.AddChild((slotUIs[i] = InventorySlotUI.Generate($"ori:button.cooking_pot_item_{i}", $"ori:image.cooking_pot_item_{i}", itemView.gridLayoutGroup.cellSize)).button);
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

        public Vector2Int posDown;
        public string cookingResult;

        public override void DoStart()
        {
            base.DoStart();

            GenerateItemView().gameObject.SetActive(false);
            posDown = new(pos.x, pos.y - 1);
        }

        public override bool PlayerInteraction(Player caller)
        {
            //如果 没配方 - 下面是篝火
            if (string.IsNullOrWhiteSpace(cookingResult))
            {
                if (chunk.map.TryGetBlock(posDown, layer, out Block result) && result.data.id == BlockID.Campfire)
                {
                    foreach (var mod in ModFactory.mods) foreach (var cr in mod.cookingRecipes)
                        {
                            //如果全部原料都可以匹配就添加
                            if (Player.WhetherCanBeCrafted(cr, items, out var stuff))
                            {
                                foreach (var stuffItems in stuff)
                                {
                                    foreach (var item in stuffItems)
                                    {
                                        var temp = items[item.Key];

                                        temp.count -= item.Value;

                                        items[item.Key] = temp.count > 0 ? temp : null;
                                    }
                                }

                                cookingResult = cr.result.id;
                                RefreshItemView();
                                GAudio.Play(AudioID.Cooking);
                                return true;
                            }
                        }

                    return base.PlayerInteraction(caller);
                }
            }
            //如果 有配方 - 拿着碗
            else if (cookingResult.Contains(':') && caller.TryGetUsingItem()?.data?.id == ItemID.WoodenBowl)
            {
                string[] splitted = cookingResult.Split(':');

                if (splitted.Length != 2)
                {
                    Debug.LogError($"菜谱ID {cookingResult} 格式必须为 namespace:name");
                    return false;
                }

                string itemId = $"{splitted[0]}:wooden_bowl_with_{splitted[1]}";
                ItemData target = ModFactory.CompareItem(itemId);

                if (target == null)
                {
                    Debug.LogError($"无法匹配到物品 {itemId}");
                    return false;
                }

                if (GControls.mode == ControlMode.Gamepad)
                    GControls.GamepadVibrationSlightMedium();

                caller.ServerAddItem(target.ToExtended());
                caller.ServerReduceItemCount(caller.usingItemIndex.ToString(), 1);

                GAudio.Play(AudioID.FillingWaterBowl);

                cookingResult = null;
                return true;
            }

            return base.PlayerInteraction(caller);
        }
    }
}