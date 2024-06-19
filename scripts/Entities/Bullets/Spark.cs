using System;
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

            rb.velocity = customData["ori:bullet"]["velocity"].ToVector2();

            //TODO: 灼伤
            damage = 5;

            AddSpriteRenderer("ori:spark");

            light = gameObject.AddComponent<Light2D>();
            light.color = new(1, 0.5f, 0.5f, 1);
            light.pointLightOuterRadius = 3f;
            light.intensity = 0.4f;
        }

        protected override void Update()
        {
            base.Update();

            SetScaleByTime();
            LookAtDirection();
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
    }
}