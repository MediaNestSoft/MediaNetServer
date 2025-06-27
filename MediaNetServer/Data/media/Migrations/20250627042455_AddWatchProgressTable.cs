using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Media.Migrations
{
    /// <inheritdoc />
    public partial class AddWatchProgressTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WatchProgress",
                columns: table => new
                {
                    watchProgressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    mediaId = table.Column<int>(type: "int", nullable: false),
                    lastWatched = table.Column<DateTime>(type: "datetime2", nullable: false),
                    position = table.Column<int>(type: "int", nullable: false),
                    seasonNumber = table.Column<int>(type: "int", nullable: false),
                    episodeNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchProgress", x => x.watchProgressId);
                    table.ForeignKey(
                        name: "FK_WatchProgress_MediaItems_mediaId",
                        column: x => x.mediaId,
                        principalTable: "MediaItems",
                        principalColumn: "MediaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WatchProgress_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WatchProgress_mediaId",
                table: "WatchProgress",
                column: "mediaId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchProgress_UserId",
                table: "WatchProgress",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WatchProgress");
        }
    }
}
