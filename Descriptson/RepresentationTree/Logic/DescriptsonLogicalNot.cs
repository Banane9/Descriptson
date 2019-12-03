using System;
using System.Linq;
using System.Collections.Generic;

namespace Descriptson.RepresentationTree.Logic
{
    public class DescriptsonLogicalNot<TTarget> : DescriptsonLogicalElement<TTarget>
    {
        public DescriptsonLogicalElement<TTarget> SubExpression { get; }

        public override bool Test(TTarget target)
        {
            return SubExpressions[0].Test(target);
        }
    }
}