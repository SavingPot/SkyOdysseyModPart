using GameCore.High;
using Newtonsoft.Json.Linq;
using SP.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GameCore
{
    public class Door : Block, IGroupBlock
    {
        public Vector2Int offset { get; set; }
        public bool isGroupCenter { get; set; }
        public Vector2Int[] groupPosTemp { get; set; }
        public bool hasBeenSpreadOnRecovered { get; set; }

        public override void DoStart()
        {
            base.DoStart();

            GroupBlockBehaviour.DoStart(this);
        }

        public override void OnUpdate()
        {
            GroupBlockBehaviour.SpreadOnUpdate(this);
        }

        public void UpdateWithoutSpread()
        {
            base.OnUpdate();

            //决定贴图
            var texture = isGroupCenter ? $"{data.defaultTexture.id}_down" : $"{data.defaultTexture.id}_up";

            //如果门是开的还要改变贴图
            if (blockCollider.isTrigger)
                texture = $"{texture}_opened";

            //设置贴图
            sr.sprite = ModFactory.CompareTexture(texture).sprite;
        }

        public override bool PlayerInteraction(Player player)
        {
            //自身先改
            blockCollider.isTrigger = !blockCollider.isTrigger;

            //播放开关门音效
            if (blockCollider.isTrigger)
                GAudio.Play(AudioID.OpenDoor);
            else
                GAudio.Play(AudioID.CloseDoor);

            //然后改组里的其他方块
            foreach (var groupPos in groupPosTemp)
            {
                var group = Map.instance.GetBlock(groupPos, isBackground);

                if (group != null)
                    group.blockCollider.isTrigger = blockCollider.isTrigger;
            }

            //更新贴图
            OnUpdate();

            return true;
        }

        public override void OnRecovered()
        {
            base.OnRecovered();

            //TODO: 为什么有的时候方块会无缘无故消失呢
            GroupBlockBehaviour.SpreadOnRecovered(this);
        }

        public void OnRecoveredWithoutSpread() => base.OnRecovered();

        public override void OutputDrops(Vector3 pos)
        {
            //清除自定义数据中的偏移信息
            if (customData?["offset"] != null)
                customData.Remove("offset");

            base.OutputDrops(pos);
        }
    }
}
