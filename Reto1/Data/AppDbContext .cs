using Microsoft.EntityFrameworkCore;
using Reto1.Models;

namespace Reto1.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // ORM
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Payment>().ToTable("Payments");

        modelBuilder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Payment>()
            .Property(p => p.MerchantNormalized)
            .IsRequired()
            .HasMaxLength(140);

        modelBuilder.Entity<Payment>()
            .Property(p => p.Merchant)
            .IsRequired()
            .HasMaxLength(120);

        // Evitar duplicados
        modelBuilder.Entity<Payment>()
            .HasIndex(p => new { p.PaidOn, p.Amount, p.MerchantNormalized })
            .IsUnique();
    }
}