using GameCore.High;
using Newtonsoft.Json.Linq;
using SP.Tools;
using SP.Tools.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameCore
{
    public class TreeSeedDatum
    {
        public StructureData structure;
    }

    public class TreeSeedBlockBehaviour : Block
    {
        public static Dictionary<string, TreeSeedDatum> dataTemps = new();
        public TreeSeedDatum treeSeedDatum;
        public string randomUpdateID;

        public override void DoStart()
        {
            base.DoStart();

            randomUpdateID = $"ori:tree_seeds_{GetInstanceID()}";
            treeSeedDatum = GetDatum();

            RandomUpdater.Bind(randomUpdateID, 0.2f, Grow);
        }

        public override void OnRecovered()
        {
            base.OnRecovered();

            RandomUpdater.Unbind(randomUpdateID);
        }

        void Grow()
        {
            if (treeSeedDatum.structure == null)
                return;

            //应该以下面的方块位置来运算, 这样才和群系生成一致
            Vector2Int downPos = pos + Vector2Int.down;

            //放置方块
            foreach (var structBlock in treeSeedDatum.structure.fixedBlocks)
            {
                chunk.map.SetBlockNet(downPos + structBlock.offset, structBlock.layer, structBlock.blockId, null);
            }

            //销毁自己
            chunk.map.DestroyBlockNet(pos, layer);
        }



        public TreeSeedDatum GetDatum()
        {
            if (dataTemps.TryGetValue(data.id, out TreeSeedDatum value))
            {
                return value;
            }

            //添加默认值
            TreeSeedDatum treeDatum = new();

            if (GameTools.CompareVersions(data.jsonFormat, "0.7.4", Operators.thanOrEqual))
            {
                JToken jt = data.jo?["ori:tree_seed"];
                JToken prop = jt?["property"];
                string structId = prop?["structure"]?.ToString();

                if (!string.IsNullOrEmpty(structId)) treeDatum.structure = ModFactory.CompareStructure(structId);
            }

            dataTemps.Add(data.id, treeDatum);
            return treeDatum;
        }
    }
}
