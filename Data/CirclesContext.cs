using Dapper;
using Npgsql;
using System.Data;
using TestMVC.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace TestMVC.Data
{
    public class CircleResultsContext
    {
        private readonly string _connectionString;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CircleResultsContext(string connectionString, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = connectionString;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            Log.Information($"{GetUserPrefix()} CircleResultsContext инициализирован со строкой подключения.");
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

        public async Task<IEnumerable<CircleResults>> GetCircleResultsByUserRaceIdAsync(int userRaceId)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение результатов кругов для UserRaceId {userRaceId}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"
                    SELECT cr.""Id"", cr.""UserRaceId"", cr.""CircleNum"", cr.""CircleTime"",
                           ur.""Id"" AS Id, ur.""UserId"", ur.""RaceId"", ur.""CartId"", ur.""Position""
                    FROM ""CircleResults"" cr
                    LEFT JOIN ""UserRaces"" ur ON cr.""UserRaceId"" = ur.""Id""
                    WHERE cr.""UserRaceId"" = @UserRaceId
                    ORDER BY cr.""CircleNum""";
                var results = await connection.QueryAsync<CircleResults, UserRace, CircleResults>(
                    sql,
                    (circleResult, userRace) =>
                    {
                        circleResult.UserRace = userRace;
                        if (userRace == null)
                            Log.Warning($"{prefix} CircleResult {circleResult.Id} has null UserRace (UserRaceId: {circleResult.UserRaceId})");
                        return circleResult;
                    },
                    new { UserRaceId = userRaceId },
                    splitOn: "Id");
                Log.Information($"{prefix} Получено {results.Count()} результатов кругов для UserRaceId {userRaceId}");
                return results;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch circle results for UserRaceId {userRaceId}");
                throw;
            }
        }

        public async Task<CircleResults> GetCircleResultByIdAsync(int id)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение результата круга с ID {id}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"
                    SELECT cr.""Id"", cr.""UserRaceId"", cr.""CircleNum"", cr.""CircleTime"",
                           ur.""Id"" AS Id, ur.""UserId"", ur.""RaceId"", ur.""CartId"", ur.""Position""
                    FROM ""CircleResults"" cr
                    LEFT JOIN ""UserRaces"" ur ON cr.""UserRaceId"" = ur.""Id""
                    WHERE cr.""Id"" = @Id";
                var result = await connection.QueryAsync<CircleResults, UserRace, CircleResults>(
                    sql,
                    (circleResult, userRace) =>
                    {
                        circleResult.UserRace = userRace;
                        if (userRace == null)
                            Log.Warning($"{prefix} CircleResult {circleResult.Id} has null UserRace (UserRaceId: {circleResult.UserRaceId})");
                        return circleResult;
                    },
                    new { Id = id },
                    splitOn: "Id");
                Log.Information($"{prefix} Результат круга {id} успешно получен");
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch circle result with ID {id}");
                throw;
            }
        }

        public async Task<int> CreateCircleResultAsync(CircleResults circleResult)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Создание нового результата круга для UserRaceId {circleResult.UserRaceId}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"
                    INSERT INTO ""CircleResults"" (""UserRaceId"", ""CircleNum"", ""CircleTime"")
                    VALUES (@UserRaceId, @CircleNum, @CircleTime)
                    RETURNING ""Id""";
                var id = await connection.ExecuteScalarAsync<int>(sql, circleResult);
                Log.Information($"{prefix} Результат круга {id} успешно создан");
                return id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to create circle result for UserRaceId {circleResult.UserRaceId}");
                throw;
            }
        }

        public async Task UpdateCircleResultAsync(CircleResults circleResult)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Обновление результата круга с ID {circleResult.Id}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"
                    UPDATE ""CircleResults""
                    SET ""UserRaceId"" = @UserRaceId,
                        ""CircleNum"" = @CircleNum,
                        ""CircleTime"" = @CircleTime
                    WHERE ""Id"" = @Id";
                var affectedRows = await connection.ExecuteAsync(sql, circleResult);
                if (affectedRows == 0)
                {
                    Log.Warning($"{prefix} CircleResult {circleResult.Id} not found for update.");
                    throw new KeyNotFoundException($"Circle result with ID {circleResult.Id} not found.");
                }
                Log.Information($"{prefix} Результат круга {circleResult.Id} успешно обновлен");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to update circle result with ID {circleResult.Id}");
                throw;
            }
        }

        public async Task DeleteCircleResultAsync(int id)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Удаление результата круга с ID {id}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"DELETE FROM ""CircleResults"" WHERE ""Id"" = @Id";
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
                if (affectedRows == 0)
                {
                    Log.Warning($"{prefix} CircleResult {id} not found for deletion.");
                    throw new KeyNotFoundException($"Circle result with ID {id} not found.");
                }
                Log.Information($"{prefix} Результат круга {id} успешно удален");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to delete circle result with ID {id}");
                throw;
            }
        }

        public async Task<UserRace> GetUserRaceByIdAsync(int id)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение пользовательской гонки с ID {id}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"
                    SELECT ur.""Id"", ur.""UserId"", ur.""RaceId"", ur.""CartId"", ur.""Position"",
                           u.""Id"" AS Id, u.""Email"" AS Email, u.""FullName"" AS FullName,
                           r.""Id"" AS Id, r.""StartDate"", r.""FinishDate"", r.""RaceCategoryId"", r.""RaceStatusId"",
                           c.""Id"" AS Id, c.""Name"" AS Name
                    FROM ""UserRaces"" ur
                    LEFT JOIN ""AspNetUsers"" u ON ur.""UserId"" = u.""Id""
                    LEFT JOIN ""Races"" r ON ur.""RaceId"" = r.""Id""
                    LEFT JOIN ""Carts"" c ON ur.""CartId"" = c.""Id""
                    WHERE ur.""Id"" = @Id";
                var result = await connection.QueryAsync<UserRace, ApplicationUser, Races, Cart, UserRace>(
                    sql,
                    (userRace, user, race, cart) =>
                    {
                        userRace.User = user;
                        userRace.Race = race;
                        userRace.Cart = cart;
                        if (user == null)
                            Log.Warning($"{prefix} UserRace {userRace.Id} has null User (UserId: {userRace.UserId})");
                        if (race == null)
                            Log.Warning($"{prefix} UserRace {userRace.Id} has null Race (RaceId: {userRace.RaceId})");
                        if (cart == null && userRace.CartId.HasValue)
                            Log.Warning($"{prefix} UserRace {userRace.Id} has null Cart (CartId: {userRace.CartId})");
                        return userRace;
                    },
                    new { Id = id },
                    splitOn: "Id,Id,Id");
                Log.Information($"{prefix} Пользовательская гонка {id} успешно получена");
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch user race with ID {id}");
                throw;
            }
        }
    }
}