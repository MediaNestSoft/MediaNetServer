using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaNetServer.Migrations
{
    /// <inheritdoc />
    public partial class FixTmdbRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_MediaItems_tmdbId",
                table: "Images");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_MediaItems_TMDbId",
                table: "MediaItems",
                column: "TMDbId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_MediaItems_tmdbId",
                table: "Images",
                column: "tmdbId",
                principalTable: "MediaItems",
                principalColumn: "TMDbId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_MediaItems_tmdbId",
                table: "Images");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_MediaItems_TMDbId",
                table: "MediaItems");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_MediaItems_tmdbId",
                table: "Images",
                column: "tmdbId",
                principalTable: "MediaItems",
                principalColumn: "MediaId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
