using SP.Tools.Unity;
using UnityEngine;

namespace GameCore
{
    public class Arrow : Bullet
    {
        protected override void Update()
        {
            base.Update();

            LookAtDirection();
        }
    }
}