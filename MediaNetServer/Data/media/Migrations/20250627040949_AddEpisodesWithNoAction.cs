using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Media.Migrations
{
    /// <inheritdoc />
    public partial class AddEpisodesWithNoAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Episodes",
                columns: table => new
                {
                    mediaId = table.Column<int>(type: "int", nullable: false),
                    seasonNumber = table.Column<int>(type: "int", nullable: false),
                    episodeNumber = table.Column<int>(type: "int", nullable: false),
                    episodeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    duration = table.Column<int>(type: "int", nullable: false),
                    overview = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    stillPath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Episodes", x => x.mediaId);
                    table.ForeignKey(
                        name: "FK_Episodes_MediaItems_mediaId",
                        column: x => x.mediaId,
                        principalTable: "MediaItems",
                        principalColumn: "MediaId");
                    table.ForeignKey(
                        name: "FK_Episodes_Seasons_seasonNumber",
                        column: x => x.seasonNumber,
                        principalTable: "Seasons",
                        principalColumn: "mediaId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_seasonNumber",
                table: "Episodes",
                column: "seasonNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Episodes");
        }
    }
}
