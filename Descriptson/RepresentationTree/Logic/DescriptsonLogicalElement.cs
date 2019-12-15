using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Descriptson.RepresentationTree.Test;
using Newtonsoft.Json;
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

        public static DescriptsonLogicalElement<TTarget> Make(JsonReader reader)
        {
            return (DescriptsonLogicalElement<TTarget>)Make(typeof(TTarget), reader);
        }

        public abstract bool Test(TTarget target);

        protected static IEnumerable<IDescriptsonTest<TTarget>> GetSubExpressions(JsonReader reader)
        {
            while (reader.Read() && (reader.TokenType != JsonToken.EndObject))
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    throw new InvalidOperationException("Unknown JObject conversion state");

                string propertyName = (string)reader.Value;

                if (IsKnownOperator(propertyName))
                {
                    yield return Make(reader);
                }
                else
                {
                    reader.Read();

                    switch (reader.TokenType)
                    {
                        case JsonToken.StartArray:
                            if (!DescriptsonPropertyTest.EndsWithTestIndicator(propertyName))
                                throw new InvalidOperationException("Logical expressions can't contain assignments!");

                            //calculated property test
                            yield return DescriptsonCalculatedPropertyTest<TTarget>.CreateFrom(propertyName, reader);
                            break;

                        case JsonToken.StartObject:
                            if (DescriptsonPropertyTest.EndsWithTestIndicator(propertyName))
                                yield return DescriptsonPropertyTest<TTarget>.CreateFrom(propertyName, reader);

                            var getValue = DescriptsonPropertyManager<TTarget>.ParseAccessPath(propertyName);

                            yield return (IDescriptsonTest<TTarget>)Activator.CreateInstance(
                                typeof(DescriptsonSubPropertySelect<,>)
                                    .MakeGenericType(typeof(TTarget), getValue.Body.Type),
                                getValue,
                                reader);
                            break;

                        default:
                            throw new InvalidOperationException();
                    }
                }
            }
        }
    }
}