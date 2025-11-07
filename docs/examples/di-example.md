# Dependency Injection Example

## 📋 Overview

This document provides a complete, working example of implementing Dependency Injection (DI) in a WinForms application using Microsoft.Extensions.DependencyInjection. The example demonstrates a customer management system with proper separation of concerns, testability, and loose coupling.

**What This Example Covers**:
- Setting up the DI container in Program.cs
- Registering services with different lifetimes
- Constructor injection in forms, services, and repositories
- Configuration management
- Logging integration
- Best practices for WinForms DI patterns

## 🎯 What You'll Learn

By following this example, you will understand:

1. **Container Setup**: How to configure Microsoft.Extensions.DependencyInjection in WinForms
2. **Service Registration**: Proper registration of services, repositories, and forms
3. **Constructor Injection**: Injecting dependencies into forms and services
4. **Service Lifetimes**: When to use Singleton, Scoped, and Transient
5. **Testing Benefits**: How DI makes unit testing easier
6. **Real-World Patterns**: Production-ready code structure

## 📦 Prerequisites

### Target Framework
- **.NET 6.0+** (recommended) or **.NET Framework 4.7.2+**
- **C# 10.0+**

### Required NuGet Packages

```bash
# Core DI package
dotnet add package Microsoft.Extensions.DependencyInjection --version 8.0.0

# Configuration support
dotnet add package Microsoft.Extensions.Configuration --version 8.0.0
dotnet add package Microsoft.Extensions.Configuration.Json --version 8.0.0
dotnet add package Microsoft.Extensions.Configuration.Binder --version 8.0.0

# Logging support
dotnet add package Microsoft.Extensions.Logging --version 8.0.0
dotnet add package Microsoft.Extensions.Logging.Console --version 8.0.0

# Optional: Serilog for better logging
dotnet add package Serilog.Extensions.Logging --version 8.0.0
dotnet add package Serilog.Sinks.File --version 5.0.0
```

## 🏗️ Project Structure

```
/CustomerManagement
    ├── Program.cs                      # DI container setup
    ├── appsettings.json                # Configuration file
    ├── /Forms
    │   └── CustomerForm.cs             # Main form with DI
    ├── /Services
    │   ├── ICustomerService.cs         # Service interface
    │   └── CustomerService.cs          # Service implementation
    ├── /Repositories
    │   ├── ICustomerRepository.cs      # Repository interface
    │   └── CustomerRepository.cs       # Repository implementation
    ├── /Models
    │   └── Customer.cs                 # Domain model
    └── /Configuration
        └── AppSettings.cs              # Strongly-typed config
```

## 📝 Step-by-Step Implementation

### Step 1: Create the Domain Model

**File**: `Models/Customer.cs`

```csharp
namespace CustomerManagement.Models;

/// <summary>
/// Represents a customer entity
/// </summary>
public class Customer
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public bool IsActive { get; set; }

    /// <summary>
    /// Gets the full name of the customer
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";
}
```

### Step 2: Create Configuration Model

**File**: `Configuration/AppSettings.cs`

```csharp
namespace CustomerManagement.Configuration;

/// <summary>
/// Strongly-typed configuration settings
/// </summary>
public class AppSettings
{
    public string ApplicationName { get; set; } = "Customer Management";

    public DatabaseSettings Database { get; set; } = new();

    public LoggingSettings Logging { get; set; } = new();
}

public class DatabaseSettings
{
    public string ConnectionString { get; set; } = string.Empty;

    public int CommandTimeout { get; set; } = 30;
}

public class LoggingSettings
{
    public string LogLevel { get; set; } = "Information";

    public string LogPath { get; set; } = "logs/app.log";
}
```

### Step 3: Create Repository Layer

**File**: `Repositories/ICustomerRepository.cs`

```csharp
using CustomerManagement.Models;

namespace CustomerManagement.Repositories;

/// <summary>
/// Repository interface for customer data access
/// </summary>
public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllAsync();

    Task<Customer?> GetByIdAsync(int id);

    Task<Customer> CreateAsync(Customer customer);

    Task<Customer> UpdateAsync(Customer customer);

    Task<bool> DeleteAsync(int id);

    Task<IEnumerable<Customer>> SearchAsync(string searchTerm);
}
```

**File**: `Repositories/CustomerRepository.cs`

