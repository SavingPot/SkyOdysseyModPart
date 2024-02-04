using UnityEngine;

namespace GameCore
{
    public abstract class Livestock : IdleCreature
    {
        public LivestockState status;
        public float hurtTimer;
        public string textureId;

        public override void Initialize()
        {
            base.Initialize();

            escapeLastTime = 8;

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