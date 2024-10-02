using System.Collections;
using GameCore.High;
using SP.Tools.Unity;
using UnityEngine;

namespace GameCore
{
    public class PickedBerryBush : Plant
    {
        public const int pickCD = 90;
        public Coroutine setterCoroutine;

        public override void DoStart()
        {
            base.DoStart();

            setterCoroutine = CoroutineStarter.instance.StartCoroutine(IESetToNotPicked());
        }

        IEnumerator IESetToNotPicked()
        {
            //等待 CD
            yield return new WaitForSeconds(pickCD);

            //设置为未采集过
            chunk.map.SetBlockNet(pos, isBackground, BlockStatus.Normal, BlockID.BerryBush, null);
        }

        public override void OnRecovered()
        {
            base.OnRecovered();

            if (setterCoroutine != null)
                CoroutineStarter.instance.StopCoroutine(setterCoroutine);
        }
    }
}