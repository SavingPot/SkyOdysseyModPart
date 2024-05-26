using GameCore.UI;
using SP.Tools;
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





        public static void InitUI()
        {
            bottomPanel = GameUI.AddBackpackFormedPanel("ori:panel.building_center.bottom", GameUI.canvasRT, true);



            /* ----------------------------------- 出租 ----------------------------------- */
            housingRentalPanel = GameUI.AddPanel("ori:panel.building_center.housing_rental", bottomPanel);
            housingRentalPanel.AfterRefreshing += _ =>
            {
                foreach (var item in housingRentalScrollView.content.GetComponentsInChildren<ButtonIdentity>())
                {
                    housingRentalScrollViewPool.Recover(item);
                }

                foreach (var chunk in Map.instance.chunks)
                {
                    //只会寻找当前区域内的房屋
                    if (chunk.regionIndex != operatingBuildingCenter.chunk.regionIndex)
                        continue;

                    foreach (var block in chunk.blocks)
                    {
                        if (block is Doorplate doorplate)
                        {
                            var button = housingRentalScrollViewPool.Get();
                            var name = doorplate.GetRoomName();
                            var hasName = !name.IsNullOrWhiteSpace();
                            var tenantName = doorplate.GetTenantName();
                            var hasTenant = !tenantName.IsNullOrWhiteSpace();

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

            housingRentalScrollView = GameUI.AddScrollView(UIA.Middle, "ori:sv.building_center.housing_rental", housingRentalPanel);
            housingRentalScrollViewPool = new();



            /* ----------------------------------- 购买 ----------------------------------- */
            housingPurchasePanel = GameUI.AddPanel("ori:panel.building_center.housing_purchase", bottomPanel, true);

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
}