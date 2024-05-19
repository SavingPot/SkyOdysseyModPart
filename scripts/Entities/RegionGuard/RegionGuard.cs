using UnityEngine;

namespace GameCore
{
    [NotSummonable]
    public abstract class BiomeGuard : Entity
    {
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
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (particleSystem != null)
                GameObject.Destroy(particleSystem.gameObject);
        }

        public override void Hide()
        {
            base.Hide();

            particleSystem.Stop();
        }

        public override void Show()
        {
            base.Show();

            particleSystem.Play();
        }
    }
}