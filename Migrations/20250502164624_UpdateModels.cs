using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestMVC.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_UserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderStatuses_OrderStatusId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Races");

            migrationBuilder.AddColumn<int>(
                name: "RaceCategoryId",
                table: "Races",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RaceStatusId",
                table: "Races",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId",
                table: "Orders",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Races_RaceCategoryId",
                table: "Races",
                column: "RaceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Races_RaceStatusId",
                table: "Races",
                column: "RaceStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ApplicationUserId",
                table: "Orders",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_ApplicationUserId",
                table: "Orders",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderStatuses_OrderStatusId",
                table: "Orders",
                column: "OrderStatusId",
                principalTable: "OrderStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Races_RaceCategories_RaceCategoryId",
                table: "Races",
                column: "RaceCategoryId",
                principalTable: "RaceCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Races_RaceStatuses_RaceStatusId",
                table: "Races",
                column: "RaceStatusId",
                principalTable: "RaceStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_ApplicationUserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_UserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderStatuses_OrderStatusId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Races_RaceCategories_RaceCategoryId",
                table: "Races");

            migrationBuilder.DropForeignKey(
                name: "FK_Races_RaceStatuses_RaceStatusId",
                table: "Races");

            migrationBuilder.DropIndex(
                name: "IX_Races_RaceCategoryId",
                table: "Races");

            migrationBuilder.DropIndex(
                name: "IX_Races_RaceStatusId",
                table: "Races");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ApplicationUserId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RaceCategoryId",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "RaceStatusId",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Races",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Races",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderStatuses_OrderStatusId",
                table: "Orders",
                column: "OrderStatusId",
                principalTable: "OrderStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
