using SP.Tools.Unity;
using UnityEngine;
using static GameCore.PlayerUI;

namespace GameCore
{
    public class RemoteMarket : Block
    {
        public override bool PlayerInteraction(Player player)
        {
            if (player.TryGetUsingItem(out var item) && item.data.economy.worth != 0)
            {
                player.ServerReduceUsingItemCount(1);
                player.AddCoin(item.data.economy.worth);
                return true;
            }

            return base.PlayerInteraction(player);
        }
    }
}