```csharp
using CustomerManagement.Models;
using Microsoft.Extensions.Logging;

namespace CustomerManagement.Repositories;

/// <summary>
/// In-memory implementation of customer repository
/// In production, this would use Entity Framework Core or ADO.NET
/// </summary>
public class CustomerRepository : ICustomerRepository
{
    private readonly ILogger<CustomerRepository> _logger;
    private readonly List<Customer> _customers;
    private int _nextId = 1;

    public CustomerRepository(ILogger<CustomerRepository> logger)
    {
        _logger = logger;
        _customers = new List<Customer>();

        // Seed with sample data
        SeedData();
    }

    public Task<IEnumerable<Customer>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all customers");
        return Task.FromResult<IEnumerable<Customer>>(_customers.ToList());
    }

    public Task<Customer?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving customer with ID: {CustomerId}", id);
        var customer = _customers.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(customer);
    }

    public Task<Customer> CreateAsync(Customer customer)
    {
        _logger.LogInformation("Creating new customer: {CustomerName}", customer.FullName);

        customer.Id = _nextId++;
        customer.CreatedDate = DateTime.Now;
        _customers.Add(customer);

        return Task.FromResult(customer);
    }

    public Task<Customer> UpdateAsync(Customer customer)
    {
        _logger.LogInformation("Updating customer: {CustomerId}", customer.Id);

        var existing = _customers.FirstOrDefault(c => c.Id == customer.Id);
        if (existing != null)
        {
            existing.FirstName = customer.FirstName;
            existing.LastName = customer.LastName;
            existing.Email = customer.Email;
            existing.Phone = customer.Phone;
            existing.IsActive = customer.IsActive;
        }

        return Task.FromResult(customer);
    }

    public Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting customer: {CustomerId}", id);

        var customer = _customers.FirstOrDefault(c => c.Id == id);
        if (customer != null)
        {
            _customers.Remove(customer);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public Task<IEnumerable<Customer>> SearchAsync(string searchTerm)
    {
        _logger.LogInformation("Searching customers with term: {SearchTerm}", searchTerm);

        var results = _customers.Where(c =>
            c.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            c.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            c.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
        ).ToList();

        return Task.FromResult<IEnumerable<Customer>>(results);
    }

    private void SeedData()
    {
        _customers.AddRange(new[]
        {
            new Customer
            {
                Id = _nextId++,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "555-0100",
                CreatedDate = DateTime.Now.AddDays(-30),
                IsActive = true
            },
            new Customer
            {
                Id = _nextId++,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Phone = "555-0101",
                CreatedDate = DateTime.Now.AddDays(-15),
                IsActive = true
            }
        });
    }
}
```

### Step 4: Create Service Layer

**File**: `Services/ICustomerService.cs`

```csharp
using CustomerManagement.Models;

namespace CustomerManagement.Services;

/// <summary>
/// Service interface for customer business logic
/// </summary>
public interface ICustomerService
{
    Task<IEnumerable<Customer>> GetAllCustomersAsync();

    Task<Customer?> GetCustomerByIdAsync(int id);

    Task<Customer> CreateCustomerAsync(Customer customer);

    Task<Customer> UpdateCustomerAsync(Customer customer);

    Task<bool> DeleteCustomerAsync(int id);

    Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm);

    Task<bool> ValidateCustomerAsync(Customer customer);
}
```

**File**: `Services/CustomerService.cs`

