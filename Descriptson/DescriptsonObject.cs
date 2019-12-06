using System;
using System.Collections.Generic;

namespace Descriptson
{
    public abstract class DescriptsonObject<TObject> where TObject : DescriptsonObject<TObject>, new()
    {
        public DescriptsonObjectBlueprint<TObject> Blueprint { get; internal set; }

        internal void SetupCalculatedProperties()
        {
            Blueprint.SetupCalculatedProperties((TObject)this);
        }

        internal bool Validate()
        {
            return Blueprint.Validate((TObject)this);
        }
    }
}