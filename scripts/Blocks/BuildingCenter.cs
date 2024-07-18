using System;
using GameCore.UI;
using SP.Tools;
using SP.Tools.Unity;
using UnityEngine;

namespace GameCore
{
    public class BuildingCenter : Block
    {
        public static PanelIdentity bottomPanel;

        public static PanelIdentity housingRentalPanel;
        public static ButtonIdentity switchToHousingRentalPanelButton;
        public static ScrollViewIdentity housingRentalScrollView;
        static HousingRentalScrollViewPool housingRentalScrollViewPool;
        public static PanelIdentity housingInfoPanel;
        public static ButtonIdentity housingInfoToRentalButton;
        public static ButtonIdentity housingInfoCollectRentButton;
        public static TextIdentity housingInfoNameText;
        public static ScrollViewIdentity housingRentalNPCScrollView;
        static HousingRentalNPCScrollViewPool housingRentalNPCScrollViewPool;

        public static PanelIdentity housingPurchasePanel;
        public static ButtonIdentity switchToHousingPurchasePanelButton;


        static BuildingCenter operatingBuildingCenter;





        public override bool PlayerInteraction(Player player)
        {
            //刷新属性
            operatingBuildingCenter = this;
            housingRentalPanel.RefreshUI();

            //设置页面
            GameUI.SetPage(bottomPanel);

            return true;
        }




        public static int GetRentAmount(string tenantName)
        {
            //TODO: 记录上次收租时间，并根据它计算租金
            return 1;
        }







        public static void InitUI()
        {
            bottomPanel = GameUI.AddBackpackFormedPanel("ori:panel.building_center.bottom", GameUI.canvasRT, true);



            /* ----------------------------------- 出租 ----------------------------------- */
            housingRentalPanel = GameUI.AddPanel("ori:panel.building_center.housing_rental", bottomPanel);
            housingRentalPanel.AfterRefreshing += _ =>
            {
                foreach (var item in housingRentalScrollView.content.GetComponentsInChildren<ButtonIdentity>())
                    housingRentalScrollViewPool.Recover(item);

                foreach (var chunk in Map.instance.chunks)
                {
                    //只会寻找当前区域内的房屋
                    if (chunk.regionIndex != operatingBuildingCenter.chunk.regionIndex)
                        continue;

                    foreach (var block in chunk.wallBlocks)
                    {
                        if (block is Doorplate doorplate)
                        {
                            var button = housingRentalScrollViewPool.Get();
                            var name = doorplate.GetRoomName();
                            var hasName = !name.IsNullOrWhiteSpace();
                            var tenantName = doorplate.GetTenantName();
                            var hasTenant = !tenantName.IsNullOrWhiteSpace();

                            //如果点击了就进入房屋设置
                            button.SetOnClickBind(() =>
                            {
                                ShowHouseInfo(doorplate, name, tenantName);
                                housingInfoPanel.gameObject.SetActive(true);
                            });
                            button.BindButtonAudio();

                            button.SetColor((hasName, hasTenant) switch
                            {
                                (true, false) => Color.white, //有房间名无租客
                                (true, true) => Color.gray,   //有房间名有租客
                                _ => Color.red                //无房间名
                            });

                            button.SetText($"{(hasName ? name : "未命名房间")} {block.pos}");
                        }
                    }
                }
            };

            //侧边栏按钮
            (_, switchToHousingRentalPanelButton) = GameUI.GenerateSidebarSwitchButton(
                "ori:image.building_center.switch_to_housing_rental.background",
                "ori:button.building_center.switch_to_housing_rental",
                "ori:switch_to_housing_rental",
                bottomPanel,
                0);
            switchToHousingRentalPanelButton.OnClickBind(() =>
            {
                housingRentalPanel.RefreshUI();
                housingRentalPanel.gameObject.SetActive(true);
                housingPurchasePanel.gameObject.SetActive(false);
            });



            //房屋列表
            housingRentalScrollView = GameUI.AddScrollView(UIA.Middle, "ori:sv.building_center.housing_rental", housingRentalPanel);
            housingRentalScrollView.SetSizeDeltaX(250);
            housingRentalScrollView.SetGridLayoutGroupCellSizeToMax(45);
            housingRentalScrollViewPool = new();



            //房屋信息面板
            housingInfoPanel = GameUI.AddPanel("ori:panel.building_center.housing_info", housingRentalPanel, true);

            housingInfoToRentalButton = GameUI.AddButton(UIA.UpperLeft, "ori:button.building_center.housing_info_to_rental", housingInfoPanel);
            housingInfoToRentalButton.SetAPos(housingInfoToRentalButton.sd.x / 2 + 15, -housingInfoToRentalButton.sd.y / 2 - 15);
            housingInfoToRentalButton.OnClickBind(() =>
            {
                housingInfoPanel.gameObject.SetActive(false);
                housingRentalPanel.RefreshUI();
            });

            housingInfoCollectRentButton = GameUI.AddButton(UIA.Down, "ori:button.building_center.housing_info_collect_rent", housingInfoPanel);
            housingInfoCollectRentButton.SetAPos(0, housingInfoCollectRentButton.sd.y / 2 + 15);
            housingInfoCollectRentButton.buttonText.autoCompareText = false;

            housingInfoNameText = GameUI.AddText(UIA.Middle, "ori:text.building_center.housing_info_name", housingInfoPanel);
            housingInfoNameText.autoCompareText = false;
            housingInfoNameText.text.raycastTarget = false;
            housingInfoNameText.AddAPosY(150);



            //NPC列表
            housingRentalNPCScrollView = GameUI.AddScrollView(UIA.Middle, "ori:sv.building_center.housing_rental_npc", housingInfoPanel);
            housingRentalNPCScrollView.SetSizeDeltaX(250);
            housingRentalNPCScrollView.SetGridLayoutGroupCellSizeToMax(45);
            housingRentalNPCScrollViewPool = new();



            /* ----------------------------------- 购买 ----------------------------------- */
            housingPurchasePanel = GameUI.AddPanel("ori:panel.building_center.housing_purchase", bottomPanel, true);

            //侧边栏按钮
            (_, switchToHousingPurchasePanelButton) = GameUI.GenerateSidebarSwitchButton(
                "ori:image.building_center.switch_to_housing_purchase.background",
                "ori:button.building_center.switch_to_housing_purchase",
                "ori:switch_to_housing_purchase",
                bottomPanel,
                1);
            switchToHousingPurchasePanelButton.OnClickBind(() =>
            {
                housingRentalPanel.gameObject.SetActive(false);
                housingPurchasePanel.gameObject.SetActive(true);
            });
        }