```csharp
using CustomerManagement.Models;
using CustomerManagement.Repositories;
using Microsoft.Extensions.Logging;

namespace CustomerManagement.Services;

/// <summary>
/// Service implementation containing customer business logic
/// </summary>
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

    public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving all customers from service layer");
            return await _repository.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers");
            throw;
        }
    }

    public async Task<Customer?> GetCustomerByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Retrieving customer {CustomerId}", id);
            return await _repository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer {CustomerId}", id);
            throw;
        }
    }

    public async Task<Customer> CreateCustomerAsync(Customer customer)
    {
        try
        {
            _logger.LogInformation("Creating customer: {CustomerName}", customer.FullName);

            // Business validation
            if (!await ValidateCustomerAsync(customer))
            {
                throw new InvalidOperationException("Customer validation failed");
            }

            return await _repository.CreateAsync(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            throw;
        }
    }

    public async Task<Customer> UpdateCustomerAsync(Customer customer)
    {
        try
        {
            _logger.LogInformation("Updating customer {CustomerId}", customer.Id);

            // Business validation
            if (!await ValidateCustomerAsync(customer))
            {
                throw new InvalidOperationException("Customer validation failed");
            }

            return await _repository.UpdateAsync(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer {CustomerId}", customer.Id);
            throw;
        }
    }

    public async Task<bool> DeleteCustomerAsync(int id)
    {
        try
        {
            _logger.LogInformation("Deleting customer {CustomerId}", id);
            return await _repository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer {CustomerId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm)
    {
        try
        {
            _logger.LogInformation("Searching customers with term: {SearchTerm}", searchTerm);

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllCustomersAsync();
            }

            return await _repository.SearchAsync(searchTerm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching customers");
            throw;
        }
    }

    public Task<bool> ValidateCustomerAsync(Customer customer)
    {
        // Business validation logic
        if (string.IsNullOrWhiteSpace(customer.FirstName))
        {
            _logger.LogWarning("Customer validation failed: FirstName is required");
            return Task.FromResult(false);
        }

        if (string.IsNullOrWhiteSpace(customer.LastName))
        {
            _logger.LogWarning("Customer validation failed: LastName is required");
            return Task.FromResult(false);
        }

        if (string.IsNullOrWhiteSpace(customer.Email))
        {
            _logger.LogWarning("Customer validation failed: Email is required");
            return Task.FromResult(false);
        }

        // Simple email validation
        if (!customer.Email.Contains("@"))
        {
            _logger.LogWarning("Customer validation failed: Invalid email format");
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }
}
```

### Step 5: Configure DI Container

**File**: `Program.cs`

```csharp
using CustomerManagement.Configuration;
using CustomerManagement.Forms;
using CustomerManagement.Repositories;
using CustomerManagement.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CustomerManagement;

static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Build configuration
        var configuration = BuildConfiguration();

        // Setup DI container
        var services = new ServiceCollection();
        ConfigureServices(services, configuration);

        // Build service provider
        using var serviceProvider = services.BuildServiceProvider();

        // Resolve and run the main form
        var mainForm = serviceProvider.GetRequiredService<CustomerForm>();
        Application.Run(mainForm);
    }

    /// <summary>
    /// Builds configuration from appsettings.json
    /// </summary>
    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Production"}.json", optional: true)
            .Build();
    }

    /// <summary>
    /// Configures all services for dependency injection
    /// </summary>
    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Configuration
        services.AddSingleton(configuration);
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

        // Logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Repositories (Scoped for database contexts, Singleton for in-memory)
        services.AddSingleton<ICustomerRepository, CustomerRepository>();

        // Services (Transient - created each time they're requested)
        services.AddTransient<ICustomerService, CustomerService>();

        // Forms (Transient - new instance each time)
        services.AddTransient<CustomerForm>();
    }
}
```

### Step 6: Create Form with Constructor Injection

**File**: `Forms/CustomerForm.cs`

