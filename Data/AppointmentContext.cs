using Dapper;
using Npgsql;
using System.Data;
using TestMVC.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace TestMVC.Data
{
    public class AppointmentContext
    {
        private readonly string _connectionString;

        public AppointmentContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        public async Task<IEnumerable<DateTime>> GetPlannedRaces(DateTime date)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT ""StartDate"" FROM ""Races"" WHERE DATE(""StartDate"") = @Date";
            return await connection.QueryAsync<DateTime>(sql, new { Date = date.Date });
        }

        public async Task<IEnumerable<DateTime>> GetTechBreaks(DateTime date)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT ""DateStart"" FROM ""TechnicalBreaks"" WHERE DATE(""DateStart"") = @Date";
            return await connection.QueryAsync<DateTime>(sql, new { Date = date.Date });
        }

        public async Task<int> CreateOrderAsync(int userId, DateOnly date, List<TimeOnly> times, bool isUniform, List<int> raceCategoryIds)
        {
            using var connection = CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Get default OrderStatusId (e.g., "Pending")
                var orderStatusSql = @"SELECT ""Id"" FROM ""OrderStatuses"" WHERE ""Status"" = @Status LIMIT 1";
                var orderStatusId = await connection.QuerySingleOrDefaultAsync<int>(
                    orderStatusSql, new { Status = "Pending" }, transaction);

                if (orderStatusId == 0)
                {
                    throw new Exception("Order status 'Pending' not found.");
                }

                // Insert Order
                var orderSql = @"
                    INSERT INTO ""Orders"" (""UserId"", ""OrderDate"", ""Price"", ""OrderStatusId"")
                    VALUES (@UserId, @OrderDate, @Price, @OrderStatusId)
                    RETURNING ""Id""";
                var orderId = await connection.QuerySingleAsync<int>(
                    orderSql,
                    new
                    {
                        UserId = userId,
                        OrderDate = DateTime.Now, // Server time, adjust timezone if needed
                        Price = 0m,
                        OrderStatusId = orderStatusId
                    },
                    transaction);

                // Get default RaceStatusId (e.g., "Planned")
                var raceStatusSql = @"SELECT ""Id"" FROM ""RaceStatuses"" WHERE ""Status"" = @Status LIMIT 1";
                var raceStatusId = await connection.QuerySingleOrDefaultAsync<int>(
                    raceStatusSql, new { Status = "Planned" }, transaction);

                if (raceStatusId == 0)
                {
                    throw new Exception("Race status 'Planned' not found.");
                }

                // Insert Races
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
                return orderId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<RaceCategory>> GetRaceCategoriesAsync()
        {
            using var connection = CreateConnection();
            var sql = @"SELECT ""Id"", ""Category"" FROM ""RaceCategories"" ORDER BY ""Category""";
            return await connection.QueryAsync<RaceCategory>(sql);
        }
    }
}