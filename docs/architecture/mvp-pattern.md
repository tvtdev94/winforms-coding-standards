# MVP (Model-View-Presenter) Pattern for WinForms

> **Quick Reference**: The recommended architectural pattern for WinForms applications. Separates UI from business logic for better testability and maintainability.

⭐ **RECOMMENDED** for medium to large WinForms applications

---

## 📖 What is MVP?

**MVP (Model-View-Presenter)** is a UI pattern that separates:
- **Model**: Data and business entities
- **View**: UI layer (Forms) - passive, no logic
- **Presenter**: Presentation logic - mediates between View and Model

### Why MVP for WinForms?

✅ **Perfect fit** - WinForms lacks strong data binding (unlike WPF)
✅ **Testable** - Presenter can be unit tested without UI
✅ **Separation** - UI logic separate from business logic
✅ **Microsoft recommended** - CAB framework uses MVP

---

## 🏗️ MVP Architecture

```
┌─────────┐         ┌───────────┐         ┌─────────┐
│  View   │ ◄──────►│ Presenter │ ◄──────►│  Model  │
│ (Form)  │         │           │         │ Service │
└─────────┘         └───────────┘         └─────────┘
    │                     │                     │
    │                     │                     │
 Passive UI        Presentation            Business
 Implements        Logic + Tests           Logic
 IView
```

### Key Principles:

1. **View (Form)**:
   - Implements `IView` interface
   - Passive - no business logic
   - Raises events, Presenter handles them
   - Updates UI when Presenter tells it to

2. **Presenter**:
   - Holds reference to `IView` (not concrete Form)
   - Contains presentation logic
   - Calls Services for business operations
   - Testable (no UI dependencies)

3. **Model/Service**:
   - Business logic
   - Data access
   - Independent of UI

---

## 💻 Complete MVP Example

### Step 1: Define the View Interface

```csharp
// ICustomerView.cs
public interface ICustomerView
{
    // Properties for data binding
    int CustomerId { get; set; }
    string CustomerName { get; set; }
    string Email { get; set; }
    string Phone { get; set; }
    bool IsActive { get; set; }

    // Properties for UI state
    bool IsSaveButtonEnabled { get; set; }
    bool IsLoadingVisible { get; set; }

    // Events raised by View
    event EventHandler LoadRequested;
    event EventHandler SaveRequested;
    event EventHandler CancelRequested;

    // Methods for View to display data/errors
    void ShowError(string message);
    void ShowSuccess(string message);
    void Close();
}
```

### Step 2: Implement the View (Form)

```csharp
// CustomerForm.cs
public partial class CustomerForm : Form, ICustomerView
{
    // Presenter reference
    private readonly CustomerPresenter _presenter;

    // Constructor - receives presenter
    public CustomerForm(CustomerPresenter presenter)
    {
        InitializeComponent();
        _presenter = presenter;

        // Attach presenter to this view
        _presenter.AttachView(this);

        // Wire up UI events to View events
        btnSave.Click += (s, e) => SaveRequested?.Invoke(this, EventArgs.Empty);
        btnCancel.Click += (s, e) => CancelRequested?.Invoke(this, EventArgs.Empty);
        Load += (s, e) => LoadRequested?.Invoke(this, EventArgs.Empty);
    }

    // ICustomerView Properties
    public int CustomerId { get; set; }

    public string CustomerName
    {
        get => txtName.Text;
        set => txtName.Text = value;
    }

    public string Email
    {
        get => txtEmail.Text;
        set => txtEmail.Text = value;
    }

    public string Phone
    {
        get => txtPhone.Text;
        set => txtPhone.Text = value;
    }

    public bool IsActive
    {
        get => chkActive.Checked;
        set => chkActive.Checked = value;
    }

    public bool IsSaveButtonEnabled
    {
        get => btnSave.Enabled;
        set => btnSave.Enabled = value;
    }

    public bool IsLoadingVisible
    {
        get => lblLoading.Visible;
        set => lblLoading.Visible = value;
    }

    // ICustomerView Events
    public event EventHandler LoadRequested;
    public event EventHandler SaveRequested;
    public event EventHandler CancelRequested;

    // ICustomerView Methods
    public void ShowError(string message)
    {
        MessageBox.Show(message, "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public void ShowSuccess(string message)
    {
        MessageBox.Show(message, "Success",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    // Form is PASSIVE - no business logic!
    // All logic is in Presenter
}
```

### Step 3: Create the Presenter

