using Dapper;
using Npgsql;
using System.Data;
using TestMVC.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace TestMVC.Data
{
    public class SettingsContext
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SettingsContext(string connectionString, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = connectionString;
            _httpContextAccessor = httpContextAccessor;
            Log.Information($"{GetUserPrefix()} SettingsContext initialized with connection string.");
        }

        private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        private string GetUserPrefix()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
            {
                return $"[{user.Identity.Name ?? "UnknownEmail"}] [Admin]";
            }
            return "[Unknown]";
        }

        public async Task<IEnumerable<Settings>> GetAllSettingsAsync()
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Fetching all settings");
            try
            {
                using var connection = CreateConnection();
                var sql = @"SELECT ""Id"", ""DayStart"", ""DayFinish"", ""RaceDuration"", ""IsSelected"" FROM ""Settings""";
                var result = await connection.QueryAsync<Settings>(sql);
                Log.Information($"{prefix} Retrieved {result.Count()} settings");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch settings");
                throw;
            }
        }

        public async Task<Settings> GetSettingsByIdAsync(int id)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Fetching settings with ID {id}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"SELECT ""Id"", ""DayStart"", ""DayFinish"", ""RaceDuration"", ""IsSelected"" FROM ""Settings"" WHERE ""Id"" = @Id";
                var result = await connection.QuerySingleOrDefaultAsync<Settings>(sql, new { Id = id });
                if (result == null)
                {
                    Log.Warning($"{prefix} Settings with ID {id} not found");
                }
                else
                {
                    Log.Information($"{prefix} Retrieved settings with ID {id}");
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch settings with ID {id}");
                throw;
            }
        }

        public async Task<Settings> GetSelectedSettingsAsync()
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Fetching selected settings");
            try
            {
                using var connection = CreateConnection();
                var sql = @"SELECT ""Id"", ""DayStart"", ""DayFinish"", ""RaceDuration"", ""IsSelected"" FROM ""Settings"" WHERE ""IsSelected"" = TRUE LIMIT 1";
                var result = await connection.QuerySingleOrDefaultAsync<Settings>(sql);
                if (result == null)
                {
                    Log.Warning($"{prefix} No selected settings found");
                }
                else
                {
                    Log.Information($"{prefix} Retrieved selected settings with ID {result.Id}");
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch selected settings");
                throw;
            }
        }

        public async Task<int> CreateSettingsAsync(Settings settings)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Creating new settings");
            try
            {
                using var connection = CreateConnection();
                connection.Open();
                using var transaction = connection.BeginTransaction();

                if (settings.IsSelected)
                {
                    await connection.ExecuteAsync(
                        @"UPDATE ""Settings"" SET ""IsSelected"" = FALSE WHERE ""IsSelected"" = TRUE",
                        transaction: transaction);
                }

                var sql = @"
                    INSERT INTO ""Settings"" (""DayStart"", ""DayFinish"", ""RaceDuration"", ""IsSelected"")
                    VALUES (@DayStart, @DayFinish, @RaceDuration, @IsSelected)
                    RETURNING ""Id""";
                var id = await connection.QuerySingleAsync<int>(sql, settings, transaction);

                transaction.Commit();
                Log.Information($"{prefix} Created settings with ID {id}");
                return id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to create settings");
                throw;
            }
        }

        public async Task UpdateSettingsAsync(Settings settings)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Updating settings with ID {settings.Id}");
            try
            {
                using var connection = CreateConnection();
                connection.Open();
                using var transaction = connection.BeginTransaction();

                if (settings.IsSelected)
                {
                    await connection.ExecuteAsync(
                        @"UPDATE ""Settings"" SET ""IsSelected"" = FALSE WHERE ""IsSelected"" = TRUE AND ""Id"" != @Id",
                        new { Id = settings.Id },
                        transaction);
                }

                var sql = @"
                    UPDATE ""Settings""
                    SET ""DayStart"" = @DayStart, ""DayFinish"" = @DayFinish, ""RaceDuration"" = @RaceDuration, ""IsSelected"" = @IsSelected
                    WHERE ""Id"" = @Id";
                await connection.ExecuteAsync(sql, settings, transaction);

                transaction.Commit();
                Log.Information($"{prefix} Updated settings with ID {settings.Id}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to update settings with ID {settings.Id}");
                throw;
            }
        }

        public async Task SelectSettingsAsync(int id)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Selecting settings with ID {id}");
            try
            {
                using var connection = CreateConnection();
                connection.Open();
                using var transaction = connection.BeginTransaction();

                await connection.ExecuteAsync(
                    @"UPDATE ""Settings"" SET ""IsSelected"" = FALSE WHERE ""IsSelected"" = TRUE",
                    transaction: transaction);

                var sql = @"UPDATE ""Settings"" SET ""IsSelected"" = TRUE WHERE ""Id"" = @Id";
                await connection.ExecuteAsync(sql, new { Id = id }, transaction);

                transaction.Commit();
                Log.Information($"{prefix} Selected settings with ID {id}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to select settings with ID {id}");
                throw;
            }
        }

        public async Task DeleteSettingsAsync(int id)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Deleting settings with ID {id}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"DELETE FROM ""Settings"" WHERE ""Id"" = @Id";
                var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
                if (rowsAffected == 0)
                {
                    Log.Warning($"{prefix} Settings with ID {id} not found for deletion");
                }
                else
                {
                    Log.Information($"{prefix} Deleted settings with ID {id}");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to delete settings with ID {id}");
                throw;
            }
        }
    }
}