using TestMVC.Data;
using TestMVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TestMVC.Service;
using TestMVC.Utility;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the DatabaseService with the connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("NoDBConnection");
builder.Services.AddSingleton(new DatabaseService(connectionString));
connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders()
.AddRoles<ApplicationRole>();

// Register contexts with proper dependencies
builder.Services.AddScoped<AppointmentContext>(provider => 
    new AppointmentContext(
        provider.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<UserContext>(provider => 
{
    var connectionString = provider.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection");
    var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
    var signInManager = provider.GetRequiredService<SignInManager<ApplicationUser>>();
    
    return new UserContext(connectionString, userManager, signInManager);
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin"); // Secure admin area
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

app.Run();