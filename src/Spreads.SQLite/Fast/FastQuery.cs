// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Spreads.Buffers;
using Spreads.SQLite.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using static Spreads.SQLite.Interop.Constants;
using static Spreads.SQLite.Interop.NativeMethods.Sqlite3_spreads_sqlite3;

namespace Spreads.SQLite.Fast
{
    // Persistent query for frequent reuse. Owns a connection. Single statement.

    // https://www.sqlite.org/c3ref/stmt.html
    // The life-cycle of a prepared statement object usually goes like this:
    // 1. Create the prepared statement object using sqlite3_prepare_v2().
    // 2. Bind values to parameters using the sqlite3_bind_* () interfaces.
    // 3. Run the SQL by calling sqlite3_step() one or more times.
    // 4. Reset the prepared statement using sqlite3_reset() then go back to step 2. Do this zero or more times.
    // 5. Destroy the object using sqlite3_finalize().


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IQueryBinderAction<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="state"></param>
        [Pure]
        void Invoke(QueryBinder binder, T state);
    }


    // bool, QueryReader, TState, TResult

    public interface IStepReader<TState, TResult>
    {
        [Pure]
        void Invoke(bool hasRow, QueryReader reader, TState state, out TResult result);
    }

    public readonly struct QueryBinder
    {
        private readonly IntPtr _pStmtHandle;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal QueryBinder(Sqlite3StmtHandle pStmt)
        {
            _pStmtHandle = pStmt.Handle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BindBlob(int i, IntPtr zData, int nData)
        {
            return sqlite3_bind_blob(_pStmtHandle, i, zData, nData, SQLITE_STATIC);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BindBlob(int i, DirectBuffer buffer)
        {
            return sqlite3_bind_blob(_pStmtHandle, i, buffer.IntPtr, buffer.Length, SQLITE_STATIC);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BindDouble(int i, double value)
        {
            return sqlite3_bind_double(_pStmtHandle, i, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BindInt64(int i, long value)
        {
            return sqlite3_bind_int64(_pStmtHandle, i, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BindNull(int i)
        {
            return sqlite3_bind_null(_pStmtHandle, i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ClearBindings()
        {
            return sqlite3_clear_bindings(_pStmtHandle);
        }

        // TODO Text & other types
    }

    public readonly struct QueryReader
    {
        private readonly IntPtr _pStmtHandle;
        private readonly IntPtr _dbHandle;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal QueryReader(Sqlite3StmtHandle pStmt, Sqlite3Handle dbHandle)
        {
            _pStmtHandle = pStmt.Handle;
            _dbHandle = dbHandle.Handle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IntPtr ColumnBlob(int i)
        {
            return sqlite3_column_blob(_pStmtHandle, i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ColumnDouble(int i)
        {
            return sqlite3_column_double(_pStmtHandle, i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ColumnInt64(int i)
        {
            return sqlite3_column_int64(_pStmtHandle, i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ColumnBytes(int i)
        {
            return sqlite3_column_bytes(_pStmtHandle, i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ColumnCount()
        {
            return sqlite3_column_count(_pStmtHandle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ColumnType(int i)
        {
            return sqlite3_column_type(_pStmtHandle, i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Changes()
        {
            return sqlite3_changes(_dbHandle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long LastRowId()
        {
            return sqlite3_last_insert_rowid(_dbHandle);
        }

        // TODO Text & other types
    }

    public class FastQuery : IDisposable
    {
        private readonly Sqlite3Handle _dbHandle;
        private readonly ConnectionPool _connectionPool;
        private readonly string _query;
        private readonly SqliteConnection _connection;
        internal static int Counter;
        private Sqlite3StmtHandle _pStmt;

        public FastQuery(string query, SqliteConnection connection, ConnectionPool pool = null, bool persistent = false)
        {
            _dbHandle = connection.DbHandle;
            _connectionPool = pool;
            _query = query;
            _connection = connection;
            var prepStatements = PrepareAndEnumerateStatements().ToArray();
            if (prepStatements.Length != 1)
            {
                foreach (var t in prepStatements)
                {
                    t.Dispose();
                }

                ThrowHelper.ThrowInvalidOperationException("FastQuery supports only a single statement");
            }

            _pStmt = prepStatements[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsBusy(int rc)
            => rc == SQLITE_LOCKED
               || rc == SQLITE_BUSY
               || rc == SQLITE_BUSY_SNAPSHOT
               || rc == SQLITE_LOCKED_SHAREDCACHE;

        private IEnumerable<Sqlite3StmtHandle> PrepareAndEnumerateStatements()
        {
            Stopwatch timer = Stopwatch.StartNew();
            int rc;
            Sqlite3StmtHandle stmt;
            var tail = _query;
            do
            {
                string nextTail;
                while (IsBusy(rc = NativeMethods.sqlite3_prepare_v3(_dbHandle.Handle, tail, out stmt, out nextTail)))
                {
                    if (timer.ElapsedMilliseconds >= 30 * 1000)
                    {
                        break;
                    }

                    Thread.Sleep(150);
                }
                tail = nextTail;

                MarshalEx.ThrowExceptionForRC(rc, _dbHandle);

                // Statement was empty, white space, or a comment
                if (stmt.IsInvalid)
                {
                    if (!string.IsNullOrEmpty(tail))
                    {
                        continue;
                    }

                    break;
                }

                yield return stmt;
            }
            while (!string.IsNullOrEmpty(tail));
        }

        public void Dispose()
        {
            _pStmt.Dispose();
            _connectionPool.Release(_connection);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Bind<T>(Action<QueryBinder, T> bindAction, T state)
        {
            bindAction.Invoke(new QueryBinder(_pStmt), state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Bind<TAction, T>(T state)
            where TAction : struct, IQueryBinderAction<T>
        {
            default(TAction).Invoke(new QueryBinder(_pStmt), state);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TResult Step<TState, TResult>(Func<bool, QueryReader, TState, TResult> readerFunc, TState state)
        {
            int rc;
            var timer = Stopwatch.StartNew();
            while (IsBusy(rc = sqlite3_step(_pStmt.Handle)))
            {
                if (timer.ElapsedMilliseconds >= 30 * 1000)
                {
                    break;
                }

                // https://www.sqlite.org/c3ref/step.html
                // If the statement is a COMMIT or occurs outside of an explicit transaction, then you can retry the statement.
                Thread.Sleep(50);
            }

            MarshalEx.ThrowExceptionForRC(rc, _dbHandle);
            var hasRow = rc == SQLITE_ROW;
            return readerFunc.Invoke(hasRow, new QueryReader(_pStmt, _dbHandle), state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int RawStep<TState, TResult>(Func<bool, QueryReader, TState, TResult> readerFunc, TState state, out TResult result)
        {
            var rc = sqlite3_step(_pStmt.Handle);
            result = default;
            bool hasRow = false;
            if (rc == SQLITE_OK
                || (hasRow = rc == SQLITE_ROW)
                || rc == SQLITE_DONE)
            {
                result = readerFunc(hasRow, new QueryReader(_pStmt, _dbHandle), state);
            }
           
            return rc;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int RawStep<TReader, TState, TResult>(TState state, out TResult result) 
            where TReader : struct, IStepReader<TState, TResult>
        {
            var rc = sqlite3_step(_pStmt.Handle);
            result = default;
            bool hasRow = false;
            if (rc == SQLITE_OK
                || (hasRow = rc == SQLITE_ROW)
                || rc == SQLITE_DONE)
            {
                default(TReader).Invoke(hasRow, new QueryReader(_pStmt, _dbHandle), state, out result);
            }

            return rc;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            sqlite3_reset(_pStmt.Handle);
        }

        public bool IsSqlite3StmtReadonly
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => sqlite3_stmt_readonly(_pStmt.Handle) != 0;
        }
    }
}
