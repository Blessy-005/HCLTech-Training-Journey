using Microsoft.EntityFrameworkCore;
using ProductApi.Models;

namespace ProductApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Unique rule: the same product name cannot exist twice in the same category
        modelBuilder.Entity<Product>()
            .HasIndex(p => new { p.Name, p.Category })
            .IsUnique();
    }
}