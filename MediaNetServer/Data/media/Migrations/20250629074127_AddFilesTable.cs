using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Media.Migrations
{
    /// <inheritdoc />
    public partial class AddFilesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "seasonNumber",
                table: "Images");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "seasonNumber",
                table: "Images",
                type: "int",
                nullable: true);
        }
    }
}
