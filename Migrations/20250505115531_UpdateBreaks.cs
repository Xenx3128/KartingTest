using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TestMVC.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBreaks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "TechnicalBreaks",
                newName: "Desc");

            migrationBuilder.AddColumn<int>(
                name: "CartStatusId",
                table: "TechnicalBreaks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CartStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartStatuses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalBreaks_CartStatusId",
                table: "TechnicalBreaks",
                column: "CartStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_TechnicalBreaks_CartStatuses_CartStatusId",
                table: "TechnicalBreaks",
                column: "CartStatusId",
                principalTable: "CartStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TechnicalBreaks_CartStatuses_CartStatusId",
                table: "TechnicalBreaks");

            migrationBuilder.DropTable(
                name: "CartStatuses");

            migrationBuilder.DropIndex(
                name: "IX_TechnicalBreaks_CartStatusId",
                table: "TechnicalBreaks");

            migrationBuilder.DropColumn(
                name: "CartStatusId",
                table: "TechnicalBreaks");

            migrationBuilder.RenameColumn(
                name: "Desc",
                table: "TechnicalBreaks",
                newName: "Status");
        }
    }
}
