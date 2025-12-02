# WinForms Legacy Code Migration Guide

> **Purpose**: Step-by-step guide for migrating legacy WinForms applications to modern patterns
> **Target Audience**: Teams with existing WinForms codebases
> **Goal**: Incremental adoption of best practices without complete rewrite

---

## Table of Contents

1. [Overview](#overview)
2. [Migration Strategy](#migration-strategy)
3. [Step-by-Step Migration](#step-by-step-migration)
4. [Common Migration Patterns](#common-migration-patterns)
5. [Testing Strategy](#testing-strategy)
6. [Risk Mitigation](#risk-mitigation)
7. [Success Metrics](#success-metrics)

---

## Overview

### Why Migrate?

**Current Legacy Patterns**:
- ❌ Business logic in Forms
- ❌ Direct database access from UI
- ❌ Synchronous I/O blocking UI
- ❌ No dependency injection
- ❌ Difficult to test
- ❌ Tight coupling

**Modern Patterns (This Repository)**:
- ✅ MVP/MVVM architecture
- ✅ Repository + Unit of Work
- ✅ Async/await for I/O
- ✅ Dependency Injection
- ✅ Fully testable
- ✅ Loose coupling

**Benefits**:
- Better testability (can unit test business logic)
- Improved maintainability
- Easier to add features
- Better performance (async I/O)
- Team collaboration (clear separation of concerns)
- Future-proof architecture

---

## Migration Strategy

### Approach: **Incremental Migration** (NOT Big Bang Rewrite)

**Philosophy**: Strangle the Legacy
- Migrate one feature/form at a time
- Keep old code running during migration
- Gradually replace legacy components
- Minimize risk

### Migration Phases

```
Phase 1: Foundation (Week 1-2)
├── Add DI container
├── Create core interfaces
├── Setup logging
└── Add testing framework

Phase 2: Data Layer (Week 3-4)
├── Create DbContext
├── Implement Repository pattern
├── Implement Unit of Work
└── Write data layer tests

Phase 3: Business Logic (Week 5-6)
├── Extract business logic to Services
├── Implement validation
├── Add error handling
└── Write service tests

Phase 4: Presentation (Week 7-8)
├── Refactor Forms to MVP
├── Create Presenters
├── Implement Factory pattern
└── Write integration tests

Phase 5: Polish (Week 9-10)
├── Performance optimization
├── Add missing tests
├── Documentation
└── Code review
```

**Timeline**: ~10 weeks for typical medium-sized application (20-30 forms)

---

## Step-by-Step Migration

### Phase 1: Foundation Setup

#### Step 1.1: Add NuGet Packages

```bash
# Essential packages
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.Extensions.Logging
dotnet add package Serilog.Extensions.Logging
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer

# Testing packages
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
dotnet add package Moq
dotnet add package FluentAssertions
```

#### Step 1.2: Setup Dependency Injection

**Before** (Legacy):
```csharp
// Program.cs
static void Main()
{
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
    Application.Run(new MainForm());
}
```

**After** (Modern):
```csharp
// Program.cs
static void Main()
{
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);

    // Configure DI
    var services = new ServiceCollection();
    ConfigureServices(services);
    var serviceProvider = services.BuildServiceProvider();

    // Run with DI
    var mainForm = serviceProvider.GetRequiredService<MainForm>();
    Application.Run(mainForm);
}

static void ConfigureServices(IServiceCollection services)
{
    // Logging
    services.AddLogging(builder =>
    {
        builder.AddSerilog();
    });

    // DbContext
    services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(ConfigurationManager.ConnectionStrings["Default"].ConnectionString));

    // Unit of Work
    services.AddScoped<IUnitOfWork, UnitOfWork>();

    // Repositories (registered in UnitOfWork)

    // Services
    services.AddScoped<ICustomerService, CustomerService>();

    // Factory
    services.AddSingleton<IFormFactory, FormFactory>();

    // Forms
    services.AddTransient<MainForm>();
    services.AddTransient<CustomerListForm>();
    services.AddTransient<CustomerEditForm>();
}
```

#### Step 1.3: Setup Logging

```csharp
// Create Log.cs
public static class Log
{
    private static ILoggerFactory? _loggerFactory;

    public static void Configure(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public static ILogger<T> For<T>() => _loggerFactory!.CreateLogger<T>();
}
```

---

### Phase 2: Data Layer Migration

#### Step 2.1: Create DbContext

**Before** (Legacy):
```csharp
// Direct database access in Form
private void LoadCustomers()
{
    using var connection = new SqlConnection(connectionString);
    connection.Open();

    var command = new SqlCommand("SELECT * FROM Customers", connection);
    var reader = command.ExecuteReader();

    var customers = new List<Customer>();
    while (reader.Read())
    {
        customers.Add(new Customer
        {
            Id = (int)reader["Id"],
            Name = reader["Name"].ToString()
        });
    }

    dgvCustomers.DataSource = customers;
}
```

**After** (Modern):
```csharp
// AppDbContext.cs
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(255);
        });
    }
}
```

#### Step 2.2: Create Repositories

```csharp
// ICustomerRepository.cs
public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<List<Customer>> SearchByNameAsync(string searchTerm, CancellationToken ct = default);
}

// CustomerRepository.cs
public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context) { }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == email, ct);
    }

    public async Task<List<Customer>> SearchByNameAsync(string searchTerm, CancellationToken ct = default)
    {
        return await _context.Customers
            .Where(c => c.Name.Contains(searchTerm))
            .OrderBy(c => c.Name)
            .ToListAsync(ct);
    }
}
```

#### Step 2.3: Create Unit of Work

```csharp
// IUnitOfWork.cs
public interface IUnitOfWork : IDisposable
{
    ICustomerRepository Customers { get; }
    // Add other repositories here

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

// UnitOfWork.cs
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private ICustomerRepository? _customers;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public ICustomerRepository Customers =>
        _customers ??= new CustomerRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
```

---

### Phase 3: Business Logic Migration

#### Step 3.1: Extract Business Logic to Services

**Before** (Legacy - Business logic in Form):
```csharp
// CustomerForm.cs
private async void btnSave_Click(object sender, EventArgs e)
{
    // ❌ Validation in Form
    if (string.IsNullOrWhiteSpace(txtName.Text))
    {
        MessageBox.Show("Name is required");
        return;
    }

    if (!txtEmail.Text.Contains("@"))
    {
        MessageBox.Show("Invalid email");
        return;
    }

    // ❌ Business logic in Form
    var customer = new Customer
    {
        Name = txtName.Text,
        Email = txtEmail.Text,
        Phone = txtPhone.Text,
        CreatedDate = DateTime.Now
    };

    // ❌ Direct database access
    using var connection = new SqlConnection(connectionString);
    await connection.OpenAsync();

    var command = new SqlCommand(
        "INSERT INTO Customers (Name, Email, Phone, CreatedDate) VALUES (@Name, @Email, @Phone, @CreatedDate)",
        connection);

    command.Parameters.AddWithValue("@Name", customer.Name);
    command.Parameters.AddWithValue("@Email", customer.Email);
    command.Parameters.AddWithValue("@Phone", customer.Phone ?? (object)DBNull.Value);
    command.Parameters.AddWithValue("@CreatedDate", customer.CreatedDate);

    await command.ExecuteNonQueryAsync();

    MessageBox.Show("Customer saved successfully");
}
```

**After** (Modern - Business logic in Service):
```csharp
// ICustomerService.cs
public interface ICustomerService
{
    Task<Result<int>> CreateCustomerAsync(Customer customer, CancellationToken ct = default);
}

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

    public async Task<Result<int>> CreateCustomerAsync(Customer customer, CancellationToken ct = default)
    {
        try
        {
            // ✅ Validation in Service
            if (customer == null)
                return Result<int>.Failure("Customer cannot be null");

            if (string.IsNullOrWhiteSpace(customer.Name))
                return Result<int>.Failure("Name is required");

            if (string.IsNullOrWhiteSpace(customer.Email) || !customer.Email.Contains("@"))
                return Result<int>.Failure("Valid email is required");

            // ✅ Check for duplicates
            var existing = await _unitOfWork.Customers.GetByEmailAsync(customer.Email, ct);
            if (existing != null)
                return Result<int>.Failure($"Customer with email {customer.Email} already exists");

            // ✅ Business logic
            customer.CreatedDate = DateTime.Now;
            customer.IsActive = true;

            // ✅ Save via Unit of Work
            await _unitOfWork.Customers.AddAsync(customer, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Customer created: {CustomerId} - {CustomerName}", customer.Id, customer.Name);

            return Result<int>.Success(customer.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer: {CustomerName}", customer?.Name);
            return Result<int>.Failure("An error occurred while creating the customer");
        }
    }
}

// CustomerForm.cs (simplified)
private async void btnSave_Click(object sender, EventArgs e)
{
    // ✅ Form just triggers event
    SaveRequested?.Invoke(this, EventArgs.Empty);
}
```

---

### Phase 4: Presentation Layer Migration (MVP)

#### Step 4.1: Create View Interface

```csharp
// ICustomerEditView.cs
public interface ICustomerEditView
{
    // Properties
    int CustomerId { get; set; }
    string CustomerName { get; set; }
    string CustomerEmail { get; set; }
    string CustomerPhone { get; set; }

    // Events
    event EventHandler? LoadRequested;
    event EventHandler? SaveRequested;
    event EventHandler? CancelRequested;

    // Methods
    void ShowSuccess(string message);
    void ShowError(string message);
    void Close();
}
```

#### Step 4.2: Create Presenter

```csharp
// CustomerEditPresenter.cs
public class CustomerEditPresenter : IDisposable
{
    private readonly ICustomerEditView _view;
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomerEditPresenter> _logger;
    private readonly CancellationTokenSource _cts = new();

    public CustomerEditPresenter(
        ICustomerEditView view,
        ICustomerService customerService,
        ILogger<CustomerEditPresenter> logger)
    {
        _view = view;
        _customerService = customerService;
        _logger = logger;

        // Subscribe to view events
        _view.LoadRequested += OnLoadRequested;
        _view.SaveRequested += OnSaveRequested;
        _view.CancelRequested += OnCancelRequested;
    }

    private async void OnLoadRequested(object? sender, EventArgs e)
    {
        if (_view.CustomerId == 0)
            return; // New customer

        try
        {
            var result = await _customerService.GetByIdAsync(_view.CustomerId, _cts.Token);

            if (result.IsSuccess && result.Data != null)
            {
                _view.CustomerName = result.Data.Name;
                _view.CustomerEmail = result.Data.Email ?? string.Empty;
                _view.CustomerPhone = result.Data.Phone ?? string.Empty;
            }
            else
            {
                _view.ShowError(result.ErrorMessage ?? "Customer not found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading customer {CustomerId}", _view.CustomerId);
            _view.ShowError("Failed to load customer");
        }
    }

    private async void OnSaveRequested(object? sender, EventArgs e)
    {
        var customer = new Customer
        {
            Id = _view.CustomerId,
            Name = _view.CustomerName,
            Email = _view.CustomerEmail,
            Phone = _view.CustomerPhone
        };

        try
        {
            Result<int> result;

            if (customer.Id == 0)
            {
                result = await _customerService.CreateCustomerAsync(customer, _cts.Token);
            }
            else
            {
                result = await _customerService.UpdateCustomerAsync(customer, _cts.Token);
            }

            if (result.IsSuccess)
            {
                _view.ShowSuccess("Customer saved successfully");
                _view.Close();
            }
            else
            {
                _view.ShowError(result.ErrorMessage ?? "Failed to save customer");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving customer");
            _view.ShowError("An unexpected error occurred");
        }
    }

    private void OnCancelRequested(object? sender, EventArgs e)
    {
        _view.Close();
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();

        _view.LoadRequested -= OnLoadRequested;
        _view.SaveRequested -= OnSaveRequested;
        _view.CancelRequested -= OnCancelRequested;
    }
}
```

#### Step 4.3: Refactor Form to Implement View

```csharp
// CustomerEditForm.cs
public partial class CustomerEditForm : Form, ICustomerEditView
{
    private readonly CustomerEditPresenter _presenter;

    public CustomerEditForm(
        ICustomerService customerService,
        ILogger<CustomerEditPresenter> logger)
    {
        InitializeComponent();

        // Create presenter
        _presenter = new CustomerEditPresenter(this, customerService, logger);
    }

    #region ICustomerEditView Implementation

    public int CustomerId { get; set; }

    public string CustomerName
    {
        get => txtName.Text;
        set => txtName.Text = value;
    }

    public string CustomerEmail
    {
        get => txtEmail.Text;
        set => txtEmail.Text = value;
    }

    public string CustomerPhone
    {
        get => txtPhone.Text;
        set => txtPhone.Text = value;
    }

    public event EventHandler? LoadRequested;
    public event EventHandler? SaveRequested;
    public event EventHandler? CancelRequested;

    public void ShowSuccess(string message)
    {
        MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        DialogResult = DialogResult.OK;
    }

    public void ShowError(string message)
    {
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    #endregion

    private void CustomerEditForm_Load(object sender, EventArgs e)
    {
        LoadRequested?.Invoke(this, EventArgs.Empty);
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        SaveRequested?.Invoke(this, EventArgs.Empty);
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        CancelRequested?.Invoke(this, EventArgs.Empty);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _presenter?.Dispose();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
```

#### Step 4.4: Implement Factory Pattern

```csharp
// IFormFactory.cs
public interface IFormFactory
{
    TForm Create<TForm>() where TForm : Form;
}

// FormFactory.cs
public class FormFactory : IFormFactory
{
    private readonly IServiceProvider _serviceProvider;

    public FormFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public TForm Create<TForm>() where TForm : Form
    {
        return _serviceProvider.GetRequiredService<TForm>();
    }
}

// Usage in other forms
private void btnEditCustomer_Click(object sender, EventArgs e)
{
    // ✅ Use Factory instead of IServiceProvider
    var editForm = _formFactory.Create<CustomerEditForm>();
    editForm.CustomerId = selectedCustomerId;
    editForm.ShowDialog(this);
}
```

---

## Common Migration Patterns

### Pattern 1: Migrate Synchronous to Async

**Before**:
```csharp
private void LoadData()
{
    var customers = _repository.GetAll(); // ❌ Blocks UI
    dgvCustomers.DataSource = customers;
}
```

**After**:
```csharp
private async void LoadData()
{
    try
    {
        IsLoading = true;
        var customers = await _service.GetAllAsync(_cts.Token); // ✅ Non-blocking

        if (InvokeRequired)
            Invoke(() => dgvCustomers.DataSource = customers);
        else
            dgvCustomers.DataSource = customers;
    }
    finally
    {
        IsLoading = false;
    }
}
```

### Pattern 2: Extract Validation

**Before** (in Form):
```csharp
if (string.IsNullOrWhiteSpace(txtName.Text))
    MessageBox.Show("Name is required");
```

**After** (in Service):
```csharp
// Service
if (string.IsNullOrWhiteSpace(customer.Name))
    return Result.Failure("Name is required");
```

### Pattern 3: Replace Direct DB Access

**Before**:
```csharp
using var conn = new SqlConnection(connectionString);
var cmd = new SqlCommand("SELECT * FROM Customers WHERE Id = @Id", conn);
cmd.Parameters.AddWithValue("@Id", customerId);
```

**After**:
```csharp
var customer = await _unitOfWork.Customers.GetByIdAsync(customerId, ct);
```

---

## Testing Strategy

### Test Coverage Goals

- **Data Layer**: 90%+ coverage
- **Business Layer**: 95%+ coverage
- **Presentation Layer**: 70%+ coverage (Presenters)

### Example Unit Test (Service)

```csharp
public class CustomerServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILogger<CustomerService>> _mockLogger;
    private readonly CustomerService _sut;

    public CustomerServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILogger<CustomerService>>();
        _sut = new CustomerService(_mockUnitOfWork.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task CreateCustomerAsync_ValidCustomer_ReturnsSuccess()
    {
        // Arrange
        var customer = new Customer { Name = "John Doe", Email = "john@example.com" };
        _mockUnitOfWork.Setup(u => u.Customers.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _sut.CreateCustomerAsync(customer, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockUnitOfWork.Verify(u => u.Customers.AddAsync(customer, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

---

## Risk Mitigation

### Risks and Mitigation Strategies

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|------------|
| Breaking existing functionality | High | High | Comprehensive testing; migrate incrementally |
| Performance degradation | Medium | Medium | Performance testing; benchmarking |
| Team resistance | Medium | Low | Training; documentation; gradual adoption |
| Schedule overrun | Medium | Medium | Realistic timeline; prioritize critical features |
| Data loss | Low | Critical | Database backups; migration scripts review |

### Rollback Plan

1. **Keep old code**: Don't delete legacy code until migration is verified
2. **Feature flags**: Use flags to switch between old/new implementations
3. **Database backups**: Before any data migration
4. **Git branches**: Keep migration in separate branch until tested

---

## Success Metrics

### KPIs to Track

**Technical Metrics**:
- ✅ Test coverage: >85%
- ✅ Code duplication: <5%
- ✅ Cyclomatic complexity: <10
- ✅ Build time: <2 minutes
- ✅ Test execution: <30 seconds

**Quality Metrics**:
- ✅ Bug count: Decrease by 50%
- ✅ Code review time: Decrease by 30%
- ✅ Feature development time: Decrease by 40%

**Team Metrics**:
- ✅ Developer satisfaction: Increase
- ✅ Onboarding time: Decrease
- ✅ Knowledge sharing: Increase

---

## Migration Checklist

### Pre-Migration

- [ ] Review this guide completely
- [ ] Get team buy-in
- [ ] Setup development environment
- [ ] Create migration branch
- [ ] Setup CI/CD pipeline
- [ ] Create test plan

### During Migration

- [ ] Phase 1: Foundation (DI, logging)
- [ ] Phase 2: Data layer (DbContext, repositories)
- [ ] Phase 3: Business logic (services)
- [ ] Phase 4: Presentation (MVP, presenters)
- [ ] Phase 5: Testing (unit, integration)
- [ ] Phase 6: Documentation

### Post-Migration

- [ ] Performance testing
- [ ] Security review
- [ ] User acceptance testing
- [ ] Deploy to production
- [ ] Monitor for issues
- [ ] Gather feedback
- [ ] Update documentation

---

## Resources

### Documentation
- [Architecture Guide](docs/architecture/mvp-pattern.md)
- [Repository Pattern](docs/data-access/repository-pattern.md)
- [Unit of Work Pattern](docs/data-access/unit-of-work-pattern.md)
- [Async Best Practices](docs/best-practices/async-await.md)

### Templates
- [Form Template](templates/form-template.cs)
- [Service Template](templates/service-template.cs)
- [Repository Template](templates/repository-template.cs)
- [Test Template](templates/test-template.cs)

### Slash Commands
- `/cook "refactor CustomerForm to MVP"` - Refactor form to MVP pattern
- `/cook "create OrderService"` - Create new service
- `/cook "create ProductRepository"` - Create new repository
- `/cook "add tests for CustomerService"` - Add unit tests

---

## FAQ

**Q: Do we need to migrate everything at once?**
A: No! Incremental migration is recommended. Start with one critical feature/form.

**Q: What if we can't change the database schema?**
A: Use EF Core fluent API to map to existing schema. No schema changes required.

**Q: How long will migration take?**
A: Depends on codebase size. Typical timeline: 10-20 weeks for medium-sized apps.

**Q: Can we keep using our current ORM?**
A: Yes, but EF Core is recommended. Repository pattern works with any ORM.

**Q: What about legacy .NET Framework projects?**
A: These patterns work with .NET Framework 4.6.1+. Some NuGet packages may differ.

**Q: Is Claude Code helpful for migration?**
A: Absolutely! Use `/cook "refactor to MVP"` to automate refactoring.

---

## Getting Help

1. **Review documentation**: [docs/](docs/)
2. **Check examples**: [example-project/](example-project/)
3. **Use slash commands**: Type `/` in Claude Code
4. **Ask Claude**: Claude Code can guide you through migration

---

**Last Updated**: 2025-11-17
**Version**: 1.0
**Author**: WinForms Coding Standards Team

**Next Steps**: Start with [Phase 1: Foundation Setup](#phase-1-foundation-setup)
