using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Microsoft.Data.Sqlite.Tests.EntityFramework {

    // this is stupid, but EF7 doesn't support class libraries
    // http://benjii.me/2016/06/entity-framework-core-migrations-for-class-library-projects/
    // http://www.michael-whelan.net/ef-core-101-migrations-in-separate-assembly/

    public static class Program {
        public static void Main() {
            var appDbContext = new BasicEFTests.BloggingContext();
            //https://github.com/aspnet/Microsoft.Data.Sqlite/issues/219
            appDbContext.Database.EnsureCreated();
            appDbContext.Database.Migrate();
            

        }
    }

    // https://docs.microsoft.com/en-us/ef/core/get-started/full-dotnet/new-db

    public class BasicEFTests {

        public class BloggingContext : DbContext {
            public DbSet<Blog> Blogs { get; set; }
            public DbSet<Post> Posts { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
                optionsBuilder.UseSqlite(@"Filename=..\..\Data\BasicEFTests.db");
            }
        }

        public class Blog {
            public int BlogId { get; set; }
            public string Url { get; set; }
            public string NewColumn { get; set; }
            public List<Post> Posts { get; set; }
        }

        public class Post {
            public int PostId { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }

            public int BlogId { get; set; }
            public Blog Blog { get; set; }
        }

        [Fact]
        public void CouldUseEF() {
            using (var db = new BloggingContext()) {
                db.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
                var count = db.SaveChanges();
                Console.WriteLine("{0} records saved to database", count);

                Console.WriteLine();
                Console.WriteLine("All blogs in database:");
                foreach (var blog in db.Blogs) {
                    Console.WriteLine(" - {0}", blog.Url);
                }
            }
        }
    }
}