```csharp
// CustomerPresenter.cs
public class CustomerPresenter
{
    private ICustomerView? _view;
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomerPresenter> _logger;
    private readonly int? _customerId;

    public CustomerPresenter(
        ICustomerService customerService,
        ILogger<CustomerPresenter> logger,
        int? customerId = null)
    {
        _customerService = customerService;
        _logger = logger;
        _customerId = customerId;
    }

    public void AttachView(ICustomerView view)
    {
        _view = view;

        // Subscribe to View events
        _view.LoadRequested += OnLoadRequested;
        _view.SaveRequested += OnSaveRequested;
        _view.CancelRequested += OnCancelRequested;
    }

    public void DetachView()
    {
        if (_view != null)
        {
            // Unsubscribe from events
            _view.LoadRequested -= OnLoadRequested;
            _view.SaveRequested -= OnSaveRequested;
            _view.CancelRequested -= OnCancelRequested;
            _view = null;
        }
    }

    private async void OnLoadRequested(object? sender, EventArgs e)
    {
        if (_view == null) return;

        try
        {
            _view.IsLoadingVisible = true;
            _view.IsSaveButtonEnabled = false;

            if (_customerId.HasValue)
            {
                // Edit mode - load existing customer
                var customer = await _customerService.GetByIdAsync(_customerId.Value);

                if (customer != null)
                {
                    // Update View with customer data
                    _view.CustomerId = customer.Id;
                    _view.CustomerName = customer.Name;
                    _view.Email = customer.Email;
                    _view.Phone = customer.Phone;
                    _view.IsActive = customer.IsActive;
                }
                else
                {
                    _view.ShowError("Customer not found");
                    _view.Close();
                }
            }
            else
            {
                // Add mode - initialize empty form
                _view.CustomerId = 0;
                _view.CustomerName = string.Empty;
                _view.Email = string.Empty;
                _view.Phone = string.Empty;
                _view.IsActive = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading customer");
            _view.ShowError("Failed to load customer data");
        }
        finally
        {
            _view.IsLoadingVisible = false;
            _view.IsSaveButtonEnabled = true;
        }
    }

    private async void OnSaveRequested(object? sender, EventArgs e)
    {
        if (_view == null) return;

        try
        {
            // Validation (Presentation logic)
            if (string.IsNullOrWhiteSpace(_view.CustomerName))
            {
                _view.ShowError("Customer name is required");
                return;
            }

            if (string.IsNullOrWhiteSpace(_view.Email))
            {
                _view.ShowError("Email is required");
                return;
            }

            if (!IsValidEmail(_view.Email))
            {
                _view.ShowError("Invalid email format");
                return;
            }

            _view.IsLoadingVisible = true;
            _view.IsSaveButtonEnabled = false;

            // Create model from View data
            var customer = new Customer
            {
                Id = _view.CustomerId,
                Name = _view.CustomerName,
                Email = _view.Email,
                Phone = _view.Phone,
                IsActive = _view.IsActive
            };

            // Save via Service (Business logic)
            var success = await _customerService.SaveAsync(customer);

            if (success)
            {
                _logger.LogInformation($"Customer saved: {customer.Name}");
                _view.ShowSuccess("Customer saved successfully");
                _view.Close();
            }
            else
            {
                _view.ShowError("Failed to save customer");
            }
        }
        catch (ValidationException vex)
        {
            _view.ShowError(vex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving customer");
            _view.ShowError("An error occurred while saving");
        }
        finally
        {
            _view.IsLoadingVisible = false;
            _view.IsSaveButtonEnabled = true;
        }
    }

    private void OnCancelRequested(object? sender, EventArgs e)
    {
        _view?.Close();
    }

    // Presentation logic helper methods
    private bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
}
```

### Step 4: Register with DI Container

```csharp
// Program.cs
private static void ConfigureServices(ServiceCollection services)
{
    // Services
    services.AddSingleton<ICustomerService, CustomerService>();

    // Presenters
    services.AddTransient<CustomerPresenter>();

    // Forms
    services.AddTransient<CustomerForm>();
}
```

### Step 5: Create Forms with MVP

```csharp
// MainForm.cs
public partial class MainForm : Form
{
    private readonly IServiceProvider _serviceProvider;

    public MainForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
    }

    private void BtnAddCustomer_Click(object sender, EventArgs e)
    {
        // Create presenter (add mode)
        var presenter = _serviceProvider.GetRequiredService<CustomerPresenter>();

        // Create view with presenter
        var form = new CustomerForm(presenter);
        form.ShowDialog();

        RefreshCustomerGrid();
    }

    private void BtnEditCustomer_Click(object sender, EventArgs e)
    {
        if (dgvCustomers.SelectedRows.Count == 0) return;

        int customerId = (int)dgvCustomers.SelectedRows[0].Cells["Id"].Value;

        // Create presenter with customerId (edit mode)
        var customerService = _serviceProvider.GetRequiredService<ICustomerService>();
        var logger = _serviceProvider.GetRequiredService<ILogger<CustomerPresenter>>();
        var presenter = new CustomerPresenter(customerService, logger, customerId);

        // Create view
        var form = new CustomerForm(presenter);
        form.ShowDialog();

        RefreshCustomerGrid();
    }
}
```

---

## 🧪 Unit Testing the Presenter

The beauty of MVP: **Presenter is fully testable without UI!**

