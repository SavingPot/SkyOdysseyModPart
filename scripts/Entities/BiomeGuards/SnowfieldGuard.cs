using GameCore.High;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.SnowfieldGuard)]
    public class SnowfieldGuard : BiomeGuard
    {
        protected override void ReleaseAttack()
        {
            var bulletAmount = Random.Range(10, 21);

            for (var i = 0; i < bulletAmount; i++)
            {
                var velocity = new Vector2(Random.Range(-10, 11), Random.Range(12, 18));

                GM.instance.SummonBullet(transform.position, EntityID.SnowfieldGuardSnowball, velocity, netId);
            }
        }
    }
}