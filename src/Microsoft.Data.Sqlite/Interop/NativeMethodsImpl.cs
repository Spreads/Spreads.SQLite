﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

#if NET451
using Microsoft.Data.Sqlite.Utilities;
#endif

namespace Microsoft.Data.Sqlite.Interop
{
    partial class NativeMethods
    {
        private static ISqlite3 Load(string dllName)
        {
            switch (dllName)
            {
                case "spreads_sqlite3":
                    return new Sqlite3_spreads_sqlite3();

                default:
                    Debug.Fail("Unexpected dllName: " + dllName);
                    goto case "spreads_sqlite3";
            }
        }

        private class Sqlite3_spreads_sqlite3 : ISqlite3
        {
            private const string DllName = "spreads_sqlite3";

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int sqlite3_bind_blob(Sqlite3StmtHandle pStmt, int i, byte[] zData, int nData, IntPtr xDel);

            public int bind_blob(Sqlite3StmtHandle pStmt, int i, byte[] zData, int nData, IntPtr xDel)
                => sqlite3_bind_blob(pStmt, i, zData, nData, xDel);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int sqlite3_bind_double(Sqlite3StmtHandle pStmt, int i, double rValue);

            public int bind_double(Sqlite3StmtHandle pStmt, int i, double rValue)
                => sqlite3_bind_double(pStmt, i, rValue);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int sqlite3_bind_int64(Sqlite3StmtHandle pStmt, int i, long iValue);

            public int bind_int64(Sqlite3StmtHandle pStmt, int i, long iValue)
                => sqlite3_bind_int64(pStmt, i, iValue);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int sqlite3_bind_null(Sqlite3StmtHandle pStmt, int i);

            public int bind_null(Sqlite3StmtHandle pStmt, int i)
                => sqlite3_bind_null(pStmt, i);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int sqlite3_bind_parameter_count(Sqlite3StmtHandle stmt);

            public int bind_parameter_count(Sqlite3StmtHandle stmt)
                => sqlite3_bind_parameter_count(stmt);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int sqlite3_bind_parameter_index(Sqlite3StmtHandle pStmt, IntPtr zName);

            public int bind_parameter_index(Sqlite3StmtHandle pStmt, IntPtr zName)
                => sqlite3_bind_parameter_index(pStmt, zName);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern IntPtr sqlite3_bind_parameter_name(Sqlite3StmtHandle stmt, int i);

            public IntPtr bind_parameter_name(Sqlite3StmtHandle stmt, int i)
                => sqlite3_bind_parameter_name(stmt, i);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int sqlite3_bind_text(Sqlite3StmtHandle pStmt, int i, IntPtr zData, int n, IntPtr xDel);

            public int bind_text(Sqlite3StmtHandle pStmt, int i, IntPtr zData, int n, IntPtr xDel)
                => sqlite3_bind_text(pStmt, i, zData, n, xDel);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int sqlite3_busy_timeout(Sqlite3Handle db, int ms);

            public int busy_timeout(Sqlite3Handle db, int ms)
                => sqlite3_busy_timeout(db, ms);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int sqlite3_changes(Sqlite3Handle db);

            public int changes(Sqlite3Handle db)
                => sqlite3_changes(db);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int sqlite3_close(IntPtr db);

            public int close(IntPtr db)
                => sqlite3_close(db);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int sqlite3_close_v2(IntPtr db);

            public int close_v2(IntPtr db)
                => sqlite3_close_v2(db);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern IntPtr sqlite3_column_blob(Sqlite3StmtHandle pStmt, int iCol);

            public IntPtr column_blob(Sqlite3StmtHandle pStmt, int iCol)
                => sqlite3_column_blob(pStmt, iCol);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int sqlite3_column_bytes(Sqlite3StmtHandle pStmt, int iCol);

            public int column_bytes(Sqlite3StmtHandle pStmt, int iCol)
                => sqlite3_column_bytes(pStmt, iCol);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int sqlite3_column_count(Sqlite3StmtHandle stmt);

