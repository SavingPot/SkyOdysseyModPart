using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.GrasslandGuard)]
    public class GrasslandGuard : BiomeGuard
    {
        public const int attackRadius = 15 * 15; // 15^2
        public float attackTimer;
        public StateMachine machine;
        public LineRenderer lineRenderer;


        public override void AfterInitialization()
        {
            base.AfterInitialization();

            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.green;
            lineRenderer.material = GInit.instance.spriteDefaultMat;

            machine = new(this);
            machine.ChangeState(new MovingState(machine));
        }

        protected override void Update()
        {
            base.Update();

            machine.Update();
        }























        public sealed class MovingState : IState
        {
            public StateMachine machine;
            public float motionDiameter = 20;
            public float timeToRest;

            public MovingState(StateMachine machine)
            {
                this.machine = machine;
                this.timeToRest = Tools.time + 10;
            }



            public void OnEnter() { }

            public void OnUpdate()
            {
                var guard = (GrasslandGuard)machine.entity;

                /* ----------------------------------- 运动 ----------------------------------- */
                float xDelta = Mathf.PerlinNoise1D(Time.time * 0.5f) - 0.5f; //from -0.5 to 0.5
                float yDelta = Mathf.PerlinNoise1D((Time.time + 10) * 0.5f) - 0.5f;
                Vector3 delta = new(xDelta * motionDiameter, yDelta * motionDiameter);
                guard.gameObject.transform.position = Vector3.Lerp(guard.transform.position, guard.originPosition + delta, Tools.deltaTime * 10f);


                /* ----------------------------------- 休息 ----------------------------------- */
                if (Tools.time >= timeToRest)
                {
                    machine.ChangeState(new RestState(machine));
                }

                /* ----------------------------------- 躲藏 ----------------------------------- */
                else if (Tools.Prob100(Tools.deltaTime * 5))
                {
                    var block = GetGrassBlockToHideIn();
                    if (block != null)
                    {
                        machine.ChangeState(new HideState(machine, block));
                    }
                }

                /* ----------------------------------- 攻击 ----------------------------------- */
                else if (Tools.time >= guard.attackTimer)
                {
                    foreach (var player in PlayerCenter.all)
                    {
                        if ((player.transform.position - guard.transform.position).sqrMagnitude <= attackRadius)
                        {
                            var velocity = AngleTools.GetAngleVector2(guard.transform.position, player.transform.position).normalized * 30;

                            JObject jo = new();
                            jo.AddObject("ori:bullet");
                            jo["ori:bullet"].AddProperty("ownerId", guard.netId);
                            jo["ori:bullet"].AddProperty("velocity", velocity.x, velocity.y + 1); //y轴 +1 是为了抬高一点角度

                            //TODO: 发射树种
                            GM.instance.SummonEntity(guard.transform.position, EntityID.FlintArrow, null, true, null, jo.ToString());
                        }
                    }

                    guard.attackTimer = Tools.time + 1;
                }
            }

            public void OnExit() { }



            Block GetGrassBlockToHideIn()
            {
                var chunk = Map.instance.AddChunk(machine.entity.chunkIndex);

                foreach (var block in chunk.blocks)
                {
                    if (block != null && block.data.id == BlockID.GrassBlock)
                    {
                        return block;
                    }
                }

                return null;
            }
        }





        public sealed class RestState : IState
        {
            public StateMachine machine;
            readonly GrasslandGuard guard;
            public float timeToResume;

            public RestState(StateMachine machine)
            {
                this.machine = machine;
                this.guard = (GrasslandGuard)machine.entity;
                this.timeToResume = Tools.time + 5;
            }



            public void OnEnter()
            {
                guard.particleSystem.Stop();
                guard.rb.gravityScale = 3;
            }

            public void OnUpdate()
            {
                if (Tools.time >= timeToResume)
                {
                    machine.ChangeState(new MovingState(machine));
                }
            }

            public void OnExit()
            {
                guard.particleSystem.Play();
                guard.rb.gravityScale = guard.data.gravity;
            }
        }





        public sealed class HideState : IState
        {
            readonly StateMachine machine;
            readonly Block blockToHideIn;
            readonly Vector3 targetPosition;
            readonly GrasslandGuard guard;
            float timeToEscape;

            public HideState(StateMachine machine, Block blockToHideIn)
            {
                this.machine = machine;
                this.blockToHideIn = blockToHideIn;
                this.guard = (GrasslandGuard)machine.entity;
                this.targetPosition = blockToHideIn.transform.position;
            }



            public void OnEnter()
            {
                timeToEscape = Tools.time + 5;
                guard.Hide();
                guard.lineRenderer.SetPositions(new Vector3[] { guard.transform.position, targetPosition });
            }

            public void OnUpdate()
            {
                guard.transform.position = Vector3.Lerp(guard.transform.position, targetPosition, Tools.deltaTime * 5f);
                guard.lineRenderer.SetPosition(0, guard.transform.position);

                if (Tools.time >= timeToEscape)
                {
                    machine.entity.Show();
                    machine.ChangeState(new MovingState(machine));
                }
            }

            public void OnExit() { }
        }
    }
}