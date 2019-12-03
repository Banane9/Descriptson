using System;
using System.Collections.Generic;
using Descriptson.RepresentationTree.Logic;

namespace Descriptson.RepresentationTree.Calculation
{
    public sealed class DescriptsonConditional<TTarget, TResult> : DescriptsonCalculation<TTarget, TResult>
    {
        private readonly DescriptsonLogicalElement<TTarget> condition;
        private readonly DescriptsonCalculation<TTarget, TResult> whenFalse;
        private readonly DescriptsonCalculation<TTarget, TResult> whenTrue;

        internal DescriptsonConditional()
        {
            whenTrue = whenTrue;
            whenFalse = whenFalse;
        }

        public override TResult Calculate(TTarget target)
        {
            if (condition.Test(target))
                return whenTrue.Calculate(target);
            else
                return whenFalse.Calculate(target);
        }
    }
}