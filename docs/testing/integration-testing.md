# Integration Testing in WinForms

**Last Updated**: 2025-11-07
**Applies To**: .NET 8.0, .NET Framework 4.8, EF Core 8.0
**Difficulty**: Intermediate

---

## 📋 Overview

Integration testing verifies that multiple components work together correctly, particularly focusing on data access layer interactions with actual databases. Unlike unit tests that mock dependencies, integration tests use real database connections to validate repository implementations, EF Core queries, and database constraints.

**What You'll Learn**:
- Differences between unit and integration tests
- Setting up test databases (in-memory, Docker, dedicated DB)
- Testing repository CRUD operations
- Testing EF Core queries and relationships
- Managing transactions and concurrency
- Best practices for reliable integration tests

---

## 🎯 Why This Matters

Integration tests are critical for:

1. **Validating Data Access Logic**: Ensure repositories correctly interact with databases
2. **Testing Complex Queries**: LINQ queries may behave differently with real databases
3. **Verifying Constraints**: Foreign keys, unique constraints, and triggers
4. **Catching Migration Issues**: Detect schema/code mismatches early
5. **Database-Specific Behavior**: SQL Server vs SQLite vs PostgreSQL differences
6. **Preventing Production Bugs**: Find issues before they reach users

**Real-World Impact**:
```
❌ Without integration tests: A typo in a LINQ query passes unit tests but
   throws exceptions in production when hitting the real database.

✅ With integration tests: The same error is caught immediately during the test run,
   saving hours of debugging and preventing customer impact.
```

---

## 🔄 Integration Tests vs Unit Tests

### Differences

| Aspect | Unit Tests | Integration Tests |
|--------|-----------|-------------------|
| **Scope** | Single class/method | Multiple components (Repository + DB) |
| **Speed** | Fast (milliseconds) | Slower (seconds) |
| **Dependencies** | Mocked/faked | Real (database, file system) |
| **Isolation** | Complete isolation | Tests interact with external systems |
| **Purpose** | Verify business logic | Verify component integration |
| **Frequency** | Run constantly | Run before commits/in CI |
| **Maintenance** | Low | Medium (database setup required) |
| **Deterministic** | Always | Depends on database state |

### When to Use Each

**✅ Use Unit Tests For**:
```csharp
// Business logic, calculations, validation rules
public class OrderService
{
    // Unit test: Mock IOrderRepository
    public decimal CalculateTotal(Order order)
    {
        decimal subtotal = order.Items.Sum(i => i.Price * i.Quantity);
        decimal tax = subtotal * order.TaxRate;
        return subtotal + tax;
    }
}
```

**✅ Use Integration Tests For**:
```csharp
// Database operations, EF Core queries
public class OrderRepository : IOrderRepository
{
    // Integration test: Use real database
    public async Task<List<Order>> GetOrdersWithItemsAsync()
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.Status == OrderStatus.Pending)
            .ToListAsync();
    }
}
```

**Rule of Thumb**:
- 70-80% unit tests (fast feedback loop)
- 20-30% integration tests (catch real-world issues)
- <5% UI/end-to-end tests (slowest, most brittle)

---

## 📦 What to Integration Test

### Repository Implementations

**✅ Test CRUD Operations**:
```csharp
[Fact]
public async Task AddAsync_ShouldPersistCustomerToDatabase()
{
    // Arrange
    var customer = new Customer
    {
        Name = "John Doe",
        Email = "john@example.com"
    };

    // Act
    await _repository.AddAsync(customer);

    // Assert: Verify in database
    var saved = await _context.Customers.FindAsync(customer.Id);
    Assert.NotNull(saved);
    Assert.Equal("John Doe", saved.Name);
}
```

**✅ Test Custom Queries**:
```csharp
[Fact]
public async Task GetActiveCustomersAsync_ShouldReturnOnlyActiveCustomers()
{
    // Arrange: Seed data
    _context.Customers.AddRange(
        new Customer { Name = "Active 1", IsActive = true },
        new Customer { Name = "Inactive", IsActive = false },
        new Customer { Name = "Active 2", IsActive = true }
    );
    await _context.SaveChangesAsync();

    // Act
    var result = await _repository.GetActiveCustomersAsync();

    // Assert
    Assert.Equal(2, result.Count);
    Assert.All(result, c => Assert.True(c.IsActive));
}
```

### EF Core DbContext

**✅ Test Relationships**:
```csharp
[Fact]
public async Task Customer_ShouldLoadOrdersWithInclude()
{
    // Arrange
    var customer = new Customer { Name = "Test" };
    customer.Orders.Add(new Order { OrderDate = DateTime.Now });
    _context.Customers.Add(customer);
    await _context.SaveChangesAsync();

    _context.ChangeTracker.Clear(); // Detach entities

    // Act
    var loaded = await _context.Customers
        .Include(c => c.Orders)
        .FirstOrDefaultAsync(c => c.Id == customer.Id);

    // Assert
    Assert.NotNull(loaded);
    Assert.Single(loaded.Orders);
}
```

