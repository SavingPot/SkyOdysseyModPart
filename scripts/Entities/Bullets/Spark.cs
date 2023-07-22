using UnityEngine.Rendering.Universal;

namespace GameCore
{
    [EntityBinding(EntityID.Spark)]
    public class Spark : Bullet
    {
        protected override void Awake()
        {
            base.Awake();

            livingTime = 1;
        }

        protected override void Start()
        {
            base.Start();
            WhenCorrectedSyncVars(() =>
            {
                rb.velocity = customData["ori:bullet"]["velocity"].ToVector2();
            });

            //TODO: 灼伤
            damage = 5;
            AddSpriteRenderer("ori:spark");

            var light = gameObject.AddComponent<Light2D>();
            light.color = new(1, 0.5f, 0.5f, 1);
            light.pointLightOuterRadius = 3f;
            light.intensity = 0.4f;
        }

        protected override void Update()
        {
            base.Update();

            LookAtDirection();
        }
    }
}