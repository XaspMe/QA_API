using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QA_API.Migrations
{
    /// <inheritdoc />
    public partial class add_fav_elements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsersFavoriteCategories");

            migrationBuilder.CreateTable(
                name: "UserFavoriteElements",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QAElementId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavoriteElements", x => new { x.UserId, x.QAElementId });
                    table.ForeignKey(
                        name: "FK_UserFavoriteElements_Elements_QAElementId",
                        column: x => x.QAElementId,
                        principalTable: "Elements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavoriteElements_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteElements_QAElementId",
                table: "UserFavoriteElements",
                column: "QAElementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFavoriteElements");

            migrationBuilder.CreateTable(
                name: "UsersFavoriteCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersFavoriteCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersFavoriteCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UsersFavoriteCategories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersFavoriteCategories_CategoryId",
                table: "UsersFavoriteCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersFavoriteCategories_UserId",
                table: "UsersFavoriteCategories",
                column: "UserId");
        }
    }
}
