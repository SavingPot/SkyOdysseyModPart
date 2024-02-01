using SP.Tools;
using UnityEngine;

namespace GameCore
{
    public abstract class Poultry : Livestock
    {
        public string eggId;

        protected override void Start()
        {
            base.Start();

            if (isServer && !eggId.IsNullOrWhiteSpace())
            {
                RandomUpdater.Bind(gameObject.GetInstanceID().ToString(), 1, () => GM.instance.SummonDrop(transform.position, eggId));
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