using System;
using System.Collections.Generic;

namespace Descriptson.RepresentationTree
{
    public interface IDescriptsonTest<in TTarget>
    {
        bool Test(TTarget target);
    }
}