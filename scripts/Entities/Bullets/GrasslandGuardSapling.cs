using SP.Tools.Unity;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.GrasslandGuardSapling)]
    public class GrasslandGuardSapling : Bullet
    {
        public override void Initialize()
        {
            base.Initialize();
            
            damage = 0;
        }
        
        public override void BlockCollision(Block block)
        {
            if (!isDead)
            {
                base.BlockCollision(block);

                GM.instance.SummonEntity(transform.position, EntityID.OakTreeMan, Tools.randomGUID);
                Death();
            }
        }
    }
}