using System.Linq;
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

            this.LoadItemsFromCustomData(itemCount, ref customData);
        }

        public override bool PlayerInteraction(Player player)
        {
            RefreshItemView();
            player.pui.Backpack.ShowOrHideBackpackAndSetPanelTo(backpackPanelId);

            return true;
        }

        public override void OnServerSetCustomData()
        {
            RefreshItemView();
        }

        protected virtual void CreateItemView()
        {
            (var modId, var panelName) = Tools.SplitModIdAndName(backpackPanelId);

            //物品视图
            (itemPanel, itemView) = Player.local.pui.Backpack.GenerateItemViewBackpackPanel(
                backpackPanelId,
                $"{modId}:switch_button.{panelName}",
                80,
                Vector2.zero,
                Vector2.zero,
                RefreshItemView,
                () => itemView.gameObject.SetActive(true),
                null,
                AutoDestroyBackpackPanel);

            //初始化所有UI
            for (int i = 0; i < slotUIs.Length; i++)
            {
                itemView.AddChild((slotUIs[i] = new($"{modId}:button.{panelName}_item_{i}", $"{modId}:image.{panelName}_item_{i}", itemView.gridLayoutGroup.cellSize)).button);
            }
        }

        public virtual void RefreshItemView()
        {
            if (!itemView)
                CreateItemView();

            //刷新每个格子
            for (int i = 0; i < slotUIs.Length; i++)
            {
                slotUIs[i].Refresh(this, i.ToString());
            }
        }

        protected virtual void AutoDestroyBackpackPanel()
        {
            if (!Player.TryGetLocal(out var player))
                return;

            //矩形十格以内不销毁背包面板
            if (Mathf.Abs(player.transform.position.x - pos.x) < 8 && Mathf.Abs(player.transform.position.y - pos.y) < 8)
                return;

            //检测一下背包面板是否还存在
            if (player.pui.Backpack.backpackPanels.Any(p => p.id == backpackPanelId))
                player.pui.Backpack.DestroyBackpackPanel(backpackPanelId);
        }





        Item IItemContainer.GetItem(string index)
        {
            return items[index.ToInt()];
        }

        void IItemContainer.SetItem(string index, Item value)
        {
            items[index.ToInt()] = value;

            RefreshItemView();

            //写入数据到存档并推给服务器
            this.WriteItemsToCustomData(itemCount, ref customData);
            PushCustomDataToServer();
        }
    }
}