using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.AspNetCore.Identity;
using System;
using TestMVC.Models;
#nullable disable

namespace TestMVC.Migrations
{
    /// <inheritdoc />
    public partial class SeedGuestAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed Guest User with specific Id
            var guestEmail = "guest@example.com";
            var guestPassword = "Guestg8#z6h1X#Cb!cG-liC@123"; // Temporary password for hashing
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var guestUser = new ApplicationUser
            {
                Id = 10, // Explicitly set Id to 10
                UserName = guestEmail,
                NormalizedUserName = guestEmail.ToUpper(),
                Email = guestEmail,
                NormalizedEmail = guestEmail.ToUpper(),
                EmailConfirmed = false,
                FullName = "Guest User",
                BirthDate = DateTime.UtcNow, // Required
                FromWhereFoundOut = null,
                Note = "Default guest account for unauthorized orders",
                AcceptTerms = true, // Required
                ReceivePromotions = false, // Required
                RegistrationDate = DateTime.UtcNow, // Required
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumber = null,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnd = null,
                LockoutEnabled = false, // Disable lockout for guest account
                AccessFailedCount = 0
            };
            var hashedPassword = passwordHasher.HashPassword(guestUser, guestPassword);

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] {
                    "Id", // Added Id column
                    "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed",
                    "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed",
                    "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount", "FullName",
                    "BirthDate", "FromWhereFoundOut", "Note", "AcceptTerms", "ReceivePromotions",
                    "RegistrationDate"
                },
                values: new object[] {
                    guestUser.Id, // Set Id to 10
                    guestUser.UserName,
                    guestUser.NormalizedUserName,
                    guestUser.Email,
                    guestUser.NormalizedEmail,
                    guestUser.EmailConfirmed,
                    hashedPassword,
                    guestUser.SecurityStamp,
                    guestUser.ConcurrencyStamp,
                    guestUser.PhoneNumber,
                    guestUser.PhoneNumberConfirmed,
                    guestUser.TwoFactorEnabled,
                    guestUser.LockoutEnd,
                    guestUser.LockoutEnabled,
                    guestUser.AccessFailedCount,
                    guestUser.FullName,
                    guestUser.BirthDate,
                    guestUser.FromWhereFoundOut,
                    guestUser.Note,
                    guestUser.AcceptTerms,
                    guestUser.ReceivePromotions,
                    guestUser.RegistrationDate
                });

            // Assign User Role to Guest User
            // Use subquery to get RoleId and UserId dynamically
            migrationBuilder.Sql(@"
                INSERT INTO ""AspNetUserRoles"" (""UserId"", ""RoleId"")
                SELECT u.""Id"", r.""Id""
                FROM ""AspNetUsers"" u
                CROSS JOIN ""AspNetRoles"" r
                WHERE u.""Email"" = 'guest@example.com'
                AND r.""Name"" = 'User';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove Guest User Role
            migrationBuilder.Sql(@"
                DELETE FROM ""AspNetUserRoles""
                WHERE ""UserId"" IN (SELECT ""Id"" FROM ""AspNetUsers"" WHERE ""Email"" = 'guest@example.com')
                AND ""RoleId"" IN (SELECT ""Id"" FROM ""AspNetRoles"" WHERE ""Name"" = 'User');
            ");

            // Remove Guest User
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Email",
                keyValue: "guest@example.com");
        }
    }
}
