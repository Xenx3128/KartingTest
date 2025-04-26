using Microsoft.AspNetCore.Identity;
using TestMVC.Models;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using System.Data;

namespace TestMVC.Data
{
    public class UserContext
    {
        private readonly string _connectionString;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserContext(
            string connectionString,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _connectionString = connectionString;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        // User Registration using Identity
        public async Task<IdentityResult> RegisterUserAsync(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        // User Login using Identity
        public async Task<SignInResult> LoginUserAsync(string email, string password, bool rememberMe)
        {
            return await _signInManager.PasswordSignInAsync(
                email, password, rememberMe, lockoutOnFailure: false);
        }

        // Get user by email (Dapper query example)
        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT * FROM ""AspNetUsers"" WHERE ""Email"" = @Email";
            return await connection.QuerySingleOrDefaultAsync<ApplicationUser>(sql, new { Email = email });
        }

        // Update user profile (hybrid approach)
        public async Task<IdentityResult> UpdateUserProfileAsync(ApplicationUser user)
        {
            // Update core identity properties
            var result = await _userManager.UpdateAsync(user);
            
            if (!result.Succeeded) return result;

            // Update custom properties via Dapper if needed
            using var connection = CreateConnection();
            var sql = @"
                UPDATE ""AspNetUsers"" 
                SET ""FullName"" = @FullName,
                    ""BirthDate"" = @BirthDate,
                    ""PhoneNum"" = @PhoneNum,
                    ""FromWhereFoundOut"" = @FromWhereFoundOut,
                    ""Status"" = @Status,
                    ""Note"" = @Note
                WHERE ""Id"" = @Id";
            
            await connection.ExecuteAsync(sql, new {
                user.FullName,
                user.BirthDate,
                user.PhoneNum,
                user.FromWhereFoundOut,
                user.Status,
                user.Note,
                user.Id
            });

            return result;
        }

        // Get all users (Dapper query)
        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            using var connection = CreateConnection();
            var sql = @"SELECT * FROM ""AspNetUsers""";
            return await connection.QueryAsync<ApplicationUser>(sql);
        }

        // Delete user (Identity + Dapper cleanup)
        public async Task<IdentityResult> DeleteUserAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            // First delete related data if needed
            using var connection = CreateConnection();
            await connection.ExecuteAsync(
                @"DELETE FROM ""UserCarts"" WHERE ""UserId"" = @UserId",
                new { UserId = userId });

            // Then delete the user
            return await _userManager.DeleteAsync(user);
        }

        // Check if email exists (Dapper example)
        public async Task<bool> EmailExistsAsync(string email)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT COUNT(1) FROM ""AspNetUsers"" WHERE ""Email"" = @Email";
            return await connection.ExecuteScalarAsync<bool>(sql, new { Email = email });
        }

        //Roles

        public async Task<IdentityResult> AssignRoleAsync(int userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            return await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IList<string>> GetUserRolesAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user != null 
                ? await _userManager.GetRolesAsync(user) 
                : new List<string>();
        }

        public async Task<bool> IsInRoleAsync(int userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user != null && await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<List<RaceHistoryViewModel>> GetUserRaceHistoryAsync(int userId)
        {
            using var connection = CreateConnection();
            var sql = @"
                    SELECT r.StartDate, r.FinishDate, r.Category
                    FROM Races r
                    INNER JOIN Orders o ON r.OrderId = o.Id
                    WHERE o.UserId = @UserId
                    ORDER BY r.StartDate DESC";
            var raceHistory = await connection.QueryAsync<RaceHistoryViewModel>(
                sql,
                new { UserId = userId }
            );

            return raceHistory.AsList();
        }
    }
}