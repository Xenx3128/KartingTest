using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestMVC.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserRace2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "UserRaces",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "UserRaces");
        }
    }
}