### Database Constraints

**✅ Test Foreign Key Constraints**:
```csharp
[Fact]
public async Task DeleteCustomer_WithOrders_ShouldThrowException()
{
    // Arrange
    var customer = new Customer { Name = "Test" };
    customer.Orders.Add(new Order { OrderDate = DateTime.Now });
    _context.Customers.Add(customer);
    await _context.SaveChangesAsync();

    // Act & Assert
    _context.Customers.Remove(customer);
    await Assert.ThrowsAsync<DbUpdateException>(
        async () => await _context.SaveChangesAsync()
    );
}
```

---

## 🗄️ Test Database Strategies

### Strategy 1: SQLite In-Memory Database (RECOMMENDED)

**Best for**: Fast, isolated tests without external dependencies.

**Setup**:
```csharp
public class TestDatabaseFixture : IDisposable
{
    public ApplicationDbContext Context { get; private set; }
    private SqliteConnection _connection;

    public TestDatabaseFixture()
    {
        // IMPORTANT: Use "DataSource=:memory:" with connection kept open
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open(); // Keep connection open for in-memory DB

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        Context = new ApplicationDbContext(options);
        Context.Database.EnsureCreated(); // Create schema
    }

    public void Dispose()
    {
        Context?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}
```

**✅ Pros**:
- Extremely fast (runs in memory)
- No external dependencies
- Isolated per test class
- Works in CI/CD without setup

**❌ Cons**:
- SQLite differs from SQL Server (limited features)
- Some SQL Server-specific features won't work
- Doesn't test stored procedures/triggers

### Strategy 2: Dedicated Test Database

**Best for**: Testing SQL Server-specific features.

**Setup**:
```csharp
public class SqlServerTestFixture : IDisposable
{
    public ApplicationDbContext Context { get; private set; }
    private const string ConnectionString =
        "Server=(localdb)\\mssqllocaldb;Database=TestDb_Integration;Trusted_Connection=True;";

    public SqlServerTestFixture()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        Context = new ApplicationDbContext(options);
        Context.Database.EnsureDeleted(); // Clean slate
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context?.Database.EnsureDeleted(); // Cleanup
        Context?.Dispose();
    }
}
```

**✅ Pros**:
- Tests real SQL Server behavior
- Supports stored procedures, triggers, full-text search
- Matches production environment

**❌ Cons**:
- Slower than in-memory
- Requires SQL Server installed
- Can have state pollution between tests

### Strategy 3: Docker Containers (BEST FOR CI/CD)

**Best for**: Production-like testing in CI/CD pipelines.

**Setup with Testcontainers**:
```csharp
public class DockerTestFixture : IAsyncLifetime
{
    private MsSqlContainer _container;
    public ApplicationDbContext Context { get; private set; }

    public async Task InitializeAsync()
    {
        _container = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .Build();

        await _container.StartAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(_container.GetConnectionString())
            .Options;

        Context = new ApplicationDbContext(options);
        await Context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await Context.DisposeAsync();
        await _container.StopAsync();
    }
}
```

**✅ Pros**:
- Real SQL Server in isolated container
- Clean database per test run
- Excellent for CI/CD
- No local SQL Server required

**❌ Cons**:
- Requires Docker installed
- Slower startup time
- More complex setup

---

## ⚙️ Setting Up Integration Tests

### Complete SQLite In-Memory Setup

**1. Install NuGet Packages**:
```xml
<ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.0" />
    <PackageReference Include="xunit" Version="2.6.0" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.0" />
</ItemGroup>
```

**2. Create Test Base Class**:
```csharp
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;

namespace MyApp.Tests.Integration
{
    /// <summary>
    /// Base class for integration tests using SQLite in-memory database.
    /// </summary>
    public abstract class IntegrationTestBase : IDisposable
    {
        protected ApplicationDbContext Context { get; private set; }
        private readonly SqliteConnection _connection;

        protected IntegrationTestBase()
        {
            // Create and open connection (must stay open for in-memory DB)
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // Configure DbContext
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .EnableSensitiveDataLogging() // Helpful for debugging
                .Options;

            Context = new ApplicationDbContext(options);
            Context.Database.EnsureCreated();

            // Seed base data if needed
            SeedTestData();
        }

        /// <summary>
        /// Override to seed test data in derived classes.
        /// </summary>
        protected virtual void SeedTestData()
        {
            // Base implementation - override in test classes
        }

        public void Dispose()
        {
            Context?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
```

