using GameCore.UI;
using UnityEngine;

namespace GameCore
{
    public class RemoteMarket : Block
    {
        public TradeUI tradeUI;

        public override void DoStart()
        {
            base.DoStart();

            tradeUI = new("ori:remote_market", new TradeUI.ItemTrade[]
            {
                new(BlockID.Dirt, 1, 1),
                new(BlockID.Sand, 1, 2),
                new(BlockID.PotatoCrop, 1, 3),
                new(BlockID.OnionCrop, 1, 5),
                new(BlockID.CarrotCrop, 1, 7),
                new(BlockID.PumpkinCrop, 1, 10),
                new(BlockID.TomatoCrop, 1, 10),
                new(BlockID.WatermelonCrop, 1, 10),
            });
        }

        public override bool PlayerInteraction(Player player)
        {
            if (player.TryGetUsingItem(out var item) && item.data.economy.worth != 0)
            {
                player.ServerReduceUsingItemCount(1);
                player.AddCoin(item.data.economy.worth);
                GAudio.Play(AudioID.Trade);
                return true;
            }
            else
            {
                player.pui.ShowOrHideBackpackAndSetPanelTo(tradeUI.backpackPanelId);
                return true;
            }
        }
    }
}