using UnityEngine;

namespace GameCore
{
    public class QuickGrowDecorator : CropDecorator
    {
        public override float DecideGrowProbability(Block underBlock) => crop.DecideGrowProbability(underBlock) * 1.15f;
        public override void Grow() => crop.Grow();
        public override DropResult[] CutResults(Vector3 pos) => crop.CutResults(pos);
        public override DropResult[] HarvestResults(Vector3 pos) => crop.HarvestResults(pos);



        public QuickGrowDecorator(ICrop crop, CropBlock block) : base(crop, block) { }
    }

    public class DoubleHarvestDecorator : CropDecorator
    {
        public override float DecideGrowProbability(Block underBlock) => crop.DecideGrowProbability(underBlock);
        public override void Grow() => crop.Grow();
        public override DropResult[] CutResults(Vector3 pos) => crop.CutResults(pos);
        public override DropResult[] HarvestResults(Vector3 pos)
        {
            DropResult[] results = crop.HarvestResults(pos);

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

    public class CoinDecorator : CropDecorator
    {
        public override float DecideGrowProbability(Block underBlock) => crop.DecideGrowProbability(underBlock);
        public override void Grow() => crop.Grow();
        public override DropResult[] CutResults(Vector3 pos) => crop.CutResults(pos);
        public override DropResult[] HarvestResults(Vector3 pos)
        {
            //20% 的几率掉落 1 个硬币
            if (Tools.Prob100(20))
                GM.instance.SummonCoinEntity(pos, 1);

            return crop.HarvestResults(pos);
        }



        public CoinDecorator(ICrop crop, CropBlock block) : base(crop, block) { }
    }
}