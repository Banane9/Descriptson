using System;
using System.Collections.Generic;

namespace Descriptson
{
    public class DescriptsonCalculatedProperty<TObject> where TObject : DescriptsonObject<TObject>, new()
    {
        public object Calculation { get; }
        public Action<TObject, object> SetupProperty { get; }

        public DescriptsonCalculatedProperty(Action<TObject, object> setupProperty, object calculation)
        {
            SetupProperty = setupProperty;
            Calculation = calculation;
        }

        public void Setup(TObject obj)
        {
            var instancedCalculation = Activator.CreateInstance(
                typeof(DescriptsonCalculated<,>)
                    .MakeGenericType(Calculation.GetType().GetGenericArguments()),
                obj);

            SetupProperty(obj, instancedCalculation);
        }
    }
}