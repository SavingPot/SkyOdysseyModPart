using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using SP.Tools;
using SP.Tools.Unity;
using UnityEngine;

namespace GameCore
{
    public class BattleCube : Block
    {
        static string[] entitiesSpawnable;
        Entity[] entitiesSummoned;



        internal static void Init()
        {
            var entitiesSpawnableTemp = new List<string>();

            foreach (var mod in ModFactory.mods)
            {
                foreach (var entity in mod.entities)
                {
                    if (entity.behaviourType != null && entity.behaviourType.IsSubclassOf(typeof(Enemy)))
                    {
                        entitiesSpawnableTemp.Add(entity.id);
                    }
                }
            }

            entitiesSpawnable = entitiesSpawnableTemp.ToArray();
        }

        public override bool PlayerInteraction(Player player)
        {
            //检查是否激活过了
            if (entitiesSummoned != null)
                return false;

            var entity = entitiesSpawnable.Extract(Tools.staticRandom);
            var entityCount = Tools.staticRandom.Next(2, 6);

            //生成实体
            entitiesSummoned = new Entity[entityCount];
            WaitForEntitiesDeath();
            for (int i = 0; i < entityCount; i++)
            {
                var index = i;
                GM.instance.SummonEntityCallback(pos.To3(), entity, entity =>
                {
                    entitiesSummoned[index] = entity;
                });
            }

            return true;
        }

        async void WaitForEntitiesDeath()
        {
            //等待所有实体到位（必须使用 for 并且实时获取元素）
            for (int i = 0; i < entitiesSummoned.Length; i++)
                await UniTask.WaitUntil(() => entitiesSummoned[i] != null);

            //等待所有实体死亡
            foreach (var entity in entitiesSummoned)
                await UniTask.WaitUntil(() => !entity || entity.isDead);

            //给予战利品并销毁方块
            GM.instance.SummonCoinEntity(pos.To3(), 30);
            RemoveSelf();
        }
    }
}