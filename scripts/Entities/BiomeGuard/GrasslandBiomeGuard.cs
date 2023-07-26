using GameCore.High;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.GrasslandGuard)]
    public class GrasslandGuard : BiomeGuard
    {
        public float attackTimer;
        public int attackRadius = 15 * 15; // 15^2

        protected override void Update()
        {
            base.Update();

            if (Tools.time >= attackTimer)
            {
                foreach (var player in PlayerCenter.allReady)
                {
                    if ((player.transform.position - transform.position).sqrMagnitude <= attackRadius)
                    {
                        var velocity = Tools.GetAngleVector2(transform.position, player.transform.position).normalized * 15;

                        JObject jo = new();
                        jo.AddObject("ori:bullet");
                        jo["ori:bullet"].AddProperty("ownerId", netId);
                        jo["ori:bullet"].AddProperty("velocity", velocity.x, velocity.y);

                        GM.instance.SummonEntity(transform.position, EntityID.GrasslandGuardStorm, Tools.randomGUID, true, null, jo.ToString());
                    }
                }

                attackTimer = Tools.time + 1;
            }
        }
    }
}