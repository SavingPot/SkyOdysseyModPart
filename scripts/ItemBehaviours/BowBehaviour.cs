using SP.Tools;

namespace GameCore
{
    public abstract class BowBehaviour: ItemBehaviour
    {
        public float shootTimer;

        public BowBehaviour(IInventoryOwner owner, Item instance, string inventoryIndex) : base(owner, instance, inventoryIndex)
        {

        }
    }
}
