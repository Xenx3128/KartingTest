using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.AspNetCore.Identity;
using System;
using TestMVC.Models;

namespace TestMVC.Migrations
{
    public partial class SeedAdminAccount3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed Admin Role
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { 1, "Admin", "ADMIN", Guid.NewGuid().ToString() });

            // Seed Admin User
            var adminEmail = "admin@example.com";
            var adminPassword = "Admin@123"; // Temporary password for hashing
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var adminUser = new ApplicationUser
            {
                Id = 1,
                UserName = adminEmail,
                NormalizedUserName = adminEmail.ToUpper(),
                Email = adminEmail,
                NormalizedEmail = adminEmail.ToUpper(),
                EmailConfirmed = true,
                FullName = "Admin User",
                BirthDate = DateTime.UtcNow, // Required
                FromWhereFoundOut = null,
                //Status = null,
                Note = null,
                AcceptTerms = true, // Required
                ReceivePromotions = false, // Required
                RegistrationDate = DateTime.UtcNow, // Required
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumber = null,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnd = null,
                LockoutEnabled = true,
                AccessFailedCount = 0
            };
            var hashedPassword = passwordHasher.HashPassword(adminUser, adminPassword);

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] {
                    "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed",
                    "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed",
                    "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount", "FullName",
                    "BirthDate", "FromWhereFoundOut", "Note", "AcceptTerms", "ReceivePromotions",
                    "RegistrationDate"
                },
                values: new object[] {
                    adminUser.Id,
                    adminUser.UserName,
                    adminUser.NormalizedUserName,
                    adminUser.Email,
                    adminUser.NormalizedEmail,
                    adminUser.EmailConfirmed,
                    hashedPassword,
                    adminUser.SecurityStamp,
                    adminUser.ConcurrencyStamp,
                    adminUser.PhoneNumber,
                    adminUser.PhoneNumberConfirmed,
                    adminUser.TwoFactorEnabled,
                    adminUser.LockoutEnd,
                    adminUser.LockoutEnabled,
                    adminUser.AccessFailedCount,
                    adminUser.FullName,
                    adminUser.BirthDate,
                    adminUser.FromWhereFoundOut,
                    //adminUser.Status,
                    adminUser.Note,
                    adminUser.AcceptTerms,
                    adminUser.ReceivePromotions,
                    adminUser.RegistrationDate
                });

            // Assign Admin Role to Admin User
            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { 1, 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove Admin User Role
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { 1, 1 });

            // Remove Admin User
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1);

            // Remove Admin Role
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}