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

            destroyOnCollideWithBlock = false;
            damage = 15;
        }

        protected override void HitEntity(Entity entity)
        {
            base.HitEntity(entity);
            
            entity.ServerSetTemperatureEffect(TemperatureEffectType.Frozen);
        }
    }
}