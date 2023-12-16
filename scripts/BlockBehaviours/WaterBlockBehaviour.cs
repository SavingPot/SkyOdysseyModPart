using GameCore.High;
using SP.Tools.Unity;
using System.Linq;
using UnityEngine;

namespace GameCore
{
    public class WaterBlockBehaviour : Block
    {
        public static float entityFallingSpeed = 2;
        public static float resistance = 1.75f;
        public static float streamSpeed = 0.5f;

        public Vector2Int posTempDown;
        public Vector2Int posTempLeft;
        public Vector2Int posTempRight;
        public float physicTimer;

        public override void DoStart()
        {
            base.DoStart();

            sr.color = new(1, 1, 1, 0.4f);

            posTempDown = new(pos.x, pos.y - 1);
            posTempLeft = new(pos.x - 1, pos.y);
            posTempRight = new(pos.x + 1, pos.y);

            physicTimer = Tools.time + streamSpeed;

            if (Server.isServer)
                MethodAgent.updates += WaterPhysics;
        }

        public override void OnRecovered()
        {
            if (Server.isServer)
                MethodAgent.updates -= WaterPhysics;
        }

        public void WaterPhysics()
        {
            if (Tools.time >= physicTimer)
            {
                physicTimer = Tools.time + streamSpeed;

                //如果当前沙盒不存在未生成完, 不会执行物理
                if (GFiles.world.TryGetSandbox(chunk.sandboxIndex, out Sandbox sb) && (!GM.instance.generatingExistingSandbox || GM.instance.generatedExistingSandboxes.Any(p => p.index == chunk.sandboxIndex)))
                {
                    if (!chunk.map.HasBlock(posTempDown, isBackground))
                    {
                        chunk.map.SetBlockNet(posTempDown, isBackground, data.id, null);
                        chunk.map.DestroyBlockNet(pos, isBackground);
                    }
                    else
                    {
                        bool leftContains = chunk.map.HasBlock(posTempLeft, isBackground);
                        bool rightContains = chunk.map.HasBlock(posTempRight, isBackground);

                        if (!leftContains)
                        {
                            chunk.map.SetBlockNet(posTempLeft, isBackground, data.id, null);
                        }
                        if (!rightContains)
                        {
                            chunk.map.SetBlockNet(posTempRight, isBackground, data.id, null);
                        }

                        if (!leftContains || !rightContains)
                        {
                            chunk.map.DestroyBlockNet(pos, isBackground);
                        }
                    }
                }
            }
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
