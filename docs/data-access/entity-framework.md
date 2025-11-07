# Entity Framework Core in WinForms

> **Quick Reference**: Complete guide to using Entity Framework Core for data access in WinForms applications with best practices and patterns.

---

## 📋 Overview

**Entity Framework Core (EF Core)** is a modern, lightweight, and extensible ORM (Object-Relational Mapper) that enables .NET developers to work with databases using .NET objects, eliminating the need for most data-access code.

### Key Features:
- LINQ queries for type-safe database operations
- Change tracking for automatic updates
- Migrations for database schema management
- Support for multiple database providers (SQL Server, SQLite, PostgreSQL, etc.)
- Code-First and Database-First approaches

---

## 🎯 Why This Matters

### Benefits of EF Core over Raw ADO.NET:

✅ **Productivity** - Write less boilerplate code
✅ **Type Safety** - Compile-time checking with LINQ
✅ **Maintainability** - Entity classes are easier to maintain than SQL strings
✅ **Change Tracking** - Automatic detection of entity changes
✅ **Migrations** - Version control for database schema
✅ **Abstraction** - Work with objects instead of DataReaders/DataSets
✅ **Testing** - Easy to mock and unit test

### When to Use EF Core:
- CRUD-heavy applications
- Applications requiring complex object graphs
- Projects needing database abstraction
- When rapid development is prioritized

### When ADO.NET Might Be Better:
- Extremely high-performance scenarios
- Bulk operations (use bulk extensions with EF Core)
- Simple data access with minimal object mapping

---

## 🛠️ Getting Started

### Installing EF Core

#### Required NuGet Packages:

```bash
# Core EF packages
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer

# Design-time tools (for migrations)
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools

# Optional: For SQLite (alternative to SQL Server)
dotnet add package Microsoft.EntityFrameworkCore.Sqlite

# Optional: For PostgreSQL
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

#### Package.json Example:

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0">
    <PrivateAssets>all</PrivateAssets>
  </PackageReference>
  <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0">
    <PrivateAssets>all</PrivateAssets>
  </PackageReference>
</ItemGroup>
```

---

### Creating DbContext

The `DbContext` is the bridge between your entities and the database.

```csharp
// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    // DbSet properties represent tables
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    // Constructor for DI
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Configure connection string (if not using DI)
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=MyAppDb;Trusted_Connection=True;");
        }
    }

    // Configure entity relationships and constraints
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Customer configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(c => c.Email).IsUnique();
            entity.Property(c => c.RowVersion).IsRowVersion(); // Concurrency token
        });

        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.OrderDate).IsRequired();
            entity.Property(o => o.TotalAmount).HasPrecision(18, 2);

            // One-to-Many: Customer -> Orders
            entity.HasOne(o => o.Customer)
                  .WithMany(c => c.Orders)
                  .HasForeignKey(o => o.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // OrderItem configuration
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi => oi.Id);
            entity.Property(oi => oi.Quantity).IsRequired();
            entity.Property(oi => oi.UnitPrice).HasPrecision(18, 2);

            // Many-to-One: OrderItem -> Order
            entity.HasOne(oi => oi.Order)
                  .WithMany(o => o.OrderItems)
                  .HasForeignKey(oi => oi.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Many-to-One: OrderItem -> Product
            entity.HasOne(oi => oi.Product)
                  .WithMany()
                  .HasForeignKey(oi => oi.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed data
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Product A", Price = 10.00m },
            new Product { Id = 2, Name = "Product B", Price = 20.00m }
        );
    }
}
```

---

### Connection String Configuration

#### appsettings.json (Recommended):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyAppDb;Trusted_Connection=True;TrustServerCertificate=True;",
    "Development": "Server=(localdb)\\mssqllocaldb;Database=MyAppDb_Dev;Trusted_Connection=True;",
    "Production": "Server=prod-server;Database=MyAppDb;User Id=appuser;Password=***;Encrypt=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  }
}
```

#### Configure in Program.cs with DI:

```csharp
// Program.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        ApplicationConfiguration.Initialize();

        var services = new ServiceCollection();
        ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();

        var mainForm = serviceProvider.GetRequiredService<MainForm>();
        Application.Run(mainForm);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // DbContext with connection string from config
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                });

            // Enable sensitive data logging in development only
            #if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            #endif
        });

        // Repositories
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        // Forms
        services.AddTransient<MainForm>();
    }
}
```

---

## 🚀 Code-First Approach

### Creating Entity Classes

#### POCO Classes (Plain Old CLR Objects):

```csharp
// Models/Customer.cs
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public byte[]? RowVersion { get; set; } // For concurrency

    // Navigation property
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

// Models/Order.cs
public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }

    // Navigation properties
    public Customer Customer { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

// Models/OrderItem.cs
public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    // Navigation properties
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

// Models/Product.cs
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}

