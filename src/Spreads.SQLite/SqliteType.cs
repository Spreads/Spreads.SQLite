// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Spreads.SQLite.Interop;

namespace Spreads.SQLite
{
    /// <summary>
    /// Represents the type affinities used by columns in SQLite tables.
    /// </summary>
    /// <seealso href="http://sqlite.org/datatype3.html">Datatypes In SQLite Version 3</seealso>
    public enum SqliteType
    {
        /// <summary>
        /// A signed integer.
        /// </summary>
        Integer = Constants.SQLITE_INTEGER,

        /// <summary>
        /// A floating point value.
        /// </summary>
        Real = Constants.SQLITE_FLOAT,

        /// <summary>
        /// A text string.
        /// </summary>
        Text = Constants.SQLITE_TEXT,

        /// <summary>
        /// A blob of data.
        /// </summary>
        Blob = Constants.SQLITE_BLOB
    }
}
