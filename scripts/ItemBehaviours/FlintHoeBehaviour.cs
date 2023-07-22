using SP.Tools;

namespace GameCore
{
    [ItemBinding(ItemID.FlintHoe)]
    public class FlintHoeBehaviour : HoeBehaviour
    {
        public FlintHoeBehaviour(IInventoryOwner owner, Item data, string inventoryIndex) : base(owner, data, inventoryIndex)
        {

        }
    }
}