public enum OrderStatus
{
    Pending = 0,
    Processing = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4
}
```

#### Data Annotations Alternative:

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Customers")]
public class Customer
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [MaxLength(20)]
    public string? Phone { get; set; }

    [Timestamp]
    public byte[]? RowVersion { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
```

---

### Migrations

Migrations provide version control for your database schema.

#### Create Initial Migration:

```bash
# Using Package Manager Console
Add-Migration InitialCreate

# Using .NET CLI
dotnet ef migrations add InitialCreate
```

#### Apply Migration to Database:

```bash
# Package Manager Console
Update-Database

# .NET CLI
dotnet ef database update
```

#### Add New Migration After Model Changes:

```bash
dotnet ef migrations add AddPhoneToCustomer
dotnet ef database update
```

#### Rollback Migration:

```bash
# Rollback to specific migration
dotnet ef database update PreviousMigrationName

# Rollback all migrations
dotnet ef database update 0
```

#### Remove Last Migration (if not applied):

```bash
dotnet ef migrations remove
```

#### Seeding Data in Migrations:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Product>().HasData(
        new Product { Id = 1, Name = "Laptop", Price = 999.99m, StockQuantity = 50 },
        new Product { Id = 2, Name = "Mouse", Price = 29.99m, StockQuantity = 200 },
        new Product { Id = 3, Name = "Keyboard", Price = 79.99m, StockQuantity = 150 }
    );
}
```

---

### Database Creation

#### EnsureCreated vs Migrations:

```csharp
// ❌ EnsureCreated - Creates DB but NO migration history
// Use only for prototyping or testing
using (var context = new AppDbContext())
{
    context.Database.EnsureCreated(); // Creates if not exists
}

// ✅ Migrations - Production-ready approach
// Tracks schema changes over time
using (var context = new AppDbContext())
{
    context.Database.Migrate(); // Applies pending migrations
}
```

#### When to Use Each:

**EnsureCreated:**
- Rapid prototyping
- Unit testing with in-memory databases
- Demos and proof-of-concepts

**Migrations:**
- Production applications
- Team development
- When you need schema version control
- When updating existing databases

---

## 🔄 Database-First Approach

### Scaffold-DbContext

Reverse engineer an existing database into entity classes.

```bash
# Scaffold entire database
dotnet ef dbcontext scaffold "Server=(localdb)\mssqllocaldb;Database=ExistingDb;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models

# Scaffold specific tables
dotnet ef dbcontext scaffold "ConnectionString" Microsoft.EntityFrameworkCore.SqlServer -o Models -t Customers -t Orders

# Force overwrite
dotnet ef dbcontext scaffold "ConnectionString" Microsoft.EntityFrameworkCore.SqlServer -o Models --force

# Use Data Annotations instead of Fluent API
dotnet ef dbcontext scaffold "ConnectionString" Microsoft.EntityFrameworkCore.SqlServer -o Models --data-annotations
```

#### Generated DbContext Example:

```csharp
public partial class ExistingDbContext : DbContext
{
    public ExistingDbContext()
    {
    }

    public ExistingDbContext(DbContextOptions<ExistingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Scaffolded configuration
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
```

---

### Keeping in Sync

#### Protecting Custom Code with Partial Classes:

```csharp
// ExistingDbContext.Generated.cs (auto-generated, will be overwritten)
public partial class ExistingDbContext : DbContext
{
    // Generated code
}

// ExistingDbContext.Custom.cs (your custom code, won't be overwritten)
public partial class ExistingDbContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        // Your custom Fluent API configurations
        modelBuilder.Entity<Customer>()
            .HasQueryFilter(c => !c.IsDeleted); // Soft delete filter
    }

