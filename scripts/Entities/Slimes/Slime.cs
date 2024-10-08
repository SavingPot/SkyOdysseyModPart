using DG.Tweening;
using GameCore.High;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    public abstract class Slime : Enemy, IEnemyJumpToTarget
    {
        public string Texture;

        public bool isPursuing { get; set; }
        public bool isPursuingLastFrame { get; set; }
        public float jumpForce => 46;






        public override void Initialize()
        {
            base.Initialize();

            /* ---------------------------------- 设置贴图 ---------------------------------- */
            AddSpriteRenderer(Texture);

            jumpCD = 2;
            fallenDamageHeight = float.MaxValue; //免疫摔落伤害

            transform.localScale= Vector3.one * Random.Range(0.9f, 1.5f);
        }

        public override Vector2 GetMovementDirection()
        {
            if (isDead)
                return Vector2.zero;

            return EnemyJumpToTargetBehaviour.GetMovementVelocity(this);
        }
    }
}
