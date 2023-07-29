namespace GameCore
{
    public class EntitiesModEntry : ModEntry
    {
        public override void OnLoaded()
        {
            base.OnLoaded();

            Player.GravitySet += caller =>
            {
                if (caller.inventory.HasItem(ItemID.FeatherWing))
                {
                    caller.gravity *= 0.75f;
                }
            };

            Player.backpackSidebarTable.Add("ori:barrel", (() => { BarrelBlockBehaviour.GenerateItemView().gameObject.SetActive(true); }, () => { BarrelBlockBehaviour.GenerateItemView().gameObject.SetActive(false); }));
            Player.backpackSidebarTable.Add("ori:cooking_pot", (() => { CookingPotBlockBehaviour.GenerateItemView().gameObject.SetActive(true); }, () => { CookingPotBlockBehaviour.GenerateItemView().gameObject.SetActive(false); }));
            Player.backpackSidebarTable.Add("ori:wooden_chest", (() => { WoodenChestBlockBehaviour.GenerateItemView().gameObject.SetActive(true); }, () => { WoodenChestBlockBehaviour.GenerateItemView().gameObject.SetActive(false); }));
            //Player.backpackSidebarTable.Add("ori:wooden_bowl_with_water", (() => { WoodenBowlWithWaterBehaviour.GenerateItemView().gameObject.SetActive(true); }, () => { WoodenBowlWithWaterBehaviour.GenerateItemView().gameObject.SetActive(false); }));

            GScene.AfterChanged += scene =>
            {
                switch (scene.name)
                {
                    case SceneNames.mainScene:
                        GAudio.Play(AudioID.Town);
                        GAudio.Stop(AudioID.WhyNotComeToTheParty);
                        break;

                    case SceneNames.gameScene:
                        GAudio.Stop(AudioID.Town);
                        GAudio.Play(AudioID.WhyNotComeToTheParty);
                        break;
                }
            };
        }
    }
}