    public async Task<List<Customer>> GetActiveCustomersAsync()
    {
        return await Customers.Where(c => c.IsActive).ToListAsync();
    }
}
```

---

## 💾 CRUD Operations

### Querying Data

```csharp
// Repository/CustomerRepository.cs
public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    // Get all
    public async Task<List<Customer>> GetAllAsync()
    {
        return await _context.Customers
            .AsNoTracking() // Read-only, better performance
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    // Get by ID
    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await _context.Customers
            .Include(c => c.Orders) // Eager load related data
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    // Get with filter
    public async Task<List<Customer>> SearchAsync(string searchTerm)
    {
        return await _context.Customers
            .AsNoTracking()
            .Where(c => c.Name.Contains(searchTerm) || c.Email.Contains(searchTerm))
            .ToListAsync();
    }

    // Get with paging
    public async Task<List<Customer>> GetPagedAsync(int page, int pageSize)
    {
        return await _context.Customers
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    // Single vs First vs FirstOrDefault
    public async Task<Customer> GetByEmailSingleAsync(string email)
    {
        // Single - throws if 0 or >1 results
        return await _context.Customers.SingleAsync(c => c.Email == email);
    }

    public async Task<Customer?> GetByEmailFirstAsync(string email)
    {
        // FirstOrDefault - returns null if not found, first if multiple
        return await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
    }
}
```

---

### Adding Data

```csharp
// Add single entity
public async Task<int> AddAsync(Customer customer)
{
    _context.Customers.Add(customer);
    await _context.SaveChangesAsync();
    return customer.Id; // ID is auto-populated after save
}

// Add multiple entities (better performance)
public async Task AddRangeAsync(List<Customer> customers)
{
    _context.Customers.AddRange(customers);
    await _context.SaveChangesAsync();
}

// Add with related entities
public async Task<int> AddOrderWithItemsAsync(Order order)
{
    // EF Core will automatically add related OrderItems
    _context.Orders.Add(order);
    await _context.SaveChangesAsync();
    return order.Id;
}
```

---

### Updating Data

```csharp
// Update tracked entity (recommended)
public async Task<bool> UpdateAsync(Customer customer)
{
    var existing = await _context.Customers.FindAsync(customer.Id);
    if (existing == null)
        return false;

    // Update properties
    existing.Name = customer.Name;
    existing.Email = customer.Email;
    existing.Phone = customer.Phone;

    await _context.SaveChangesAsync();
    return true;
}

// Update untracked entity
public async Task<bool> UpdateUntrackedAsync(Customer customer)
{
    _context.Customers.Update(customer); // Marks all properties as modified
    await _context.SaveChangesAsync();
    return true;
}

// Update specific properties
public async Task<bool> UpdateEmailAsync(int customerId, string newEmail)
{
    var customer = await _context.Customers.FindAsync(customerId);
    if (customer == null)
        return false;

    customer.Email = newEmail;
    // Only Email will be updated in database
    await _context.SaveChangesAsync();
    return true;
}

// Batch update (EF Core 7+)
public async Task<int> DeactivateOldCustomersAsync(DateTime cutoffDate)
{
    return await _context.Customers
        .Where(c => c.CreatedDate < cutoffDate)
        .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.IsActive, false));
}
```

---

### Deleting Data

```csharp
// Delete by entity
public async Task<bool> DeleteAsync(int customerId)
{
    var customer = await _context.Customers.FindAsync(customerId);
    if (customer == null)
        return false;

    _context.Customers.Remove(customer);
    await _context.SaveChangesAsync();
    return true;
}

// Delete without loading (better performance)
public async Task<bool> DeleteByIdAsync(int customerId)
{
    // Create a stub entity
    var customer = new Customer { Id = customerId };
    _context.Customers.Attach(customer);
    _context.Customers.Remove(customer);

    try
    {
        await _context.SaveChangesAsync();
        return true;
    }
    catch (DbUpdateConcurrencyException)
    {
        return false; // Entity didn't exist
    }
}

// Soft delete pattern (recommended)
public async Task<bool> SoftDeleteAsync(int customerId)
{
    var customer = await _context.Customers.FindAsync(customerId);
    if (customer == null)
        return false;

    customer.IsDeleted = true;
    customer.DeletedDate = DateTime.UtcNow;
    await _context.SaveChangesAsync();
    return true;
}

// Batch delete (EF Core 7+)
public async Task<int> DeleteInactiveCustomersAsync()
{
    return await _context.Customers
        .Where(c => !c.IsActive && c.CreatedDate < DateTime.UtcNow.AddYears(-2))
        .ExecuteDeleteAsync();
}
```

---

## 🔍 Advanced Querying

### Eager Loading

Load related data upfront to avoid multiple database calls (N+1 problem).

```csharp
// Include - load related entities
public async Task<Customer?> GetCustomerWithOrdersAsync(int customerId)
{
    return await _context.Customers
        .Include(c => c.Orders)
        .FirstOrDefaultAsync(c => c.Id == customerId);
}

// ThenInclude - load nested related entities
public async Task<Customer?> GetCustomerWithOrderDetailsAsync(int customerId)
{
    return await _context.Customers
        .Include(c => c.Orders)
            .ThenInclude(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
        .FirstOrDefaultAsync(c => c.Id == customerId);
}

// Multiple includes
public async Task<Order?> GetOrderDetailsAsync(int orderId)
{
    return await _context.Orders
        .Include(o => o.Customer)
        .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
        .FirstOrDefaultAsync(o => o.Id == orderId);
}
```

---

### Lazy Loading

Load related data on-demand (generally not recommended for WinForms).

```csharp
// Enable lazy loading (requires package)
// dotnet add package Microsoft.EntityFrameworkCore.Proxies

// In DbContext OnConfiguring:
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseLazyLoadingProxies();
}

