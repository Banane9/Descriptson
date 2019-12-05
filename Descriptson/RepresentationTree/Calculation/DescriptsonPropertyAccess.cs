using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace Descriptson.RepresentationTree.Calculation
{
    public class DescriptsonPropertyAccess<TTarget, TResult> : IDescriptsonCalculation<TTarget, TResult>
    {
        private readonly Func<TTarget, object> getValue;
        public Expression AccessExpression { get; }

        public DescriptsonPropertyAccess(Expression<Func<TTarget, object>> accessExpression)
            : base()
        {
            AccessExpression = accessExpression.Body;
            getValue = accessExpression.Compile();
        }

        public static DescriptsonPropertyAccess<TTarget, TResult> CreateFrom(IEnumerable<JToken> jTokens)
        {
            var getValue = DescriptsonPropertyManager<TTarget>.ParseAccessPath(jTokens.Single().Value<string>());

            return new DescriptsonPropertyAccess<TTarget, TResult>(getValue);
        }

        public TResult Calculate(TTarget target)
        {
            return (TResult)getValue(target);
        }
    }
}