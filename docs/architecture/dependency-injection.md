# Dependency Injection in WinForms

> **Quick Reference**: Setup and use Microsoft.Extensions.DependencyInjection in WinForms applications for loose coupling and testability.

---

## 📖 What is Dependency Injection?

**Dependency Injection (DI)** is a design pattern where:
- Objects receive their dependencies from external sources
- Rather than creating dependencies themselves
- Promotes loose coupling and testability

### Benefits:
✅ **Testability** - Easy to mock dependencies
✅ **Loose Coupling** - Components don't know about concrete implementations
✅ **Maintainability** - Change implementations without changing consumers
✅ **Lifetime Management** - Container manages object lifecycles

---

## 🛠️ Setup DI in WinForms

### Step 1: Install NuGet Package

```bash
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.Extensions.Logging
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Json
```

### Step 2: Configure DI Container in Program.cs

```csharp
// Program.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        ApplicationConfiguration.Initialize();

        // Build DI container
        var services = new ServiceCollection();
        ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();

        // Resolve and run main form
        var mainForm = serviceProvider.GetRequiredService<MainForm>();
        Application.Run(mainForm);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Logging
        services.AddLogging(configure =>
        {
            configure.AddConsole();
            configure.AddDebug();
            configure.SetMinimumLevel(LogLevel.Information);
        });

        // Configuration
        services.AddSingleton<IConfiguration>(provider =>
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
        });

        // Services (Business Logic)
        services.AddSingleton<ICustomerService, CustomerService>();
        services.AddSingleton<IOrderService, OrderService>();
        services.AddTransient<IEmailService, EmailService>();

        // Repositories (Data Access)
        services.AddSingleton<ICustomerRepository, CustomerRepository>();
        services.AddSingleton<IOrderRepository, OrderRepository>();

        // DbContext (if using EF Core)
        services.AddDbContext<AppDbContext>(options =>
        {
            var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        });

        // Forms
        services.AddTransient<MainForm>();
        services.AddTransient<CustomerForm>();
        services.AddTransient<OrderForm>();

        // Presenters (if using MVP)
        services.AddTransient<CustomerPresenter>();
        services.AddTransient<OrderPresenter>();

        // Form Factory
        services.AddSingleton<IFormFactory, FormFactory>();
    }
}
```

---

## 🔄 Service Lifetimes

### Singleton
- **One instance** for entire application lifetime
- Created on first request
- **Use for**: Stateless services, repositories, caching

```csharp
services.AddSingleton<ICustomerService, CustomerService>();
```

### Transient
- **New instance** every time it's requested
- **Use for**: Lightweight, stateless services, forms

```csharp
services.AddTransient<CustomerForm>();
services.AddTransient<IEmailService, EmailService>();
```

### Scoped
- **One instance per scope** (request in web apps)
- Not commonly used in WinForms
- **Use for**: DbContext in specific scenarios

```csharp
services.AddScoped<AppDbContext>();
```

### Lifetime Comparison:

```csharp
// Example lifecycle
services.AddSingleton<SingletonService>();    // Created once
services.AddScoped<ScopedService>();          // Created per scope
services.AddTransient<TransientService>();    // Created each time

// When you request:
var svc1 = provider.GetService<SingletonService>();  // Instance A
var svc2 = provider.GetService<SingletonService>();  // Instance A (same)

var svc3 = provider.GetService<TransientService>(); // Instance B
var svc4 = provider.GetService<TransientService>(); // Instance C (different)
```

---

## 💉 Constructor Injection

### Forms with DI

```csharp
// CustomerForm.cs
public partial class CustomerForm : Form
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomerForm> _logger;

    // Constructor receives dependencies
    public CustomerForm(
        ICustomerService customerService,
        ILogger<CustomerForm> logger)
    {
        _customerService = customerService;
        _logger = logger;
        InitializeComponent();
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            var customer = GetCustomerFromForm();
            await _customerService.SaveAsync(customer);
            _logger.LogInformation("Customer saved: {Name}", customer.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving customer");
        }
    }
}
```

### Services with DI