// Make navigation properties virtual
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

// Usage - Orders are loaded when accessed
var customer = await _context.Customers.FindAsync(customerId);
// Database query happens here:
var orderCount = customer.Orders.Count;
```

**⚠️ Warning:** Lazy loading can cause performance issues in WinForms due to:
- N+1 query problem
- Unpredictable database calls
- UI thread blocking

**Recommendation:** Use eager loading with `Include()` instead.

---

### Explicit Loading

Selectively load related data after retrieving the entity.

```csharp
public async Task<Customer> GetCustomerThenLoadOrdersAsync(int customerId)
{
    var customer = await _context.Customers.FindAsync(customerId);

    // Load orders explicitly
    await _context.Entry(customer)
        .Collection(c => c.Orders)
        .LoadAsync();

    return customer;
}

// Load with filter
public async Task LoadRecentOrdersAsync(Customer customer)
{
    await _context.Entry(customer)
        .Collection(c => c.Orders)
        .Query()
        .Where(o => o.OrderDate > DateTime.UtcNow.AddMonths(-3))
        .LoadAsync();
}
```

---

### Raw SQL Queries

Execute raw SQL when LINQ is insufficient.

```csharp
// Query entities with raw SQL
public async Task<List<Customer>> GetCustomersByRegionAsync(string region)
{
    return await _context.Customers
        .FromSqlRaw("SELECT * FROM Customers WHERE Region = {0}", region)
        .ToListAsync();
}

// Parameterized query (prevents SQL injection)
public async Task<List<Customer>> SearchCustomersAsync(string searchTerm)
{
    var param = new SqlParameter("@searchTerm", $"%{searchTerm}%");
    return await _context.Customers
        .FromSqlRaw("SELECT * FROM Customers WHERE Name LIKE @searchTerm", param)
        .ToListAsync();
}

// Execute non-query (INSERT, UPDATE, DELETE)
public async Task<int> UpdateCustomerStatusAsync(int customerId, string status)
{
    return await _context.Database.ExecuteSqlRawAsync(
        "UPDATE Customers SET Status = {0} WHERE Id = {1}", status, customerId);
}
```

**⚠️ SQL Injection Warning:**
- Always use parameterized queries
- Never concatenate user input into SQL strings

---

## ⚡ Performance Optimization

### AsNoTracking

Use for read-only queries to improve performance.

```csharp
// ❌ With tracking (slower, more memory)
public async Task<List<Customer>> GetAllCustomersTracked()
{
    return await _context.Customers.ToListAsync();
}

// ✅ Without tracking (faster, less memory)
public async Task<List<Customer>> GetAllCustomersNoTracking()
{
    return await _context.Customers
        .AsNoTracking()
        .ToListAsync();
}

// Use when:
// - Displaying read-only data in DataGridView
// - Populating dropdowns
// - Generating reports
// - No updates needed

// Don't use when:
// - You need to update entities
// - Change tracking is required
```

---

### Compiled Queries

Pre-compile frequently used queries for better performance.

```csharp
public class CustomerRepository
{
    // Define compiled query
    private static readonly Func<AppDbContext, int, Task<Customer?>> _getByIdQuery =
        EF.CompileAsyncQuery((AppDbContext context, int id) =>
            context.Customers
                .Include(c => c.Orders)
                .FirstOrDefault(c => c.Id == id));

    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    // Use compiled query
    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await _getByIdQuery(_context, id);
    }
}
```

---

### Batch Operations

Minimize database round-trips.

```csharp
// ❌ Multiple round-trips
foreach (var customer in customers)
{
    _context.Customers.Add(customer);
    await _context.SaveChangesAsync(); // Database call per iteration
}

// ✅ Single round-trip
_context.Customers.AddRange(customers);
await _context.SaveChangesAsync(); // One database call

// ✅ Bulk extensions (for massive operations)
// Install: dotnet add package EFCore.BulkExtensions
await _context.BulkInsertAsync(customers);
await _context.BulkUpdateAsync(customers);
await _context.BulkDeleteAsync(customers);
```

---

### Connection Pooling

Configure connection pooling for better performance.

```csharp
// Connection string with pooling
"Server=myserver;Database=mydb;User Id=user;Password=pass;Max Pool Size=100;Min Pool Size=10;Pooling=true;"

// DbContext options
services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.MaxBatchSize(100); // Batch multiple operations
        sqlOptions.CommandTimeout(30); // 30 seconds timeout
        sqlOptions.EnableRetryOnFailure(3); // Retry 3 times on failure
    });
});
```

---

## 🔄 Change Tracking

### ChangeTracker

Monitor and control entity changes.

```csharp
// Check entity state
public void CheckEntityState(Customer customer)
{
    var entry = _context.Entry(customer);
    var state = entry.State; // Added, Modified, Deleted, Unchanged, Detached
}

