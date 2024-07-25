using DG.Tweening;
using GameCore.High;
using SP.Tools.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    [EntityBinding(EntityID.IceSlime)]
    public class IceSlime : Slime
    {
        protected override void Awake()
        {
            base.Awake();

            Texture = "ori:ice_slime.idle";
        }

        public override bool AttackEntity(Entity entity)
        {
            //如果攻击发送成功，则冻结目标实体
            if (base.AttackEntity(entity))
            {
                entity.ServerSetTemperatureEffect(TemperatureEffectType.Frozen);
                return true;
            }

            return false;
        }
    }
}
