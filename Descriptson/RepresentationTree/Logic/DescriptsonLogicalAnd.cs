using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

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

        public static DescriptsonLogicalElement<TTarget> CreateFrom(JsonReader reader)
        {
            return new DescriptsonLogicalAnd<TTarget>(GetSubExpressions(reader));
        }

        public override bool Test(TTarget target)
        {
            return SubExpressions.All(subExpression => subExpression.Test(target));
        }
    }
}