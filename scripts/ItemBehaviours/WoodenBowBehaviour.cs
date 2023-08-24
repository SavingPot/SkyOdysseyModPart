using GameCore.High;
using Newtonsoft.Json.Linq;
using SP.Tools;
using UnityEngine;

namespace GameCore
{
    [ItemBinding(ItemID.WoodenBow)]
    public class WoodenBowBehaviour : BowBehaviour
    {
        public Timer clearTimer;

        public override bool Use()
        {
            bool shotted = false;

            if (clearTimer.HasFinished())
            {
                if (owner is Player player)
                {
                    var velocity = Tools.GetAngleVector2(player.transform.position, player.cursorWorldPos).normalized * 18;

                    JObject jo = new();
                    jo.AddObject("ori:bullet");
                    jo["ori:bullet"].AddProperty("ownerId", player.netId);
                    jo["ori:bullet"].AddProperty("velocity", velocity.x, velocity.y);
                    GM.instance.SummonEntity(player.transform.position, EntityID.WoodenArrow, Tools.randomGUID, true, null, jo.ToString());
                    shotted = true;
                    clearTimer.Start(0.5f);
                }
            }

            return shotted;
        }

        public WoodenBowBehaviour(IInventoryOwner owner, Item instance, string inventoryIndex) : base(owner, instance, inventoryIndex)
        {

        }
    }
}
