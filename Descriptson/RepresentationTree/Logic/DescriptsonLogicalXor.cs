using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

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

        public override DescriptsonLogicalElement<TTarget> CreateFrom(JObject jObject)
        {
            return new DescriptsonLogicalXOr<TTarget>(GetSubExpressions(jObject));
        }

        public override bool Test(TTarget target)
        {
            return SubExpressions.Count(subExpression => subExpression.Test(target)) == 1;
        }
    }
}