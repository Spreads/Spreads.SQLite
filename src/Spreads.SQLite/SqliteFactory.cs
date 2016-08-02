﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Data.Common;

namespace Microsoft.Data.Sqlite
{
    /// <summary>
    /// Creates instances of various Microsoft.Data.Sqlite classes.
    /// </summary>
    public class SqliteFactory : DbProviderFactory
    {
        private SqliteFactory()
        {
        }

        /// <summary>
        /// The singleton instance.
        /// </summary>
        public readonly static SqliteFactory Instance = new SqliteFactory();

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <returns>The new command.</returns>
        public override DbCommand CreateCommand()
            => new SqliteCommand();

        /// <summary>
        /// Creates a new connection.
        /// </summary>
        /// <returns>The new connection.</returns>
        public override DbConnection CreateConnection()
            => new SqliteConnection();

        /// <summary>
        /// Creates a new connection string builder.
        /// </summary>
        /// <returns>The new connection string builder.</returns>
        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
            => new SqliteConnectionStringBuilder();

        /// <summary>
        /// Creates a new parameter.
        /// </summary>
        /// <returns>The new parameter.</returns>
        public override DbParameter CreateParameter()
            => new SqliteParameter();
    }
}
