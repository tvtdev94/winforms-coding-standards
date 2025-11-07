# Repository Pattern in WinForms

> **Quick Reference**: The Repository pattern abstracts data access logic, providing a collection-like interface for domain entities. Essential for testable, maintainable WinForms applications.

---

## 📋 Overview

The **Repository Pattern** is a design pattern that:
- Mediates between the domain and data mapping layers
- Provides a collection-like interface for accessing domain objects
- Encapsulates data access logic in a separate layer
- Makes code more testable and maintainable

**Key Concept**: Think of a repository as an in-memory collection of domain objects, even though data comes from a database.

---

## 🎯 Why This Matters

### Benefits

✅ **Abstraction**: Hide database implementation details from business logic
✅ **Testability**: Easy to mock repositories for unit testing
✅ **Separation of Concerns**: Data access logic separate from business logic
✅ **Centralization**: Query logic in one place, easier to maintain
✅ **Flexibility**: Switch data sources without changing business logic

### When to Use

**Use Repository Pattern when**:
- Building medium to large applications
- Need to unit test business logic without database
- Multiple data sources (SQL, NoSQL, Web API)
- Complex queries need to be reused
- Want to centralize data access logic

**Skip Repository Pattern when**:
- Simple CRUD application with few entities
- Using CQRS pattern (use MediatR instead)
- Prototype or proof-of-concept projects
- Already using ORM with excellent query capabilities

---

## 💡 Repository Pattern Concepts

### What is Repository Pattern?

```
┌─────────────┐
│   Service   │  Business Logic Layer
└──────┬──────┘
       │ uses
       ▼
┌─────────────┐
│ Repository  │  Data Access Layer
└──────┬──────┘
       │ uses
       ▼
┌─────────────┐
│  DbContext  │  ORM Layer
└──────┬──────┘
       │
       ▼
   Database
```

**Repository acts as a collection**:
```csharp
// Repository feels like working with a collection
IEnumerable<Customer> customers = repository.GetAll();
Customer customer = repository.GetById(1);
repository.Add(newCustomer);
repository.Update(customer);
repository.Delete(customer);
```

### Repository vs DbContext Directly

| Aspect | DbContext Direct | Repository Pattern |
|--------|-----------------|-------------------|
| **Complexity** | Lower | Higher (extra layer) |
| **Testability** | Harder | Easier (mock repository) |
| **Query Location** | Scattered | Centralized |
| **Domain Focus** | Database-centric | Domain-centric |
| **Best For** | Simple apps | Complex apps |

**Example: DbContext Direct**
```csharp
// ❌ Service directly uses DbContext
public class CustomerService
{
    private readonly AppDbContext _context;

    public async Task<List<Customer>> GetActiveCustomers()
    {
        return await _context.Customers
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}
```

**Example: Repository Pattern**
```csharp
// ✅ Service uses Repository
public class CustomerService
{
    private readonly ICustomerRepository _repository;

    public async Task<List<Customer>> GetActiveCustomers()
    {
        return await _repository.GetActiveAsync();
    }
}
```

---

## 🔧 Generic Repository

### IRepository<T> Interface

```csharp
// IRepository.cs
public interface IRepository<T> where T : class
{
    // Query operations
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    // Single item queries
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    // Aggregate operations
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    // Write operations
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);
}
```

### Generic Repository Implementation

```csharp
// Repository.cs
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly DbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public virtual async Task AddRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public virtual void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void DeleteRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }
}
```

### Limitations of Generic Repository

**Problem**: Generic repository can't express domain-specific queries

```csharp
// ❌ Complex query with generic repository - leaks into service
public async Task<List<Customer>> GetTopCustomers()
{
    var customers = await _repository.FindAsync(c => c.IsActive);
    return customers
        .Where(c => c.Orders.Sum(o => o.Total) > 1000)
        .OrderByDescending(c => c.Orders.Count)
        .Take(10)
        .ToList(); // Inefficient - loads all active customers!
}
```

**Solution**: Create specific repositories with domain methods

---

## 🎨 Specific Repositories

### ICustomerRepository Interface

```csharp
// ICustomerRepository.cs
public interface ICustomerRepository : IRepository<Customer>
{
    // Domain-specific queries
    Task<IEnumerable<Customer>> GetActiveCustomersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetTopCustomersAsync(int count, CancellationToken cancellationToken = default);
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetCustomersWithOrdersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, int? excludeCustomerId = null, CancellationToken cancellationToken = default);
}
```

