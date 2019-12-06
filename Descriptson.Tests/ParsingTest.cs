using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Descriptson.Tests
{
    [TestClass]
    public class ParsingTest
    {
        private readonly JsonSerializer jsonSerializer = new JsonSerializer();

        private readonly string test1 =
"{" +
"\"TestValue\": \"Test\"" +
"}";

        private readonly string test2 =
"{" +
"\"Validate\": {" +
"\"RegularInt=\": 1" +
"}}";

        [TestMethod]
        public void JustTestValueDeserializes()
        {
            var result = jsonSerializer.Deserialize<TestObjectBlueprint>(new JsonTextReader(new StringReader(test1)));
            Assert.AreEqual("Test", result.TestValue);
        }

        [TestMethod]
        public void SinglePropertyTestDeserializes()
        {
            var result = jsonSerializer.Deserialize<TestObjectBlueprint>(new JsonTextReader(new StringReader(test2)));
            Assert.IsTrue(result.Validate(new TestObject()));
        }
    }
}