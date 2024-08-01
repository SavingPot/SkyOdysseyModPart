using GameCore.UI;

namespace GameCore
{
    [ItemBinding(ItemID.SpellManuscript)]
    public class SpellManuscriptBehaviour : ItemBehaviour, ISpellContainer
    {
        public Spell spell { get; set; }
        public SpellBehaviour spellBehaviour { get; set; }

        public SpellManuscriptBehaviour(IInventoryOwner owner, Item data, string inventoryIndex) : base(owner, data, inventoryIndex)
        {
            ISpellContainer.LoadFromJObject(this, instance.customData);
        }
    }
}
