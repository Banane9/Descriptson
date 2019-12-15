using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Descriptson.RepresentationTree.Logic
{
    public class DescriptsonLogicalOr<TTarget> : DescriptsonLogicalElement<TTarget>
    {
        public DescriptsonLogicalOr(IEnumerable<IDescriptsonTest<TTarget>> subExpressions)
            : base(subExpressions)
        { }

        public DescriptsonLogicalOr(params IDescriptsonTest<TTarget>[] subExpressions)
            : base(subExpressions)
        { }

        public static DescriptsonLogicalElement<TTarget> CreateFrom(JsonReader reader)
        {
            return new DescriptsonLogicalOr<TTarget>(GetSubExpressions(reader));
        }

        public override bool Test(TTarget target)
        {
            return SubExpressions.Any(subExpression => subExpression.Test(target));
        }
    }
}