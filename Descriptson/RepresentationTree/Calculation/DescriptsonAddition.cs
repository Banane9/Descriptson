using System;
using System.Collections.Generic;
using System.Linq;

namespace Descriptson.RepresentationTree.Calculation
{
    public class DescriptsonAddition<TTarget, TResult> : DescriptsonCalculation<TTarget, TResult>
    {
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