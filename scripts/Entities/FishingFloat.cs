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
        float lastTimeHookedUp;

        public override void Initialize()
        {
            base.Initialize();

            //添加贴图
            AddSpriteRenderer("ori:fishing_float");
        }

        public override void OnDeathServer()
        {
            base.OnDeathServer();

            //在 3 秒内收竿
            if (Tools.time < lastTimeHookedUp + 3)
            {
                Debug.Log("FishingFloat died");
                GM.instance.SummonDrop(transform.position, ItemID.Cod);
            }
        }

        void HookingUp()
        {
            Debug.Log("Hooking up");
            lastTimeHookedUp = Tools.time;
        }

        protected override void OnBlockEnter(Block block)
        {
            base.OnBlockEnter(block);

            if (block.data.id == BlockID.Water)
            {
                RandomUpdater.Bind("ori:fishing_float", 50, HookingUp);
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
