using GameCore.High;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.SnowfieldGuard)]
    public class SnowfieldGuard : BiomeGuard
    {
        public SpriteRenderer spriteRenderer;

        public override void AfterInitialization()
        {
            base.AfterInitialization();

            //添加贴图
            spriteRenderer = AddSpriteRenderer(BlockID.SnowBlock);
        }

        protected override void ReleaseAttack()
        {
            var bulletAmount = Random.Range(5, 16);

            for (var i = 0; i < bulletAmount; i++)
            {
                var velocity = new Vector2(Random.Range(-10, 11), Random.Range(12, 18));

                GM.instance.SummonBullet(transform.position, EntityID.SnowfieldGuardSnowball, velocity, netId);
            }
        }
    }
}