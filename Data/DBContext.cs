using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TestMVC.Models;


namespace TestMVC.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Races> Races { get; set; }
    public DbSet<TechnicalBreaks> TechnicalBreaks { get; set; }
    public DbSet<Settings> Settings { get; set; }
    public DbSet<RaceCart> RaceCarts { get; set; }
    public DbSet<UserCart> UserCarts { get; set; }
    public DbSet<CircleResults> CircleResults { get; set; }
    public DbSet<OrderStatus> OrderStatuses { get; set; }
    public DbSet<RaceStatus> RaceStatuses { get; set; }
    public DbSet<RaceCategory> RaceCategories { get; set; }
    public DbSet<UserStatus> UserStatuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configure relationships using Fluent API (without navigation properties)
        
        // Order -> Races (one-to-many)
        modelBuilder.Entity<Races>()
            .HasOne<Order>()
            .WithMany()
            .HasForeignKey(r => r.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        // UserCart -> User (many-to-one)
        modelBuilder.Entity<UserCart>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // UserCart -> RaceCart (many-to-one)
        modelBuilder.Entity<UserCart>()
            .HasOne<RaceCart>()
            .WithMany()
            .HasForeignKey(uc => uc.RaceCartId)
            .OnDelete(DeleteBehavior.Restrict);

        // CircleResults -> RaceCart (many-to-one)
        modelBuilder.Entity<CircleResults>()
            .HasOne<RaceCart>()
            .WithMany()
            .HasForeignKey(cr => cr.RaceCartId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.UserRoles)
            .WithOne()
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

        modelBuilder.Entity<ApplicationRole>()
            .HasMany(r => r.UserRoles)
            .WithOne()
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();
    }
}