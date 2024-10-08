using UnityEngine;
using static GameCore.Player;

namespace GameCore
{
    public class Campfire : AnimatedBlock
    {
        public static uint campfireCount;

        public override void DoStart()
        {
            base.DoStart();

            campfireCount++;
            GAudio.Play(AudioID.Campfire, pos, true);
        }

        public override void OnRecovered()
        {
            base.OnRecovered();

            if (campfireCount == 1)
                GAudio.Stop(AudioID.Campfire);

            campfireCount--;
        }

        public override bool PlayerInteraction(Player caller)
        {
            //替换为熄灭的篝火
            chunk.map.SetBlockNet(pos, isBackground, BlockStatus.Normal, BlockID.ExtinguishedCampfire, null);
            GAudio.Play(AudioID.Smother, pos, true);

            return true;
        }
    }
}