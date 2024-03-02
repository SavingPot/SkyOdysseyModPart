using UnityEngine;

namespace GameCore
{
    public class BoundaryGate : Block
    {
        public override bool PlayerInteraction(Player player)
        {
            //获取在本区域中的位置
            var posInRegion = chunk.MapToRegionPos(pos);

            //计算出差值
            Vector2Int indexDelta = posInRegion switch
            {
                { x: var x, y: var y } when x < 0 && y == 0 => new(-1, 0), //左
                { x: var x, y: var y } when x > 0 && y == 0 => new(1, 0),  //右
                { x: var x, y: var y } when x == 0 && y > 0 => new(0, 1),  //上
                { x: var x, y: var y } when x == 0 && y < 0 => new(0, -1), //下
                _ => throw new()
            };

            if (!player.askingForGeneratingRegion)
            {
                //生成沙盒
                player.ServerGenerateRegion(chunk.regionIndex + indexDelta, player.generatedFirstRegion, null);
            }

            return true;
        }
    }
}