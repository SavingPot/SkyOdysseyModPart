using UnityEngine;
using static GameCore.Player;

namespace GameCore
{
    public class ExtinguishedCampfire : Block
    {
        public override bool PlayerInteraction(Player caller)
        {
            //替换为燃烧的篝火
            chunk.map.SetBlockNet(pos, isBackground, BlockStatus.Normal, BlockID.Campfire, null);
            GAudio.Play(AudioID.Ignite, pos, true);

            return true;
        }
    }
}