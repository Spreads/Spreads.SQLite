using System;
using System.Runtime.CompilerServices;
using Spreads.Buffers;
using SQLitePCL;

namespace Spreads.SQLite
{
    public readonly struct QueryBinder
    {
        internal readonly sqlite3_stmt Statement;
        internal readonly IntPtr StatementHandle;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal QueryBinder(sqlite3_stmt statement, IntPtr statementHandle)
        {
            Statement = statement;
            StatementHandle = statementHandle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int BindBlob(int parameterIndex, ReadOnlySpan<byte> blob)
        {
            if (SpreadsSQLite.IsInitializedWithSpreads)
            {
                fixed (byte* blobP = &blob.GetPinnableReference())
                    return NativeMethods.sqlite3_bind_blob(StatementHandle, parameterIndex, (IntPtr)blobP, blob.Length, new IntPtr(-1));
            }

            return raw.sqlite3_bind_blob(Statement, parameterIndex, blob);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int BindText(int parameterIndex, ReadOnlySpan<byte> val)
        {
            if (SpreadsSQLite.IsInitializedWithSpreads)
            {
                fixed (byte* valP = &val.GetPinnableReference())
                    return NativeMethods.sqlite3_bind_text(StatementHandle, parameterIndex, (IntPtr)valP, val.Length, new IntPtr(-1));
            }

            return raw.sqlite3_bind_text(Statement, parameterIndex, val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BindBlob(int parameterIndex, DirectBuffer buffer) =>
            SpreadsSQLite.IsInitializedWithSpreads
                ? NativeMethods.sqlite3_bind_blob(StatementHandle, parameterIndex, buffer.DataIntPtr, buffer.Length, new IntPtr(-1))
                : raw.sqlite3_bind_blob(Statement, parameterIndex, buffer.Span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BindDouble(int parameterIndex, double value) =>
            SpreadsSQLite.IsInitializedWithSpreads
                ? NativeMethods.sqlite3_bind_double(StatementHandle, parameterIndex, value)
                : raw.sqlite3_bind_double(Statement, parameterIndex, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BindInt64(int parameterIndex, long value) =>
            SpreadsSQLite.IsInitializedWithSpreads
                ? NativeMethods.sqlite3_bind_int64(StatementHandle, parameterIndex, value)
                : raw.sqlite3_bind_int64(Statement, parameterIndex, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BindNull(int parameterIndex) =>
            SpreadsSQLite.IsInitializedWithSpreads
                ? NativeMethods.sqlite3_bind_null(StatementHandle, parameterIndex)
                : raw.sqlite3_bind_null(Statement, parameterIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ClearBindings() =>
            SpreadsSQLite.IsInitializedWithSpreads
                ? NativeMethods.sqlite3_clear_bindings(StatementHandle)
                : raw.sqlite3_clear_bindings(Statement);

        // TODO Text & other types
    }
}