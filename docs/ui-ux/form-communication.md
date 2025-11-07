# Form Communication in WinForms

> **Quick Reference**: Patterns and best practices for communicating between forms in WinForms applications. Choose the right pattern for maintainable, loosely-coupled code.

---

## 📋 Overview

**Form communication** is one of the most common challenges in WinForms development. When you have multiple forms that need to share data or trigger actions in each other, choosing the right communication pattern is critical for maintainability.

### Common Scenarios:
- Opening a dialog to select an item and return the selection
- Refreshing data in a parent form after a child form saves changes
- Passing configuration or context data to a new form
- Notifying multiple forms about state changes
- Implementing master-detail relationships

### Why Proper Communication Matters:
- **Maintainability** - Loosely coupled forms are easier to modify
- **Testability** - Decoupled forms can be tested independently
- **Reusability** - Forms with clean interfaces can be reused
- **Scalability** - Proper patterns prevent "spaghetti code" as apps grow

---

## 🎯 Why This Matters

### The Problem:
```csharp
// ❌ BAD - Tight coupling, hard to maintain
public partial class CustomerForm : Form
{
    private MainForm _mainForm; // Direct reference to parent!

    private void btnSave_Click(object sender, EventArgs e)
    {
        SaveCustomer();
        _mainForm.RefreshCustomerGrid(); // Tightly coupled!
        _mainForm.UpdateStatusBar("Customer saved");
    }
}
```

**Problems with this approach:**
- CustomerForm can't be reused without MainForm
- Hard to test CustomerForm independently
- Changes to MainForm break CustomerForm
- Violates Dependency Inversion Principle

### The Solution:
Use proper communication patterns (events, callbacks, shared services) to decouple forms and make them reusable and testable.

---

## 📡 Communication Patterns

## Pattern 1: Constructor Parameters

**Use Case**: Passing initial data or context when creating a form.

### Basic Example:
```csharp
// CustomerForm.cs
public partial class CustomerForm : Form
{
    private readonly int _customerId;
    private readonly bool _isEditMode;

    // Constructor receives data
    public CustomerForm(int customerId, bool isEditMode)
    {
        InitializeComponent();
        _customerId = customerId;
        _isEditMode = isEditMode;
    }

    private async void CustomerForm_Load(object sender, EventArgs e)
    {
        if (_isEditMode)
        {
            await LoadCustomerAsync(_customerId);
        }
    }
}

// Usage from parent form
private void btnEditCustomer_Click(object sender, EventArgs e)
{
    int selectedId = GetSelectedCustomerId();
    using var form = new CustomerForm(selectedId, isEditMode: true);
    form.ShowDialog();
}
```

### Pros:
✅ Simple and straightforward
✅ Data immutable after construction
✅ Clear what data is required

### Cons:
❌ Can only pass data one way (to child)
❌ Constructors can become large with many parameters
❌ No way to get data back from child form

---

## Pattern 2: Public Properties

**Use Case**: Setting optional data after construction or getting results from a dialog.

### Example - Setting Data:
```csharp
// OrderForm.cs
public partial class OrderForm : Form
{
    // Public properties for data
    public int CustomerId { get; set; }
    public decimal DiscountPercent { get; set; }
    public bool IsRushOrder { get; set; }

    private async void OrderForm_Load(object sender, EventArgs e)
    {
        await LoadCustomerAsync(CustomerId);
        txtDiscount.Text = DiscountPercent.ToString();
        chkRush.Checked = IsRushOrder;
    }
}

// Usage
private void btnCreateOrder_Click(object sender, EventArgs e)
{
    var form = new OrderForm
    {
        CustomerId = GetSelectedCustomerId(),
        DiscountPercent = 10.0m,
        IsRushOrder = false
    };
    form.ShowDialog();
}
```

