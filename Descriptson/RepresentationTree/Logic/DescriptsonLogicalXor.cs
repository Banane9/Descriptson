using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Descriptson.RepresentationTree.Logic
{
    public class DescriptsonLogicalXOr<TTarget> : DescriptsonLogicalElement<TTarget>
    {
        public DescriptsonLogicalXOr(IEnumerable<IDescriptsonTest<TTarget>> subExpressions)
            : base(subExpressions)
        { }

        public DescriptsonLogicalXOr(params IDescriptsonTest<TTarget>[] subExpressions)
            : base(subExpressions)
        { }

        public static DescriptsonLogicalElement<TTarget> CreateFrom(JsonReader reader)
        {
            return new DescriptsonLogicalXOr<TTarget>(GetSubExpressions(reader));
        }

        public override bool Test(TTarget target)
        {
            return SubExpressions.Count(subExpression => subExpression.Test(target)) == 1;
        }
    }
}