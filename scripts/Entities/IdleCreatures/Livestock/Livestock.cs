using UnityEngine;

namespace GameCore
{
    public abstract class LivestockProperties<T> : CoreCreatureProperties<T> where T : LivestockProperties<T>, new()
    {
        public abstract string Texture();
        public virtual float FlusteredTime() => 8;
    }

    public abstract class Livestock<T> : EscapableIdleCreature where T : LivestockProperties<T>, new()
    {
        public LivestockState status;
        public float hurtTimer;

        protected override void Start()
        {
            base.Start();

            escapeTime = LivestockProperties<T>.instance.FlusteredTime();

            /* ---------------------------------- 设置贴图 ---------------------------------- */
            AddSpriteRenderer(LivestockProperties<T>.instance.Texture());
        }
    }

    public enum LivestockState : byte
    {
        Idle,
        GoLeft,
        GoRight,
    }
}