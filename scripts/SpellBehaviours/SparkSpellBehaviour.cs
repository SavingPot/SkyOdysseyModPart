using GameCore.High;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GameCore
{
    [SpellBinding(SpellID.Spark)]
    public class SparkSpellBehaviour : SpellBehaviour
    {
        public override void Release(Vector2 releaseDirection, Vector2 releasePosition, Player player)
        {
            //标准化以保证速度正常, 乘以数字以加快子弹速度
            var velocity = releaseDirection.normalized * 15;

            JObject jo = new();
            jo.AddObject("ori:bullet");
            jo["ori:bullet"].AddProperty("ownerId", player.netId);
            jo["ori:bullet"].AddProperty("velocity", velocity.x, velocity.y);
            GM.instance.SummonEntity(player.transform.position, EntityID.Spark, Tools.randomGUID, true, null, jo.ToString());
        }

        public SparkSpellBehaviour(IManaContainer manaContainer, ISpellContainer spellContainer, Spell instance) : base(manaContainer, spellContainer, instance)
        {

        }
    }
}