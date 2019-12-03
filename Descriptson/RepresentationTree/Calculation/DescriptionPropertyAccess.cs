using System;
using System.Collections.Generic;
using System.Reflection;

namespace Descriptson.RepresentationTree.Calculation
{
    public class DescriptionPropertyAccess<TTarget, TResult> : IDescriptsonCalculation<TTarget, TResult>
    {
        public PropertyInfo Property { get; }

        public TResult Calculate(TTarget target)
        {
            return (TResult)Property.GetValue(target);
        }
    }
}