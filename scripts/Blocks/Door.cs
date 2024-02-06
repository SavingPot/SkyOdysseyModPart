using GameCore.High;
using Newtonsoft.Json.Linq;
using SP.Tools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameCore
{
    public class Door : Block, IGroupBlock
    {
        public Vector2Int offset { get; set; }
        public bool isGroupCenter { get; set; }
        public Vector2Int[] groupPosTemp { get; set; }

        public override void DoStart()
        {
            base.DoStart();

            GroupBlockBehaviour.DoStart(this);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            GroupBlockBehaviour.OnUpdate(this);
        }

        public void UpdateWithoutSpread()
        {
            var texture = isGroupCenter ? $"{data.defaultTexture.id}_down" : $"{data.defaultTexture.id}_up";

            //如果门是开的
            if (blockCollider.isTrigger)
                texture = $"{texture}_opened";

            sr.sprite = ModFactory.CompareTexture(texture).sprite;
        }

        public override bool PlayerInteraction(Player player)
        {
            blockCollider.isTrigger = !blockCollider.isTrigger;

            foreach (var group in groupPosTemp)
            {
                var child = Map.instance.GetBlock(group, isBackground);

                if (child != null)
                    child.blockCollider.isTrigger = blockCollider.isTrigger;
            }

            OnUpdate();

            return true;
        }
    }
}
