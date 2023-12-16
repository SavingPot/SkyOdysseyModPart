using System.Collections;
using GameCore.High;
using UnityEngine;

namespace GameCore
{
    public class PickedBerryBushBehaviour : Block
    {
        public const int pickCD = 90;
        public Coroutine setterCoroutine;

        public override void DoStart()
        {
            base.DoStart();

            StartCoroutine(IESetToNotPicked());
        }

        IEnumerator IESetToNotPicked()
        {
            //等待 CD
            yield return new WaitForSeconds(pickCD);

            //设置为未采集过
            chunk.map.SetBlockNet(pos, isBackground, BlockID.BerryBush, null);
        }

        public override void OnRecovered()
        {
            base.OnRecovered();

            if (setterCoroutine != null)
                StopCoroutine(setterCoroutine);
        }
    }
}