### Example - Getting Results:
```csharp
// CustomerSelectionDialog.cs
public partial class CustomerSelectionDialog : Form
{
    // Property to return selected customer
    public int? SelectedCustomerId { get; private set; }
    public string SelectedCustomerName { get; private set; }

    private void btnSelect_Click(object sender, EventArgs e)
    {
        if (dgvCustomers.SelectedRows.Count > 0)
        {
            SelectedCustomerId = (int)dgvCustomers.SelectedRows[0].Cells["Id"].Value;
            SelectedCustomerName = dgvCustomers.SelectedRows[0].Cells["Name"].Value.ToString();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

// Usage - Get selected customer
private void btnSelectCustomer_Click(object sender, EventArgs e)
{
    using var dialog = new CustomerSelectionDialog();
    if (dialog.ShowDialog() == DialogResult.OK)
    {
        int customerId = dialog.SelectedCustomerId.Value;
        string customerName = dialog.SelectedCustomerName;
        lblSelectedCustomer.Text = $"Selected: {customerName}";
    }
}
```

### Pros:
✅ Flexible - can set/get multiple values
✅ Works well with object initializers
✅ Good for dialog return values

### Cons:
❌ Properties might be set in wrong order
❌ No validation that required properties are set
❌ Still one-way communication

---

## Pattern 3: Events (RECOMMENDED)

**Use Case**: Notifying parent form of changes without tight coupling.

### Complete Example:
```csharp
// 1. Define custom EventArgs
public class CustomerSavedEventArgs : EventArgs
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public DateTime SavedAt { get; set; }
}

// 2. CustomerForm raises event
public partial class CustomerForm : Form
{
    // Define event
    public event EventHandler<CustomerSavedEventArgs>? CustomerSaved;

    private async void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            var customer = new Customer
            {
                Name = txtName.Text,
                Email = txtEmail.Text
            };

            int customerId = await _service.SaveCustomerAsync(customer);

            // Raise event - notify any subscribers
            CustomerSaved?.Invoke(this, new CustomerSavedEventArgs
            {
                CustomerId = customerId,
                CustomerName = customer.Name,
                SavedAt = DateTime.Now
            });

            MessageBox.Show("Customer saved successfully!");
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
        }
    }
}

// 3. Parent form subscribes to event
public partial class MainForm : Form
{
    private void btnAddCustomer_Click(object sender, EventArgs e)
    {
        var form = new CustomerForm();

        // Subscribe to event
        form.CustomerSaved += OnCustomerSaved;

        form.ShowDialog();

        // Unsubscribe to prevent memory leaks
        form.CustomerSaved -= OnCustomerSaved;
    }

    private void OnCustomerSaved(object? sender, CustomerSavedEventArgs e)
    {
        // Refresh grid
        RefreshCustomerGrid();

        // Update status
        lblStatus.Text = $"Customer '{e.CustomerName}' saved at {e.SavedAt:HH:mm:ss}";

        // Log activity
        _logger.LogInformation($"Customer saved: {e.CustomerName} (ID: {e.CustomerId})");
    }
}
```

### Advanced - Multiple Event Subscribers:
```csharp
// Multiple forms can subscribe to the same event
public partial class DashboardForm : Form
{
    private void OpenCustomerForm()
    {
        var form = new CustomerForm();
        form.CustomerSaved += OnCustomerSaved; // Dashboard subscribes
        form.ShowDialog();
        form.CustomerSaved -= OnCustomerSaved;
    }

    private void OnCustomerSaved(object? sender, CustomerSavedEventArgs e)
    {
        // Update dashboard statistics
        UpdateCustomerCount();
        ShowNotification($"New customer: {e.CustomerName}");
    }
}
```

### Pros:
✅ **Loose coupling** - child doesn't know about parent
✅ **Reusable** - any form can subscribe to events
✅ **Testable** - can test event raising independently
✅ **Multiple subscribers** - many forms can listen
✅ **Standard .NET pattern** - familiar to all C# developers

### Cons:
❌ Requires unsubscribing to prevent memory leaks
❌ Slightly more code than properties
❌ Events can't return values to child

---

## Pattern 4: Callbacks/Delegates

**Use Case**: Simple one-way notifications or when you need to return a value.

