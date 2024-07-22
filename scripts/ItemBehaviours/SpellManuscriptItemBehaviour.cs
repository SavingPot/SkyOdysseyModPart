using GameCore.UI;

namespace GameCore
{
    [ItemBinding(ItemID.SpellManuscript)]
    public class SpellManuscriptItemBehaviour : ItemBehaviour, ISpellContainer
    {
        public Spell spell { get; set; }
        public SpellBehaviour spellBehaviour { get; set; }

        public SpellManuscriptItemBehaviour(IInventoryOwner owner, Item data, string inventoryIndex) : base(owner, data, inventoryIndex)
        {
            ISpellContainer.LoadFromJObject(this, instance.customData);
        }
    }
}
