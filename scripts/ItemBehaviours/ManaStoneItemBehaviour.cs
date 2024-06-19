using GameCore.UI;
using UnityEngine;

namespace GameCore
{
    [ItemBinding(ItemID.ManaStone)]
    public class ManaStoneItemBehaviour : ItemBehaviour, ISpellContainer
    {
        public int maxMana => 100;
        public Spell spell { get; set; }
        public SpellBehaviour spellBehaviour { get; set; }

        public override void ModifyInfo(ItemInfoShower ui)
        {
            base.ModifyInfo(ui);

            //TODO: Compare text
            if (spell == null)
                ui.detailText.text.text += $"魔咒: 空";
            else
                ui.detailText.text.text += $"魔咒: {spell.id}";
        }

        public override bool Use(Vector2 point)
        {
            //TODO: this only offers player logic
            if (owner is not Player player)
                return false;

            if (player.mana >= spell.cost)
            {
                //释放
                spellBehaviour.Release(AngleTools.GetAngleVector2(owner.transform.position, point), owner.transform.position, (Player)owner);

                //播放手臂动画
                if (!((Player)owner).animWeb.GetAnim("slight_rightarm_lift", 0).isPlaying)
                    ((Player)owner).animWeb.SwitchPlayingTo("slight_rightarm_lift");

                //减少魔能
                player.mana -= spell.cost;

                return true;
            }

            return false;
        }

        public override void OnHand()
        {
            base.OnHand();
        }

        public ManaStoneItemBehaviour(IInventoryOwner owner, Item data, string inventoryIndex) : base(owner, data, inventoryIndex)
        {
            //加载魔咒
            ISpellContainer.LoadFromJObject(this, instance.customData);
        }
    }
}
