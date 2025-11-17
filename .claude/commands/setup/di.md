---
description: Setup Dependency Injection for WinForms application
---

You are tasked with setting up Dependency Injection (DI) for a WinForms application.

## Workflow

1. **Ask the user**:
   - Is this a new project or existing one?
   - Which DI container to use? (Microsoft.Extensions.DependencyInjection recommended)
   - What services need to be registered?

2. **Install required NuGet packages**:
```bash
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.Extensions.Logging
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Json
```

3. **Setup DI Container in Program.cs**:

### Step 1: Configure Services

✅ **Create Program.cs with DI setup**:
```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Windows.Forms;

namespace YourApp
{
    static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Setup DI container
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            // Setup global exception handlers
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Run main form from DI container
            var mainForm = ServiceProvider.GetRequiredService<MainForm>();
            Application.Run(mainForm);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // Logging
            services.AddLogging(configure =>
            {
                configure.AddDebug();
                configure.AddConsole();
                // Or use Serilog, NLog, etc.
            });

            // Database Context
            services.AddDbContext<AppDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);
            });

            // Unit of Work (Scoped - one instance per request/scope)
            // IMPORTANT: Use Unit of Work pattern instead of registering repositories directly
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductService, ProductService>();

            // Presenters
            services.AddTransient<CustomerPresenter>();
            services.AddTransient<OrderPresenter>();
            services.AddTransient<ProductPresenter>();

            // Forms
            services.AddTransient<MainForm>();
            services.AddTransient<CustomerForm>();
            services.AddTransient<OrderForm>();
            services.AddTransient<ProductForm>();

            // Other services
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<IConfigurationService, ConfigurationService>();
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            var logger = ServiceProvider.GetService<ILogger<Program>>();
            logger?.LogError(e.Exception, "Unhandled thread exception");

            MessageBox.Show($"An error occurred:\n\n{e.Exception.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var logger = ServiceProvider.GetService<ILogger<Program>>();
            logger?.LogCritical(e.ExceptionObject as Exception, "Unhandled domain exception");

            MessageBox.Show($"A critical error occurred:\n\n{(e.ExceptionObject as Exception)?.Message}",
                "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
```

### Step 2: Update Form Constructors

✅ **Use constructor injection in forms**:
```csharp
public partial class CustomerForm : Form, ICustomerView
{
    private readonly CustomerPresenter _presenter;
    private readonly ILogger<CustomerForm> _logger;

    // Constructor injection
    public CustomerForm(
        CustomerPresenter presenter,
        ILogger<CustomerForm> logger)
    {
        InitializeComponent();

        _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _logger.LogInformation("CustomerForm initialized");
    }

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

### Step 3: Open Forms from DI Container

✅ **Resolve forms from DI container**:
```csharp
public partial class MainForm : Form
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MainForm> _logger;

    public MainForm(
        IServiceProvider serviceProvider,
        ILogger<MainForm> logger)
    {
        InitializeComponent();

        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    private void btnCustomers_Click(object sender, EventArgs e)
    {
        // Resolve form from DI container
        using (var form = _serviceProvider.GetRequiredService<CustomerForm>())
        {
            form.ShowDialog(this);
        }
    }

    private void btnOrders_Click(object sender, EventArgs e)
    {
        // Create scope for scoped services
        using (var scope = _serviceProvider.CreateScope())
        {
            var form = scope.ServiceProvider.GetRequiredService<OrderForm>();
            form.ShowDialog(this);
        }
    }

    // Or for non-modal forms
    private void btnProducts_Click(object sender, EventArgs e)
    {
        var form = _serviceProvider.GetRequiredService<ProductForm>();
        form.Show();
    }
}
```

### Step 4: Service Registration Patterns

✅ **Different service lifetimes**:
```csharp
private static void ConfigureServices(IServiceCollection services)
{
    // SINGLETON - Created once for entire application lifetime
    // Use for: Configuration, caching, stateless services
    services.AddSingleton<IAuthenticationService, AuthenticationService>();
    services.AddSingleton<ICacheService, MemoryCacheService>();

    // SCOPED - Created once per scope (per form/operation)
    // Use for: Database contexts, Unit of Work, business services
    services.AddScoped<AppDbContext>();
    services.AddScoped<IUnitOfWork, UnitOfWork>(); // Unit of Work manages repositories
    services.AddScoped<ICustomerService, CustomerService>();

    // TRANSIENT - Created every time it's requested
    // Use for: Lightweight services, presenters, forms
    services.AddTransient<CustomerPresenter>();
    services.AddTransient<CustomerForm>();
    services.AddTransient<IEmailService, EmailService>();
}
```

### Step 5: Configuration (appsettings.json)

✅ **Create appsettings.json**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=YourApp;Trusted_Connection=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "AppSettings": {
    "MaxRetries": 3,
    "Timeout": 30,
    "CacheExpiration": 300
  }
}
```

