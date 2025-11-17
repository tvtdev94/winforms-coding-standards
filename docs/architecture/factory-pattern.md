# Factory Pattern for WinForms

## Overview

The **Factory Pattern** is a creational design pattern that provides an interface for creating objects without specifying their exact classes. In WinForms applications, it's used to create forms with proper dependency injection, replacing the Service Locator anti-pattern.

**Key Benefits:**
- ✅ Eliminates Service Locator anti-pattern
- ✅ Explicit dependencies in constructors
- ✅ Easier to test and mock
- ✅ Centralized form creation logic
- ✅ Type-safe form instantiation
- ✅ Better separation of concerns

---

## Architecture

```
┌─────────────────────────────────────┐
│   Presentation Layer (Forms)        │
│   MainForm                          │
│   ├─ Injects: IFormFactory          │
│   └─ Creates: CustomerForm          │
└──────────────┬──────────────────────┘
               │
               v
┌─────────────────────────────────────┐
│   Factory (IFormFactory)            │
│   FormFactory                       │
│   ├─ Uses: IServiceProvider         │ ← Only place for ServiceProvider
│   └─ Returns: Form instances        │
└──────────────┬──────────────────────┘
               │
               v
┌─────────────────────────────────────┐
│   DI Container                      │
│   ServiceProvider                   │
│   ├─ Resolves: Forms                │
│   ├─ Resolves: Services             │
│   └─ Resolves: Dependencies         │
└─────────────────────────────────────┘
```

---

## Problem: Service Locator Anti-Pattern

### ❌ Before (Service Locator - BAD)

```csharp
public class MainForm : Form
{
    private readonly IServiceProvider _serviceProvider; // ❌ Anti-pattern

    public MainForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
    }

    private void OnOpenCustomersClick(object sender, EventArgs e)
    {
        // ❌ Hidden dependency - hard to test
        var form = _serviceProvider.GetRequiredService<CustomerForm>();
        form.ShowDialog(this);
    }
}
```

**Problems:**
1. ❌ **Hidden dependencies** - You can't see what the form needs from its constructor
2. ❌ **Hard to test** - Have to mock entire IServiceProvider
3. ❌ **Service Locator anti-pattern** - Violates Dependency Inversion Principle
4. ❌ **Runtime errors** - Missing services only discovered when form opens
5. ❌ **Tight coupling** to DI container

---

## Solution: Factory Pattern

### ✅ After (Factory Pattern - GOOD)

```csharp
public class MainForm : Form
{
    private readonly IFormFactory _formFactory; // ✅ Clean dependency

    public MainForm(IFormFactory formFactory)
    {
        _formFactory = formFactory ?? throw new ArgumentNullException(nameof(formFactory));
        InitializeComponent();
    }

    private void OnOpenCustomersClick(object sender, EventArgs e)
    {
        // ✅ Explicit, testable, clean
        var form = _formFactory.Create<CustomerForm>();
        form.ShowDialog(this);
    }
}
```

**Benefits:**
1. ✅ **Explicit dependency** - Clear what the form needs
2. ✅ **Easy to test** - Simple to mock IFormFactory
3. ✅ **Factory Pattern** - Follows design pattern best practices
4. ✅ **Compile-time safety** - Missing registrations fail at startup
5. ✅ **Loose coupling** - No direct dependency on DI container

---

## Implementation

### 1. IFormFactory Interface

```csharp
using System.Windows.Forms;

namespace YourNamespace.Factories
{
    /// <summary>
    /// Factory interface for creating forms with dependency injection.
    /// This pattern replaces the Service Locator anti-pattern and provides
    /// better testability and explicit dependencies.
    /// </summary>
    public interface IFormFactory
    {
        /// <summary>
        /// Creates an instance of the specified form type with all dependencies resolved.
        /// </summary>
        /// <typeparam name="TForm">The form type to create.</typeparam>
        /// <returns>A fully initialized form instance.</returns>
        TForm Create<TForm>() where TForm : Form;
    }
}
```

### 2. FormFactory Implementation

```csharp
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;

namespace YourNamespace.Factories
{
    /// <summary>
    /// Implementation of the form factory pattern.
    /// Creates forms with all dependencies resolved from the DI container.
    /// </summary>
    public class FormFactory : IFormFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider for dependency resolution.</param>
        public FormFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <inheritdoc/>
        public TForm Create<TForm>() where TForm : Form
        {
            return _serviceProvider.GetRequiredService<TForm>();
        }
    }
}
```

**Note:** This is the **ONLY PLACE** where `IServiceProvider` should be injected!

### 3. DI Registration

```csharp
// Program.cs
private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Database Context
    services.AddDbContext<AppDbContext>(options =>
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        options.UseSqlite(connectionString);
    });

    // Unit of Work (Scoped)
    services.AddScoped<IUnitOfWork, UnitOfWork>();

    // Services (Scoped)
    services.AddScoped<ICustomerService, CustomerService>();
    services.AddScoped<IOrderService, OrderService>();

    // Factory Pattern (Singleton - one instance for entire app)
    services.AddSingleton<IFormFactory, FormFactory>();

    // Forms (Transient - new instance each time)
    services.AddTransient<MainForm>();
    services.AddTransient<CustomerForm>();
    services.AddTransient<OrderForm>();
}
```

