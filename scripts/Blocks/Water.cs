using Cysharp.Threading.Tasks.Triggers;
using GameCore.High;
using GameCore.Network;
using Newtonsoft.Json.Linq;
using Random = UnityEngine.Random;
using SP.Tools.Unity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine;

namespace GameCore
{
    public static class WaterCenter
    {
        static readonly List<Action> operationsToExecute = new();
        public static List<Water> waterBlocks = new();
        public static float physicsTimer;
        public static float streamSpeed = 0.2f;

        public static void Add(Water water)
        {
            waterBlocks.Add(water);
        }

        public static void Remove(Water water)
        {
            waterBlocks.Remove(water);
        }

        public static void WriteAllCustomDataToSave()
        {
            foreach (var water in waterBlocks)
            {
                if (water.customData == null)
                    continue;

                water.customData["ori:water_filled_level"] = water.filledLevel;
                water.WriteCustomDataToSave();
            }
        }

        /// <returns>origin 是否流完了</returns>
        public static bool StreamIntoWater(Water origin, Water target)
        {
            Interlocked.Increment(ref target.filledLevel);
            Interlocked.Decrement(ref origin.filledLevel);

            operationsToExecute.Add(() =>
            {
                target.OnUpdate();

                if (origin.filledLevel <= 0)
                {
                    //如果流尽了就删除
                    origin.RemoveFromMap();
                }
                else
                {
                    //如果没流尽就更新
                    origin.OnUpdate();
                }
            });

            return origin.filledLevel <= 0;
        }

        /// <returns>origin 是否流完了</returns>
        public static bool StreamIntoAir(Water origin, Vector2Int target)
        {
            var jo = new JObject();
            jo.AddProperty("ori:water_filled_level", 1);

            Interlocked.Decrement(ref origin.filledLevel);

            operationsToExecute.Add(() =>
            {
                Map.instance.SetBlockNet(target, origin.isBackground, BlockID.Water, jo.ToString());

                if (origin.filledLevel <= 0)
                {
                    //如果流尽了就删除
                    origin.RemoveFromMap();
                }
                else
                {
                    //如果没流尽就更新
                    origin.OnUpdate();
                }
            });

            return origin.filledLevel <= 0;
        }

        /// <returns>origin 是否流完了</returns>
        public static bool StreamIntoBlock(Water origin, Vector2Int target)
        {
            var jo = new JObject();
            jo.AddProperty("ori:water_filled_level", 1);

            Interlocked.Decrement(ref origin.filledLevel);

            operationsToExecute.Add(() =>
            {
                //摧毁原来的方块
                if (Map.instance.TryGetBlock(target, origin.isBackground, out Block block))
                {
                    block.Destroy();
                }
                //放水
                Map.instance.SetBlockNet(target, origin.isBackground, BlockID.Water, jo.ToString());

                if (origin.filledLevel <= 0)
                {
                    //如果流尽了就删除
                    origin.RemoveFromMap();
                }
                else
                {
                    //如果没流尽就更新
                    origin.OnUpdate();
                }
            });

            return origin.filledLevel <= 0;
        }

        //TODO: 水物理十分耗时
        public static void WaterPhysics()
        {
            //如果当前区域不存在或未生成完, 不会执行水物理
            if (Server.isServer && Tools.time >= physicsTimer && !GM.instance.generatingExistingRegion)
            {
                physicsTimer = Tools.time + streamSpeed;

                //执行水物理
                Parallel.ForEach(waterBlocks, SingleWaterPhysics);

                //执行水物理留下的待执行操作
                foreach (var item in operationsToExecute)
                    item();

                //清除待执行操作
                operationsToExecute.Clear();
            }
        }

