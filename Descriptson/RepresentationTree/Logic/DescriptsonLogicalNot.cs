using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Descriptson.RepresentationTree.Logic
{
    public class DescriptsonLogicalNot<TTarget> : DescriptsonLogicalElement<TTarget>
    {
        public DescriptsonLogicalNot(IDescriptsonTest<TTarget> subExpression)
            : base(subExpression)
        { }

        public static DescriptsonLogicalElement<TTarget> CreateFrom(JsonReader reader)
        {
            //if (jObject.Children().Count() > 1)
            //    throw new InvalidOperationException("A not-operator can only have one sub-expression!");

            return new DescriptsonLogicalNot<TTarget>(GetSubExpressions(reader).Single());
        }

        public override bool Test(TTarget target)
        {
            return SubExpressions[0].Test(target);
        }
    }
}