✅ **Read configuration in services**:
```csharp
public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CustomerService> _logger;
    private readonly int _maxRetries;

    public CustomerService(
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        ILogger<CustomerService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Read from configuration
        _maxRetries = _configuration.GetValue<int>("AppSettings:MaxRetries", 3);
    }

    public async Task CreateCustomerAsync(Customer customer)
    {
        // Access repository via Unit of Work
        await _unitOfWork.Customers.AddAsync(customer);
        await _unitOfWork.SaveChangesAsync();
    }
}
```

### Step 6: Factory Pattern for Complex Creation

✅ **Use factory for complex form creation**:
```csharp
public interface IFormFactory
{
    T Create<T>() where T : Form;
}

public class FormFactory : IFormFactory
{
    private readonly IServiceProvider _serviceProvider;

    public FormFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T Create<T>() where T : Form
    {
        return _serviceProvider.GetRequiredService<T>();
    }
}

// Register factory
services.AddSingleton<IFormFactory, FormFactory>();

// Use factory
public partial class MainForm : Form
{
    private readonly IFormFactory _formFactory;

    public MainForm(IFormFactory formFactory)
    {
        InitializeComponent();
        _formFactory = formFactory;
    }

    private void btnCustomers_Click(object sender, EventArgs e)
    {
        using (var form = _formFactory.Create<CustomerForm>())
        {
            form.ShowDialog(this);
        }
    }
}
```

### Step 7: Testing with DI

✅ **Easy to mock dependencies in tests**:
```csharp
[Fact]
public async Task SaveCustomer_ValidData_Succeeds()
{
    // Arrange
    var mockService = new Mock<ICustomerService>();
    var mockLogger = new Mock<ILogger<CustomerPresenter>>();
    var view = new Mock<ICustomerView>();

    view.SetupGet(v => v.CustomerName).Returns("John Doe");
    view.SetupGet(v => v.CustomerEmail).Returns("john@example.com");

    mockService
        .Setup(s => s.CreateCustomerAsync(It.IsAny<Customer>()))
        .ReturnsAsync(true);

    var presenter = new CustomerPresenter(view.Object, mockService.Object, mockLogger.Object);

    // Act
    view.Raise(v => v.SaveRequested += null, EventArgs.Empty);

    // Assert
    mockService.Verify(s => s.CreateCustomerAsync(It.IsAny<Customer>()), Times.Once);
    view.Verify(v => v.ShowSuccess(It.IsAny<string>()), Times.Once);
}
```

4. **Best Practices**:
   - ✅ Register services in Program.cs ConfigureServices
   - ✅ Use constructor injection for all dependencies
   - ✅ Choose appropriate service lifetime (Singleton/Scoped/Transient)
   - ✅ Use IServiceProvider to resolve forms
   - ✅ Store configuration in appsettings.json
   - ✅ Don't use ServiceLocator anti-pattern
   - ✅ Create scopes for long-running operations
   - ✅ Dispose forms properly after use

5. **Common Pitfalls to Avoid**:
   - ❌ Storing IServiceProvider in fields (use IServiceScopeFactory instead)
   - ❌ Resolving scoped services from root provider
   - ❌ Circular dependencies between services
   - ❌ Using Service Locator pattern instead of injection
   - ❌ Not disposing forms created from DI
   - ❌ Mixing DI and manual instantiation

6. **Show the user**:
   - Updated Program.cs with DI setup
   - Updated forms with constructor injection
   - appsettings.json configuration file
   - Example of opening forms from DI
   - Service lifetime explanations
   - Offer to register more services
