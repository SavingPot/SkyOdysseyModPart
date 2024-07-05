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
                if (owner is Player player)
                {
                    var velocity = AngleTools.GetAngleVector2(player.transform.position, point).normalized * 20;

                    GM.instance.SummonEntityCallback(player.transform.position, EntityID.FlintArrow, entity => entity.ServerSetVelocity(velocity));
                    shotted = true;
                    shootTimer = Tools.time + 1;

                    //播放手臂动画
                    if (!player.animWeb.GetAnim("slight_rightarm_lift", 0).isPlaying)
                        player.animWeb.SwitchPlayingTo("slight_rightarm_lift");
                }
            }

            return shotted;
        }

        public WoodenBowBehaviour(IInventoryOwner owner, Item instance, string inventoryIndex) : base(owner, instance, inventoryIndex)
        {

        }
    }
}