### CustomerRepository Implementation

```csharp
// CustomerRepository.cs
public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    private readonly AppDbContext _appContext;

    public CustomerRepository(AppDbContext context) : base(context)
    {
        _appContext = context;
    }

    public async Task<IEnumerable<Customer>> GetActiveCustomersAsync(
        CancellationToken cancellationToken = default)
    {
        return await _appContext.Customers
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Customer>> GetTopCustomersAsync(
        int count,
        CancellationToken cancellationToken = default)
    {
        return await _appContext.Customers
            .Include(c => c.Orders)
            .Where(c => c.IsActive)
            .OrderByDescending(c => c.Orders.Sum(o => o.Total))
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<Customer?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await _appContext.Customers
            .FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<Customer>> GetCustomersWithOrdersAsync(
        CancellationToken cancellationToken = default)
    {
        return await _appContext.Customers
            .Include(c => c.Orders)
            .Where(c => c.Orders.Any())
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Customer>> SearchAsync(
        string searchTerm,
        CancellationToken cancellationToken = default)
    {
        return await _appContext.Customers
            .Where(c => c.Name.Contains(searchTerm) ||
                       c.Email.Contains(searchTerm) ||
                       c.Phone.Contains(searchTerm))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(
        string email,
        int? excludeCustomerId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _appContext.Customers.Where(c => c.Email == email);

        if (excludeCustomerId.HasValue)
        {
            query = query.Where(c => c.Id != excludeCustomerId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}
```

---

## 🔄 Unit of Work Pattern

### Why Unit of Work?

**Problem without Unit of Work**:
```csharp
// ❌ Multiple SaveChanges calls = multiple transactions
public async Task TransferOrder(int orderId, int newCustomerId)
{
    var order = await _orderRepository.GetByIdAsync(orderId);
    order.CustomerId = newCustomerId;
    await _orderRepository.SaveChangesAsync(); // Transaction 1

    var customer = await _customerRepository.GetByIdAsync(newCustomerId);
    customer.OrderCount++;
    await _customerRepository.SaveChangesAsync(); // Transaction 2 - might fail!
}
```

**Solution with Unit of Work**:
```csharp
// ✅ Single SaveChanges = atomic transaction
public async Task TransferOrder(int orderId, int newCustomerId)
{
    var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
    order.CustomerId = newCustomerId;

    var customer = await _unitOfWork.Customers.GetByIdAsync(newCustomerId);
    customer.OrderCount++;

    await _unitOfWork.SaveChangesAsync(); // Single transaction
}
```

### IUnitOfWork Interface

```csharp
// IUnitOfWork.cs
public interface IUnitOfWork : IDisposable
{
    // Repository properties
    ICustomerRepository Customers { get; }
    IOrderRepository Orders { get; }
    IProductRepository Products { get; }

    // Transaction operations
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
```

### UnitOfWork Implementation

```csharp
// UnitOfWork.cs
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    // Lazy initialization of repositories
    private ICustomerRepository? _customers;
    private IOrderRepository? _orders;
    private IProductRepository? _products;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public ICustomerRepository Customers
    {
        get
        {
            _customers ??= new CustomerRepository(_context);
            return _customers;
        }
    }

    public IOrderRepository Orders
    {
        get
        {
            _orders ??= new OrderRepository(_context);
            return _orders;
        }
    }

    public IProductRepository Products
    {
        get
        {
            _products ??= new ProductRepository(_context);
            return _products;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
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
```

---

## 🔌 Dependency Injection Setup

### Registering Repositories

```csharp
// Program.cs
public static class ServiceConfiguration
{
    public static void ConfigureRepositories(IServiceCollection services, string connectionString)
    {
        // DbContext - Scoped lifetime
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Unit of Work - Scoped (one per request/operation)
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Individual Repositories (if not using UnitOfWork)
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
    }
}

// Main method
[STAThread]
static void Main()
{
    var services = new ServiceCollection();

    var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
    ServiceConfiguration.ConfigureRepositories(services, connectionString);

    services.AddScoped<ICustomerService, CustomerService>();

    var serviceProvider = services.BuildServiceProvider();

    Application.Run(serviceProvider.GetRequiredService<MainForm>());
}
```

### Injecting into Services

