// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Spreads.SQLite.Properties;
using Xunit;

namespace Spreads.SQLite.Tests
{
    public class SqliteConnectionStringBuilderTest
    {
        [Fact]
        public void Ctor_parses_options()
        {
            var builder = new SqliteConnectionStringBuilder("Data Source=test.db");

            Assert.Equal("test.db", builder.DataSource);
        }

        [Fact]
        public void Ctor_parses_SharedCache()
        {
            Assert.Equal(SqliteCacheMode.Private, new SqliteConnectionStringBuilder("Cache=Private").Cache);
            Assert.Equal(SqliteCacheMode.Shared, new SqliteConnectionStringBuilder("Cache=Shared").Cache);
        }

        [Fact]
        public void Ctor_parses_mode()
        {
            var builder = new SqliteConnectionStringBuilder("Mode=Memory");

            Assert.Equal(SqliteOpenMode.Memory, builder.Mode);
        }

        [Fact]
        public void Filename_is_alias_for_DataSource()
        {
            var builder = new SqliteConnectionStringBuilder("Filename=inline.db");
            Assert.Equal("inline.db", builder.DataSource);
        }

        [Fact]
        public void It_takes_last_alias_specified()
        {
            var builder = new SqliteConnectionStringBuilder("Filename=ignore me.db; Data Source=and me too.db; DataSource=this_one.db");

            Assert.Equal("this_one.db", builder.DataSource);
        }

        [Fact]
        public void DataSource_works()
        {
            var builder = new SqliteConnectionStringBuilder();

            builder.DataSource = "test.db";

            Assert.Equal("test.db", builder.DataSource);
        }

        [Fact]
        public void DataSource_defaults_to_empty()
        {
            Assert.Empty(new SqliteConnectionStringBuilder().DataSource);
        }

        [Fact]
        public void Mode_works()
        {
            var builder = new SqliteConnectionStringBuilder();

            builder.Mode = SqliteOpenMode.Memory;

            Assert.Equal(SqliteOpenMode.Memory, builder.Mode);
        }

        [Fact]
        public void Mode_defaults_to_ReadWriteCreate()
            => Assert.Equal(SqliteOpenMode.ReadWriteCreate, new SqliteConnectionStringBuilder().Mode);

        [Fact]
        public void Cache_defaults()
        {
            Assert.Equal(SqliteCacheMode.Default, new SqliteConnectionStringBuilder().Cache);
        }

        [Fact]
        public void Keys_works()
        {
            var keys = (ICollection<string>)new SqliteConnectionStringBuilder().Keys;

            Assert.True(keys.IsReadOnly);
            Assert.Equal(3, keys.Count);
            Assert.Contains("Data Source", keys);
            Assert.Contains("Mode", keys);
            Assert.Contains("Cache", keys);
        }

        [Fact]
        public void Values_works()
        {
            var values = (ICollection<object>)new SqliteConnectionStringBuilder().Values;

            Assert.True(values.IsReadOnly);
            Assert.Equal(3, values.Count);
        }

        [Fact]
        public void Item_validates_argument()
        {
            var ex = Assert.Throws<ArgumentException>(() => new SqliteConnectionStringBuilder()["Invalid"]);
            Assert.Equal(Strings.KeywordNotSupported("Invalid"), ex.Message);

            ex = Assert.Throws<ArgumentException>(() => new SqliteConnectionStringBuilder()["Invalid"] = 0);
            Assert.Equal(Strings.KeywordNotSupported("Invalid"), ex.Message);
        }

        [Fact]
        public void Item_resets_value_when_null()
        {
            var builder = new SqliteConnectionStringBuilder();
            builder.DataSource = "test.db";

            builder["Data Source"] = null;

            Assert.Empty(builder.DataSource);
        }

        [Fact]
        public void Item_gets_value()
        {
            var builder = new SqliteConnectionStringBuilder();
            builder.DataSource = "test.db";

            Assert.Equal("test.db", builder["Data Source"]);
        }

        [Fact]
        public void Item_sets_value()
        {
            var builder = new SqliteConnectionStringBuilder();

            builder["Data Source"] = "test.db";

            Assert.Equal("test.db", builder.DataSource);
        }

