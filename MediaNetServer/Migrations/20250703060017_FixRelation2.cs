using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaNetServer.Migrations
{
    /// <inheritdoc />
    public partial class FixRelation2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_MediaItems_tmdbId",
                table: "Files");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_MediaItems_tmdbId",
                table: "Files",
                column: "tmdbId",
                principalTable: "MediaItems",
                principalColumn: "TMDbId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_MediaItems_tmdbId",
                table: "Files");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_MediaItems_tmdbId",
                table: "Files",
                column: "tmdbId",
                principalTable: "MediaItems",
                principalColumn: "MediaId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
