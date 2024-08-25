using System.Collections;
using SP.Tools.Unity;
using UnityEngine;

namespace GameCore
{
    public class SleepingPortal : ChallengeEntrance
    {
        string GetGuardId() => $"{GFiles.world.GetOrAddRegion(chunk.regionIndex).biomeId}_guard";

        //TODO: 击败后把方块变成 ActivatedPortal
        public override bool PlayerInteraction(Player player)
        {
            if (chunk.regionIndex.x == 0)
            {
                Debug.LogError($"沉睡的传送门不应出现在中心区域");
                return true;
            }

            if (ModFactory.CompareEntity(GetGuardId()) == null)
            {
                Debug.LogError($"守护者召唤失败：无法匹配到实体 {GetGuardId()}");
                return true;
            }

            player.GenerateChallengeRoom("");
            CoroutineStarter.Do(WaitForEnteringTheRoom(player));
            return true;
        }

        IEnumerator WaitForEnteringTheRoom(Player player)
        {
            while (player.regionIndex.y != ChallengeRoomGeneration.challengeRoomIndexY)
                yield return null;

            GM.instance.SummonEntity(Region.GetMiddle(player.regionIndex), GetGuardId());
        }
    }
}