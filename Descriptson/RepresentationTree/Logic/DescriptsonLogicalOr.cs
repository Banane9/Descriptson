using System;
using System.Collections.Generic;
using System.Linq;

namespace Descriptson.RepresentationTree.Logic
{
    public class DescriptsonLogicalOr<TTarget> : DescriptsonLogicalElement<TTarget>
    {
        public override bool Test(TTarget target)
        {
            return SubExpressions.Any(subExpression => subExpression.Test(target));
        }
    }
}