using Npgsql;
using System;
using System.Threading.Tasks;
using TestMVC.Utility;

namespace TestMVC.Service;

public class DatabaseService
{
    public string _connectionString {get; set;}

    public DatabaseService(string connectionString)
    {
        _connectionString = connectionString;

    }

    public async Task InitializeDatabaseAsync()
    {
        await using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var dbcheck = @"
                SELECT 1 FROM pg_catalog.pg_database WHERE datname = 'karting'
            ";
            
            using (var command = new NpgsqlCommand(dbcheck, connection))
            {
                var dbExists = await command.ExecuteScalarAsync() != null;
                if (!dbExists){
                    var createDbSql = @"
                        CREATE DATABASE karting;
                    ";

                    using (var createDbCommand = new NpgsqlCommand(createDbSql, connection))
                    {
                        await createDbCommand.ExecuteNonQueryAsync();
                    }
                    _connectionString += "Database=karting";
                }
            };
            await connection.CloseAsync();
        }
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var sql = @"

            CREATE TABLE IF NOT EXISTS users(
                id SERIAL PRIMARY KEY,
                userName VARCHAR,
                birthDate DATE,
                phoneNum VARCHAR,
                email VARCHAR UNIQUE,
                pwd VARCHAR,
                userRole VARCHAR,
                fromWhereFoundOut VARCHAR,
                status VARCHAR,
                note VARCHAR
            );

            CREATE TABLE IF NOT EXISTS orders(
                id SERIAL PRIMARY KEY,
                userId INTEGER REFERENCES users(id) ON DELETE CASCADE,
                orderDate TIMESTAMP,
                raceDate TIMESTAMP,
                price DECIMAL,
                status VARCHAR
            );

            CREATE TABLE IF NOT EXISTS races(
                id SERIAL PRIMARY KEY,
                orderId INTEGER REFERENCES orders(id) ON DELETE CASCADE,
                startDate TIMESTAMP,
                finishDate TIMESTAMP,
                category VARCHAR,
                status VARCHAR
            );

            CREATE TABLE IF NOT EXISTS raceCart(
                id SERIAL PRIMARY KEY,
                position INTEGER
            );

            CREATE TABLE IF NOT EXISTS userCart(
                id SERIAL PRIMARY KEY,
                userId INTEGER REFERENCES users(id) ON DELETE CASCADE,
                raceCartId INTEGER REFERENCES raceCart(id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS circleResults(
                id SERIAL PRIMARY KEY,
                raceCartId INTEGER REFERENCES raceCart(id) ON DELETE CASCADE,
                circleNum INTEGER,
                circleTime TIME
            );

            CREATE TABLE IF NOT EXISTS settings(
                id SERIAL PRIMARY KEY,
                dayStart TIME,
                dayFinish TIME,
                raceDuration TIME
            );

            CREATE TABLE IF NOT EXISTS technicalBreaks(
                id SERIAL PRIMARY KEY,
                dateStart TIMESTAMP,
                dateFinish TIMESTAMP,
                status VARCHAR
            );

            CREATE TABLE IF NOT EXISTS userStatus(
                id SERIAL PRIMARY KEY,
                status VARCHAR
            );

            CREATE TABLE IF NOT EXISTS orderStatus(
                id SERIAL PRIMARY KEY,
                status VARCHAR
            );

            CREATE TABLE IF NOT EXISTS raceStatus(
                id SERIAL PRIMARY KEY,
                status VARCHAR
            );

            CREATE TABLE IF NOT EXISTS RaceCategory(
                id SERIAL PRIMARY KEY,
                category VARCHAR
            );
            ";

            using (var command = new NpgsqlCommand(sql, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
            await connection.CloseAsync();
        }
    }
}
