using System;
using System.Collections.Generic;
using Descriptson.RepresentationTree;

namespace Descriptson
{
    public class DescriptsonCalculated<TContainer, TResult>
    {
        private readonly TContainer instance;
        public IDescriptsonCalculation<TContainer, TResult> Calculation { get; }
        public TResult Value { get { return Calculation.Calculate(instance); } }

        public DescriptsonCalculated(TContainer container, IDescriptsonCalculation<TContainer, TResult> calculation)
        {
            instance = container;
            Calculation = calculation;
        }

        public static implicit operator TResult(DescriptsonCalculated<TContainer, TResult> calculatedValue)
        {
            return calculatedValue.Value;
        }
    }
}