```csharp
// CustomerService.cs
public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(IUnitOfWork unitOfWork, ILogger<CustomerService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await _unitOfWork.Customers.GetByIdAsync(id);
    }

    public async Task<List<Customer>> GetAllActiveAsync()
    {
        var customers = await _unitOfWork.Customers.GetActiveCustomersAsync();
        return customers.ToList();
    }

    public async Task<bool> SaveAsync(Customer customer)
    {
        try
        {
            // Validation
            if (await _unitOfWork.Customers.EmailExistsAsync(customer.Email, customer.Id))
            {
                throw new ValidationException("Email already exists");
            }

            if (customer.Id == 0)
            {
                await _unitOfWork.Customers.AddAsync(customer);
            }
            else
            {
                _unitOfWork.Customers.Update(customer);
            }

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"Customer saved: {customer.Name}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving customer");
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null) return false;

            _unitOfWork.Customers.Delete(customer);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Customer deleted: {id}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting customer {id}");
            return false;
        }
    }
}
```

### Injecting into Presenters

```csharp
// CustomerPresenter.cs (MVP pattern)
public class CustomerPresenter
{
    private ICustomerView? _view;
    private readonly ICustomerService _customerService;

    public CustomerPresenter(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public void AttachView(ICustomerView view)
    {
        _view = view;
        _view.LoadRequested += OnLoadRequested;
        _view.SaveRequested += OnSaveRequested;
    }

    private async void OnLoadRequested(object? sender, EventArgs e)
    {
        var customers = await _customerService.GetAllActiveAsync();
        _view.SetCustomers(customers);
    }
}
```

---

## ✅ Best Practices

### DO:

✅ **Use specific repositories** for domain-specific queries
```csharp
// ICustomerRepository with domain methods
Task<IEnumerable<Customer>> GetTopCustomersAsync(int count);
```

✅ **Use Unit of Work** for multi-repository transactions
```csharp
await _unitOfWork.Customers.AddAsync(customer);
await _unitOfWork.Orders.AddAsync(order);
await _unitOfWork.SaveChangesAsync(); // Atomic
```

✅ **Return IEnumerable/IQueryable** from repositories, not concrete types
```csharp
Task<IEnumerable<Customer>> GetAllAsync();
```

✅ **Include related entities** in repository methods
```csharp
return await _context.Customers
    .Include(c => c.Orders)
    .ToListAsync();
```

✅ **Use async methods** for all database operations
```csharp
Task<Customer?> GetByIdAsync(int id);
```

✅ **Validate in Service layer**, not Repository
```csharp
// Service validates, Repository just saves
if (await _repository.EmailExistsAsync(email))
    throw new ValidationException("Email exists");
```

✅ **Use CancellationToken** for long-running queries
```csharp
Task<List<Customer>> GetAllAsync(CancellationToken cancellationToken);
```

✅ **Lazy-load repositories** in Unit of Work
```csharp
public ICustomerRepository Customers => _customers ??= new CustomerRepository(_context);
```

✅ **Register repositories as Scoped** in DI
```csharp
services.AddScoped<ICustomerRepository, CustomerRepository>();
```

✅ **Dispose Unit of Work** properly
```csharp
public void Dispose()
{
    _transaction?.Dispose();
    _context.Dispose();
}
```

### DON'T:

❌ **Don't call SaveChanges in Repository** (use Unit of Work)
```csharp
// ❌ BAD
public async Task AddAsync(Customer customer)
{
    await _context.Customers.AddAsync(customer);
    await _context.SaveChangesAsync(); // Don't!
}

// ✅ GOOD
public async Task AddAsync(Customer customer)
{
    await _context.Customers.AddAsync(customer);
    // Let Unit of Work call SaveChanges
}
```

❌ **Don't expose IQueryable** from repositories
```csharp
// ❌ BAD - leaks query logic to service
IQueryable<Customer> GetQuery();

// ✅ GOOD - specific methods
Task<IEnumerable<Customer>> GetActiveAsync();
```

❌ **Don't put business logic in repositories**
```csharp
// ❌ BAD
public async Task DeactivateCustomer(int id)
{
    var customer = await GetByIdAsync(id);
    customer.IsActive = false; // Business logic!
}

// ✅ GOOD - business logic in Service
```

