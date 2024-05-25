using System.Collections;
using GameCore.High;
using UnityEngine;

namespace GameCore
{
    public class BerryBush : Plant
    {
        public override bool PlayerInteraction(Player caller)
        {
            //在方块的位置生成物品
            GM.instance.SummonDrop(transform.position, ItemID.Berry);

            //播放采摘音效
            GAudio.Play(AudioID.PickBerryBush);

            //修改方块
            chunk.map.SetBlockNet(pos, isBackground, BlockID.PickedBerryBush, null);

            return true;
        }
    }
}