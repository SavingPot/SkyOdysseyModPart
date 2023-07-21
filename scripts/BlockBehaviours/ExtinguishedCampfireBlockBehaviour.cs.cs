using UnityEngine;
using static GameCore.Player;

namespace GameCore
{
    public class ExtinguishedCampfireBlockBehaviour : Block
    {
        public override bool PlayerInteraction(Player caller)
        {
            //替换为燃烧的篝火
            chunk.map.SetBlock(pos, layer, ModFactory.CompareBlockDatum(BlockID.Campfire), null, true);
            GAudio.Play(AudioID.Ignite, true);

            return true;
        }
    }
}