**3. Create Repository Test Class**:
```csharp
using Xunit;
using System.Threading.Tasks;

namespace MyApp.Tests.Integration.Repositories
{
    public class CustomerRepositoryTests : IntegrationTestBase
    {
        private readonly CustomerRepository _repository;

        public CustomerRepositoryTests()
        {
            _repository = new CustomerRepository(Context);
        }

        protected override void SeedTestData()
        {
            // Seed data specific to customer tests
            Context.Customers.AddRange(
                new Customer { Name = "Alice", Email = "alice@example.com" },
                new Customer { Name = "Bob", Email = "bob@example.com" }
            );
            Context.SaveChanges();
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsCustomer()
        {
            // Arrange
            var expectedCustomer = Context.Customers.First();

            // Act
            var result = await _repository.GetByIdAsync(expectedCustomer.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCustomer.Name, result.Name);
        }
    }
}
```

### Database Initialization Strategies

**Option 1: EnsureCreated() - Simple**:
```csharp
Context.Database.EnsureCreated(); // Creates schema from model
```

**Option 2: Migrations - Production-like**:
```csharp
Context.Database.Migrate(); // Applies all pending migrations
```

**Recommendation**: Use `EnsureCreated()` for integration tests (faster), `Migrate()` for staging/production.

---

## 🧪 Testing Repository Methods

### Testing Create/Add Operations

```csharp
[Fact]
public async Task AddAsync_ValidCustomer_ShouldPersistToDatabase()
{
    // Arrange
    var customer = new Customer
    {
        Name = "New Customer",
        Email = "new@example.com",
        PhoneNumber = "555-1234",
        CreatedDate = DateTime.UtcNow
    };

    // Act
    await _repository.AddAsync(customer);

    // Assert: Verify ID was generated
    Assert.NotEqual(0, customer.Id);

    // Assert: Verify persisted to database
    var saved = await Context.Customers.FindAsync(customer.Id);
    Assert.NotNull(saved);
    Assert.Equal("New Customer", saved.Name);
    Assert.Equal("new@example.com", saved.Email);
}
```

### Testing Read/Query Operations

```csharp
[Fact]
public async Task GetAllAsync_ShouldReturnAllCustomers()
{
    // Arrange: Data seeded in SeedTestData()

    // Act
    var result = await _repository.GetAllAsync();

    // Assert
    Assert.NotEmpty(result);
    Assert.Equal(2, result.Count); // Alice and Bob from seed data
}

[Fact]
public async Task GetByEmailAsync_ExistingEmail_ReturnsCustomer()
{
    // Arrange
    const string email = "alice@example.com";

    // Act
    var result = await _repository.GetByEmailAsync(email);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(email, result.Email);
    Assert.Equal("Alice", result.Name);
}

[Fact]
public async Task GetByEmailAsync_NonExistentEmail_ReturnsNull()
{
    // Act
    var result = await _repository.GetByEmailAsync("nonexistent@example.com");

    // Assert
    Assert.Null(result);
}
```

### Testing Update Operations

```csharp
[Fact]
public async Task UpdateAsync_ExistingCustomer_ShouldPersistChanges()
{
    // Arrange: Get existing customer
    var customer = await Context.Customers.FirstAsync();
    var originalName = customer.Name;
    var customerId = customer.Id;

    // Act: Update customer
    customer.Name = "Updated Name";
    customer.Email = "updated@example.com";
    await _repository.UpdateAsync(customer);

    // Assert: Clear change tracker to force fresh query
    Context.ChangeTracker.Clear();

    // Assert: Verify changes persisted
    var updated = await Context.Customers.FindAsync(customerId);
    Assert.NotNull(updated);
    Assert.Equal("Updated Name", updated.Name);
    Assert.Equal("updated@example.com", updated.Email);
    Assert.NotEqual(originalName, updated.Name);
}
```

### Testing Delete Operations

```csharp
[Fact]
public async Task DeleteAsync_ExistingCustomer_ShouldRemoveFromDatabase()
{
    // Arrange
    var customer = await Context.Customers.FirstAsync();
    var customerId = customer.Id;

    // Act
    await _repository.DeleteAsync(customerId);

    // Assert: Verify removed from database
    var deleted = await Context.Customers.FindAsync(customerId);
    Assert.Null(deleted);
}

[Fact]
public async Task DeleteAsync_NonExistentId_ShouldNotThrowException()
{
    // Act & Assert: Should not throw
    await _repository.DeleteAsync(99999);

    // Verify database unchanged
    Assert.Equal(2, await Context.Customers.CountAsync());
}
```

---

## 🔍 Testing EF Core Queries

### Testing Complex LINQ Queries

