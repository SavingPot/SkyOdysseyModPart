namespace GameCore
{
    [ItemBinding(ItemID.FeatherWing)]
    public class FeatherWingBehaviour : ItemBehaviour
    {
        public FeatherWingBehaviour(IInventoryOwner owner, Item instance, string inventoryIndex) : base(owner, instance, inventoryIndex)
        {

        }
    }
}
