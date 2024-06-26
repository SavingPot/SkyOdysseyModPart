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

    public class TreeSeed : Block
    {
        public static Dictionary<string, TreeSeedDatum> dataTemps = new();
        public TreeSeedDatum treeSeedDatum;
        public string randomUpdateID;

        public override void DoStart()
        {
            base.DoStart();

            randomUpdateID = $"ori:tree_seeds_{gameObject.GetInstanceID()}";
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

            //TODO: 考虑周围有无障碍物, 若有则不生成
            //应该以下面的方块位置来运算, 这样才和群系生成一致
            Vector2Int downPos = pos + Vector2Int.down;

            //放置方块
            chunk.map.GenerateStructure(treeSeedDatum.structure, downPos);

            //注：这里并不需要删除树种，因为它会被木头直接替换
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