```csharp
[Fact]
public async Task GetCustomersWithRecentOrders_ShouldReturnCorrectCustomers()
{
    // Arrange
    var customer1 = new Customer { Name = "Customer 1" };
    var customer2 = new Customer { Name = "Customer 2" };

    customer1.Orders.Add(new Order
    {
        OrderDate = DateTime.UtcNow.AddDays(-5),
        Total = 100
    });

    customer2.Orders.Add(new Order
    {
        OrderDate = DateTime.UtcNow.AddDays(-40),
        Total = 50
    }); // Old order

    Context.Customers.AddRange(customer1, customer2);
    await Context.SaveChangesAsync();

    // Act: Get customers with orders in last 30 days
    var result = await _repository.GetCustomersWithRecentOrdersAsync(days: 30);

    // Assert
    Assert.Single(result);
    Assert.Equal("Customer 1", result.First().Name);
}
```

### Testing Include/ThenInclude (Eager Loading)

```csharp
[Fact]
public async Task GetOrderWithDetails_ShouldLoadCustomerAndItems()
{
    // Arrange
    var customer = new Customer { Name = "Test Customer" };
    var order = new Order
    {
        Customer = customer,
        OrderDate = DateTime.UtcNow
    };
    order.Items.Add(new OrderItem { ProductName = "Product 1", Quantity = 2 });
    order.Items.Add(new OrderItem { ProductName = "Product 2", Quantity = 1 });

    Context.Orders.Add(order);
    await Context.SaveChangesAsync();
    Context.ChangeTracker.Clear(); // Detach all entities

    // Act
    var result = await Context.Orders
        .Include(o => o.Customer)
        .Include(o => o.Items)
        .FirstAsync(o => o.Id == order.Id);

    // Assert: Verify navigation properties loaded
    Assert.NotNull(result.Customer);
    Assert.Equal("Test Customer", result.Customer.Name);
    Assert.Equal(2, result.Items.Count);
    Assert.Contains(result.Items, i => i.ProductName == "Product 1");
}
```

### Testing Async Methods

```csharp
[Fact]
public async Task GetTopCustomersAsync_ShouldReturnCustomersOrderedByTotalSpent()
{
    // Arrange
    var customer1 = new Customer { Name = "Big Spender" };
    customer1.Orders.Add(new Order { Total = 1000 });

    var customer2 = new Customer { Name = "Small Spender" };
    customer2.Orders.Add(new Order { Total = 100 });

    Context.Customers.AddRange(customer1, customer2);
    await Context.SaveChangesAsync();

    // Act
    var result = await _repository.GetTopCustomersAsync(limit: 10);

    // Assert
    Assert.Equal(2, result.Count);
    Assert.Equal("Big Spender", result.First().Name);
    Assert.Equal("Small Spender", result.Last().Name);
}
```

---

## 💾 Transaction Management in Tests

### Transaction Rollback Pattern

**Purpose**: Keep database clean between tests without recreating schema.

```csharp
public class TransactionTestBase : IDisposable
{
    protected ApplicationDbContext Context { get; private set; }
    private readonly SqliteConnection _connection;
    private IDbContextTransaction _transaction;

    public TransactionTestBase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        Context = new ApplicationDbContext(options);
        Context.Database.EnsureCreated();

        // Begin transaction - will rollback in Dispose
        _transaction = Context.Database.BeginTransaction();
    }

    public void Dispose()
    {
        _transaction?.Rollback(); // Undo all changes
        _transaction?.Dispose();
        Context?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}
```

**❌ Limitation**: SQLite in-memory doesn't support `BeginTransaction()` for rollback. Use this pattern with real SQL Server databases.

### Database Reset Between Tests

**For SQLite In-Memory**:
```csharp
protected void ResetDatabase()
{
    // Delete all data
    Context.RemoveRange(Context.Orders);
    Context.RemoveRange(Context.Customers);
    Context.SaveChanges();

    // Optionally reseed base data
    SeedTestData();
}

[Fact]
public async Task Test1()
{
    // Test logic

    ResetDatabase(); // Clean for next test
}
```

---

## 🔒 Testing Database Constraints

### Foreign Key Constraints

