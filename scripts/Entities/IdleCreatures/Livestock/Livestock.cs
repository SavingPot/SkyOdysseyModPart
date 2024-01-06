using UnityEngine;

namespace GameCore
{
    public abstract class Livestock : IdleCreature
    {
        public LivestockState status;
        public float hurtTimer;
        public string textureId;

        protected override void Start()
        {
            base.Start();

            escapeTime = 8;

            /* ---------------------------------- 设置贴图 ---------------------------------- */
            AddSpriteRenderer(textureId);
        }
    }

    public enum LivestockState : byte
    {
        Idle,
        GoLeft,
        GoRight,
    }
}