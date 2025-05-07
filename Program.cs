using TestMVC.Data;
using TestMVC.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Identity;
using TestMVC.Utility;
using Microsoft.AspNetCore.Http;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors()
    .LogTo(message => Log.Information(message), LogLevel.Information));

// Add Identity services
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Password requirements
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // Sign-in settings
    options.SignIn.RequireConfirmedAccount = true;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders()
.AddRoles<ApplicationRole>();

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register contexts with proper dependencies
builder.Services.AddScoped<AppointmentContext>(sp =>
{
    Log.Information("Регистрация AppointmentContext с строкой подключения.");
    return new AppointmentContext(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sp.GetRequiredService<UserManager<ApplicationUser>>(),
        sp.GetRequiredService<IHttpContextAccessor>());
});

builder.Services.AddScoped<UserContext>(provider =>
{
    Log.Information("Регистрация UserContext с строкой подключения и сервисами Identity.");
    var connectionString = provider.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection");
    var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
    var signInManager = provider.GetRequiredService<SignInManager<ApplicationUser>>();
    var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
    
    return new UserContext(connectionString, userManager, signInManager, httpContextAccessor);
});

builder.Services.AddScoped<OrderContext>(sp =>
{
    Log.Information("Регистрация OrderContext с строкой подключения.");
    return new OrderContext(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sp.GetRequiredService<UserManager<ApplicationUser>>(),
        sp.GetRequiredService<IHttpContextAccessor>());
});

builder.Services.AddScoped<RaceContext>(sp =>
{
    Log.Information("Регистрация RaceContext с строкой подключения.");
    return new RaceContext(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sp.GetRequiredService<UserManager<ApplicationUser>>(),
        sp.GetRequiredService<IHttpContextAccessor>());
});

builder.Services.AddScoped<BreakContext>(sp =>
{
    Log.Information("Регистрация BreakContext с строкой подключения.");
    return new BreakContext(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sp.GetRequiredService<UserManager<ApplicationUser>>(),
        sp.GetRequiredService<IHttpContextAccessor>());
});

builder.Services.AddScoped<CartContext>(sp =>
{
    Log.Information("Регистрация CartContext с строкой подключения.");
    return new CartContext(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sp.GetRequiredService<UserManager<ApplicationUser>>(),
        sp.GetRequiredService<IHttpContextAccessor>());
});

builder.Services.AddScoped<CircleResultsContext>(sp =>
{
    Log.Information("Регистрация CircleResultsContext с строкой подключения.");
    return new CircleResultsContext(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sp.GetRequiredService<UserManager<ApplicationUser>>(),
        sp.GetRequiredService<IHttpContextAccessor>());
});

builder.Services.AddScoped<SettingsContext>(sp =>
{
    Log.Information("Регистрация SettingsContext с строкой подключения.");
    return new SettingsContext(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sp.GetRequiredService<IHttpContextAccessor>());
});

// Logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/sql.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

Log.Information("Запуск приложения...");
try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}