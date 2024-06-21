using GameCore.High;
using Newtonsoft.Json;
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
            var velocity = releaseDirection.normalized * 16;

            JObject jo = new();
            jo.AddObject("ori:bullet");
            jo["ori:bullet"].AddProperty("ownerId", player.netId);
            jo["ori:bullet"].AddProperty("velocity", velocity.x, velocity.y);
            GM.instance.SummonEntity(player.transform.position, EntityID.Spark, customData: jo.ToString(Formatting.None));
        }

        public SparkSpellBehaviour(ISpellContainer spellContainer, Spell instance) : base(spellContainer, instance)
        {

        }
    }
}