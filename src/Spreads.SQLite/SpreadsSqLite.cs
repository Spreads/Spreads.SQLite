// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data.Common;
using System.Runtime.InteropServices;
using SQLitePCL;

namespace Spreads.SQLite
{
    // ReSharper disable once InconsistentNaming
    public static class SpreadsSQLite
    {
        private class NativeLibraryAdapter : IGetFunctionPointer
        {
            private readonly IntPtr _library;

            public NativeLibraryAdapter(string name)
                => _library = NativeLibrary.Load(name);

            public IntPtr GetFunctionPointer(string name)
                => NativeLibrary.TryGetExport(_library, name, out var address)
                    ? address
                    : IntPtr.Zero;
        }
        
        public const string NativeLibraryName = "spreads_sqlite3";

        internal static readonly bool IsInitialized = GetNativeLibraryName() != null;
        internal static readonly bool IsInitializedWithSpreads = GetNativeLibraryName() == NativeLibraryName;
        
        public static string? GetNativeLibraryName()
        {
            try
            {
                return raw.GetNativeLibraryName();
            }
            catch
            {
                try
                {
                    InitializeProvider();
                    return NativeLibraryName;
                }
                catch
                {
                    return null;
                }
            }
        }

        public static bool InitializeProvider()
        {
            SQLite3Provider_dynamic_cdecl.Setup(NativeLibraryName, new NativeLibraryAdapter(NativeLibraryName));
            raw.SetProvider(new SQLite3Provider_dynamic_cdecl());
            raw.FreezeProvider();
            return true;
        }

        public static int ExecuteNonQuery(this DbConnection connection, string commandText, int? timeout = null)
        {
            var command = connection.CreateCommand();
            command.CommandTimeout = timeout ?? connection.ConnectionTimeout;
            command.CommandText = commandText;

            return command.ExecuteNonQuery();
        }

        public static T ExecuteScalar<T>(this DbConnection connection, string commandText, int? timeout = null)
        {
            var command = connection.CreateCommand();
            command.CommandTimeout = timeout ?? connection.ConnectionTimeout;
            command.CommandText = commandText;
            return (T)command.ExecuteScalar();
        }
    }
}