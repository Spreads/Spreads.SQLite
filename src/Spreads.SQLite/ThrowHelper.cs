using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Spreads.SQLite
{
    internal static class ThrowHelper
    {
        [DebuggerStepThrough]
        [DoesNotReturn]
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException(string message) => throw new InvalidOperationException(message);
    }
}

#if !NETCOREAPP && !NETSTANDARD2_1

namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    ///     Specifies that <see langword="null"/> is allowed as an input even if the
    ///     corresponding type disallows it.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property,
        Inherited = false
    )]
    internal sealed class AllowNullAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AllowNullAttribute"/> class.
        /// </summary>
        public AllowNullAttribute()
        {
        }
    }

    /// <summary>
    ///     Specifies that <see langword="null"/> is disallowed as an input even if the
    ///     corresponding type allows it.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property,
        Inherited = false
    )]
    internal sealed class DisallowNullAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DisallowNullAttribute"/> class.
        /// </summary>
        public DisallowNullAttribute()
        {
        }
    }

    /// <summary>
    ///     Specifies that a method that will never return under any circumstance.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    internal sealed class DoesNotReturnAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DoesNotReturnAttribute"/> class.
        /// </summary>
        public DoesNotReturnAttribute()
        {
        }
    }
}

#endif