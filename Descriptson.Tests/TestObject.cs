using System;
using System.Collections.Generic;

namespace Descriptson.Tests
{
    public class TestObject : DescriptsonObject<TestObject>
    {
        public DescriptsonCalculated<TestObject, int> CalculatedInt { get; private set; }

        [DescriptsonGet]
        public int RegularInt { get; private set; } = 1;

        public int TestedInt { get; private set; }
    }
}