using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GameCore
{
    [ItemBinding(ItemID.WoodenBow)]
    public class WoodenBowBehaviour : BowBehaviour
    {
        public override bool Use(Vector2 point)
        {
            bool shotted = false;

            if (Tools.time >= shootTimer)
            {
                if (owner is Creature creature)
                {
                    var velocity = AngleTools.GetAngleVector2(creature.transform.position, point).normalized * 20;

                    GM.instance.SummonBullet(creature.transform.position, EntityID.FlintArrow, velocity, creature.netId);
                    shotted = true;
                    shootTimer = Tools.time + 1;

                    //播放手臂动画
                    if (!creature.animWeb.GetAnim("slight_rightarm_lift", 0).isPlaying)
                        creature.animWeb.SwitchPlayingTo("slight_rightarm_lift");
                }
            }

            return shotted;
        }

        public WoodenBowBehaviour(IInventoryOwner owner, Item instance, string inventoryIndex) : base(owner, instance, inventoryIndex)
        {

        }
    }
}
