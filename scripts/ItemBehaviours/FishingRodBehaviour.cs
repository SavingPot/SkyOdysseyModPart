using GameCore.High;
using Newtonsoft.Json.Linq;
using SP.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    [ItemBinding(ItemID.FishingRod)]
    public class FishingRodBehaviour : ItemBehaviour
    {
        FishingFloat fishingFloat;
        LineRenderer lineRenderer;
        const int lineSampleCount = 20;

        public override bool Use(Vector2 point)
        {
            bool baseUse = base.Use(point);

            if (baseUse)
                return baseUse;

            if (fishingFloat)
            {
                //收竿
                fishingFloat.Death();
                fishingFloat = null;
                lineRenderer.startWidth = 0;
                lineRenderer.endWidth = 0;

                //播放收竿动画
                if (owner is Player player) player.animWeb.SwitchPlayingTo("slight_rightarm_lift");
            }
            else
            {
                //计算抛出的速度
                var velocity = AngleTools.GetAngleVector2(owner.transform.position, point).normalized * 10;

                //播放甩竿动画
                if (owner is Player player) player.animWeb.SwitchPlayingTo("attack_rightarm", 0);

                //生成浮标
                GM.instance.SummonEntityCallback(owner.transform.position, EntityID.FishingFloat, entity =>
                {
                    //设置浮标的速度
                    fishingFloat = (FishingFloat)entity;
                    fishingFloat.ServerSetVelocity(velocity);

                    //显示鱼线
                    lineRenderer.startWidth = 0.1f;
                    lineRenderer.endWidth = 0.1f;
                });
            }

            return true;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            lineRenderer = new GameObject("fishing_line_renderer").AddComponent<LineRenderer>();
            lineRenderer.material = GInit.instance.spriteLitMat;
            lineRenderer.positionCount = lineSampleCount;
        }

        public override void OnExit()
        {
            base.OnExit();

            GameObject.Destroy(lineRenderer.gameObject);
            if (fishingFloat) fishingFloat.Death();
        }

        public override void OnHand()
        {
            base.OnHand();

            if (fishingFloat)
            {
                UpdateFishline();
            }
        }

        /// <summary>
        /// 更新鱼线
        /// </summary>
        void UpdateFishline()
        {
            //获取鱼线的起点和终点
            var origin = owner.transform.position;
            var dest = fishingFloat.transform.position;
            var xLength = dest.x - origin.x;
            var yLength = dest.y - origin.y;

            //采样
            Vector3[] samples = new Vector3[lineSampleCount];
            samples[0] = origin;
            samples[^1] = dest;
            for (int i = 1; i < lineSampleCount - 1; i++)
            {
                var rad = i / (float)lineSampleCount * Mathf.PI * 0.5f; // 0~90度
                samples[i] = new(origin.x + xLength * Mathf.Sin(rad), origin.y + yLength * (i / (float)lineSampleCount), origin.z);
            }

            //绘制鱼线
            lineRenderer.SetPositions(samples);
        }

        public FishingRodBehaviour(IInventoryOwner owner, Item data, string inventoryIndex) : base(owner, data, inventoryIndex)
        {

        }
    }
}
