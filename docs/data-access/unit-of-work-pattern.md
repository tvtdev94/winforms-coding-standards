# Unit of Work Pattern

## Overview

The **Unit of Work pattern** maintains a list of objects affected by a business transaction and coordinates the writing of changes and the resolution of concurrency problems. It provides a centralized mechanism for managing database transactions and ensures that all changes are committed or rolled back together.

**Key Benefits:**
- ✅ Centralizes transaction management at the service layer
- ✅ Ensures atomic operations across multiple repositories
- ✅ Reduces code duplication (no SaveChanges in repositories)
- ✅ Better separation of concerns
- ✅ Easier to test and mock
- ✅ Supports explicit transactions for complex operations

---

## Architecture

```
┌─────────────────────┐
│   Presentation      │
│   Layer (Forms)     │
└──────────┬──────────┘
           │
           v
┌─────────────────────┐
│   Business Layer    │
│   (Services)        │───► Uses IUnitOfWork
└──────────┬──────────┘
           │
           v
┌─────────────────────┐
│   Unit of Work      │───► Manages Repositories
│                     │───► Controls SaveChanges
│                     │───► Transaction Management
└──────────┬──────────┘
           │
           v
┌─────────────────────┐
│   Repositories      │───► NO SaveChanges
│                     │───► Only track changes
└──────────┬──────────┘
           │
           v
┌─────────────────────┐
│   DbContext         │
│   (EF Core)         │
└─────────────────────┘
```

---

## Implementation

### 1. IUnitOfWork Interface

```csharp
using CustomerManagement.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CustomerManagement.Data
{
    /// <summary>
    /// Unit of Work interface for managing database transactions.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets the Customer repository.
        /// </summary>
        ICustomerRepository Customers { get; }

        /// <summary>
        /// Gets the Order repository.
        /// </summary>
        IOrderRepository Orders { get; }

        /// <summary>
        /// Saves all changes made in this unit of work to the database.
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Begins a new database transaction.
        /// </summary>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Rolls back the current transaction.
        /// </summary>
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
```

### 2. UnitOfWork Implementation

```csharp
using CustomerManagement.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CustomerManagement.Data
{
    /// <summary>
    /// Unit of Work implementation.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed;

        // Lazy-loaded repositories
        private ICustomerRepository? _customerRepository;
        private IOrderRepository? _orderRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc/>
        public ICustomerRepository Customers
        {
            get
            {
                _customerRepository ??= new CustomerRepository(_context);
                return _customerRepository;
            }
        }

        /// <inheritdoc/>
        public IOrderRepository Orders
        {
            get
            {
                _orderRepository ??= new OrderRepository(_context);
                return _orderRepository;
            }
        }

        /// <inheritdoc/>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while saving changes.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }

            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction is in progress.");
            }

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                await _transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <inheritdoc/>
        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction is in progress.");
            }

            try
            {
                await _transaction.RollbackAsync(cancellationToken);
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    // Note: DbContext is managed by DI, disposed by container
                }
                _disposed = true;
            }
        }
    }
}
```

### 3. Repository Without SaveChanges

**❌ Old Way (Repository calls SaveChanges):**
```csharp
public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public async Task<Customer> AddAsync(Customer entity)
    {
        _context.Customers.Add(entity);
        await _context.SaveChangesAsync(); // ❌ BAD: Repository controls saving
        return entity;
    }
}
```

**✅ New Way (Unit of Work controls SaveChanges):**
```csharp
public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public async Task<Customer> AddAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        await _context.Customers.AddAsync(entity, cancellationToken);
        // ✅ GOOD: No SaveChanges - managed by Unit of Work
        return entity;
    }

    public Task UpdateAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        _context.Customers.Update(entity);
        // ✅ GOOD: No SaveChanges
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        _context.Customers.Remove(entity);
        // ✅ GOOD: No SaveChanges
        return Task.CompletedTask;
    }
}
```

### 4. Service Layer Using Unit of Work

