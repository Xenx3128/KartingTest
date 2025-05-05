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
    public class CartContext
    {
        private readonly string _connectionString;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartContext(string connectionString, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = connectionString;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            Log.Information($"{GetUserPrefix()} CartContext инициализирован со строкой подключения.");
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

        public async Task<IEnumerable<Cart>> GetAllCartsAsync()
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение всех картов");
            try
            {
                using var connection = CreateConnection();
                var sql = @"SELECT ""Id"", ""Name"", ""Desc"" FROM ""Carts"" ORDER BY ""Name""";
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

        public async Task<Cart> GetCartByIdAsync(int id)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Получение карта с ID {id}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"SELECT ""Id"", ""Name"", ""Desc"" FROM ""Carts"" WHERE ""Id"" = @Id";
                var result = await connection.QueryFirstOrDefaultAsync<Cart>(sql, new { Id = id });
                Log.Information($"{prefix} Карт {id} успешно получен");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to fetch cart with ID {id}");
                throw;
            }
        }

        public async Task CreateCartAsync(Cart cart)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Создание нового карта с именем {cart.Name}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"
                    INSERT INTO ""Carts"" (""Name"", ""Desc"")
                    VALUES (@Name, @Desc)
                    RETURNING ""Id""";
                cart.Id = await connection.ExecuteScalarAsync<int>(sql, cart);
                Log.Information($"{prefix} Карт {cart.Id} успешно создан");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to create cart with name {cart.Name}");
                throw;
            }
        }

        public async Task UpdateCartAsync(Cart cart)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Обновление карта с ID {cart.Id}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"
                    UPDATE ""Carts""
                    SET ""Name"" = @Name,
                        ""Desc"" = @Desc
                    WHERE ""Id"" = @Id";
                await connection.ExecuteAsync(sql, cart);
                Log.Information($"{prefix} Карт {cart.Id} успешно обновлен");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to update cart with ID {cart.Id}");
                throw;
            }
        }

        public async Task DeleteCartAsync(int id)
        {
            var prefix = GetUserPrefix();
            Log.Information($"{prefix} Удаление карта с ID {id}");
            try
            {
                using var connection = CreateConnection();
                var sql = @"DELETE FROM ""Carts"" WHERE ""Id"" = @Id";
                await connection.ExecuteAsync(sql, new { Id = id });
                Log.Information($"{prefix} Карт {id} успешно удален");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{prefix} Failed to delete cart with ID {id}");
                throw;
            }
        }
    }
}