        public static void ShowHouseInfo(Doorplate doorplate, string name, string tenantName)
        {
            //TODO: 需要确保NPC和房屋没有在查看时因为其它玩家失效
            //设置房屋名
            housingInfoNameText.SetText(name);
            housingInfoCollectRentButton.SetText(tenantName.IsNullOrWhiteSpace() ? "收租：无租客" : $"向 {GameUI.CompareText(tenantName)} 收租：{GetRentAmount(tenantName)}");

            //回收 NPC 列表
            foreach (var item in housingRentalNPCScrollView.content.GetComponentsInChildren<ButtonIdentity>())
                housingRentalNPCScrollViewPool.Recover(item);

            //生成 NPC 列表
            foreach (var npc in NPCCenter.all)
            {
                var button = housingRentalNPCScrollViewPool.Get();
                button.SetText($"{GameUI.CompareText(npc.data.id)}");
                button.SetOnClickBind(() =>
                {
                    doorplate.SetTenantName(npc.data.id);

                    //刷新房屋信息
                    ShowHouseInfo(doorplate, name, doorplate.GetTenantName());
                });
                button.BindButtonAudio();
            }
        }
    }

    public sealed class HousingRentalScrollViewPool : ObjectPool<ButtonIdentity>
    {
        public override ButtonIdentity Get()
        {
            var result = base.Get();
            result.gameObject.SetActive(true);
            return result;
        }

        public override void Recover(ButtonIdentity obj)
        {
            obj.gameObject.SetActive(false);

            base.Recover(obj);
        }

        public override ButtonIdentity Generation()
        {
            var result = GameUI.AddButton(UIA.Middle, $"ori:button.building_center.housing_rental_item_{Tools.randomInt}", BuildingCenter.housingRentalScrollView.content);

            result.buttonText.autoCompareText = false;

            return result;
        }
    }

    public sealed class HousingRentalNPCScrollViewPool : ObjectPool<ButtonIdentity>
    {
        public override ButtonIdentity Get()
        {
            var result = base.Get();
            result.gameObject.SetActive(true);
            return result;
        }

        public override void Recover(ButtonIdentity obj)
        {
            obj.gameObject.SetActive(false);

            base.Recover(obj);
        }

        public override ButtonIdentity Generation()
        {
            var result = GameUI.AddButton(UIA.Middle, $"ori:button.building_center.housing_rental_npc_item_{Tools.randomInt}", BuildingCenter.housingRentalNPCScrollView.content);

            result.buttonText.autoCompareText = false;

            return result;
        }
    }
}