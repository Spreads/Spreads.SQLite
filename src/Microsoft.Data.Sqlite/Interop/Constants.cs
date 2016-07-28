// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Data.Sqlite.Interop
{
    internal static class Constants
    {
        // Result Codes

        /// Successful result					
        public const int SQLITE_OK = 0;
        /// SQL error or missing database					
        public const int SQLITE_ERROR = 1;
        /// Internal logic error in SQLite					
        public const int SQLITE_INTERNAL = 2;
        /// Access permission denied					
        public const int SQLITE_PERM = 3;
        /// Callback routine requested an abort					
        public const int SQLITE_ABORT = 4;
        /// The database file is locked					
        public const int SQLITE_BUSY = 5;
        /// A table in the database is locked					
        public const int SQLITE_LOCKED = 6;
        /// A malloc() failed					
        public const int SQLITE_NOMEM = 7;
        /// Attempt to write a readonly database					
        public const int SQLITE_READONLY = 8;
        /// Operation terminated by sqlite3_interrupt(					
        public const int SQLITE_INTERRUPT = 9;
        /// Some kind of disk I/O error occurred					
        public const int SQLITE_IOERR = 10;
        /// The database disk image is malformed					
        public const int SQLITE_CORRUPT = 11;
        /// Unknown opcode in sqlite3_file_control()					
        public const int SQLITE_NOTFOUND = 12;
        /// Insertion failed because database is full					
        public const int SQLITE_FULL = 13;
        /// Unable to open the database file					
        public const int SQLITE_CANTOPEN = 14;
        /// Database lock protocol error					
        public const int SQLITE_PROTOCOL = 15;
        /// Database is empty					
        public const int SQLITE_EMPTY = 16;
        /// The database schema changed					
        public const int SQLITE_SCHEMA = 17;
        /// String or BLOB exceeds size limit					
        public const int SQLITE_TOOBIG = 18;
        /// Abort due to constraint violation					
        public const int SQLITE_CONSTRAINT = 19;
        /// Data type mismatch					
        public const int SQLITE_MISMATCH = 20;
        /// Library used incorrectly					
        public const int SQLITE_MISUSE = 21;
        /// Uses OS features not supported on host					
        public const int SQLITE_NOLFS = 22;
        /// Authorization denied					
        public const int SQLITE_AUTH = 23;
        /// Auxiliary database format error					
        public const int SQLITE_FORMAT = 24;
        /// 2nd parameter to sqlite3_bind out of range					
        public const int SQLITE_RANGE = 25;
        /// File opened that is not a database file					
        public const int SQLITE_NOTADB = 26;
        /// Notifications from sqlite3_log()					
        public const int SQLITE_NOTICE = 27;
        /// Warnings from sqlite3_log()					
        public const int SQLITE_WARNING = 28;
        /// sqlite3_step() has another row ready					
        public const int SQLITE_ROW = 100;
        /// sqlite3_step() has finished executing					
        public const int SQLITE_DONE = 101;



        public const int SQLITE_INTEGER = 1;
        public const int SQLITE_FLOAT = 2;
        public const int SQLITE_TEXT = 3;
        public const int SQLITE_BLOB = 4;
        public const int SQLITE_NULL = 5;

        public const int SQLITE_OPEN_READONLY = 0x00000001;
        public const int SQLITE_OPEN_READWRITE = 0x00000002;
        public const int SQLITE_OPEN_CREATE = 0x00000004;
        public const int SQLITE_OPEN_URI = 0x00000040;
        public const int SQLITE_OPEN_MEMORY = 0x00000080;
        public const int SQLITE_OPEN_SHAREDCACHE = 0x00020000;
        public const int SQLITE_OPEN_PRIVATECACHE = 0x00040000;

        public static readonly IntPtr SQLITE_TRANSIENT = new IntPtr(-1);
    }
}
