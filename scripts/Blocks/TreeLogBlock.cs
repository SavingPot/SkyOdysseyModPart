using GameCore.High;
using UnityEngine;

namespace GameCore
{
    public class TreeLogBlock : Block
    {
        public SpriteRenderer leaveRenderer;

        public override void OnUpdate()
        {
            base.OnUpdate();

            RecoverLeaveRenderer();

            if (chunk.map.GetBlock(new(pos.x, pos.y + 1), isBackground)?.data?.id != data.id)
            {
                leaveRenderer = LitSpriteRendererPool.Get(sr.sortingOrder);
                leaveRenderer.sprite = ModFactory.CompareTexture("ori:oak_tree_leaf").sprite;
                leaveRenderer.transform.localPosition = new(pos.x, pos.y + 1.5f, 0);
            }
            else
            {

            }
        }

        public override void OnRecovered()
        {
            base.OnRecovered();



            //回收方块上方的木头
            if (Server.isServer)
            {
                var upperPosVector = new Vector2Int(pos.x, pos.y + 1);
                var upperBlock = Map.instance.GetBlock(upperPosVector, isBackground);

                //如果也是木头，就销毁
                if (upperBlock != null && upperBlock.data.id == data.id)
                {
                    upperBlock.Destroy();
                }
            }



            RecoverLeaveRenderer();
        }

        void RecoverLeaveRenderer()
        {
            if (leaveRenderer)
            {
                LitSpriteRendererPool.Recover(leaveRenderer);
                leaveRenderer = null;
            }
        }
    }
}