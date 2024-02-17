using SP.Tools.Unity;
using UnityEngine;
using static GameCore.PlayerUI;

namespace GameCore
{
    public abstract class CookingStorageBlock : StorageBlock
    {
        public abstract string cookingType { get; }
        public abstract string uncookedTexture { get; }
        public abstract string cookedTexture { get; }
        
        public Vector2Int posDown;
        public CookingRecipe cookedRecipe = null;

        public override void DoStart()
        {
            base.DoStart();

            posDown = new(pos.x, pos.y - 1);
        }

        public override bool PlayerInteraction(Player caller)
        {
            //如果 没配方 - 下面是篝火
            if (cookedRecipe == null)
            {
                if (chunk.map.TryGetBlock(posDown, isBackground, out Block blockDown) && blockDown.data.id == BlockID.Campfire)
                {
                    foreach (var mod in ModFactory.mods) foreach (var cr in mod.cookingRecipes)
                        {
                            //如果全部原料都可以匹配就添加
                            if (cr.type == cookingType && cr.WhetherCanBeCrafted(items, out var ingredientTables))
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

                                cookedRecipe = cr;
                                sr.sprite = ModFactory.CompareTexture(cookedTexture).sprite;
                                RefreshItemView();
                                GAudio.Play(AudioID.Cooking);
                                return true;
                            }
                        }

                    return base.PlayerInteraction(caller);
                }
            }
            //如果 有配方 - 拿着碗
            else if (!cookedRecipe.needBowl || caller.TryGetUsingItem()?.data?.id == ItemID.WoodenBowl)
            {
                // string[] splitted = cookingResult.Split(':');

                // if (splitted.Length != 2)
                // {
                //     Debug.LogError($"菜谱ID {cookingResult} 格式必须为 namespace:name");
                //     return false;
                // }

                string itemId = cookedRecipe.result.id;//$"{splitted[0]}:wooden_bowl_with_{splitted[1]}";
                ItemData target = ModFactory.CompareItem(itemId);

                if (target == null)
                {
                    Debug.LogError($"烹饪失败: 无法匹配到物品 {itemId}");
                    return false;
                }

                if (GControls.mode == ControlMode.Gamepad)
                    GControls.GamepadVibrationSlightMedium();

                caller.ServerAddItem(target.DataToItem());
                caller.ServerReduceItemCount(caller.usingItemIndex.ToString(), 1);

                GAudio.Play(AudioID.FillingWaterBowl);

                cookedRecipe = null;
                sr.sprite = ModFactory.CompareTexture(uncookedTexture).sprite;
                return true;
            }

            return base.PlayerInteraction(caller);
        }
    }
}