        //TODO: 保存水的 customData, 同时发送给客户端 (NMSetBlockCustomData)
        public static void SingleWaterPhysics(Water water)
        {
            //TODO: 冲走可以被冲走的小方块
            //向下流
            var downBlock = water.chunk.map.GetBlock(water.posTempDown, water.isBackground);
            if (downBlock == null)
            {
                if (StreamIntoAir(water, water.posTempDown))
                    return;
            }
            else if (downBlock.data.HasTag("ori:plant"))
            {
                if (StreamIntoBlock(water, water.posTempDown))
                    return;
            }
            else
            {
                if (downBlock is Water downWater && downWater.filledLevel < 8)
                {
                    if (StreamIntoWater(water, downWater))
                        return;
                }
                else
                {
                    var leftBlock = water.chunk.map.GetBlock(water.posTempLeft, water.isBackground);
                    var rightBlock = water.chunk.map.GetBlock(water.posTempRight, water.isBackground);

                    //向左流
                    if (leftBlock == null && water.filledLevel > 0)
                    {
                        if (StreamIntoAir(water, water.posTempLeft))
                            return;
                    }
                    else if (leftBlock != null && leftBlock.data.HasTag("ori:plant") && water.filledLevel > 0)
                    {
                        if (StreamIntoBlock(water, water.posTempLeft))
                            return;
                    }
                    else if (leftBlock is Water leftWater && water.filledLevel > leftWater.filledLevel)
                    {
                        if (StreamIntoWater(water, leftWater))
                            return;
                    }

                    //向右流
                    if (rightBlock == null && water.filledLevel > 0)
                    {
                        if (StreamIntoAir(water, water.posTempRight))
                            return;
                    }
                    else if (rightBlock != null && rightBlock.data.HasTag("ori:plant") && water.filledLevel > 0)
                    {
                        if (StreamIntoBlock(water, water.posTempRight))
                            return;
                    }
                    else if (rightBlock is Water rightWater && water.filledLevel > rightWater.filledLevel)
                    {
                        if (StreamIntoWater(water, rightWater))
                            return;
                    }
                }
            }
        }
    }

    public class Water : Block
    {
        public static float entityFallingSpeed = 2;
        public static float resistance = 1.8f;
        public static float creatureSwimmingSpeed = 7;

        public Vector2Int posTempDown;
        public Vector2Int posTempLeft;
        public Vector2Int posTempRight;

        public int filledLevel = 8;

        public override void DoStart()
        {
            base.DoStart();

            filledLevel = customData?["ori:water_filled_level"]?.ToObject<int>() ?? 8;

            sr.sortingOrder = 10;
            sr.color = new(1, 1, 1, 0.4f);

            posTempDown = new(pos.x, pos.y - 1);
            posTempLeft = new(pos.x - 1, pos.y);
            posTempRight = new(pos.x + 1, pos.y);


            WaterCenter.Add(this);
        }

        public override void OnRecovered()
        {
            sr.sortingOrder = 1;

            WaterCenter.Remove(this);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (filledLevel != 0)
                sr.sprite = ModFactory.CompareTexture($"ori:water_{filledLevel}").sprite;
        }

        public override void OnEntityStay(Entity entity)
        {
            if (entity.rb && entity.rb.velocity.y != 0)
            {
                if (entity.rb.velocity.y < -entityFallingSpeed)
                {
                    entity.AddVelocityY(resistance);
                }
                else if (entity.rb.velocity.y > -entityFallingSpeed)
                {
                    entity.AddVelocityY(-resistance);
                }
            }

            if (entity is Creature creature)
            {
                creature.fallenY = entity.transform.position.y;

                //TODO: 任一实体的游泳
                if (entity.isPlayer)
                {
                    var player = (Player)entity;

                    if (Player.PlayerCanControl(player) && player.playerController.HoldingJump())
                    {
                        if (player.rb.velocity.y < creatureSwimmingSpeed)
                        {
                            player.AddVelocityY(resistance * 2f);

                            string swimAudio = Random.value switch
                            {
                                < 0.25f => AudioID.Swim0,
                                < 0.5f => AudioID.Swim1,
                                < 0.75f => AudioID.Swim2,
                                _ => AudioID.Swim3,
                            };

                            if (GAudio.GetAudio(swimAudio).sources.Count == 0)
                            {
                                GAudio.Play(swimAudio, true);
                            }
                        }
                        else if (player.rb.velocity.y > creatureSwimmingSpeed)
                        {
                            player.AddVelocityY(-resistance * 2f);
                        }
                    }
                }
            }
        }

        public override void OnEntityExit(Entity entity)
        {
            base.OnEntityExit(entity);

            if (entity.isLocalPlayer)
            {
                GAudio.Play(Random.Range(0, 2) switch
                {
                    0 => AudioID.SwimExit0,
                    1 => AudioID.SwimExit1,
                    _ => throw new(),
                });
            }
        }
    }
}