```csharp
using CustomerManagement.Models;
using CustomerManagement.Services;
using Microsoft.Extensions.Logging;

namespace CustomerManagement.Forms;

/// <summary>
/// Main customer management form with dependency injection
/// </summary>
public partial class CustomerForm : Form
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomerForm> _logger;

    // UI Controls
    private DataGridView dgvCustomers;
    private TextBox txtFirstName;
    private TextBox txtLastName;
    private TextBox txtEmail;
    private TextBox txtPhone;
    private TextBox txtSearch;
    private Button btnSave;
    private Button btnDelete;
    private Button btnNew;
    private Button btnSearch;
    private CheckBox chkIsActive;
    private Label lblFirstName;
    private Label lblLastName;
    private Label lblEmail;
    private Label lblPhone;

    private Customer? _currentCustomer;

    /// <summary>
    /// Constructor with dependency injection
    /// </summary>
    public CustomerForm(
        ICustomerService customerService,
        ILogger<CustomerForm> logger)
    {
        _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        InitializeComponent();
        InitializeCustomComponents();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        // Form settings
        this.ClientSize = new Size(900, 600);
        this.Text = "Customer Management - DI Example";
        this.StartPosition = FormStartPosition.CenterScreen;

        this.ResumeLayout(false);
    }

    private void InitializeCustomComponents()
    {
        // DataGridView
        dgvCustomers = new DataGridView
        {
            Location = new Point(20, 20),
            Size = new Size(550, 400),
            AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            ReadOnly = true
        };
        dgvCustomers.SelectionChanged += DgvCustomers_SelectionChanged;

        // Search
        txtSearch = new TextBox { Location = new Point(20, 430), Size = new Size(450, 25) };
        btnSearch = new Button { Location = new Point(480, 428), Size = new Size(90, 30), Text = "Search" };
        btnSearch.Click += async (s, e) => await SearchCustomersAsync();

        // Input fields
        lblFirstName = new Label { Location = new Point(600, 20), Size = new Size(100, 20), Text = "First Name:" };
        txtFirstName = new TextBox { Location = new Point(600, 45), Size = new Size(250, 25) };

        lblLastName = new Label { Location = new Point(600, 80), Size = new Size(100, 20), Text = "Last Name:" };
        txtLastName = new TextBox { Location = new Point(600, 105), Size = new Size(250, 25) };

        lblEmail = new Label { Location = new Point(600, 140), Size = new Size(100, 20), Text = "Email:" };
        txtEmail = new TextBox { Location = new Point(600, 165), Size = new Size(250, 25) };

        lblPhone = new Label { Location = new Point(600, 200), Size = new Size(100, 20), Text = "Phone:" };
        txtPhone = new TextBox { Location = new Point(600, 225), Size = new Size(250, 25) };

        chkIsActive = new CheckBox { Location = new Point(600, 260), Size = new Size(100, 25), Text = "Active" };

        // Buttons
        btnNew = new Button { Location = new Point(600, 300), Size = new Size(80, 35), Text = "New" };
        btnNew.Click += BtnNew_Click;

        btnSave = new Button { Location = new Point(690, 300), Size = new Size(80, 35), Text = "Save" };
        btnSave.Click += async (s, e) => await SaveCustomerAsync();

        btnDelete = new Button { Location = new Point(780, 300), Size = new Size(80, 35), Text = "Delete" };
        btnDelete.Click += async (s, e) => await DeleteCustomerAsync();

        // Add controls to form
        this.Controls.AddRange(new Control[]
        {
            dgvCustomers, txtSearch, btnSearch,
            lblFirstName, txtFirstName, lblLastName, txtLastName,
            lblEmail, txtEmail, lblPhone, txtPhone, chkIsActive,
            btnNew, btnSave, btnDelete
        });

        // Load initial data
        this.Load += async (s, e) => await LoadCustomersAsync();
    }

    private async Task LoadCustomersAsync()
    {
        try
        {
            _logger.LogInformation("Loading customers in form");

            var customers = await _customerService.GetAllCustomersAsync();

            dgvCustomers.DataSource = customers.ToList();
            dgvCustomers.Columns["Id"].Visible = false;
            dgvCustomers.Columns["CreatedDate"].HeaderText = "Created";

            _logger.LogInformation("Loaded {Count} customers", customers.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading customers");
            MessageBox.Show($"Error loading customers: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task SearchCustomersAsync()
    {
        try
        {
            var searchTerm = txtSearch.Text;
            _logger.LogInformation("Searching for: {SearchTerm}", searchTerm);

            var customers = await _customerService.SearchCustomersAsync(searchTerm);
            dgvCustomers.DataSource = customers.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching customers");
            MessageBox.Show($"Error searching: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task SaveCustomerAsync()
    {
        try
        {
            var customer = new Customer
            {
                Id = _currentCustomer?.Id ?? 0,
                FirstName = txtFirstName.Text,
                LastName = txtLastName.Text,
                Email = txtEmail.Text,
                Phone = txtPhone.Text,
                IsActive = chkIsActive.Checked
            };

            if (_currentCustomer == null)
            {
                await _customerService.CreateCustomerAsync(customer);
                _logger.LogInformation("Created new customer");
                MessageBox.Show("Customer created successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                await _customerService.UpdateCustomerAsync(customer);
                _logger.LogInformation("Updated customer {CustomerId}", customer.Id);
                MessageBox.Show("Customer updated successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            await LoadCustomersAsync();
            ClearForm();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving customer");
            MessageBox.Show($"Error saving customer: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task DeleteCustomerAsync()
    {
        if (_currentCustomer == null)
        {
            MessageBox.Show("Please select a customer to delete", "Warning",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var result = MessageBox.Show(
            $"Are you sure you want to delete {_currentCustomer.FullName}?",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            try
            {
                await _customerService.DeleteCustomerAsync(_currentCustomer.Id);
                _logger.LogInformation("Deleted customer {CustomerId}", _currentCustomer.Id);
                MessageBox.Show("Customer deleted successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                await LoadCustomersAsync();
                ClearForm();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer");
                MessageBox.Show($"Error deleting customer: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void DgvCustomers_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvCustomers.SelectedRows.Count > 0)
        {
            _currentCustomer = dgvCustomers.SelectedRows[0].DataBoundItem as Customer;
            if (_currentCustomer != null)
            {
                txtFirstName.Text = _currentCustomer.FirstName;
                txtLastName.Text = _currentCustomer.LastName;
                txtEmail.Text = _currentCustomer.Email;
                txtPhone.Text = _currentCustomer.Phone;
                chkIsActive.Checked = _currentCustomer.IsActive;
            }
        }
    }

    private void BtnNew_Click(object? sender, EventArgs e)
    {
        ClearForm();
    }

    private void ClearForm()
    {
        _currentCustomer = null;
        txtFirstName.Clear();
        txtLastName.Clear();
        txtEmail.Clear();
        txtPhone.Clear();
        chkIsActive.Checked = true;
        txtFirstName.Focus();
    }
}
```

