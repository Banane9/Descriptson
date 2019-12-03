using System;
using System.Collections.Generic;
using System.Reflection;

namespace Descriptson.RepresentationTree.Test
{
    public class DescriptsonPropertyTest<TTarget, TValue> : IDescriptsonTest<TTarget>
    {
        public DescriptsonComparisonType ComparisonType { get; }
        public PropertyInfo Property { get; }
        public TValue Value { get; }

        internal DescriptsonPropertyTest()
        {
        }

        public bool Test(TTarget target)
        {
            // Test stuff lol
            throw new NotImplementedException();
        }
    }
}