---
description: Refactor existing WinForms code to MVP pattern
---

You are tasked with refactoring existing WinForms code to follow the MVP (Model-View-Presenter) pattern.

---

## 🔥 STEP 0: MANDATORY Context Loading (DO THIS FIRST!)

**Before ANY refactoring, you MUST:**

### 1. Read Project Configuration
```
READ: .claude/project-context.md
```
Extract: `UI_FRAMEWORK`, `PATTERN`, `FRAMEWORK`

### 2. Load Required Guides
- `docs/patterns/mvp-pattern.md` → MVP implementation
- `templates/form-template.cs` (or dx/rt variant) → Target structure
- `docs/architecture/dependency-injection.md` → DI setup

### 3. Critical Rules

| 🚫 NEVER | ✅ ALWAYS |
|----------|----------|
| Business logic in Form | Logic in Presenter |
| Direct data access | Use Services |
| Inject IServiceProvider | Use IFormFactory |
| Skip IView interface | Create IView for Form |

---

## Workflow

1. **Ask the user**:
   - Which form file to refactor?
   - Does it have business logic mixed with UI code?

2. **Read the form file** and analyze:
   - Business logic that should be in Presenter
   - Data access that should be in Service/Repository
   - UI logic that stays in Form

3. **Refactoring Strategy**:

### Step 1: Identify Code to Extract

❌ **Before - Mixed concerns**:
```csharp
public partial class CustomerForm : Form
{
    private SqlConnection _connection;

    public CustomerForm()
    {
        InitializeComponent();
        _connection = new SqlConnection("connection string");
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        // Validation logic (should be in Presenter)
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            MessageBox.Show("Name is required");
            return;
        }

        // Business logic (should be in Service)
        if (txtName.Text.Length < 3)
        {
            MessageBox.Show("Name must be at least 3 characters");
            return;
        }

        // Data access (should be in Repository)
        try
        {
            await _connection.OpenAsync();
            var cmd = new SqlCommand(
                "INSERT INTO Customers (Name, Email) VALUES (@Name, @Email)",
                _connection);
            cmd.Parameters.AddWithValue("@Name", txtName.Text);
            cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
            await cmd.ExecuteNonQueryAsync();

            MessageBox.Show("Saved successfully!");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
        }
        finally
        {
            _connection.Close();
        }
    }
}
```

### Step 2: Create View Interface

✅ **Create ICustomerView.cs**:
```csharp
public interface ICustomerView
{
    // Properties for data binding
    string CustomerName { get; set; }
    string CustomerEmail { get; set; }
    int CustomerId { get; set; }

    // UI state
    bool IsSaveEnabled { get; set; }
    bool IsLoading { get; set; }

    // Events
    event EventHandler SaveRequested;
    event EventHandler LoadRequested;
    event EventHandler<int> DeleteRequested;

    // Methods
    void ShowSuccess(string message);
    void ShowError(string message);
    void Close();
}
```

### Step 3: Implement View Interface in Form

✅ **Update CustomerForm.cs**:
```csharp
public partial class CustomerForm : Form, ICustomerView
{
    private readonly CustomerPresenter _presenter;

    public CustomerForm()
    {
        InitializeComponent();

        // Create presenter with dependency injection
        var repository = new CustomerRepository(new AppDbContext());
        var service = new CustomerService(repository, logger);
        _presenter = new CustomerPresenter(this, service);
    }

    // Or with DI container
    public CustomerForm(CustomerPresenter presenter)
    {
        InitializeComponent();
        _presenter = presenter;
    }

    // Implement ICustomerView properties
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

    public int CustomerId { get; set; }

    public bool IsSaveEnabled
    {
        get => btnSave.Enabled;
        set => btnSave.Enabled = value;
    }

    public bool IsLoading
    {
        set
        {
            progressBar.Visible = value;
            Cursor = value ? Cursors.WaitCursor : Cursors.Default;
        }
    }

    // Implement events
    public event EventHandler SaveRequested;
    public event EventHandler LoadRequested;
    public event EventHandler<int> DeleteRequested;

    // UI event handlers just raise view events
    private void btnSave_Click(object sender, EventArgs e)
    {
        SaveRequested?.Invoke(this, EventArgs.Empty);
    }

    private void btnLoad_Click(object sender, EventArgs e)
    {
        LoadRequested?.Invoke(this, EventArgs.Empty);
    }

    // Implement view methods
    public void ShowSuccess(string message)
    {
        MessageBox.Show(message, "Success",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public void ShowError(string message)
    {
        MessageBox.Show(message, "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    // Form lifecycle
    private void CustomerForm_Load(object sender, EventArgs e)
    {
        _presenter.Initialize();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
            _presenter?.Dispose();
        }
        base.Dispose(disposing);
    }
}
```

### Step 4: Create Presenter

