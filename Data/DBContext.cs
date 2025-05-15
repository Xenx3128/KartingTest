using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TestMVC.Models;
using Serilog;

namespace TestMVC.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options)
        {
            Log.Information("AppDbContext инициализирован.");
        }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Races> Races { get; set; }
        public DbSet<TechnicalBreaks> TechnicalBreaks { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<UserRace> UserRaces { get; set; }
        public DbSet<CircleResults> CircleResults { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<RaceStatus> RaceStatuses { get; set; }
        public DbSet<BreakStatuses> BreakStatuses { get; set; }
        public DbSet<RaceCategory> RaceCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Log.Information("Настройка связей модели в AppDbContext.");
            base.OnModelCreating(modelBuilder);

            try
            {
                // Order -> Races (one-to-many)
                modelBuilder.Entity<Order>()
                    .HasMany(o => o.Races)
                    .WithOne(r => r.Order)
                    .HasForeignKey(r => r.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Order -> OrderStatus (many-to-one)
                modelBuilder.Entity<Order>()
                    .HasOne(o => o.OrderStatus)
                    .WithMany()
                    .HasForeignKey(o => o.OrderStatusId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Order -> ApplicationUser (many-to-one)
                modelBuilder.Entity<Order>()
                    .HasOne(o => o.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

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

                // UserRace -> ApplicationUser (many-to-one)
                modelBuilder.Entity<UserRace>()
                    .HasOne(ur => ur.User)
                    .WithMany()
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // UserRace -> Races (many-to-one)
                modelBuilder.Entity<UserRace>()
                    .HasOne(ur => ur.Race)
                    .WithMany()
                    .HasForeignKey(ur => ur.RaceId)
                    .OnDelete(DeleteBehavior.Cascade);

                // UserRace -> Cart (many-to-one)
                modelBuilder.Entity<UserRace>()
                    .HasOne(ur => ur.Cart)
                    .WithMany()
                    .HasForeignKey(ur => ur.CartId)
                    .OnDelete(DeleteBehavior.SetNull);

                // UserRace -> CircleResults (many-to-one)
                modelBuilder.Entity<CircleResults>()
                    .HasOne(cr => cr.UserRace)
                    .WithMany()
                    .HasForeignKey(cr => cr.UserRaceId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Identity relationships
                modelBuilder.Entity<ApplicationUser>()
                    .HasMany(u => u.UserRoles)
                    .WithOne()
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
                
                // TechnicalBreaks -> BreakStatus (many-to-one)
                modelBuilder.Entity<TechnicalBreaks>()
                    .HasOne(tb => tb.BreakStatus)
                    .WithMany()
                    .HasForeignKey(tb => tb.BreakStatusId)
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<ApplicationRole>()
                    .HasMany(r => r.UserRoles)
                    .WithOne()
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                modelBuilder.Entity<Order>()
                    .HasIndex(o => o.UserId);
                modelBuilder.Entity<Order>()
                    .HasIndex(o => o.OrderStatusId);
                modelBuilder.Entity<Races>()
                    .HasIndex(r => r.OrderId);
                modelBuilder.Entity<Races>()
                    .HasIndex(r => r.RaceCategoryId);
                modelBuilder.Entity<Races>()
                    .HasIndex(r => r.RaceStatusId);
                modelBuilder.Entity<UserRace>()
                    .HasIndex(ur => ur.UserId);
                modelBuilder.Entity<UserRace>()
                    .HasIndex(ur => ur.RaceId);
                modelBuilder.Entity<UserRace>()
                    .HasIndex(ur => ur.CartId);
                modelBuilder.Entity<CircleResults>()
                    .HasIndex(cr => cr.UserRaceId);
                modelBuilder.Entity<TechnicalBreaks>()
                    .HasIndex(tb => tb.BreakStatusId);

                Log.Information("Связи модели успешно настроены.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to configure model relationships in AppDbContext.");
                throw;
            }
        }
    }
}