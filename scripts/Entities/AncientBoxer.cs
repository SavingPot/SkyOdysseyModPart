using DG.Tweening;
using SP.Tools.Unity;
using System.Collections;
using UnityEngine;
using static GameCore.UniversalEntityBehaviour;
using Random = UnityEngine.Random;

namespace GameCore
{
    [EntityBinding(EntityID.AncientBoxer)]
    public class AncientBoxer : Enemy
    {
        public EnemyMoveToTarget ai;
        public bool isPursuing;
        public bool isPursuingLastFrame;



        protected override void Awake()
        {
            base.Awake();

            ai = new(this, 20, () => GAudio.Play(AudioID.AncientBoxerSpare, true));
        }

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

            /* ----------------------------------- 战立 ----------------------------------- */
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
            animWeb.AddAnim("attack", -1, new AnimFragment[] {
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
        }

        public override void Movement()
        {
            base.Movement();

            if (!isServer || isDead)
                return;

            isPursuing = targetTransform;

            if (isPursuing)
            {
                if (!isPursuingLastFrame)
                {
                    ServerOnStartMovement();
                }

                ai.Pursuit();
            }
            else
            {
                if (isPursuingLastFrame)
                {
                    ServerOnStopMovement();

                    rb.velocity = Vector2.zero;
                }

                ai.Stroll();
            }

            isPursuingLastFrame = isPursuing;
        }
    }
}
