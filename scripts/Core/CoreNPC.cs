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

            if (!cd.TryGetValue("ori:npc", out JToken value))
            {
                cd.Add(new JProperty("ori:npc", new JObject()));
                value = cd["ori:npc"];
            }

            customData = cd;

            return Load(value);
        }

        public void SaveNPCData<T>(T newValue) where T : NPCData
        {
            var cd = customData ?? new();

            if (cd.TryGetValue("ori:npc", out JToken _))
                cd.Remove("ori:npc");

            cd.Add(new JProperty("ori:npc", JsonTools.LoadJObjectByString(JsonTools.ToJson(newValue))));
            customData = cd;
        }
    }

    [Serializable]
    public class NPCData
    {

    }
}