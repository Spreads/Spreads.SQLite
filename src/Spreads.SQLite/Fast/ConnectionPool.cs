// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Spreads.Collections.Concurrent;
using System;
using System.Data;
using Spreads.SQLite.Utilities;
using static Spreads.SQLite.Interop.NativeMethods.Sqlite3_spreads_sqlite3;

namespace Spreads.SQLite.Fast
{
    public class ConnectionPool : IDisposable
    {
        private ConnectionState _state;
        private readonly LockedObjectPool<SqliteConnection> _pool;

        public ConnectionPool(string connectionString)
        {
            ConnectionString = connectionString;
            _pool = new LockedObjectPool<SqliteConnection>(Environment.ProcessorCount * 2, OpenConnection);
            // construct first object for exception on construction if any
            _pool.Return(_pool.Rent());
            _state = ConnectionState.Open;
        }

        public string ConnectionString { get; }

        private SqliteConnection OpenConnection()
        {
            var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            sqlite3_extended_result_codes(connection.DbHandle, 1);
            InitConnection(connection);
            return connection;
        }

        public virtual void InitConnection(SqliteConnection connection)
        {
            connection.ExecuteNonQuery("PRAGMA main.page_size = 4096; ");
            connection.ExecuteNonQuery("PRAGMA main.cache_size = 25000;");
            connection.ExecuteNonQuery("PRAGMA synchronous = NORMAL;");
            connection.ExecuteNonQuery("PRAGMA journal_mode = WAL;");
        }

        public void Release(SqliteConnection handle)
        {
            lock (_pool)
            {
                if (_state == ConnectionState.Closed)
                {
                    ThrowHelper.ThrowInvalidOperationException("_state == ConnectionState.Closed");
                }

                var pooled = _pool.Return(handle);
                if (!pooled)
                {
                    handle.Dispose();
                }
            }
        }

        public SqliteConnection Rent()
        {
            lock (_pool)
            {
                if (_state == ConnectionState.Closed)
                {
                    ThrowHelper.ThrowInvalidOperationException("_state == ConnectionState.Closed");
                }
                return _pool.Rent();
            }
        }

        public void Dispose()
        {
            lock (_pool)
            {
                _pool.Dispose();
                _state = ConnectionState.Closed;
            }
        }
    }
}
