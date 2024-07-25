using DG.Tweening;
using GameCore.High;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    [EntityBinding(EntityID.FireSlime)]
    public class FireSlime : Slime
    {
        protected override void Awake()
        {
            base.Awake();

            Texture = "ori:fire_slime.idle";
        }

        public override bool AttackEntity(Entity entity)
        {
            //如果攻击发送成功，则使目标实体着火
            if (base.AttackEntity(entity))
            {
                entity.ServerSetTemperatureEffect(TemperatureEffectType.OnFire);
                return true;
            }

            return false;
        }
    }
}
