using Mirror;
using UnityEngine;

namespace GameCore
{
    [NotSummonable]
    public abstract class RegionGuard : Creature
    {
        public ParticleSystem particleSystem;
        public BiomeGuardParticle particleScript;

        protected override void Start()
        {
            base.Start();

            //TODO: pool-ify
            particleSystem = GameObject.Instantiate(GInit.instance.BiomeGuardParticleSystemPrefab);
            particleSystem.transform.position = transform.position;
            particleSystem.textureSheetAnimation.AddSprite(ModFactory.CompareTexture("ori:biome_guard_particle").sprite);
            particleSystem.gameObject.AddComponent<BiomeGuardParticle>();
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

            if (particleSystem.isPlaying && isHurting)
                particleSystem.Stop();

            if (particleSystem.isStopped && !isHurting)
                particleSystem.Play();
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (particleSystem != null)
                GameObject.Destroy(particleSystem.gameObject);
        }
    }
}