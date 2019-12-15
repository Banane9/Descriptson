using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace Descriptson.RepresentationTree.Logic
{
    public class DescriptsonSubPropertySelect<TTarget, TSubTarget> : DescriptsonLogicalElement<TSubTarget>, IDescriptsonTest<TTarget>
    {
        private readonly Func<TTarget, TSubTarget> getValue;
        public Expression AccessExpression { get; }

        public DescriptsonSubPropertySelect(LambdaExpression getValue, JsonReader reader)
            : base(GetSubExpressions(reader))
        {
            AccessExpression = getValue.Body;
            this.getValue = (Func<TTarget, TSubTarget>)getValue.Compile();
        }

        public bool Test(TTarget target)
        {
            return Test(getValue(target));
        }

        public override bool Test(TSubTarget target)
        {
            return SubExpressions.All(subExpression => subExpression.Test(target));
        }
    }
}