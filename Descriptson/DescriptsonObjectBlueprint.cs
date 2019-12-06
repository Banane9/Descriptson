using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Descriptson.RepresentationTree;
using Descriptson.RepresentationTree.Calculation;
using Descriptson.RepresentationTree.Logic;
using Descriptson.RepresentationTree.Test;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Descriptson
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class DescriptsonObjectBlueprint<TObject> where TObject : DescriptsonObject<TObject>, new()
    {
        private DescriptsonCalculatedProperty<TObject>[] calculations;

        [JsonExtensionData(WriteData = false)]
        private Dictionary<string, JToken> contentData = new Dictionary<string, JToken>(StringComparer.OrdinalIgnoreCase);

        public IDescriptsonTest<TObject> ValidationTest { get; private set; }

        public bool CreateObjectFromBlueprint(Action<TObject> configure, out TObject obj)
        {
            obj = new TObject { Blueprint = this };

            configure(obj);
            obj.SetupCalculatedProperties();

            return obj.Validate();
        }

        public void SetupCalculatedProperties(TObject obj)
        {
            foreach (var calculation in calculations)
                calculation.Setup(obj);
        }

        public bool Validate(TObject obj)
        {
            return ValidationTest.Test(obj);
        }

        [OnDeserialized]
        private void onDeserialized(StreamingContext streamingContext)
        {
            if (contentData.ContainsKey("validate"))
                ValidationTest = DescriptsonLogicalAnd<TObject>.CreateFrom((JObject)contentData["validate"]);
            else
                ValidationTest = new DescriptsonTrueTest<TObject>();

            if (!contentData.ContainsKey("calculate"))
                calculations = new DescriptsonCalculatedProperty<TObject>[0];
            else
                calculations = contentData["calculate"].Cast<JProperty>().Select(jProperty =>
                {
                    var setupProperty = DescriptsonPropertyManager<TObject>.ParseWritePath(jProperty.Name);
                    var calculation = DescriptsonCalculation.Make(typeof(TObject), setupProperty.Body.Type.GetGenericArguments()[1], (JArray)jProperty.Value);

                    return new DescriptsonCalculatedProperty<TObject>(setupProperty.Compile(), calculation);
                })
                .ToArray();
        }
    }
}