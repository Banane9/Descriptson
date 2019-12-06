using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using Descriptson.RepresentationTree.Calculation;
using Newtonsoft.Json.Linq;

namespace Descriptson.RepresentationTree.Test
{
    public class DescriptsonCalculatedPropertyTest<TTarget> : DescriptsonPropertyTest, IDescriptsonTest<TTarget>
    {
        private readonly Func<TTarget, object> getValue;
        public Expression AccessExpression { get; }
        public IDescriptsonCalculation<TTarget, object> Calculation { get; }
        public DescriptsonComparisonType ComparisonType { get; }
        public Type ValueType { get; }

        public DescriptsonCalculatedPropertyTest(LambdaExpression getValue, DescriptsonComparisonType comparisonType, IDescriptsonCalculation<TTarget, object> calculation)
        {
            AccessExpression = getValue.Body;
            this.getValue = (Func<TTarget, object>)getValue.Compile();
            ComparisonType = comparisonType;
            Calculation = calculation;
        }

        public DescriptsonCalculatedPropertyTest<TTarget> CreateFrom(JProperty jProperty)
        {
            var path = jProperty.Name.Substring(0, jProperty.Name.Length - 1);
            var getValue = DescriptsonPropertyManager<TTarget>.ParseAccessPath(path);
            var comparisonType = (DescriptsonComparisonType)jProperty.Name[jProperty.Name.Length - 1];
            var calculation = (IDescriptsonCalculation<TTarget, object>)DescriptsonCalculation.Make(typeof(TTarget), getValue.Body.Type, (JArray)jProperty.Value);

            return new DescriptsonCalculatedPropertyTest<TTarget>(getValue, comparisonType, calculation);
        }

        public DescriptsonPropertyTest<TTarget> Make(JProperty jProperty)
        {
            return (DescriptsonPropertyTest<TTarget>)Make(typeof(TTarget), jProperty);
        }

        public bool Test(TTarget target)
        {
            var propValue = getValue(target);
            var calculatedValue = Calculation.Calculate(target);

            switch (ComparisonType)
            {
                case DescriptsonComparisonType.AtLeast:
                    return calculatedValue.Equals(propValue)
                        || (propValue is IComparable compareValue1 && compareValue1.CompareTo(calculatedValue) > 0);

                case DescriptsonComparisonType.Equal:
                    return calculatedValue.Equals(getValue(target));

                case DescriptsonComparisonType.UpTo:
                    return calculatedValue.Equals(propValue)
                        || (propValue is IComparable compareValue2 && compareValue2.CompareTo(calculatedValue) < 0);
            }

            return false;
        }
    }
}