### Step 7: Configuration File

**File**: `appsettings.json`

```json
{
  "AppSettings": {
    "ApplicationName": "Customer Management System",
    "Database": {
      "ConnectionString": "Server=(localdb)\\mssqllocaldb;Database=CustomerManagementDb;Trusted_Connection=True;",
      "CommandTimeout": 30
    },
    "Logging": {
      "LogLevel": "Information",
      "LogPath": "logs/app.log"
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "System": "Warning"
    }
  }
}
```

**Important**: Set the file properties:
- **Copy to Output Directory**: Copy if newer
- **Build Action**: Content

## 🚀 Running the Application

1. **Build the project**:
   ```bash
   dotnet build
   ```

2. **Run the application**:
   ```bash
   dotnet run
   ```

3. **What happens**:
   - The DI container creates `CustomerRepository` with `ILogger<CustomerRepository>`
   - The container creates `CustomerService` with the repository and `ILogger<CustomerService>`
   - The container creates `CustomerForm` with the service and `ILogger<CustomerForm>`
   - The form displays, loads customers, and all operations work through injected dependencies

## 🎯 What Happens Behind the Scenes

### Dependency Resolution Flow

1. **Application starts** → `Program.Main()` is called
2. **Configuration is built** → Reads `appsettings.json`
3. **Service container is created** → `ServiceCollection` is instantiated
4. **Services are registered** → All interfaces and implementations are registered
5. **Container is built** → `BuildServiceProvider()` creates the container
6. **Form is requested** → `GetRequiredService<CustomerForm>()` is called
7. **Dependencies are resolved**:
   - Container sees `CustomerForm` needs `ICustomerService` and `ILogger<CustomerForm>`
   - Container sees `CustomerService` needs `ICustomerRepository` and `ILogger<CustomerService>`
   - Container sees `CustomerRepository` needs `ILogger<CustomerRepository>`
   - Container creates logger instances
   - Container creates repository with logger
   - Container creates service with repository and logger
   - Container creates form with service and logger
8. **Form is displayed** → `Application.Run(mainForm)`

## ✅ Benefits Demonstrated

### 1. Loose Coupling
- `CustomerForm` depends on `ICustomerService`, not the concrete implementation
- Easy to swap implementations without changing form code

### 2. Testability
- All dependencies can be mocked
- Unit tests don't need real database or UI

### 3. Single Responsibility
- Each class has one clear purpose
- Business logic separated from UI and data access

### 4. Dependency Inversion
- High-level modules (Form) don't depend on low-level modules (Repository)
- Both depend on abstractions (interfaces)

### 5. Configuration Management
- Centralized configuration
- Easy to change settings without code changes

## 🧪 Testing Example

**File**: `CustomerManagement.Tests/CustomerServiceTests.cs`

