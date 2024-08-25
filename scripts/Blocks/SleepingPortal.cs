using System.Collections;
using System.Linq;
using SP.Tools.Unity;
using UnityEngine;

namespace GameCore
{
    public sealed class SleepingPortal : ChallengeEntrance
    {
        string GetGuardId() => $"{GFiles.world.GetOrAddRegion(chunk.regionIndex).biomeId}_guard";



        //TODO: 击败后把方块变成 ActivatedPortal
        public override bool PlayerInteraction(Player player)
        {
            if (hasChallengeBegun)
                return false;

            //检查 x轴
            if (chunk.regionIndex.x == 0)
            {
                Debug.LogError($"沉睡的传送门不应出现在中心区域");
                return true;
            }

            //检查守护者
            if (ModFactory.CompareEntity(GetGuardId()) == null)
            {
                Debug.LogError($"守护者召唤失败：无法匹配到实体 {GetGuardId()}");
                return true;
            }

            //开始挑战
            StartChallenge(player, null);
            return true;
        }

        protected override IEnumerator WaitForChallengeRoom(Player player)
        {
            yield return base.WaitForChallengeRoom(player);

            //生成守护者
            GM.instance.SummonEntityCallback(Region.GetMiddle(player.regionIndex), GetGuardId(), entity => CoroutineStarter.Do(WaitForChallengeEnd(player, (BiomeGuard)entity)));
        }

        IEnumerator WaitForChallengeEnd(Player player, BiomeGuard guard)
        {
            while (player.regionIndex.y == ChallengeRoomGeneration.challengeRoomIndexY)
            {
                //若玩家死亡那么不等了
                if (player.isDead)
                {
                    hasChallengeBegun = false;
                    yield break;
                }

                //若守护者死亡，则挑战成功
                if (guard.isDead) guard = null;
                if (guard == null)
                {
                    //送玩家回到原本的区域
                    var portalRegion = PosConvert.MapPosToRegionIndex(pos);
                    player.GenerateRegion(portalRegion, true);

                    //等待区域生成
                    while (!GM.instance.generatedExistingRegions.Any(r => r.index == portalRegion))
                        yield return null;

                    //激活传送门
                    Map.instance.SetBlockNet(pos, isBackground, BlockID.Portal, null);
                    yield break;
                }
                //如果还在挑战那么继续等待
                else
                {
                    yield return null;
                }
            }

            //玩家意外离开了挑战区
            hasChallengeBegun = false;
        }
    }
}