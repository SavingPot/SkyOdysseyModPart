using UnityEngine;
using SP.Tools.Unity;
using System.Linq;

namespace GameCore
{
    public class OriginModEntry : ModEntry
    {
        public override void OnLoaded()
        {
            base.OnLoaded();



            //让玩家穿着羽翼时重力降低
            Player.GravitySet += caller =>
            {
                if (caller.inventory.breastplate?.data?.id == ItemID.FeatherWing)
                {
                    caller.gravity *= 0.7f;
                }
            };


            //注册玩家的远程命令
            //注意：以后可能得为所有 Entity 都注册，毕竟 Entity 以后可能也可以使用魔法
            PlayerCenter.OnAddPlayer += player =>
            {
                player.RegisterParamRemoteCommand(LaserSpellBehaviour.LaserLightCommandId, LaserSpellBehaviour.LaserLight);

                if (player.isServer)
                {
                    //解锁技能时刷新一下农作物装饰器
                    player.pui.OnUnlockSkill += skill =>
                    {
                        foreach (var chunk in Map.instance.chunks)
                        {
                            foreach (var block in chunk.blocks)
                            {
                                if (block is CropBlock cropBlock)
                                {
                                    cropBlock.crop = CropBlock.GetCrop(cropBlock);
                                }
                            }

                        }
                    };
                }
            };




            //注册水物理
            GM.OnUpdate += WaterCenter.WaterPhysics;
            GM.OnUpdate += FightingMusic;



            //绑定场景切换事件
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

        void FightingMusic()
        {
            if (!Player.TryGetLocal(out Player player))
                return;

            int enemyTargetingPlayer = EnemyTargetingPlayer(player);

            if (GAudio.GetAudio(AudioID.Skirmish0).sources.Any(source => source.isPlaying))
            {
                if (enemyTargetingPlayer == 0)
                {
                    GAudio.Stop(AudioID.Skirmish0);
                    GAudio.Play(AudioID.WhyNotComeToTheParty);
                }
            }
            else
            {
                if (enemyTargetingPlayer > 0)
                {
                    GAudio.Stop(AudioID.WhyNotComeToTheParty);
                    GAudio.Play(AudioID.Skirmish0);
                }
            }
        }

        int EnemyTargetingPlayer(Player player)
        {
            int result = 0;

            foreach (var enemy in EnemyCenter.all)
                if (enemy.targetEntity == player)
                    result++;

            return result;
        }
    }
}
