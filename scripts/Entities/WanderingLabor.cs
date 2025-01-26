using GameCore.Network;
using GameCore.UI;
using Mirror;
using SP.Tools;
using SP.Tools.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.WanderingLabor), NotSummonable]
    public sealed class WanderingLabor : Entity, IInteractableEntity
    {
        TextImageIdentity coinTextImage;
        int recruitmentExpenses;
        public Vector2 interactionSize { get; } = new(1f, 1.8f);

        public override void Initialize()
        {
            base.Initialize();

            isHurtable = false;

            //添加贴图
            AddSpriteRenderer("ori:labor_body");

            //TODO: 存档化、受游戏进程影响
            recruitmentExpenses = Random.Range(60, 100);

            //获取画布
            GetOrAddEntityCanvas();
            usingCanvas.transform.localPosition = new(0, 1.6f);
            coinTextImage = GameUI.GenerateCoinTextImage(13, usingCanvas.transform);
            coinTextImage.SetText(recruitmentExpenses);
        }

        public bool PlayerInteraction(Player player)
        {
            if (player.coin >= recruitmentExpenses)
                player.pui.DisplayDialog(new("ori:wandering_labor", "ori:button", new DialogData.DialogDatum[] {
                    new(GameUI.CompareText("ori:dialog.wandering_labor.confirm"), "ori:labor_body")}));
            else
                player.pui.DisplayDialog(new("ori:wandering_labor", "ori:button", new DialogData.DialogDatum[] {
                    new(GameUI.CompareText("ori:dialog.wandering_labor.coin_not_enough"), "ori:labor_body")}));

            return true;
        }
    }
}
