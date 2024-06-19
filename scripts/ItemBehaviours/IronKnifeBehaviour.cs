using SP.Tools;

namespace GameCore
{
    [ItemBinding(ItemID.IronKnife)]
    public class IronKnifeBehaviour : KnifeBehaviour
    {
        public IronKnifeBehaviour(IInventoryOwner owner, Item instance, string inventoryIndex) : base(owner, instance, inventoryIndex)
        {

        }
    }
}
