using Cysharp.Threading.Tasks;
using GameCore.Converters;
using SP.Tools;
using SP.Tools.Unity;
using System;
using UnityEngine;

namespace GameCore
{
    public abstract class CoreEnemyProperties<T> : CoreCreatureProperties<T> where T : CoreEnemyProperties<T>, new()
    {
        public virtual ushort SearchRadius() => 25;
        public virtual float AttackRadius() => 2f;
        public virtual float NormalAttackDamage() => 20;
        public virtual float AttackCD() => 2;
    }

    public class CoreEnemy<PropertyT> : Enemy
        where PropertyT : CoreEnemyProperties<PropertyT>, new()
    {
        /* -------------------------------------------------------------------------- */
        /*                                     接口                                     */
        /* -------------------------------------------------------------------------- */
        public float searchTime = float.NegativeInfinity;





        /* -------------------------------------------------------------------------- */
        /*                               Static & Const                               */
        /* -------------------------------------------------------------------------- */
        public virtual void TryAttack()
        {
            //为了性能使用 x - x, y - y 而不是 Vector2.Distance()
            float disX = Mathf.Abs(transform.position.x - targetTransform.position.x);
            float disY = Mathf.Abs(transform.position.y - targetTransform.position.y);

            //在攻击范围内, 并且 CD 已过
            if (disX <= CoreEnemyProperties<PropertyT>.instance.AttackRadius() && disY <= CoreEnemyProperties<PropertyT>.instance.AttackRadius() && (Time.time - attackTime >= CoreEnemyProperties<PropertyT>.instance.AttackCD()))
            {
                AttackCore();
            }
        }

        //如果与目标距离大于搜索半径则停止追击
        public void CheckEnemyTarget()
        {
            if (targetTransform && Vector2.Distance(targetTransform.position, transform.position) > CoreEnemyProperties<PropertyT>.instance.SearchRadius())
                targetTransform = null;
        }

        public string[] attackAnimations = new[] { "attack_leftarm", "attack_rightarm" };

        public virtual void AttackCore()
        {
            attackTime = Time.time;

            //设置动画
            foreach (var animId in attackAnimations)
            {
                anim.SetAnim(animId);
            }

            if (UObjectTools.GetComponent(targetTransform, out Entity entity))
            {
                entity.TakeDamage(CoreEnemyProperties<PropertyT>.instance.NormalAttackDamage(), 0.3f, transform.position, transform.position.x < targetTransform.position.x ? Vector2.right * 12 : Vector2.left * 12);
            }
        }





        /* -------------------------------------------------------------------------- */
        /*                                   Dynamic                                  */
        /* -------------------------------------------------------------------------- */
        public float attackTime;





        protected override void Start()
        {
            base.Start();

            //初始化敌人特性
            PropertyT et = CoreEnemyProperties<PropertyT>.instance;

            //初始化碰撞体
            FindTarget = () =>
            {
                //搜索 CD 3s
                if (Tools.time < searchTime + 3)
                    return null;

                searchTime = Tools.time;

                //在搜索半径中寻找物体
                Collider2D[] overlapResults = Physics2D.OverlapCircleAll(transform.position, et.SearchRadius());

                for (int i = 0; i < overlapResults.Length; i++)
                {
                    if (overlapResults[i].TryGetComponent<Player>(out Player player))
                    {
                        return player.transform;
                    }
                }

                return null;
            };
        }
    }

    public enum BasicEnemyState : byte
    {
        Idle,
        Movement,
    }
}
