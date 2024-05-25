using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.GrasslandSpore)]
    public class GrasslandSpore : Enemy
    {
        //TODO
        protected override void Update()
        {
            if (!targetEntity)
            {
                rb.velocity = AngleTools.GetAngleVector2(transform.position, targetEntity.transform.position).normalized * 5;
            }
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);

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