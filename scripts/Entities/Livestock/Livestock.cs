using UnityEngine;

namespace GameCore
{
    public abstract class LivestockProperties<T> : CoreCreatureProperties<T> where T : LivestockProperties<T>, new()
    {
        public abstract string Texture();
        public virtual float FlusteredTime() => 8;
    }

    public abstract class Livestock<T> : Creature where T : LivestockProperties<T>, new()
    {
        public LivestockState status;
        public float hurtTimer;

        protected override void Start()
        {
            base.Start();

            /* ---------------------------------- 设置贴图 ---------------------------------- */
            AddSpriteRenderer(LivestockProperties<T>.instance.Texture());
        }

        protected override void Update()
        {
            base.Update();

            /* -------------------------------------------------------------------------- */
            /*                                     站立                                     */
            /* -------------------------------------------------------------------------- */
            if (status == LivestockState.Idle)
            {
                if (hurtTimer > 0 || Tools.Prob100(5 * Tools.deltaTime))
                {
                    status = Tools.randomBool ? LivestockState.GoLeft : LivestockState.GoRight;
                }
            }
            /* -------------------------------------------------------------------------- */
            /*                                     左拐                                     */
            /* -------------------------------------------------------------------------- */
            else if (status == LivestockState.GoLeft)
            {
                if ((hurtTimer > 0 && Tools.Prob100(40 * Tools.deltaTime)) || (hurtTimer <= 0 && Tools.Prob100(100 * Tools.deltaTime)))
                {
                    status = LivestockState.Idle;
                }
            }
            /* -------------------------------------------------------------------------- */
            /*                                     右拐                                     */
            /* -------------------------------------------------------------------------- */
            else if (status == LivestockState.GoRight)
            {
                if ((hurtTimer > 0 && Tools.Prob100(40 * Tools.deltaTime)) || (hurtTimer <= 0 && Tools.Prob100(100 * Tools.deltaTime)))
                {
                    status = LivestockState.Idle;
                }
            }


            /* -------------------------------------------------------------------------- */
            /*                                     设置速度                                     */
            /* -------------------------------------------------------------------------- */

            if (status == LivestockState.GoLeft)
            {
                TurnLeft();
            }
            else if (status == LivestockState.GoRight)
            {
                TurnRight();
            }



            //计算计时器
            hurtTimer -= Tools.deltaTime;
        }

        public override void DoGetHurtServer()
        {
            hurtTimer = LivestockProperties<T>.instance.FlusteredTime();
        }

        public virtual void Turn()
        {

        }

        public virtual void TurnLeft()
        {
            Turn();

            Vector2 velocity = new(-1, 0);
            rb.velocity = GetMovementVelocity(velocity);

            SetOrientation(false);
        }

        public virtual void TurnRight()
        {
            Turn();

            Vector2 velocity = new(1, 0);
            rb.velocity = GetMovementVelocity(velocity);

            SetOrientation(true);
        }
    }

    public enum LivestockState : byte
    {
        Idle,
        GoLeft,
        GoRight,
    }
}