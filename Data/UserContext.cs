using Microsoft.AspNetCore.Identity;
using TestMVC.Models;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using System.Data;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System;

namespace TestMVC.Data
{
    public class UserContext
    {
        private readonly string _connectionString;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(
            string connectionString,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = connectionString;
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            Log.Information($"{GetUserPrefix()} UserContext инициализирован со строкой подключения и сервисами Identity.");
        }

        private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        private string GetUserPrefix()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(user);
                var applicationUser = _userManager.FindByIdAsync(userId).GetAwaiter().GetResult();
                var roles = _userManager.GetRolesAsync(applicationUser).GetAwaiter().GetResult();
                var role = roles.FirstOrDefault() ?? "NoRole";
                return $"[{applicationUser?.Email ?? "UnknownEmail"}] [{role}]";
            }
            return "[Unknown]";
        }

        public async Task<IdentityResult> RegisterUserAsync(ApplicationUser user, string password)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Регистрация пользователя с email {user.Email}");
            try
            {
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    Log.Information($"{prefix} Пользователь с email {user.Email} успешно зарегистрирован");
                }
                else
                {
                    Log.Warning($"{prefix} User registration failed for email {user.Email}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to register user with email {user.Email}");
                throw;
            }
        }

        public async Task<SignInResult> LoginUserAsync(string email, string password, bool rememberMe)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Попытка входа для пользователя с email {email}");
            try
            {
                var result = await _signInManager.PasswordSignInAsync(
                    email, password, rememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    Log.Information($"{prefix} Пользователь с email {email} успешно вошел");
                }
                else
                {
                    Log.Warning($"{prefix} Login failed for user with email {email}");
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to login user with email {email}");
                throw;
            }
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение пользователя с email {email}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"SELECT * FROM ""AspNetUsers"" WHERE ""Email"" = @Email";
                var result = await connection.QuerySingleOrDefaultAsync<ApplicationUser>(sql, new { Email = email });
                Log.Information($"{prefix} Пользователь с email {email} успешно получен");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch user with email {email}");
                throw;
            }
        }

        public async Task<IdentityResult> UpdateUserProfileAsync(ApplicationUser user)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Обновление профиля для пользователя с ID {user.Id}");
            try
            {
                var result = await _userManager.UpdateAsync(user);
                
                if (!result.Succeeded)
                {
                    Log.Warning($"{prefix} Failed to update Identity profile for user with ID {user.Id}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    return result;
                }

                using var connection = CreateConnection();
                var sql = @"
                    UPDATE ""AspNetUsers"" 
                    SET ""FullName"" = @FullName,
                        ""BirthDate"" = @BirthDate,
                        ""PhoneNumber"" = @PhoneNum,
                        ""FromWhereFoundOut"" = @FromWhereFoundOut,
                        ""Note"" = @Note
                    WHERE ""Id"" = @Id";
                
                await connection.ExecuteAsync(sql, new {
                    user.FullName,
                    user.BirthDate,
                    user.PhoneNumber,
                    user.FromWhereFoundOut,
                    user.Note,
                    user.Id
                });

                Log.Information($"{prefix} Профиль для пользователя с ID {user.Id} успешно обновлен");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to update profile for user with ID {user.Id}");
                throw;
            }
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение всех пользователей");
            try
            {
                using var connection = CreateConnection();
                var sql = @"SELECT * FROM ""AspNetUsers""";
                var result = await connection.QueryAsync<ApplicationUser>(sql);
                Log.Information($"{prefix} Получено {result.Count()} пользователей");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch all users");
                throw;
            }
        }

        public async Task<IdentityResult> DeleteUserAsync(int userId)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Удаление пользователя с ID {userId}");
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    Log.Warning($"{prefix} User with ID {userId} not found for deletion");
                    return IdentityResult.Failed(new IdentityError { Description = "User not found" });
                }

                using var connection = CreateConnection();
                await connection.ExecuteAsync(
                    @"DELETE FROM ""UserCarts"" WHERE ""UserId"" = @UserId",
                    new { UserId = userId });

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    Log.Information($"{prefix} Пользователь с ID {userId} успешно удален");
                }
                else
                {
                    Log.Warning($"{prefix} Failed to delete user with ID {userId}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to delete user with ID {userId}");
                throw;
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Проверка существования email {email}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"SELECT COUNT(1) FROM ""AspNetUsers"" WHERE ""Email"" = @Email";
                var exists = await connection.ExecuteScalarAsync<bool>(sql, new { Email = email });
                Log.Information($"{prefix} Email {email} существует: {exists}");
                return exists;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to check if email {email} exists");
                throw;
            }
        }

        public async Task<IdentityResult> AssignRoleAsync(int userId, string roleName)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Назначение роли {roleName} пользователю с ID {userId}");
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    Log.Warning($"{prefix} User with ID {userId} not found for role assignment");
                    return IdentityResult.Failed(new IdentityError { Description = "User not found" });
                }

                var result = await _userManager.AddToRoleAsync(user, roleName);
                if (result.Succeeded)
                {
                    Log.Information($"{prefix} Роль {roleName} успешно назначена пользователю с ID {userId}");
                }
                else
                {
                    Log.Warning($"{prefix} Failed to assign role {roleName} to user with ID {userId}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to assign role {roleName} to user with ID {userId}");
                throw;
            }
        }

        public async Task<IList<string>> GetUserRolesAsync(int userId)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение ролей для пользователя с ID {userId}");
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    Log.Warning($"{prefix} User with ID {userId} not found for role retrieval");
                    return new List<string>();
                }

                var roles = await _userManager.GetRolesAsync(user);
                Log.Information($"{prefix} Получено {roles.Count} ролей для пользователя с ID {userId}");
                return roles;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch roles for user with ID {userId}");
                throw;
            }
        }

        public async Task<bool> IsInRoleAsync(int userId, string roleName)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Проверка, находится ли пользователь с ID {userId} в роли {roleName}");
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    Log.Warning($"{prefix} User with ID {userId} not found for role check");
                    return false;
                }

                var isInRole = await _userManager.IsInRoleAsync(user, roleName);
                Log.Information($"{prefix} Пользователь с ID {userId} находится в роли {roleName}: {isInRole}");
                return isInRole;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to check if user with ID {userId} is in role {roleName}");
                throw;
            }
        }

        public async Task<List<RaceHistoryViewModel>> GetUserRaceHistoryAsync(int userId)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение истории гонок для пользователя с ID {userId}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"
                    SELECT 
                        ur.""Id"" AS UserRaceId,
                        r.""Id"" AS RaceId,
                        r.""StartDate"",
                        r.""FinishDate"",
                        rc.""Category"" AS Category,
                        rs.""Status"" AS RaceStatus,
                        c.""Name"" AS CartName,
                        ur.""Position""
                    FROM ""UserRaces"" ur
                    INNER JOIN ""Races"" r ON ur.""RaceId"" = r.""Id""
                    INNER JOIN ""RaceCategories"" rc ON r.""RaceCategoryId"" = rc.""Id""
                    INNER JOIN ""RaceStatuses"" rs ON r.""RaceStatusId"" = rs.""Id""
                    LEFT JOIN ""Carts"" c ON ur.""CartId"" = c.""Id""
                    WHERE ur.""UserId"" = @UserId
                    ORDER BY r.""StartDate"" DESC";
                
                var history = await connection.QueryAsync<RaceHistoryViewModel>(
                    sql,
                    new { UserId = userId }
                );

                Log.Information($"{prefix} Получено {history.Count()} записей истории гонок для пользователя с ID {userId}");
                return history.ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch race history for user with ID {userId}");
                throw;
            }
        }
    }
}