### Using Action Delegates:
```csharp
// CustomerForm.cs
public partial class CustomerForm : Form
{
    private Action<Customer>? _onCustomerSaved;

    // Constructor accepts callback
    public CustomerForm(Action<Customer>? onCustomerSaved = null)
    {
        InitializeComponent();
        _onCustomerSaved = onCustomerSaved;
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        var customer = new Customer
        {
            Name = txtName.Text,
            Email = txtEmail.Text
        };

        int customerId = await _service.SaveCustomerAsync(customer);
        customer.Id = customerId;

        // Invoke callback
        _onCustomerSaved?.Invoke(customer);

        Close();
    }
}

// Usage
private void btnAddCustomer_Click(object sender, EventArgs e)
{
    var form = new CustomerForm(customer =>
    {
        // Callback executed when customer is saved
        RefreshCustomerGrid();
        lblStatus.Text = $"Added: {customer.Name}";
    });

    form.ShowDialog();
}
```

### Using Func for Return Values:
```csharp
// ValidationDialog.cs
public partial class ValidationDialog : Form
{
    private Func<string, bool>? _validateInput;

    public ValidationDialog(Func<string, bool> validateInput)
    {
        InitializeComponent();
        _validateInput = validateInput;
    }

    private void btnValidate_Click(object sender, EventArgs e)
    {
        string input = txtInput.Text;

        // Call validation function from parent
        bool isValid = _validateInput?.Invoke(input) ?? false;

        lblResult.Text = isValid ? "Valid!" : "Invalid!";
    }
}

// Usage
private void btnOpenValidator_Click(object sender, EventArgs e)
{
    var form = new ValidationDialog(input =>
    {
        // Custom validation logic
        return input.Length >= 5 && input.Contains("@");
    });

    form.ShowDialog();
}
```

### Pros:
✅ Simple and concise
✅ Works well with lambdas
✅ Can pass complex logic
✅ Func can return values

### Cons:
❌ Less discoverable than events
❌ Can become complex with multiple callbacks
❌ Harder to test

---

## Pattern 5: Shared Service (DI)

**Use Case**: Complex scenarios with shared state across multiple forms.

### Complete Example:
```csharp
// 1. Define shared service
public interface ICustomerStateService
{
    int? CurrentCustomerId { get; set; }
    Customer? CurrentCustomer { get; set; }

    event EventHandler<CustomerChangedEventArgs>? CustomerChanged;

    void SelectCustomer(Customer customer);
    void ClearSelection();
}

public class CustomerStateService : ICustomerStateService
{
    public int? CurrentCustomerId { get; set; }
    public Customer? CurrentCustomer { get; set; }

    public event EventHandler<CustomerChangedEventArgs>? CustomerChanged;

    public void SelectCustomer(Customer customer)
    {
        CurrentCustomerId = customer.Id;
        CurrentCustomer = customer;

        // Notify all subscribers
        CustomerChanged?.Invoke(this, new CustomerChangedEventArgs
        {
            Customer = customer
        });
    }

    public void ClearSelection()
    {
        CurrentCustomerId = null;
        CurrentCustomer = null;
        CustomerChanged?.Invoke(this, new CustomerChangedEventArgs());
    }
}

// 2. Register as singleton in DI
// Program.cs
private static void ConfigureServices(ServiceCollection services)
{
    services.AddSingleton<ICustomerStateService, CustomerStateService>();
    services.AddTransient<MainForm>();
    services.AddTransient<CustomerListForm>();
    services.AddTransient<OrderForm>();
}

// 3. Forms inject and use shared service
public partial class CustomerListForm : Form
{
    private readonly ICustomerStateService _stateService;

    public CustomerListForm(ICustomerStateService stateService)
    {
        _stateService = stateService;
        InitializeComponent();
    }

    private void dgvCustomers_SelectionChanged(object sender, EventArgs e)
    {
        if (dgvCustomers.SelectedRows.Count > 0)
        {
            var customer = (Customer)dgvCustomers.SelectedRows[0].DataBoundItem;

            // Update shared state - all forms will be notified
            _stateService.SelectCustomer(customer);
        }
    }
}

public partial class OrderForm : Form
{
    private readonly ICustomerStateService _stateService;

    public OrderForm(ICustomerStateService stateService)
    {
        _stateService = stateService;
        InitializeComponent();

        // Subscribe to state changes
        _stateService.CustomerChanged += OnCustomerChanged;
    }

    private void OnCustomerChanged(object? sender, CustomerChangedEventArgs e)
    {
        if (e.Customer != null)
        {
            // Update UI with selected customer
            lblCustomer.Text = e.Customer.Name;
            txtEmail.Text = e.Customer.Email;
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        // Unsubscribe to prevent memory leaks
        _stateService.CustomerChanged -= OnCustomerChanged;
        base.OnFormClosed(e);
    }
}
```

