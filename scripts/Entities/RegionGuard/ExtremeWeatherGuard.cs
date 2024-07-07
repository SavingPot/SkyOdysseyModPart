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
                        GM.instance.SummonBullet(transform.position, EntityID.ExtremeWeatherGuardSand, velocity, netId);
                    else
                        GM.instance.SummonBullet(transform.position, EntityID.ExtremeWeatherGuardSnowball, velocity, netId);
                }

                attackTimer = Tools.time + 2f;
            }
        }
    }
}