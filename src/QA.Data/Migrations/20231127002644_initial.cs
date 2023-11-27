using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QA.Data.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFavoriteElements_Elements_QAElementId",
                table: "UserFavoriteElements");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFavoriteElements_Users_UserId",
                table: "UserFavoriteElements");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFavoriteElements_Elements_QAElementId",
                table: "UserFavoriteElements",
                column: "QAElementId",
                principalTable: "Elements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFavoriteElements_Users_UserId",
                table: "UserFavoriteElements",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFavoriteElements_Elements_QAElementId",
                table: "UserFavoriteElements");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFavoriteElements_Users_UserId",
                table: "UserFavoriteElements");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFavoriteElements_Elements_QAElementId",
                table: "UserFavoriteElements",
                column: "QAElementId",
                principalTable: "Elements",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFavoriteElements_Users_UserId",
                table: "UserFavoriteElements",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
