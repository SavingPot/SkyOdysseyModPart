using SP.Tools.Unity;
using UnityEngine;
using static GameCore.PlayerUI;

namespace GameCore
{
    public class CookingPot : StorageBlock
    {
        #region 存储

        public static BackpackPanel staticItemPanel;
        public static ScrollViewIdentity staticItemView;
        public static InventorySlotUI[] staticSlotUIs = new InventorySlotUI[defaultItemCountConst];
        public const int defaultItemCountConst = 4 * 2;
        public override int itemCount => defaultItemCountConst;
        public override string backpackPanelId => "ori:cooking_pot";
        public override BackpackPanel itemPanel { get => staticItemPanel; set => staticItemPanel = value; }
        public override ScrollViewIdentity itemView { get => staticItemView; set => staticItemView = value; }
        public override InventorySlotUI[] slotUIs { get => staticSlotUIs; set => staticSlotUIs = value; }

        #endregion

        public Vector2Int posDown;
        public string cookingResult;

        public override void DoStart()
        {
            base.DoStart();

            posDown = new(pos.x, pos.y - 1);
        }

        public override bool PlayerInteraction(Player caller)
        {
            //如果 没配方 - 下面是篝火
            if (string.IsNullOrWhiteSpace(cookingResult))
            {
                if (chunk.map.TryGetBlock(posDown, isBackground, out Block blockDown) && blockDown.data.id == BlockID.Campfire)
                {
                    foreach (var mod in ModFactory.mods) foreach (var cr in mod.cookingRecipes)
                        {
                            //如果全部原料都可以匹配就添加
                            if (cr.WhetherCanBeCrafted(items, out var ingredientTables))
                            {
                                //消耗掉原料
                                foreach (var ingredients in ingredientTables)
                                {
                                    foreach (var item in ingredients)
                                    {
                                        var temp = items[item.Key];

                                        temp.count -= item.Value;

                                        items[item.Key] = temp.count > 0 ? temp : null;
                                    }
                                }

                                cookingResult = cr.result.id;
                                sr.sprite = ModFactory.CompareTexture("ori:cooking_pot_filled").sprite;
                                RefreshItemView();
                                GAudio.Play(AudioID.Cooking);
                                return true;
                            }
                        }

                    return base.PlayerInteraction(caller);
                }
            }
            //如果 有配方 - 拿着碗
            else if (cookingResult.Contains(":") && caller.TryGetUsingItem()?.data?.id == ItemID.WoodenBowl)
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

                caller.ServerAddItem(target.DataToItem());
                caller.ServerReduceItemCount(caller.usingItemIndex.ToString(), 1);

                GAudio.Play(AudioID.FillingWaterBowl);

                cookingResult = null;
                sr.sprite = ModFactory.CompareTexture("ori:cooking_pot").sprite;
                return true;
            }

            return base.PlayerInteraction(caller);
        }
    }
}