// Get all tracked entities
public List<Customer> GetModifiedCustomers()
{
    return _context.ChangeTracker
        .Entries<Customer>()
        .Where(e => e.State == EntityState.Modified)
        .Select(e => e.Entity)
        .ToList();
}

// Detect changes manually
public void DetectChanges()
{
    _context.ChangeTracker.DetectChanges();
}

// Disable auto-detect changes (for bulk operations)
public async Task BulkUpdateAsync(List<Customer> customers)
{
    _context.ChangeTracker.AutoDetectChangesEnabled = false;

    foreach (var customer in customers)
    {
        _context.Customers.Update(customer);
    }

    _context.ChangeTracker.DetectChanges();
    await _context.SaveChangesAsync();

    _context.ChangeTracker.AutoDetectChangesEnabled = true;
}
```

---

### DetachAllEntities

Clear the context of all tracked entities.

```csharp
public void DetachAllEntities()
{
    var entries = _context.ChangeTracker.Entries()
        .Where(e => e.State != EntityState.Detached)
        .ToList();

    foreach (var entry in entries)
    {
        entry.State = EntityState.Detached;
    }
}

// Use when:
// - Reusing DbContext for unrelated operations
// - Clearing cache between operations
// - Resolving tracking conflicts
```

---

## 🔐 Transaction Management

### SaveChanges Transactions

`SaveChanges()` automatically wraps changes in a transaction.

```csharp
// Automatic transaction
public async Task<bool> TransferCustomerOrdersAsync(int fromCustomerId, int toCustomerId)
{
    try
    {
        var orders = await _context.Orders
            .Where(o => o.CustomerId == fromCustomerId)
            .ToListAsync();

        foreach (var order in orders)
        {
            order.CustomerId = toCustomerId;
        }

        // All changes committed or rolled back together
        await _context.SaveChangesAsync();
        return true;
    }
    catch
    {
        // Automatic rollback on exception
        return false;
    }
}
```

---

### Explicit Transactions

Control transaction scope manually.

```csharp
public async Task<bool> ProcessOrderWithInventoryAsync(Order order)
{
    using var transaction = await _context.Database.BeginTransactionAsync();

    try
    {
        // Add order
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Update inventory
        foreach (var item in order.OrderItems)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product == null || product.StockQuantity < item.Quantity)
            {
                throw new InvalidOperationException("Insufficient stock");
            }

            product.StockQuantity -= item.Quantity;
        }

        await _context.SaveChangesAsync();

        // Commit if all successful
        await transaction.CommitAsync();
        return true;
    }
    catch
    {
        // Rollback on any error
        await transaction.RollbackAsync();
        return false;
    }
}
```

---

### Distributed Transactions

Use `TransactionScope` for operations spanning multiple contexts or databases.

```csharp
using System.Transactions;

public async Task<bool> ProcessCrossContextOperationAsync()
{
    using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

    try
    {
        using (var context1 = new AppDbContext())
        {
            // Operation in first context
            context1.Customers.Add(new Customer { Name = "Test" });
            await context1.SaveChangesAsync();
        }

        using (var context2 = new AnotherDbContext())
        {
            // Operation in second context
            await context2.SaveChangesAsync();
        }

        scope.Complete(); // Commit both
        return true;
    }
    catch
    {
        // Both rolled back automatically
        return false;
    }
}
```

---

## ✅ Best Practices

### DO:

✅ **Use async/await** for all database operations
```csharp
var customers = await _context.Customers.ToListAsync();
```

✅ **Use AsNoTracking** for read-only queries
```csharp
var customers = await _context.Customers.AsNoTracking().ToListAsync();
```

✅ **Use Include** to avoid N+1 queries
```csharp
var customers = await _context.Customers.Include(c => c.Orders).ToListAsync();
```

✅ **Parameterize raw SQL queries**
```csharp
await _context.Customers.FromSqlRaw("SELECT * FROM Customers WHERE Id = {0}", id).ToListAsync();
```

✅ **Use migrations** for schema management
```bash
dotnet ef migrations add AddNewColumn
dotnet ef database update
```

✅ **Dispose DbContext** properly
```csharp
using (var context = new AppDbContext())
{
    // Use context
}
```

✅ **Use scoped lifetime** for DbContext in DI
```csharp
services.AddDbContext<AppDbContext>();
```

✅ **Handle concurrency** with RowVersion
```csharp
[Timestamp]
public byte[]? RowVersion { get; set; }
```

✅ **Use repository pattern** for testability
```csharp
public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int id);
}
```

✅ **Log SQL queries** in development
```csharp
options.EnableSensitiveDataLogging();
options.LogTo(Console.WriteLine);
```

✅ **Use transactions** for multi-step operations
```csharp
using var transaction = await _context.Database.BeginTransactionAsync();
```

✅ **Validate before SaveChanges**
```csharp
if (customer.IsValid())
    await _context.SaveChangesAsync();
