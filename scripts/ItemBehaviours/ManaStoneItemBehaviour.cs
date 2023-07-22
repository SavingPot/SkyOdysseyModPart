using UnityEngine;

namespace GameCore
{
    [ItemBinding(ItemID.ManaStone)]
    public class ManaStoneItemBehaviour : ItemBehaviour, IManaContainer, ISpellContainer
    {
        public int totalMana { get; set; }
        public Spell spell { get; set; }
        public SpellBehaviour spellBehaviour { get; set; }

        public override void ModifyInfo(ItemInfoUI ui)
        {
            base.ModifyInfo(ui);

            //TODO: COmpare text
            ui.detailText.text.text += $"\n魔能: {totalMana}\n";
            if (spell == null)
                ui.detailText.text.text += $"魔咒: 空";
            else
                ui.detailText.text.text += $"魔咒: {spell.id}";
        }

        public override bool Use()
        {
            //TODO: this only offers player logic
            if (totalMana >= spell.cost)
            {
                totalMana -= spell.cost;
                spellBehaviour.Release(Tools.GetAngleVector2((Vector2)((Player)owner).transform.position, ((Player)owner).cursorWorldPos), (Vector2)((Player)owner).transform.position, (Player)owner);
                return true;
            }

            return false;
        }

        public ManaStoneItemBehaviour(IInventoryOwner owner, Item data, string inventoryIndex) : base(owner, data, inventoryIndex)
        {
            ISpellContainer.LoadFromJObject(this, instance.customData);
            IManaContainer.LoadFromJObject(this, instance.customData);
        }
    }
}
