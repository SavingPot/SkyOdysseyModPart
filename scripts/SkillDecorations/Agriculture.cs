using UnityEngine;

namespace GameCore
{
    public class DoubleHarvestDecorator : CropDecorator
    {
        public override void Grow() => crop.Grow();

        public override HarvestResult[] HarvestResults(Vector3 pos)
        {
            HarvestResult[] results = crop.HarvestResults(pos);

            //有 10% 的几率使结果数量翻倍
            if (Tools.Prob100(10))
            {
                for (int i = 0; i < results.Length; i++)
                {
                    var result = results[i];

                    //数量翻倍
                    result.count = (ushort)Mathf.Min(result.count * 2, ModFactory.CompareItem(result.id).maxCount);

                    results[i] = result;
                }
            }

            return results;
        }



        public DoubleHarvestDecorator(ICrop crop) : base(crop) { }
    }
}