            public int column_count(Sqlite3StmtHandle stmt)
                => sqlite3_column_count(stmt);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern IntPtr sqlite3_column_decltype(Sqlite3StmtHandle stmt, int iCol);

            public IntPtr column_decltype(Sqlite3StmtHandle stmt, int iCol)
                => sqlite3_column_decltype(stmt, iCol);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern double sqlite3_column_double(Sqlite3StmtHandle stmt, int iCol);

            public double column_double(Sqlite3StmtHandle stmt, int iCol)
                => sqlite3_column_double(stmt, iCol);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern long sqlite3_column_int64(Sqlite3StmtHandle stmt, int iCol);

            public long column_int64(Sqlite3StmtHandle stmt, int iCol)
                => sqlite3_column_int64(stmt, iCol);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern IntPtr sqlite3_column_name(Sqlite3StmtHandle stmt, int iCol);

            public IntPtr column_name(Sqlite3StmtHandle stmt, int iCol)
                => sqlite3_column_name(stmt, iCol);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern IntPtr sqlite3_column_text(Sqlite3StmtHandle stmt, int iCol);

            public IntPtr column_text(Sqlite3StmtHandle stmt, int iCol)
                => sqlite3_column_text(stmt, iCol);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int sqlite3_column_type(Sqlite3StmtHandle stmt, int iCol);

            public int column_type(Sqlite3StmtHandle stmt, int iCol)
                => sqlite3_column_type(stmt, iCol);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern IntPtr sqlite3_db_filename(Sqlite3Handle db, IntPtr zDbName);

            public IntPtr db_filename(Sqlite3Handle db, IntPtr zDbName)
                => sqlite3_db_filename(db, zDbName);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int sqlite3_enable_load_extension(Sqlite3Handle db, int onoff);

            public int enable_load_extension(Sqlite3Handle db, int onoff)
                => sqlite3_enable_load_extension(db, onoff);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern IntPtr sqlite3_errmsg(Sqlite3Handle db);

            public IntPtr errmsg(Sqlite3Handle db)
                => sqlite3_errmsg(db);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern IntPtr sqlite3_errstr(int rc);

            public IntPtr errstr(int rc)
                => sqlite3_errstr(rc);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int sqlite3_finalize(IntPtr pStmt);

            public int finalize(IntPtr pStmt)
                => sqlite3_finalize(pStmt);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern IntPtr sqlite3_libversion();

            public IntPtr libversion()
                => sqlite3_libversion();

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int sqlite3_open_v2(IntPtr filename, out Sqlite3Handle ppDb, int flags, IntPtr vfs);

            public int open_v2(IntPtr filename, out Sqlite3Handle ppDb, int flags, IntPtr vfs)
                => sqlite3_open_v2(filename, out ppDb, flags, vfs);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int sqlite3_prepare_v2(
                Sqlite3Handle db,
                IntPtr zSql,
                int nByte,
                out Sqlite3StmtHandle ppStmt,
                out IntPtr pzTail);

            public int prepare_v2(Sqlite3Handle db, IntPtr zSql, int nByte, out Sqlite3StmtHandle ppStmt, out IntPtr pzTail)
                => sqlite3_prepare_v2(db, zSql, nByte, out ppStmt, out pzTail);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int sqlite3_reset(Sqlite3StmtHandle stmt);

            public int reset(Sqlite3StmtHandle stmt)
                => sqlite3_reset(stmt);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int sqlite3_step(Sqlite3StmtHandle stmt);

            public int step(Sqlite3StmtHandle stmt)
                => sqlite3_step(stmt);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            private static extern int sqlite3_stmt_readonly(Sqlite3StmtHandle pStmt);

            public int stmt_readonly(Sqlite3StmtHandle pStmt)
                => sqlite3_stmt_readonly(pStmt);
        }
        
    }
}
