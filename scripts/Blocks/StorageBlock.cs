using GameCore.Network;
using GameCore.UI;
using SP.Tools;
using UnityEngine;

namespace GameCore
{
    public abstract class StorageBlock : Block, IItemContainer
    {
        public abstract BackpackPanel itemPanel { get; set; }
        public abstract ScrollViewIdentity itemView { get; set; }
        public abstract InventorySlotUI[] slotUIs { get; set; }





        public abstract int itemCount { get; }
        public abstract string backpackPanelId { get; }
        public Item[] items { get; set; }





        public override void DoStart()
        {
            base.DoStart();

            this.LoadItemsFromCustomData(customData, itemCount);
        }

        public override bool PlayerInteraction(Player player)
        {
            RefreshItemView();
            player.pui.ShowOrHideBackpackAndSetPanelTo(backpackPanelId);

            return true;
        }

        public override void OnServerSetCustomData()
        {
            RefreshItemView();
        }

        public virtual void RefreshItemView()
        {
            if (!itemView)
            {
                (var modId, var panelName) = Tools.SplitModIdAndName(backpackPanelId);

                //物品视图
                (itemPanel, itemView) = Player.local.pui.GenerateItemViewBackpackPanel(
                    backpackPanelId,
                    $"{modId}:switch_button.{panelName}",
                    80,
                    Vector2.zero,
                    Vector2.zero,
                    RefreshItemView,
                    () => itemView.gameObject.SetActive(true));

                //初始化所有UI
                for (int i = 0; i < slotUIs.Length; i++)
                {
                    itemView.AddChild((slotUIs[i] = new($"{modId}:button.{panelName}_item_{i}", $"{modId}:image.{panelName}_item_{i}", itemView.gridLayoutGroup.cellSize)).button);
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

            //写入数据到存档
            this.WriteItemsToCustomData(customData);
            if (Server.isServer) WriteCustomDataToSave();

            //使客户端刷新
            Map.instance.SetBlockCustomDataOL(this);
        }
    }
}