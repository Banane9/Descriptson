using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Descriptson.RepresentationTree.Logic
{
    public abstract class DescriptsonLogicalElement<TTarget> : IDescriptsonTest<TTarget>
    {
        public ReadOnlyCollection<IDescriptsonTest<TTarget>> SubExpressions { get; }

        public abstract bool Test(TTarget target);
    }
}