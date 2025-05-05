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
    public class BreakContext
    {
        private readonly string _connectionString;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BreakContext(string connectionString, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = connectionString;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            Log.Information($"{GetUserPrefix()} BreakContext инициализирован со строкой подключения.");
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

        public async Task<IEnumerable<TechnicalBreaks>> GetAllBreaksAsync()
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение всех технических перерывов");
            try
            {
                using var connection = CreateConnection();
                var sql = @"
                    SELECT b.""Id"", b.""DateStart"", b.""DateFinish"", b.""Desc"", b.""BreakStatusId"",
                           bs.""Id"" AS Id, bs.""Status"" AS Status
                    FROM ""TechnicalBreaks"" b
                    LEFT JOIN ""BreakStatuses"" bs ON b.""BreakStatusId"" = bs.""Id""
                    ORDER BY b.""DateStart"" DESC";
                var breaks = await connection.QueryAsync<TechnicalBreaks, BreakStatuses, TechnicalBreaks>(
                    sql,
                    (breakItem, breakStatus) =>
                    {
                        breakItem.BreakStatus = breakStatus;
                        if (breakStatus == null)
                            Log.Warning($"{prefix} TechnicalBreak {breakItem.Id} has null BreakStatus (BreakStatusId: {breakItem.BreakStatusId})");
                        return breakItem;
                    },
                    splitOn: "Id");
                Log.Information($"{prefix} Получено {breaks.Count()} технических перерывов");
                return breaks;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch all technical breaks");
                throw;
            }
        }

        public async Task<TechnicalBreaks> GetBreakByIdAsync(int id)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение технического перерыва с ID {id}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"
                    SELECT b.""Id"", b.""DateStart"", b.""DateFinish"", b.""Desc"", b.""BreakStatusId"",
                           bs.""Id"" AS Id, bs.""Status"" AS Status
                    FROM ""TechnicalBreaks"" b
                    LEFT JOIN ""BreakStatuses"" bs ON b.""BreakStatusId"" = bs.""Id""
                    WHERE b.""Id"" = @Id";
                var result = await connection.QueryAsync<TechnicalBreaks, BreakStatuses, TechnicalBreaks>(
                    sql,
                    (breakItem, breakStatus) =>
                    {
                        breakItem.BreakStatus = breakStatus;
                        if (breakStatus == null)
                            Log.Warning($"{prefix} TechnicalBreak {breakItem.Id} has null BreakStatus (BreakStatusId: {breakItem.BreakStatusId})");
                        return breakItem;
                    },
                    new { Id = id },
                    splitOn: "Id");
                Log.Information($"{prefix} Технический перерыв {id} успешно получен");
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch technical break with ID {id}");
                throw;
            }
        }

        public async Task<int> CreateBreakAsync(TechnicalBreaks breakItem)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Создание нового технического перерыва, начиная с {breakItem.DateStart}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"
                    INSERT INTO ""TechnicalBreaks"" (""DateStart"", ""DateFinish"", ""Desc"", ""BreakStatusId"")
                    VALUES (@DateStart, @DateFinish, @Desc, @BreakStatusId)
                    RETURNING ""Id""";
                var id = await connection.ExecuteScalarAsync<int>(sql, breakItem);
                Log.Information($"{prefix} Технический перерыв {id} успешно создан");
                return id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to create technical break starting at {breakItem.DateStart}");
                throw;
            }
        }

        public async Task UpdateBreakAsync(TechnicalBreaks breakItem)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Обновление технического перерыва с ID {breakItem.Id}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"
                    UPDATE ""TechnicalBreaks""
                    SET ""DateStart"" = @DateStart,
                        ""DateFinish"" = @DateFinish,
                        ""Desc"" = @Desc,
                        ""BreakStatusId"" = @BreakStatusId
                    WHERE ""Id"" = @Id";
                var affectedRows = await connection.ExecuteAsync(sql, breakItem);
                if (affectedRows == 0)
                {
                    Log.Warning($"{prefix} TechnicalBreak {breakItem.Id} not found for update.");
                    throw new KeyNotFoundException($"Technical break with ID {breakItem.Id} not found.");
                }
                Log.Information($"{prefix} Технический перерыв {breakItem.Id} успешно обновлен");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to update technical break with ID {breakItem.Id}");
                throw;
            }
        }

        public async Task DeleteBreakAsync(int id)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Удаление технического перерыва с ID {id}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"DELETE FROM ""TechnicalBreaks"" WHERE ""Id"" = @Id";
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
                if (affectedRows == 0)
                {
                    Log.Warning($"{prefix} TechnicalBreak {id} not found for deletion.");
                    throw new KeyNotFoundException($"Technical break with ID {id} not found.");
                }
                Log.Information($"{prefix} Технический перерыв {id} успешно удален");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to delete technical break with ID {id}");
                throw;
            }
        }

        public async Task<List<BreakStatuses>> GetBreakStatusesAsync()
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение всех статусов перерывов");
            try
            {
                using var connection = CreateConnection();
                var sql = @"SELECT ""Id"", ""Status"" FROM ""BreakStatuses"" ORDER BY ""Status""";
                var result = (await connection.QueryAsync<BreakStatuses>(sql)).AsList();
                Log.Information($"{prefix} Получено {result.Count} статусов перерывов");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch break statuses");
                throw;
            }
        }
    }
}