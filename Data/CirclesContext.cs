using Dapper;
using Npgsql;
using System.Data;
using TestMVC.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

namespace TestMVC.Data
{
    public class CircleResultsContext
    {
        private readonly string _connectionString;

        public CircleResultsContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        // Get all circle results by UserRaceId
        public async Task<IEnumerable<CircleResults>> GetCircleResultsByUserRaceIdAsync(int userRaceId)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT cr.""Id"", cr.""UserRaceId"", cr.""CircleNum"", cr.""CircleTime"",
                       ur.""Id"" AS Id, ur.""UserId"", ur.""RaceId"", ur.""CartId"", ur.""Position""
                FROM ""CircleResults"" cr
                LEFT JOIN ""UserRaces"" ur ON cr.""UserRaceId"" = ur.""Id""
                WHERE cr.""UserRaceId"" = @UserRaceId
                ORDER BY cr.""CircleNum""";
            var results = await connection.QueryAsync<CircleResults, UserRace, CircleResults>(
                sql,
                (circleResult, userRace) =>
                {
                    circleResult.UserRace = userRace;
                    if (userRace == null)
                        Debug.WriteLine($"CircleResult {circleResult.Id} has null UserRace (UserRaceId: {circleResult.UserRaceId})");
                    return circleResult;
                },
                new { UserRaceId = userRaceId },
                splitOn: "Id");
            return results;
        }

        // Get a circle result by ID
        public async Task<CircleResults> GetCircleResultByIdAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT cr.""Id"", cr.""UserRaceId"", cr.""CircleNum"", cr.""CircleTime"",
                       ur.""Id"" AS Id, ur.""UserId"", ur.""RaceId"", ur.""CartId"", ur.""Position""
                FROM ""CircleResults"" cr
                LEFT JOIN ""UserRaces"" ur ON cr.""UserRaceId"" = ur.""Id""
                WHERE cr.""Id"" = @Id";
            var result = await connection.QueryAsync<CircleResults, UserRace, CircleResults>(
                sql,
                (circleResult, userRace) =>
                {
                    circleResult.UserRace = userRace;
                    if (userRace == null)
                        Debug.WriteLine($"CircleResult {circleResult.Id} has null UserRace (UserRaceId: {circleResult.UserRaceId})");
                    return circleResult;
                },
                new { Id = id },
                splitOn: "Id");
            return result.FirstOrDefault();
        }

        // Create a new circle result
        public async Task<int> CreateCircleResultAsync(CircleResults circleResult)
        {
            using var connection = CreateConnection();
            var sql = @"
                INSERT INTO ""CircleResults"" (""UserRaceId"", ""CircleNum"", ""CircleTime"")
                VALUES (@UserRaceId, @CircleNum, @CircleTime)
                RETURNING ""Id""";
            return await connection.ExecuteScalarAsync<int>(sql, circleResult);
        }

        // Update an existing circle result
        public async Task UpdateCircleResultAsync(CircleResults circleResult)
        {
            using var connection = CreateConnection();
            var sql = @"
                UPDATE ""CircleResults""
                SET ""UserRaceId"" = @UserRaceId,
                    ""CircleNum"" = @CircleNum,
                    ""CircleTime"" = @CircleTime
                WHERE ""Id"" = @Id";
            var affectedRows = await connection.ExecuteAsync(sql, circleResult);
            if (affectedRows == 0)
            {
                Debug.WriteLine($"CircleResult {circleResult.Id} not found for update.");
                throw new KeyNotFoundException($"Circle result with ID {circleResult.Id} not found.");
            }
        }

        // Delete a circle result
        public async Task DeleteCircleResultAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = @"DELETE FROM ""CircleResults"" WHERE ""Id"" = @Id";
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
            if (affectedRows == 0)
            {
                Debug.WriteLine($"CircleResult {id} not found for deletion.");
                throw new KeyNotFoundException($"Circle result with ID {id} not found.");
            }
        }

        // Get a UserRace by ID
        public async Task<UserRace> GetUserRaceByIdAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT ur.""Id"", ur.""UserId"", ur.""RaceId"", ur.""CartId"", ur.""Position"",
                       u.""Id"" AS Id, u.""Email"" AS Email, u.""FullName"" AS FullName,
                       r.""Id"" AS Id, r.""StartDate"", r.""FinishDate"", r.""RaceCategoryId"", r.""RaceStatusId"",
                       c.""Id"" AS Id, c.""Name"" AS Name
                FROM ""UserRaces"" ur
                LEFT JOIN ""AspNetUsers"" u ON ur.""UserId"" = u.""Id""
                LEFT JOIN ""Races"" r ON ur.""RaceId"" = r.""Id""
                LEFT JOIN ""Carts"" c ON ur.""CartId"" = c.""Id""
                WHERE ur.""Id"" = @Id";
            var result = await connection.QueryAsync<UserRace, ApplicationUser, Races, Cart, UserRace>(
                sql,
                (userRace, user, race, cart) =>
                {
                    userRace.User = user;
                    userRace.Race = race;
                    userRace.Cart = cart;
                    if (user == null)
                        Debug.WriteLine($"UserRace {userRace.Id} has null User (UserId: {userRace.UserId})");
                    if (race == null)
                        Debug.WriteLine($"UserRace {userRace.Id} has null Race (RaceId: {userRace.RaceId})");
                    if (cart == null && userRace.CartId.HasValue)
                        Debug.WriteLine($"UserRace {userRace.Id} has null Cart (CartId: {userRace.CartId})");
                    return userRace;
                },
                new { Id = id },
                splitOn: "Id,Id,Id");
            return result.FirstOrDefault();
        }
    }
}