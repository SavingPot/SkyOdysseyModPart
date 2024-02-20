using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.GrasslandSpore)]
    public class GrasslandSpore : Enemy
    {
        //TODO
        protected override void Update()
        {
            if (!targetTransform)
            {
                rb.velocity = Tools.GetAngleVector2(transform.position, targetTransform.position).normalized * 5;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<Creature>(out var creature))
            {
                creature.TakeDamage(10);
            }
        }

        public override Vector2 GetMovementDirection()
        {
            return Vector2.zero;
        }
    }
}