using System;
using System.Collections.Generic;

namespace Descriptson.RepresentationTree
{
    public interface IDescriptsonCalculation<in TTarget, out TResult>
    {
        TResult Calculate(TTarget target);
    }
}