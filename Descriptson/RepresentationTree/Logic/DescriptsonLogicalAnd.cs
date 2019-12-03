using System;
using System.Collections.Generic;
using System.Linq;

namespace Descriptson.RepresentationTree.Logic
{
    public class DescriptsonLogicalAnd<TTarget> : DescriptsonLogicalElement<TTarget>
    {
        public override bool Test(TTarget target)
        {
            return SubExpressions.All(subExpression => subExpression.Test(target));
        }
    }
}