using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
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

        public static object Make(string name, Type type, JObject jObject)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Must contain a name!", nameof(name));

            if (!LogicalOperators.ContainsKey(name))
                throw new InvalidOperationException($"No operator found with name [{name}]!");

            return LogicalOperators[name].MakeGenericType(type)
                .InvokeMember("CreateFrom", BindingFlags.Public | BindingFlags.Static, null, null, new[] { jObject });
        }
    }

    public abstract class DescriptsonLogicalElement<TTarget> : DescriptsonLogicalElement, IDescriptsonTest<TTarget>
    {
        private static readonly HashSet<string> propertyNames = new HashSet<string>(typeof(TTarget).GetProperties().Select(p => p.Name), StringComparer.OrdinalIgnoreCase);

        public ReadOnlyCollection<IDescriptsonTest<TTarget>> SubExpressions { get; }

        protected DescriptsonLogicalElement(IEnumerable<IDescriptsonTest<TTarget>> subExpressions)
        {
            SubExpressions = subExpressions.ToList().AsReadOnly();
        }

        protected DescriptsonLogicalElement(params IDescriptsonTest<TTarget>[] subExpressions)
            : this((IEnumerable<IDescriptsonTest<TTarget>>)subExpressions)
        { }

        public static DescriptsonLogicalElement<TTarget> Make(string name, JObject jObject)
        {
            return (DescriptsonLogicalElement<TTarget>)Make(name, typeof(TTarget), jObject);
        }

        public abstract DescriptsonLogicalElement<TTarget> CreateFrom(JObject jObject);

        public abstract bool Test(TTarget target);

        protected static IEnumerable<IDescriptsonTest<TTarget>> GetSubExpressions(JObject jObject)
        {
            foreach (var jProperty in jObject.Children().Cast<JProperty>())
            {
                if (IsKnownOperator(jProperty.Name))
                {
                    if (jProperty.Value.Type != JTokenType.Object)
                        throw new InvalidOperationException("Operator needs an array!");

                    yield return Make(jProperty.Name, (JObject)jProperty.Value);
                }
                else
                {
                    // resolve property testing / indexing []
                    // if (propertyNames.Contains(jProperty.Name))
                }
            }
        }
    }
}