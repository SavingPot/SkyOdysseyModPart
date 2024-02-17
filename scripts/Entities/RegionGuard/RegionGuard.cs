using UnityEngine;

namespace GameCore
{
    [NotSummonable]
    public abstract class RegionGuard : Entity
    {
        public float motionDiameter = 20;
        public Vector3 originPosition;
        public ParticleSystem particleSystem;
        public BiomeGuardParticle particleScript;

        public override void Initialize()
        {
            base.Initialize();

            originPosition = transform.position;

            //TODO: pool-ify
            particleSystem = GameObject.Instantiate(GInit.instance.BiomeGuardParticleSystemPrefab, transform);
            particleSystem.transform.localPosition = Vector3.zero;
            particleSystem.textureSheetAnimation.AddSprite(ModFactory.CompareTexture("ori:biome_guard_particle").sprite);
            particleSystem.gameObject.AddComponent<BiomeGuardParticle>();
            Component.Destroy(rb);
        }

        protected override void ServerUpdate()
        {
            base.ServerUpdate();

            /* ----------------------------------- 运动 ----------------------------------- */
            float xDelta = Mathf.PerlinNoise1D(Time.time * 0.5f) - 0.5f; //from -0.5 to 0.5
            float yDelta = Mathf.PerlinNoise1D((Time.time + 10) * 0.5f) - 0.5f;
            Vector3 delta = new(xDelta * motionDiameter, yDelta * motionDiameter);
            gameObject.transform.position = originPosition + delta;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (particleSystem != null)
                GameObject.Destroy(particleSystem.gameObject);
        }
    }
}