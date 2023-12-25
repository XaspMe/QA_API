using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QA.Data.Migrations
{
    /// <inheritdoc />
    public partial class add_access_time : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastAccess",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastAccess",
                table: "Users");
        }
    }
}
