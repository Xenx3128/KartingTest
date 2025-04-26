using Dapper;
using Npgsql;
using TestMVC.Models;
using System.Data;

namespace TestMVC.Data;

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
        var sql = "SELECT startDate from races where startDate::date = @Date::date";
        return await connection.QueryAsync<DateTime>(sql, new { Date = date });
    }

    public async Task<IEnumerable<DateTime>> GetTechBreaks(DateTime date)
    {
        using var connection = CreateConnection();
        var sql = "SELECT dateStart from technicalBreaks where dateStart::date = @Date::date";
        return await connection.QueryAsync<DateTime>(sql, new { Date = date });
    }

    public async Task<int> PostOrder(DateOnly date, List<TimeOnly> times, string raceTypeMode, List<string> modes)
    {
        using var connection = CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        
        try
        {
            // Insert new order
            var orderSql = """
                        INSERT INTO orders (userId, orderDate, price, status)
                        VALUES (@UserId, @OrderDate, @Price, @Status)
                        RETURNING id;
                        """;
            
            var orderArgs = new {
                UserId = 1,
                OrderDate = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Ekaterinburg Standard Time")),
                Price = 0.0,
                Status = "New",
            };
            var orderId = await connection.QuerySingleAsync<int>(orderSql, orderArgs, transaction);

            var racesSql = """
                          INSERT INTO races (orderId, startDate, finishDate, category, status)
                          VALUES (@OrderId, @StartDate, @FinishDate, @Category, @Status);
                          """;
            var racesArgs = new List<object>();
            
            for (int i = 0; i < times.Count; i++)
            {
                var time = times[i];
                var startDate = date.ToDateTime(time);
                var finishDate = startDate.AddMinutes(15);
                var category = raceTypeMode == "uniform" ? modes[0] : modes[i];
                
                racesArgs.Add(new {
                    OrderId = orderId,
                    StartDate = startDate,
                    FinishDate = finishDate,
                    Category = category,
                    Status = "Planned"
                });
            } 

            await connection.ExecuteAsync(racesSql, racesArgs, transaction);
            transaction.Commit();
            return orderId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}