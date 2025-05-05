using Dapper;
using Npgsql;
using System.Data;
using TestMVC.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace TestMVC.Data
{
    public class OrderContext
    {
        private readonly string _connectionString;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderContext(string connectionString, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = connectionString;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            Log.Information($"{GetUserPrefix()} OrderContext инициализирован со строкой подключения.");
        }

        private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        private string GetUserPrefix()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(user);
                var applicationUser = _userManager.FindByIdAsync(userId).GetAwaiter().GetResult();
                var roles = _userManager.GetRolesAsync(applicationUser).GetAwaiter().GetResult();
                var role = roles.FirstOrDefault() ?? "NoRole";
                return $"[{applicationUser?.Email ?? "UnknownEmail"}] [{role}]";
            }
            return "[Unknown]";
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение всех заказов");
            try
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
                            Log.Warning($"{prefix} Order {order.Id} has null OrderStatus (OrderStatusId: {order.OrderStatusId})");
                        if (user == null)
                            Log.Warning($"{prefix} Order {order.Id} has null User (UserId: {order.UserId})");
                        return order;
                    },
                    splitOn: "Id,Id");
                Log.Information($"{prefix} Получено {orders.Count()} заказов");
                return orders;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch all orders");
                throw;
            }
        }

        public async Task<IEnumerable<Races>> GetRacesByOrderIdAsync(int orderId)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение гонок для OrderId {orderId}");
            try
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
                var races = await connection.QueryAsync<Races, RaceCategory, RaceStatus, Races>(
                    sql,
                    (race, raceCategory, raceStatus) =>
                    {
                        race.RaceCategory = raceCategory;
                        race.RaceStatus = raceStatus;
                        if (raceCategory == null)
                            Log.Warning($"{prefix} Race {race.Id} has null RaceCategory (RaceCategoryId: {race.RaceCategoryId})");
                        if (raceStatus == null)
                            Log.Warning($"{prefix} Race {race.Id} has null RaceStatus (RaceStatusId: {race.RaceStatusId})");
                        return race;
                    },
                    new { OrderId = orderId },
                    splitOn: "Id,Id");
                Log.Information($"{prefix} Получено {races.Count()} гонок для OrderId {orderId}");
                return races;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch races for OrderId {orderId}");
                throw;
            }
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение заказа с ID {id}");
            try
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
                Log.Information($"{prefix} Заказ {id} успешно получен");
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch order with ID {id}");
                throw;
            }
        }

        public async Task<int> CreateOrderAsync(Order order)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Создание нового заказа для UserId {order.UserId}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"
                    INSERT INTO ""Orders"" (""UserId"", ""OrderDate"", ""Price"", ""OrderStatusId"")
                    VALUES (@UserId, @OrderDate, @Price, @OrderStatusId)
                    RETURNING ""Id""";
                var id = await connection.ExecuteScalarAsync<int>(sql, order);
                Log.Information($"{prefix} Заказ {id} успешно создан");
                return id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to create order for UserId {order.UserId}");
                throw;
            }
        }

        public async Task UpdateOrderAsync(Order order)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Обновление заказа с ID {order.Id}");
            try
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
                Log.Information($"{prefix} Заказ {order.Id} успешно обновлен");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to update order with ID {order.Id}");
                throw;
            }
        }

        public async Task DeleteOrderAsync(int id)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Удаление заказа с ID {id}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"DELETE FROM ""Orders"" WHERE ""Id"" = @Id";
                await connection.ExecuteAsync(sql, new { Id = id });
                Log.Information($"{prefix} Заказ {id} успешно удален");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to delete order with ID {id}");
                throw;
            }
        }

        public async Task<IEnumerable<OrderStatus>> GetAllOrderStatusesAsync()
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение всех статусов заказов");
            try
            {
                using var connection = CreateConnection();
                var sql = @"SELECT * FROM ""OrderStatuses"" ORDER BY ""Status""";
                var result = await connection.QueryAsync<OrderStatus>(sql);
                Log.Information($"{prefix} Получено {result.Count()} статусов заказов");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch order statuses");
                throw;
            }
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Проверка существования пользователя с ID {userId}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"SELECT COUNT(1) FROM ""AspNetUsers"" WHERE ""Id"" = @UserId";
                var exists = await connection.ExecuteScalarAsync<bool>(sql, new { UserId = userId });
                Log.Information($"{prefix} Пользователь с ID {userId} существует: {exists}");
                return exists;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to check if user with ID {userId} exists");
                throw;
            }
        }

        public async Task<IEnumerable<UserDropdownModel>> GetUsersForDropdownAsync()
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение пользователей для выпадающего списка");
            try
            {
                using var connection = CreateConnection();
                var sql = @"SELECT ""Id"", ""Email"" FROM ""AspNetUsers"" ORDER BY ""Email""";
                var result = await connection.QueryAsync<UserDropdownModel>(sql);
                Log.Information($"{prefix} Получено {result.Count()} пользователей для выпадающего списка");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch users for dropdown");
                throw;
            }
        }
    }
}