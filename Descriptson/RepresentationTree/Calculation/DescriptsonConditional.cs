using System;
using System.Collections.Generic;
using System.Linq;
using Descriptson.RepresentationTree.Logic;
using Newtonsoft.Json.Linq;

namespace Descriptson.RepresentationTree.Calculation
{
    public sealed class DescriptsonConditional<TTarget, TResult> : IDescriptsonCalculation<TTarget, TResult>
    {
        public IDescriptsonTest<TTarget> Condition { get; }
        public IDescriptsonCalculation<TTarget, TResult> WhenFalse { get; }
        public IDescriptsonCalculation<TTarget, TResult> WhenTrue { get; }

        public DescriptsonConditional(IDescriptsonTest<TTarget> condition, IDescriptsonCalculation<TTarget, TResult> whenTrue, IDescriptsonCalculation<TTarget, TResult> whenFalse)
        {
            Condition = condition;
            WhenTrue = whenTrue;
            WhenFalse = whenFalse;
        }

        public static DescriptsonConditional<TTarget, TResult> CreateFrom(IEnumerable<JToken> jTokens)
        {
            var jParameters = jTokens.ToArray();

            if (jParameters.Length != 3)
                throw new InvalidOperationException("An If Conditional needs exactly 3 parameters!");

            if (jParameters[0].Type != JTokenType.Object)
                throw new InvalidOperationException("The first parameter of an If Conditional needs to be an object!");

            var calculations = DescriptsonCalculation<TTarget, TResult>.GetSubCalculations(jParameters.Skip(1));

            return new DescriptsonConditional<TTarget, TResult>(
                DescriptsonLogicalAnd<TTarget>.CreateFrom((JObject)jParameters[0]),
                calculations.First(),
                calculations.Last());
        }

        public TResult Calculate(TTarget target)
        {
            if (Condition.Test(target))
                return WhenTrue.Calculate(target);

            return WhenFalse.Calculate(target);
        }
    }
}