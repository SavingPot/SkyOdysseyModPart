using UnityEngine;
using SP.Tools.Unity;

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

            PlayerCenter.OnAddPlayer += player =>
            {
                player.RegisterParamRemoteCommand(LaserSpellBehaviour.LaserLightCommandId, LaserSpellBehaviour.LaserLight);
            };

            GM.OnUpdate += WaterCenter.WaterPhysics;

            GScene.AfterChanged += scene =>
            {
                //清理对象池
                LaserLightPool.stack.Clear();

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
