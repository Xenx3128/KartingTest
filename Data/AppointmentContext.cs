using Dapper;
using Npgsql;
using System.Data;
using TestMVC.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Serilog;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading;

namespace TestMVC.Data
{
    public class AppointmentContext
    {
        private readonly string _connectionString;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppointmentContext(string connectionString, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = connectionString;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            Log.Information($"{GetUserPrefix()} AppointmentContext инициализирован со строкой подключения.");
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

        public async Task<IEnumerable<DateTime>> GetPlannedRaces(DateTime date)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение запланированных гонок на дату {date.Date}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"SELECT ""StartDate"" FROM ""Races"" WHERE DATE(""StartDate"") = @Date";
                var result = await connection.QueryAsync<DateTime>(sql, new { Date = date.Date });
                Log.Information($"{prefix} Получено {result.Count()} запланированных гонок на дату {date.Date}");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch planned races for date {date.Date}");
                throw;
            }
        }

        public async Task<IEnumerable<DateTime>> GetTechBreaks(DateTime date)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение технических перерывов на дату {date.Date}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"SELECT ""DateStart"" FROM ""TechnicalBreaks"" WHERE DATE(""DateStart"") = @Date";
                var result = await connection.QueryAsync<DateTime>(sql, new { Date = date.Date });
                Log.Information($"{prefix} Получено {result.Count()} технических перерывов на дату {date.Date}");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch technical breaks for date {date.Date}");
                throw;
            }
        }

        public async Task<int> CreateOrderAsync(int userId, DateOnly date, List<TimeOnly> times, bool isUniform, List<int> raceCategoryIds)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Создание заказа для пользователя {userId} на дату {date} с {times.Count} временами");
            using var connection = CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var orderStatusSql = @"SELECT ""Id"" FROM ""OrderStatuses"" WHERE ""Status"" = @Status LIMIT 1";
                var orderStatusId = await connection.QuerySingleOrDefaultAsync<int>(
                    orderStatusSql, new { Status = "Pending" }, transaction);

                if (orderStatusId == 0)
                {
                    Log.Error($"{prefix} Order status 'Pending' not found for user {userId}");
                    throw new Exception("Order status 'Pending' not found.");
                }

                var orderSql = @"
                    INSERT INTO ""Orders"" (""UserId"", ""OrderDate"", ""Price"", ""OrderStatusId"")
                    VALUES (@UserId, @OrderDate, @Price, @OrderStatusId)
                    RETURNING ""Id""";
                var orderId = await connection.QuerySingleAsync<int>(
                    orderSql,
                    new
                    {
                        UserId = userId,
                        OrderDate = DateTime.Now,
                        Price = 0m,
                        OrderStatusId = orderStatusId
                    },
                    transaction);

                var raceStatusSql = @"SELECT ""Id"" FROM ""RaceStatuses"" WHERE ""Status"" = @Status LIMIT 1";
                var raceStatusId = await connection.QuerySingleOrDefaultAsync<int>(
                    raceStatusSql, new { Status = "Planned" }, transaction);

                if (raceStatusId == 0)
                {
                    Log.Error($"{prefix} Race status 'Planned' not found for order {orderId}");
                    throw new Exception("Race status 'Planned' not found.");
                }

                var raceSql = @"
                    INSERT INTO ""Races"" (""OrderId"", ""StartDate"", ""FinishDate"", ""RaceCategoryId"", ""RaceStatusId"")
                    VALUES (@OrderId, @StartDate, @FinishDate, @RaceCategoryId, @RaceStatusId)";
                var raceParams = new List<object>();

                for (int i = 0; i < times.Count; i++)
                {
                    var startDate = date.ToDateTime(times[i]);
                    var finishDate = startDate.AddMinutes(15);
                    var categoryId = isUniform ? raceCategoryIds[0] : raceCategoryIds[i];

                    raceParams.Add(new
                    {
                        OrderId = orderId,
                        StartDate = startDate,
                        FinishDate = finishDate,
                        RaceCategoryId = categoryId,
                        RaceStatusId = raceStatusId
                    });
                }

                await connection.ExecuteAsync(raceSql, raceParams, transaction);
                transaction.Commit();
                Log.Information($"{prefix} Заказ {orderId} успешно создан для пользователя {userId}");
                return orderId;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Log.Error(ex, $"{prefix} Failed to create order for user {userId} on date {date}");
                throw

;
            }
        }

        public async Task<IEnumerable<RaceCategory>> GetRaceCategoriesAsync()
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение всех категорий гонок");
            try
            {
                using var connection = CreateConnection();
                var sql = @"SELECT ""Id"", ""Category"" FROM ""RaceCategories"" ORDER BY ""Category""";
                var result = await connection.QueryAsync<RaceCategory>(sql);
                Log.Information($"{prefix} Получено {result.Count()} категорий гонок");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch race categories");
                throw;
            }
        }
    }
}