```

✅ **Use specific DbSet methods**
```csharp
await _context.Customers.FindAsync(id); // Uses primary key
```

✅ **Implement soft delete** instead of hard delete
```csharp
customer.IsDeleted = true;
await _context.SaveChangesAsync();
```

✅ **Use DTOs** for data transfer (not entities directly)
```csharp
var customerDto = customer.ToDto();
```

---

### DON'T:

❌ **Don't use synchronous methods** (blocks UI)
```csharp
var customers = _context.Customers.ToList(); // BAD
```

❌ **Don't forget AsNoTracking** for read-only data
```csharp
var customers = await _context.Customers.ToListAsync(); // Unnecessary tracking
```

❌ **Don't create DbContext as singleton**
```csharp
services.AddSingleton<AppDbContext>(); // BAD - use Scoped
```

❌ **Don't concatenate SQL** (SQL injection risk)
```csharp
var sql = $"SELECT * FROM Customers WHERE Name = '{name}'"; // DANGEROUS
```

❌ **Don't call SaveChanges in loops**
```csharp
foreach (var item in items)
{
    _context.Add(item);
    await _context.SaveChangesAsync(); // BAD - call once outside loop
}
```

❌ **Don't ignore concurrency conflicts**
```csharp
try { await _context.SaveChangesAsync(); }
catch (DbUpdateConcurrencyException) { } // BAD - handle properly
```

❌ **Don't use lazy loading** in WinForms (performance issues)
```csharp
options.UseLazyLoadingProxies(); // Avoid in WinForms
```

❌ **Don't expose DbContext** to UI layer
```csharp
public class CustomerForm
{
    private AppDbContext _context; // BAD - use repository
}
```

❌ **Don't use EnsureCreated** in production
```csharp
_context.Database.EnsureCreated(); // Only for testing
```

❌ **Don't forget to dispose** DbContext
```csharp
var context = new AppDbContext(); // BAD - use 'using'
```

❌ **Don't query in loops** (N+1 problem)
```csharp
foreach (var customer in customers)
{
    var orders = await _context.Orders.Where(o => o.CustomerId == customer.Id).ToListAsync();
}
```

❌ **Don't use .Result or .Wait()** (deadlock risk)
```csharp
var customers = _context.Customers.ToListAsync().Result; // BAD
```

---

## 🏗️ Common Patterns for WinForms

### Using DbContext with DI

```csharp
// Program.cs - Register DbContext with scoped lifetime
services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Repository - Inject DbContext
public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Customer>> GetAllAsync()
    {
        return await _context.Customers.AsNoTracking().ToListAsync();
    }
}

// Form - Inject Repository (not DbContext directly)
public partial class CustomerForm : Form
{
    private readonly ICustomerRepository _repository;

    public CustomerForm(ICustomerRepository repository)
    {
        _repository = repository;
        InitializeComponent();
    }

    private async void CustomerForm_Load(object sender, EventArgs e)
    {
        var customers = await _repository.GetAllAsync();
        dgvCustomers.DataSource = customers;
    }
}
```

---

### Unit of Work Pattern

Coordinates multiple repositories with a single transaction.

```csharp
// IUnitOfWork.cs
public interface IUnitOfWork : IDisposable
{
    ICustomerRepository Customers { get; }
    IOrderRepository Orders { get; }
    IProductRepository Products { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

// UnitOfWork.cs
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Customers = new CustomerRepository(_context);
        Orders = new OrderRepository(_context);
        Products = new ProductRepository(_context);
    }

    public ICustomerRepository Customers { get; }
    public IOrderRepository Orders { get; }
    public IProductRepository Products { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

// Usage in Service
public class OrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> CreateOrderAsync(Order order)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Add order
            await _unitOfWork.Orders.AddAsync(order);

            // Update product stock
            foreach (var item in order.OrderItems)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product == null || product.StockQuantity < item.Quantity)
                    throw new InvalidOperationException("Insufficient stock");

                product.StockQuantity -= item.Quantity;
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            return false;
        }
    }
}
```

---

### DbContext per Form/Operation

Create and dispose DbContext for each operation (alternative to DI scoped).

```csharp
// DbContextFactory
public interface IDbContextFactory
{
    AppDbContext CreateDbContext();
}

public class AppDbContextFactory : IDbContextFactory
{
    private readonly DbContextOptions<AppDbContext> _options;

    public AppDbContextFactory(DbContextOptions<AppDbContext> options)
    {
        _options = options;
    }

