using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaNetServer.Migrations
{
    /// <inheritdoc />
    public partial class FixRelation3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaCasts_MediaItems_MediaItemMediaId",
                table: "MediaCasts");

            migrationBuilder.DropIndex(
                name: "IX_MediaCasts_MediaItemMediaId",
                table: "MediaCasts");

            migrationBuilder.DropColumn(
                name: "MediaItemMediaId",
                table: "MediaCasts");

            migrationBuilder.CreateIndex(
                name: "IX_MediaCasts_tmdbId",
                table: "MediaCasts",
                column: "tmdbId");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaCasts_MediaItems_tmdbId",
                table: "MediaCasts",
                column: "tmdbId",
                principalTable: "MediaItems",
                principalColumn: "TMDbId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaCasts_MediaItems_tmdbId",
                table: "MediaCasts");

            migrationBuilder.DropIndex(
                name: "IX_MediaCasts_tmdbId",
                table: "MediaCasts");

            migrationBuilder.AddColumn<int>(
                name: "MediaItemMediaId",
                table: "MediaCasts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MediaCasts_MediaItemMediaId",
                table: "MediaCasts",
                column: "MediaItemMediaId");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaCasts_MediaItems_MediaItemMediaId",
                table: "MediaCasts",
                column: "MediaItemMediaId",
                principalTable: "MediaItems",
                principalColumn: "MediaId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
