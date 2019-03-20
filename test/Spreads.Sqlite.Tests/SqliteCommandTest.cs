// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data;
using Spreads.SQLite.Properties;
using Spreads.SQLite.Tests.TestUtilities;
using Spreads.SQLite.Utilities;
using Xunit;

namespace Spreads.SQLite.Tests
{
    public class SqliteCommandTest
    {
        [Fact]
        public void Ctor_sets_values()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var command = new SqliteCommand("SELECT 1;", connection, transaction);

                    Assert.Equal("SELECT 1;", command.CommandText);
                    Assert.Same(connection, command.Connection);
                    Assert.Same(transaction, command.Transaction);
                }
            }
        }

        [Fact]
        public void CommandType_text_by_default()
        {
            Assert.Equal(CommandType.Text, new SqliteCommand().CommandType);
        }

        [Fact]
        public void CommandType_validates_value()
        {
            var ex = Assert.Throws<ArgumentException>(() => new SqliteCommand().CommandType = CommandType.StoredProcedure);

            Assert.Equal(Strings.InvalidCommandType(CommandType.StoredProcedure), ex.Message);
        }

        [Fact]
        public void Parameters_works()
        {
            var command = new SqliteCommand();

            var result = command.Parameters;

            Assert.NotNull(result);
            Assert.Same(result, command.Parameters);
        }

        [Fact]
        public void CreateParameter_works()
        {
            Assert.NotNull(new SqliteCommand().CreateParameter());
        }


        [Fact]
        public void ExecuteReader_throws_when_no_connection()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => new SqliteCommand().ExecuteReader());

            Assert.Equal(Strings.CallRequiresOpenConnection("ExecuteReader"), ex.Message);
        }

        [Fact]
        public void ExecuteReader_throws_when_connection_closed()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var ex = Assert.Throws<InvalidOperationException>(() => connection.CreateCommand().ExecuteReader());

                Assert.Equal(Strings.CallRequiresOpenConnection("ExecuteReader"), ex.Message);
            }
        }

        [Fact]
        public void ExecuteReader_throws_when_no_command_text()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                connection.Open();

                var ex = Assert.Throws<InvalidOperationException>(() => connection.CreateCommand().ExecuteReader());

                Assert.Equal(Strings.CallRequiresSetCommandText("ExecuteReader"), ex.Message);
            }
        }

        [Fact]
        public void ExecuteReader_throws_on_error()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "INVALID";
                connection.Open();

                var ex = Assert.Throws<SqliteException>(() => command.ExecuteReader());

                Assert.Equal(Constants.SQLITE_ERROR, ex.SqliteErrorCode);
            }
        }

        [Fact]
        public void ExecuteScalar_throws_when_no_connection()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => new SqliteCommand().ExecuteScalar());

            Assert.Equal(Strings.CallRequiresOpenConnection("ExecuteScalar"), ex.Message);
        }

        [Fact]
        public void ExecuteScalar_throws_when_connection_closed()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var ex = Assert.Throws<InvalidOperationException>(() => connection.CreateCommand().ExecuteScalar());

                Assert.Equal(Strings.CallRequiresOpenConnection("ExecuteScalar"), ex.Message);
            }
        }

        [Fact]
        public void ExecuteReader_throws_when_transaction_required()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT 1;";
                connection.Open();

                using (connection.BeginTransaction())
                {
                    var ex = Assert.Throws<InvalidOperationException>(() => command.ExecuteReader());

                    Assert.Equal(Strings.TransactionRequired, ex.Message);
                }
            }
        }

        [Fact]
        public void ExecuteScalar_throws_when_no_command_text()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                connection.Open();

                var ex = Assert.Throws<InvalidOperationException>(() => connection.CreateCommand().ExecuteScalar());

                Assert.Equal(Strings.CallRequiresSetCommandText("ExecuteScalar"), ex.Message);
            }
        }

        [Fact]
        public void ExecuteScalar_returns_null_when_empty()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT 1 WHERE 0 = 1;";
                connection.Open();

                Assert.Null(command.ExecuteScalar());
            }
        }

        [Fact]
        public void ExecuteScalar_returns_long_when_integer()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT 1;";
                connection.Open();

                Assert.Equal(1L, command.ExecuteScalar());
            }
        }

        [Fact]
        public void ExecuteScalar_returns_double_when_real()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT 3.14;";
                connection.Open();

                Assert.Equal(3.14, command.ExecuteScalar());
            }
        }

        [Fact]
        public void ExecuteScalar_returns_string_when_text()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT 'test';";
                connection.Open();

                Assert.Equal("test", command.ExecuteScalar());
            }
        }

        [Fact]
        public void ExecuteScalar_returns_byte_array_when_blob()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT x'7e57';";
                connection.Open();

                Assert.Equal(new byte[] { 0x7e, 0x57 }, command.ExecuteScalar());
            }
        }

        [Fact]
        public void ExecuteScalar_returns_DBNull_when_null()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT NULL;";
                connection.Open();

                Assert.Equal(DBNull.Value, command.ExecuteScalar());
            }
        }

        [Fact]
        public void ExecuteReader_binds_parameters()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT @Parameter;";
                command.Parameters.AddWithValue("@Parameter", 1);
                connection.Open();

                Assert.Equal(1L, command.ExecuteScalar());
            }
        }

        [Fact]
        public void ExecuteReader_throws_when_parameter_unset()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT @Parameter;";
                connection.Open();

                var ex = Assert.Throws<InvalidOperationException>(() => command.ExecuteScalar());
                Assert.Equal(Strings.MissingParameters("@Parameter"), ex.Message);
            }
        }

        [Fact]
        public void ExecuteScalar_returns_long_when_batching()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT 42; SELECT 43;";
                connection.Open();

                Assert.Equal(42L, command.ExecuteScalar());
            }
        }

        [Fact]
        public void ExecuteScalar_returns_long_when_multiple_columns()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT 42, 43;";
                connection.Open();

                Assert.Equal(42L, command.ExecuteScalar());
            }
        }

        [Fact]
        public void ExecuteScalar_returns_long_when_multiple_rows()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT 42 UNION SELECT 43;";
                connection.Open();

                Assert.Equal(42L, command.ExecuteScalar());
            }
        }

        [Fact]
        public void ExecuteNonQuery_throws_when_no_connection()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => new SqliteCommand().ExecuteNonQuery());

            Assert.Equal(Strings.CallRequiresOpenConnection("ExecuteNonQuery"), ex.Message);
        }

        [Fact]
        public void ExecuteNonQuery_throws_when_connection_closed()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var ex = Assert.Throws<InvalidOperationException>(() => connection.CreateCommand().ExecuteNonQuery());

                Assert.Equal(Strings.CallRequiresOpenConnection("ExecuteNonQuery"), ex.Message);
            }
        }

        [Fact]
        public void ExecuteNonQuery_throws_when_no_command_text()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                connection.Open();

                var ex = Assert.Throws<InvalidOperationException>(() => connection.CreateCommand().ExecuteNonQuery());

                Assert.Equal(Strings.CallRequiresSetCommandText("ExecuteNonQuery"), ex.Message);
            }
        }

        [Fact]
        public void ExecuteReader_throws_when_transaction_mismatched()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT 1;";
                connection.Open();

                using (var otherConnection = new SqliteConnection("Data Source=:memory:"))
                {
                    otherConnection.Open();

                    using (var transction = otherConnection.BeginTransaction())
                    {
                        command.Transaction = transction;

                        var ex = Assert.Throws<InvalidOperationException>(() => command.ExecuteReader());

                        Assert.Equal(Strings.TransactionConnectionMismatch, ex.Message);
                    }
                }
            }
        }

        [Fact]
        public void ExecuteNonQuery_works()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT 1;";
                connection.Open();

                Assert.Equal(-1, command.ExecuteNonQuery());
            }
        }

        [Fact]
        public void ExecuteReader_works()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT 1;";
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    Assert.NotNull(reader);
                }
            }
        }

        [Fact]
        public void ExecuteReader_skips_DML_statements()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                connection.Open();
                connection.ExecuteNonQuery("CREATE TABLE Test(Value);");

                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Test VALUES(1);
                    SELECT 1;";

                using (var reader = command.ExecuteReader())
                {
                    var hasData = reader.Read();
                    Assert.True(hasData);

                    Assert.Equal(1L, reader.GetInt64(0));
                }
            }
        }

        [Fact]
        public void ExecuteReader_works_when_comments()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "-- TODO: Write SQL";
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    Assert.False(reader.HasRows);
                    Assert.Equal(-1, reader.RecordsAffected);
                }
            }
        }

        [Fact]
        public void ExecuteReader_works_when_trailing_comments()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT 0; -- My favorite number";
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    var hasResult = reader.NextResult();
                    Assert.False(hasResult);
                }
            }
        }

        [Fact]
        public void Cancel_does_nothing()
        {
            new SqliteCommand().Cancel();
        }

        [Fact]
        public void ExecuteReader_supports_SequentialAccess()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT 0;";
                connection.Open();

                using (var reader = command.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    var hasResult = reader.NextResult();
                    Assert.False(hasResult);
                }
            }
        }

        [Fact]
        public void ExecuteReader_supports_SingleResult()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT 0;";
                connection.Open();

                using (var reader = command.ExecuteReader(CommandBehavior.SingleResult))
                {
                    var hasResult = reader.NextResult();
                    Assert.False(hasResult);
                }
            }
        }

        [Fact]
        public void ExecuteReader_supports_SingleRow()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT 0;";
                connection.Open();

                using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    var hasResult = reader.NextResult();
                    Assert.False(hasResult);
                }
            }
        }

        [Fact]
        public void ExecuteReader_supports_CloseConnection()
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT 0;";
                connection.Open();

                using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    var hasResult = reader.NextResult();
                    Assert.False(hasResult);
                }
                Assert.Equal(ConnectionState.Closed, connection.State);
            }
        }

        [Theory]
        [InlineData(CommandBehavior.KeyInfo)]
        [InlineData(CommandBehavior.SchemaOnly)]
        public void ExecuteReader_throws_for_unsupported_(CommandBehavior behavior)
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT 0;";
                connection.Open();

                var ex = Assert.Throws<ArgumentException>(() => command.ExecuteReader(behavior));
                Assert.Equal(Strings.InvalidCommandBehavior(behavior), ex.Message);
            }
        }

        //[Theory]
        //[InlineData(true)]
        //[InlineData(false)]
        //public Task ExecuteReader_retries_when_locked(bool extendedErrorCode)
        //{
        //    const string connectionString = "Data Source=locked;Mode=Memory;Cache=Shared";

        //    var selectedSignal = new AutoResetEvent(initialState: false);

        //    return Task.WhenAll(
        //        Task.Run(
        //            async () =>
        //            {
        //                using (var connection = new SqliteConnection(connectionString))
        //                {
        //                    connection.Open();
        //                    if (extendedErrorCode)
        //                    {
        //                        NativeMethods.sqlite3_extended_result_codes(connection.Handle, 1);
        //                    }

        //                    connection.ExecuteNonQuery(
        //                        "CREATE TABLE Data (Value); INSERT INTO Data VALUES (0);");

        //                    using (connection.ExecuteReader("SELECT * FROM Data;"))
        //                    {
        //                        selectedSignal.Set();

        //                        await Task.Delay(1000);
        //                    }
        //                }
        //            }),
        //        Task.Run(
        //            () =>
        //            {
        //                using (var connection = new SqliteConnection(connectionString))
        //                {
        //                    connection.Open();
        //                    if (extendedErrorCode)
        //                    {
        //                        raw.sqlite3_extended_result_codes(connection.DbHandle, 1);
        //                    }

        //                    selectedSignal.WaitOne();

        //                    var command = connection.CreateCommand();
        //                    command.CommandText = "DROP TABLE Data;";

        //                    command.ExecuteNonQuery();
        //                }
        //            }));
        //}
    }
}
