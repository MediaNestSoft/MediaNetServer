using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Media.Migrations
{
    /// <inheritdoc />
    public partial class AddSeriesDetailTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SeriesDetail",
                columns: table => new
                {
                    mediaId = table.Column<int>(type: "int", nullable: false),
                    firstAirDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    numberOfSeasons = table.Column<int>(type: "int", nullable: false),
                    numberOfEpisodes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeriesDetail", x => x.mediaId);
                    table.ForeignKey(
                        name: "FK_SeriesDetail_MediaItems_mediaId",
                        column: x => x.mediaId,
                        principalTable: "MediaItems",
                        principalColumn: "MediaId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SeriesDetail");
        }
    }
}
