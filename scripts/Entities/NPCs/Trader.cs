using UnityEngine;
using System;
using UnityEngine.InputSystem;
using GameCore.UI;
using GameCore.High;

namespace GameCore
{
    [EntityBinding(EntityID.Trader)]
    public class Trader : CoreNPC
    {
        public int autoTalkRadius = 5 * 5; //5^2
        public TradeUI tradeUI;



        public override void Initialize()
        {
            base.Initialize();

            //添加肢体
            Player.GeneratePlayerShapedModel(this,
                                                ModFactory.CompareTexture("ori:trader_head").sprite,
                                                ModFactory.CompareTexture("ori:trader_body").sprite,
                                                ModFactory.CompareTexture("ori:trader_right_arm").sprite,
                                                ModFactory.CompareTexture("ori:trader_left_arm").sprite,
                                                ModFactory.CompareTexture("ori:trader_right_leg").sprite,
                                                ModFactory.CompareTexture("ori:trader_left_leg").sprite,
                                                ModFactory.CompareTexture("ori:trader_right_foot").sprite,
                                                ModFactory.CompareTexture("ori:trader_left_foot").sprite);
        }

        public override bool PlayerInteraction(Player player)
        {
            //Todo: Auto destroy after player has been far away from the NPC
            tradeUI ??= TradeUI.GenerateItemViewBackpackPanel(player.pui, "ori:trader", new TradeUI.ItemTrade[]
            {
                new(ItemID.BlockGun, 1, 100),
            });

            player.pui.Backpack.ShowOrHideBackpackAndSetPanelTo(tradeUI.backpackPanelId);
            return true;
        }
    }
}
