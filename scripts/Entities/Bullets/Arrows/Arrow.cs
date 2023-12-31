using SP.Tools.Unity;
using UnityEngine;

namespace GameCore
{
    public class Arrow : Bullet
    {
        protected override void Start()
        {
            base.Start();

            rb.velocity = customData["ori:bullet"]["velocity"].ToVector2();
        }

        protected override void Update()
        {
            base.Update();

            LookAtDirection();
        }
    }
}