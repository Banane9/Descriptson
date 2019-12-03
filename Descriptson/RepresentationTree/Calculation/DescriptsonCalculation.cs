using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Descriptson.RepresentationTree.Calculation
{
    public abstract class DescriptsonCalculation<TTarget, TResult> : IDescriptsonCalculation<TTarget, TResult>
    {
        public ReadOnlyCollection<IDescriptsonCalculation<TTarget, TResult>> Parameters { get; }

        internal DescriptsonCalculation()
        {
        }

        public abstract TResult Calculate(TTarget target);
    }
}