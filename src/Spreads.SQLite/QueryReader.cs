using System;
using System.Runtime.CompilerServices;
using SQLitePCL;

namespace Spreads.SQLite
{
    public readonly struct QueryReader
    {
        internal readonly QueryBinder Binder;

        private readonly sqlite3 _db;
        private readonly IntPtr _dbHandle;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal QueryReader(QueryBinder binder, sqlite3 db, IntPtr dbHandle)
        {
            Binder = binder;
            _db = db;
            _dbHandle = dbHandle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ReadOnlySpan<byte> ColumnBlob(int i)
        {
            if (SpreadsSQLite.IsInitializedWithSpreads)
            {
                IntPtr num = NativeMethods.sqlite3_column_blob(Binder.StatementHandle, i);
                if (num == IntPtr.Zero)
                    return (ReadOnlySpan<byte>)(byte[]?)null;
                int length = NativeMethods.sqlite3_column_bytes(Binder.StatementHandle, i);
                return new ReadOnlySpan<byte>(num.ToPointer(), length);
            }

            return raw.sqlite3_column_blob(Binder.Statement, i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public utf8z ColumnText(int column) =>
            SpreadsSQLite.IsInitializedWithSpreads
                ? utf8z.FromIntPtr(NativeMethods.sqlite3_column_text(Binder.StatementHandle, column))
                : raw.sqlite3_column_text(Binder.Statement, column);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ColumnDouble(int column) =>
            SpreadsSQLite.IsInitializedWithSpreads
                ? NativeMethods.sqlite3_column_double(Binder.StatementHandle, column)
                : raw.sqlite3_column_double(Binder.Statement, column);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ColumnInt64(int column) =>
            SpreadsSQLite.IsInitializedWithSpreads
                ? NativeMethods.sqlite3_column_int64(Binder.StatementHandle, column)
                : raw.sqlite3_column_int64(Binder.Statement, column);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ColumnBytes(int column) =>
            SpreadsSQLite.IsInitializedWithSpreads
                ? NativeMethods.sqlite3_column_bytes(Binder.StatementHandle, column)
                : raw.sqlite3_column_bytes(Binder.Statement, column);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ColumnCount() =>
            SpreadsSQLite.IsInitializedWithSpreads
                ? NativeMethods.sqlite3_column_count(Binder.StatementHandle)
                : raw.sqlite3_column_count(Binder.Statement);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ColumnType(int column) =>
            SpreadsSQLite.IsInitializedWithSpreads
                ? NativeMethods.sqlite3_column_type(Binder.StatementHandle, column)
                : raw.sqlite3_column_type(Binder.Statement, column);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Changes() =>
            SpreadsSQLite.IsInitializedWithSpreads
                ? NativeMethods.sqlite3_changes(_dbHandle)
                : raw.sqlite3_changes(_db);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long LastRowId() =>
            SpreadsSQLite.IsInitializedWithSpreads
                ? NativeMethods.sqlite3_last_insert_rowid(_dbHandle)
                : raw.sqlite3_last_insert_rowid(_db);

        // TODO Text & other types
    }
}