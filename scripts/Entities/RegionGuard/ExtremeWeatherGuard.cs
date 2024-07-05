using GameCore.High;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.ExtremeWeatherGuard)]
    public class ExtremeWeatherGuard : BiomeGuard
    {
        public float attackTimer;
        public int attackRadius = 10 * 10; // 10^2

        protected override void Update()
        {
            base.Update();

            if (Tools.time >= attackTimer)
            {
                var bulletAmount = Random.Range(10, 21);

                for (var i = 0; i < bulletAmount; i++)
                {
                    var velocity = new Vector2(Random.Range(-10, 11), Random.Range(10, 16));

                    if (Tools.randomBool)
                        GM.instance.SummonEntityCallback(transform.position, EntityID.ExtremeWeatherGuardSand, entity => entity.ServerSetVelocity(velocity));
                    else
                        GM.instance.SummonEntityCallback(transform.position, EntityID.ExtremeWeatherGuardSnowball, entity => entity.ServerSetVelocity(velocity));
                }

                attackTimer = Tools.time + 2f;
            }
        }
    }
}