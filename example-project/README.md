# Customer Management System - WinForms Example

A complete WinForms application demonstrating best practices for building maintainable, testable, and scalable Windows Forms applications.

## 📋 Overview

This project showcases professional C# WinForms development patterns including:

- ✅ **MVP Pattern** - Model-View-Presenter for separation of concerns
- ✅ **Dependency Injection** - Microsoft.Extensions.DependencyInjection
- ✅ **Entity Framework Core** - SQLite database with Code-First approach
- ✅ **Repository Pattern** - Generic and specific repositories
- ✅ **Async/Await** - Responsive UI with asynchronous operations
- ✅ **Input Validation** - Business logic validation with ErrorProvider
- ✅ **Error Handling** - Comprehensive exception handling and logging
- ✅ **Unit Testing** - xUnit with Moq and FluentAssertions
- ✅ **Integration Testing** - Repository tests with SQLite in-memory
- ✅ **Logging** - Serilog for structured logging

## 🏗️ Project Structure

```
CustomerManagement/
├── src/CustomerManagement/
│   ├── Models/                # Domain entities
│   │   ├── Customer.cs
│   │   └── Order.cs
│   ├── Data/                  # EF Core context
│   │   ├── AppDbContext.cs
│   │   └── DbInitializer.cs
│   ├── Repositories/          # Data access layer
│   │   ├── IRepository.cs
│   │   ├── ICustomerRepository.cs
│   │   └── CustomerRepository.cs
│   ├── Services/              # Business logic layer
│   │   ├── ICustomerService.cs
│   │   └── CustomerService.cs
│   ├── Views/                 # MVP View interfaces
│   │   ├── ICustomerListView.cs
│   │   └── ICustomerEditView.cs
│   ├── Presenters/            # MVP Presenters
│   │   ├── CustomerListPresenter.cs
│   │   └── CustomerEditPresenter.cs
│   ├── Forms/                 # WinForms UI
│   │   ├── MainForm.cs
│   │   ├── CustomerListForm.cs
│   │   └── CustomerEditForm.cs
│   ├── Program.cs             # DI configuration & startup
│   └── appsettings.json       # Application configuration
├── tests/
│   ├── CustomerManagement.Tests/              # Unit tests
│   │   ├── Services/CustomerServiceTests.cs
│   │   └── Presenters/CustomerListPresenterTests.cs
│   └── CustomerManagement.IntegrationTests/   # Integration tests
│       └── Repositories/CustomerRepositoryTests.cs
└── CustomerManagement.sln
```

## 🚀 Getting Started

### Prerequisites

- **.NET 8.0 SDK** or later
- **Visual Studio 2022** or **JetBrains Rider** (optional but recommended)
- **Windows OS** (for WinForms)

### Running the Application

1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd example-project
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Build the solution**:
   ```bash
   dotnet build
   ```

4. **Run the application**:
   ```bash
   cd src/CustomerManagement
   dotnet run
   ```

The database will be automatically created with seed data on first run.

### Running Tests

**Run all tests**:
```bash
dotnet test
```

**Run unit tests only**:
```bash
dotnet test tests/CustomerManagement.Tests/
```

**Run integration tests only**:
```bash
dotnet test tests/CustomerManagement.IntegrationTests/
```

**Run tests with coverage**:
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

## 🎯 Key Features Demonstrated

### 1. MVP (Model-View-Presenter) Pattern

**Why MVP?**
- Separates UI logic from business logic
- Makes the application highly testable
- Allows unit testing presenters without UI dependencies

**Example**:
```csharp
// View interface (contract)
public interface ICustomerListView
{
    event EventHandler? LoadRequested;
    List<Customer> Customers { get; set; }
    void ShowError(string message);
}

// Presenter (business logic)
public class CustomerListPresenter
{
    private readonly ICustomerListView _view;
    private readonly ICustomerService _service;

    public CustomerListPresenter(ICustomerListView view, ICustomerService service)
    {
        _view = view;
        _service = service;
        _view.LoadRequested += OnLoadRequested;
    }

    private async void OnLoadRequested(object? sender, EventArgs e)
    {
        var customers = await _service.GetAllCustomersAsync();
        _view.Customers = customers;
    }
}

// Form (implements view interface)
public class CustomerListForm : Form, ICustomerListView
{
    public event EventHandler? LoadRequested;
    public List<Customer> Customers { get; set; }
    // ...
}
```

