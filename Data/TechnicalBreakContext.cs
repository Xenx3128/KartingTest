using Dapper;
using Npgsql;
using System.Data;
using TestMVC.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

namespace TestMVC.Data
{
    public class BreakContext
    {
        private readonly string _connectionString;

        public BreakContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        // Get all technical breaks with BreakStatus
        public async Task<IEnumerable<TechnicalBreaks>> GetAllBreaksAsync()
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
                        Debug.WriteLine($"TechnicalBreak {breakItem.Id} has null BreakStatus (BreakStatusId: {breakItem.BreakStatusId})");
                    return breakItem;
                },
                splitOn: "Id");
            return breaks;
        }

        // Get a technical break by ID
        public async Task<TechnicalBreaks> GetBreakByIdAsync(int id)
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
                        Debug.WriteLine($"TechnicalBreak {breakItem.Id} has null BreakStatus (BreakStatusId: {breakItem.BreakStatusId})");
                    return breakItem;
                },
                new { Id = id },
                splitOn: "Id");
            return result.FirstOrDefault();
        }

        // Create a new technical break
        public async Task<int> CreateBreakAsync(TechnicalBreaks breakItem)
        {
            using var connection = CreateConnection();
            var sql = @"
                INSERT INTO ""TechnicalBreaks"" (""DateStart"", ""DateFinish"", ""Desc"", ""BreakStatusId"")
                VALUES (@DateStart, @DateFinish, @Desc, @BreakStatusId)
                RETURNING ""Id""";
            return await connection.ExecuteScalarAsync<int>(sql, breakItem);
        }

        // Update an existing technical break
        public async Task UpdateBreakAsync(TechnicalBreaks breakItem)
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
                Debug.WriteLine($"TechnicalBreak {breakItem.Id} not found for update.");
                throw new KeyNotFoundException($"Technical break with ID {breakItem.Id} not found.");
            }
        }

        // Delete a technical break
        public async Task DeleteBreakAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = @"DELETE FROM ""TechnicalBreaks"" WHERE ""Id"" = @Id";
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
            if (affectedRows == 0)
            {
                Debug.WriteLine($"TechnicalBreak {id} not found for deletion.");
                throw new KeyNotFoundException($"Technical break with ID {id} not found.");
            }
        }

        // Get all break statuses
        public async Task<List<BreakStatuses>> GetBreakStatusesAsync()
        {
            using var connection = CreateConnection();
            var sql = @"SELECT ""Id"", ""Status"" FROM ""BreakStatuses"" ORDER BY ""Status""";
            return (await connection.QueryAsync<BreakStatuses>(sql)).AsList();
        }
    }
}