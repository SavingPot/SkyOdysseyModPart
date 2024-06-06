using System.Collections;
using GameCore.High;
using SP.Tools;
using UnityEngine;

namespace GameCore
{
    public class Flowerpot : Block
    {
        SpriteRenderer plantRenderer;
        string plantId;

        public override void DoStart()
        {
            base.DoStart();

            //初始化植物渲染器
            plantRenderer = ObjectTools.CreateSpriteObject(transform, "plant");
            plantRenderer.transform.localPosition = new(0, 0.15f);
            plantRenderer.transform.localScale = new(0.7f, 0.7f);
            plantRenderer.sortingOrder = sr.sortingOrder;
            plantRenderer.color = sr.color;

            //刷新植物渲染器
            RefreshPlantRenderer();
        }

        void RefreshPlantRenderer()
        {
            if (plantId.IsNullOrWhiteSpace())
                plantRenderer.sprite = null;
            else
                plantRenderer.sprite = ModFactory.CompareTexture(plantId).sprite;
        }

        public override void OnRecovered()
        {
            //让种的植物掉落
            DropPlantedPlant();

            base.OnRecovered();
        }

        void DropPlantedPlant()
        {
            if (plantId.IsNullOrWhiteSpace())
                return;

            GM.instance.SummonDrop(transform.position, plantId);
        }

        public override bool PlayerInteraction(Player caller)
        {
            //种植物
            if (caller.TryGetUsingItem(out Item item) && item.data.GetTag("ori:plant").hasTag)
            {
                //令原有的植物掉落
                DropPlantedPlant();

                //种入新的植物
                plantId = item.data.id;
                caller.ServerReduceUsingItemCount(1);
                RefreshPlantRenderer();

                //播放音效
                GAudio.Play(AudioID.PlaceBlock);
                return true;
            }

            return false;
        }
    }
}