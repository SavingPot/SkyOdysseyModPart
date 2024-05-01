using UnityEngine;

namespace GameCore
{
    public class QuickGrowDecorator : CropDecorator
    {
        public override float DecideGrowProbability(Block underBlock) => crop.DecideGrowProbability(underBlock) * 1.15f;
        public override void Grow() => crop.Grow();
        public override HarvestResult[] HarvestResults(Vector3 pos) => crop.HarvestResults(pos);



        public QuickGrowDecorator(ICrop crop, CropBlock block) : base(crop, block) { }
    }

    public class DoubleHarvestDecorator : CropDecorator
    {
        public override float DecideGrowProbability(Block underBlock) => crop.DecideGrowProbability(underBlock);
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



        public DoubleHarvestDecorator(ICrop crop, CropBlock block) : base(crop, block) { }
    }
}