using System;
using Newtonsoft.Json.Linq;
using SP.Tools;
using UnityEngine;

namespace GameCore
{
    //TODO: GOODBYE
    [NotSummonable]
    public class CoreNPC : NPC
    {
        public T LoadNPCData<T>(Func<JToken, T> Load) where T : NPCData
        {
            var cd = customData ?? new();

            //如果不存在就添加
            if (!cd.TryGetJToken("ori:npc", out JToken value))
            {
                cd.AddProperty("ori:npc", new JObject());
                value = cd["ori:npc"];
            }

            //修改 customData
            customData = cd;

            return Load(value);
        }

        public void SaveNPCData<T>(T newValue) where T : NPCData
        {
            var cd = customData ?? new();

            //如果存在就删掉
            if (cd.TryGetJToken("ori:npc", out JToken _))
                cd.Remove("ori:npc");

            //添加数据
            cd.AddProperty("ori:npc", JsonTools.LoadJObjectByString(JsonTools.ToJson(newValue)));

            //修改 customData
            customData = cd;
        }
    }

    [Serializable]
    public class NPCData
    {

    }
}