❌ **Don't use generic repository exclusively**
```csharp
// ❌ BAD - complex query in service
var customers = await _repository.FindAsync(c =>
    c.Orders.Sum(o => o.Total) > 1000); // Inefficient!

// ✅ GOOD - specific repository method
var customers = await _repository.GetHighValueCustomersAsync();
```

❌ **Don't mix synchronous and asynchronous methods**
```csharp
// ❌ BAD
public Customer GetById(int id) { }
public async Task<List<Customer>> GetAllAsync() { }

// ✅ GOOD - all async
public async Task<Customer?> GetByIdAsync(int id) { }
public async Task<List<Customer>> GetAllAsync() { }
```

❌ **Don't forget to handle cancellation**
```csharp
// ❌ BAD
public async Task<List<Customer>> GetAllAsync()
{
    return await _context.Customers.ToListAsync();
}

// ✅ GOOD
public async Task<List<Customer>> GetAllAsync(CancellationToken ct = default)
{
    return await _context.Customers.ToListAsync(ct);
}
```

❌ **Don't create multiple DbContext instances**
```csharp
// ❌ BAD - multiple contexts
var customers = new CustomerRepository(new AppDbContext());
var orders = new OrderRepository(new AppDbContext());

// ✅ GOOD - shared context via Unit of Work
var unitOfWork = new UnitOfWork(context);
```

❌ **Don't return null for collections**
```csharp
// ❌ BAD
public async Task<List<Customer>?> GetAllAsync()
{
    return null; // Don't!
}

// ✅ GOOD
public async Task<List<Customer>> GetAllAsync()
{
    return new List<Customer>(); // Empty list, not null
}
```

---

## 🧪 Testing with Repositories

### Mock Repositories with Moq

```csharp
// CustomerServiceTests.cs
public class CustomerServiceTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<ICustomerRepository> _mockCustomerRepo;
    private Mock<ILogger<CustomerService>> _mockLogger;
    private CustomerService _service;

    public CustomerServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCustomerRepo = new Mock<ICustomerRepository>();
        _mockLogger = new Mock<ILogger<CustomerService>>();

        // Setup Unit of Work to return mock repository
        _mockUnitOfWork.Setup(u => u.Customers).Returns(_mockCustomerRepo.Object);

        _service = new CustomerService(_mockUnitOfWork.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingCustomer_ReturnsCustomer()
    {
        // Arrange
        var expectedCustomer = new Customer { Id = 1, Name = "John Doe" };
        _mockCustomerRepo
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(expectedCustomer);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John Doe", result.Name);
        _mockCustomerRepo.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task SaveAsync_DuplicateEmail_ThrowsException()
    {
        // Arrange
        var customer = new Customer { Id = 0, Email = "test@example.com" };
        _mockCustomerRepo
            .Setup(r => r.EmailExistsAsync(customer.Email, null, default))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _service.SaveAsync(customer));
    }

    [Fact]
    public async Task SaveAsync_NewCustomer_SavesSuccessfully()
    {
        // Arrange
        var customer = new Customer { Id = 0, Name = "Jane Doe", Email = "jane@example.com" };
        _mockCustomerRepo
            .Setup(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<int?>(), default))
            .ReturnsAsync(false);
        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        var result = await _service.SaveAsync(customer);

        // Assert
        Assert.True(result);
        _mockCustomerRepo.Verify(r => r.AddAsync(customer, default), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }
}
```

---

## 🔀 Repository Patterns Comparison

### Active Record vs Repository

| Aspect | Active Record | Repository |
|--------|--------------|------------|
| **Pattern** | Domain model contains data access | Separate data access layer |
| **Example** | `customer.Save()` | `repository.Save(customer)` |
| **Best For** | Simple CRUD apps | Complex domain logic |
| **Testing** | Harder (coupled to DB) | Easier (mock repository) |

### Data Mapper vs Repository

**Complementary patterns** - Often used together:
- **Data Mapper**: Maps between database and domain objects (like EF Core)
- **Repository**: Provides collection interface over Data Mapper

---

## 📚 Related Topics

- [MVP Pattern](../architecture/mvp-pattern.md) - Using repositories in presenters
- [Dependency Injection](../architecture/dependency-injection.md) - Registering repositories
- [Async/Await](../best-practices/async-await.md) - Async repository methods
- [Unit Testing](../testing/unit-testing.md) - Testing with mock repositories

---

**Last Updated**: 2025-11-07
