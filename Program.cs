using TestMVC.Data;
using TestMVC.Models;
using Microsoft.EntityFrameworkCore;
using Dapper;
using Npgsql;
using TestMVC.Utility;
using TestMVC.Service;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// Register the DatabaseService with the connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("NoDBConnection");
Console.WriteLine(connectionString);
builder.Services.AddSingleton(new DatabaseService(connectionString));
connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


var app = builder.Build();

// Initialize the database
using (var scope = app.Services.CreateScope())
{
    var dbService = scope.ServiceProvider.GetRequiredService<DatabaseService>();
    dbService.InitializeDatabaseAsync().Wait();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();