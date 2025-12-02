# WinForms Code Generation Guide

> **Purpose**: Guide for generating Forms, Services, Repositories, and Tests
> **Rule**: NEVER generate from scratch - ALWAYS use templates!

---

## Templates Overview

### Standard Templates

| Template | Path | Purpose |
|----------|------|---------|
| Form | `form-template.cs` | MVP form + presenter |
| Service | `service-template.cs` | Business logic + UoW |
| Repository | `repository-template.cs` | Data access (NO SaveChanges) |
| UnitOfWork | `unitofwork-template.cs` | Transaction coordinator |
| Factory | `form-factory-template.cs` | Form factory for DI |
| Test | `test-template.cs` | Unit test with Moq |

### Framework-Specific Templates

| Framework | Templates |
|-----------|-----------|
| DevExpress | `dx-form-templates.cs`, `dx-data-templates.cs` |
| ReaLTaiizor | `rt-templates.cs` |

### Choosing Templates

Check `project-context.md` for UI framework:
- **DevExpress**: Use `dx-*.cs` templates
- **ReaLTaiizor**: Use `rt-templates.cs`
- **Standard**: Use `form-template.cs`

---

## Generating Forms (MVP Pattern)

### Required Files (3)

1. `ICustomerView.cs` - View interface
2. `CustomerForm.cs` - Form implementation
3. `CustomerPresenter.cs` - Presenter

### View Interface

```csharp
public interface ICustomerView
{
    // Properties
    string CustomerName { get; set; }
    string Email { get; set; }

    // Events
    event EventHandler LoadRequested;
    event EventHandler SaveRequested;

    // Methods
    void ShowError(string message);
    void ShowSuccess(string message);
    void Close();
}
```

### Form (Minimal)

```csharp
public partial class CustomerForm : Form, ICustomerView
{
    private readonly CustomerPresenter _presenter;

    public CustomerForm(CustomerPresenter presenter)
    {
        InitializeComponent();
        _presenter = presenter;
        _presenter.SetView(this);
    }

    public string CustomerName
    {
        get => txtName.Text;
        set => txtName.Text = value;
    }

    public event EventHandler LoadRequested;
    public event EventHandler SaveRequested;

    private void btnSave_Click(object s, EventArgs e)
        => SaveRequested?.Invoke(this, EventArgs.Empty);

    public void ShowError(string msg)
        => MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
```

### Presenter (Minimal)

```csharp
public class CustomerPresenter
{
    private readonly ICustomerService _service;
    private readonly ILogger<CustomerPresenter> _logger;
    private ICustomerView _view;

    public CustomerPresenter(ICustomerService service, ILogger<CustomerPresenter> logger)
    {
        _service = service;
        _logger = logger;
    }

    public void SetView(ICustomerView view)
    {
        _view = view;
        _view.SaveRequested += OnSaveRequested;
    }

    private async void OnSaveRequested(object s, EventArgs e)
    {
        try
        {
            var customer = new Customer { Name = _view.CustomerName, Email = _view.Email };
            await _service.AddAsync(customer);
            _view.ShowSuccess("Saved!");
            _view.Close();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Save failed");
            _view.ShowError(ex.Message);
        }
    }
}
```

---

## Generating Services

### Structure

```csharp
public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;  // NOT IRepository!
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(IUnitOfWork unitOfWork, ILogger<CustomerService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Customer> AddAsync(Customer customer, CancellationToken ct = default)
    {
        // 1. Validate
        if (customer == null) throw new ArgumentNullException(nameof(customer));
        if (string.IsNullOrWhiteSpace(customer.Name))
            throw new ArgumentException("Name required");

        // 2. Business logic
        customer.CreatedDate = DateTime.UtcNow;

        // 3. Repository via UoW
        await _unitOfWork.Customers.AddAsync(customer, ct);

        // 4. SaveChanges via UoW
        await _unitOfWork.SaveChangesAsync(ct);

        return customer;
    }
}
```

### Key Rules
- Inject `IUnitOfWork`, NOT `IRepository`
- Call `SaveChangesAsync()` in service, NOT repository
- Validate all inputs
- Log operations

---