    public AppDbContext CreateDbContext()
    {
        return new AppDbContext(_options);
    }
}

// Register in DI
services.AddSingleton<IDbContextFactory, AppDbContextFactory>();

// Usage in Form
public partial class CustomerForm : Form
{
    private readonly IDbContextFactory _contextFactory;

    public CustomerForm(IDbContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
        InitializeComponent();
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        using (var context = _contextFactory.CreateDbContext())
        {
            var customer = new Customer { Name = txtName.Text };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();
        }
    }

    private async void btnLoad_Click(object sender, EventArgs e)
    {
        using (var context = _contextFactory.CreateDbContext())
        {
            var customers = await context.Customers.AsNoTracking().ToListAsync();
            dgvCustomers.DataSource = customers;
        }
    }
}
```

---

## 🚨 Error Handling

### Common EF Exceptions

```csharp
public async Task<bool> SaveCustomerAsync(Customer customer)
{
    try
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return true;
    }
    catch (DbUpdateException ex)
    {
        // Database update error (constraint violation, etc.)
        _logger.LogError(ex, "Database update failed");

        if (ex.InnerException is SqlException sqlEx)
        {
            switch (sqlEx.Number)
            {
                case 2627: // Unique constraint violation
                case 2601:
                    MessageBox.Show("Customer with this email already exists.");
                    break;
                case 547: // Foreign key violation
                    MessageBox.Show("Cannot delete customer with existing orders.");
                    break;
                default:
                    MessageBox.Show($"Database error: {sqlEx.Message}");
                    break;
            }
        }

        return false;
    }
    catch (DbUpdateConcurrencyException ex)
    {
        // Optimistic concurrency conflict
        _logger.LogError(ex, "Concurrency conflict");
        MessageBox.Show("This record was modified by another user. Please reload and try again.");
        return false;
    }
    catch (SqlException ex)
    {
        // SQL Server connection/timeout errors
        _logger.LogError(ex, "SQL error");
        MessageBox.Show("Database connection error. Please try again.");
        return false;
    }
    catch (Exception ex)
    {
        // Unexpected error
        _logger.LogError(ex, "Unexpected error saving customer");
        MessageBox.Show("An unexpected error occurred.");
        return false;
    }
}
```

---

### Concurrency Handling

Prevent data loss when multiple users edit the same record.

```csharp
// Entity with RowVersion
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    [Timestamp]
    public byte[]? RowVersion { get; set; } // Concurrency token
}

// Update with concurrency handling
public async Task<bool> UpdateCustomerAsync(Customer customer)
{
    try
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
        return true;
    }
    catch (DbUpdateConcurrencyException ex)
    {
        var entry = ex.Entries.Single();
        var databaseValues = await entry.GetDatabaseValuesAsync();

        if (databaseValues == null)
        {
            // Record was deleted
            MessageBox.Show("This customer was deleted by another user.");
            return false;
        }

        var databaseCustomer = (Customer)databaseValues.ToObject();

        // Show conflict resolution dialog
        var result = MessageBox.Show(
            $"This customer was modified by another user.\n\n" +
            $"Current Name: {databaseCustomer.Name}\n" +
            $"Your Name: {customer.Name}\n\n" +
            $"Overwrite changes?",
            "Concurrency Conflict",
            MessageBoxButtons.YesNo);

        if (result == DialogResult.Yes)
        {
            // Overwrite database values
            entry.OriginalValues.SetValues(databaseValues);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
}
```

---

## 🧪 Testing with EF Core

### In-Memory Database

```csharp
// Install: dotnet add package Microsoft.EntityFrameworkCore.InMemory

[Fact]
public async Task AddCustomer_ValidCustomer_ReturnsId()
{
    // Arrange
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDb")
        .Options;

    using (var context = new AppDbContext(options))
    {
        var repository = new CustomerRepository(context);
        var customer = new Customer { Name = "Test", Email = "test@example.com" };

        // Act
        var customerId = await repository.AddAsync(customer);

        // Assert
        Assert.True(customerId > 0);
        Assert.Equal(1, await context.Customers.CountAsync());
    }
}
```

**Limitations:**
- Doesn't enforce all database constraints
- No SQL Server-specific features
- Different behavior than real database

---

### SQLite In-Memory (Better Alternative)

```csharp
// Install: dotnet add package Microsoft.EntityFrameworkCore.Sqlite

[Fact]
public async Task AddCustomer_ValidCustomer_ReturnsId()
{
    // Arrange
    var connection = new SqliteConnection("DataSource=:memory:");
    connection.Open();

    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlite(connection)
        .Options;

    using (var context = new AppDbContext(options))
    {
        await context.Database.EnsureCreatedAsync();

        var repository = new CustomerRepository(context);
        var customer = new Customer { Name = "Test", Email = "test@example.com" };

        // Act
        var customerId = await repository.AddAsync(customer);

        // Assert
        Assert.True(customerId > 0);
    }

    connection.Close();
}
```

**Advantages over InMemory:**
- Enforces constraints (unique, foreign key)
- Closer to real database behavior
- Supports transactions

---

## 📦 Complete Working Example

```csharp
// Complete WinForms CRUD with EF Core

// Models/Customer.cs
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [Timestamp]
    public byte[]? RowVersion { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

// Repository/CustomerRepository.cs
public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(int id);
    Task<int> AddAsync(Customer customer);
    Task<bool> UpdateAsync(Customer customer);
    Task<bool> DeleteAsync(int id);
}

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Customer>> GetAllAsync()
    {
        return await _context.Customers
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await _context.Customers
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<int> AddAsync(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return customer.Id;
    }

    public async Task<bool> UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
            return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return true;
    }
}