### Pros:
✅ **Best for complex scenarios** - multiple forms sharing state
✅ **Fully decoupled** - forms don't know about each other
✅ **Testable** - can mock the service
✅ **Centralized state** - single source of truth

### Cons:
❌ More setup required (DI container)
❌ Overkill for simple scenarios
❌ Must manage subscriptions carefully

---

## Pattern 6: Mediator Pattern

**Use Case**: Complex form communication with many-to-many relationships.

### Implementation:
```csharp
// 1. Define mediator interface
public interface IFormMediator
{
    void Register(string channel, Form form, Action<object> handler);
    void Unregister(string channel, Form form);
    void Send(string channel, object data);
}

// 2. Implement mediator
public class FormMediator : IFormMediator
{
    private readonly Dictionary<string, List<(Form form, Action<object> handler)>> _channels = new();

    public void Register(string channel, Form form, Action<object> handler)
    {
        if (!_channels.ContainsKey(channel))
        {
            _channels[channel] = new List<(Form, Action<object>)>();
        }

        _channels[channel].Add((form, handler));
    }

    public void Unregister(string channel, Form form)
    {
        if (_channels.ContainsKey(channel))
        {
            _channels[channel].RemoveAll(x => x.form == form);
        }
    }

    public void Send(string channel, object data)
    {
        if (_channels.ContainsKey(channel))
        {
            foreach (var (form, handler) in _channels[channel])
            {
                if (form.IsHandleCreated && !form.IsDisposed)
                {
                    form.Invoke(() => handler(data));
                }
            }
        }
    }
}

// 3. Register mediator in DI
services.AddSingleton<IFormMediator, FormMediator>();

// 4. Forms use mediator
public partial class CustomerForm : Form
{
    private readonly IFormMediator _mediator;

    public CustomerForm(IFormMediator mediator)
    {
        _mediator = mediator;
        InitializeComponent();
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        var customer = await SaveCustomerAsync();

        // Broadcast to all subscribers on "customer.saved" channel
        _mediator.Send("customer.saved", new { Customer = customer });

        Close();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _mediator.Unregister("customer.saved", this);
        base.OnFormClosed(e);
    }
}

public partial class DashboardForm : Form
{
    private readonly IFormMediator _mediator;

    public DashboardForm(IFormMediator mediator)
    {
        _mediator = mediator;
        InitializeComponent();

        // Subscribe to customer saved events
        _mediator.Register("customer.saved", this, OnCustomerSaved);
    }

    private void OnCustomerSaved(object data)
    {
        dynamic eventData = data;
        UpdateStatistics();
        ShowNotification($"Customer saved: {eventData.Customer.Name}");
    }
}
```

### Pros:
✅ **Completely decoupled** - forms don't reference each other
✅ **Flexible** - easy to add new communication channels
✅ **Scalable** - handles complex communication patterns

### Cons:
❌ Most complex pattern
❌ "Magic strings" for channels (can use constants)
❌ Harder to debug

---

## 🪟 Modal vs Modeless Dialogs

