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
                new(BlockID.Dirt, 1, 1),
                new(BlockID.Sand, 1, 2),
                new(BlockID.Gravel,1,3),
                new(BlockID.PotatoCrop, 1, 3),
                new(BlockID.OnionCrop, 1, 5),
                new(BlockID.CarrotCrop, 1, 7),
                new(BlockID.PumpkinCrop, 1, 10),
                new(BlockID.TomatoCrop, 1, 10),
                new(BlockID.WatermelonCrop, 1, 10),
                new(BlockID.OakSeed, 1, 15),
                new(BlockID.AcaciaSeed, 1, 22),
                new(BlockID.MangroveSeed, 1, 30),
            });
        }

        public override void PlayerInteraction(Player player)
        {
            base.PlayerInteraction(player);

            player.pui.ShowOrHideBackpackAndSetPanelTo(tradeUI.backpackPanelId);
        }
    }
}
