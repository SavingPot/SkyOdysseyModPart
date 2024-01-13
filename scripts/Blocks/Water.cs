using Cysharp.Threading.Tasks.Triggers;
using GameCore.High;
using Newtonsoft.Json.Linq;
using SP.Tools.Unity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace GameCore
{
    public static class WaterCenter
    {
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

        public static bool StreamIntoWater(Water origin, Water target)
        {
            target.filledLevel += 1;
            origin.filledLevel -= 1;

            MethodAgent.QueueOnMainThread(() => target.OnUpdate());

            if (origin.filledLevel <= 0)
            {
                //如果流尽了就删除
                MethodAgent.QueueOnMainThread(() => Map.instance.DestroyBlockNet(origin.pos, origin.isBackground));
                return true;
            }
            else
            {
                MethodAgent.QueueOnMainThread(() => origin.OnUpdate());
                return false;
            }
        }

        public static bool StreamIntoAir(Water origin, Vector2Int target)
        {
            var jo = new JObject();
            jo.AddProperty("ori:water_filled_level", 1);

            origin.filledLevel -= 1;

            MethodAgent.QueueOnMainThread(() => Map.instance.SetBlockNet(target, origin.isBackground, BlockID.Water, jo.ToString()));

            if (origin.filledLevel <= 0)
            {
                //如果流尽了就删除
                MethodAgent.QueueOnMainThread(() => Map.instance.DestroyBlockNet(origin.pos, origin.isBackground));
                return true;
            }
            else
            {
                MethodAgent.QueueOnMainThread(() => origin.OnUpdate());
                return false;
            }
        }

        //TODO: 水物理十分耗时
        public static void WaterPhysics()
        {
            //如果当前区域不存在或未生成完, 不会执行水物理
            if (Server.isServer && Tools.time >= physicsTimer && !GM.instance.generatingExistingRegion)
            {
                physicsTimer = Tools.time + streamSpeed;

                Parallel.ForEach(waterBlocks, SingleWaterPhysics);
            }
        }

        //TODO: 保存水的 customData, 同时发送给客户端 (NMSetBlockCustomData)
        public static void SingleWaterPhysics(Water water)
        {
            //TODO: 冲走可以被冲走的小方块
            var downBlock = water.chunk.map.GetBlock(water.posTempDown, water.isBackground);
            if (downBlock == null)
            {
                if (StreamIntoAir(water, water.posTempDown))
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

                    if (leftBlock == null && water.filledLevel > 0)
                    {
                        if (StreamIntoAir(water, water.posTempLeft))
                            return;
                    }
                    else if (leftBlock is Water leftWater && leftWater.filledLevel < 8 && water.filledLevel > 0)
                    {
                        if (StreamIntoWater(water, leftWater))
                            return;
                    }

                    if (rightBlock == null && water.filledLevel > 0)
                    {
                        if (StreamIntoAir(water, water.posTempRight))
                            return;
                    }
                    else if (rightBlock is Water rightWater && rightWater.filledLevel < 8 && water.filledLevel > 0)
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

        public byte filledLevel = 8;

        public override void DoStart()
        {
            base.DoStart();

            filledLevel = customData?["ori:water_filled_level"]?.ToObject<byte>() ?? 8;

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

            if (filledLevel > 0)
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

                if (entity.isPlayer)
                {
                    var player = (Player)entity;

                    if (PlayerControls.HoldJump(player))
                    {
                        if (player.rb.velocity.y < creatureSwimmingSpeed)
                        {
                            player.AddVelocityY(resistance * 2f);
                        }
                        else if (player.rb.velocity.y > creatureSwimmingSpeed)
                        {
                            player.AddVelocityY(-resistance * 2f);
                        }
                    }
                }
            }
        }
    }
}