### Modal Dialogs - ShowDialog()
```csharp
// Modal dialog - blocks parent until closed
private void btnEdit_Click(object sender, EventArgs e)
{
    using var form = new CustomerForm(selectedId);

    DialogResult result = form.ShowDialog(); // Blocks here

    if (result == DialogResult.OK)
    {
        RefreshGrid();
    }
    else if (result == DialogResult.Cancel)
    {
        // User cancelled
    }
}

// CustomerForm.cs
private void btnSave_Click(object sender, EventArgs e)
{
    if (SaveCustomer())
    {
        DialogResult = DialogResult.OK; // Set result
        Close(); // Close will return to ShowDialog() caller
    }
}

private void btnCancel_Click(object sender, EventArgs e)
{
    DialogResult = DialogResult.Cancel;
    Close();
}
```

### Modeless Dialogs - Show()
```csharp
// Modeless dialog - parent remains active
private CustomerForm? _customerForm;

private void btnOpenCustomer_Click(object sender, EventArgs e)
{
    if (_customerForm == null || _customerForm.IsDisposed)
    {
        _customerForm = new CustomerForm();
        _customerForm.CustomerSaved += OnCustomerSaved;
        _customerForm.FormClosed += (s, e) => _customerForm = null;
        _customerForm.Show(); // Non-blocking
    }
    else
    {
        // Bring existing form to front
        _customerForm.BringToFront();
        _customerForm.Focus();
    }
}

private void OnCustomerSaved(object? sender, CustomerSavedEventArgs e)
{
    RefreshGrid();
}
```

### When to Use Each:
| Use Modal (ShowDialog) | Use Modeless (Show) |
|------------------------|---------------------|
| Login/authentication | Tool windows |
| Confirmations | Multi-document interface |
| Critical input required | Status/monitoring windows |
| Wizard workflows | Floating palettes |
| Settings dialogs | Find/Replace windows |

---

## 👨‍👦 Parent-Child Communication

### Child Accessing Parent (Avoid!)
```csharp
// ❌ BAD - Don't do this!
public partial class ChildForm : Form
{
    private void btnUpdate_Click(object sender, EventArgs e)
    {
        var parent = (MainForm)this.Owner; // Tight coupling!
        parent.RefreshData(); // Assumes Owner is MainForm
    }
}
```

### Better Approach - Events:
```csharp
// ✅ GOOD - Use events instead
public partial class ChildForm : Form
{
    public event EventHandler? DataChanged;

    private void btnUpdate_Click(object sender, EventArgs e)
    {
        SaveData();
        DataChanged?.Invoke(this, EventArgs.Empty); // Notify subscribers
    }
}

// Parent
private void OpenChild()
{
    var child = new ChildForm();
    child.DataChanged += (s, e) => RefreshData();
    child.Show();
}
```

### Best Practice - Interfaces:
```csharp
// Define interface for parent capabilities
public interface IDataRefreshable
{
    void RefreshData();
}

public partial class ChildForm : Form
{
    private readonly IDataRefreshable? _parent;

    public ChildForm(IDataRefreshable? parent = null)
    {
        _parent = parent;
        InitializeComponent();
    }

    private void btnUpdate_Click(object sender, EventArgs e)
    {
        SaveData();
        _parent?.RefreshData(); // Parent implements interface
    }
}

// Parent implements interface
public partial class MainForm : Form, IDataRefreshable
{
    public void RefreshData()
    {
        LoadCustomers();
    }

    private void OpenChild()
    {
        var child = new ChildForm(this); // Pass interface
        child.Show();
    }
}
```

---

## 🔄 Data Refresh Patterns

### Pattern 1: Direct Callback After Dialog
```csharp
private void btnEdit_Click(object sender, EventArgs e)
{
    using var form = new CustomerForm(selectedId);

    if (form.ShowDialog() == DialogResult.OK)
    {
        // Refresh immediately after dialog closes
        await LoadCustomersAsync();
    }
}
```

### Pattern 2: Event-Based Refresh
```csharp
private void btnEdit_Click(object sender, EventArgs e)
{
    var form = new CustomerForm(selectedId);
    form.CustomerSaved += async (s, e) =>
    {
        // Refresh when event is raised
        await LoadCustomersAsync();
    };
    form.ShowDialog();
}
```

