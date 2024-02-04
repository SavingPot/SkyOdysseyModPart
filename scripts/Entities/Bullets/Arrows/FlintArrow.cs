using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameCore
{
    [EntityBinding(EntityID.FlintArrow)]
    public class FlintArrow : Arrow
    {
        public override void Initialize()
        {
            base.Initialize();

            damage = 10;
            AddSpriteRenderer("ori:flint_arrow");
        }
    }
}