```csharp
[Fact]
public async Task DeleteCustomer_WithOrders_ShouldFailDueToForeignKey()
{
    // Arrange
    var customer = new Customer { Name = "Test" };
    customer.Orders.Add(new Order { OrderDate = DateTime.UtcNow });
    Context.Customers.Add(customer);
    await Context.SaveChangesAsync();

    // Act & Assert
    Context.Customers.Remove(customer);

    var exception = await Assert.ThrowsAsync<DbUpdateException>(
        () => Context.SaveChangesAsync()
    );

    Assert.Contains("FOREIGN KEY constraint", exception.InnerException?.Message);
}

[Fact]
public async Task DeleteCustomer_WithCascadeDelete_ShouldDeleteOrders()
{
    // Assumes cascade delete configured in OnModelCreating:
    // modelBuilder.Entity<Order>()
    //     .HasOne(o => o.Customer)
    //     .WithMany(c => c.Orders)
    //     .OnDelete(DeleteBehavior.Cascade);

    // Arrange
    var customer = new Customer { Name = "Test" };
    customer.Orders.Add(new Order { OrderDate = DateTime.UtcNow });
    Context.Customers.Add(customer);
    await Context.SaveChangesAsync();
    var orderId = customer.Orders.First().Id;

    // Act
    Context.Customers.Remove(customer);
    await Context.SaveChangesAsync();

    // Assert: Order should be deleted too
    var order = await Context.Orders.FindAsync(orderId);
    Assert.Null(order);
}
```

### Unique Constraints

```csharp
[Fact]
public async Task AddCustomer_DuplicateEmail_ShouldThrowException()
{
    // Assumes unique index on Email:
    // modelBuilder.Entity<Customer>()
    //     .HasIndex(c => c.Email)
    //     .IsUnique();

    // Arrange
    Context.Customers.Add(new Customer
    {
        Name = "User 1",
        Email = "duplicate@example.com"
    });
    await Context.SaveChangesAsync();

    // Act & Assert
    Context.Customers.Add(new Customer
    {
        Name = "User 2",
        Email = "duplicate@example.com"
    });

    await Assert.ThrowsAsync<DbUpdateException>(
        () => Context.SaveChangesAsync()
    );
}
```

---

## ⚡ Testing Concurrency

### Optimistic Concurrency

```csharp
[Fact]
public async Task UpdateCustomer_ConcurrentUpdate_ShouldThrowConcurrencyException()
{
    // Arrange: Customer entity with RowVersion
    // [Timestamp]
    // public byte[] RowVersion { get; set; }

    var customer = new Customer { Name = "Original" };
    Context.Customers.Add(customer);
    await Context.SaveChangesAsync();

    // Simulate two concurrent contexts
    var options = Context.Database.GetDbContextOptions<ApplicationDbContext>();
    using var context1 = new ApplicationDbContext(options);
    using var context2 = new ApplicationDbContext(options);

    var customer1 = await context1.Customers.FindAsync(customer.Id);
    var customer2 = await context2.Customers.FindAsync(customer.Id);

    // Act: Both update
    customer1.Name = "Updated by User 1";
    await context1.SaveChangesAsync(); // First update succeeds

    customer2.Name = "Updated by User 2";

    // Assert: Second update should fail
    await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
        () => context2.SaveChangesAsync()
    );
}
```

---

## ✅ Best Practices

### ✅ DO:

1. **Use Isolated Test Databases**:
```csharp
// ✅ GOOD: Each test class gets own database
public class CustomerTests : IClassFixture<DatabaseFixture>
{
    private readonly ApplicationDbContext _context;

    public CustomerTests(DatabaseFixture fixture)
    {
        _context = fixture.CreateContext();
    }
}
```

2. **Clear Change Tracker Between Operations**:
```csharp
// ✅ GOOD: Ensure fresh query
Context.ChangeTracker.Clear();
var customer = await Context.Customers.FindAsync(id);
```

3. **Test Both Success and Failure Paths**:
```csharp
// ✅ GOOD: Test validation
[Fact]
public async Task AddCustomer_NullEmail_ShouldThrowException()
{
    var customer = new Customer { Name = "Test", Email = null };
    await Assert.ThrowsAsync<DbUpdateException>(
        () => _repository.AddAsync(customer)
    );
}
```

4. **Use Descriptive Test Data**:
```csharp
// ✅ GOOD: Clear intent
var activeCustomer = new Customer { Name = "Active Customer", IsActive = true };
var inactiveCustomer = new Customer { Name = "Inactive Customer", IsActive = false };
```

5. **Test Relationships and Navigation Properties**:
```csharp
// ✅ GOOD: Verify eager loading works
var order = await Context.Orders.Include(o => o.Customer).FirstAsync();
Assert.NotNull(order.Customer);
```

6. **Use Async Methods Consistently**:
```csharp
// ✅ GOOD: All async
[Fact]
public async Task GetAllAsync_ShouldReturnCustomers()
{
    var result = await _repository.GetAllAsync();
    Assert.NotEmpty(result);
}
```

7. **Clean Up Resources**:
```csharp
// ✅ GOOD: Implement IDisposable
public void Dispose()
{
    Context?.Dispose();
    _connection?.Dispose();
}
```

