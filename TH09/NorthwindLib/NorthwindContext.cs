using Microsoft.EntityFrameworkCore;

namespace NorthwindLib;

/// <summary>
/// Northwind Database Context
/// This class represents a session with the Northwind database
/// </summary>
public class NorthwindContext : DbContext
{
    /// <summary>
    /// Suppliers DbSet
    /// </summary>
    public DbSet<Supplier> Suppliers { get; set; } = null!;

    /// <summary>
    /// Products DbSet
    /// </summary>
    public DbSet<Product> Products { get; set; } = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    public NorthwindContext()
    {
    }

    /// <summary>
    /// Constructor with options
    /// </summary>
    /// <param name="options">DbContext options</param>
    public NorthwindContext(DbContextOptions<NorthwindContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Configures the database connection
    /// </summary>
    /// <param name="optionsBuilder">Options builder</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Default connection string for local SQL Server with Northwind database
            // Update this connection string according to your SQL Server setup
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=Northwind;Trusted_Connection=True;TrustServerCertificate=True;"
            );
        }
    }

    /// <summary>
    /// Configures the model
    /// </summary>
    /// <param name="modelBuilder">Model builder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Supplier-Product relationship
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Supplier)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SupplierID)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
