using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Descriptson.RepresentationTree.Test;
using Newtonsoft.Json.Linq;

namespace Descriptson.RepresentationTree.Logic
{
    public abstract class DescriptsonLogicalElement
    {
        public static Dictionary<string, Type> LogicalOperators { get; } = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            { "and", typeof(DescriptsonLogicalAnd<>) },
            { "not", typeof(DescriptsonLogicalNot<>) },
            { "or", typeof(DescriptsonLogicalOr<>) },
            { "xor", typeof(DescriptsonLogicalXOr<>) },
        };

        public static bool IsKnownOperator(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Must contain a name!", nameof(name));

            return LogicalOperators.ContainsKey(name);
        }

        public static object Make(Type targetType, JProperty jProperty)
        {
            if (string.IsNullOrWhiteSpace(jProperty.Name))
                throw new ArgumentException("Must contain a name!", nameof(jProperty.Name));

            if (jProperty.Value.Type != JTokenType.Object)
                throw new InvalidOperationException("Operator needs an object!");

            if (!LogicalOperators.ContainsKey(jProperty.Name))
            {
                //throw new InvalidOperationException($"No operator found with name [{jProperty.Name}]!");
                if (!DescriptsonPropertyTest.EndsWithTestIndicator(jProperty.Name))
                    throw new InvalidOperationException("Assignments are not allowed in a test context!");

                return DescriptsonPropertyTest.Make(targetType, jProperty);
            }

            return LogicalOperators[jProperty.Name]
                .MakeGenericType(targetType)
                .GetMethod("CreateFrom")
                .Invoke(null, new[] { (JObject)jProperty.Value });
        }
    }

    public abstract class DescriptsonLogicalElement<TTarget> : DescriptsonLogicalElement, IDescriptsonTest<TTarget>
    {
        public ReadOnlyCollection<IDescriptsonTest<TTarget>> SubExpressions { get; }

        protected DescriptsonLogicalElement(IEnumerable<IDescriptsonTest<TTarget>> subExpressions)
        {
            SubExpressions = subExpressions.ToList().AsReadOnly();
        }

        protected DescriptsonLogicalElement(params IDescriptsonTest<TTarget>[] subExpressions)
            : this((IEnumerable<IDescriptsonTest<TTarget>>)subExpressions)
        { }

        /*public static DescriptsonLogicalElement<TTarget> CreateFrom(JObject jObject)
        { }*/

        public static DescriptsonLogicalElement<TTarget> Make(JProperty jProperty)
        {
            return (DescriptsonLogicalElement<TTarget>)Make(typeof(TTarget), jProperty);
        }

        public abstract bool Test(TTarget target);

        protected static IEnumerable<IDescriptsonTest<TTarget>> GetSubExpressions(JObject jObject)
        {
            foreach (var jProperty in jObject.Children().Cast<JProperty>())
            {
                if (IsKnownOperator(jProperty.Name))
                {
                    yield return Make(jProperty);
                }
                else
                {
                    if (jProperty.Value.Type == JTokenType.Array)
                        throw new InvalidOperationException("Logical expressions can't contain assignments!");

                    if (jProperty.Value.Type == JTokenType.Object)
                    {
                        var getValue = DescriptsonPropertyManager<TTarget>.ParseAccessPath(jProperty.Name);

                        yield return (IDescriptsonTest<TTarget>)Activator.CreateInstance(
                            typeof(DescriptsonSubPropertySelect<,>)
                                .MakeGenericType(typeof(TTarget),
                            getValue.Body.Type),
                            (JObject)jProperty.Value);
                        continue;
                    }

                    //property test
                    yield return DescriptsonPropertyTest<TTarget>.CreateFrom(jProperty);
                    // resolve property testing / indexing []
                    // if (propertyNames.Contains(jProperty.Name))
                }
            }
        }
    }
}