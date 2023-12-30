using UnityEngine;

namespace GameCore
{
    public class EscapableIdleCreature : IdleCreature
    {
        public float escapeTimer;
        public float escapeTime = 4;

        public override void OnGetHurtClient()
        {
            base.OnGetHurtClient();

            escapeTimer = Tools.time + escapeTime;
        }

        protected override void Update()
        {
            base.Update();

            if (Tools.time < escapeTimer)
            {
                Vector2 velocity = Random.Range(-1, 2) switch
                {
                    -1 => TurnLeft(),
                    0 => rb.velocity,
                    1 => TurnRight(),
                    _ => throw new()
                };
                rb.velocity = velocity == Vector2.zero ? GetMovementVelocity(rb.velocity) : velocity;
            }
        }
    }
}