// Forms/CustomerForm.cs
public partial class CustomerForm : Form
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<CustomerForm> _logger;

    public CustomerForm(ICustomerRepository repository, ILogger<CustomerForm> logger)
    {
        _repository = repository;
        _logger = logger;
        InitializeComponent();
    }

    private async void CustomerForm_Load(object sender, EventArgs e)
    {
        await LoadCustomersAsync();
    }

    private async Task LoadCustomersAsync()
    {
        try
        {
            prgLoading.Visible = true;
            var customers = await _repository.GetAllAsync();
            dgvCustomers.DataSource = customers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading customers");
            MessageBox.Show("Error loading customers", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            prgLoading.Visible = false;
        }
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        if (!ValidateInput())
            return;

        try
        {
            var customer = new Customer
            {
                Name = txtName.Text.Trim(),
                Email = txtEmail.Text.Trim()
            };

            var id = await _repository.AddAsync(customer);
            _logger.LogInformation("Customer created: {Id}", id);

            MessageBox.Show("Customer saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadCustomersAsync();
            ClearForm();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
        {
            MessageBox.Show("A customer with this email already exists.", "Duplicate Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving customer");
            MessageBox.Show("Error saving customer", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnDelete_Click(object sender, EventArgs e)
    {
        if (dgvCustomers.CurrentRow == null)
            return;

        var customer = (Customer)dgvCustomers.CurrentRow.DataBoundItem;

        var result = MessageBox.Show(
            $"Delete customer '{customer.Name}'?",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            try
            {
                await _repository.DeleteAsync(customer.Id);
                _logger.LogInformation("Customer deleted: {Id}", customer.Id);
                await LoadCustomersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer");
                MessageBox.Show("Error deleting customer", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            MessageBox.Show("Name is required", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtEmail.Text))
        {
            MessageBox.Show("Email is required", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        return true;
    }

    private void ClearForm()
    {
        txtName.Clear();
        txtEmail.Clear();
        txtName.Focus();
    }
}
```

---

## 🔧 Troubleshooting

### Issue: DbContext disposed too early
**Solution:** Use scoped lifetime or create new context per operation

### Issue: N+1 query problem (slow performance)
**Solution:** Use `.Include()` for eager loading

### Issue: Concurrency exception
**Solution:** Add `[Timestamp]` property and handle conflicts

### Issue: Connection string not found
**Solution:** Verify appsettings.json exists and is copied to output

### Issue: Migrations not applying
**Solution:** Run `dotnet ef database update` or call `context.Database.Migrate()`

### Issue: Entity not tracked
**Solution:** Don't use `AsNoTracking()` if you need to update the entity

---

## 🔄 Migration from EF6

### Key Differences:

| EF6 | EF Core 8 |
|-----|-----------|
| `DbSet.Find()` | `DbSet.FindAsync()` (async preferred) |
| `DbSet.Add()` returns void | `DbSet.Add()` returns EntityEntry |
| Lazy loading by default | Lazy loading opt-in via proxies |
| `Database.Log` | `LogTo()` method |
| Complex types | Owned entities |

### Migration Steps:
1. Update packages to EF Core
2. Replace `DbSet.Find()` with `FindAsync()`
3. Enable lazy loading explicitly if needed
4. Update configuration from `DbModelBuilder` to `ModelBuilder`
5. Test thoroughly (behavior differences exist)

---

## 🔗 Related Topics

- [Dependency Injection](../architecture/dependency-injection.md) - DI setup for DbContext
- [Async/Await Pattern](../best-practices/async-await.md) - Async database operations
- [Repository Pattern](repository-pattern.md) - Abstracting data access
- [Unit Testing](../testing/unit-testing.md) - Testing with EF Core
- [Error Handling](../best-practices/error-handling.md) - Exception handling strategies

---

**Last Updated**: 2025-11-07
