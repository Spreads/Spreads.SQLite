using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using Spreads.Utils;

namespace Spreads.SQLite.Tests.Fast
{
    public struct TestBinderAction : IQueryBinderAction<long>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(QueryBinder b, long state)
        {
            if (0 != b.BindInt64(1, state))
            {
                Assert.Fail();
            }
        }
    }

    public struct TestReader : IStepReader<long, bool>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(bool hasRow, QueryReader reader, long state, out bool result)
        {
            if (hasRow)
            {
                var val = reader.ColumnInt64(0);
                if (state != val)
                    Assert.False(true);
            }

            result = hasRow;
        }
    }

    public class FastTest
    {
        [Test]
        public void FastQueryTest()
        {
            var connStr = "Data Source=data.db";
            var conn = new SqliteConnection(connStr);

            var query = "SELECT @X + @X";

            var fastQuery = new FastQuery(query, conn);

            var count = 1000;

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                fastQuery.Bind<TestBinderAction, long>(i);

                fastQuery.Step<TestReader, long, bool>(i, out var _);

                fastQuery.Reset();
            }

            sw.Stop();

            fastQuery.Dispose();
        }

        [Test]
        public void FastQueryTestDelegates()
        {
            var connectionString = "Data Source=data.db";
            var connection = new SqliteConnection(connectionString);

            var query = "SELECT @X + @X"; // single parameter

            using var fastQuery = new FastQuery(query, connection);

            var count = 1000;

            // Cache delegates to avoid allocations
            // Do not turn them into local function as Rider suggests
            
            Action<QueryBinder, long> bindMethod = (binder, value) =>
            {
                // Bind all parameters
                
                // @X, one based
                var parameterIndex = 1; 
                
                var rc = binder.BindInt64(parameterIndex, value);
                if (rc != 0)
                    SqliteException.ThrowExceptionForRC(rc, connection.Handle);
                
                // same logic for other parameters
            };
            
            Func<bool, QueryReader, long, bool> readerMethod = (hasRow, reader, state) =>
            {
                // state is passed from Step method, that helps to avoid delegate allocation
                if (hasRow)
                {
                    var column = 0;
                    var val = reader.ColumnInt64(column);
                    
                    if (state != val)
                        Assert.False(true);
                }

                return hasRow;
            };

            for (int i = 0; i < count; i++)
            {
                fastQuery.Bind(bindMethod, i);
                var rc = fastQuery.Step(readerMethod, i, out var _);
                if(rc != 0)
                    SqliteException.ThrowExceptionForRC(rc, connection.Handle);
                fastQuery.Reset();
            }
            
            fastQuery.Dispose();
        }

        [Test, Explicit]
        public void FastBench()
        {
            var connStr = "Data Source=data.db";

            var rounds = 20;
            var count = 1_000_000;
            SpreadsSQLite.InitializeProvider();

            using var connection = new SqliteConnection(connStr);
            connection.Open();
            var query = "SELECT @X + @X";
            using var fastQuery = new FastQuery(query, connection);

            for (int _ = 0; _ < rounds; _++)
            {
                Benchmark.Run("Fast", () =>
                {
                    for (int i = 0; i < count; i++)
                    {
                        fastQuery.Bind<TestBinderAction, long>(i);
                        fastQuery.Step<TestReader, long, bool>(2 * i, out var _);
                        fastQuery.Reset();
                    }
                }, count);

                Benchmark.Run("Default", () =>
                {
                    var command = new SqliteCommand(query, connection);
                    command.Parameters.Add("@X", SqliteType.Integer);

                    for (long i = 0; i < count; i++)
                    {
                        command.Parameters["@X"].Value = (long)i;

                        using (var result = command.ExecuteReader())
                        {
                            result.Read();
                            var val = result.GetInt64(0);
                            if (2 * i != val)
                            {
                                Assert.False(true);
                            }
                        }
                    }
                }, count);

            }

            Benchmark.Dump(opsAggregate: Benchmark.OpsAggregate.MinTime);
        }

        [Test, Explicit]
        public void FastBenchDelegates()
        {
            var connStr = "Data Source=data.db";

            var rounds = 20;
            var count = 1_000_000;
            SpreadsSQLite.InitializeProvider();

            using var connection = new SqliteConnection(connStr);
            connection.Open();
            var query = "SELECT @X + @X";
            using var fastQuery = new FastQuery(query, connection);

            for (int _ = 0; _ < rounds; _++)
            {
                // cache delegates
                Action<QueryBinder, long> bindMethod = BindAction;
                Func<bool, QueryReader, long, bool> readerMethod = ReaderFunc;

                Benchmark.Run("FastQuery", () =>
                {
                    for (int i = 0; i < count; i++)
                    {
                        fastQuery.Bind(bindMethod, i);
                        fastQuery.Step(readerMethod, 2 * i, out var _);
                        fastQuery.Reset();
                    }
                }, count);

                Benchmark.Run("MSFT.Data.SQLite", () =>
                {
                    var command = new SqliteCommand(query, connection);
                    command.Parameters.Add("@X", SqliteType.Integer);

                    for (long i = 0; i < count; i++)
                    {
                        command.Parameters["@X"].Value = (long)i;

                        using (var result = command.ExecuteReader())
                        {
                            result.Read();
                            var val = result.GetInt64(0);
                            if (2 * i != val)
                            {
                                Assert.False(true);
                            }
                        }
                    }
                }, count);

            }

            Benchmark.Dump(opsAggregate: Benchmark.OpsAggregate.MinTime);
        }

        [Test, Explicit]
        public void FastBenchShortLiveConnection()
        {
            var connStr = "Data Source=data.db"; // ; Pooling = False

            var rounds = 10;
            var count = 100_000;
            SpreadsSQLite.InitializeProvider();

            for (int _ = 0; _ < rounds; _++)
            {
                Benchmark.Run("Fast", () =>
                {
                    for (int i = 0; i < count; i++)
                    {
                        using var connection = new SqliteConnection(connStr);
                        connection.Open();
                        using var fastQuery = new FastQuery("SELECT @X", connection);

                        fastQuery.Bind<TestBinderAction, long>(i);

                        fastQuery.Step<TestReader, long, bool>(i, out var _);

                        fastQuery.Reset();
                    }
                }, count);

                {

                    Benchmark.Run("Default", () =>
                    {
                        for (long i = 0; i < count; i++)
                        {
                            using var connection = new SqliteConnection(connStr);
                            connection.Open();
                            using var command = new SqliteCommand("SELECT @X;", connection);
                            command.Parameters.Add("@X", SqliteType.Integer);

                            command.Parameters["@X"].Value = (long)i;

                            using (var result = command.ExecuteReader())
                            {
                                result.Read();
                                var val = result.GetInt64(0);
                                if (i != val)
                                {
                                    Assert.False(true);
                                }
                            }
                        }
                    }, count);
                }

            }

            Benchmark.Dump(opsAggregate: Benchmark.OpsAggregate.MinTime);
        }

        private bool ReaderFunc(bool hasRow, QueryReader reader, long i1)
        {
            if (hasRow)
            {
                var val = reader.ColumnInt64(0);
                if (i1 != val)
                {
                    Assert.False(true);
                }
            }

            return hasRow;
        }

        private void BindAction(QueryBinder b, long val)
        {
            if (0 != b.BindInt64(1, val))
            {
                Assert.False(true);
            }
        }
    }
}