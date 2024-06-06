using GameCore.High;
using SP.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    [ItemBinding(ItemID.LootBag)]
    public class LootBagBehaviour : ItemBehaviour
    {
        //TODO: Implement loot bag behaviour
        public override bool Use(Vector2 point)
        {
            return true;
        }

        public LootBagBehaviour(IInventoryOwner owner, Item instance, string inventoryIndex) : base(owner, instance, inventoryIndex) { }
    }
}
