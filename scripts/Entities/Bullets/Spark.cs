using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GameCore
{
    [EntityBinding(EntityID.Spark)]
    public class Spark : Bullet
    {
        Light2D light;



        protected override void Awake()
        {
            base.Awake();
        }


        public override void Initialize()
        {
            base.Initialize();

            //TODO: 灼伤
            damage = 5;

            AddSpriteRenderer("ori:spark");

            //创建子物体
            light = Instantiate(GInit.instance.GetLightPrefab());
            light.gameObject.name = "ori:spark_light";
            light.color = new(1, 0.5f, 0.5f, 1);
            light.pointLightOuterRadius = 2f;
            light.intensity = 0.3f;
        }

        protected override void Update()
        {
            base.Update();

            SetScaleByTime();
            LookAtDirection();

            light.transform.position = transform.position;
        }

        void SetScaleByTime()
        {
            float value = Math.Max(0, (timeToAutoDestroy - Tools.time) / Init.data.lifetime);
            transform.localScale = new(value, value);
        }





        public override void Hide()
        {
            base.Hide();

            light.enabled = false;
        }

        public override void Show()
        {
            base.Show();

            light.enabled = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Destroy(light.gameObject);
        }
    }
}