using UnityEngine;

namespace GameCore
{
    public class BiomeGuardParticle : MonoBehaviour
    {
        private void OnParticleCollision(GameObject other)
        {
            if (other.TryGetComponent(out Entity entity) && entity is not BiomeGuard)
            {
                entity.TakeDamage(2, 0.2f, transform.position, Vector2.zero);
            }
        }
    }
}