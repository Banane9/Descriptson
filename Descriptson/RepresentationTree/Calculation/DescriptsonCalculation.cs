using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Descriptson.RepresentationTree.Calculation
{
    public abstract class DescriptsonCalculation
    {
        public static Dictionary<string, Type> Operations { get; } = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            { "add", typeof(DescriptsonAddition<,>) },
            { "if", typeof(DescriptsonConditional<,>) },
            { "read", typeof(DescriptsonPropertyAccess<,>) },
        };

        public static bool IsKnownOperator(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Must contain a name!", nameof(name));

            return Operations.ContainsKey(name);
        }

        public static object Make(Type targetType, Type resultType, JArray jArray)
        {
            if (jArray.First.Type != JTokenType.String)
                throw new InvalidOperationException("First element of the calculation array must be the operation's name!");

            var operation = jArray.First.Value<string>();

            if (string.IsNullOrWhiteSpace(operation))
                throw new ArgumentException("Must have an operation!", nameof(operation));

            if (!Operations.ContainsKey(operation))
                throw new InvalidOperationException($"No operator found with name [{operation}]!");

            return Operations[operation]
                .MakeGenericType(targetType, resultType)
                .GetMethod("CreateFrom")
                .Invoke(null, new[] { jArray.Children().Skip(1) });
        }
    }

    public abstract class DescriptsonCalculation<TTarget, TResult> : DescriptsonCalculation, IDescriptsonCalculation<TTarget, TResult>
    {
        public ReadOnlyCollection<IDescriptsonCalculation<TTarget, TResult>> Parameters { get; }

        protected DescriptsonCalculation(IEnumerable<IDescriptsonCalculation<TTarget, TResult>> parameters)
        {
            Parameters = parameters.ToList().AsReadOnly();
        }

        protected DescriptsonCalculation(params IDescriptsonCalculation<TTarget, TTarget>[] parameters)
            : this((IEnumerable<IDescriptsonCalculation<TTarget, TResult>>)parameters)
        { }

        /*public static DescriptsonCalculation<TTarget, TResult> CreateFrom(IEnumerable<JToken> jTokens)
        { }*/

        public static IEnumerable<IDescriptsonCalculation<TTarget, TResult>> GetSubCalculations(IEnumerable<JToken> jTokens)
        {
            foreach (var jToken in jTokens)
                if (jToken.Type == JTokenType.Array)
                    yield return Make((JArray)jToken);
                else
                    yield return MakeLiteral(jToken);
        }

        public static DescriptsonCalculation<TTarget, TResult> Make(JArray jArray)
        {
            return (DescriptsonCalculation<TTarget, TResult>)Make(typeof(TTarget), typeof(TResult), jArray);
        }

        public static DescriptsonLiteral<TTarget, TResult> MakeLiteral(JToken jToken)
        {
            return new DescriptsonLiteral<TTarget, TResult>(jToken.Value<TResult>());
        }

        public abstract TResult Calculate(TTarget target);
    }
}