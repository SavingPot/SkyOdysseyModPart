using SP.Tools;
using SP.Tools.Unity;
using UnityEngine;

namespace GameCore
{
    public class KnifeBehaviour : ItemBehaviour
    {
        public override bool Use(Vector2 point)
        {
            bool baseUse = base.Use(point);

            if (baseUse)
                return baseUse;

            if (owner is Player player)
            {
                foreach (var entity in EntityCenter.all)
                {
                    //当使用物品时检测光标是否在掉落物上
                    if (entity is Drop drop && drop.mainCollider.IsInCollider(point))
                    {
                        ValueTag<string> tag = drop.item.data.GetValueTag("ori:knife_cutting", str => str, string.Empty);

                        //如果没有 ori:knife_cutting 标签就跳过
                        if (!tag.hasTag)
                            continue;



                        var splitted = tag.tagValue.Split(',');

                        if (splitted.Length != 2)
                        {
                            Debug.LogError($"切割物品 {drop.item.data.id} 失败, 其格式不正确, 格式应为 \"ori:knife_cutting=ori:dog_tail_grass_noodles,1\"");
                            return false;
                        }

                        string id = splitted[0];
                        ushort count = splitted[1].ToInt().ToUShort();

                        ItemData basic = ModFactory.CompareItem(id);

                        if (basic != null)
                        {
                            //获取结果
                            Item datum = basic.DataToItem();
                            datum.count = count;

                            //给予玩家物品
                            player.ServerAddItem(datum);

                            //减少掉落物的物品数量, 并检测要不要销毁掉落物
                            drop.item.count--;
                            if (drop.item.count == 0)
                                drop.Death();
                        }
                        else
                        {
                            Debug.LogError($"切割物品 {drop.item.data.id} 失败, 无法匹配到产物 {id}");
                            return false;
                        }
                    }
                }
            }

            return false;
        }

        public KnifeBehaviour(IInventoryOwner owner, Item instance, string inventoryIndex) : base(owner, instance, inventoryIndex)
        {

        }
    }
}
