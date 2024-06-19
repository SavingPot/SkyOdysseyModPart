using SP.Tools;

namespace GameCore
{
    [ItemBinding(ItemID.IronHoe)]
    public class IronHoeBehaviour : HoeBehaviour
    {
        public IronHoeBehaviour(IInventoryOwner owner, Item data, string inventoryIndex) : base(owner, data, inventoryIndex)
        {

        }
    }
}
