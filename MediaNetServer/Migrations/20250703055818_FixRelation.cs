using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaNetServer.Migrations
{
    /// <inheritdoc />
    public partial class FixRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_History_MediaItems_tmdbId",
                table: "History");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistItems_MediaItems_tmdbId",
                table: "PlaylistItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WatchProgress_MediaItems_tmdbId",
                table: "WatchProgress");

            migrationBuilder.AddForeignKey(
                name: "FK_History_MediaItems_tmdbId",
                table: "History",
                column: "tmdbId",
                principalTable: "MediaItems",
                principalColumn: "TMDbId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistItems_MediaItems_tmdbId",
                table: "PlaylistItems",
                column: "tmdbId",
                principalTable: "MediaItems",
                principalColumn: "TMDbId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WatchProgress_MediaItems_tmdbId",
                table: "WatchProgress",
                column: "tmdbId",
                principalTable: "MediaItems",
                principalColumn: "TMDbId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_History_MediaItems_tmdbId",
                table: "History");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistItems_MediaItems_tmdbId",
                table: "PlaylistItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WatchProgress_MediaItems_tmdbId",
                table: "WatchProgress");

            migrationBuilder.AddForeignKey(
                name: "FK_History_MediaItems_tmdbId",
                table: "History",
                column: "tmdbId",
                principalTable: "MediaItems",
                principalColumn: "MediaId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistItems_MediaItems_tmdbId",
                table: "PlaylistItems",
                column: "tmdbId",
                principalTable: "MediaItems",
                principalColumn: "MediaId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WatchProgress_MediaItems_tmdbId",
                table: "WatchProgress",
                column: "tmdbId",
                principalTable: "MediaItems",
                principalColumn: "MediaId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
