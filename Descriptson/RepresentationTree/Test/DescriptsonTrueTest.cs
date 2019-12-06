using System;
using System.Collections.Generic;

namespace Descriptson.RepresentationTree.Test
{
    public class DescriptsonTrueTest<TTarget> : IDescriptsonTest<TTarget>
    {
        public bool Test(TTarget target)
        {
            return true;
        }
    }
}