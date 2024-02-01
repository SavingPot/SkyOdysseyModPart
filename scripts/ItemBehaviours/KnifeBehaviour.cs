using SP.Tools;
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
                //TODO: 检测光标位置是否存在物品
                // if (backItem != null)
                // {
                //     ValueTag<string> tag = backItem.data.GetValueTag("ori:knife_cutting", str => str, string.Empty);

                //     if (tag.hasTag)
                //     {
                //         var splitted = tag.tagValue.Split(',');

                //         if (splitted.Length == 2)
                //         {
                //             string id = splitted[0];
                //             ushort count = splitted[1].ToInt().ToUShort();

                //             ItemData basic = ModFactory.CompareItem(id);

                //             if (basic != null)
                //             {
                //                 Item datum = basic.DataToItem();
                //                 datum.count = count;

                //                 player.ServerAddItem(datum);
                //                 player.ServerReduceItemCount(backIndex.ToString(), 1);
                //             }
                //         }
                //     }
                // }
            }

            return false;
        }

        public KnifeBehaviour(IInventoryOwner owner, Item instance, string inventoryIndex) : base(owner, instance, inventoryIndex)
        {

        }
    }
}