### Pattern 3: Observer Pattern
```csharp
// Observable service
public interface ICustomerRepository
{
    event EventHandler<CustomerChangedEventArgs>? CustomerChanged;
    Task SaveAsync(Customer customer);
}

// Forms subscribe to repository
public partial class CustomerGridForm : Form
{
    private readonly ICustomerRepository _repository;

    public CustomerGridForm(ICustomerRepository repository)
    {
        _repository = repository;
        _repository.CustomerChanged += OnCustomerChanged;
        InitializeComponent();
    }

    private async void OnCustomerChanged(object? sender, CustomerChangedEventArgs e)
    {
        // Auto-refresh when any form saves a customer
        await LoadCustomersAsync();
    }
}
```

---

## ✅ Best Practices

### DO:
✅ **Use events for loose coupling** - Child raises events, parent subscribes
✅ **Use shared services with DI** - For complex state management
✅ **Unsubscribe from events** - In Form.Disposed or FormClosed to prevent memory leaks
✅ **Pass interfaces, not concrete forms** - IView, IDataRefreshable, etc.
✅ **Use DialogResult** - For modal dialogs to indicate outcome
✅ **Document communication** - Comment which events/callbacks are expected

### DON'T:
❌ **Don't reference parent forms directly** - Use events or callbacks instead
❌ **Don't use static/global variables** - Creates hidden dependencies
❌ **Don't forget to dispose forms** - Use `using` or call Dispose()
❌ **Don't access controls directly** - Use properties or methods
❌ **Don't create circular references** - Parent → Child → Parent
❌ **Don't pass too many parameters** - Use a context object or service

---

## 💡 Complete Working Examples

### Example 1: Customer Selection Dialog

```csharp
// CustomerSelectionDialog.cs - Reusable customer picker
public partial class CustomerSelectionDialog : Form
{
    private readonly ICustomerService _service;
    private List<Customer> _customers = new();

    public Customer? SelectedCustomer { get; private set; }

    public CustomerSelectionDialog(ICustomerService service)
    {
        _service = service;
        InitializeComponent();
    }

    private async void CustomerSelectionDialog_Load(object sender, EventArgs e)
    {
        await LoadCustomersAsync();
    }

    private async Task LoadCustomersAsync()
    {
        try
        {
            _customers = await _service.GetAllAsync();
            dgvCustomers.DataSource = _customers;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading customers: {ex.Message}");
        }
    }

    private void dgvCustomers_DoubleClick(object sender, EventArgs e)
    {
        SelectCurrentCustomer();
    }

    private void btnSelect_Click(object sender, EventArgs e)
    {
        SelectCurrentCustomer();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void SelectCurrentCustomer()
    {
        if (dgvCustomers.SelectedRows.Count > 0)
        {
            SelectedCustomer = (Customer)dgvCustomers.SelectedRows[0].DataBoundItem;
            DialogResult = DialogResult.OK;
            Close();
        }
        else
        {
            MessageBox.Show("Please select a customer.");
        }
    }
}

// Usage in any form
private async void btnSelectCustomer_Click(object sender, EventArgs e)
{
    var service = _serviceProvider.GetRequiredService<ICustomerService>();
    using var dialog = new CustomerSelectionDialog(service);

    if (dialog.ShowDialog() == DialogResult.OK && dialog.SelectedCustomer != null)
    {
        var customer = dialog.SelectedCustomer;
        txtCustomerId.Text = customer.Id.ToString();
        txtCustomerName.Text = customer.Name;
        txtEmail.Text = customer.Email;
    }
}
```

### Example 2: Master-Detail Forms

