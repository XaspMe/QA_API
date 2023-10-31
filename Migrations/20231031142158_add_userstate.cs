using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QA_API.Migrations
{
    /// <inheritdoc />
    public partial class add_userstate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentQuestionId = table.Column<int>(type: "int", nullable: true),
                    TelegramChatId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserStates_Elements_CurrentQuestionId",
                        column: x => x.CurrentQuestionId,
                        principalTable: "Elements",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserStates_CurrentQuestionId",
                table: "UserStates",
                column: "CurrentQuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserStates");
        }
    }
}
