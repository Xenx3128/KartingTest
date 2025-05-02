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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Order -> Races (one-to-many)
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Races)
            .WithOne(r => r.Order)
            .HasForeignKey(r => r.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Order -> OrderStatus (many-to-one)
        modelBuilder.Entity<Order>()
            .HasOne(o => o.OrderStatus)
            .WithMany()
            .HasForeignKey(o => o.OrderStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        // Order -> ApplicationUser (many-to-one)
        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Races -> RaceCategory (many-to-one)
        modelBuilder.Entity<Races>()
            .HasOne(r => r.RaceCategory)
            .WithMany()
            .HasForeignKey(r => r.RaceCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Races -> RaceStatus (many-to-one)
        modelBuilder.Entity<Races>()
            .HasOne(r => r.RaceStatus)
            .WithMany()
            .HasForeignKey(r => r.RaceStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        // UserCart -> ApplicationUser (many-to-one)
        modelBuilder.Entity<UserCart>()
            .HasOne(uc => uc.User)
            .WithMany()
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // UserCart -> RaceCart (many-to-one)
        modelBuilder.Entity<UserCart>()
            .HasOne(uc => uc.RaceCart)
            .WithMany()
            .HasForeignKey(uc => uc.RaceCartId)
            .OnDelete(DeleteBehavior.Restrict);

        // CircleResults -> RaceCart (many-to-one)
        modelBuilder.Entity<CircleResults>()
            .HasOne(cr => cr.RaceCart)
            .WithMany()
            .HasForeignKey(cr => cr.RaceCartId)
            .OnDelete(DeleteBehavior.Restrict);

        // Identity relationships
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