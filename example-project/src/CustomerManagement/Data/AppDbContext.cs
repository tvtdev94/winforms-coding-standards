using CustomerManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Data;

/// <summary>
/// Application database context for Entity Framework Core.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the Customers DbSet.
    /// </summary>
    public DbSet<Customer> Customers => Set<Customer>();

    /// <summary>
    /// Gets or sets the Orders DbSet.
    /// </summary>
    public DbSet<Order> Orders => Set<Order>();

    /// <summary>
    /// Configures the entity mappings and relationships.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Customer entity
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customers");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.Property(e => e.Phone)
                .HasMaxLength(20);

            entity.Property(e => e.Address)
                .HasMaxLength(200);

            entity.Property(e => e.City)
                .HasMaxLength(50);

            entity.Property(e => e.Country)
                .HasMaxLength(50);

            // Configure one-to-many relationship
            entity.HasMany(e => e.Orders)
                .WithOne(e => e.Customer)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Order entity
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.OrderNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasIndex(e => e.OrderNumber)
                .IsUnique();

            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.Notes)
                .HasMaxLength(500);
        });

        // Seed data for development/testing
        SeedData(modelBuilder);
    }

    /// <summary>
    /// Seeds initial data for development and testing.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed customers
        modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                Id = 1,
                Name = "John Doe",
                Email = "john.doe@example.com",
                Phone = "+1-555-0100",
                Address = "123 Main St",
                City = "New York",
                Country = "USA",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Customer
            {
                Id = 2,
                Name = "Jane Smith",
                Email = "jane.smith@example.com",
                Phone = "+1-555-0101",
                Address = "456 Oak Ave",
                City = "Los Angeles",
                Country = "USA",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc)
            },
            new Customer
            {
                Id = 3,
                Name = "Bob Johnson",
                Email = "bob.johnson@example.com",
                Phone = "+1-555-0102",
                Address = "789 Pine Rd",
                City = "Chicago",
                Country = "USA",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 3, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // Seed orders
        modelBuilder.Entity<Order>().HasData(
            new Order
            {
                Id = 1,
                OrderNumber = "ORD-2024-001",
                CustomerId = 1,
                OrderDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                TotalAmount = 150.50m,
                Status = "Completed",
                Notes = "First order from John",
                CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc)
            },
            new Order
            {
                Id = 2,
                OrderNumber = "ORD-2024-002",
                CustomerId = 1,
                OrderDate = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                TotalAmount = 275.00m,
                Status = "Pending",
                Notes = "Second order from John",
                CreatedAt = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Order
            {
                Id = 3,
                OrderNumber = "ORD-2024-003",
                CustomerId = 2,
                OrderDate = new DateTime(2024, 1, 20, 0, 0, 0, DateTimeKind.Utc),
                TotalAmount = 425.75m,
                Status = "Shipped",
                Notes = "Express shipping requested",
                CreatedAt = new DateTime(2024, 1, 20, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// Updates the UpdatedAt timestamp for modified Customer entities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update UpdatedAt timestamp for modified customers
        var modifiedCustomers = ChangeTracker.Entries<Customer>()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in modifiedCustomers)
        {
            entry.Entity.UpdatedAt = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