8. **Test Pagination**:
```csharp
// ✅ GOOD: Test skip/take logic
[Fact]
public async Task GetCustomersPaged_ShouldReturnCorrectPage()
{
    // Seed 50 customers
    for (int i = 1; i <= 50; i++)
        Context.Customers.Add(new Customer { Name = $"Customer {i}" });
    await Context.SaveChangesAsync();

    var result = await _repository.GetPagedAsync(page: 2, pageSize: 10);

    Assert.Equal(10, result.Count);
    Assert.Equal("Customer 11", result.First().Name);
}
```

9. **Test Filtering and Sorting**:
```csharp
// ✅ GOOD: Verify query logic
[Fact]
public async Task GetCustomers_FilterByActiveAndSortByName_ShouldWork()
{
    var result = await _repository.GetCustomersAsync(
        isActive: true,
        sortBy: "Name"
    );

    Assert.All(result, c => Assert.True(c.IsActive));
    Assert.Equal(result.OrderBy(c => c.Name).ToList(), result);
}
```

10. **Test Transactions**:
```csharp
// ✅ GOOD: Verify rollback on error
[Fact]
public async Task ProcessOrder_FailureInMiddle_ShouldRollback()
{
    using var transaction = await Context.Database.BeginTransactionAsync();

    try
    {
        await _orderService.CreateOrderAsync(order);
        throw new Exception("Simulated error");
        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
    }

    // Assert: No order created
    Assert.Empty(await Context.Orders.ToListAsync());
}
```

11. **Use Test Data Builders**:
```csharp
// ✅ GOOD: Reusable builders
public class CustomerBuilder
{
    private string _name = "Default Name";
    private string _email = "default@example.com";

    public CustomerBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public CustomerBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public Customer Build() => new Customer { Name = _name, Email = _email };
}

// Usage
var customer = new CustomerBuilder()
    .WithName("John Doe")
    .WithEmail("john@example.com")
    .Build();
```

12. **Test Database Migrations**:
```csharp
// ✅ GOOD: Ensure migrations apply cleanly
[Fact]
public void Migrations_ShouldApplyWithoutErrors()
{
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseSqlServer("your-connection-string")
        .Options;

    using var context = new ApplicationDbContext(options);

    // Should not throw
    context.Database.Migrate();
}
```

### ❌ DON'T:

1. **Don't Use Shared Database State**:
```csharp
// ❌ BAD: Tests interfere with each other
private static ApplicationDbContext _sharedContext; // Multiple tests use this

// ✅ GOOD: Isolated context per test
public CustomerTests()
{
    _context = CreateNewContext();
}
```

2. **Don't Forget to Dispose**:
```csharp
// ❌ BAD: Memory leak
var context = new ApplicationDbContext(options);
// ... use context but never dispose

// ✅ GOOD: Proper disposal
using var context = new ApplicationDbContext(options);
```

3. **Don't Test Implementation Details**:
```csharp
// ❌ BAD: Testing EF Core internals
[Fact]
public void Customer_ShouldUseDbSet()
{
    Assert.IsType<DbSet<Customer>>(Context.Customers);
}

// ✅ GOOD: Test behavior
[Fact]
public async Task GetAllCustomers_ShouldReturnAllRecords()
{
    var result = await _repository.GetAllAsync();
    Assert.NotEmpty(result);
}
```

4. **Don't Ignore Async/Await**:
```csharp
// ❌ BAD: Synchronous code in async method
[Fact]
public async Task GetCustomers_ShouldWork()
{
    var result = Context.Customers.ToList(); // Missing await
}

// ✅ GOOD: Proper async
[Fact]
public async Task GetCustomers_ShouldWork()
{
    var result = await Context.Customers.ToListAsync();
}
```

5. **Don't Use Production Database**:
```csharp
// ❌ BAD: Testing against real data
var options = new DbContextOptionsBuilder()
    .UseSqlServer("Server=prod-db;Database=ProductionDB;...")

// ✅ GOOD: Isolated test database
.UseSqlite("DataSource=:memory:")
```

6. **Don't Test Framework Code**:
```csharp
// ❌ BAD: Testing EF Core's Find method
[Fact]
public async Task Find_ShouldWork()
{
    var customer = await Context.Customers.FindAsync(1);
    Assert.NotNull(customer);
}

// ✅ GOOD: Test your repository logic
[Fact]
public async Task GetActiveCustomers_ShouldReturnOnlyActive()
{
    var result = await _repository.GetActiveCustomersAsync();
    Assert.All(result, c => Assert.True(c.IsActive));
}
```

7. **Don't Use Magic Numbers**:
```csharp
// ❌ BAD: Unclear test data
var customer = await Context.Customers.FindAsync(42);

// ✅ GOOD: Explicit test data
var expectedCustomer = Context.Customers.First();
var customer = await Context.Customers.FindAsync(expectedCustomer.Id);
```