### 2. Dependency Injection

**Configuration in Program.cs**:
```csharp
services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

services.AddScoped<ICustomerRepository, CustomerRepository>();
services.AddScoped<ICustomerService, CustomerService>();
services.AddTransient<CustomerListForm>();
```

**Usage in Forms**:
```csharp
public CustomerListForm(
    IServiceProvider serviceProvider,
    ICustomerService customerService,
    ILogger<CustomerListPresenter> logger)
{
    _serviceProvider = serviceProvider;
    _presenter = new CustomerListPresenter(this, customerService, logger);
}
```

### 3. Repository Pattern

**Generic repository**:
```csharp
public interface IRepository<T> where T : class
{
    Task<List<T>> GetAllAsync(CancellationToken ct = default);
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(T entity, CancellationToken ct = default);
}
```

**Specific repository with domain logic**:
```csharp
public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<Customer?> GetWithOrdersAsync(int customerId, CancellationToken ct = default);
    Task<List<Customer>> SearchByNameAsync(string searchTerm, CancellationToken ct = default);
}
```

### 4. Async/Await for Responsive UI

**Async button click handler**:
```csharp
private async void btnLoad_Click(object sender, EventArgs e)
{
    try
    {
        IsLoading = true;
        var customers = await _service.GetAllCustomersAsync();
        dgvCustomers.DataSource = customers;
    }
    catch (Exception ex)
    {
        ShowError($"Failed to load: {ex.Message}");
    }
    finally
    {
        IsLoading = false;
    }
}
```

### 5. Input Validation

**Service-level validation**:
```csharp
public Dictionary<string, string> ValidateCustomer(Customer customer)
{
    var errors = new Dictionary<string, string>();

    if (string.IsNullOrWhiteSpace(customer.Name))
        errors[nameof(customer.Name)] = "Name is required.";

    if (!IsValidEmail(customer.Email))
        errors[nameof(customer.Email)] = "Email format is invalid.";

    return errors;
}
```

**View-level error display**:
```csharp
public void SetFieldError(string fieldName, string errorMessage)
{
    Control? control = fieldName switch
    {
        nameof(CustomerName) => _txtName,
        nameof(CustomerEmail) => _txtEmail,
        _ => null
    };

    if (control != null)
        _errorProvider.SetError(control, errorMessage);
}
```

### 6. Unit Testing with Moq

**Testing a service**:
```csharp
[Fact]
public async Task CreateCustomerAsync_ValidCustomer_ReturnsCreatedCustomer()
{
    // Arrange
    var customer = new Customer { Name = "John", Email = "john@example.com" };
    _mockRepository
        .Setup(r => r.GetByEmailAsync(customer.Email, It.IsAny<CancellationToken>()))
        .ReturnsAsync((Customer?)null);

    // Act
    var result = await _service.CreateCustomerAsync(customer);

    // Assert
    result.Should().NotBeNull();
    result.Id.Should().BeGreaterThan(0);
}
```

**Testing a presenter**:
```csharp
[Fact]
public async Task LoadRequested_LoadsCustomersSuccessfully()
{
    // Arrange
    var customers = new List<Customer> { /* ... */ };
    _mockService.Setup(s => s.GetAllCustomersAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(customers);

    // Act
    _mockView.Raise(v => v.LoadRequested += null, EventArgs.Empty);
    await Task.Delay(100); // Wait for async

    // Assert
    _mockView.VerifySet(v => v.Customers = It.IsAny<List<Customer>>(), Times.Once);
}
```

### 7. Integration Testing

**Testing repository with real database**:
```csharp
public class CustomerRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly CustomerRepository _repository;

    public CustomerRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new AppDbContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        _repository = new CustomerRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ValidCustomer_AddsToDatabase()
    {
        // Arrange
        var customer = new Customer { Name = "John", Email = "john@example.com" };

        // Act
        var result = await _repository.AddAsync(customer);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        var fromDb = await _repository.GetByIdAsync(result.Id);
        fromDb.Should().NotBeNull();
    }
}
```

## 📊 Database Schema