**Important:**
- ✅ Register `IFormFactory` as **Singleton** (stateless, one instance enough)
- ✅ Register Forms as **Transient** (new instance each time)
- ❌ **DO NOT** inject `IServiceProvider` into forms anymore!

### 4. Using the Factory in Forms

```csharp
public class MainForm : Form
{
    private readonly IFormFactory _formFactory;
    private readonly ILogger<MainForm> _logger;

    public MainForm(IFormFactory formFactory, ILogger<MainForm> logger)
    {
        _formFactory = formFactory ?? throw new ArgumentNullException(nameof(formFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        InitializeComponent();
    }

    private void btnCustomers_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Opening customer form");

            // Create form using factory
            var form = _formFactory.Create<CustomerForm>();
            form.ShowDialog(this);

            _logger.LogInformation("Customer form closed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening customer form");
            MessageBox.Show($"Failed to open customer form: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnOrders_Click(object sender, EventArgs e)
    {
        // Non-modal form
        var form = _formFactory.Create<OrderForm>();
        form.Show();
    }
}
```

---

## Advanced Usage

### With Using Statement (Modal Dialogs)

```csharp
private void btnOpenDialog_Click(object sender, EventArgs e)
{
    using (var dialog = _formFactory.Create<CustomerEditForm>())
    {
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            // Refresh list or handle result
            LoadCustomers();
        }
    }
}
```

### Passing Data to Forms

**Option 1: Constructor Injection**

```csharp
public class CustomerEditForm : Form
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomerEditForm> _logger;
    private readonly int _customerId;

    // Constructor with both DI dependencies and parameters
    public CustomerEditForm(
        ICustomerService customerService,
        ILogger<CustomerEditForm> logger,
        int customerId)
    {
        _customerService = customerService;
        _logger = logger;
        _customerId = customerId;

        InitializeComponent();
    }
}

// ❌ Problem: Can't use generic factory for parameterized constructors
// Need to create a specific factory method
```

**Option 2: Initialize Method (Recommended)**

```csharp
public class CustomerEditForm : Form
{
    private readonly ICustomerService _customerService;
    private int? _customerId;

    public CustomerEditForm(ICustomerService customerService)
    {
        _customerService = customerService;
        InitializeComponent();
    }

    /// <summary>
    /// Initializes the form with a customer ID for editing.
    /// </summary>
    public void Initialize(int customerId)
    {
        _customerId = customerId;
        LoadCustomer();
    }

    private async void LoadCustomer()
    {
        if (_customerId.HasValue)
        {
            var customer = await _customerService.GetByIdAsync(_customerId.Value);
            // Populate form fields
        }
    }
}

// ✅ Usage:
var form = _formFactory.Create<CustomerEditForm>();
form.Initialize(customerId);
form.ShowDialog(this);
```

**Option 3: Property Injection**

```csharp
public class CustomerEditForm : Form
{
    public int? CustomerId { get; set; }

    private async void CustomerEditForm_Load(object sender, EventArgs e)
    {
        if (CustomerId.HasValue)
        {
            await LoadCustomerAsync(CustomerId.Value);
        }
    }
}

// Usage:
var form = _formFactory.Create<CustomerEditForm>();
form.CustomerId = customerId;
form.ShowDialog(this);
```

---

## Testing with Factory Pattern

### Unit Testing MainForm

```csharp
[Fact]
public void OpenCustomerForm_ClickButton_CreatesFormUsingFactory()
{
    // Arrange
    var mockFactory = new Mock<IFormFactory>();
    var mockLogger = new Mock<ILogger<MainForm>>();
    var mockCustomerForm = new Mock<CustomerForm>();

    mockFactory
        .Setup(f => f.Create<CustomerForm>())
        .Returns(mockCustomerForm.Object);

    var mainForm = new MainForm(mockFactory.Object, mockLogger.Object);

    // Act
    // Simulate button click (would need to expose method or use reflection)
    mainForm.OnCustomersClick(null, EventArgs.Empty);

    // Assert
    mockFactory.Verify(f => f.Create<CustomerForm>(), Times.Once);
}
```

### Integration Testing with Real Factory

```csharp
[Fact]
public void FormFactory_CreateCustomerForm_ReturnsValidInstance()
{
    // Arrange
    var services = new ServiceCollection();
    services.AddScoped<ICustomerService, CustomerService>();
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    services.AddSingleton<IFormFactory, FormFactory>();
    services.AddTransient<CustomerForm>();

    var serviceProvider = services.BuildServiceProvider();
    var factory = serviceProvider.GetRequiredService<IFormFactory>();

    // Act
    var form = factory.Create<CustomerForm>();

    // Assert
    Assert.NotNull(form);
    Assert.IsType<CustomerForm>(form);

    // Cleanup
    form.Dispose();
}
```

