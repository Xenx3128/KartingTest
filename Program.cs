using TestMVC.Data;
using TestMVC.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Identity;
using TestMVC.Service;
using TestMVC.Utility;

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

// Add Identity services (consolidated)
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

// Register contexts with proper dependencies
builder.Services.AddSingleton<AppointmentContext>(sp =>
{
    Log.Information("Registering AppointmentContext with connection string.");
    return new AppointmentContext(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<UserContext>(provider =>
{
    Log.Information("Registering UserContext with connection string and Identity services.");
    var connectionString = provider.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection");
    var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
    var signInManager = provider.GetRequiredService<SignInManager<ApplicationUser>>();
    
    return new UserContext(connectionString, userManager, signInManager);
});

builder.Services.AddSingleton<OrderContext>(sp =>
{
    Log.Information("Registering OrderContext with connection string.");
    return new OrderContext(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSingleton<RaceContext>(sp =>
{
    Log.Information("Registering RaceContext with connection string.");
    return new RaceContext(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSingleton<BreakContext>(sp =>
{
    Log.Information("Registering BreakContext with connection string.");
    return new BreakContext(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSingleton<CartContext>(sp =>
{
    Log.Information("Registering CartContext with connection string.");
    return new CartContext(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSingleton<CircleResultsContext>(sp =>
{
    Log.Information("Registering CircleResultsContext with connection string.");
    return new CircleResultsContext(builder.Configuration.GetConnectionString("DefaultConnection"));
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

Log.Information("Application starting...");
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