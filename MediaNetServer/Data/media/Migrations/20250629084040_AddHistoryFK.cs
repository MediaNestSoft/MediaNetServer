using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Media.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoryFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_History_MediaItems_mediaId",
                table: "History");

            migrationBuilder.DropForeignKey(
                name: "FK_History_Users_UserId",
                table: "History");

            migrationBuilder.AddForeignKey(
                name: "FK_History_MediaItems_mediaId",
                table: "History",
                column: "mediaId",
                principalTable: "MediaItems",
                principalColumn: "MediaId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_History_Users_UserId",
                table: "History",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_History_MediaItems_mediaId",
                table: "History");

            migrationBuilder.DropForeignKey(
                name: "FK_History_Users_UserId",
                table: "History");

            migrationBuilder.AddForeignKey(
                name: "FK_History_MediaItems_mediaId",
                table: "History",
                column: "mediaId",
                principalTable: "MediaItems",
                principalColumn: "MediaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_History_Users_UserId",
                table: "History",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
