using CaloryfiAPI.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace CaloryfiAPI.Data;

public class AppDatabaseContext : DbContext
{
    public AppDatabaseContext(DbContextOptions<AppDatabaseContext> options)
            : base(options)
    {
    }

    // DbSety odpowiadające tabelom
    public DbSet<User> Users { get; set; }
    public DbSet<UserSetting> UserSettings { get; set; }
    public DbSet<WeightHistory> WeightHistories { get; set; }
    public DbSet<Meal> Meals { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<MealComponent> MealComponents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Konfiguracja klucza złożonego w MealComponent
        modelBuilder.Entity<MealComponent>()
            .HasKey(mc => new { mc.MealId, mc.IngredientId });

        // Konfiguracja relacji
        modelBuilder.Entity<User>()
            .HasOne(u => u.UserSetting)
            .WithOne(us => us.User)
            .HasForeignKey<UserSetting>(us => us.UserId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.WeightHistory)
            .WithOne(wh => wh.User)
            .HasForeignKey(wh => wh.UserId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Meals)
            .WithOne(m => m.User)
            .HasForeignKey(m => m.UserId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Ingredients)
            .WithOne(i => i.User)
            .HasForeignKey(i => i.UserId);

        modelBuilder.Entity<Meal>()
            .HasMany(m => m.MealComponents)
            .WithOne(mc => mc.Meal)
            .HasForeignKey(mc => mc.MealId);

        modelBuilder.Entity<Ingredient>()
            .HasMany(i => i.MealComponents)
            .WithOne(mc => mc.Ingredient)
            .HasForeignKey(mc => mc.IngredientId);

        modelBuilder.Entity<Meal>()
            .Property(m => m.Date_Added)
            .HasConversion(
                v => v.ToUniversalTime(), // zapis do DB jako UTC
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc) // odczyt z DB jako UTC
            );
    }
}
