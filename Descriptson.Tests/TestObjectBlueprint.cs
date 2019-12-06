using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Descriptson.Tests
{
    public class TestObjectBlueprint : DescriptsonObjectBlueprint<TestObject>
    {
        [JsonProperty]
        public string TestValue { get; private set; }
    }
}