```csharp
// MasterForm.cs - Shows list of orders
public partial class OrderMasterForm : Form
{
    private readonly IOrderService _orderService;
    private readonly IServiceProvider _serviceProvider;

    public OrderMasterForm(IOrderService orderService, IServiceProvider serviceProvider)
    {
        _orderService = orderService;
        _serviceProvider = serviceProvider;
        InitializeComponent();
    }

    private async void OrderMasterForm_Load(object sender, EventArgs e)
    {
        await LoadOrdersAsync();
    }

    private async Task LoadOrdersAsync()
    {
        var orders = await _orderService.GetAllAsync();
        dgvOrders.DataSource = orders;
    }

    private void btnViewDetails_Click(object sender, EventArgs e)
    {
        if (dgvOrders.SelectedRows.Count == 0) return;

        int orderId = (int)dgvOrders.SelectedRows[0].Cells["Id"].Value;

        // Open detail form
        var detailForm = _serviceProvider.GetRequiredService<OrderDetailForm>();
        detailForm.OrderId = orderId;
        detailForm.OrderUpdated += OnOrderUpdated; // Subscribe to updates
        detailForm.Show(); // Modeless - can have multiple detail windows open
    }

    private async void OnOrderUpdated(object? sender, OrderUpdatedEventArgs e)
    {
        // Refresh list when detail form updates an order
        await LoadOrdersAsync();

        // Select the updated order
        foreach (DataGridViewRow row in dgvOrders.Rows)
        {
            if ((int)row.Cells["Id"].Value == e.OrderId)
            {
                row.Selected = true;
                dgvOrders.FirstDisplayedScrollingRowIndex = row.Index;
                break;
            }
        }
    }
}

// DetailForm.cs - Shows order details
public partial class OrderDetailForm : Form
{
    private readonly IOrderService _orderService;

    public int OrderId { get; set; }
    public event EventHandler<OrderUpdatedEventArgs>? OrderUpdated;

    public OrderDetailForm(IOrderService orderService)
    {
        _orderService = orderService;
        InitializeComponent();
    }

    private async void OrderDetailForm_Load(object sender, EventArgs e)
    {
        await LoadOrderAsync();
    }

    private async Task LoadOrderAsync()
    {
        var order = await _orderService.GetByIdAsync(OrderId);
        if (order != null)
        {
            txtOrderNumber.Text = order.OrderNumber;
            txtCustomer.Text = order.CustomerName;
            dgvItems.DataSource = order.Items;
        }
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        // Save changes
        await _orderService.UpdateAsync(OrderId, GetOrderFromForm());

        // Notify subscribers
        OrderUpdated?.Invoke(this, new OrderUpdatedEventArgs { OrderId = OrderId });

        MessageBox.Show("Order updated successfully!");
    }
}
```

### Example 3: Multi-Step Wizard

```csharp
// WizardCoordinator.cs - Manages wizard flow
public class WizardCoordinator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly WizardData _data = new();

    public WizardCoordinator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public bool StartWizard()
    {
        // Step 1: Customer Selection
        if (!ShowCustomerStep()) return false;

        // Step 2: Product Selection
        if (!ShowProductStep()) return false;

        // Step 3: Review and Confirm
        if (!ShowReviewStep()) return false;

        // All steps completed
        return true;
    }

    private bool ShowCustomerStep()
    {
        var step = _serviceProvider.GetRequiredService<WizardStep1CustomerForm>();
        step.WizardData = _data;

        return step.ShowDialog() == DialogResult.OK;
    }

    private bool ShowProductStep()
    {
        var step = _serviceProvider.GetRequiredService<WizardStep2ProductForm>();
        step.WizardData = _data;

        return step.ShowDialog() == DialogResult.OK;
    }

    private bool ShowReviewStep()
    {
        var step = _serviceProvider.GetRequiredService<WizardStep3ReviewForm>();
        step.WizardData = _data;

        return step.ShowDialog() == DialogResult.OK;
    }
}

// Shared data object
public class WizardData
{
    public Customer? SelectedCustomer { get; set; }
    public List<Product> SelectedProducts { get; set; } = new();
    public decimal TotalAmount { get; set; }
}

// Step 1
public partial class WizardStep1CustomerForm : Form
{
    public WizardData? WizardData { get; set; }

    private void btnNext_Click(object sender, EventArgs e)
    {
        if (ValidateStep())
        {
            WizardData!.SelectedCustomer = GetSelectedCustomer();
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}

// Usage
private async void btnStartWizard_Click(object sender, EventArgs e)
{
    var coordinator = new WizardCoordinator(_serviceProvider);

    if (coordinator.StartWizard())
    {
        MessageBox.Show("Wizard completed successfully!");
        await RefreshDataAsync();
    }
}
```

