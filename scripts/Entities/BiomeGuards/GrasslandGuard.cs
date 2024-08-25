using System;
using Newtonsoft.Json.Linq;
using SP.Tools.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    [EntityBinding(EntityID.GrasslandGuard)]
    public class GrasslandGuard : BiomeGuard
    {
        public SpriteRenderer spriteRenderer;

        public override void AfterInitialization()
        {
            base.AfterInitialization();

            //添加贴图
            spriteRenderer = AddSpriteRenderer(BlockID.Grass);
        }

        protected override void ReleaseAttack()
        {
            foreach (var player in PlayerCenter.all)
            {
                if ((player.transform.position - transform.position).sqrMagnitude > attackRadius)
                    continue;

                var velocity = AngleTools.GetAngleVector2(transform.position, player.transform.position).normalized * 30;
                velocity.y += 1; //y轴 +1 是为了抬高一点角度

                GM.instance.SummonBullet(transform.position, EntityID.FlintArrow, velocity, netId);
            }

            //TODO: 每30s发射一次树种
        }
    }
}