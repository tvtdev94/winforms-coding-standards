# WinForms Code Generation Guide

> **Purpose**: Complete guide for generating Forms, Services, Repositories, and Tests
> **Audience**: AI assistants generating WinForms code

---

## 📋 Table of Contents

1. [Templates Overview](#templates-overview)
2. [Generating Forms](#generating-forms)
3. [Generating Services](#generating-services)
4. [Generating Repositories](#generating-repositories)
5. [Generating Unit of Work](#generating-unit-of-work)
6. [Generating Tests](#generating-tests)
7. [Common Patterns](#common-patterns)

---

## Templates Overview

### Available Templates

All templates are **production-ready** and follow all coding standards.

| Template | Path | Purpose |
|----------|------|---------|
| **Form** | `/templates/form-template.cs` | MVP pattern form with presenter |
| **Service** | `/templates/service-template.cs` | Business logic with Unit of Work |
| **Repository** | `/templates/repository-template.cs` | Data access (NO SaveChanges) |
| **Unit of Work** | `/templates/unitofwork-template.cs` | Transaction coordinator |
| **Factory** | `/templates/factory-template.cs` | Form factory for DI |
| **Test** | `/templates/test-template.cs` | Unit test with Moq |

### Critical Rule

**⚠️ NEVER generate code from scratch - ALWAYS start with templates!**

---

## Generating Forms

### 1. Start with Template

Always use `form-template.cs` as the starting point.

### 2. MVP Pattern Structure

Every form needs **3 files**:

1. **View Interface** (`ICustomerView.cs`)
2. **Form** (`CustomerForm.cs`)
3. **Presenter** (`CustomerPresenter.cs`)

### 3. View Interface

```csharp
public interface ICustomerView
{
    // Properties for data binding
    string CustomerName { get; set; }
    string Email { get; set; }
    bool IsActive { get; set; }

    // Events the presenter will handle
    event EventHandler LoadRequested;
    event EventHandler SaveRequested;
    event EventHandler<int> DeleteRequested;

    // Methods for presenter to call
    void ShowError(string message);
    void ShowSuccess(string message);
    void SetCustomerList(IEnumerable<Customer> customers);
    void Close();
}
```

### 4. Form Implementation

```csharp
public partial class CustomerForm : Form, ICustomerView
{
    private readonly CustomerPresenter _presenter;

    // Constructor injection
    public CustomerForm(CustomerPresenter presenter)
    {
        InitializeComponent();
        _presenter = presenter;
        _presenter.SetView(this);
    }

    // Implement properties
    public string CustomerName
    {
        get => txtCustomerName.Text;
        set => txtCustomerName.Text = value;
    }

    // Implement events
    public event EventHandler LoadRequested;
    public event EventHandler SaveRequested;

    // Wire up UI events to interface events
    private async void btnLoad_Click(object sender, EventArgs e)
    {
        LoadRequested?.Invoke(this, EventArgs.Empty);
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        SaveRequested?.Invoke(this, EventArgs.Empty);
    }

    // Implement view methods
    public void ShowError(string message)
    {
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    // Dispose pattern
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
```

### 5. Presenter Implementation

```csharp
public class CustomerPresenter
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomerPresenter> _logger;
    private ICustomerView _view;

    public CustomerPresenter(
        ICustomerService customerService,
        ILogger<CustomerPresenter> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    public void SetView(ICustomerView view)
    {
        _view = view;

        // Subscribe to view events
        _view.LoadRequested += OnLoadRequested;
        _view.SaveRequested += OnSaveRequested;
    }

    private async void OnLoadRequested(object sender, EventArgs e)
    {
        try
        {
            var customers = await _customerService.GetAllAsync();
            _view.SetCustomerList(customers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load customers");
            _view.ShowError($"Failed to load customers: {ex.Message}");
        }
    }

    private async void OnSaveRequested(object sender, EventArgs e)
    {
        try
        {
            var customer = new Customer
            {
                Name = _view.CustomerName,
                Email = _view.Email,
                IsActive = _view.IsActive
            };

            await _customerService.AddAsync(customer);
            _view.ShowSuccess("Customer saved successfully");
            _view.Close();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save customer");
            _view.ShowError($"Failed to save: {ex.Message}");
        }
    }
}
```

### 6. Form Generation Checklist

When generating forms:

- [x] ✅ Start with `form-template.cs`
- [x] ✅ Implement MVP pattern (Form + IView + Presenter)
- [x] ✅ Async event handlers for data operations
- [x] ✅ Try-catch with user-friendly error messages
- [x] ✅ Dispose resources in Dispose() method
- [x] ✅ Set TabIndex for proper keyboard navigation
- [x] ✅ Use meaningful control names (not button1, textBox1)
- [x] ✅ XML documentation on public members

---

## Generating Services

### 1. Start with Template

Always use `service-template.cs`.

### 2. Service Structure

```csharp
/// <summary>
/// Service for managing customer operations
/// </summary>
public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(
        IUnitOfWork unitOfWork,
        ILogger<CustomerService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Adds a new customer
    /// </summary>
    public async Task<Customer> AddAsync(
        Customer customer,
        CancellationToken cancellationToken = default)
    {
        // 1. Validate input
        if (customer == null)
            throw new ArgumentNullException(nameof(customer));

        ValidateCustomer(customer);

        try
        {
            // 2. Log operation
            _logger.LogInformation("Adding customer: {CustomerName}", customer.Name);

            // 3. Business logic
            customer.CreatedDate = DateTime.UtcNow;

            // 4. Access repository via Unit of Work
            await _unitOfWork.Customers.AddAsync(customer, cancellationToken);

            // 5. Save changes via Unit of Work
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Customer added successfully: {CustomerId}", customer.Id);
            return customer;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add customer");
            throw new InvalidOperationException("Failed to add customer", ex);
        }
    }

    private void ValidateCustomer(Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.Name))
            throw new ArgumentException("Customer name is required", nameof(customer));

        if (string.IsNullOrWhiteSpace(customer.Email))
            throw new ArgumentException("Email is required", nameof(customer));

        if (!IsValidEmail(customer.Email))
            throw new ArgumentException("Invalid email format", nameof(customer));
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
```

### 3. Service Generation Checklist

When generating services:

- [x] ✅ Start with `service-template.cs`
- [x] ✅ **Inject `IUnitOfWork`, NOT `IRepository`**
- [x] ✅ Access repositories via `_unitOfWork.EntityName`
- [x] ✅ **Call `await _unitOfWork.SaveChangesAsync()` after modifications**
- [x] ✅ Validate all inputs (ArgumentNullException, ArgumentException)
- [x] ✅ Async methods with proper cancellation token support
- [x] ✅ Log all operations (info, errors, warnings)
- [x] ✅ Wrap exceptions with meaningful messages
- [x] ✅ XML documentation on all public methods

---

## Generating Repositories

### 1. Start with Template

Always use `repository-template.cs`.

### 2. Repository Structure

```csharp
/// <summary>
/// Repository for Customer entity
/// </summary>
public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets all active customers
    /// </summary>
    public async Task<List<Customer>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AsNoTracking() // ✅ Use AsNoTracking for read-only
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets customer by ID
    /// </summary>
    public async Task<Customer?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Adds a new customer
    /// </summary>
    public async Task AddAsync(
        Customer customer,
        CancellationToken cancellationToken = default)
    {
        await _context.Customers.AddAsync(customer, cancellationToken);
        // ❌ NO SaveChangesAsync() here! Unit of Work handles this
    }

    /// <summary>
    /// Updates an existing customer
    /// </summary>
    public Task UpdateAsync(
        Customer customer,
        CancellationToken cancellationToken = default)
    {
        _context.Customers.Update(customer);
        return Task.CompletedTask;
        // ❌ NO SaveChangesAsync() here!
    }

    /// <summary>
    /// Deletes a customer (soft delete)
    /// </summary>
    public Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var customer = _context.Customers.Find(id);
        if (customer != null)
        {
            customer.IsDeleted = true;
            customer.DeletedDate = DateTime.UtcNow;
        }
        return Task.CompletedTask;
        // ❌ NO SaveChangesAsync() here!
    }
}
```

### 3. Repository Generation Checklist

When generating repositories:

- [x] ✅ Start with `repository-template.cs`
- [x] ✅ Implement generic repository pattern with entity-specific interface
- [x] ✅ **NEVER call `SaveChangesAsync()` in repositories**
- [x] ✅ Use EF Core async methods (ToListAsync, FirstOrDefaultAsync, etc.)
- [x] ✅ Use `AsNoTracking()` for read-only queries
- [x] ✅ Return `Task.CompletedTask` for Update/Delete (no SaveChanges)
- [x] ✅ Include soft-delete support if applicable
- [x] ✅ XML documentation on all methods

---

## Generating Unit of Work

### 1. Start with Template

Always use `unitofwork-template.cs`.

### 2. Unit of Work Structure

```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private ICustomerRepository _customers;
    private IOrderRepository _orders;
    private IDbContextTransaction _transaction;

    public UnitOfWork(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // Lazy-loaded repositories
    public ICustomerRepository Customers =>
        _customers ??= new CustomerRepository(_context);

    public IOrderRepository Orders =>
        _orders ??= new OrderRepository(_context);

    // Single save method
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    // Transaction support
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _transaction?.CommitAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _transaction?.RollbackAsync(cancellationToken);
    }

    // Dispose pattern
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
```

### 3. Unit of Work Generation Checklist

When creating Unit of Work:

- [x] ✅ Use `unitofwork-template.cs` as starting point
- [x] ✅ Add repository properties for each entity (lazy-loaded)
- [x] ✅ Implement `SaveChangesAsync()` method
- [x] ✅ Implement transaction methods (Begin/Commit/Rollback)
- [x] ✅ Proper disposal pattern
- [x] ✅ Register as `Scoped` in DI (one instance per scope)

---

## Generating Tests

### 1. Start with Template

Always use `test-template.cs`.

### 2. Test Structure

```csharp
public class CustomerServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<ILogger<CustomerService>> _loggerMock;
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        // Arrange: Setup mocks
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _loggerMock = new Mock<ILogger<CustomerService>>();

        // Setup Unit of Work to return repository mock
        _unitOfWorkMock.Setup(u => u.Customers)
            .Returns(_customerRepositoryMock.Object);

        _service = new CustomerService(
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task AddAsync_ValidCustomer_ReturnsCustomer()
    {
        // Arrange
        var customer = new Customer
        {
            Name = "John Doe",
            Email = "john@example.com"
        };

        _customerRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Customer>(), default))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        var result = await _service.AddAsync(customer);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John Doe", result.Name);
        _customerRepositoryMock.Verify(
            r => r.AddAsync(customer, default),
            Times.Once);
        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(default),
            Times.Once);
    }

    [Fact]
    public async Task AddAsync_NullCustomer_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _service.AddAsync(null));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task AddAsync_InvalidName_ThrowsArgumentException(string name)
    {
        // Arrange
        var customer = new Customer { Name = name, Email = "test@example.com" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.AddAsync(customer));
    }
}
```

### 3. Test Generation Checklist

When generating tests:

- [x] ✅ Start with `test-template.cs`
- [x] ✅ One test class per class under test
- [x] ✅ Use Moq for mocking dependencies
- [x] ✅ Arrange-Act-Assert structure
- [x] ✅ Test naming: `MethodName_Scenario_ExpectedResult`
- [x] ✅ Test both success and failure scenarios
- [x] ✅ Use Assert.Throws for exception testing
- [x] ✅ Verify mocks were called correctly

---

## Common Patterns

### Async Button Click Handler

```csharp
private async void btnSave_Click(object sender, EventArgs e)
{
    try
    {
        btnSave.Enabled = false; // Prevent double-click
        Cursor = Cursors.WaitCursor;

        await _presenter.SaveAsync();
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}", "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    finally
    {
        btnSave.Enabled = true;
        Cursor = Cursors.Default;
    }
}
```

### Thread-Safe UI Update

```csharp
private void UpdateProgress(int percent)
{
    if (InvokeRequired)
    {
        Invoke(new Action(() => UpdateProgress(percent)));
        return;
    }

    progressBar1.Value = percent;
    lblStatus.Text = $"Progress: {percent}%";
}
```

### Proper Resource Disposal

```csharp
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        // Dispose managed resources
        components?.Dispose();
        _presenter?.Dispose();
        _cancellationTokenSource?.Dispose();
    }
    base.Dispose(disposing);
}
```

### Data Binding to DataGridView

```csharp
private void BindCustomers(List<Customer> customers)
{
    dgvCustomers.DataSource = null; // Clear first
    dgvCustomers.DataSource = customers;
    dgvCustomers.Columns["Id"].Visible = false;
    dgvCustomers.Columns["Name"].HeaderText = "Customer Name";
    dgvCustomers.Columns["Email"].Width = 200;
}
```

---

## Summary

**Key Takeaways**:

1. **ALWAYS use templates** - Never generate from scratch
2. **Forms** - MVP pattern with 3 files (IView, Form, Presenter)
3. **Services** - Inject IUnitOfWork, validate inputs, log operations
4. **Repositories** - NEVER call SaveChangesAsync
5. **Unit of Work** - Single SaveChangesAsync, lazy-loaded repositories
6. **Tests** - Moq mocks, AAA pattern, verify calls

**Template Locations**:
- `/templates/form-template.cs`
- `/templates/service-template.cs`
- `/templates/repository-template.cs`
- `/templates/unitofwork-template.cs`
- `/templates/test-template.cs`

---

**See also**:
- [Development Workflow](../workflows/winforms-development-workflow.md) - Complete workflow guide
- [Architecture Guide](./architecture-guide.md) - Pattern explanations
- [AI Instructions](./ai-instructions.md) - DO/DON'T rules
