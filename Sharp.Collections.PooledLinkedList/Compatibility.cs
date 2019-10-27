#if !NULLABLE
using System;
using System.Diagnostics;

namespace System.Diagnostics.CodeAnalysis
{
    // To silence build errors for target frameworks that do not support the
    // System.Diagnostics.CodeAnalysis nullability attributes.

    [Conditional("NEVER")]
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    internal sealed class MaybeNullWhenAttribute : Attribute
    {
        public MaybeNullWhenAttribute(bool returnValue)
        {
            ReturnValue = returnValue;
        }

        public bool ReturnValue { get; }
    }
}
#endif