---

## 🚫 Common Mistakes

### 1. Tight Coupling
```csharp
// ❌ BAD
public partial class ChildForm : Form
{
    public MainForm ParentForm { get; set; } // Direct reference!

    private void btnSave_Click(object sender, EventArgs e)
    {
        SaveData();
        ParentForm.RefreshGrid(); // Tightly coupled
    }
}

// ✅ GOOD
public partial class ChildForm : Form
{
    public event EventHandler? DataSaved;

    private void btnSave_Click(object sender, EventArgs e)
    {
        SaveData();
        DataSaved?.Invoke(this, EventArgs.Empty); // Loosely coupled
    }
}
```

### 2. Memory Leaks from Events
```csharp
// ❌ BAD - Memory leak!
private void btnOpen_Click(object sender, EventArgs e)
{
    var form = new CustomerForm();
    form.CustomerSaved += OnCustomerSaved; // Subscribed
    form.Show();
    // Never unsubscribed - form can't be garbage collected!
}

// ✅ GOOD
private void btnOpen_Click(object sender, EventArgs e)
{
    var form = new CustomerForm();
    form.CustomerSaved += OnCustomerSaved;
    form.FormClosed += (s, e) => form.CustomerSaved -= OnCustomerSaved; // Unsubscribe
    form.Show();
}
```

### 3. Using Static Variables
```csharp
// ❌ BAD - Global state
public static class AppState
{
    public static Customer? CurrentCustomer { get; set; } // Global!
}

public partial class Form1 : Form
{
    private void SelectCustomer()
    {
        AppState.CurrentCustomer = selectedCustomer; // Hidden dependency
    }
}

// ✅ GOOD - Use DI service
services.AddSingleton<IAppStateService, AppStateService>();
```

### 4. Forgetting to Dispose
```csharp
// ❌ BAD - Resource leak
private void btnOpen_Click(object sender, EventArgs e)
{
    var form = new CustomerForm();
    form.ShowDialog(); // Form never disposed!
}

// ✅ GOOD
private void btnOpen_Click(object sender, EventArgs e)
{
    using var form = new CustomerForm();
    form.ShowDialog(); // Disposed automatically
}
```

### 5. Accessing Disposed Forms
```csharp
// ❌ BAD
private CustomerForm? _form;

private void btnOpen_Click(object sender, EventArgs e)
{
    _form = new CustomerForm();
    _form.Show();
}

private void btnUpdate_Click(object sender, EventArgs e)
{
    _form.UpdateData(); // May be disposed!
}

// ✅ GOOD
private CustomerForm? _form;

private void btnOpen_Click(object sender, EventArgs e)
{
    if (_form == null || _form.IsDisposed)
    {
        _form = new CustomerForm();
        _form.Show();
    }
    else
    {
        _form.BringToFront();
    }
}
```

---

## 🔗 Related Topics

- [MVP Pattern](../architecture/mvp-pattern.md) - Using presenters for form communication
- [MVVM Pattern](../architecture/mvvm-pattern.md) - ViewModels for data sharing
- [Dependency Injection](../architecture/dependency-injection.md) - DI setup for shared services
- [Data Binding](data-binding.md) - Binding data between forms
- [Event Handling](../best-practices/error-handling.md) - Proper event patterns

---

## 📚 Summary

**Choose the right pattern:**

| Pattern | Best For | Coupling | Complexity |
|---------|----------|----------|------------|
| Constructor Parameters | Initial data | Low | Low |
| Public Properties | Dialog results | Low | Low |
| **Events** ⭐ | Most scenarios | Very Low | Medium |
| Callbacks | Simple notifications | Low | Low |
| Shared Service | Complex state | Very Low | High |
| Mediator | Many-to-many | None | High |

**Recommended approach for most applications:**
1. **Events** for parent-child communication
2. **Shared Service (DI)** for cross-form state
3. **DialogResult + Properties** for modal dialogs

---

**Last Updated**: 2025-11-07
