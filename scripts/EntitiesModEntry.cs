namespace GameCore
{
    public class EntitiesModEntry : ModEntry
    {
        public override void OnLoaded()
        {
            base.OnLoaded();

            Player.GravitySet += caller =>
            {
                if (caller.inventory.breastplate?.data?.id == ItemID.FeatherWing)
                {
                    caller.gravity *= 0.7f;
                }
            };

            PlayerCenter.OnAddPlayer += caller =>
            {
                caller.backpackSidebarTable.Add("ori:barrel", (() => { Barrel.GenerateItemView().gameObject.SetActive(true); }, () => { Barrel.GenerateItemView().gameObject.SetActive(false); }));
                caller.backpackSidebarTable.Add("ori:cooking_pot", (() => { CookingPot.GenerateItemView().gameObject.SetActive(true); }, () => { CookingPot.GenerateItemView().gameObject.SetActive(false); }));
                caller.backpackSidebarTable.Add("ori:clay_furnace", (() => { ClayFurnace.GenerateItemView().gameObject.SetActive(true); }, () => { ClayFurnace.GenerateItemView().gameObject.SetActive(false); }));
                caller.backpackSidebarTable.Add("ori:wooden_chest", (() => { WoodenChest.GenerateItemView().gameObject.SetActive(true); }, () => { WoodenChest.GenerateItemView().gameObject.SetActive(false); }));
                //caller.backpackSidebarTable.Add("ori:wooden_bowl_with_water", (() => { WoodenBowlWithWaterBehaviour.GenerateItemView().gameObject.SetActive(true); }, () => { WoodenBowlWithWaterBehaviour.GenerateItemView().gameObject.SetActive(false); }));
            };

            GM.OnUpdate += WaterCenter.WaterPhysics;

            GScene.AfterChanged += scene =>
            {
                switch (scene.name)
                {
                    //TODO: 调整音频播放位置
                    case SceneNames.MainMenu:
                        GAudio.Play(AudioID.Town);
                        GAudio.Stop(AudioID.WhyNotComeToTheParty);
                        break;

                    case SceneNames.GameScene:
                        GAudio.Stop(AudioID.Town);
                        GAudio.Play(AudioID.WhyNotComeToTheParty);
                        break;
                }
            };
        }
    }
}
