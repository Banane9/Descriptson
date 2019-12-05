using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Descriptson.RepresentationTree.Calculation
{
    public class DescriptsonAddition<TTarget, TResult> : DescriptsonCalculation<TTarget, TResult>
    {
        public DescriptsonAddition(IEnumerable<IDescriptsonCalculation<TTarget, TResult>> parameters)
            : base(parameters)
        { }

        protected DescriptsonAddition(params IDescriptsonCalculation<TTarget, TTarget>[] parameters)
            : base(parameters)
        { }

        public static DescriptsonCalculation<TTarget, TResult> CreateFrom(IEnumerable<JToken> jTokens)
        {
            return new DescriptsonAddition<TTarget, TResult>(GetSubCalculations(jTokens));
        }

        public override TResult Calculate(TTarget target)
        {
            var results = Parameters.Select(parameter => parameter.Calculate(target));

            switch (typeof(TResult).Name)
            {
                case "Int32":
                    return (TResult)(object)results.Cast<int>().Sum();

                case "String":
                    return (TResult)(object)string.Join("", results.Cast<string>());

                    // TODO: MOAR
            }

            return default(TResult);
        }
    }
}