```csharp
// CustomerPresenterTests.cs
public class CustomerPresenterTests
{
    private Mock<ICustomerView> _mockView;
    private Mock<ICustomerService> _mockService;
    private Mock<ILogger<CustomerPresenter>> _mockLogger;
    private CustomerPresenter _presenter;

    public CustomerPresenterTests()
    {
        _mockView = new Mock<ICustomerView>();
        _mockService = new Mock<ICustomerService>();
        _mockLogger = new Mock<ILogger<CustomerPresenter>>();
    }

    [Fact]
    public async Task OnLoad_EditMode_LoadsCustomerData()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com",
            Phone = "123-456-7890",
            IsActive = true
        };

        _mockService
            .Setup(s => s.GetByIdAsync(1))
            .ReturnsAsync(customer);

        _presenter = new CustomerPresenter(_mockService.Object, _mockLogger.Object, customerId: 1);
        _presenter.AttachView(_mockView.Object);

        // Act
        _mockView.Raise(v => v.LoadRequested += null, EventArgs.Empty);
        await Task.Delay(100); // Wait for async operation

        // Assert
        _mockView.VerifySet(v => v.CustomerName = "John Doe");
        _mockView.VerifySet(v => v.Email = "john@example.com");
        _mockView.VerifySet(v => v.Phone = "123-456-7890");
        _mockView.VerifySet(v => v.IsActive = true);
    }

    [Fact]
    public async Task OnSave_EmptyName_ShowsError()
    {
        // Arrange
        _mockView.SetupGet(v => v.CustomerName).Returns(string.Empty);
        _mockView.SetupGet(v => v.Email).Returns("test@example.com");

        _presenter = new CustomerPresenter(_mockService.Object, _mockLogger.Object);
        _presenter.AttachView(_mockView.Object);

        // Act
        _mockView.Raise(v => v.SaveRequested += null, EventArgs.Empty);
        await Task.Delay(100);

        // Assert
        _mockView.Verify(v => v.ShowError("Customer name is required"), Times.Once);
        _mockService.Verify(s => s.SaveAsync(It.IsAny<Customer>()), Times.Never);
    }

    [Fact]
    public async Task OnSave_ValidData_SavesCustomer()
    {
        // Arrange
        _mockView.SetupGet(v => v.CustomerId).Returns(0);
        _mockView.SetupGet(v => v.CustomerName).Returns("Jane Doe");
        _mockView.SetupGet(v => v.Email).Returns("jane@example.com");
        _mockView.SetupGet(v => v.Phone).Returns("987-654-3210");
        _mockView.SetupGet(v => v.IsActive).Returns(true);

        _mockService
            .Setup(s => s.SaveAsync(It.IsAny<Customer>()))
            .ReturnsAsync(true);

        _presenter = new CustomerPresenter(_mockService.Object, _mockLogger.Object);
        _presenter.AttachView(_mockView.Object);

        // Act
        _mockView.Raise(v => v.SaveRequested += null, EventArgs.Empty);
        await Task.Delay(100);

        // Assert
        _mockService.Verify(s => s.SaveAsync(It.Is<Customer>(c =>
            c.Name == "Jane Doe" &&
            c.Email == "jane@example.com"
        )), Times.Once);

        _mockView.Verify(v => v.ShowSuccess("Customer saved successfully"), Times.Once);
        _mockView.Verify(v => v.Close(), Times.Once);
    }
}
```

---

## ✅ MVP Best Practices

### DO:
✅ View implements `IView` interface
✅ Presenter holds `IView` reference (not concrete Form)
✅ View is passive - raises events only
✅ All presentation logic in Presenter
✅ Presenter is testable (no UI dependencies)
✅ Use Dependency Injection for Presenter
✅ Detach View in Form.Disposed event

### DON'T:
❌ Don't put business logic in Presenter (use Service instead)
❌ Don't reference concrete Form in Presenter
❌ Don't create UI controls in Presenter
❌ Don't access Form controls directly from Presenter
❌ Don't forget to unsubscribe from events (memory leaks!)

---

## 🆚 MVP vs MVVM vs MVC

| Feature | MVP | MVVM | MVC |
|---------|-----|------|-----|
| **Data Binding** | Manual | Two-way automatic | Manual |
| **View Knowledge** | View knows Presenter | View knows ViewModel | View knows nothing |
| **Best For** | WinForms, Web Forms | WPF, UWP | Web (ASP.NET) |
| **Testability** | Excellent | Excellent | Good |
| **Complexity** | Medium | Medium-High | Medium |
| **.NET Support** | All versions | .NET 7+ for WinForms | Web only |

**Recommendation**:
- **WinForms**: Use MVP (best fit)
- **WinForms with .NET 8+**: Consider MVVM if you need strong data binding
- **Web Apps**: Use MVC

---

## 🔗 Related Topics

- [MVVM Pattern](mvvm-pattern.md) - Alternative for .NET 8+
- [Dependency Injection](dependency-injection.md) - DI setup for MVP
- [Form Communication](../ui-ux/form-communication.md) - Communicating between presenters
- [Unit Testing](../testing/unit-testing.md) - Testing presenters

---

## 📚 References

- [Martin Fowler - MVP Pattern](https://martinfowler.com/eaaDev/uiArchs.html)
- [Microsoft CAB Framework](https://docs.microsoft.com/en-us/previous-versions/msp-n-p/ff648951(v=pandp.10))

---

**Last Updated**: 2025-11-07
