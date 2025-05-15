using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestMVC.Migrations
{
    /// <inheritdoc />
    public partial class ChangeForeignKeyOnDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CircleResults_UserRaces_UserRaceId",
                table: "CircleResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_UserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderStatuses_OrderStatusId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Races_Orders_OrderId",
                table: "Races");

            migrationBuilder.DropForeignKey(
                name: "FK_Races_RaceCategories_RaceCategoryId",
                table: "Races");

            migrationBuilder.DropForeignKey(
                name: "FK_Races_RaceStatuses_RaceStatusId",
                table: "Races");

            migrationBuilder.DropForeignKey(
                name: "FK_TechnicalBreaks_BreakStatuses_BreakStatusId",
                table: "TechnicalBreaks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRaces_AspNetUsers_UserId",
                table: "UserRaces");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRaces_Carts_CartId",
                table: "UserRaces");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRaces_Races_RaceId",
                table: "UserRaces");

            migrationBuilder.AddForeignKey(
                name: "FK_CircleResults_UserRaces_UserRaceId",
                table: "CircleResults",
                column: "UserRaceId",
                principalTable: "UserRaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Races_Orders_OrderId",
                table: "Races",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Races_RaceCategories_RaceCategoryId",
                table: "Races",
                column: "RaceCategoryId",
                principalTable: "RaceCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Races_RaceStatuses_RaceStatusId",
                table: "Races",
                column: "RaceStatusId",
                principalTable: "RaceStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TechnicalBreaks_BreakStatuses_BreakStatusId",
                table: "TechnicalBreaks",
                column: "BreakStatusId",
                principalTable: "BreakStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRaces_AspNetUsers_UserId",
                table: "UserRaces",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRaces_Carts_CartId",
                table: "UserRaces",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRaces_Races_RaceId",
                table: "UserRaces",
                column: "RaceId",
                principalTable: "Races",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CircleResults_UserRaces_UserRaceId",
                table: "CircleResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_UserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderStatuses_OrderStatusId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Races_Orders_OrderId",
                table: "Races");

            migrationBuilder.DropForeignKey(
                name: "FK_Races_RaceCategories_RaceCategoryId",
                table: "Races");

            migrationBuilder.DropForeignKey(
                name: "FK_Races_RaceStatuses_RaceStatusId",
                table: "Races");

            migrationBuilder.DropForeignKey(
                name: "FK_TechnicalBreaks_BreakStatuses_BreakStatusId",
                table: "TechnicalBreaks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRaces_AspNetUsers_UserId",
                table: "UserRaces");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRaces_Carts_CartId",
                table: "UserRaces");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRaces_Races_RaceId",
                table: "UserRaces");

            migrationBuilder.AddForeignKey(
                name: "FK_CircleResults_UserRaces_UserRaceId",
                table: "CircleResults",
                column: "UserRaceId",
                principalTable: "UserRaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_Races_Orders_OrderId",
                table: "Races",
                column: "OrderId",
                principalTable: "Orders",
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

            migrationBuilder.AddForeignKey(
                name: "FK_TechnicalBreaks_BreakStatuses_BreakStatusId",
                table: "TechnicalBreaks",
                column: "BreakStatusId",
                principalTable: "BreakStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRaces_AspNetUsers_UserId",
                table: "UserRaces",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRaces_Carts_CartId",
                table: "UserRaces",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRaces_Races_RaceId",
                table: "UserRaces",
                column: "RaceId",
                principalTable: "Races",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
