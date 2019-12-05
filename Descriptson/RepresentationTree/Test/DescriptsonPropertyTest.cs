using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace Descriptson.RepresentationTree.Test
{
    public abstract class DescriptsonPropertyTest
    {
        private static readonly char[] comparisonTypeIndicators = Enum.GetValues(typeof(DescriptsonComparisonType)).Cast<char>().ToArray();

        public static bool EndsWithTestIndicator(string path)
        {
            return comparisonTypeIndicators.Any(path[path.Length - 1].Equals);
        }

        public static object Make(Type targetType, JProperty jProperty)
        {
            Type valueType = null;
            switch (jProperty.Value.Type)
            {
                case JTokenType.Array:
                    return typeof(DescriptsonCalculatedPropertyTest<>)
                        .MakeGenericType(targetType)
                        .GetMethod("CreateFrom")
                        .Invoke(null, new[] { jProperty });

                case JTokenType.Boolean:
                    valueType = typeof(bool);
                    break;

                case JTokenType.Float:
                    valueType = typeof(double);
                    break;

                case JTokenType.Integer:
                    valueType = typeof(long);
                    break;

                case JTokenType.String:
                    valueType = typeof(string);
                    break;

                default:
                    throw new InvalidOperationException("You weren't supposed to do this!");
            }

            return typeof(DescriptsonPropertyTest<>)
                .MakeGenericType(targetType, valueType)
                .GetMethod("CreateFrom")
                .Invoke(null, new[] { jProperty });
        }
    }

    public class DescriptsonPropertyTest<TTarget> : DescriptsonPropertyTest, IDescriptsonTest<TTarget>
    {
        private readonly Func<TTarget, object> getValue;
        public Expression AccessExpression { get; }
        public DescriptsonComparisonType ComparisonType { get; }
        public object Value { get; }
        public Type ValueType { get; }

        public DescriptsonPropertyTest(Expression<Func<TTarget, object>> getValue, DescriptsonComparisonType comparisonType, object value)
        {
            AccessExpression = getValue.Body;
            this.getValue = getValue.Compile();
            ComparisonType = comparisonType;
            ValueType = AccessExpression.Type;
            Value = value ?? throw new ArgumentNullException(nameof(value), "Literal value to be compared to must not be null!");
        }

        public DescriptsonPropertyTest<TTarget> CreateFrom(JProperty jProperty)
        {
            var path = jProperty.Name.Substring(0, jProperty.Name.Length - 1);
            var getValue = DescriptsonPropertyManager<TTarget>.ParseAccessPath(path);
            var comparisonType = (DescriptsonComparisonType)jProperty.Name[jProperty.Name.Length - 1];
            var value = typeof(JToken).GetMethod("Value").MakeGenericMethod(getValue.Body.Type).Invoke(jProperty.Value, null);

            return new DescriptsonPropertyTest<TTarget>(getValue, comparisonType, value);
        }

        public DescriptsonPropertyTest<TTarget> Make(JProperty jProperty)
        {
            return (DescriptsonPropertyTest<TTarget>)Make(typeof(TTarget), jProperty);
        }

        public bool Test(TTarget target)
        {
            var propValue = getValue(target);

            switch (ComparisonType)
            {
                case DescriptsonComparisonType.AtLeast:
                    return Value.Equals(propValue)
                        || (propValue is IComparable compareValue1 && compareValue1.CompareTo(Value) > 0);

                case DescriptsonComparisonType.Equal:
                    return Value.Equals(getValue(target));

                case DescriptsonComparisonType.UpTo:
                    return Value.Equals(propValue)
                        || (propValue is IComparable compareValue2 && compareValue2.CompareTo(Value) < 0);
            }

            return false;
        }
    }
}