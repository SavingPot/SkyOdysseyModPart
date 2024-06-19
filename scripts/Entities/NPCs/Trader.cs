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

            #region 添加肢体
            MethodAgent.DebugRun(() =>
            {
                CreateModel();
                body = AddBodyPart("body", ModFactory.CompareTexture("ori:nick_body_naked").sprite, Vector2.zero, 3, model.transform, BodyPartType.Body);
                head = AddBodyPart("head", ModFactory.CompareTexture("ori:nick_head").sprite, new(-0.03f, -0.06f), 6, body, BodyPartType.Head);
                rightArm = AddBodyPart("rightarm", ModFactory.CompareTexture("ori:nick_right_arm").sprite, Vector2.zero, 4, body, BodyPartType.RightArm);
                leftArm = AddBodyPart("leftarm", ModFactory.CompareTexture("ori:nick_left_arm").sprite, Vector2.zero, 2, body, BodyPartType.LeftArm);
                rightLeg = AddBodyPart("rightleg", ModFactory.CompareTexture("ori:nick_right_leg").sprite, Vector2.zero, 2, body, BodyPartType.RightLeg);
                leftLeg = AddBodyPart("leftleg", ModFactory.CompareTexture("ori:nick_left_leg").sprite, Vector2.zero, 1, body, BodyPartType.LeftLeg);
            });
            #endregion

            tradeUI = new("ori:trader", new TradeUI.ItemTrade[]
            {
                new(ItemID.BlockGun, 1, 100),
            });
        }

        public override void PlayerInteraction(Player player)
        {
            base.PlayerInteraction(player);

            player.pui.ShowOrHideBackpackAndSetPanelTo(tradeUI.backpackPanelId);
        }
    }
}
