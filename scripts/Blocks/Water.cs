using Cysharp.Threading.Tasks.Triggers;
using GameCore.High;
using Newtonsoft.Json.Linq;
using SP.Tools.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameCore
{
    public static class WaterCenter
    {
        public static List<Water> waterBlocks = new();
        public static float physicsTimer;
        public static float streamSpeed = 0.15f;

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

            target.OnUpdate();

            if (origin.filledLevel <= 0)
            {
                //如果流尽了就删除
                Map.instance.DestroyBlockNet(origin.pos, origin.isBackground);
                return true;
            }
            else
            {
                origin.OnUpdate();
                return false;
            }
        }

        public static bool StreamIntoAir(Water origin, Vector2Int target)
        {
            var jo = new JObject();
            jo.AddProperty("ori:water_filled_level", 1);

            Map.instance.SetBlockNet(target, origin.isBackground, BlockID.Water, jo.ToString());

            origin.filledLevel -= 1;


            if (origin.filledLevel <= 0)
            {
                //如果流尽了就删除
                Map.instance.DestroyBlockNet(origin.pos, origin.isBackground);
                return true;
            }
            else
            {
                origin.OnUpdate();
                return false;
            }
        }

        //TODO: 水物理十分耗时
        public static void WaterPhysics()
        {
            if (Server.isServer && Tools.time >= physicsTimer && !GM.instance.generatingExistingRegion)
            {
                physicsTimer = Tools.time + streamSpeed;

                //如果当前区域不存在未生成完, 不会执行物理
                foreach (var water in waterBlocks)
                {
                    //TODO: 冲走可以被冲走的小方块
                    var downBlock = water.GetBlock(water.posTempDown);
                    if (downBlock == null)
                    {
                        if (StreamIntoAir(water, water.posTempDown))
                            continue;
                    }
                    else
                    {
                        if (downBlock is Water downWater && downWater.filledLevel < 8)
                        {
                            if (StreamIntoWater(water, downWater))
                                continue;
                        }
                        else
                        {
                            var leftBlock = water.GetBlock(water.posTempLeft);
                            var rightBlock = water.GetBlock(water.posTempRight);

                            if (leftBlock == null && water.filledLevel > 0)
                            {
                                if (StreamIntoAir(water, water.posTempLeft))
                                    continue;
                            }
                            else if (leftBlock is Water leftWater && leftWater.filledLevel < 8 && water.filledLevel > 0)
                            {
                                if (StreamIntoWater(water, leftWater))
                                    continue;
                            }

                            if (rightBlock == null && water.filledLevel > 0)
                            {
                                if (StreamIntoAir(water, water.posTempRight))
                                    continue;
                            }
                            else if (rightBlock is Water rightWater && rightWater.filledLevel < 8 && water.filledLevel > 0)
                            {
                                if (StreamIntoWater(water, rightWater))
                                    continue;
                            }
                        }
                    }
                }
            }
        }
    }

    public class Water : Block
    {
        public static float entityFallingSpeed = 2;
        public static float resistance = 1.75f;

        public Vector2Int posTempDown;
        public Vector2Int posTempLeft;
        public Vector2Int posTempRight;
        public bool isOnChunkBoundary;
        public byte filledLevel = 8;//TODO

        public override void DoStart()
        {
            base.DoStart();

            filledLevel = customData?["ori:water_filled_level"]?.ToObject<byte>() ?? 8;

            sr.color = new(1, 1, 1, 0.4f);

            posTempDown = new(pos.x, pos.y - 1);
            posTempLeft = new(pos.x - 1, pos.y);
            posTempRight = new(pos.x + 1, pos.y);
            isOnChunkBoundary = Mathf.Max(Mathf.Abs(transform.position.x - chunk.regionMiddleX), Mathf.Abs(transform.position.y - chunk.regionMiddleY)) >= Chunk.halfBlockCountPerAxis - 1;

            WaterCenter.Add(this);
        }

        internal Block GetBlock(Vector2Int pos)
        {
            if (isOnChunkBoundary)
                return chunk.map.GetBlock(pos, isBackground);
            else
                return chunk.GetBlock(pos, isBackground);
        }

        public override void OnRecovered()
        {
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

            if (entity.isPlayer)
            {
                ((Player)entity).fallenY = entity.transform.position.y;
            }
        }
    }
}
