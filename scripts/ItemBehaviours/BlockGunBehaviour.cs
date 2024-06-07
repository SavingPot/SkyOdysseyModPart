using GameCore.High;
using SP.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    [ItemBinding(ItemID.BlockGun)]
    public class BlockGunBehaviour : ItemBehaviour
    {
        static readonly float shootDistance = 10f;



        public override bool Use(Vector2 point)
        {
            //向指针方向发射出长度为 10 的射线
            var startPoint = (Vector2)owner.transform.position;
            var direction = AngleTools.GetAngleVector2(startPoint, point).normalized;
            var hit = RayTools.Hit(startPoint, direction, shootDistance, Block.blockLayerMask);

            //找到光线的终止点
            var endPoint = hit.collider ? hit.point + hit.normal * 0.3f :   // hit.normal 时射线与碰撞体的法线向量，其指向碰撞体的外侧，我们可以通过它来使 endPoint 不处在碰撞体内
                                          startPoint + direction * shootDistance;

            //TODO: Line renderer

            //防止方块
            var blockPos = PosConvert.WorldToMapPos(endPoint);
            Map.instance.SetBlockNet(blockPos, false, BlockID.OakPlanks, null);

            return true;
        }



        public BlockGunBehaviour(IInventoryOwner owner, Item instance, string inventoryIndex) : base(owner, instance, inventoryIndex) { }
    }
}
