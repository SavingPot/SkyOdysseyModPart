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

            GM.instance.SummonBullet(player.transform.position, EntityID.Spark, velocity, player.netId);
        }

        public SparkSpellBehaviour(ISpellContainer spellContainer, Spell instance) : base(spellContainer, instance)
        {

        }
    }
}