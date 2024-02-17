using GameCore.High;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.PlantRegionGuard)]
    public class PlantRegionGuard : RegionGuard
    {
        public float attackTimer;
        public int attackRadius = 15 * 15; // 15^2

        protected override void Update()
        {
            base.Update();

            if (Tools.time >= attackTimer)
            {
                foreach (var player in PlayerCenter.all)
                {
                    if ((player.transform.position - transform.position).sqrMagnitude <= attackRadius)
                    {
                        var velocity = Tools.GetAngleVector2(transform.position, player.transform.position).normalized * 22;

                        JObject jo = new();
                        jo.AddObject("ori:bullet");
                        jo["ori:bullet"].AddProperty("ownerId", netId);
                        jo["ori:bullet"].AddProperty("velocity", velocity.x, velocity.y);

                        //TODO: 发射树种
                        GM.instance.SummonEntity(transform.position, EntityID.FlintArrow, null, true, null, jo.ToString());
                    }
                }

                attackTimer = Tools.time + 1;
            }
        }
    }
}