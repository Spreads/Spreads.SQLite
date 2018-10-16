// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Microsoft.Data.Sqlite;
using Spreads.SQLite.Utilities;
using System;
using System.Data;
using Microsoft.Data.Sqlite.Utilities;

namespace Spreads.SQLite.Fast
{
    public class ConnectionPool : IDisposable
    {
        private ConnectionState _state;
        private readonly string _connectionString;
        private LockedObjectPool<SqliteConnection> _pool = new LockedObjectPool<SqliteConnection>(Environment.ProcessorCount * 2);

        public ConnectionPool(string connectionString)
        {
            _connectionString = connectionString;
            _pool.Rent();
            // for exception on construction if any
            _pool.Return(OpenConnection());
            _state = ConnectionState.Open;
        }

        public string ConnectionString => _connectionString;

        private SqliteConnection OpenConnection()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();
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
                return _pool.Rent() ?? OpenConnection();
            }
        }

        public void Dispose()
        {
            lock (_pool)
            {
                SqliteConnection connection;
                while ((connection = _pool.Rent()) != null)
                {
                    connection.Dispose();
                }

                _state = ConnectionState.Closed;
            }
        }
    }
}
