using System.Collections.Generic;
using System.Linq;
using GameCore.UI;
using UnityEngine;

namespace GameCore
{
    public abstract class CookingStorageBlock : StorageBlock
    {
        public abstract string cookingType { get; }
        public abstract string uncookedTexture { get; }
        public abstract string cookedTexture { get; }

        public Vector2Int posDown;

        public override void DoStart()
        {
            base.DoStart();

            posDown = new(pos.x, pos.y - 1);
        }

        public override bool PlayerInteraction(Player player)
        {
            //如果 下面是篝火
            if (chunk.map.TryGetBlock(posDown, isBackground, out Block blockDown) && blockDown.data.id == BlockID.Campfire)
            {
                //TODO: 改成像塞尔达一样一次只放一些材料
                foreach (var mod in ModFactory.mods) foreach (var recipe in mod.cookingRecipes)
                    {
                        //如果全部原料都可以匹配就添加
                        if (recipe.type == cookingType && recipe.WhetherCanBeCrafted(items, out var ingredientTables))
                        {
                            return Cook(recipe, ingredientTables, player);
                        }
                    }



                //如果没有匹配的配方，就尝试烹饪奇怪的料理
                if (items.Any(p => !Item.Null(p)))
                {
                    //获得奇怪的料理配方
                    var strangeDish = ModFactory.CompareCookingRecipe(CookingRecipeID.StrangeDish);

                    //处理原料表，也就是把所有的物品都煮掉，变成奇怪的料理
                    var strangeIngredientTables = new List<Dictionary<int, ushort>>();
                    for (int i = 0; i < items.Length; i++)
                    {
                        var item = items[i];

                        if (!Item.Null(item))
                        {
                            var temp = new Dictionary<int, ushort>
                            {
                                { i, item.count }
                            };
                            strangeIngredientTables.Add(temp);
                        }
                    }

                    //烹饪奇怪的料理
                    return Cook(strangeDish, strangeIngredientTables, player);
                }
            }

            return base.PlayerInteraction(player);
        }

        bool Cook(CookingRecipe recipe, List<Dictionary<int, ushort>> ingredientTables, Player player)
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



            //TODO: 烹饪的过程时间



            //给予物品
            string itemId = recipe.result.id;//$"{splitted[0]}:wooden_bowl_with_{splitted[1]}";
            ItemData targetItem = ModFactory.CompareItem(itemId);

            if (targetItem == null)
            {
                Debug.LogError($"烹饪失败: 无法匹配到物品 {itemId}");
                return false;
            }

            player.ServerAddItem(targetItem.DataToItem());
            player.pui.ShowGainRareItemUI(targetItem);



            //体验反馈
            if (GControls.mode == ControlMode.Gamepad)
                GControls.GamepadVibrationSlightMedium();

            GAudio.Play(AudioID.Cooking);




            // sr.sprite = ModFactory.CompareTexture(uncookedTexture).sprite;
            // sr.sprite = ModFactory.CompareTexture(cookedTexture).sprite;
            //刷新
            RefreshItemView();
            return true;
        }
    }
}