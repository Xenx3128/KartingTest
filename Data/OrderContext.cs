using Dapper;
using Npgsql;
using System.Data;
using TestMVC.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

namespace TestMVC.Data
{
    public class OrderContext
    {
        private readonly string _connectionString;

        public OrderContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        // Get all orders with status and user details
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT o.""Id"", o.""UserId"", o.""OrderDate"", o.""Price"", o.""OrderStatusId"",
                       os.""Id"" AS Id, os.""Status"" AS Status,
                       u.""Id"" AS Id, u.""Email"" AS Email
                FROM ""Orders"" o
                LEFT JOIN ""OrderStatuses"" os ON o.""OrderStatusId"" = os.""Id""
                LEFT JOIN ""AspNetUsers"" u ON o.""UserId"" = u.""Id""
                ORDER BY o.""OrderDate"" DESC";
            var orders = await connection.QueryAsync<Order, OrderStatus, ApplicationUser, Order>(
                sql,
                (order, orderStatus, user) =>
                {
                    order.OrderStatus = orderStatus;
                    order.User = user;
                    if (orderStatus == null)
                        Debug.WriteLine($"Order {order.Id} has null OrderStatus (OrderStatusId: {order.OrderStatusId})");
                    if (user == null)
                        Debug.WriteLine($"Order {order.Id} has null User (UserId: {order.UserId})");
                    return order;
                },
                splitOn: "Id,Id");
            return orders;
        }

        // Get races by order ID
        public async Task<IEnumerable<Races>> GetRacesByOrderIdAsync(int orderId)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT r.""Id"", r.""OrderId"", r.""StartDate"", r.""FinishDate"", r.""RaceCategoryId"", r.""RaceStatusId"",
                       rc.""Id"" AS Id, rc.""Category"" AS Category,
                       rs.""Id"" AS Id, rs.""Status"" AS Status
                FROM ""Races"" r
                LEFT JOIN ""RaceCategories"" rc ON r.""RaceCategoryId"" = rc.""Id""
                LEFT JOIN ""RaceStatuses"" rs ON r.""RaceStatusId"" = rs.""Id""
                WHERE r.""OrderId"" = @OrderId
                ORDER BY r.""StartDate""";
            return await connection.QueryAsync<Races, RaceCategory, RaceStatus, Races>(
                sql,
                (race, raceCategory, raceStatus) =>
                {
                    race.RaceCategory = raceCategory;
                    race.RaceStatus = raceStatus;
                    if (raceCategory == null)
                        Debug.WriteLine($"Race {race.Id} has null RaceCategory (RaceCategoryId: {race.RaceCategoryId})");
                    if (raceStatus == null)
                        Debug.WriteLine($"Race {race.Id} has null RaceStatus (RaceStatusId: {race.RaceStatusId})");
                    return race;
                },
                new { OrderId = orderId },
                splitOn: "Id,Id");
        }

        // Get order by ID
        public async Task<Order> GetOrderByIdAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT o.""Id"", o.""UserId"", o.""OrderDate"", o.""Price"", o.""OrderStatusId"",
                       os.""Id"" AS Id, os.""Status"" AS Status,
                       u.""Id"" AS Id, u.""Email"" AS Email
                FROM ""Orders"" o
                LEFT JOIN ""OrderStatuses"" os ON o.""OrderStatusId"" = os.""Id""
                LEFT JOIN ""AspNetUsers"" u ON o.""UserId"" = u.""Id""
                WHERE o.""Id"" = @Id";
            var result = await connection.QueryAsync<Order, OrderStatus, ApplicationUser, Order>(
                sql,
                (order, orderStatus, user) =>
                {
                    order.OrderStatus = orderStatus;
                    order.User = user;
                    return order;
                },
                new { Id = id },
                splitOn: "Id,Id");
            return result.FirstOrDefault();
        }

        // Create a new order
        public async Task<int> CreateOrderAsync(Order order)
        {
            using var connection = CreateConnection();
            var sql = @"
                INSERT INTO ""Orders"" (""UserId"", ""OrderDate"", ""Price"", ""OrderStatusId"")
                VALUES (@UserId, @OrderDate, @Price, @OrderStatusId)
                RETURNING ""Id""";
            return await connection.ExecuteScalarAsync<int>(sql, order);
        }

        // Update an existing order
        public async Task UpdateOrderAsync(Order order)
        {
            using var connection = CreateConnection();
            var sql = @"
                UPDATE ""Orders""
                SET ""UserId"" = @UserId,
                    ""OrderDate"" = @OrderDate,
                    ""Price"" = @Price,
                    ""OrderStatusId"" = @OrderStatusId
                WHERE ""Id"" = @Id";
            await connection.ExecuteAsync(sql, order);
        }

        // Delete an order
        public async Task DeleteOrderAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = @"DELETE FROM ""Orders"" WHERE ""Id"" = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }

        // Get all order statuses
        public async Task<IEnumerable<OrderStatus>> GetAllOrderStatusesAsync()
        {
            using var connection = CreateConnection();
            var sql = @"SELECT * FROM ""OrderStatuses"" ORDER BY ""Status""";
            return await connection.QueryAsync<OrderStatus>(sql);
        }

        // Validate UserId exists
        public async Task<bool> UserExistsAsync(int userId)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT COUNT(1) FROM ""AspNetUsers"" WHERE ""Id"" = @UserId";
            return await connection.ExecuteScalarAsync<bool>(sql, new { UserId = userId });
        }

        // Get all users for dropdown
        public async Task<IEnumerable<UserDropdownModel>> GetUsersForDropdownAsync()
        {
            using var connection = CreateConnection();
            var sql = @"SELECT ""Id"", ""Email"" FROM ""AspNetUsers"" ORDER BY ""Email""";
            return await connection.QueryAsync<UserDropdownModel>(sql);
        }
    }
}