using Dapper;
using Npgsql;
using System.Data;
using TestMVC.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TestMVC.Data
{
    public class RaceContext
    {
        private readonly string _connectionString;

        public RaceContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        // Get all races with order, user, category, and status details
        public async Task<IEnumerable<Races>> GetAllRacesAsync()
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT r.""Id"", r.""OrderId"", r.""StartDate"", r.""FinishDate"", r.""RaceCategoryId"", r.""RaceStatusId"",
                       o.""Id"" AS Id, o.""UserId"" AS UserId,
                       u.""Id"" AS Id, u.""Email"" AS Email,
                       rc.""Id"" AS Id, rc.""Category"" AS Category,
                       rs.""Id"" AS Id, rs.""Status"" AS Status
                FROM ""Races"" r
                LEFT JOIN ""Orders"" o ON r.""OrderId"" = o.""Id""
                LEFT JOIN ""AspNetUsers"" u ON o.""UserId"" = u.""Id""
                LEFT JOIN ""RaceCategories"" rc ON r.""RaceCategoryId"" = rc.""Id""
                LEFT JOIN ""RaceStatuses"" rs ON r.""RaceStatusId"" = rs.""Id""
                ORDER BY r.""StartDate"" DESC";
            var races = await connection.QueryAsync<Races, Order, ApplicationUser, RaceCategory, RaceStatus, Races>(
                sql,
                (race, order, user, raceCategory, raceStatus) =>
                {
                    race.Order = order;
                    if (order != null)
                    {
                        order.User = user;
                    }
                    race.RaceCategory = raceCategory;
                    race.RaceStatus = raceStatus;
                    if (order == null)
                        Debug.WriteLine($"Race {race.Id} has null Order (OrderId: {race.OrderId})");
                    if (user == null && order != null)
                        Debug.WriteLine($"Order {order.Id} has null User (UserId: {order.UserId})");
                    if (raceCategory == null)
                        Debug.WriteLine($"Race {race.Id} has null RaceCategory (RaceCategoryId: {race.RaceCategoryId})");
                    if (raceStatus == null)
                        Debug.WriteLine($"Race {race.Id} has null RaceStatus (RaceStatusId: {race.RaceStatusId})");
                    return race;
                },
                splitOn: "Id,Id,Id,Id");
            return races;
        }

        // Get races by status
        public async Task<IEnumerable<Races>> GetRacesByStatusAsync(string status)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT r.""Id"", r.""OrderId"", r.""StartDate"", r.""FinishDate"", r.""RaceCategoryId"", r.""RaceStatusId"",
                       o.""Id"" AS Id, o.""UserId"" AS UserId,
                       u.""Id"" AS Id, u.""Email"" AS Email,
                       rc.""Id"" AS Id, rc.""Category"" AS Category,
                       rs.""Id"" AS Id, rs.""Status"" AS Status
                FROM ""Races"" r
                LEFT JOIN ""Orders"" o ON r.""OrderId"" = o.""Id""
                LEFT JOIN ""AspNetUsers"" u ON o.""UserId"" = u.""Id""
                LEFT JOIN ""RaceCategories"" rc ON r.""RaceCategoryId"" = rc.""Id""
                LEFT JOIN ""RaceStatuses"" rs ON r.""RaceStatusId"" = rs.""Id""
                WHERE rs.""Status"" = @Status
                ORDER BY r.""StartDate"" DESC";
            var races = await connection.QueryAsync<Races, Order, ApplicationUser, RaceCategory, RaceStatus, Races>(
                sql,
                (race, order, user, raceCategory, raceStatus) =>
                {
                    race.Order = order;
                    if (order != null)
                    {
                        order.User = user;
                    }
                    race.RaceCategory = raceCategory;
                    race.RaceStatus = raceStatus;
                    if (order == null)
                        Debug.WriteLine($"Race {race.Id} has null Order (OrderId: {race.OrderId})");
                    if (user == null && order != null)
                        Debug.WriteLine($"Order {order.Id} has null User (UserId: {order.UserId})");
                    if (raceCategory == null)
                        Debug.WriteLine($"Race {race.Id} has null RaceCategory (RaceCategoryId: {race.RaceCategoryId})");
                    if (raceStatus == null)
                        Debug.WriteLine($"Race {race.Id} has null RaceStatus (RaceStatusId: {race.RaceStatusId})");
                    return race;
                },
                new { Status = status },
                splitOn: "Id,Id,Id,Id");
            return races;
        }

        // Get race by ID
        public async Task<Races> GetRaceByIdAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT r.""Id"", r.""OrderId"", r.""StartDate"", r.""FinishDate"", r.""RaceCategoryId"", r.""RaceStatusId"",
                       o.""Id"" AS Id, o.""UserId"" AS UserId,
                       u.""Id"" AS Id, u.""Email"" AS Email,
                       rc.""Id"" AS Id, rc.""Category"" AS Category,
                       rs.""Id"" AS Id, rs.""Status"" AS Status
                FROM ""Races"" r
                LEFT JOIN ""Orders"" o ON r.""OrderId"" = o.""Id""
                LEFT JOIN ""AspNetUsers"" u ON o.""UserId"" = u.""Id""
                LEFT JOIN ""RaceCategories"" rc ON r.""RaceCategoryId"" = rc.""Id""
                LEFT JOIN ""RaceStatuses"" rs ON r.""RaceStatusId"" = rs.""Id""
                WHERE r.""Id"" = @Id";
            var result = await connection.QueryAsync<Races, Order, ApplicationUser, RaceCategory, RaceStatus, Races>(
                sql,
                (race, order, user, raceCategory, raceStatus) =>
                {
                    race.Order = order;
                    if (order != null)
                    {
                        order.User = user;
                    }
                    race.RaceCategory = raceCategory;
                    race.RaceStatus = raceStatus;
                    return race;
                },
                new { Id = id },
                splitOn: "Id,Id,Id,Id");
            return result.FirstOrDefault();
        }

        // Create a new race
        public async Task<int> CreateRaceAsync(Races race)
        {
            using var connection = CreateConnection();
            var sql = @"
                INSERT INTO ""Races"" (""OrderId"", ""StartDate"", ""FinishDate"", ""RaceCategoryId"", ""RaceStatusId"")
                VALUES (@OrderId, @StartDate, @FinishDate, @RaceCategoryId, @RaceStatusId)
                RETURNING ""Id""";
            return await connection.ExecuteScalarAsync<int>(sql, race);
        }

        // Update an existing race
        public async Task UpdateRaceAsync(Races race)
        {
            using var connection = CreateConnection();
            var sql = @"
                UPDATE ""Races""
                SET ""OrderId"" = @OrderId,
                    ""StartDate"" = @StartDate,
                    ""FinishDate"" = @FinishDate,
                    ""RaceCategoryId"" = @RaceCategoryId,
                    ""RaceStatusId"" = @RaceStatusId
                WHERE ""Id"" = @Id";
            await connection.ExecuteAsync(sql, race);
        }

        // Delete a race
        public async Task DeleteRaceAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = @"DELETE FROM ""Races"" WHERE ""Id"" = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }

        // Get all orders for dropdown
        public async Task<IEnumerable<OrderDropdownModel>> GetOrdersForDropdownAsync()
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT o.""Id"", u.""Email""
                FROM ""Orders"" o
                LEFT JOIN ""AspNetUsers"" u ON o.""UserId"" = u.""Id""
                ORDER BY o.""Id""";
            return await connection.QueryAsync<OrderDropdownModel>(sql);
        }

        // Get all race categories
        public async Task<IEnumerable<RaceCategory>> GetRaceCategoriesAsync()
        {
            using var connection = CreateConnection();
            var sql = @"SELECT * FROM ""RaceCategories"" ORDER BY ""Category""";
            return await connection.QueryAsync<RaceCategory>(sql);
        }

        // Get all race statuses
        public async Task<IEnumerable<RaceStatus>> GetRaceStatusesAsync()
        {
            using var connection = CreateConnection();
            var sql = @"SELECT * FROM ""RaceStatuses"" ORDER BY ""Status""";
            return await connection.QueryAsync<RaceStatus>(sql);
        }

        // Validate OrderId exists
        public async Task<bool> OrderExistsAsync(int orderId)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT COUNT(1) FROM ""Orders"" WHERE ""Id"" = @OrderId";
            return await connection.ExecuteScalarAsync<bool>(sql, new { OrderId = orderId });
        }

        // Get all user races by race ID
        public async Task<IEnumerable<UserRace>> GetUserRacesByRaceIdAsync(int raceId)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT ur.""Id"", ur.""UserId"", ur.""RaceId"", ur.""CartId"", ur.""Position"",
                       u.""Id"" AS Id, u.""FullName"" AS FullName, u.""Email"" AS Email,
                       r.""Id"" AS Id,
                       c.""Id"" AS Id, c.""Name"" AS Name
                FROM ""UserRaces"" ur
                LEFT JOIN ""AspNetUsers"" u ON ur.""UserId"" = u.""Id""
                LEFT JOIN ""Races"" r ON ur.""RaceId"" = r.""Id""
                LEFT JOIN ""Carts"" c ON ur.""CartId"" = c.""Id""
                WHERE ur.""RaceId"" = @RaceId
                ORDER BY ur.""Id""";
            var userRaces = await connection.QueryAsync<UserRace, ApplicationUser, Races, Cart, UserRace>(
                sql,
                (userRace, user, race, cart) =>
                {
                    userRace.User = user;
                    userRace.Race = race;
                    userRace.Cart = cart;
                    return userRace;
                },
                new { RaceId = raceId },
                splitOn: "Id,Id,Id");
            return userRaces;
        }

        // Get user race by ID
        public async Task<UserRace> GetUserRaceByIdAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT ur.""Id"", ur.""UserId"", ur.""RaceId"", ur.""CartId"", ur.""Position"",
                       u.""Id"" AS Id, u.""FullName"" AS FullName, u.""Email"" AS Email,
                       r.""Id"" AS Id,
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
                    return userRace;
                },
                new { Id = id },
                splitOn: "Id,Id,Id");
            return result.FirstOrDefault();
        }

        // Create a new user race
        public async Task CreateUserRaceAsync(UserRace userRace)
        {
            using var connection = CreateConnection();
            var sql = @"
                INSERT INTO ""UserRaces"" (""UserId"", ""RaceId"", ""CartId"", ""Position"")
                VALUES (@UserId, @RaceId, @CartId, @Position)
                RETURNING ""Id""";
            userRace.Id = await connection.ExecuteScalarAsync<int>(sql, userRace);
        }

        // Update an existing user race
        public async Task UpdateUserRaceAsync(UserRace userRace)
        {
            using var connection = CreateConnection();
            var sql = @"
                UPDATE ""UserRaces""
                SET ""UserId"" = @UserId,
                    ""RaceId"" = @RaceId,
                    ""CartId"" = @CartId,
                    ""Position"" = @Position
                WHERE ""Id"" = @Id";
            await connection.ExecuteAsync(sql, userRace);
        }

        // Delete a user race
        public async Task DeleteUserRaceAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = @"DELETE FROM ""UserRaces"" WHERE ""Id"" = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }

        // Get all users
        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            using var connection = CreateConnection();
            var sql = @"SELECT ""Id"", ""FullName"", ""Email"" FROM ""AspNetUsers"" ORDER BY ""FullName""";
            return await connection.QueryAsync<ApplicationUser>(sql);
        }

        // Get all carts
        public async Task<IEnumerable<Cart>> GetAllCartsAsync()
        {
            using var connection = CreateConnection();
            var sql = @"SELECT ""Id"", ""Name"" FROM ""Carts"" ORDER BY ""Name""";
            return await connection.QueryAsync<Cart>(sql);
        }
    }

    // Model for order dropdown
    public class OrderDropdownModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
    }
}