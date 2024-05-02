using GameCore.UI;
using UnityEngine;

namespace GameCore
{
    [ItemBinding(ItemID.ManaStone)]
    public class ManaStoneItemBehaviour : ItemBehaviour, IManaContainer, ISpellContainer
    {
        int _totalMana;
        public int totalMana
        {
            get => _totalMana;
            set
            {
                //必须在 Inventory.ExecuteOperation 中设置 totalMana
                owner.GetInventory().ExecuteOperation(_ =>
                {
                    //设置魔能值
                    _totalMana = value;

                    //写入 customData
                    IManaContainer.WriteIntoJObject(this, instance.customData);
                }, inventoryIndex);
            }
        }
        public int maxMana => 100;
        public Spell spell { get; set; }
        public SpellBehaviour spellBehaviour { get; set; }

        public override void ModifyInfo(ItemInfoShower ui)
        {
            base.ModifyInfo(ui);

            //TODO: Compare text
            ui.detailText.text.text += $"\n魔能: {totalMana}\n";
            if (spell == null)
                ui.detailText.text.text += $"魔咒: 空";
            else
                ui.detailText.text.text += $"魔咒: {spell.id}";
        }

        public override bool Use(Vector2 point)
        {
            //TODO: this only offers player logic
            if (totalMana >= spell.cost)
            {
                //释放
                spellBehaviour.Release(AngleTools.GetAngleVector2(owner.transform.position, point), owner.transform.position, (Player)owner);

                //播放手臂动画
                if (!((Player)owner).animWeb.GetAnim("slight_rightarm_lift", 0).isPlaying)
                    ((Player)owner).animWeb.SwitchPlayingTo("slight_rightarm_lift");

                //减少魔能
                totalMana -= spell.cost;

                return true;
            }

            return false;
        }

        public override void OnHand()
        {
            base.OnHand();

            //TODO
            if (totalMana < maxMana)
            {
                int delta = 1;

                //增加魔能 (限制在 maxMana 以下)
                totalMana = Mathf.Min(totalMana + delta, maxMana);
            }
        }

        public ManaStoneItemBehaviour(IInventoryOwner owner, Item data, string inventoryIndex) : base(owner, data, inventoryIndex)
        {
            //加载魔咒和魔能
            ISpellContainer.LoadFromJObject(this, instance.customData);
            IManaContainer.LoadFromJObject(this, instance.customData);
        }
    }
}
