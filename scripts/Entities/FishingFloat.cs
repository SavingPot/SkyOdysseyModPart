using DG.Tweening;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.FishingFloat)]
    public sealed class FishingFloat : Entity
    {
        internal float lastTimeHookedUp;

        public override void Initialize()
        {
            base.Initialize();

            //添加贴图
            AddSpriteRenderer("ori:fishing_float");
        }

        void HookingUp()
        {
            Debug.Log("Hooking up");
            lastTimeHookedUp = Tools.time;
            rb.AddVelocityY(5);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            rb.SetVelocityX(Mathf.Lerp(rb.velocity.x, 0, 1.2f * Time.fixedDeltaTime));
        }

        protected override void OnBlockEnter(Block block)
        {
            base.OnBlockEnter(block);

            if (block.data.id == BlockID.Water)
            {
                RandomUpdater.Bind("ori:fishing_float", 30, HookingUp);
            }
        }

        protected override void OnBlockStay(Block block)
        {
            base.OnBlockStay(block);

            if (block.data.id == BlockID.Water)
            {
                bool hasUpperWater = block.chunk.map.TryGetBlock(new(block.pos.x, block.pos.y + 1), block.isBackground, out var upper) &&
                                     upper.data.id == BlockID.Water;

                //如果到达了水面, 那么不动
                if (!hasUpperWater && transform.position.y - block.pos.y > 0f)
                    rb.SetVelocityY(Mathf.Lerp(rb.velocity.y, 0, 0.2f * Time.fixedDeltaTime));

                //上钩五秒内不动
                if (Tools.time > lastTimeHookedUp + 1f)
                    rb.AddVelocityY(2.3f);
            }
        }

        protected override void OnBlockExit(Block block)
        {
            base.OnBlockExit(block);

            if (block.data.id == BlockID.Water)
            {
                RandomUpdater.Unbind("ori:fishing_float");
            }
        }
    }
}
