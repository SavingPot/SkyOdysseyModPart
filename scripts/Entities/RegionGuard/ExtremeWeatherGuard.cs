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

                    JObject jo = new();
                    jo.AddObject("ori:bullet");
                    jo["ori:bullet"].AddProperty("ownerId", netId);
                    jo["ori:bullet"].AddProperty("velocity", velocity.x, velocity.y);

                    if (Tools.randomBool)
                        GM.instance.SummonEntity(transform.position, EntityID.ExtremeWeatherGuardSand, Tools.randomGUID, true, null, jo.ToString());
                    else
                        GM.instance.SummonEntity(transform.position, EntityID.ExtremeWeatherGuardSnowball, Tools.randomGUID, true, null, jo.ToString());
                }

                attackTimer = Tools.time + 2f;
            }
        }
    }
}