### Customer Table
| Column | Type | Constraints |
|--------|------|-------------|
| Id | INTEGER | PRIMARY KEY |
| Name | TEXT(100) | NOT NULL |
| Email | TEXT(100) | NOT NULL, UNIQUE |
| Phone | TEXT(20) | NULL |
| Address | TEXT(200) | NULL |
| City | TEXT(50) | NULL |
| Country | TEXT(50) | NULL |
| IsActive | BOOLEAN | NOT NULL |
| CreatedAt | DATETIME | NOT NULL |
| UpdatedAt | DATETIME | NULL |

### Order Table
| Column | Type | Constraints |
|--------|------|-------------|
| Id | INTEGER | PRIMARY KEY |
| OrderNumber | TEXT(20) | NOT NULL, UNIQUE |
| CustomerId | INTEGER | NOT NULL, FOREIGN KEY |
| OrderDate | DATETIME | NOT NULL |
| TotalAmount | DECIMAL(18,2) | NOT NULL |
| Status | TEXT(20) | NOT NULL |
| Notes | TEXT(500) | NULL |
| CreatedAt | DATETIME | NOT NULL |

**Relationship**: One Customer → Many Orders (Cascade Delete)

## 🧪 Testing Strategy

### Test Coverage

- **Unit Tests**: Services, Presenters
- **Integration Tests**: Repositories, Database operations
- **Target Coverage**: 80%+ overall, 90%+ for business logic

### Running Coverage Reports

1. **Generate coverage**:
   ```bash
   dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
   ```

2. **Generate HTML report** (requires reportgenerator):
   ```bash
   dotnet tool install -g dotnet-reportgenerator-globaltool
   reportgenerator -reports:coverage.cobertura.xml -targetdir:coveragereport
   ```

3. **Open report**:
   ```bash
   start coveragereport/index.html
   ```

## 📝 Logging

Application logs are written to `logs/app-YYYYMMDD.log` using Serilog.

**Log levels**:
- **Information**: Normal operations (customer created, loaded, etc.)
- **Warning**: Validation failures, business rule violations
- **Error**: Exceptions, database errors
- **Fatal**: Application crashes

**Example log output**:
```
2024-01-15 10:30:45.123 +00:00 [INF] Loading customers
2024-01-15 10:30:45.456 +00:00 [INF] Retrieved 15 customers
2024-01-15 10:30:50.789 +00:00 [WRN] Validation failed for customer: Name is required
2024-01-15 10:31:00.123 +00:00 [INF] Customer created successfully with ID: 16
```

## 🔧 Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=customers.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### Changing Database Location

Edit `ConnectionStrings:DefaultConnection` in `appsettings.json`:
```json
"DefaultConnection": "Data Source=C:\\MyData\\customers.db"
```

## 🎓 Learning Resources

This example project is part of the **WinForms Coding Standards** repository.

**Related Documentation**:
- [MVP Pattern Guide](../docs/architecture/mvp-pattern.md)
- [Dependency Injection Setup](../docs/architecture/dependency-injection.md)
- [Entity Framework with WinForms](../docs/data-access/entity-framework.md)
- [Repository Pattern](../docs/data-access/repository-pattern.md)
- [Unit Testing Guide](../docs/testing/unit-testing.md)
- [Integration Testing Guide](../docs/testing/integration-testing.md)

## 🚦 Common Tasks

### Adding a New Entity

1. Create model in `Models/`
2. Add DbSet to `AppDbContext`
3. Create repository interface and implementation
4. Create service interface and implementation
5. Create view interface for forms
6. Create presenter for business logic
7. Create form implementing view interface
8. Add to DI container in `Program.cs`
9. Write unit and integration tests

### Debugging Tips

**Enable SQL logging**:
```csharp
options.UseSqlite(connectionString)
    .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors();
```

**Recreate database**:
```csharp
await DbInitializer.RecreateAsync(context, logger);
```

## 📄 License

This example project is provided as-is for educational purposes.

## 🤝 Contributing

This is an example project. For questions or suggestions about the WinForms Coding Standards repository, please open an issue in the main repository.

---

**Version**: 1.0
**Last Updated**: 2024
**Target Framework**: .NET 8.0
**UI Framework**: Windows Forms
