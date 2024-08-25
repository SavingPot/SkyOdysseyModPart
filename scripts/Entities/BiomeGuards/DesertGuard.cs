using GameCore.High;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.DesertGuard)]
    public class DesertGuard : BiomeGuard
    {
        public SpriteRenderer spriteRenderer;

        public override void AfterInitialization()
        {
            base.AfterInitialization();

            //添加贴图
            spriteRenderer = AddSpriteRenderer(BlockID.Sand);
        }

        protected override void ReleaseAttack()
        {
            var bulletAmount = Random.Range(8, 25);

            for (var i = 0; i < bulletAmount; i++)
            {
                var velocity = new Vector2(Random.Range(-10, 11), Random.Range(10, 16));

                GM.instance.SummonBullet(transform.position, EntityID.DesertGuardSand, velocity, netId);
            }
        }
    }
}