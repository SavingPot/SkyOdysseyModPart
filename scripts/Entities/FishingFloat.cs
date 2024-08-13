using GameCore.Network;
using Mirror;
using SP.Tools;
using SP.Tools.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.FishingFloat), NotSummonable]
    public sealed class FishingFloat : Entity
    {
        internal FishingRodBehaviour rod;
        internal float lastTimeHookedUp;
        bool hasBoundUpdate;
        const float BUOYANCY = 4;

        public override void Initialize()
        {
            base.Initialize();

            isHurtable = false;

            //添加贴图
            AddSpriteRenderer("ori:fishing_float");
        }

        void HookingUp()
        {
            Debug.Log("Hooking up");
            lastTimeHookedUp = Tools.time;
            transform.AddLocalPosY(0.3f);
            rb.AddVelocityY(5);
        }

        /// <summary>
        /// 必须得在服务器端调用，这样才可以获取世界数据
        /// </summary>
        [ServerRpc]
        internal void GetLoot(NetworkConnection caller = null)
        {
            //如果在 3 秒内收竿
            if (rod.TryGetBait(out var bait))
            {
                //扣除鱼饵
                rod.player.ServerReduceItemCount(bait.index.ToString(), 1);

                //获取战利品
                if (GFiles.world.TryGetRegion(PosConvert.WorldPosToRegionIndex(transform.position), out var region))
                {
                    //找到符合群系要求的结果
                    var biome = region.biomeId;
                    var availableResults = new List<FishingResult>();
                    var sum = 0f;
                    foreach (var r in ModFactory.globalFishingResults)
                    {
                        if (r.biome.IsNullOrWhiteSpace() || r.biome == biome)
                        {
                            availableResults.Add(r);
                            sum += r.probability;
                        }
                    }

                    //抽取隧机数，区间是 [O, sum)（要进行加权抽取）
                    var random = Random.Range(0, sum);
                    var currentRange = 0f;

                    //遍历每个区间。如果 random 在此区间内，返回此 item
                    foreach (var result in availableResults)
                    {
                        currentRange += result.probability;
                        if (random < currentRange)
                        {
                            GM.instance.SummonDrop(transform.position, result.result);
                        }
                    }
                }
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            //速度衰减
            rb.SetVelocityX(Mathf.Lerp(rb.velocity.x, 0, 1.2f * Time.fixedDeltaTime));
        }

        protected override void OnBlockStay(Block block)
        {
            base.OnBlockStay(block);

            //浸在水里
            if (block.data.id == BlockID.Water)
            {
                //绑定更新
                if (!hasBoundUpdate)
                {
                    hasBoundUpdate = true;
                    RandomUpdater.Bind("ori:fishing_float", GetHookingUpProbability(), HookingUp);
                }

                //漂浮效果
                float sizeY = mainCollider.size.y;
                bool hasUpperWater = block.chunk.map.TryGetBlock(new(block.pos.x, block.pos.y + 1), block.isBackground, out var upper) &&
                                     upper.data.id == BlockID.Water;
                float objectBottom = transform.position.y - sizeY * 0.5f;
                float waterLevel = hasUpperWater ? upper.pos.y : block.pos.y + 0.2f;

                //如果底部未到达水面, 那么上浮
                if (objectBottom < waterLevel)
                {
                    //计算浮力
                    float submergedPercentage = (waterLevel - objectBottom) / mainCollider.size.y;
                    float force = Mathf.Clamp(BUOYANCY * submergedPercentage, 0, BUOYANCY);

                    //应用浮力
                    rb.SetVelocityY(Mathf.Min(rb.velocity.y + force, BUOYANCY));
                }
            }
        }

        float GetHookingUpProbability()
        {
            return 30;
        }

        protected override void OnBlockExit(Block block)
        {
            base.OnBlockExit(block);

            if (block.data.id == BlockID.Water)
            {
                if (hasBoundUpdate)
                {
                    hasBoundUpdate = false;
                    RandomUpdater.Unbind("ori:fishing_float");
                }
            }
        }
    }
}
