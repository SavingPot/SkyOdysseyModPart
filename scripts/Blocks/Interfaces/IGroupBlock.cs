using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SP.Tools;
using SP.Tools.Unity;
using UnityEngine;

namespace GameCore
{
    public sealed class GroupBlockDatum : ModClass
    {
        public GroupBlockDatum_Part[] parts;
    }

    public sealed class GroupBlockDatum_Part
    {
        public Vector2Int offset;
        public string texture;
    }

    public static class GroupBlockBehaviour
    {
        static readonly Dictionary<string, GroupBlockDatum> dataTemps = new();


        public static GroupBlockDatum GetDatum(BlockData data)
        {
            if (dataTemps.TryGetValue(data.id, out var value))
            {
                return value;
            }

            //添加默认值
            GroupBlockDatum newDatum = new();

            if (GameTools.CompareVersions(data.jsonFormat, "0.7.8", Operators.thanOrEqual))
            {
                JToken cropJT = data.jo?["ori:group_block"];

                if (cropJT != null && cropJT.Type == JTokenType.Object)
                {
                    var parts = cropJT["parts"];

                    if (parts == null || parts.Type != JTokenType.Array || parts.Count() == 0)
                    {
                        Debug.LogError($"加载组方块失败: 组方块 {data.id} 的 json 文件中不包含 ori:group_block/parts 或者 parts 不是数组!");
                        return null;
                    }
                    else
                    {
                        List<GroupBlockDatum_Part> partsList = new();

                        parts.For(j =>
                        {
                            partsList.Add(new()
                            {
                                offset = j["offset"].ToVector2Int(),
                                texture = j["texture"].ToString(),
                            }); ;
                        });

                        if (partsList[0].offset != Vector2Int.zero)
                        {
                            Debug.LogError($"加载组方块失败: 组方块 {data.id} 的 json 文件中 ori:group_block/parts 的一个元素偏移必须为 [0, 0]!");
                            return null;
                        }

                        newDatum.parts = partsList.ToArray();
                    }
                }
                else
                {
                    Debug.LogError($"加载组方块失败: 组方块 {data.id} 的 json 文件中不包含 ori:group_block 或者 ori:group_block 不是对象!");
                    return null;
                }
            }
            else
            {
                Debug.LogError($"加载组方块失败: 方块 {data.id} 的 json 格式版本 {data.jsonFormat} 不受支持!");
                return null;
            }

            dataTemps.Add(data.id, newDatum);
            return newDatum;
        }





        public static void DoStart<T>(T block) where T : Block, IGroupBlock
        {
            var datum = GetDatum(block.data);

            /* ----------------------------- 决定该方块是否是组方块的中心 ----------------------------- */
            block.customData ??= new();
            var offsetJson = block.customData["offset"];
            if (offsetJson == null)
            {
                block.customData.AddProperty("offset", VectorConverter.ToIntArray(Vector2Int.zero));
                block.WriteCustomDataToSave();
                block.offset = Vector2Int.zero;
                block.isGroupCenter = true;
            }
            else
            {
                block.offset = offsetJson.ToVector2Int();
                block.isGroupCenter = block.offset == Vector2Int.zero;
            }


            /* ------------------------------ 找到当前偏移值对应的贴图 ------------------------------ */
            foreach (var part in datum.parts)
            {
                if (part.offset == block.offset)
                {
                    block.sr.sprite = ModFactory.CompareTexture(part.texture).sprite;
                    break;
                }
            }


            /* --------------------------------- 补全方块组 (放下非中心方块) -------------------------------- */
            block.groupPosTemp = new Vector2Int[datum.parts.Length - 1];

            //注意起始索引是 1
            for (int i = 1; i < datum.parts.Length; i++)
            {
                var entourageOffset = datum.parts[i].offset;
                var entouragePos = block.pos + entourageOffset;
                block.groupPosTemp[i - 1] = entouragePos;

                if (block.isGroupCenter)
                {
                    if (!Map.instance.HasBlock(entouragePos, block.isBackground))
                    {
                        JObject customData = new();
                        customData.AddProperty("offset", VectorConverter.ToIntArray(entourageOffset));
                        Map.instance.SetBlock(entouragePos, block.isBackground, block.data, customData.ToString(Formatting.None), true, false);
                    }
                }
            }
        }



        /// <summary>
        /// 这个方法的作用是传播更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="block"></param>
        public static void OnUpdate<T>(T block) where T : Block, IGroupBlock
        {
            block.UpdateWithoutSpread();

            if (block.isGroupCenter)
            {
                //让子方块更新
                foreach (var groupPos in block.groupPosTemp)
                {
                    var group = Map.instance.GetBlock(groupPos, block.isBackground);

                    if (group != null)
                    {
                        ((IGroupBlock)group).UpdateWithoutSpread();
                    }
                }
            }
            else
            {
                //让中心方块更新
                var group = Map.instance.GetBlock(block.pos - block.offset, block.isBackground);

                if (group != null)
                {
                    ((IGroupBlock)group).UpdateWithoutSpread();
                }
            }
        }
    }

    public interface IGroupBlock
    {
        Vector2Int[] groupPosTemp { get; set; }
        Vector2Int offset { get; set; }
        bool isGroupCenter { get; set; }
        void UpdateWithoutSpread();
    }
}