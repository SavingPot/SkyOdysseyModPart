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

            //获取组方块中除了自己的其他部分
            List<GroupBlockDatum_Part> partsWithoutSelf = new(block.groupPosTemp.Length);
            foreach (var part in datum.parts)
                if (part.offset != block.offset)
                    partsWithoutSelf.Add(part);


            //遍历获得到的部分
            for (int i = 0; i < partsWithoutSelf.Count; i++)
            {
                var entourageOffset = partsWithoutSelf[i].offset;
                var entouragePos = block.pos - block.offset + entourageOffset;
                block.groupPosTemp[i] = entouragePos;

                if (block.isGroupCenter)
                {
                    //如果存档中不存在子方块
                    if (!GFiles.world.TryGetRegion(block.chunk.regionIndex, out var region) ||
                        region.GetBlock(block.data.id, block.isBackground)?.GetLocation(entouragePos.x, entouragePos.y) == null)
                    {
                        //创建子方块
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
        public static void SpreadOnUpdate<T>(T block) where T : Block, IGroupBlock
        {
            if (block.isGroupCenter)
            {
                block.UpdateWithoutSpread();

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
                //让中心方块更新 (中心方块的更新会传播到子方块)
                var group = Map.instance.GetBlock(block.pos - block.offset, block.isBackground);

                group?.OnUpdate();
            }
        }

        public static void SpreadOnRecovered<T>(T block) where T : Block, IGroupBlock
        {
            //回收整个组
            foreach (var groupPos in block.groupPosTemp)
            {
                //获取同组方块
                var group = Map.instance.GetBlock(groupPos, block.isBackground);

                //检查空值
                if (group is null)
                {
                    Debug.LogError($"位于 {block.pos} 的方块 {block.data.id} 的组方块 {groupPos} 不存在!");
                    return;
                }

                //检查类型
                if (group is T groupAsT)
                {
                    if (!groupAsT.hasBeenSpreadOnRecovered)
                    {
                        groupAsT.hasBeenSpreadOnRecovered = true;
                        Map.instance.RemoveBlock(groupPos, block.isBackground, true, true);
                    }
                }
                else
                {
                    Debug.LogError($"位于 {block.pos} 的方块 {block.data.id} 的组方块 {groupPos} 与其组成员类型不一致!");
                    return;
                }
            }
        }
    }

    public interface IGroupBlock
    {
        Vector2Int[] groupPosTemp { get; set; }
        Vector2Int offset { get; set; }
        bool isGroupCenter { get; set; }
        bool hasBeenSpreadOnRecovered { get; set; }
        void UpdateWithoutSpread();
        void OnRecoveredWithoutSpread();
    }
}