using System;
using Newtonsoft.Json.Linq;
using SP.Tools;
using UnityEngine;

namespace GameCore
{
    public abstract class CoreNPCProperties<T> : CoreCreatureProperties<T> where T : CoreNPCProperties<T>, new()
    {

    }

    //TODO: GOODBYE
    [NotSummonable]
    public class CoreNPC<PropertyT> : NPC
            where PropertyT : CoreNPCProperties<PropertyT>, new()
    {
        public T LoadNPCData<T>(Func<JToken, T> Load) where T : NPCData
        {
            if (!customData.TryGetValue("ori:npc", out JToken value))
            {
                customData.Add(new JProperty("ori:npc", new JObject()));
                value = customData["ori:npc"];
            }

            return Load(value);
        }

        public void SaveNPCData<T>(T newValue) where T : NPCData
        {
            if (customData.TryGetValue("ori:npc", out JToken _))
                customData.Remove("ori:npc");

            customData.Add(new JProperty("ori:npc", JsonTools.LoadJObjectByString(JsonTools.ToJson(newValue))));
        }
    }

    [Serializable]
    public class NPCData
    {

    }
}