        [Theory]
        [InlineData("Shared")]
        [InlineData("SHARED")]
        [InlineData(SqliteCacheMode.Shared)]
        [InlineData((int)SqliteCacheMode.Shared)]
        public void Item_converts_to_enum_on_set(object value)
        {
            var builder = new SqliteConnectionStringBuilder();

            builder["Cache"] = value;

            Assert.Equal(SqliteCacheMode.Shared, builder["Cache"]);
        }

        [Theory]
        [InlineData(42)]
        [InlineData("Unknown")]
        [InlineData((SqliteCacheMode)42)]
        [InlineData(SqliteOpenMode.ReadOnly)]
        public void Item_throws_when_cannot_convert_to_enum_on_set(object value)
        {
            var builder = new SqliteConnectionStringBuilder();

            Assert.ThrowsAny<ArgumentException>(() => builder["Cache"] = value);
        }

        [Fact]
        public void Clear_resets_everything()
        {
            var builder = new SqliteConnectionStringBuilder("Data Source=test.db;Mode=Memory;Cache=Shared");

            builder.Clear();

            Assert.Empty(builder.DataSource);
            Assert.Equal(SqliteOpenMode.ReadWriteCreate, builder.Mode);
            Assert.Equal(SqliteCacheMode.Default, builder.Cache);
        }

        [Fact]
        public void ContainsKey_returns_true_when_exists()
        {
            Assert.True(new SqliteConnectionStringBuilder().ContainsKey("Data Source"));
        }

        [Fact]
        public void ContainsKey_returns_false_when_not_exists()
        {
            Assert.False(new SqliteConnectionStringBuilder().ContainsKey("Invalid"));
        }

        [Fact]
        public void Remove_returns_false_when_not_exists()
        {
            Assert.False(new SqliteConnectionStringBuilder().Remove("Invalid"));
        }

        [Fact]
        public void Remove_resets_option()
        {
            var builder = new SqliteConnectionStringBuilder("Data Source=test.db");

            var removed = builder.Remove("Data Source");

            Assert.True(removed);
            Assert.Empty(builder.DataSource);
        }

        [Fact]
        public void ShouldSerialize_returns_false_when_not_exists()
        {
            Assert.False(new SqliteConnectionStringBuilder().ShouldSerialize("Invalid"));
        }

        //[ConditionalFact]
        //[FrameworkSkipCondition(
        //    RuntimeFrameworks.Mono,
        //    SkipReason = "Test fails with Mono 4.0.4. Build rarely reaches testing with Mono 4.2.1")]
        //public void ShouldSerialize_returns_false_when_unset()
        //{
        //    Assert.False(new SqliteConnectionStringBuilder().ShouldSerialize("Data Source"));
        //}

        //[ConditionalFact]
        //[FrameworkSkipCondition(
        //    RuntimeFrameworks.Mono,
        //    SkipReason = "Test fails with Mono 4.0.4. Build rarely reaches testing with Mono 4.2.1")]
        //public void ShouldSerialize_returns_true_when_set()
        //{
        //    var builder = new SqliteConnectionStringBuilder("Data Source=test.db");

        //    Assert.True(builder.ShouldSerialize("Data Source"));
        //}

        [Fact]
        public void TryGetValue_returns_false_when_not_exists()
        {
            object value;
            var retrieved = new SqliteConnectionStringBuilder().TryGetValue("Invalid", out value);

            Assert.False(retrieved);
            Assert.Null(value);
        }

        [Fact]
        public void TryGetValue_returns_true_when_exists()
        {
            var builder = new SqliteConnectionStringBuilder("Data Source=test.db");

            object value;
            var retrieved = builder.TryGetValue("Data Source", out value);

            Assert.True(retrieved);
            Assert.Equal("test.db", value);
        }

        [Fact]
        public void ToString_builds_string()
        {
            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = "test.db",
                Cache = SqliteCacheMode.Shared,
                Mode = SqliteOpenMode.Memory
            };

            Assert.Equal("Data Source=test.db;Mode=Memory;Cache=Shared", builder.ToString());
        }

        [Fact]
        public void ToString_builds_minimal_string()
        {
            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = "test.db"
            };

            Assert.Equal("Data Source=test.db", builder.ToString());
        }
    }
}
