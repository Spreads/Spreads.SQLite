// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;

namespace Microsoft.Data.Sqlite.Utilities
{
    internal static class TypeExtensions
    {
        public static Type UnwrapEnumType(this Type type)
            => type.GetTypeInfo().IsEnum ? Enum.GetUnderlyingType(type) : type;

        public static Type UnwrapNullableType(this Type type)
            => Nullable.GetUnderlyingType(type) ?? type;
    }
}
