using SP.Tools;
using UnityEngine;

namespace GameCore
{
    public abstract class Poultry : Livestock
    {
        public string eggId;

        public override void Initialize()
        {
            base.Initialize();

            if (isServer && !eggId.IsNullOrWhiteSpace())
            {
                RandomUpdater.Bind(gameObject.GetInstanceID().ToString(), 0.5f, () => GM.instance.SummonDrop(transform.position, eggId));
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (isServer)
            {
                RandomUpdater.Unbind(gameObject.GetInstanceID().ToString());
            }
        }
    }
}