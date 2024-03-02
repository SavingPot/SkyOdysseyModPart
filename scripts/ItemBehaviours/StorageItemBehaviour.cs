using SP.Tools;
using UnityEngine;
using static GameCore.PlayerUI;

namespace GameCore
{
    public abstract class StorageItemBehaviour : ItemBehaviour, IItemContainer
    {
        public abstract BackpackPanel itemPanel { get; set; }
        public abstract ScrollViewIdentity itemView { get; set; }
        public abstract InventorySlotUI[] slotUIs { get; set; }





        public abstract int itemCount { get; set; }
        public abstract string backpackPanelId { get; set; }
        public Item[] items { get; set; }





        public override void OnEnter()
        {
            base.OnEnter();

            this.LoadItemsFromCustomData(instance.customData, itemCount);
        }

        public override bool Use(Vector2 point)
        {
            bool baseUse = base.Use(point);

            if (baseUse)
                return baseUse;

            if (owner is Player player)
            {
                RefreshItemView();
                player.pui.ShowOrHideBackpackAndSetPanelTo(backpackPanelId);
            }

            return true;
        }

        public virtual void RefreshItemView()
        {
            if (!itemView)
            {
                (var modId, var panelName) = Tools.SplitModIdAndName(backpackPanelId);

                //物品视图
                (itemPanel, itemView) = Player.local.pui.GenerateItemViewBackpackPanel(
                    backpackPanelId,
                    $"{modId}:button.switch_{panelName}",
                    80,
                    Vector2.zero,
                    Vector2.zero,
                    RefreshItemView,
                    () => itemView.gameObject.SetActive(true));

                //初始化所有UI
                for (int i = 0; i < slotUIs.Length; i++)
                {
                    itemView.AddChild((slotUIs[i] = InventorySlotUI.Generate($"{modId}:button.{panelName}_item_{i}", $"{modId}:image.{panelName}_item_{i}", itemView.gridLayoutGroup.cellSize)).button);
                }
            }


            for (int i = 0; i < slotUIs.Length; i++)
            {
                slotUIs[i].Refresh(this, i.ToString());
            }
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
