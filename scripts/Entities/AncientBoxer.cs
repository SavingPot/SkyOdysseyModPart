using DG.Tweening;
using SP.Tools.Unity;
using System.Collections;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.AncientBoxer)]
    public class AncientBoxer : Enemy, IEnemyWalkToTarget
    {
        public bool isPursuing { get; set; }
        public bool isPursuingLastFrame { get; set; }
        public float jumpForce => 20;








        public override void Initialize()
        {
            base.Initialize();

            /* -------------------------------------------------------------------------- */
            /*                                     贴图                                     */
            /* -------------------------------------------------------------------------- */
            var sr = AddSpriteRenderer("ori:ancient_boxer.idle_0");



            /* -------------------------------------------------------------------------- */
            /*                                     动画                                     */
            /* -------------------------------------------------------------------------- */
            attackAnimations = new[] { "attack" };
            animWeb = new();

            /* ----------------------------------- 站立 ----------------------------------- */
            animWeb.AddAnim("idle", -1, new AnimFragment[] {
                new SpriteAnimFragment(
                    value => { if (sr) sr.sprite = value; return sr; },
                    new[]
                    {
                        ModFactory.CompareTexture("ori:ancient_boxer.idle_0").sprite,
                        ModFactory.CompareTexture("ori:ancient_boxer.idle_1").sprite
                    },
                    1.5f,
                    Ease.Linear
                )
            });

            /* ----------------------------------- 跑步 ----------------------------------- */
            animWeb.AddAnim("run", -1, new AnimFragment[] {
                new SpriteAnimFragment(
                    value => { if (sr) sr.sprite = value; return sr; },
                    new[]
                    {
                        ModFactory.CompareTexture("ori:ancient_boxer.run_0").sprite,
                        ModFactory.CompareTexture("ori:ancient_boxer.run_1").sprite,
                        ModFactory.CompareTexture("ori:ancient_boxer.run_2").sprite,
                        ModFactory.CompareTexture("ori:ancient_boxer.run_3").sprite,
                    },
                    0.7f,
                    Ease.Linear
                )
            });

            /* ----------------------------------- 攻击 ----------------------------------- */
            animWeb.AddAnim("attack", 1, new AnimFragment[] {
                new SpriteAnimFragment(
                    value => { if (sr) sr.sprite = value; return sr; },
                    new[]
                    {
                        ModFactory.CompareTexture("ori:ancient_boxer.attack_0").sprite,
                        ModFactory.CompareTexture("ori:ancient_boxer.attack_1").sprite,
                        ModFactory.CompareTexture("ori:ancient_boxer.attack_2").sprite,
                        ModFactory.CompareTexture("ori:ancient_boxer.attack_3").sprite,
                    },
                    0.4f,
                    Ease.Linear
                )
            });

            animWeb.CreateConnectionFromTo("attack", "idle", () => true, 0.4f);
        }

        public override Vector2 GetMovementDirection()
        {
            if (isDead)
                return Vector2.zero;

            return EnemyWalkToTargetBehaviour.GetMovementVelocity(this);
        }

        public void WhenStroll()
        {
            GAudio.Play(AudioID.AncientBoxerSpare, transform.position, true);
        }
    }
}
