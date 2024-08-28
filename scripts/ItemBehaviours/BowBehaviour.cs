using SP.Tools;
using UnityEngine;

namespace GameCore
{
    public abstract class BowBehaviour : ItemBehaviour
    {
        protected float shootTimer;
        protected abstract float shootInterval { get; }
        protected abstract float shootVelocity { get; }
        protected abstract int extraDamage { get; }

        public override bool Use(Vector2 point)
        {
            bool shottedSuccessfully = false;

            if (Tools.time >= shootTimer)
            {
                if (owner is Creature creature)
                {
                    var velocity = AngleTools.GetAngleVector2(creature.transform.position, point).normalized * shootVelocity;

                    GM.instance.SummonBullet(creature.transform.position, EntityID.FlintArrow, velocity, creature.netId);
                    shottedSuccessfully = true;
                    shootTimer = Tools.time + shootInterval;

                    //播放手臂动画
                    if (!creature.animWeb.GetAnim("slight_rightarm_lift", 0).isPlaying)
                        creature.animWeb.SwitchPlayingTo("slight_rightarm_lift");
                }
            }

            return shottedSuccessfully;
        }

        public BowBehaviour(IInventoryOwner owner, Item instance, string inventoryIndex) : base(owner, instance, inventoryIndex)
        {

        }
    }
}
