using System.Collections.Generic;
using GameCore.Network;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GameCore
{
    public class TreeLogBlock : Block
    {
        public SpriteRenderer leaveRenderer;

        public override void DoStart()
        {
            base.DoStart();

            treeLogBlockDatum = GetDatum(this);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            RecoverLeaveRenderer();

            if (chunk.map.GetBlock(new(pos.x, pos.y + 1), isBackground)?.data?.id != data.id)
            {
                leaveRenderer = LitSpriteRendererPool.Get(sr.sortingOrder);
                leaveRenderer.sprite = ModFactory.CompareTexture(treeLogBlockDatum.leaf).sprite;
                leaveRenderer.transform.localPosition = new(pos.x, pos.y + 1.5f, 0);
            }
            else
            {

            }
        }

        public override void OnRecovered()
        {
            base.OnRecovered();



            //回收方块上方的木头
            if (Server.isServer)
            {
                var upperPosVector = new Vector2Int(pos.x, pos.y + 1);
                var upperBlock = Map.instance.GetBlock(upperPosVector, isBackground);

                //如果也是木头，就销毁
                if (upperBlock != null && upperBlock.data.id == data.id)
                {
                    upperBlock.Destroy();
                }
            }



            RecoverLeaveRenderer();
        }

        void RecoverLeaveRenderer()
        {
            if (leaveRenderer)
            {
                LitSpriteRendererPool.Recycle(leaveRenderer);
                leaveRenderer = null;
            }
        }




        public static Dictionary<string, TreeLogBlockDatum> dataTemps = new();
        public TreeLogBlockDatum treeLogBlockDatum;

        public class TreeLogBlockDatum
        {
            public string leaf;
        }

        public static TreeLogBlockDatum GetDatum(Block block)
        {
            var data = block.data;

            if (dataTemps.TryGetValue(data.id, out TreeLogBlockDatum value))
            {
                return value;
            }

            //添加默认值
            TreeLogBlockDatum logDatum = new();

            if (true)//(GameTools.CompareVersions(datum.jsonFormat, "0.6.2", Operators.thanOrEqual))
            {
                JToken root = data.jo["ori:tree_log"];

                logDatum.leaf = root["leaf"].ToString();
            }

            dataTemps.Add(data.id, logDatum);
            return logDatum;
        }
    }
}