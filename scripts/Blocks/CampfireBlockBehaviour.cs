using UnityEngine;
using static GameCore.Player;

namespace GameCore
{
    public class CampfireBlockBehaviour : AnimatedBlock
    {
        public static uint campfireCount;

        public override void DoStart()
        {
            base.DoStart();

            campfireCount++;
            GAudio.Play(AudioID.Campfire, true);
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
            chunk.map.SetBlock(pos, isBackground, ModFactory.CompareBlockDatum(BlockID.ExtinguishedCampfire), null, true);
            GAudio.Play(AudioID.Smother, true);

            return true;
        }
    }
}