**❌ Old Way (Injecting Repository):**
```csharp
public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository; // ❌ BAD

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task CreateCustomerAsync(Customer customer)
    {
        await _customerRepository.AddAsync(customer); // Auto-saves
    }
}
```

**✅ New Way (Injecting Unit of Work):**
```csharp
public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork; // ✅ GOOD
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(IUnitOfWork unitOfWork, ILogger<CustomerService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task CreateCustomerAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate
            ValidateCustomer(customer);

            // Add via repository
            await _unitOfWork.Customers.AddAsync(customer, cancellationToken);

            // Save via Unit of Work
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Customer created: {Id}", customer.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            throw new InvalidOperationException("Failed to create customer.", ex);
        }
    }

    public async Task UpdateCustomerAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check exists
            var existing = await _unitOfWork.Customers.GetByIdAsync(customer.Id, cancellationToken);
            if (existing == null)
                throw new InvalidOperationException($"Customer {customer.Id} not found.");

            // Validate
            ValidateCustomer(customer);

            // Update via repository
            await _unitOfWork.Customers.UpdateAsync(customer, cancellationToken);

            // Save via Unit of Work
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Customer updated: {Id}", customer.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer");
            throw;
        }
    }
}
```

---

## Advanced Usage

### Multiple Operations in One Transaction

```csharp
public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrderService> _logger;

    public async Task CreateOrderWithCustomerAsync(
        Customer customer,
        Order order,
        CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Create customer
            await _unitOfWork.Customers.AddAsync(customer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken); // Get customer.Id

            // Create order linked to customer
            order.CustomerId = customer.Id;
            await _unitOfWork.Orders.AddAsync(order, cancellationToken);

            // Commit transaction
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Order and customer created successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Error creating order with customer");
            throw;
        }
    }
}
```

### Batch Operations

```csharp
public async Task ImportCustomersAsync(
    List<Customer> customers,
    CancellationToken cancellationToken = default)
{
    await _unitOfWork.BeginTransactionAsync(cancellationToken);

    try
    {
        foreach (var customer in customers)
        {
            // Validate each customer
            ValidateCustomer(customer);

            // Add to context (not saved yet)
            await _unitOfWork.Customers.AddAsync(customer, cancellationToken);
        }

        // Save all at once (atomic)
        await _unitOfWork.CommitTransactionAsync(cancellationToken);

        _logger.LogInformation("Imported {Count} customers", customers.Count);
    }
    catch (Exception ex)
    {
        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
        _logger.LogError(ex, "Error importing customers");
        throw;
    }
}
```

---

## Dependency Injection Setup

```csharp
// Program.cs
private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Database Context (Scoped)
    services.AddDbContext<AppDbContext>(options =>
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        options.UseSqlite(connectionString);
    });

    // Unit of Work (Scoped - one per request/scope)
    services.AddScoped<IUnitOfWork, UnitOfWork>();

    // Services (Scoped - use Unit of Work)
    services.AddScoped<ICustomerService, CustomerService>();
    services.AddScoped<IOrderService, OrderService>();

    // Forms (Transient)
    services.AddTransient<CustomerListForm>();
    services.AddTransient<CustomerEditForm>();
}
```

**Important Notes:**
- ❌ **DO NOT** register repositories individually
- ✅ **DO** register Unit of Work as `Scoped`
- ✅ **DO** inject `IUnitOfWork` into services
- ✅ **DO** call `SaveChangesAsync()` from services

---

## Testing with Unit of Work

