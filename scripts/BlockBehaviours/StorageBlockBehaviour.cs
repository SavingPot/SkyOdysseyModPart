using GameCore.High;
using Newtonsoft.Json.Linq;
using SP.Tools;

namespace GameCore
{
    public abstract class StorageBlockBehaviour : Block, IItemContainer
    {
        public abstract void RefreshItemView();





        public abstract int defaultItemCount { get; set; }
        public abstract string sidebarId { get; set; }
        public Item[] items { get; set; }





        public override void DoStart()
        {
            base.DoStart();

            this.LoadItemsFromCustomData(customData, defaultItemCount);
        }

        public override bool PlayerInteraction(Player caller)
        {
            caller.SetBackpackSidebar(sidebarId);
            RefreshItemView();
            caller.ShowBackpackMask();

            return true;
        }

        public override void OnServerSetCustomData()
        {
            RefreshItemView();
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
            Blockmap.instance.SetBlockCustomDataOL(this);
        }
    }
}