## Generating Repositories

### Structure

```csharp
public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context) => _context = context;

    public async Task<List<Customer>> GetAllAsync(CancellationToken ct = default)
        => await _context.Customers.AsNoTracking().ToListAsync(ct);

    public async Task<Customer?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.Customers.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task AddAsync(Customer customer, CancellationToken ct = default)
    {
        await _context.Customers.AddAsync(customer, ct);
        // NO SaveChangesAsync() here!
    }

    public Task UpdateAsync(Customer customer, CancellationToken ct = default)
    {
        _context.Customers.Update(customer);
        return Task.CompletedTask;
        // NO SaveChangesAsync() here!
    }
}
```

### Key Rules
- **NEVER** call `SaveChangesAsync()` in repository
- Use `AsNoTracking()` for read-only queries
- Support `CancellationToken`

---

## Generating Unit of Work

### Structure

```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private ICustomerRepository _customers;

    public UnitOfWork(AppDbContext context) => _context = context;

    // Lazy-loaded repositories
    public ICustomerRepository Customers
        => _customers ??= new CustomerRepository(_context);

    // Single save point
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);

    public void Dispose() => _context.Dispose();
}
```

---

## Generating Tests

### Structure

```csharp
public class CustomerServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ICustomerRepository> _repoMock;
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _repoMock = new Mock<ICustomerRepository>();
        _uowMock.Setup(u => u.Customers).Returns(_repoMock.Object);

        _service = new CustomerService(_uowMock.Object, Mock.Of<ILogger<CustomerService>>());
    }

    [Fact]
    public async Task AddAsync_ValidCustomer_Saves()
    {
        // Arrange
        var customer = new Customer { Name = "John", Email = "j@test.com" };
        _uowMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        var result = await _service.AddAsync(customer);

        // Assert
        Assert.NotNull(result);
        _repoMock.Verify(r => r.AddAsync(customer, default), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task AddAsync_NullCustomer_Throws()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddAsync(null));
    }
}
```

### Key Rules
- Use Moq for mocking
- AAA pattern (Arrange-Act-Assert)
- Naming: `MethodName_Scenario_ExpectedResult`
- Verify mock calls

---

## Common Patterns

### Async Button Click

```csharp
private async void btnSave_Click(object s, EventArgs e)
{
    try
    {
        btnSave.Enabled = false;
        await _presenter.SaveAsync();
    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.Message, "Error");
    }
    finally
    {
        btnSave.Enabled = true;
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
}
```

---

## DevExpress Code Generation

### Key Differences

| Standard | DevExpress |
|----------|------------|
| `Form` | `XtraForm` |
| `TextBox` | `TextEdit` |
| `ComboBox` | `LookUpEdit` |
| `DataGridView` | `GridControl` |
| `MessageBox` | `XtraMessageBox` |

### DevExpress Naming

| Control | Prefix | Example |
|---------|--------|---------|
| GridControl | `grc` | `grcCustomers` |
| TextEdit | `txt` | `txtName` |
| LookUpEdit | `lke` | `lkeType` |
| DateEdit | `dte` | `dteCreated` |
| SimpleButton | `btn` | `btnSave` |

### Grid Configuration

```csharp
var gv = gridControl1.MainView as GridView;
gv.OptionsBehavior.Editable = false;
gv.OptionsFind.AlwaysVisible = true;  // Built-in search
gv.BestFitColumns();
```

---

## Summary

| Component | Template | Key Rule |
|-----------|----------|----------|
| Form | `form-template.cs` | MVP pattern (3 files) |
| Service | `service-template.cs` | Inject IUnitOfWork, call SaveChangesAsync |
| Repository | `repository-template.cs` | NEVER SaveChangesAsync |
| UnitOfWork | `unitofwork-template.cs` | Single SaveChangesAsync |
| Test | `test-template.cs` | Moq, AAA pattern |
| DevExpress | `dx-form-templates.cs` | XtraForm, LayoutControl |

---

**See also**:
- [Development Workflow](../workflows/development-workflow.md)
- [Architecture Guide](./architecture-guide.md)
- [AI Instructions](./ai-instructions.md)
