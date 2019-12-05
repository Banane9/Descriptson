using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Descriptson.RepresentationTree.Logic
{
    public class DescriptsonLogicalAnd<TTarget> : DescriptsonLogicalElement<TTarget>
    {
        public DescriptsonLogicalAnd(IEnumerable<IDescriptsonTest<TTarget>> subExpressions)
            : base(subExpressions)
        { }

        public DescriptsonLogicalAnd(params IDescriptsonTest<TTarget>[] subExpressions)
            : base(subExpressions)
        { }

        public static DescriptsonLogicalElement<TTarget> CreateFrom(JObject jObject)
        {
            return new DescriptsonLogicalAnd<TTarget>(GetSubExpressions(jObject));
        }

        public override bool Test(TTarget target)
        {
            return SubExpressions.All(subExpression => subExpression.Test(target));
        }
    }
}