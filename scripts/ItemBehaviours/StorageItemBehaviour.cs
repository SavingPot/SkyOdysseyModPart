using SP.Tools;
using UnityEngine;

namespace GameCore
{
    public abstract class StorageItemBehaviour : ItemBehaviour, IItemContainer
    {
        public abstract void RefreshItemView();





        public abstract int defaultItemCount { get; set; }
        public abstract string sidebarId { get; set; }
        public Item[] items { get; set; }





        public override void OnEnter()
        {
            base.OnEnter();

            this.LoadItemsFromCustomData(instance.customData, defaultItemCount);
        }

        public override bool Use(Vector2 point)
        {
            bool baseUse = base.Use(point);

            if (baseUse)
                return baseUse;

            if (owner is Player player)
            {
                player.ShowOrHideBackpackAndSetSideBarTo(sidebarId);
                RefreshItemView();
            }

            return true;
        }





        Item IItemContainer.GetItem(string index)
        {
            return items[index.ToInt()];
        }

        void IItemContainer.SetItem(string index, Item value)
        {
            items[index.ToInt()] = value;

            RefreshItemView();

            this.WriteItemsToCustomData(instance.customData);
        }

        public StorageItemBehaviour(IInventoryOwner owner, Item instance, string inventoryIndex) : base(owner, instance, inventoryIndex)
        {

        }
    }
}
