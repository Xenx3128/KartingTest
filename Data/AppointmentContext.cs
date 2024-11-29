using Npgsql;
using TestMVC.Models;
using TestMVC.Service;
namespace TestMVC.Data;


public class AppointmentSlotLayer{
    private readonly string connectionString;
    private NpgsqlDataSource DataSource { get; set; }
    private NpgsqlConnection Conn { get; set; }

    public AppointmentSlotLayer(string connectionString)
    {
        this.connectionString = connectionString;
        this.DataSource = NpgsqlDataSource.Create(connectionString);
        this.Conn = new NpgsqlConnection(this.connectionString);
    }

    public async void CreateSlots(){
        await using var cmd = DataSource.CreateCommand(
            """
            create table slots(
                id integer primary key,
                slotStart timestamp,
                slotEnd timestamp,
                patientId integer,
                status varchar 
            );
            """
        );
    }

    public async Task<IEnumerable<AppointmentSlot>> GetSlots(DateTime start, DateTime end)
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
                instance.SlotStart = Convert.ToDateTime(reader["slotStart"]);
                instance.SlotEnd = Convert.ToDateTime(reader["slotEnd"]);
                instance.PatientId = reader["patientId"].ToString();
                instance.Status = reader["status"].ToString();
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
                instance.SlotStart = Convert.ToDateTime(reader["start"]);
                instance.SlotEnd = Convert.ToDateTime(reader["slotEnd"]);
                instance.PatientId = reader["patientId"].ToString();
                instance.Status = reader["status"].ToString();
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
                instance.SlotStart = Convert.ToDateTime(reader["slotStart"]);
                instance.SlotEnd = Convert.ToDateTime(reader["slotEnd"]);
                instance.PatientId = reader["patientId"].ToString();
                instance.Status = reader["status"].ToString();
                
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
            cmd.Parameters.Add(new NpgsqlParameter("", appSlot.PatientId));
        }
        if (update.Status != null){
            cmd.Parameters.Add(new NpgsqlParameter("", update.Status));
        }
        else{
            cmd.Parameters.Add(new NpgsqlParameter("", appSlot.Status));
        }
        await cmd.ExecuteNonQueryAsync();
    }

    public async void PostAppointmentSlot(AppointmentSlot slot){
        await using var cmd = DataSource.CreateCommand(
            """
            Insert into slots (id, slotStart, slotEnd, patientId, status)
            Values ($1, $2, $3, $4, $5);
            """
        );
        cmd.Parameters.Add(new NpgsqlParameter("", slot.Id));
        cmd.Parameters.Add(new NpgsqlParameter("", slot.SlotStart));
        cmd.Parameters.Add(new NpgsqlParameter("", slot.SlotEnd));
        cmd.Parameters.Add(new NpgsqlParameter("", slot.PatientId));
        cmd.Parameters.Add(new NpgsqlParameter("", slot.Status));
        await cmd.ExecuteNonQueryAsync();
    }
    public async void PostAppointmentSlots(AppointmentSlotRange range){
        
        var slots = Timeline.GenerateSlots(range.Start, range.End, range.Scale);
        
        foreach (var slot in slots){
            await using var cmd = DataSource.CreateCommand(
                """
                Insert into slots (slotStart, slotEnd, patientId)
                Values ($1, $2, $3);
                """
            );
            cmd.Parameters.Add(new NpgsqlParameter("", slot.SlotStart));
            cmd.Parameters.Add(new NpgsqlParameter("", slot.SlotEnd));
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
