// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Spreads.SQLite.Interop
{
    public class Sqlite3StmtHandle : SafeHandle
    {
        protected Sqlite3StmtHandle()
            : base(IntPtr.Zero, ownsHandle: true)
        {
        }

        internal IntPtr Handle
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => handle;
        }

        public override bool IsInvalid
            => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            var rc = NativeMethods.sqlite3_finalize(handle);
            handle = IntPtr.Zero;

            return rc == Constants.SQLITE_OK;
        }
    }
}
