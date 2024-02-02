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