✅ **Create CustomerPresenter.cs**:
```csharp
public class CustomerPresenter : IDisposable
{
    private readonly ICustomerView _view;
    private readonly ICustomerService _service;
    private readonly ILogger<CustomerPresenter> _logger;

    public CustomerPresenter(
        ICustomerView view,
        ICustomerService service,
        ILogger<CustomerPresenter> logger)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Subscribe to view events
        _view.SaveRequested += OnSaveRequested;
        _view.LoadRequested += OnLoadRequested;
        _view.DeleteRequested += OnDeleteRequested;
    }

    public void Initialize()
    {
        // Initialize view state
        _view.IsSaveEnabled = true;
        _view.IsLoading = false;
    }

    private async void OnSaveRequested(object sender, EventArgs e)
    {
        try
        {
            _view.IsSaveEnabled = false;
            _view.IsLoading = true;

            // Validation
            if (string.IsNullOrWhiteSpace(_view.CustomerName))
            {
                _view.ShowError("Name is required");
                return;
            }

            if (_view.CustomerName.Length < 3)
            {
                _view.ShowError("Name must be at least 3 characters");
                return;
            }

            // Create model from view data
            var customer = new Customer
            {
                Id = _view.CustomerId,
                Name = _view.CustomerName,
                Email = _view.CustomerEmail
            };

            // Save through service
            if (customer.Id == 0)
            {
                await _service.CreateCustomerAsync(customer);
                _view.ShowSuccess("Customer created successfully!");
            }
            else
            {
                await _service.UpdateCustomerAsync(customer);
                _view.ShowSuccess("Customer updated successfully!");
            }

            _view.Close();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving customer");
            _view.ShowError($"Failed to save customer: {ex.Message}");
        }
        finally
        {
            _view.IsSaveEnabled = true;
            _view.IsLoading = false;
        }
    }

    private async void OnLoadRequested(object sender, EventArgs e)
    {
        try
        {
            _view.IsLoading = true;

            var customer = await _service.GetCustomerAsync(_view.CustomerId);

            if (customer != null)
            {
                _view.CustomerName = customer.Name;
                _view.CustomerEmail = customer.Email;
            }
            else
            {
                _view.ShowError("Customer not found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading customer");
            _view.ShowError($"Failed to load customer: {ex.Message}");
        }
        finally
        {
            _view.IsLoading = false;
        }
    }

    private async void OnDeleteRequested(object sender, int customerId)
    {
        try
        {
            await _service.DeleteCustomerAsync(customerId);
            _view.ShowSuccess("Customer deleted successfully!");
            _view.Close();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer");
            _view.ShowError($"Failed to delete customer: {ex.Message}");
        }
    }

    public void Dispose()
    {
        // Unsubscribe from events
        _view.SaveRequested -= OnSaveRequested;
        _view.LoadRequested -= OnLoadRequested;
        _view.DeleteRequested -= OnDeleteRequested;
    }
}
```

### Step 5: Create Service Layer (if not exists)

✅ **Create ICustomerService.cs and CustomerService.cs**:
```csharp
public interface ICustomerService
{
    Task<Customer> GetCustomerAsync(int id);
    Task<List<Customer>> GetAllCustomersAsync();
    Task CreateCustomerAsync(Customer customer);
    Task UpdateCustomerAsync(Customer customer);
    Task DeleteCustomerAsync(int id);
}

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(
        ICustomerRepository repository,
        ILogger<CustomerService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Customer> GetCustomerAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid customer ID", nameof(id));

        _logger.LogInformation("Loading customer {CustomerId}", id);
        return await _repository.GetByIdAsync(id);
    }

    public async Task CreateCustomerAsync(Customer customer)
    {
        if (customer == null)
            throw new ArgumentNullException(nameof(customer));

        // Business logic
        ValidateCustomer(customer);

        _logger.LogInformation("Creating customer {CustomerName}", customer.Name);
        await _repository.AddAsync(customer);
    }

    private void ValidateCustomer(Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.Name))
            throw new ValidationException("Customer name is required");

        if (customer.Name.Length < 3)
            throw new ValidationException("Customer name must be at least 3 characters");

        // Add more business rules...
    }

    // Implement other methods...
}
```

4. **Migration Checklist**:
   - [ ] Create IView interface with properties and events
   - [ ] Implement IView in Form
   - [ ] Move business logic to Presenter
   - [ ] Move data access to Service/Repository
   - [ ] Create event-based communication View ↔ Presenter
   - [ ] Add proper error handling in Presenter
   - [ ] Add logging in Presenter
   - [ ] Dispose Presenter in Form
   - [ ] Write unit tests for Presenter

5. **Benefits of MVP**:
   - ✅ Testable presentation logic
   - ✅ Clear separation of concerns
   - ✅ Reusable business logic
   - ✅ Better maintainability
   - ✅ Easier to mock for testing

6. **Show the user**:
   - Refactored Form with IView implementation
   - New Presenter class
   - Service class (if needed)
   - Explanation of separation of concerns
   - Offer to create unit tests for Presenter
