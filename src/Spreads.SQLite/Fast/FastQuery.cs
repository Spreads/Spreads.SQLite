// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Spreads.Buffers;
using Spreads.SQLite.Interop;
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

    public readonly struct QueryBinder
    {
        private readonly Sqlite3StmtHandle _pStmt;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal QueryBinder(Sqlite3StmtHandle pStmt)
        {
            _pStmt = pStmt;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BindBlob(int i, IntPtr zData, int nData)
        {
            return sqlite3_bind_blob(_pStmt, i, zData, nData, SQLITE_STATIC);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BindBlob(int i, DirectBuffer buffer)
        {
            return sqlite3_bind_blob(_pStmt, i, buffer.IntPtr, buffer.Length, SQLITE_STATIC);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BindDouble(int i, double value)
        {
            return sqlite3_bind_double(_pStmt, i, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BindInt64(int i, long value)
        {
            return sqlite3_bind_int64(_pStmt, i, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BindNull(int i)
        {
            return sqlite3_bind_null(_pStmt, i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ClearBindings()
        {
            return sqlite3_clear_bindings(_pStmt);
        }

        // TODO Text & other types
    }

    public readonly struct QueryReader
    {
        private readonly Sqlite3StmtHandle _pStmt;
        private readonly Sqlite3Handle _dbHandle;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal QueryReader(Sqlite3StmtHandle pStmt, Sqlite3Handle dbHandle)
        {
            _pStmt = pStmt;
            _dbHandle = dbHandle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IntPtr ColumnBlob(int i)
        {
            return sqlite3_column_blob(_pStmt, i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ColumnDouble(int i)
        {
            return sqlite3_column_double(_pStmt, i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ColumnInt64(int i)
        {
            return sqlite3_column_int64(_pStmt, i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ColumnBytes(int i)
        {
            return sqlite3_column_bytes(_pStmt, i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ColumnCount()
        {
            return sqlite3_column_count(_pStmt);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ColumnType(int i)
        {
            return sqlite3_column_type(_pStmt, i);
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

        public FastQuery(string query, SqliteConnection connection, ConnectionPool pool = null)
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
                while (IsBusy(rc = NativeMethods.sqlite3_prepare_v3(_dbHandle, tail, out stmt, out nextTail)))
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
        public TResult Step<TState, TResult>(Func<bool, QueryReader, TState, TResult> readerFunc, TState state)
        {
            int rc;
            var timer = Stopwatch.StartNew();
            while (IsBusy(rc = sqlite3_step(_pStmt)))
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
        public void Reset()
        {
            sqlite3_reset(_pStmt);
        }

        public bool IsSqlite3StmtReadonly
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => sqlite3_stmt_readonly(_pStmt) != 0;
        }
    }
}
