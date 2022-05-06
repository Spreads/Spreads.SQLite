// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace Spreads.SQLite
{
    // Persistent query for frequent reuse. Owns a connection. Single statement.

    // https://www.sqlite.org/c3ref/stmt.html
    // The life-cycle of a prepared statement object usually goes like this:
    // 1. Create the prepared statement object using sqlite3_prepare_v2().
    // 2. Bind values to parameters using the sqlite3_bind_* () interfaces.
    // 3. Run the SQL by calling sqlite3_step() one or more times.
    // 4. Reset the prepared statement using sqlite3_reset() then go back to step 2. Do this zero or more times.
    // 5. Destroy the object using sqlite3_finalize().

    public class FastQuery : IDisposable
    {
        private readonly sqlite3 _db;
        private readonly string _query;
        private readonly sqlite3_stmt _statement;
        private readonly IntPtr _statementHandle;
        private readonly QueryReader _reader;
        private QueryBinder _binder => _reader.Binder;

        public FastQuery(string query, SqliteConnection connection)
        {
            if (!SpreadsSQLite.IsInitialized)
                throw new InvalidOperationException("SQLite native library must be properly set up");
            _db = connection.Handle ?? throw new InvalidOperationException("Connection must be open");
            _query = query;
            var prepStatements = PrepareAndEnumerateStatements().ToArray();
            if (prepStatements.Length != 1)
            {
                foreach (var t in prepStatements)
                {
                    t.Dispose();
                }

                throw new InvalidOperationException("FastQuery supports only a single statement");
            }

            _statement = prepStatements[0];
            _statementHandle = _statement.DangerousGetHandle();
            _reader = new QueryReader(new QueryBinder(_statement, _statement.DangerousGetHandle()), _db, _db.DangerousGetHandle());

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsBusy(int rc)
            => rc == raw.SQLITE_LOCKED
               || rc == raw.SQLITE_BUSY
               || rc == raw.SQLITE_BUSY_SNAPSHOT
               || rc == raw.SQLITE_LOCKED_SHAREDCACHE;

        private IEnumerable<sqlite3_stmt> PrepareAndEnumerateStatements()
        {
            Stopwatch timer = Stopwatch.StartNew();
            int rc;
            sqlite3_stmt stmt;
            var tail = _query;
            do
            {
                string nextTail;
                const int SQLITE_PREPARE_PERSISTENT = 1;
                while (IsBusy(rc = raw.sqlite3_prepare_v3(_db, tail, SQLITE_PREPARE_PERSISTENT, out stmt, out nextTail)))
                {
                    if (timer.ElapsedMilliseconds >= 30 * 1000)
                    {
                        break;
                    }

                    Thread.Sleep(150);
                }

                tail = nextTail;

                SqliteException.ThrowExceptionForRC(rc, _db);

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
            } while (!string.IsNullOrEmpty(tail));
        }

        public void Dispose()
        {
            _statement.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Bind<T>(Action<QueryBinder, T> bindAction, T state)
        {
            bindAction.Invoke(_reader.Binder, state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Bind<TAction, T>(T state)
            where TAction : struct, IQueryBinderAction<T>
        {
            default(TAction).Invoke(_reader.Binder, state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int native_step()
        {
            return SpreadsSQLite.IsInitializedWithSpreads
                ? NativeMethods.sqlite3_step(_statementHandle)
                : raw.sqlite3_step(_statement);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TResult Step<TState, TResult>(Func<bool, QueryReader, TState, TResult> readerFunc, TState state)
        {
            int rc;
            var timer = Stopwatch.StartNew();
            while (IsBusy(rc = native_step()))
            {
                if (timer.ElapsedMilliseconds >= 30 * 1000)
                {
                    break;
                }

                // https://www.sqlite.org/c3ref/step.html
                // If the statement is a COMMIT or occurs outside of an explicit transaction, then you can retry the statement.
                Thread.Sleep(50);
            }

            SqliteException.ThrowExceptionForRC(rc, _db);
            var hasRow = rc == raw.SQLITE_ROW;
            return readerFunc.Invoke(hasRow, _reader, state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Step<TState, TResult>(Func<bool, QueryReader, TState, TResult> readerFunc, TState state, out TResult result)
        {
            var rc = native_step();
            result = default;
            bool hasRow = false;
            if (rc == raw.SQLITE_OK
                || (hasRow = rc == raw.SQLITE_ROW)
                || rc == raw.SQLITE_DONE)
            {
                result = readerFunc(hasRow, _reader, state);
            }

            return rc;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Step<TReader, TState, TResult>(TState state, out TResult result)
            where TReader : struct, IStepReader<TState, TResult>
        {
            var rc = native_step();
            result = default;
            bool hasRow = false;
            if (rc == raw.SQLITE_OK
                || (hasRow = rc == raw.SQLITE_ROW)
                || rc == raw.SQLITE_DONE)
            {
                default(TReader).Invoke(hasRow, _reader, state, out result);
            }

            return rc;
        }

        /// <summary>
        /// Contrary to the intuition of many, sqlite3_reset() does not reset the bindings on a prepared statement.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearAndReset()
        {
            if (SpreadsSQLite.IsInitializedWithSpreads)
            {
                NativeMethods.sqlite3_clear_bindings(_statementHandle);
                NativeMethods.sqlite3_reset(_statementHandle);
            }
            else
            {
                raw.sqlite3_clear_bindings(_statement);
                raw.sqlite3_reset(_statement);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            if (SpreadsSQLite.IsInitializedWithSpreads)
                NativeMethods.sqlite3_reset(_statementHandle);
            else
                raw.sqlite3_reset(_statement);
        }

        public bool IsSqlite3StmtReadonly
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => raw.sqlite3_stmt_readonly(_statement) != 0;
        }
    }
}