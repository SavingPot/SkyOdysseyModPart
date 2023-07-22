using SP.Tools;

namespace GameCore
{
    [ItemBinding(ItemID.FlintKnife)]
    public class FlintKnifeBehaviour : KnifeBehaviour
    {
        public FlintKnifeBehaviour(IInventoryOwner owner, Item instance, string inventoryIndex) : base(owner, instance, inventoryIndex)
        {

        }
    }
}
