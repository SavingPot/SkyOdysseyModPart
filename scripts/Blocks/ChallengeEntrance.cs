using SP.Tools.Unity;
using System.Collections;
using UnityEngine;

namespace GameCore
{
    public class ChallengeEntrance : Block
    {
        //TODO: 等待其他玩家加入
        protected bool hasChallengeBegun;

        protected virtual void StartChallenge(Player player, string challengeId)
        {
            hasChallengeBegun = true;
            player.GenerateChallengeRoom(challengeId);
            CoroutineStarter.Do(WaitForChallengeRoom(player));
        }

        protected virtual IEnumerator WaitForChallengeRoom(Player player)
        {
            while (player.regionIndex.y != ChallengeRoomGeneration.challengeRoomIndexY)
                yield return null;
        }
    }
}