using System;
using System.Collections.Generic;

namespace Descriptson.RepresentationTree.Logic
{
    public class DescriptsonLogicalXor<TTarget> : DescriptsonLogicalElement<TTarget>
    {
        public override bool Test(TTarget target)
        {
            return SubExpressions[0].Test(target) ^ SubExpressions[1].Test(target);
        }
    }
}