```csharp
[Fact]
public async Task CreateCustomer_ValidData_SavesSuccessfully()
{
    // Arrange
    var mockUnitOfWork = new Mock<IUnitOfWork>();
    var mockRepository = new Mock<ICustomerRepository>();
    var mockLogger = new Mock<ILogger<CustomerService>>();

    // Setup Unit of Work to return mock repository
    mockUnitOfWork.Setup(u => u.Customers).Returns(mockRepository.Object);
    mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(1);

    var service = new CustomerService(mockUnitOfWork.Object, mockLogger.Object);

    var customer = new Customer
    {
        Name = "John Doe",
        Email = "john@example.com"
    };

    mockRepository.Setup(r => r.GetByEmailAsync(customer.Email, It.IsAny<CancellationToken>()))
        .ReturnsAsync((Customer?)null);
    mockRepository.Setup(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync((Customer c, CancellationToken ct) => { c.Id = 1; return c; });

    // Act
    var result = await service.CreateCustomerAsync(customer);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(1, result.Id);

    // Verify repository was called
    mockRepository.Verify(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);

    // Verify SaveChanges was called
    mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
}
```

---

## Best Practices

### ✅ DO:
1. **Inject `IUnitOfWork` into services**, not individual repositories
2. **Call `SaveChangesAsync()` from service layer**, not repositories
3. **Use explicit transactions** for complex multi-step operations
4. **Dispose Unit of Work** properly (automatic with DI scoped lifetime)
5. **Use CancellationToken** for all async operations
6. **Log before and after** SaveChanges for debugging
7. **Wrap SaveChanges in try-catch** to handle concurrency exceptions

### ❌ DON'T:
1. **Don't call `SaveChangesAsync()` in repositories**
2. **Don't inject repositories directly** into services (use Unit of Work)
3. **Don't create Unit of Work manually** (use DI)
4. **Don't nest transactions** without careful consideration
5. **Don't forget to call SaveChangesAsync** after modifications
6. **Don't share Unit of Work** across threads
7. **Don't use Unit of Work as Singleton** (must be Scoped)

---

## Common Patterns

### Pattern 1: Simple CRUD
```csharp
// Create
await _unitOfWork.Entities.AddAsync(entity);
await _unitOfWork.SaveChangesAsync();

// Update
await _unitOfWork.Entities.UpdateAsync(entity);
await _unitOfWork.SaveChangesAsync();

// Delete
await _unitOfWork.Entities.DeleteAsync(entity);
await _unitOfWork.SaveChangesAsync();
```

### Pattern 2: Transaction with Rollback
```csharp
await _unitOfWork.BeginTransactionAsync();
try
{
    // Multiple operations
    await _unitOfWork.Entities1.AddAsync(entity1);
    await _unitOfWork.Entities2.AddAsync(entity2);

    await _unitOfWork.CommitTransactionAsync();
}
catch
{
    await _unitOfWork.RollbackTransactionAsync();
    throw;
}
```

### Pattern 3: Conditional Save
```csharp
var hasChanges = false;

if (condition1)
{
    await _unitOfWork.Customers.UpdateAsync(customer);
    hasChanges = true;
}

if (condition2)
{
    await _unitOfWork.Orders.UpdateAsync(order);
    hasChanges = true;
}

if (hasChanges)
{
    await _unitOfWork.SaveChangesAsync();
}
```

---

## Migration Guide

### Old Code (Repository Pattern)
```csharp
// DI Registration
services.AddScoped<ICustomerRepository, CustomerRepository>();
services.AddScoped<ICustomerService, CustomerService>();

// Service
public class CustomerService
{
    private readonly ICustomerRepository _repository;

    public async Task CreateAsync(Customer c)
    {
        await _repository.AddAsync(c); // Auto-saves
    }
}
```

### New Code (Unit of Work Pattern)
```csharp
// DI Registration
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped<ICustomerService, CustomerService>();

// Service
public class CustomerService
{
    private readonly IUnitOfWork _unitOfWork;

    public async Task CreateAsync(Customer c)
    {
        await _unitOfWork.Customers.AddAsync(c);
        await _unitOfWork.SaveChangesAsync(); // Explicit save
    }
}
```

---

## See Also

- [Repository Pattern](repository-pattern.md)
- [Entity Framework Best Practices](entity-framework.md)
- [Dependency Injection](../architecture/dependency-injection.md)
- [Service Layer Pattern](../architecture/project-structure.md)
