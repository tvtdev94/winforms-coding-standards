# WinForms Architecture Guide

> **Purpose**: Comprehensive guide to architecture patterns for WinForms applications
> **Audience**: AI assistants and developers building maintainable WinForms apps

---

## 📋 Table of Contents

1. [Project Structure](#project-structure)
2. [MVP Pattern](#mvp-pattern)
3. [MVVM Pattern](#mvvm-pattern)
4. [Dependency Injection](#dependency-injection)
5. [Factory Pattern](#factory-pattern)
6. [Unit of Work Pattern](#unit-of-work-pattern)
7. [Complete Architecture Flow](#complete-architecture-flow)

---

## Project Structure

### Standard WinForms Project Layout

```
/ProjectName
├── /Forms              # UI Layer (minimal logic)
│   ├── MainForm.cs
│   ├── CustomerForm.cs
│   └── ICustomerView.cs
├── /Presenters         # MVP Presenters (if using MVP)
│   └── CustomerPresenter.cs
├── /ViewModels         # MVVM ViewModels (if using MVVM)
│   └── CustomerViewModel.cs
├── /Controls           # Custom user controls
│   └── CustomerSearchControl.cs
├── /Models             # Business/data models
│   ├── Customer.cs
│   └── Order.cs
├── /Services           # Business logic
│   ├── ICustomerService.cs
│   └── CustomerService.cs
├── /Repositories       # Data access layer
│   ├── ICustomerRepository.cs
│   └── CustomerRepository.cs
├── /Data               # EF Core DbContext
│   ├── AppDbContext.cs
│   └── IUnitOfWork.cs
├── /Factories          # Factory Pattern implementations
│   ├── IFormFactory.cs
│   └── FormFactory.cs
├── /Utils              # Helpers, extensions
│   └── StringExtensions.cs
├── /Resources          # Icons, strings, localization
│   └── Strings.resx
├── Program.cs
└── App.config
```

### Key Principles

1. **Forms** contain **UI handling only**, no business logic
2. **Business logic** lives in **Services**
3. **Data access** goes through **Repositories**
4. **Unit of Work** coordinates repositories and transactions
5. **Factory Pattern** creates forms (replaces Service Locator)
6. **Dependency Injection** for loose coupling

---

## MVP Pattern

### Overview

**MVP (Model-View-Presenter)** is the **recommended pattern** for WinForms apps.

**Benefits**:
- ✅ Clear separation of concerns
- ✅ Testable UI logic (test Presenters without Forms)
- ✅ Works well with WinForms event-driven model

### Architecture

```
┌─────────┐      ┌───────────┐      ┌─────────┐
│  View   │◄────►│ Presenter │◄────►│ Service │
│ (Form)  │      │           │      │         │
└─────────┘      └───────────┘      └─────────┘
                       ▲
                       │
                       ▼
                  ┌─────────┐
                  │  Model  │
                  └─────────┘
```

### Implementation

**1. Define View Interface**

```csharp
public interface ICustomerView
{
    // Properties for data binding
    string CustomerName { get; set; }
    string Email { get; set; }
    bool IsActive { get; set; }

    // Events
    event EventHandler LoadRequested;
    event EventHandler SaveRequested;

    // Methods
    void ShowError(string message);
    void ShowSuccess(string message);
    void Close();
}
```

**2. Implement View (Form)**

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

    // ICustomerView properties
    public string CustomerName
    {
        get => txtCustomerName.Text;
        set => txtCustomerName.Text = value;
    }

    // Events
    public event EventHandler LoadRequested;
    public event EventHandler SaveRequested;

    private void btnLoad_Click(object sender, EventArgs e)
    {
        LoadRequested?.Invoke(this, EventArgs.Empty);
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        SaveRequested?.Invoke(this, EventArgs.Empty);
    }
}
```

**3. Implement Presenter**

```csharp
public class CustomerPresenter
{
    private readonly ICustomerService _customerService;
    private ICustomerView _view;

    public CustomerPresenter(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public void SetView(ICustomerView view)
    {
        _view = view;
        _view.LoadRequested += OnLoadRequested;
        _view.SaveRequested += OnSaveRequested;
    }

    private async void OnLoadRequested(object sender, EventArgs e)
    {
        try
        {
            var customer = await _customerService.GetByIdAsync(1);
            _view.CustomerName = customer.Name;
            _view.Email = customer.Email;
        }
        catch (Exception ex)
        {
            _view.ShowError($"Failed to load: {ex.Message}");
        }
    }
}
```

📖 **Full documentation**: [docs/architecture/mvp-pattern.md](../../docs/architecture/mvp-pattern.md)

---

## MVVM Pattern

### Overview

**MVVM (Model-View-ViewModel)** is an alternative pattern, best for **.NET 8+** with modern binding.

**When to use**:
- ✅ .NET 8+ projects
- ✅ Need for complex data binding
- ✅ Team familiar with MVVM from WPF/Xamarin

**When NOT to use**:
- ❌ .NET Framework 4.8 (limited binding support)
- ❌ Simple CRUD forms (MVP is easier)

### Architecture

```
┌─────────┐      ┌────────────┐      ┌─────────┐
│  View   │◄────►│ ViewModel  │◄────►│ Service │
│ (Form)  │      │            │      │         │
└─────────┘      └────────────┘      └─────────┘
                       ▲
                       │
                       ▼
                  ┌─────────┐
                  │  Model  │
                  └─────────┘
```

📖 **Full documentation**: [docs/architecture/mvvm-pattern.md](../../docs/architecture/mvvm-pattern.md)

---

## Dependency Injection

### Overview

**Dependency Injection (DI)** is **mandatory** for all WinForms projects following these standards.

**Benefits**:
- ✅ Loose coupling
- ✅ Testability
- ✅ Flexibility
- ✅ Lifetime management

### Setup with Microsoft.Extensions.DependencyInjection

**Program.cs**

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var host = CreateHostBuilder().Build();
        ServiceProvider = host.Services;

        var mainForm = ServiceProvider.GetRequiredService<MainForm>();
        Application.Run(mainForm);
    }

    static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Register DbContext
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(connectionString));

                // Register Unit of Work
                services.AddScoped<IUnitOfWork, UnitOfWork>();

                // Register Repositories
                services.AddScoped<ICustomerRepository, CustomerRepository>();

                // Register Services
                services.AddScoped<ICustomerService, CustomerService>();

                // Register Factories
                services.AddSingleton<IFormFactory, FormFactory>();

                // Register Forms
                services.AddTransient<MainForm>();
                services.AddTransient<CustomerForm>();

                // Register Presenters
                services.AddTransient<CustomerPresenter>();
            });
    }

    public static IServiceProvider ServiceProvider { get; private set; }
}
```

### Service Lifetimes

| Lifetime | Description | Use For |
|----------|-------------|---------|
| **Transient** | New instance every time | Forms, Presenters, lightweight services |
| **Scoped** | One instance per scope | DbContext, Unit of Work, Repositories |
| **Singleton** | One instance for app lifetime | Factories, Logging, Configuration |

📖 **Full documentation**: [docs/architecture/dependency-injection.md](../../docs/architecture/dependency-injection.md)

---

## Factory Pattern

### Overview

**Factory Pattern** is used to create forms with proper dependency injection.

**⚠️ CRITICAL**: Replaces the **Service Locator anti-pattern**!

### Why Factory Pattern?

```csharp
// ❌ WRONG: Service Locator anti-pattern
public class MainForm : Form
{
    private readonly IServiceProvider _serviceProvider;

    public MainForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private void btnOpenCustomer_Click(object sender, EventArgs e)
    {
        // Anti-pattern! Form knows about DI container
        var customerForm = _serviceProvider.GetRequiredService<CustomerForm>();
        customerForm.Show();
    }
}

// ✅ CORRECT: Factory Pattern
public class MainForm : Form
{
    private readonly IFormFactory _formFactory;

    public MainForm(IFormFactory formFactory)
    {
        _formFactory = formFactory;
    }

    private void btnOpenCustomer_Click(object sender, EventArgs e)
    {
        var customerForm = _formFactory.Create<CustomerForm>();
        customerForm.Show();
    }
}
```

### Implementation

**1. Define Factory Interface**

```csharp
public interface IFormFactory
{
    TForm Create<TForm>() where TForm : Form;
}
```

**2. Implement Factory**

```csharp
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
```

**3. Register in DI**

```csharp
services.AddSingleton<IFormFactory, FormFactory>();
```

📖 **Full documentation**: [docs/architecture/factory-pattern.md](../../docs/architecture/factory-pattern.md)

---

## Unit of Work Pattern

### Overview

**Unit of Work** manages repositories and database transactions as a single unit.

**Benefits**:
- ✅ Single `SaveChangesAsync()` call
- ✅ Transaction coordination
- ✅ Centralized repository access
- ✅ Reduced coupling

### Architecture

```
┌─────────────┐
│   Service   │
└──────┬──────┘
       │ Inject IUnitOfWork
       ▼
┌─────────────────────┐
│   Unit of Work      │
│  ┌───────────────┐  │
│  │ Customers     │  │ ◄─── Repository properties
│  │ Orders        │  │
│  │ Products      │  │
│  └───────────────┘  │
│  SaveChangesAsync() │ ◄─── Single save method
└─────────────────────┘
```

### Implementation

**1. Define Interface**

```csharp
public interface IUnitOfWork : IDisposable
{
    // Repository properties
    ICustomerRepository Customers { get; }
    IOrderRepository Orders { get; }

    // Save and transaction methods
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
```

**2. Implement Unit of Work**

```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private ICustomerRepository _customers;
    private IOrderRepository _orders;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
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

    public void Dispose()
    {
        _context.Dispose();
    }
}
```

**3. Use in Services**

```csharp
public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AddCustomerAsync(Customer customer)
    {
        await _unitOfWork.Customers.AddAsync(customer);
        await _unitOfWork.SaveChangesAsync(); // ✅ Single save call
    }
}
```

### Critical Rules

1. ✅ **Inject IUnitOfWork** into services, NOT `IRepository`
2. ✅ **Call SaveChangesAsync** in services, NOT repositories
3. ✅ **Never call SaveChangesAsync** in repositories
4. ✅ **Register as Scoped** in DI container

📖 **Full documentation**: [docs/data-access/unit-of-work-pattern.md](../../docs/data-access/unit-of-work-pattern.md)

---

## Complete Architecture Flow

### Data Flow

```
┌──────────────────────────────────────────────────────────────────┐
│                         Complete Flow                            │
└──────────────────────────────────────────────────────────────────┘

1. User clicks button
   ▼
2. Form raises event (ICustomerView.SaveRequested)
   ▼
3. Presenter handles event
   ▼
4. Presenter calls Service (ICustomerService.AddCustomerAsync)
   ▼
5. Service validates and calls Repository via Unit of Work
   ▼
6. Unit of Work.Customers.AddAsync(customer)
   ▼
7. Unit of Work.SaveChangesAsync()
   ▼
8. Database updated
   ▼
9. Presenter updates View
   ▼
10. User sees success message
```

### Example: Complete CRUD Operation

**Form (View)**:
```csharp
public class CustomerForm : Form, ICustomerView
{
    private readonly CustomerPresenter _presenter;

    public CustomerForm(CustomerPresenter presenter)
    {
        _presenter = presenter;
        _presenter.SetView(this);
    }

    public event EventHandler SaveRequested;

    private void btnSave_Click(object sender, EventArgs e)
    {
        SaveRequested?.Invoke(this, EventArgs.Empty);
    }
}
```

**Presenter**:
```csharp
public class CustomerPresenter
{
    private readonly ICustomerService _service;
    private ICustomerView _view;

    public CustomerPresenter(ICustomerService service)
    {
        _service = service;
    }

    private async void OnSaveRequested(object sender, EventArgs e)
    {
        var customer = new Customer { Name = _view.CustomerName };
        await _service.AddCustomerAsync(customer);
        _view.ShowSuccess("Saved!");
    }
}
```

**Service**:
```csharp
public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AddCustomerAsync(Customer customer)
    {
        ValidateCustomer(customer);
        await _unitOfWork.Customers.AddAsync(customer);
        await _unitOfWork.SaveChangesAsync();
    }
}
```

**Unit of Work + Repository**:
```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public ICustomerRepository Customers { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public async Task AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        // ❌ NO SaveChangesAsync here!
    }
}
```

---

## Summary

**Key Architecture Patterns**:

1. **MVP Pattern** - Recommended for all WinForms projects
2. **Factory Pattern** - Create forms with DI (NO Service Locator!)
3. **Unit of Work** - Coordinate repositories and transactions
4. **Dependency Injection** - Mandatory for loose coupling
5. **Repository Pattern** - Abstract data access

**Flow**:
```
Form → Presenter → Service → Unit of Work → Repository → Database
```

**Critical Rules**:
- ✅ Use IFormFactory, NOT IServiceProvider in forms
- ✅ Inject IUnitOfWork, NOT IRepository in services
- ✅ Call SaveChangesAsync in Unit of Work, NOT repositories
- ✅ Register DbContext and Unit of Work as Scoped
- ✅ Register Forms and Presenters as Transient
- ✅ Register Factories as Singleton

---

**See also**:
- [MVP Pattern](../../docs/architecture/mvp-pattern.md) - Full MVP documentation
- [Factory Pattern](../../docs/architecture/factory-pattern.md) - Detailed factory guide
- [Unit of Work](../../docs/data-access/unit-of-work-pattern.md) - Complete UoW pattern
- [Dependency Injection](../../docs/architecture/dependency-injection.md) - Full DI setup