---

## Comparison with Service Locator

| Aspect | Service Locator ❌ | Factory Pattern ✅ |
|--------|-------------------|-------------------|
| **Dependencies** | Hidden | Explicit |
| **Testability** | Hard to mock | Easy to mock |
| **Compile-time safety** | No | Yes |
| **Pattern type** | Anti-pattern | Design pattern |
| **Coupling** | Tight to DI | Loose coupling |
| **Discovery** | Runtime errors | Startup errors |
| **Best practice** | Avoid | Recommended |

---

## Best Practices

### ✅ DO:

1. **Inject `IFormFactory` into forms** that create other forms
2. **Register factory as Singleton** (stateless, one instance enough)
3. **Register forms as Transient** (new instance each time)
4. **Use `Create<TForm>()` method** to create forms
5. **Dispose forms properly** after use (especially modal dialogs)
6. **Keep IServiceProvider** only in FormFactory implementation
7. **Use Initialize() method** for passing data to forms

### ❌ DON'T:

1. **Don't inject `IServiceProvider` into forms** (use IFormFactory instead)
2. **Don't mix Factory and Service Locator** in the same codebase
3. **Don't create forms manually with `new`** (breaks DI)
4. **Don't register forms as Singleton** (causes issues with disposal)
5. **Don't pass complex data via constructor** (use Initialize() instead)
6. **Don't forget to register forms** in DI container
7. **Don't use factory for non-form objects** (use direct injection)

---

## Common Patterns

### Pattern 1: Modal Dialog

```csharp
private void OnEditCustomer_Click(object sender, EventArgs e)
{
    using (var editForm = _formFactory.Create<CustomerEditForm>())
    {
        editForm.Initialize(selectedCustomerId);

        if (editForm.ShowDialog(this) == DialogResult.OK)
        {
            // Refresh data after edit
            await LoadCustomersAsync();
        }
    }
}
```

### Pattern 2: Non-Modal Form

```csharp
private void OnOpenDashboard_Click(object sender, EventArgs e)
{
    // Check if already open
    if (_dashboardForm == null || _dashboardForm.IsDisposed)
    {
        _dashboardForm = _formFactory.Create<DashboardForm>();
        _dashboardForm.FormClosed += (s, e) => _dashboardForm = null;
        _dashboardForm.Show();
    }
    else
    {
        _dashboardForm.BringToFront();
    }
}
```

### Pattern 3: MDI Child Form

```csharp
private void OnNewDocument_Click(object sender, EventArgs e)
{
    var childForm = _formFactory.Create<DocumentForm>();
    childForm.MdiParent = this;
    childForm.Initialize(documentId);
    childForm.Show();
}
```

---

## Migration Guide

### Step 1: Create Factory Interface and Implementation

```csharp
// 1. Create IFormFactory.cs
// 2. Create FormFactory.cs
```

### Step 2: Register Factory in DI

```csharp
// Program.cs - ConfigureServices
services.AddSingleton<IFormFactory, FormFactory>();
```

### Step 3: Update Forms

**Before:**
```csharp
public MainForm(IServiceProvider serviceProvider)
{
    _serviceProvider = serviceProvider;
}

var form = _serviceProvider.GetRequiredService<CustomerForm>();
```

**After:**
```csharp
public MainForm(IFormFactory formFactory)
{
    _formFactory = formFactory;
}

var form = _formFactory.Create<CustomerForm>();
```

### Step 4: Remove IServiceProvider References

Search for `IServiceProvider` in forms and replace with `IFormFactory`.

---

## Combining with Unit of Work Pattern

Factory Pattern and Unit of Work work at different layers:

```csharp
┌─────────────────────────────────────┐
│   Presentation (Forms)              │
│   Uses: IFormFactory ✅              │ ← Factory creates forms
└─────────────────┬───────────────────┘
                  │
                  v
┌─────────────────────────────────────┐
│   Business Layer (Services)         │
│   Uses: IUnitOfWork ✅               │ ← UnitOfWork manages data
└─────────────────┬───────────────────┘
                  │
                  v
┌─────────────────────────────────────┐
│   Data Access (Repositories)        │
└─────────────────────────────────────┘
```

**Example:**

```csharp
// MainForm uses Factory
public class MainForm : Form
{
    private readonly IFormFactory _formFactory; // ✅ Form creation

    private void OnCustomers_Click(object sender, EventArgs e)
    {
        var form = _formFactory.Create<CustomerForm>();
        form.ShowDialog(this);
    }
}

// CustomerService uses Unit of Work
public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork; // ✅ Data management

    public async Task CreateAsync(Customer customer)
    {
        await _unitOfWork.Customers.AddAsync(customer);
        await _unitOfWork.SaveChangesAsync();
    }
}
```

**Perfect separation of concerns!** ✅

---

## See Also

- [Dependency Injection](dependency-injection.md)
- [Unit of Work Pattern](../data-access/unit-of-work-pattern.md)
- [MVP Pattern](mvp-pattern.md)
- [Project Structure](project-structure.md)
