using UnityEngine;
using SP.Tools.Unity;
using System.Linq;
using System.Collections.Generic;
using GameCore.UI;
using SP.Tools;

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
                    caller.fallenY = caller.transform.position.y;
                }
            };


            //注册玩家的远程命令
            //注意：以后可能得为所有 Entity 都注册，毕竟 Entity 以后可能也可以使用魔法
            PlayerCenter.OnAddPlayer += player =>
            {
                player.RegisterParamRemoteCommand(LaserSpellBehaviour.LaserLightCommandId, LaserSpellBehaviour.LaserLight);

                if (player.isLocalPlayer && player.isServer)
                {
                    //解锁技能时刷新一下农作物装饰器
                    player.pui.Backpack.OnUnlockSkill += skill =>
                    {
                        //TODO: 解锁锄头合成配方
                        CropBlock.GetCrop();
                    };
                }
            };




            //注册更新
            GM.OnUpdate += WaterCenter.WaterPhysics;
            GM.OnUpdate += FightingMusic;
            GM.OnUpdate += PlantCenter.Update;



            //保存事件
            World.ApplyBlockData += WaterCenter.WriteAllCustomDataToSave;



            //添加物品信息修改器
            Item.infoModifiersForId.Add(ItemID.ManaStone, item =>
            {
                var spellId = ISpellContainer.GetSpellIdFromJObject(item.customData);

                if (spellId == null)
                    return $"魔咒: 空";
                else
                    return $"魔咒: {spellId}";
            });
            Item.infoModifiersForId.Add(ItemID.SpellManuscript, item =>
            {
                var spellId = ISpellContainer.GetSpellIdFromJObject(item.customData);

                if (spellId == null)
                    return $"魔咒: 空";
                else
                    return $"魔咒: {spellId}";
            });
            Item.infoModifiersForId.Add(ItemID.SkillManuscript, item =>
            {
                //这个检查极少失败，因为通常只有本地玩家会用到 infoModifiersForId
                if (!Player.TryGetLocal(out Player player))
                {
                    Debug.LogError($"没有本地玩家");
                    return null;
                }

                var id = SkillManuscriptBehaviour.GetSkillId(item.customData);
                return id.IsNullOrWhiteSpace() ? $"技能: 空" : $"技能: {GameUI.CompareText(player.pui.Backpack.skillNodeTree.GetNodeButtonId(id) + ".text")}";
            });
            Item.infoModifiersForTag.Add("ori:edible", item => $"回血: {item.data.Edible().tagValue}");
            Item.infoModifiersForTag.Add("ori:bait", item => $"鱼饵: {item.data.GetValueTagToInt("ori:bait").tagValue}");



            //绑定场景切换事件
            GScene.AfterChanged += scene =>
            {
                //清理对象池
                LeaveRendererPool.Reset();
                LaserLightPool.Reset();

                switch (scene.name)
                {
                    //TODO: 调整音频播放位置
                    case SceneNames.MainMenu:
                        GAudio.Play(AudioID.Town, null);
                        GAudio.Stop(AudioID.WhyNotComeToTheParty);
                        GAudio.Stop(AudioID.Skirmish0);
                        break;

                    case SceneNames.GameScene:
                        GAudio.Stop(AudioID.Town);
                        GAudio.Play(AudioID.WhyNotComeToTheParty, null);

                        //初始化 UI
                        Doorplate.InitUI();
                        break;
                }
            };

            Debug.Log("OriginModEntry Loaded");
        }

        void FightingMusic()
        {
            if (!Player.TryGetLocal(out Player player))
                return;

            bool enemyTargetingPlayer = EnemyCenter.all.Any(enemy => enemy.targetEntity == player && Tools.instance.IsInView2DFaster(enemy.transform.position));

            if (GAudio.GetAudio(AudioID.Skirmish0).sources.Any(source => source.isPlaying))
            {
                if (!enemyTargetingPlayer)
                {
                    GAudio.Stop(AudioID.Skirmish0);
                    GAudio.Play(AudioID.WhyNotComeToTheParty, null);
                }
            }
            else
            {
                if (enemyTargetingPlayer)
                {
                    GAudio.Stop(AudioID.WhyNotComeToTheParty);
                    GAudio.Play(AudioID.Skirmish0, null);
                }
            }
        }

        int CountOfEnemyTargetingPlayer(Player player)
        {
            int result = 0;

            foreach (var enemy in EnemyCenter.all)
                if (enemy.targetEntity == player)
                    result++;

            return result;
        }

        public override void OnAllModsLoaded()
        {
            base.OnAllModsLoaded();

            BattleCube.Init();
        }
    }
}