8. **Don't Mix Unit and Integration Tests**:
```csharp
// ❌ BAD: Mixing concerns
[Fact]
public async Task Service_ShouldCalculateAndSaveTotal()
{
    // Unit test (calculation) + Integration test (save) in one test
}

// ✅ GOOD: Separate tests
[Fact]
public void CalculateTotal_ShouldReturnCorrectAmount() { } // Unit

[Fact]
public async Task SaveOrder_ShouldPersistToDatabase() { } // Integration
```

9. **Don't Leave Debug Code**:
```csharp
// ❌ BAD: Leftover debugging
[Fact]
public async Task TestMethod()
{
    Console.WriteLine("Debug: Starting test");
    var result = await _repository.GetAllAsync();
    Console.WriteLine($"Debug: Got {result.Count} items");
}

// ✅ GOOD: Clean test
[Fact]
public async Task GetAll_ShouldReturnAllCustomers()
{
    var result = await _repository.GetAllAsync();
    Assert.NotEmpty(result);
}
```

10. **Don't Skip Cleanup**:
```csharp
// ❌ BAD: No cleanup
[Fact]
public async Task TestMethod()
{
    var context = CreateContext();
    // ... test logic
    // Context never disposed
}

// ✅ GOOD: Cleanup in base class or using statement
using var context = CreateContext();
```

---

## 🎯 Complete Working Example

### CustomerRepository Integration Tests

```csharp
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Models;
using MyApp.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MyApp.Tests.Integration.Repositories
{
    /// <summary>
    /// Integration tests for CustomerRepository.
    /// Uses SQLite in-memory database for fast, isolated testing.
    /// </summary>
    public class CustomerRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CustomerRepository _repository;
        private readonly SqliteConnection _connection;

        public CustomerRepositoryTests()
        {
            // Setup in-memory database
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            _repository = new CustomerRepository(_context);

            // Seed test data
            SeedData();
        }

        private void SeedData()
        {
            _context.Customers.AddRange(
                new Customer
                {
                    Name = "Alice Johnson",
                    Email = "alice@example.com",
                    PhoneNumber = "555-0001",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow.AddMonths(-6)
                },
                new Customer
                {
                    Name = "Bob Smith",
                    Email = "bob@example.com",
                    PhoneNumber = "555-0002",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow.AddMonths(-3)
                },
                new Customer
                {
                    Name = "Charlie Brown",
                    Email = "charlie@example.com",
                    PhoneNumber = "555-0003",
                    IsActive = false,
                    CreatedDate = DateTime.UtcNow.AddYears(-1)
                }
            );
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCustomers()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Contains(result, c => c.Name == "Alice Johnson");
            Assert.Contains(result, c => c.Name == "Bob Smith");
            Assert.Contains(result, c => c.Name == "Charlie Brown");
        }

        [Fact]
        public async Task GetByIdAsync_ExistingCustomer_ReturnsCustomer()
        {
            // Arrange
            var expectedCustomer = _context.Customers.First();

            // Act
            var result = await _repository.GetByIdAsync(expectedCustomer.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCustomer.Id, result.Id);
            Assert.Equal(expectedCustomer.Name, result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistentId_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(99999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_ValidCustomer_ShouldPersist()
        {
            // Arrange
            var newCustomer = new Customer
            {
                Name = "David Lee",
                Email = "david@example.com",
                PhoneNumber = "555-0004",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            // Act
            await _repository.AddAsync(newCustomer);

            // Assert: ID should be generated
            Assert.NotEqual(0, newCustomer.Id);

            // Assert: Should exist in database
            _context.ChangeTracker.Clear();
            var saved = await _context.Customers.FindAsync(newCustomer.Id);
            Assert.NotNull(saved);
            Assert.Equal("David Lee", saved.Name);
        }

        [Fact]
        public async Task UpdateAsync_ExistingCustomer_ShouldPersistChanges()
        {
            // Arrange
            var customer = await _context.Customers.FirstAsync();
            var customerId = customer.Id;

            // Act
            customer.Name = "Updated Name";
            customer.Email = "updated@example.com";
            await _repository.UpdateAsync(customer);

            // Assert
            _context.ChangeTracker.Clear();
            var updated = await _context.Customers.FindAsync(customerId);
            Assert.Equal("Updated Name", updated.Name);
            Assert.Equal("updated@example.com", updated.Email);
        }

        [Fact]
        public async Task DeleteAsync_ExistingCustomer_ShouldRemove()
        {
            // Arrange
            var customer = await _context.Customers.FirstAsync();
            var customerId = customer.Id;

            // Act
            await _repository.DeleteAsync(customerId);

            // Assert
            var deleted = await _context.Customers.FindAsync(customerId);
            Assert.Null(deleted);
            Assert.Equal(2, await _context.Customers.CountAsync());
        }

        [Fact]
        public async Task GetActiveCustomersAsync_ShouldReturnOnlyActiveCustomers()
        {
            // Act
            var result = await _repository.GetActiveCustomersAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, c => Assert.True(c.IsActive));
            Assert.DoesNotContain(result, c => c.Name == "Charlie Brown");
        }

        [Fact]
        public async Task GetByEmailAsync_ExistingEmail_ReturnsCustomer()
        {
            // Act
            var result = await _repository.GetByEmailAsync("alice@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Alice Johnson", result.Name);
        }

        [Fact]
        public async Task GetByEmailAsync_NonExistentEmail_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByEmailAsync("nonexistent@example.com");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SearchByNameAsync_PartialMatch_ReturnsMatchingCustomers()
        {
            // Act
            var result = await _repository.SearchByNameAsync("john");

            // Assert
            Assert.Single(result);
            Assert.Equal("Alice Johnson", result.First().Name);
        }

        public void Dispose()
        {
            _context?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
```

