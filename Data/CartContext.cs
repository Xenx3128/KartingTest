using Dapper;
using Npgsql;
using System.Data;
using TestMVC.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TestMVC.Data
{
    public class CartContext
    {
        private readonly string _connectionString;

        public CartContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        // Get all carts
        public async Task<IEnumerable<Cart>> GetAllCartsAsync()
        {
            using var connection = CreateConnection();
            var sql = @"SELECT ""Id"", ""Name"", ""Desc"" FROM ""Carts"" ORDER BY ""Name""";
            return await connection.QueryAsync<Cart>(sql);
        }

        // Get cart by ID
        public async Task<Cart> GetCartByIdAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT ""Id"", ""Name"", ""Desc"" FROM ""Carts"" WHERE ""Id"" = @Id";
            return await connection.QueryFirstOrDefaultAsync<Cart>(sql, new { Id = id });
        }

        // Create a new cart
        public async Task CreateCartAsync(Cart cart)
        {
            using var connection = CreateConnection();
            var sql = @"
                INSERT INTO ""Carts"" (""Name"", ""Desc"")
                VALUES (@Name, @Desc)
                RETURNING ""Id""";
            cart.Id = await connection.ExecuteScalarAsync<int>(sql, cart);
        }

        // Update an existing cart
        public async Task UpdateCartAsync(Cart cart)
        {
            using var connection = CreateConnection();
            var sql = @"
                UPDATE ""Carts""
                SET ""Name"" = @Name,
                    ""Desc"" = @Desc
                WHERE ""Id"" = @Id";
            await connection.ExecuteAsync(sql, cart);
        }

        // Delete a cart
        public async Task DeleteCartAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = @"DELETE FROM ""Carts"" WHERE ""Id"" = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}