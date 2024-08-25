using SP.Tools.Unity;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.SnowfieldGuardSnowball)]
    public class SnowfieldGuardSnowball : Bullet
    {
        public override void Initialize()
        {
            base.Initialize();

            damage = 1;

            AddSpriteRenderer("ori:snow_block");//TODO
        }

        protected override void HitEntity(Entity entity)
        {
            base.HitEntity(entity);
            
            entity.ServerSetTemperatureEffect(TemperatureEffectType.Frozen);
        }
    }
}