using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameCore
{
    [EntityBinding(EntityID.WoodenArrow)]
    public class WoodenArrow : Arrow
    {
        protected override void Start()
        {
            base.Start();

            damage = 10;
            AddSpriteRenderer("ori:wooden_arrow");
        }
    }
}