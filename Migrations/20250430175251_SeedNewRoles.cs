using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TestMVC.Migrations
{
    public partial class SeedNewRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed Superadmin Role
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { 2, "SuperAdmin", "SUPERADMIN", Guid.NewGuid().ToString() });

            // Seed User Role
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { 3, "User", "USER", Guid.NewGuid().ToString() });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            // Remove Superadmin Role
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2);

            // Remove User Role
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}