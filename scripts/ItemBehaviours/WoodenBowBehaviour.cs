using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GameCore
{
    [ItemBinding(ItemID.WoodenBow)]
    public class WoodenBowBehaviour : BowBehaviour
    {
        protected override float shootInterval => 1;
        protected override float shootVelocity => 1;
        protected override int extraDamage => 0;

        public WoodenBowBehaviour(IInventoryOwner owner, Item instance, string inventoryIndex) : base(owner, instance, inventoryIndex)
        {

        }
    }
}
