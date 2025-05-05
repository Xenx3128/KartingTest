using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TestMVC.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBreaks2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TechnicalBreaks_CartStatuses_CartStatusId",
                table: "TechnicalBreaks");

            migrationBuilder.DropTable(
                name: "CartStatuses");

            migrationBuilder.RenameColumn(
                name: "CartStatusId",
                table: "TechnicalBreaks",
                newName: "BreakStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_TechnicalBreaks_CartStatusId",
                table: "TechnicalBreaks",
                newName: "IX_TechnicalBreaks_BreakStatusId");

            migrationBuilder.CreateTable(
                name: "BreakStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BreakStatuses", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_TechnicalBreaks_BreakStatuses_BreakStatusId",
                table: "TechnicalBreaks",
                column: "BreakStatusId",
                principalTable: "BreakStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TechnicalBreaks_BreakStatuses_BreakStatusId",
                table: "TechnicalBreaks");

            migrationBuilder.DropTable(
                name: "BreakStatuses");

            migrationBuilder.RenameColumn(
                name: "BreakStatusId",
                table: "TechnicalBreaks",
                newName: "CartStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_TechnicalBreaks_BreakStatusId",
                table: "TechnicalBreaks",
                newName: "IX_TechnicalBreaks_CartStatusId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_TechnicalBreaks_CartStatuses_CartStatusId",
                table: "TechnicalBreaks",
                column: "CartStatusId",
                principalTable: "CartStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
