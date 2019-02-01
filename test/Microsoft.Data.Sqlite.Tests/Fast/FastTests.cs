// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Spreads.SQLite.Fast;
using Xunit;
using Xunit.Abstractions;

namespace Spreads.SQLite.Tests.Fast
{
    public class FastTest
    {
        private readonly ITestOutputHelper output;

        public FastTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void FastSimpleTest()
        {
            var connStr = "Data Source=data.db";

            var pool = new ConnectionPool(connStr);

            var conn = pool.Rent();

            var fastQuery = new FastQuery("SELECT @X", conn, pool);

            var count = 10_000_000;

            // cache delegates
            Action<QueryBinder, long> bindMethod = BindAction;
            Func<bool, QueryReader, long, bool> readerMethod = ReaderFunc;

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                fastQuery.Bind(bindMethod, i);

                fastQuery.RawStep(readerMethod, i, out var _);
                //while ()
                //{ }

                fastQuery.Reset();
            }
            sw.Stop();

            output.WriteLine("FastQuery: " + sw.ElapsedMilliseconds);

            fastQuery.Dispose();

            pool.Dispose();

            using (var connection = new SqliteConnection(connStr))
            {
                connection.Open();

                var command = new SqliteCommand("SELECT @X;", connection);
                command.Parameters.Add("@X", SqliteType.Integer);

                var sw1 = Stopwatch.StartNew();
                for (long i = 0; i < count; i++)
                {
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
                sw1.Stop();
                output.WriteLine("ReaderQuery: " + sw1.ElapsedMilliseconds);
            }
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
