using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.GrasslandGuard)]
    public class GrasslandGuard : BiomeGuard
    {
        public float attackTimer;
        public int attackRadius = 10 * 10; // 10^2

        protected override void Update()
        {
            base.Update();

            if (Tools.time >= attackTimer)
            {
                foreach (var player in PlayerCenter.allReady)
                {
                    if ((player.transform.position - transform.position).sqrMagnitude <= attackRadius)
                    {

                    }
                }
            }
        }
    }
}