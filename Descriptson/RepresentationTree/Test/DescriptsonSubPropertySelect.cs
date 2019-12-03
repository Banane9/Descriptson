using System;
using System.Collections.Generic;
using System.Reflection;

namespace Descriptson.RepresentationTree.Test
{
    public class DescriptsonSubPropertySelect<TTarget, TSubTarget> : IDescriptsonTest<TTarget>
    {
        public PropertyInfo Property { get; }

        public IDescriptsonTest<TSubTarget> SubExpression { get; }

        public bool Test(TTarget target)
        {
            return SubExpression.Test((TSubTarget)Property.GetValue(target));
        }
    }
}