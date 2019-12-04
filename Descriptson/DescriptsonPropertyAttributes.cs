using System;
using System.Collections.Generic;

namespace Descriptson
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class DescriptsonGetAttribute : Attribute
    { }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class DescriptsonSetAttribute : Attribute
    { }
}