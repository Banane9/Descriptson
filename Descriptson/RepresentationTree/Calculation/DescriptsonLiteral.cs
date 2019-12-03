using System;
using System.Collections.Generic;

namespace Descriptson.RepresentationTree.Calculation
{
    public class DescriptsonLiteral<TTarget, TResult> : IDescriptsonCalculation<TTarget, TResult>
    {
        public TResult Value { get; }

        public TResult Calculate(TTarget target)
        {
            return Value;
        }
    }
}