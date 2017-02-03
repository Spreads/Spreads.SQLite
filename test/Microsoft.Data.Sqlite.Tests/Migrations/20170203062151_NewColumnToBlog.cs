using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microsoft.Data.Sqlite.Tests.Migrations
{
    public partial class NewColumnToBlog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewColumn",
                table: "Blogs",
                nullable: true,
                defaultValue: null);
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Posts", table: "Blogs");
        }
    }
}
