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
        var sql = " SELECT dateStart from races where startDate::date = @Date::date ";
        var args = new {
            Date = date
        };
        var res = await Conn.QueryAsync<DateTime>(sql, args);
        return res;
    }

    public async Task<IEnumerable<DateTime>> GetTechBreaks(DateTime date)
    {
        var sql = " SELECT dateStart from technicalBreaks where startDate::date = @Date::date ";
        var args = new {
            Date = date
        };
        var res = await Conn.QueryAsync<DateTime>(sql, args);
        return res;
    }

    /*public async Task<IEnumerable<AppointmentSlot>> GetOccupiedSlots(DateTime date)
    {
        
        var techBreaksSql = 
        var args = new {
            Date = date
        };
        
        var res2 = await Conn.QueryAsync<TechnicalBreaks>(racesSql, args);
        
        var result = new List<AppointmentSlot>();
        await using var cmd = DataSource.CreateCommand(
            
        );
        cmd.Parameters.Add(new NpgsqlParameter("", date));
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {   
                AppointmentSlot instance = new AppointmentSlot();
                instance.Id = Convert.ToInt32(reader["id"]);
                instance.SlotDate = DateOnly.FromDateTime(Convert.ToDateTime(reader["slotDate"]));
                instance.SlotStart = TimeOnly.Parse(reader["slotStart"].ToString());
                if (reader["orderId"] != DBNull.Value) instance.OrderId =  Convert.ToInt32(reader["orderId"]);
                if (reader["status"] != DBNull.Value) instance.Status = reader["status"].ToString();
                if (reader["raceType"] != DBNull.Value) instance.RaceType = reader["raceType"].ToString();
                result.Add(instance); // Adjust the index based on your data schema
            }
        }        
        return result;
    }*/
    public async void PostOrder(DateOnly date, List<TimeOnly> times, string raceTypeMode, List<string> modes){
        // Insert new order
        await using var cmd = DataSource.CreateCommand(
            """
            Insert into orders (orderDate, userId, price, status)
            Values ($1, $2, $3, $4)
            Returning id;
            """
        );
        var orderDate = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Ekaterinburg Standard Time"));
        var userId = 1;
        var price = 0.0;
        cmd.Parameters.Add(new NpgsqlParameter("", orderDate));
        cmd.Parameters.Add(new NpgsqlParameter("", userId));
        cmd.Parameters.Add(new NpgsqlParameter("", price));
        cmd.Parameters.Add(new NpgsqlParameter("", "test"));

        var orderId = await cmd.ExecuteScalarAsync();

        await using var idCmd = DataSource.CreateCommand(
            """
            Select id
            from orders
            where orderDate = $1 and userId = $2;
            """
        );
        idCmd.Parameters.Add(new NpgsqlParameter("", orderDate));
        idCmd.Parameters.Add(new NpgsqlParameter("", userId));

        //var orderId = await cmd.ExecuteScalarAsync();
        Console.WriteLine(orderId);
        for (int i = 0; i < times.Count; i++){
            await using var slotCmd = DataSource.CreateCommand(
                """
                Insert into slots (slotDate, slotStart, status, orderId, raceType)
                Values ($1, $2, $3, $4, $5);
                """
            );
            slotCmd.Parameters.Add(new NpgsqlParameter("", date));
            slotCmd.Parameters.Add(new NpgsqlParameter("", times[i]));
            slotCmd.Parameters.Add(new NpgsqlParameter("", "booked"));
            slotCmd.Parameters.Add(new NpgsqlParameter("", orderId));
            if (raceTypeMode == "uniform"){
                slotCmd.Parameters.Add(new NpgsqlParameter("", modes[0]));
            }
            else if (raceTypeMode == "divided") {
                slotCmd.Parameters.Add(new NpgsqlParameter("", modes[i]));
            }

            await slotCmd.ExecuteNonQueryAsync();
        }

        
    }
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public async Task<IList<AppointmentSlot>> GetSlots(DateTime start, DateTime end)
    {
        var result = new List<AppointmentSlot>();
        await using var cmd = DataSource.CreateCommand(
            """
            SELECT * from slots s
            where not (s.slotEnd <= $1 or s.slotStart >= $2)
            """
        );
        cmd.Parameters.Add(new NpgsqlParameter("", start));
        cmd.Parameters.Add(new NpgsqlParameter("", end));
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {   
                AppointmentSlot instance = new AppointmentSlot();
                instance.Id = Convert.ToInt32(reader["id"]);
                instance.SlotDate = DateOnly.FromDateTime(Convert.ToDateTime(reader["slotDate"]));
                instance.SlotStart = TimeOnly.Parse(reader["slotStart"].ToString()); 
                instance.OrderId =  Convert.ToInt32(reader["orderId"]);
                instance.Status = reader["status"].ToString();
                instance.RaceType = reader["raceType"].ToString();
                result.Add(instance); // Adjust the index based on your data schema
            }
        }        
        return result;
    }

    public async Task<IEnumerable<AppointmentSlot>> GetFreeSlots(DateTime start, DateTime end, string? patient)
    {
        var result = new List<AppointmentSlot>();
        await using var cmd = DataSource.CreateCommand(
            """
            SELECT * from slots s
            where not (s.slotEnd <= $1 or s.slotStart >= $2) and (s.status = 'free' or (s.status != 'free' and s.patientId = $3))
            """
        );
        cmd.Parameters.Add(new NpgsqlParameter("", start));
        cmd.Parameters.Add(new NpgsqlParameter("", end));
        cmd.Parameters.Add(new NpgsqlParameter("", patient));
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {   
                AppointmentSlot instance = new AppointmentSlot();
                instance.Id = Convert.ToInt32(reader["id"]);
                instance.SlotDate = DateOnly.FromDateTime(Convert.ToDateTime(reader["slotDate"]));
                instance.SlotStart = TimeOnly.Parse(reader["slotStart"].ToString()); 
                instance.OrderId =  Convert.ToInt32(reader["orderId"]);
                instance.Status = reader["status"].ToString();
                instance.RaceType = reader["raceType"].ToString();
                result.Add(instance); // Adjust the index based on your data schema
            }
        }   
        return result;
    }

    public async Task<AppointmentSlot?> GetAppointmentSlot(int id)
    {
        AppointmentSlot instance = new AppointmentSlot();
        await using var cmd = DataSource.CreateCommand(
            """
            SELECT * from slots s
            where id = $1
            """
            );
            cmd.Parameters.Add(new NpgsqlParameter("", id));
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {   
                instance.Id = Convert.ToInt32(reader["id"]);
                instance.SlotDate = DateOnly.FromDateTime(Convert.ToDateTime(reader["slotDate"]));
                instance.SlotStart = TimeOnly.Parse(reader["slotStart"].ToString()); 
                instance.OrderId =  Convert.ToInt32(reader["orderId"]);
                instance.Status = reader["status"].ToString();
                instance.RaceType = reader["raceType"].ToString();

            }
        }
        if (instance.Id != null){
            return instance;
        }
        return null;
        
    }

    public async void PutAppointmentSlot(int id, AppointmentSlotUpdate update){
        //using var appSlot = GetAppointmentSlot(id);
        var appSlot = await GetAppointmentSlot(id);
        if (appSlot == null){
            return;
        }
        await using var cmd = DataSource.CreateCommand(
            """
            Update slots 
            set slotStart = $1, slotEnd = $2, patientId = $3, status = $4
            where id = $5
            """
        );
        cmd.Parameters.Add(new NpgsqlParameter("", update.SlotStart));
        cmd.Parameters.Add(new NpgsqlParameter("", update.SlotEnd));
        if (update.PatientId != null){
            cmd.Parameters.Add(new NpgsqlParameter("", update.PatientId));
        }
        else{
            cmd.Parameters.Add(new NpgsqlParameter("", appSlot.OrderId));
        }
        if (update.Status != null){
            cmd.Parameters.Add(new NpgsqlParameter("", update.Status));
        }
        else{
            cmd.Parameters.Add(new NpgsqlParameter("", appSlot.Status));
        }
        await cmd.ExecuteNonQueryAsync();
    }

    public async void PostAppointmentSlots(AppointmentSlotRange range){
        
        var slots = Timeline.GenerateSlots(range.Start, range.End, range.Scale);
        
        foreach (var slot in slots){
            await using var cmd = DataSource.CreateCommand(
                """
                Insert into slots (slotStart, slotEnd, status)
                Values ($1, $2, $3);
                """
            );
            cmd.Parameters.Add(new NpgsqlParameter("", slot.SlotDate));
            cmd.Parameters.Add(new NpgsqlParameter("", slot.SlotStart));
            cmd.Parameters.Add(new NpgsqlParameter("", slot.Status));
            await cmd.ExecuteNonQueryAsync();
        }

        /*using (var writer = Conn.BeginBinaryImport(
        "copy slots from STDIN (FORMAT BINARY)")){
            foreach (var slot in slots)
            {
                writer.StartRow();
                writer.Write(slot.SlotStart, NpgsqlTypes.NpgsqlDbType.Timestamp);
                writer.Write(slot.SlotEnd, NpgsqlTypes.NpgsqlDbType.Timestamp);
                writer.Write(slot.Status, NpgsqlTypes.NpgsqlDbType.Varchar);
            }

            writer.Complete();
        }*/

    }

    public async void DeleteAppointmentSlot(int id){
        await using var cmd = DataSource.CreateCommand(
            """
            Delete from slots
            where id = $1
            """
        );
        cmd.Parameters.Add(new NpgsqlParameter("", id));
        await cmd.ExecuteNonQueryAsync();
    }
}
