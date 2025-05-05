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
    public class RaceContext
    {
        private readonly string _connectionString;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RaceContext(string connectionString, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = connectionString;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            Log.Information($"{GetUserPrefix()} RaceContext инициализирован со строкой подключения.");
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

        public async Task<IEnumerable<Races>> GetAllRacesAsync()
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение всех гонок");
            try
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
                            Log.Warning($"{prefix} Race {race.Id} has null Order (OrderId: {race.OrderId})");
                        if (user == null && order != null)
                            Log.Warning($"{prefix} Order {order.Id} has null User (UserId: {order.UserId})");
                        if (raceCategory == null)
                            Log.Warning($"{prefix} Race {race.Id} has null RaceCategory (RaceCategoryId: {race.RaceCategoryId})");
                        if (raceStatus == null)
                            Log.Warning($"{prefix} Race {race.Id} has null RaceStatus (RaceStatusId: {race.RaceStatusId})");
                        return race;
                    },
                    splitOn: "Id,Id,Id,Id");
                Log.Information($"{prefix} Получено {races.Count()} гонок");
                return races;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch all races");
                throw;
            }
        }

        public async Task<IEnumerable<Races>> GetRacesByStatusAsync(string status)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение гонок со статусом {status}");
            try
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
                            Log.Warning($"{prefix} Race {race.Id} has null Order (OrderId: {race.OrderId})");
                        if (user == null && order != null)
                            Log.Warning($"{prefix} Order {order.Id} has null User (UserId: {order.UserId})");
                        if (raceCategory == null)
                            Log.Warning($"{prefix} Race {race.Id} has null RaceCategory (RaceCategoryId: {race.RaceCategoryId})");
                        if (raceStatus == null)
                            Log.Warning($"{prefix} Race {race.Id} has null RaceStatus (RaceStatusId: {race.RaceStatusId})");
                        return race;
                    },
                    new { Status = status },
                    splitOn: "Id,Id,Id,Id");
                Log.Information($"{prefix} Получено {races.Count()} гонок со статусом {status}");
                return races;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch races with status {status}");
                throw;
            }
        }

        public async Task<Races> GetRaceByIdAsync(int id)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение гонки с ID {id}");
            try
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
                Log.Information($"{prefix} Гонка {id} успешно получена");
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch race with ID {id}");
                throw;
            }
        }

        public async Task<int> CreateRaceAsync(Races race)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Создание новой гонки для OrderId {race.OrderId}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"
                    INSERT INTO ""Races"" (""OrderId"", ""StartDate"", ""FinishDate"", ""RaceCategoryId"", ""RaceStatusId"")
                    VALUES (@OrderId, @StartDate, @FinishDate, @RaceCategoryId, @RaceStatusId)
                    RETURNING ""Id""";
                var id = await connection.ExecuteScalarAsync<int>(sql, race);
                Log.Information($"{prefix} Гонка {id} успешно создана");
                return id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to create race for OrderId {race.OrderId}");
                throw;
            }
        }

        public async Task UpdateRaceAsync(Races race)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Обновление гонки с ID {race.Id}");
            try
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
                Log.Information($"{prefix} Гонка {race.Id} успешно обновлена");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to update race with ID {race.Id}");
                throw;
            }
        }

        public async Task DeleteRaceAsync(int id)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Удаление гонки с ID {id}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"DELETE FROM ""Races"" WHERE ""Id"" = @Id";
                await connection.ExecuteAsync(sql, new { Id = id });
                Log.Information($"{prefix} Гонка {id} успешно удалена");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to delete race with ID {id}");
                throw;
            }
        }

        public async Task<IEnumerable<OrderDropdownModel>> GetOrdersForDropdownAsync()
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение заказов для выпадающего списка");
            try
            {
                using var connection = CreateConnection();
                var sql = @"
                    SELECT o.""Id"", u.""Email""
                    FROM ""Orders"" o
                    LEFT JOIN ""AspNetUsers"" u ON o.""UserId"" = u.""Id""
                    ORDER BY o.""Id""";
                var result = await connection.QueryAsync<OrderDropdownModel>(sql);
                Log.Information($"{prefix} Получено {result.Count()} заказов для выпадающего списка");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch orders for dropdown");
                throw;
            }
        }

        public async Task<IEnumerable<RaceCategory>> GetRaceCategoriesAsync()
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение всех категорий гонок");
            try
            {
                using var connection = CreateConnection();
                var sql = @"SELECT * FROM ""RaceCategories"" ORDER BY ""Category""";
                var result = await connection.QueryAsync<RaceCategory>(sql);
                Log.Information($"{prefix} Получено {result.Count()} категорий гонок");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch race categories");
                throw;
            }
        }

        public async Task<IEnumerable<RaceStatus>> GetRaceStatusesAsync()
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение всех статусов гонок");
            try
            {
                using var connection = CreateConnection();
                var sql = @"SELECT * FROM ""RaceStatuses"" ORDER BY ""Status""";
                var result = await connection.QueryAsync<RaceStatus>(sql);
                Log.Information($"{prefix} Получено {result.Count()} статусов гонок");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch race statuses");
                throw;
            }
        }

        public async Task<bool> OrderExistsAsync(int orderId)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Проверка существования заказа с ID {orderId}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"SELECT COUNT(1) FROM ""Orders"" WHERE ""Id"" = @OrderId";
                var exists = await connection.ExecuteScalarAsync<bool>(sql, new { OrderId = orderId });
                Log.Information($"{prefix} Заказ с ID {orderId} существует: {exists}");
                return exists;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to check if order with ID {orderId} exists");
                throw;
            }
        }

        public async Task<IEnumerable<UserRace>> GetUserRacesByRaceIdAsync(int raceId)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение пользовательских гонок для RaceId {raceId}");
            try
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
                Log.Information($"{prefix} Получено {userRaces.Count()} пользовательских гонок для RaceId {raceId}");
                return userRaces;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch user races for RaceId {raceId}");
                throw;
            }
        }

        public async Task<UserRace> GetUserRaceByIdAsync(int id)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение пользовательской гонки с ID {id}");
            try
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
                Log.Information($"{prefix} Пользовательская гонка {id} успешно получена");
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch user race with ID {id}");
                throw;
            }
        }

        public async Task CreateUserRaceAsync(UserRace userRace)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Создание новой пользовательской гонки для UserId {userRace.UserId} и RaceId {userRace.RaceId}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"
                    INSERT INTO ""UserRaces"" (""UserId"", ""RaceId"", ""CartId"", ""Position"")
                    VALUES (@UserId, @RaceId, @CartId, @Position)
                    RETURNING ""Id""";
                userRace.Id = await connection.ExecuteScalarAsync<int>(sql, userRace);
                Log.Information($"{prefix} Пользовательская гонка {userRace.Id} успешно создана");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to create user race for UserId {userRace.UserId} и RaceId {userRace.RaceId}");
                throw;
            }
        }

        public async Task UpdateUserRaceAsync(UserRace userRace)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Обновление пользовательской гонки с ID {userRace.Id}");
            try
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
                Log.Information($"{prefix} Пользовательская гонка {userRace.Id} успешно обновлена");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to update user race with ID {userRace.Id}");
                throw;
            }
        }

        public async Task DeleteUserRaceAsync(int id)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Удаление пользовательской гонки с ID {id}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"DELETE FROM ""UserRaces"" WHERE ""Id"" = @Id";
                await connection.ExecuteAsync(sql, new { Id = id });
                Log.Information($"{prefix} Пользовательская гонка {id} успешно удалена");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to delete user race with ID {id}");
                throw;
            }
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение всех пользователей");
            try
            {
                using var connection = CreateConnection();
                var sql = @"SELECT ""Id"", ""FullName"", ""Email"" FROM ""AspNetUsers"" ORDER BY ""FullName""";
                var result = await connection.QueryAsync<ApplicationUser>(sql);
                Log.Information($"{prefix} Получено {result.Count()} пользователей");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch all users");
                throw;
            }
        }

        public async Task<IEnumerable<Cart>> GetAllCartsAsync()
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение всех картов");
            try
            {
                using var connection = CreateConnection();
                var sql = @"SELECT ""Id"", ""Name"" FROM ""Carts"" ORDER BY ""Name""";
                var result = await connection.QueryAsync<Cart>(sql);
                Log.Information($"{prefix} Получено {result.Count()} картов");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch all carts");
                throw;
            }
        }
    }

    public class OrderDropdownModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
    }
}