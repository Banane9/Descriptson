using System;
using System.Collections.Generic;
using Descriptson.RepresentationTree.Calculation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Descriptson.JsonConverters
{
    public sealed class DescriptsonCalculationConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsArray && objectType.GetElementType().GetGenericTypeDefinition() == typeof(DescriptsonCalculatedProperty<>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var targetType = objectType.GetElementType().GetGenericArguments()[0];

            return typeof(DescriptsonCalculationConverter).GetMethod(nameof(getCalculatedProperties)).MakeGenericMethod(targetType).Invoke(null, new[] { reader });
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        private static DescriptsonCalculatedProperty<TObject>[] getCalculatedProperties<TObject>(JsonReader reader) where TObject : DescriptsonObject<TObject>, new()
        {
            var calculatedProperties = new List<DescriptsonCalculatedProperty<TObject>>();

            while (reader.Read() && (reader.TokenType != JsonToken.EndObject))
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    throw new InvalidOperationException("Unknown JObject conversion state");

                var jProperty = JProperty.Load(reader);

                var setupProperty = DescriptsonPropertyManager<TObject>.ParseWritePath(jProperty.Name);
                var calculation = DescriptsonCalculation.Make(typeof(TObject), setupProperty.Body.Type.GetGenericArguments()[1], (JArray)jProperty.Value);

                calculatedProperties.Add(new DescriptsonCalculatedProperty<TObject>(setupProperty.Compile(), calculation));
            }

            return calculatedProperties.ToArray();
        }
    }
}