using SP.Tools;
using UnityEngine;

namespace GameCore
{
    [ItemBinding(ItemID.SpellManuscript)]
    public class SpellManuscriptItemBehaviour : ItemBehaviour, ISpellContainer
    {
        public Spell spell { get; set; }
        public SpellBehaviour spellBehaviour { get; set; }

        public override void ModifyInfo(ItemInfoShower ui)
        {
            base.ModifyInfo(ui);

            //TODO: COmpare text
            if (spell == null)
                ui.nameText.text.text += $" - 内容: 空";
            else
                ui.nameText.text.text += $" - 内容: {spell.id}";
        }

        public SpellManuscriptItemBehaviour(IInventoryOwner owner, Item data, string inventoryIndex) : base(owner, data, inventoryIndex)
        {
            ISpellContainer.LoadFromJObject(this, instance.customData);
        }
    }
}
