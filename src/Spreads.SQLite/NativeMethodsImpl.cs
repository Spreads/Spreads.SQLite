// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Spreads.SQLite
{
    internal static class NativeMethods
    {
        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_clear_bindings(IntPtr pStmt);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_bind_blob(IntPtr pStmt, int i, IntPtr zData, int nData, IntPtr xDel);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_bind_zeroblob(IntPtr stmt, int index, int size);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_blob_open(IntPtr db, byte[] sdb, byte[] table, byte[] col, long rowid, int flags, out IntPtr blob);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_blob_write(IntPtr blob, IntPtr b, int n, int offset);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_blob_read(IntPtr blob, IntPtr b, int n, int offset);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_blob_bytes(IntPtr blob);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_blob_close(IntPtr blob);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_bind_double(IntPtr pStmt, int i, double rValue);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_bind_int64(IntPtr pStmt, int i, long iValue);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_bind_null(IntPtr pStmt, int i);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_bind_parameter_count(IntPtr stmt);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_bind_parameter_index(IntPtr pStmt, IntPtr zName);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr sqlite3_bind_parameter_name(IntPtr stmt, int i);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_bind_text(IntPtr pStmt, int i, IntPtr zData, int n, IntPtr xDel);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_busy_timeout(IntPtr db, int ms);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_changes(IntPtr db);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern long sqlite3_last_insert_rowid(IntPtr db);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_close(IntPtr db);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_close_v2(IntPtr db);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr sqlite3_column_blob(IntPtr pStmt, int iCol);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_column_bytes(IntPtr pStmt, int iCol);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_column_count(IntPtr stmt);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr sqlite3_column_decltype(IntPtr stmt, int iCol);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern double sqlite3_column_double(IntPtr stmt, int iCol);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern long sqlite3_column_int64(IntPtr stmt, int iCol);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr sqlite3_column_name(IntPtr stmt, int iCol);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr sqlite3_column_text(IntPtr stmt, int iCol);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_column_type(IntPtr stmt, int iCol);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr sqlite3_db_filename(IntPtr db, IntPtr zDbName);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_enable_load_extension(IntPtr db, int onoff);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr sqlite3_errmsg(IntPtr db);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr sqlite3_errstr(int rc);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_finalize(IntPtr pStmt);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr sqlite3_libversion();

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_open_v2(IntPtr filename, out IntPtr ppDb, int flags, IntPtr vfs);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_extended_result_codes(IntPtr db, int oneoff);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_prepare_v2(
            IntPtr db,
            IntPtr zSql,
            int nByte,
            out IntPtr ppStmt,
            out IntPtr pzTail);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_prepare_v3(
            IntPtr db,
            IntPtr zSql,
            int nByte,
            uint prepFlags,
            out IntPtr ppStmt,
            out IntPtr pzTail);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_reset(IntPtr stmt);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_step(IntPtr stmt);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SpreadsSQLite.NativeLibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sqlite3_stmt_readonly(IntPtr pStmt);
    }
}