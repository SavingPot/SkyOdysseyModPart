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
    [EntityBinding(EntityID.WanderingLabor)]
    public sealed class WanderingLabor : Entity, IInteractableEntity
    {
        TextImageIdentity coinTextImage;
        int recruitmentExpenses;
        const int autoTalkRadius = 4 * 4; //4^2
        public Vector2 interactionSize { get; } = new(5f, 5f);

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

        protected override void Update()
        {
            base.Update();

            if (Player.TryGetLocal(out var localPlayer) && (localPlayer.transform.position - transform.position).sqrMagnitude <= autoTalkRadius)
            {
                localPlayer.pui.DisplayDialog(
                    new("ori:wandering_labor",
                    "ori:button",
                    new DialogData.DialogDatum[] { new(GameUI.CompareText("ori:dialog.wandering_labor.free_fishing_rod"), "ori:labor_body") }),
                    () =>
                    {
                        localPlayer.ServerAddItem(ModFactory.CompareItem(ItemID.FishingRod).DataToItem());
                        //TODO: 呼叫服务器标记该玩家
                    });
            }
        }

        public void Recruit(Player player)
        {
            //TODO: Network
            GFiles.world.laborData.laborCount++;
            player.ServerAddCoin(-recruitmentExpenses);
            Death();
        }

        public bool PlayerInteraction(Player player)
        {
            if (player.coin < recruitmentExpenses)
                player.pui.DisplayDialog(new("ori:wandering_labor", "ori:button", new DialogData.DialogDatum[] {
                    new(GameUI.CompareText("ori:dialog.wandering_labor.coin_not_enough"), "ori:labor_body")}));
            else if (GFiles.world.laborData.registeredHousings.Count <= GFiles.world.laborData.laborCount)//TODO: Net
                player.pui.DisplayDialog(new("ori:wandering_labor", "ori:button", new DialogData.DialogDatum[] {
                    new(GameUI.CompareText("ori:dialog.wandering_labor.house_not_enough"), "ori:labor_body")}));
            else
                player.pui.DisplayDialog(new("ori:wandering_labor", "ori:button", new DialogData.DialogDatum[] {
                    new(GameUI.CompareText("ori:dialog.wandering_labor.confirm"), "ori:labor_body", options: new System.Action[] { () => Recruit(player), null})}));

            return true;
        }
    }
}