---

## 🚀 CI/CD Integration

### GitHub Actions Example

```yaml
name: Integration Tests

on: [push, pull_request]

jobs:
  integration-tests:
    runs-on: ubuntu-latest

    services:
      sqlserver:
        image: mcr.microsoft.com/mssql/server:2022-latest
        env:
          ACCEPT_EULA: Y
          SA_PASSWORD: YourStrong@Passw0rd
        ports:
          - 1433:1433
        options: >-
          --health-cmd "/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -Q 'SELECT 1'"
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5

    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Run integration tests
        run: dotnet test --filter "Category=Integration" --logger "trx;LogFileName=test-results.trx"
        env:
          ConnectionStrings__TestDb: "Server=localhost;Database=TestDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"

      - name: Upload test results
        uses: actions/upload-artifact@v3
        if: always()
        with:
          name: test-results
          path: '**/test-results.trx'
```

---

## 🔧 Troubleshooting

### Common Issues

**Issue**: "SQLite Error: No such table"
```csharp
// ❌ Forgot to create schema
var context = new ApplicationDbContext(options);
// Missing: context.Database.EnsureCreated();

// ✅ Solution
context.Database.EnsureCreated();
```

**Issue**: "Connection closed, in-memory database lost"
```csharp
// ❌ BAD: Connection closed too early
using (var connection = new SqliteConnection("DataSource=:memory:"))
{
    connection.Open();
    var context = new ApplicationDbContext(...);
} // Connection closed here, database lost

// ✅ GOOD: Keep connection open
_connection = new SqliteConnection("DataSource=:memory:");
_connection.Open(); // Keep open for test lifetime
```

**Issue**: "Tests fail intermittently"
```csharp
// Likely cause: Shared state between tests
// ✅ Solution: Ensure test isolation
public class MyTests : IClassFixture<DatabaseFixture>
{
    // Each test gets fresh context
}
```

**Issue**: "DbUpdateConcurrencyException in tests"
```csharp
// ❌ Tracking same entity twice
var customer = await _context.Customers.FindAsync(1);
customer.Name = "Updated";
var sameCustomer = await _context.Customers.FindAsync(1); // Still tracked

// ✅ Solution: Clear change tracker
_context.ChangeTracker.Clear();
```

---

## 📚 Related Topics

- [Unit Testing](unit-testing.md) - Testing business logic in isolation
- [Testing Overview](testing-overview.md) - Overall testing strategy
- [Repository Pattern](../data-access/repository-pattern.md) - Data access abstraction
- [Entity Framework Core](../data-access/entity-framework-core.md) - ORM fundamentals
- [Async/Await](../best-practices/async-await.md) - Asynchronous programming

---

## 📖 Summary

Integration tests are essential for validating database interactions:

- **Use SQLite in-memory** for fast, isolated tests
- **Test real database behavior** with Docker or dedicated test databases
- **Focus on repositories and queries**, not business logic
- **Keep tests isolated** - no shared state
- **Test success and failure paths** - constraints, exceptions, edge cases
- **Run in CI/CD** to catch issues early

**Remember**: Integration tests are slower than unit tests but catch real-world database issues. Aim for 20-30% integration test coverage focused on data access layers.

---

**Next Steps**:
1. Set up integration test project with SQLite
2. Create test base class with database fixture
3. Write tests for all repository CRUD operations
4. Add tests for complex queries and relationships
5. Integrate into CI/CD pipeline

**Happy Testing!** 🧪✅
