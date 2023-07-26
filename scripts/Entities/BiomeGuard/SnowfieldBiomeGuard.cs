using GameCore.High;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.SnowfieldGuard)]
    public class SnowfieldGuard : BiomeGuard
    {
        public float attackTimer;
        public int attackRadius = 10 * 10; // 10^2

        protected override void Update()
        {
            base.Update();

            if (Tools.time >= attackTimer)
            {
                var snowballAmount = Random.Range(10, 21);

                for (var i = 0; i < snowballAmount; i++)
                {
                    var velocity = new Vector2(Random.Range(-10, 11), Random.Range(10, 16));

                    JObject jo = new();
                    jo.AddObject("ori:bullet");
                    jo["ori:bullet"].AddProperty("ownerId", netId);
                    jo["ori:bullet"].AddProperty("velocity", velocity.x, velocity.y);

                    GM.instance.SummonEntity(transform.position, EntityID.SnowfieldGuardSnowball, Tools.randomGUID, true, null, jo.ToString());
                }

                attackTimer = Tools.time + 2f;
            }
        }
    }
}