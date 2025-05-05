using Dapper;
using Npgsql;
using System.Data;
using TestMVC.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Serilog;

namespace TestMVC.Data
{
    public class AppointmentContext
    {
        private readonly string _connectionString;

        public AppointmentContext(string connectionString)
        {
            _connectionString = connectionString;
            Log.Information("AppointmentContext initialized with connection string.");
        }

        private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        public async Task<IEnumerable<DateTime>> GetPlannedRaces(DateTime date)
        {
            Log.Information("Fetching planned races for date {Date}", date.Date);
            try
            {
                using var connection = CreateConnection();
                var sql = @"SELECT ""StartDate"" FROM ""Races"" WHERE DATE(""StartDate"") = @Date";
                var result = await connection.QueryAsync<DateTime>(sql, new { Date = date.Date });
                Log.Information("Retrieved {Count} planned races for date {Date}", result.Count(), date.Date);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to fetch planned races for date {Date}", date.Date);
                throw;
            }
        }

        public async Task<IEnumerable<DateTime>> GetTechBreaks(DateTime date)
        {
            Log.Information("Fetching technical breaks for date {Date}", date.Date);
            try
            {
                using var connection = CreateConnection();
                var sql = @"SELECT ""DateStart"" FROM ""TechnicalBreaks"" WHERE DATE(""DateStart"") = @Date";
                var result = await connection.QueryAsync<DateTime>(sql, new { Date = date.Date });
                Log.Information("Retrieved {Count} technical breaks for date {Date}", result.Count(), date.Date);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to fetch technical breaks for date {Date}", date.Date);
                throw;
            }
        }

        public async Task<int> CreateOrderAsync(int userId, DateOnly date, List<TimeOnly> times, bool isUniform, List<int> raceCategoryIds)
        {
            Log.Information("Creating order for user {UserId} on date {Date} with {TimeCount} times", userId, date, times.Count);
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
                    Log.Error("Order status 'Pending' not found for user {UserId}", userId);
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
                    Log.Error("Race status 'Planned' not found for order {OrderId", orderId);
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
                Log.Information("Order {OrderId} created successfully for user {UserId}", orderId, userId);
                return orderId;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Log.Error(ex, "Failed to create order for user {UserId} on date {Date}", userId, date);
                throw;
            }
        }

        public async Task<IEnumerable<RaceCategory>> GetRaceCategoriesAsync()
        {
            Log.Information("Fetching all race categories");
            try
            {
                using var connection = CreateConnection();
                var sql = @"SELECT ""Id"", ""Category"" FROM ""RaceCategories"" ORDER BY ""Category""";
                var result = await connection.QueryAsync<RaceCategory>(sql);
                Log.Information("Retrieved {Count} race categories", result.Count());
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to fetch race categories");
                throw;
            }
        }
    }
}