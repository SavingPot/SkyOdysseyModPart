using SP.Tools.Unity;
using UnityEngine;

namespace GameCore
{
    public class Arrow : Bullet
    {
        public override void Initialize()
        {
            base.Initialize();

            rb.velocity = customData["ori:bullet"]["velocity"].ToVector2();
        }

        protected override void Update()
        {
            base.Update();

            LookAtDirection();
        }
    }
}