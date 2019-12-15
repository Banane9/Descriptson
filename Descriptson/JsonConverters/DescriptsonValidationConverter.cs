using System;
using Descriptson.RepresentationTree;
using Descriptson.RepresentationTree.Logic;
using Newtonsoft.Json;

namespace Descriptson.JsonConverters
{
    public sealed class DescriptsonValidationConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType.GetGenericTypeDefinition() == typeof(IDescriptsonTest<>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return Activator.CreateInstance(typeof(DescriptsonLogicalAnd<>).MakeGenericType(objectType.GetGenericArguments()), reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }
    }
}