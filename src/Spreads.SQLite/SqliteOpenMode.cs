﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Data.Sqlite
{
    /// <summary>
    /// Represents the connection modes that can be used when opening a connection.
    /// </summary>
    public enum SqliteOpenMode
    {
        /// <summary>
        /// Opens the database for reading and writing, and creates it if it doesn't exist.
        /// </summary>
        ReadWriteCreate,

        /// <summary>
        /// Opens the database for reading and writing.
        /// </summary>
        ReadWrite,

        /// <summary>
        /// Opens the database in read-only mode.
        /// </summary>
        ReadOnly,

        /// <summary>
        /// Opens an in-memory database.
        /// </summary>
        Memory
    }
}
