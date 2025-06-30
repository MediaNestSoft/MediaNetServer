using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Media.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeleteBehaviorForEpisodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_MediaItems_mediaId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_MediaItems_mediaId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistItems_MediaItems_mediaId",
                table: "PlaylistItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistItems_Playlists_playlistId",
                table: "PlaylistItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Playlists_Users_UserId",
                table: "Playlists");

            migrationBuilder.DropForeignKey(
                name: "FK_WatchProgress_MediaItems_mediaId",
                table: "WatchProgress");

            migrationBuilder.DropForeignKey(
                name: "FK_WatchProgress_Users_UserId",
                table: "WatchProgress");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_MediaItems_mediaId",
                table: "Files",
                column: "mediaId",
                principalTable: "MediaItems",
                principalColumn: "MediaId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_MediaItems_mediaId",
                table: "Images",
                column: "mediaId",
                principalTable: "MediaItems",
                principalColumn: "MediaId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistItems_MediaItems_mediaId",
                table: "PlaylistItems",
                column: "mediaId",
                principalTable: "MediaItems",
                principalColumn: "MediaId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistItems_Playlists_playlistId",
                table: "PlaylistItems",
                column: "playlistId",
                principalTable: "Playlists",
                principalColumn: "playlistId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Playlists_Users_UserId",
                table: "Playlists",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WatchProgress_MediaItems_mediaId",
                table: "WatchProgress",
                column: "mediaId",
                principalTable: "MediaItems",
                principalColumn: "MediaId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WatchProgress_Users_UserId",
                table: "WatchProgress",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_MediaItems_mediaId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_MediaItems_mediaId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistItems_MediaItems_mediaId",
                table: "PlaylistItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistItems_Playlists_playlistId",
                table: "PlaylistItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Playlists_Users_UserId",
                table: "Playlists");

            migrationBuilder.DropForeignKey(
                name: "FK_WatchProgress_MediaItems_mediaId",
                table: "WatchProgress");

            migrationBuilder.DropForeignKey(
                name: "FK_WatchProgress_Users_UserId",
                table: "WatchProgress");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_MediaItems_mediaId",
                table: "Files",
                column: "mediaId",
                principalTable: "MediaItems",
                principalColumn: "MediaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_MediaItems_mediaId",
                table: "Images",
                column: "mediaId",
                principalTable: "MediaItems",
                principalColumn: "MediaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistItems_MediaItems_mediaId",
                table: "PlaylistItems",
                column: "mediaId",
                principalTable: "MediaItems",
                principalColumn: "MediaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistItems_Playlists_playlistId",
                table: "PlaylistItems",
                column: "playlistId",
                principalTable: "Playlists",
                principalColumn: "playlistId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Playlists_Users_UserId",
                table: "Playlists",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WatchProgress_MediaItems_mediaId",
                table: "WatchProgress",
                column: "mediaId",
                principalTable: "MediaItems",
                principalColumn: "MediaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WatchProgress_Users_UserId",
                table: "WatchProgress",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
