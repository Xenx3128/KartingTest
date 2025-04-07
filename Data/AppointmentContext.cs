using Dapper;
using Npgsql;
using TestMVC.Models;
using TestMVC.Service;

namespace TestMVC.Data;


public class AppointmentContext{
    private readonly string connectionString;
    private NpgsqlConnection Conn { get; set; }

    public AppointmentContext(string connectionString)
    {
        this.connectionString = connectionString;
        this.Conn = new NpgsqlConnection(this.connectionString);
    }
    public async Task<IEnumerable<DateTime>> GetPlannedRaces(DateTime date)
    {
        var sql = " SELECT startDate from races where startDate::date = @Date::date ";
        var args = new {
            Date = date
        };
        var res = await Conn.QueryAsync<DateTime>(sql, args);
        return res;
    }

    public async Task<IEnumerable<DateTime>> GetTechBreaks(DateTime date)
    {
        var sql = " SELECT dateStart from technicalBreaks where dateStart::date = @Date::date ";
        var args = new {
            Date = date
        };
        var res = await Conn.QueryAsync<DateTime>(sql, args);
        return res;
    }

    public async void PostOrder(DateOnly date, List<TimeOnly> times, string raceTypeMode, List<string> modes){
        // Insert new order
        var orderSql =   """
                    Insert into orders (userId, orderDate, price, status)
                    Values (@UserId, @OrderDate, @Price, @Status)
                    Returning id;
                    """;
        
        var orderArgs = new {
            UserId = 1,
            OrderDate = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Ekaterinburg Standard Time")),
            Price = 0.0,
            Status = "New",
        };
        var orderId = await Conn.QuerySingleAsync<int>(orderSql, orderArgs);

        var racesSql =  """
                        Insert into races (orderId, startDate, finishDate, category, status)
                        Values (@OrderId, @StartDate, @FinishDate, @Category, @Status);
                        """;
        var racesArgs = new List<object>();
        for (int i = 0; i < times.Count; i++){
            var time = times[i];
            var startDate = date.ToDateTime(time);
            // REPLACE DURATION
            var finishDate = startDate.AddMinutes(15);
            if (raceTypeMode == "uniform"){
                racesArgs.Add(new {
                OrderId = orderId,
                StartDate = startDate,
                FinishDate = finishDate,
                Category = modes[0],
                Status = "Planned"
                });
            }
            else if (raceTypeMode == "divided") {
                racesArgs.Add(new {
                OrderId = orderId,
                StartDate = startDate,
                FinishDate = finishDate,
                Category = modes[i],
                Status = "Planned"
                });
            }

        } 

        await Conn.ExecuteAsync(racesSql, racesArgs);
    }
}
