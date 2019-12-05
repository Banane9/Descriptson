using System;
using System.Collections.Generic;

namespace Descriptson.RepresentationTree.Calculation
{
    public class DescriptsonLiteral<TTarget, TResult> : IDescriptsonCalculation<TTarget, TResult>
    {
        public TResult Value { get; }

        public DescriptsonLiteral(TResult value)
            : base()
        {
            Value = value;
        }

        public TResult Calculate(TTarget target)
        {
            return Value;
        }
    }
}