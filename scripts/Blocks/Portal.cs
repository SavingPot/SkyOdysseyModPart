using DG.Tweening.Core;
using UnityEngine;
using System.Collections.Generic;
using SP.Tools;
using Newtonsoft.Json.Linq;
using DG.Tweening;
using GameCore.High;
using SP.Tools.Unity;

namespace GameCore
{
    public class Portal : Block
    {
        public bool generatedGuard;



        //TODO: 把方块变成 ActivatedPortal
        public override bool PlayerInteraction(Player player)
        {
            if (!generatedGuard)
            {
                GM.instance.SummonEntity(new(pos.x, pos.y + 1), $"{GFiles.world.GetOrAddRegion(chunk.regionIndex).biomeId}_biome_guard");
                generatedGuard = true;
                return true;
            }
            else
            {
                return base.PlayerInteraction(player);
            }
        }
    }
}