using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QA.Data.Migrations
{
    /// <inheritdoc />
    public partial class addCurrentStepToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserCurrentStep",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserCurrentStep",
                table: "Users");
        }
    }
}
