using Dapper;
using Npgsql;
using TestMVC.Models;
using TestMVC.Service;
using TestMVC.Utility;


namespace TestMVC.Data;


public class UserContext{
    private readonly string connectionString;
    private NpgsqlConnection Conn { get; set; }

    public UserContext(string connectionString)
    {
        this.connectionString = connectionString;
        this.Conn = new NpgsqlConnection(this.connectionString);
    }

    public async Task<int> RegisterUser(CartUser user)
    {
        var sql = "INSERT INTO users (username, birthdate, phonenum, email, pwd, userrole, fromwherefoundout, status) VALUES (@UserName, @BirthDate, @PhoneNum, @Email, @Pwd, @UserRole, @FromWhereFoundOut, @Status) RETURNING id;";
        
        var id = await Conn.QuerySingleAsync<int>(sql, user);
        return id;
    }
    public async Task<IEnumerable<CartUser>> GetAllUsers()
    {
        var sql = "SELECT * FROM users";
        var res = await Conn.QueryAsync<CartUser>(sql);
        return res;
    }


    public async Task<CartUser?> GetUserById(int id)
    {
        var sql = "SELECT * FROM users WHERE id = @Id;";
        var res = await Conn.QuerySingleOrDefaultAsync<CartUser>(sql, new { Id = id });
        return res;
    }

    public async Task<CartUser?> GetUserByEmail(string email)
    {
        var sql = "SELECT * FROM users WHERE email = @Email;";
        var res = await Conn.QuerySingleOrDefaultAsync<CartUser>(sql, new { Email = email });
        return res;
    }
    public async void UpdateUser(CartUser user)
    {
        
        var sql = "UPDATE users SET username = @UserName, birthDate = @BirthDate, phoneNum = @PhoneNum, email = @Email, pwd = @Pwd, userRole = @UserRole, fromWhereFoundOut = @FromWhereFoundOut, status = @Status WHERE id = @Id;";
        var args = new {
            Id = user.Id,
            UserName = user.UserName,
            BirthDate = user.BirthDate,
            PhoneNum = user.PhoneNum,
            Email = user.Email,
            Pwd = user.Pwd,
            UserRole = user.UserRole,
            FromWhereFoundOut = user.FromWhereFoundOut,
            Status = user.Status
        };
        Console.WriteLine(args.UserName);
        
        await Conn.QueryAsync<CartUser>(sql, args)  ;
        Console.WriteLine("///");
    }

    public async void DeleteUser(int id)
    {
        var sql = "DELETE FROM users WHERE id = @Id";
        await Conn.QueryAsync<CartUser>(sql, new { Id = id });
    }


}
