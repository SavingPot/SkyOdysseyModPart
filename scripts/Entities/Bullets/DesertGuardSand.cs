using SP.Tools.Unity;
using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.DesertGuardSand)]
    public class DesertGuardSand : Bullet
    {
        public override void Initialize()
        {
            base.Initialize();

            destroyOnCollideWithBlock = false;
            damage = 20;

            AddSpriteRenderer("ori:desert_guard_sand");
        }
    }
}