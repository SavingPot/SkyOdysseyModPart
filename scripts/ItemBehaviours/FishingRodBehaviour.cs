using GameCore.High;
using SP.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    [ItemBinding(ItemID.FishingRod)]
    public class FishingRodBehaviour : ItemBehaviour
    {
        public override bool Use(Vector2 point)
        {

        }

        public FishingRodBehaviour(IInventoryOwner owner, Item data, string inventoryIndex) : base(owner, data, inventoryIndex)
        {

        }
    }
}
