using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.OakForestGuard)]
    public class OakForestGuard : BiomeGuard
    {
        public float attackTimer;
        public int attackRadius = 10 * 10; // 10^2

        protected override void Update()
        {
            base.Update();

            if (Tools.time >= attackTimer)
            {
                var velocity = new Vector2(Random.Range(-5, 6), Random.Range(5, 11));
                JObject jo = new();
                jo.AddObject("ori:bullet");
                jo["ori:bullet"].AddProperty("ownerId", netId);
                jo["ori:bullet"].AddProperty("velocity", velocity.x, velocity.y);
                GM.instance.SummonEntity(transform.position, EntityID.OakForestSapling, Tools.randomGUID, true, null, jo.ToString());

                attackTimer = Tools.time + 8f;
            }
        }
    }
}