using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.AspNetCore.Identity;
using System;
using TestMVC.Models;

#nullable disable

namespace TestMVC.Migrations
{
    /// <inheritdoc />
    public partial class SeedSuperAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            // Seed SuperAdmin User
            var superAdminEmail = "superadmin@example.com";
            var superAdminPassword = "SuperAdmin@123"; // Temporary password for hashing
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var superAdminUser = new ApplicationUser
            {
                UserName = superAdminEmail,
                NormalizedUserName = superAdminEmail.ToUpper(),
                Email = superAdminEmail,
                NormalizedEmail = superAdminEmail.ToUpper(),
                EmailConfirmed = true,
                FullName = "SuperAdmin User",
                BirthDate = DateTime.UtcNow, // Required
                FromWhereFoundOut = null,
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
            var hashedPassword = passwordHasher.HashPassword(superAdminUser, superAdminPassword);

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] {
                    "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed",
                    "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed",
                    "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount", "FullName",
                    "BirthDate", "FromWhereFoundOut", "Note", "AcceptTerms", "ReceivePromotions",
                    "RegistrationDate"
                },
                values: new object[] {
                    superAdminUser.UserName,
                    superAdminUser.NormalizedUserName,
                    superAdminUser.Email,
                    superAdminUser.NormalizedEmail,
                    superAdminUser.EmailConfirmed,
                    hashedPassword,
                    superAdminUser.SecurityStamp,
                    superAdminUser.ConcurrencyStamp,
                    superAdminUser.PhoneNumber,
                    superAdminUser.PhoneNumberConfirmed,
                    superAdminUser.TwoFactorEnabled,
                    superAdminUser.LockoutEnd,
                    superAdminUser.LockoutEnabled,
                    superAdminUser.AccessFailedCount,
                    superAdminUser.FullName,
                    superAdminUser.BirthDate,
                    superAdminUser.FromWhereFoundOut,
                    superAdminUser.Note,
                    superAdminUser.AcceptTerms,
                    superAdminUser.ReceivePromotions,
                    superAdminUser.RegistrationDate
                });

            // Assign SuperAdmin Role to SuperAdmin User
            // Use subquery to get RoleId and UserId dynamically
            migrationBuilder.Sql(@"
                INSERT INTO ""AspNetUserRoles"" (""UserId"", ""RoleId"")
                SELECT u.""Id"", r.""Id""
                FROM ""AspNetUsers"" u
                CROSS JOIN ""AspNetRoles"" r
                WHERE u.""Email"" = 'superadmin@example.com'
                AND r.""Name"" = 'SuperAdmin';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove SuperAdmin User Role
            migrationBuilder.Sql(@"
                DELETE FROM ""AspNetUserRoles""
                WHERE ""UserId"" IN (SELECT ""Id"" FROM ""AspNetUsers"" WHERE ""Email"" = 'superadmin@example.com')
                AND ""RoleId"" IN (SELECT ""Id"" FROM ""AspNetRoles"" WHERE ""Name"" = 'SuperAdmin');
            ");

            // Remove SuperAdmin User
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Email",
                keyValue: "superadmin@example.com");

            // Remove SuperAdmin Role
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Name",
                keyValue: "SuperAdmin");
        }
    }
}