```csharp
// CustomerService.cs
public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<CustomerService> _logger;
    private readonly IEmailService _emailService;

    public CustomerService(
        ICustomerRepository repository,
        ILogger<CustomerService> logger,
        IEmailService emailService)
    {
        _repository = repository;
        _logger = logger;
        _emailService = emailService;
    }

    public async Task<bool> SaveAsync(Customer customer)
    {
        var result = await _repository.SaveAsync(customer);

        if (result)
        {
            _logger.LogInformation("Customer saved: {Id}", customer.Id);
            await _emailService.SendWelcomeEmailAsync(customer.Email);
        }

        return result;
    }
}
```

---

## 🏭 Form Factory Pattern

For forms with runtime parameters (e.g., customerId), use Factory pattern:

```csharp
// IFormFactory.cs
public interface IFormFactory
{
    CustomerForm CreateCustomerForm();
    CustomerForm CreateCustomerForm(int customerId);
    OrderForm CreateOrderForm(int orderId);
}

// FormFactory.cs
public class FormFactory : IFormFactory
{
    private readonly IServiceProvider _serviceProvider;

    public FormFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public CustomerForm CreateCustomerForm()
    {
        // Resolve from DI container
        return _serviceProvider.GetRequiredService<CustomerForm>();
    }

    public CustomerForm CreateCustomerForm(int customerId)
    {
        // Resolve services, pass parameters manually
        var service = _serviceProvider.GetRequiredService<ICustomerService>();
        var logger = _serviceProvider.GetRequiredService<ILogger<CustomerForm>>();

        return new CustomerForm(service, logger, customerId);
    }

    public OrderForm CreateOrderForm(int orderId)
    {
        var service = _serviceProvider.GetRequiredService<IOrderService>();
        return new OrderForm(service, orderId);
    }
}

// Usage in MainForm
public partial class MainForm : Form
{
    private readonly IFormFactory _formFactory;

    public MainForm(IFormFactory formFactory)
    {
        _formFactory = formFactory;
        InitializeComponent();
    }

    private void btnAddCustomer_Click(object sender, EventArgs e)
    {
        var form = _formFactory.CreateCustomerForm(); // No parameters
        form.ShowDialog();
    }

    private void btnEditCustomer_Click(object sender, EventArgs e)
    {
        int customerId = GetSelectedCustomerId();
        var form = _formFactory.CreateCustomerForm(customerId); // With parameter
        form.ShowDialog();
    }
}
```

---

## ✅ Best Practices

### DO:
✅ Use constructor injection (not property injection)
✅ Depend on interfaces, not concrete types
✅ Register all forms in DI container
✅ Use appropriate lifetime (Singleton, Transient, Scoped)
✅ Use IServiceProvider only in factories
✅ Validate services are registered at startup

### DON'T:
❌ Don't use Service Locator pattern (anti-pattern)
❌ Don't inject IServiceProvider into every class
❌ Don't create your own singletons (use DI)
❌ Don't new up dependencies manually
❌ Don't use static classes for state

---

## 🧪 Testing with DI

DI makes testing easy - just inject mocks:

```csharp
[Fact]
public async Task SaveCustomer_ValidData_CallsRepository()
{
    // Arrange
    var mockRepository = new Mock<ICustomerRepository>();
    var mockLogger = new Mock<ILogger<CustomerService>>();
    var mockEmail = new Mock<IEmailService>();

    mockRepository
        .Setup(r => r.SaveAsync(It.IsAny<Customer>()))
        .ReturnsAsync(true);

    var service = new CustomerService(
        mockRepository.Object,
        mockLogger.Object,
        mockEmail.Object);

    var customer = new Customer { Name = "Test", Email = "test@example.com" };

    // Act
    var result = await service.SaveAsync(customer);

    // Assert
    Assert.True(result);
    mockRepository.Verify(r => r.SaveAsync(customer), Times.Once);
    mockEmail.Verify(e => e.SendWelcomeEmailAsync(customer.Email), Times.Once);
}
```

---

## 🔗 Related Topics

- [MVP Pattern](mvp-pattern.md) - MVP with DI
- [Project Structure](project-structure.md) - Organizing services
- [Unit Testing](../testing/unit-testing.md) - Testing with mocks

---

**Last Updated**: 2025-11-07