```csharp
using CustomerManagement.Models;
using CustomerManagement.Repositories;
using CustomerManagement.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CustomerManagement.Tests;

public class CustomerServiceTests
{
    [Fact]
    public async Task CreateCustomerAsync_ValidCustomer_ReturnsCreatedCustomer()
    {
        // Arrange
        var mockRepository = new Mock<ICustomerRepository>();
        var mockLogger = new Mock<ILogger<CustomerService>>();

        var customer = new Customer
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com"
        };

        mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<Customer>()))
            .ReturnsAsync(customer);

        var service = new CustomerService(mockRepository.Object, mockLogger.Object);

        // Act
        var result = await service.CreateCustomerAsync(customer);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.FirstName);
        mockRepository.Verify(r => r.CreateAsync(It.IsAny<Customer>()), Times.Once);
    }

    [Fact]
    public async Task CreateCustomerAsync_InvalidEmail_ThrowsException()
    {
        // Arrange
        var mockRepository = new Mock<ICustomerRepository>();
        var mockLogger = new Mock<ILogger<CustomerService>>();

        var customer = new Customer
        {
            FirstName = "Test",
            LastName = "User",
            Email = "invalid-email" // No @ symbol
        };

        var service = new CustomerService(mockRepository.Object, mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateCustomerAsync(customer));
    }
}
```

This is trivial with DI - we can easily mock `ICustomerRepository` without needing a real database!

## 🔄 Variations and Advanced Patterns

### Using IServiceProvider Directly

When you need to resolve services at runtime:

```csharp
public class CustomerForm : Form
{
    private readonly IServiceProvider _serviceProvider;

    public CustomerForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private void OpenReportForm()
    {
        // Resolve service on demand
        var reportService = _serviceProvider.GetRequiredService<IReportService>();
        var reportForm = new ReportForm(reportService);
        reportForm.Show();
    }
}
```

### Factory Pattern for Creating Forms

**Create a form factory**:

```csharp
public interface IFormFactory
{
    TForm Create<TForm>() where TForm : Form;
}

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

// Usage in form
private void OpenCustomerDetails()
{
    var detailsForm = _formFactory.Create<CustomerDetailsForm>();
    detailsForm.ShowDialog();
}
```

### Service Lifetime Examples

```csharp
// Singleton - ONE instance for entire application
services.AddSingleton<ICustomerRepository, CustomerRepository>();

// Scoped - ONE instance per scope (useful with IServiceScopeFactory)
services.AddScoped<ICustomerRepository, CustomerRepository>();

// Transient - NEW instance every time
services.AddTransient<ICustomerService, CustomerService>();
```

## ⚠️ Common Issues and Solutions

### Issue 1: Circular Dependencies

**Problem**: Service A depends on Service B, and Service B depends on Service A.

**Solution**: Refactor to extract common logic into a third service, or use events/mediator pattern.

### Issue 2: Form Constructor Without DI

**Problem**: Using `new CustomerForm()` instead of resolving from container.

**Solution**: Always resolve forms from the service provider or use a form factory.

### Issue 3: Disposing the Service Provider Too Early

**Problem**: Disposing provider before the application ends.

**Solution**: Use `using` statement around `Application.Run()` as shown in Program.cs.

### Issue 4: Singleton Services Holding Form References

**Problem**: Singleton service keeps reference to disposed form, causing memory leaks.

**Solution**: Use events or weak references, or make the service transient/scoped.

## 📚 Related Documentation

- **[Dependency Injection Architecture](../architecture/dependency-injection.md)** - Detailed DI concepts
- **[MVP Pattern](../architecture/mvp-pattern.md)** - Using DI with MVP
- **[Testing Guide](../testing/unit-testing.md)** - Testing with DI
- **[Configuration Management](../best-practices/configuration.md)** - Advanced configuration

## 📝 Summary

This example demonstrated:

- Setting up Microsoft.Extensions.DependencyInjection in WinForms
- Creating loosely-coupled architecture with interfaces
- Constructor injection throughout all layers
- Integration with logging and configuration
- Real-world patterns for production applications
- Testing benefits of dependency injection

**Key Takeaways**:
1. Always program to interfaces, not implementations
2. Use constructor injection for required dependencies
3. Register services with appropriate lifetimes
4. Resolve forms from the container, never use `new`
5. DI makes testing dramatically easier

Copy this example as a starting point for your WinForms applications with proper dependency injection!
