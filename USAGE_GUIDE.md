# 🚀 Usage Guide - Practical Examples

> **Step-by-step guide for using this repository to build WinForms applications**

This guide provides **practical, real-world examples** of how to use the coding standards, templates, and commands in this repository.

---

## 📋 Table of Contents

- [Prerequisites](#-prerequisites)
- [Scenario 1: Creating a Login Form](#-scenario-1-creating-a-login-form-from-scratch)
- [Scenario 2: Creating a Customer Management Form](#-scenario-2-creating-a-customer-management-form)
- [Scenario 3: Adding Validation to Existing Form](#-scenario-3-adding-validation-to-existing-form)
- [Scenario 4: Refactoring to MVP Pattern](#-scenario-4-refactoring-existing-code-to-mvp)
- [Scenario 5: Adding Error Handling](#-scenario-5-adding-error-handling-to-services)
- [Scenario 6: Setting Up DI for New Project](#-scenario-6-setting-up-di-for-new-project)
- [Scenario 7: Adding Data Binding](#-scenario-7-adding-data-binding-to-forms)
- [Quick Reference](#-quick-reference)

---

## 🎯 Prerequisites

Before starting, ensure you have:
- ✅ **.NET 8.0 SDK** or **.NET Framework 4.8** installed
- ✅ **Visual Studio 2022** or **JetBrains Rider**
- ✅ **Claude Code** (optional, for slash commands)
- ✅ Basic knowledge of **C# and WinForms**

---

## 📝 Scenario 1: Creating a Login Form (From Scratch)

**Goal**: Create a complete Login form with validation and MVP pattern.

### Step 1: Review Documentation

First, understand the patterns you'll use:

```bash
# Read these docs first:
docs/architecture/mvp-pattern.md        # Understand MVP
docs/ui-ux/input-validation.md         # Learn validation
templates/form-template.cs              # See form structure
```

### Step 2: Create the Model

Create `Models/LoginCredentials.cs`:

```csharp
namespace MyApp.Models;

public class LoginCredentials
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}
```

### Step 3: Create the Service Interface

Create `Services/IAuthenticationService.cs`:

```csharp
namespace MyApp.Services;

public interface IAuthenticationService
{
    Task<bool> LoginAsync(string username, string password, CancellationToken ct = default);
    Task LogoutAsync();
    bool IsAuthenticated { get; }
}
```

### Step 4: Create the Service Implementation

Create `Services/AuthenticationService.cs`:

```csharp
using Microsoft.Extensions.Logging;

namespace MyApp.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger;
    private bool _isAuthenticated;

    public AuthenticationService(ILogger<AuthenticationService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public bool IsAuthenticated => _isAuthenticated;

    public async Task<bool> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Login attempt for user: {Username}", username);

            // Validate inputs
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required.", nameof(username));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.", nameof(password));

            // TODO: Add your authentication logic here
            // For demo: accept "admin" / "password"
            await Task.Delay(500, ct); // Simulate API call

            _isAuthenticated = username == "admin" && password == "password";

            if (_isAuthenticated)
            {
                _logger.LogInformation("Login successful for user: {Username}", username);
            }
            else
            {
                _logger.LogWarning("Login failed for user: {Username}", username);
            }

            return _isAuthenticated;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user: {Username}", username);
            throw;
        }
    }

    public Task LogoutAsync()
    {
        _logger.LogInformation("User logged out");
        _isAuthenticated = false;
        return Task.CompletedTask;
    }
}
```

### Step 5: Create the View Interface

Create `Views/ILoginView.cs`:

```csharp
namespace MyApp.Views;

public interface ILoginView
{
    event EventHandler? LoginRequested;
    event EventHandler? CancelRequested;

    string Username { get; set; }
    string Password { get; set; }
    bool RememberMe { get; set; }
    bool IsLoading { get; set; }

    void SetFieldError(string fieldName, string errorMessage);
    void ClearAllErrors();
    void ShowError(string message);
    void CloseWithResult(bool success);
}
```

### Step 6: Create the Presenter

Create `Presenters/LoginPresenter.cs`:

```csharp
using MyApp.Services;
using MyApp.Views;
using Microsoft.Extensions.Logging;

namespace MyApp.Presenters;

public class LoginPresenter
{
    private readonly ILoginView _view;
    private readonly IAuthenticationService _authService;
    private readonly ILogger<LoginPresenter> _logger;

    public LoginPresenter(
        ILoginView view,
        IAuthenticationService authService,
        ILogger<LoginPresenter> logger)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Subscribe to view events
        _view.LoginRequested += OnLoginRequested;
        _view.CancelRequested += OnCancelRequested;
    }

    private async void OnLoginRequested(object? sender, EventArgs e)
    {
        try
        {
            _view.ClearAllErrors();
            _view.IsLoading = true;

            // Validate inputs
            bool hasErrors = false;

            if (string.IsNullOrWhiteSpace(_view.Username))
            {
                _view.SetFieldError(nameof(_view.Username), "Username is required.");
                hasErrors = true;
            }

            if (string.IsNullOrWhiteSpace(_view.Password))
            {
                _view.SetFieldError(nameof(_view.Password), "Password is required.");
                hasErrors = true;
            }

            if (hasErrors)
            {
                _logger.LogWarning("Login validation failed");
                return;
            }

            // Attempt login
            bool success = await _authService.LoginAsync(_view.Username, _view.Password);

            if (success)
            {
                _logger.LogInformation("Login successful");
                _view.CloseWithResult(true);
            }
            else
            {
                _view.ShowError("Invalid username or password.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            _view.ShowError($"Login failed: {ex.Message}");
        }
        finally
        {
            _view.IsLoading = false;
        }
    }

    private void OnCancelRequested(object? sender, EventArgs e)
    {
        _logger.LogInformation("Login cancelled");
        _view.CloseWithResult(false);
    }

    public void Dispose()
    {
        _view.LoginRequested -= OnLoginRequested;
        _view.CancelRequested -= OnCancelRequested;
    }
}
```

### Step 7: Create the Form

Create `Forms/LoginForm.cs`:

```csharp
using MyApp.Presenters;
using MyApp.Services;
using MyApp.Views;
using Microsoft.Extensions.Logging;

namespace MyApp.Forms;

public partial class LoginForm : Form, ILoginView
{
    private readonly LoginPresenter _presenter;
    private TextBox _txtUsername = null!;
    private TextBox _txtPassword = null!;
    private CheckBox _chkRememberMe = null!;
    private Button _btnLogin = null!;
    private Button _btnCancel = null!;
    private ErrorProvider _errorProvider = null!;

    #region ILoginView Events

    public event EventHandler? LoginRequested;
    public event EventHandler? CancelRequested;

    #endregion

    #region ILoginView Properties

    public string Username
    {
        get => _txtUsername.Text;
        set => _txtUsername.Text = value;
    }

    public string Password
    {
        get => _txtPassword.Text;
        set => _txtPassword.Text = value;
    }

    public bool RememberMe
    {
        get => _chkRememberMe.Checked;
        set => _chkRememberMe.Checked = value;
    }

    public bool IsLoading
    {
        get => !_btnLogin.Enabled;
        set
        {
            _btnLogin.Enabled = !value;
            _btnCancel.Enabled = !value;
            _txtUsername.Enabled = !value;
            _txtPassword.Enabled = !value;
            _chkRememberMe.Enabled = !value;
            Cursor = value ? Cursors.WaitCursor : Cursors.Default;
        }
    }

    #endregion

    public LoginForm(
        IAuthenticationService authService,
        ILogger<LoginPresenter> logger)
    {
        InitializeComponent();
        _presenter = new LoginPresenter(this, authService, logger);
    }

    private void InitializeComponent()
    {
        Text = "Login";
        Size = new Size(400, 250);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        _errorProvider = new ErrorProvider
        {
            BlinkStyle = ErrorBlinkStyle.NeverBlink
        };

        // Username
        var lblUsername = new Label
        {
            Text = "Username:",
            Location = new Point(30, 30),
            Size = new Size(80, 20)
        };

        _txtUsername = new TextBox
        {
            Location = new Point(120, 27),
            Size = new Size(220, 20)
        };

        // Password
        var lblPassword = new Label
        {
            Text = "Password:",
            Location = new Point(30, 70),
            Size = new Size(80, 20)
        };

        _txtPassword = new TextBox
        {
            Location = new Point(120, 67),
            Size = new Size(220, 20),
            UseSystemPasswordChar = true
        };

        // Remember Me
        _chkRememberMe = new CheckBox
        {
            Text = "Remember Me",
            Location = new Point(120, 100),
            Size = new Size(120, 20)
        };

        // Buttons
        _btnLogin = new Button
        {
            Text = "&Login",
            Location = new Point(170, 140),
            Size = new Size(80, 30)
        };
        _btnLogin.Click += (s, e) => LoginRequested?.Invoke(this, EventArgs.Empty);

        _btnCancel = new Button
        {
            Text = "&Cancel",
            Location = new Point(260, 140),
            Size = new Size(80, 30),
            DialogResult = DialogResult.Cancel
        };
        _btnCancel.Click += (s, e) => CancelRequested?.Invoke(this, EventArgs.Empty);

        // Add controls
        Controls.AddRange(new Control[]
        {
            lblUsername, _txtUsername,
            lblPassword, _txtPassword,
            _chkRememberMe,
            _btnLogin, _btnCancel
        });

        AcceptButton = _btnLogin;
        CancelButton = _btnCancel;
    }

    #region ILoginView Methods

    public void SetFieldError(string fieldName, string errorMessage)
    {
        Control? control = fieldName switch
        {
            nameof(Username) => _txtUsername,
            nameof(Password) => _txtPassword,
            _ => null
        };

        if (control != null)
            _errorProvider.SetError(control, errorMessage);
    }

    public void ClearAllErrors()
    {
        _errorProvider.Clear();
    }

    public void ShowError(string message)
    {
        MessageBox.Show(message, "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public void CloseWithResult(bool success)
    {
        DialogResult = success ? DialogResult.OK : DialogResult.Cancel;
        Close();
    }

    #endregion

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _presenter?.Dispose();
            _errorProvider?.Dispose();
        }
        base.Dispose(disposing);
    }
}
```

### Step 8: Register Services in DI Container

In your `Program.cs`:

```csharp
// Services
services.AddSingleton<IAuthenticationService, AuthenticationService>();

// Forms
services.AddTransient<LoginForm>();
```

### Step 9: Use the Login Form

```csharp
// In your main form or application entry point
var loginForm = serviceProvider.GetRequiredService<LoginForm>();
if (loginForm.ShowDialog() == DialogResult.OK)
{
    // User logged in successfully
    var mainForm = serviceProvider.GetRequiredService<MainForm>();
    Application.Run(mainForm);
}
else
{
    // Login cancelled or failed
    Application.Exit();
}
```

### ✅ Result

You now have a **complete login form** with:
- ✅ MVP pattern (View, Presenter, Service)
- ✅ Input validation with ErrorProvider
- ✅ Async authentication
- ✅ Proper error handling
- ✅ Dependency injection
- ✅ Logging

---

## 📝 Scenario 2: Creating a Customer Management Form

**Goal**: Create a form to manage customers (CRUD operations).

### Using Claude Code Slash Command

If you're using Claude Code, this is much easier!

**Step 1**: Use the `/create-form` command

```
/create-form CustomerManagement
```

Claude Code will:
1. Ask you what fields the customer has
2. Generate the complete MVP structure:
   - `Models/Customer.cs`
   - `Views/ICustomerView.cs`
   - `Presenters/CustomerPresenter.cs`
   - `Forms/CustomerForm.cs`
3. Follow all coding standards automatically

**Step 2**: Add the service layer

Tell Claude Code:
```
Create ICustomerService and CustomerService with CRUD operations for Customer
```

**Step 3**: Add repository if needed

```
Create ICustomerRepository and CustomerRepository using EF Core
```

**Step 4**: Wire up DI in Program.cs

```
Add CustomerForm, CustomerService, and CustomerRepository to DI container
```

### Manual Approach (Without Claude Code)

1. **Copy templates**:
   ```bash
   cp templates/form-template.cs Forms/CustomerForm.cs
   cp templates/service-template.cs Services/CustomerService.cs
   cp templates/repository-template.cs Repositories/CustomerRepository.cs
   ```

2. **Follow the Login Form example above**, but replace:
   - `Login` → `Customer`
   - Fields: Username, Password → Name, Email, Phone, Address

3. **Read the documentation**:
   - [MVP Example](docs/examples/mvp-example.md)
   - [DI Example](docs/examples/di-example.md)

---

## 📝 Scenario 3: Adding Validation to Existing Form

**Goal**: Add comprehensive validation to an existing form that lacks it.

### Using Claude Code

**Command**:
```
/add-validation CustomerForm.cs
```

Claude Code will:
1. Analyze your form
2. Add ErrorProvider
3. Add field validation in the presenter
4. Add business logic validation in the service
5. Show you exactly what changed

### Manual Approach

**Step 1**: Read the validation guide

```bash
docs/ui-ux/input-validation.md
```

**Step 2**: Add ErrorProvider to your form

```csharp
private ErrorProvider _errorProvider = new ErrorProvider
{
    BlinkStyle = ErrorBlinkStyle.NeverBlink
};
```

**Step 3**: Add validation method to your view interface

```csharp
void SetFieldError(string fieldName, string errorMessage);
void ClearAllErrors();
```

**Step 4**: Implement in your form

```csharp
public void SetFieldError(string fieldName, string errorMessage)
{
    Control? control = fieldName switch
    {
        nameof(CustomerName) => _txtName,
        nameof(CustomerEmail) => _txtEmail,
        _ => null
    };

    if (control != null)
        _errorProvider.SetError(control, errorMessage);
}

public void ClearAllErrors()
{
    _errorProvider.Clear();
}
```

**Step 5**: Add validation in presenter

```csharp
private void OnSaveRequested(object? sender, EventArgs e)
{
    _view.ClearAllErrors();
    bool hasErrors = false;

    if (string.IsNullOrWhiteSpace(_view.CustomerName))
    {
        _view.SetFieldError(nameof(_view.CustomerName), "Name is required.");
        hasErrors = true;
    }

    if (!IsValidEmail(_view.CustomerEmail))
    {
        _view.SetFieldError(nameof(_view.CustomerEmail), "Invalid email format.");
        hasErrors = true;
    }

    if (hasErrors)
        return;

    // Proceed with save...
}
```

---

## 📝 Scenario 4: Refactoring Existing Code to MVP

**Goal**: You have an existing form with business logic mixed in. Refactor to MVP pattern.

### Using Claude Code

**Command**:
```
/refactor-to-mvp CustomerForm.cs
```

Claude Code will:
1. Analyze your existing form
2. Extract business logic into a presenter
3. Create the IView interface
4. Refactor the form to implement IView
5. Show you a complete before/after comparison

### Manual Approach

**Step 1**: Read the refactoring guide

```bash
docs/architecture/mvp-pattern.md
.claude/commands/refactor-to-mvp.md
```

**Step 2**: Create the View interface

Extract all properties and events from your form:

```csharp
public interface ICustomerView
{
    // Events
    event EventHandler? LoadRequested;
    event EventHandler? SaveRequested;

    // Properties
    string CustomerName { get; set; }
    string CustomerEmail { get; set; }

    // Methods
    void ShowError(string message);
    void ShowSuccess(string message);
}
```

**Step 3**: Create the Presenter

Move all business logic from form to presenter:

```csharp
public class CustomerPresenter
{
    private readonly ICustomerView _view;
    private readonly ICustomerService _service;

    public CustomerPresenter(ICustomerView view, ICustomerService service)
    {
        _view = view;
        _service = service;

        _view.LoadRequested += OnLoadRequested;
        _view.SaveRequested += OnSaveRequested;
    }

    private async void OnLoadRequested(object? sender, EventArgs e)
    {
        var customers = await _service.GetAllAsync();
        _view.Customers = customers;
    }

    // ... more event handlers
}
```

**Step 4**: Refactor the Form

1. Implement `ICustomerView`
2. Remove business logic
3. Create presenter in constructor
4. Raise events instead of handling logic

---

## 📝 Scenario 5: Adding Error Handling to Services

**Goal**: Add comprehensive error handling and logging to your services.

### Using Claude Code

**Command**:
```
/add-error-handling CustomerService.cs
```

Claude Code will:
1. Add try-catch blocks
2. Add logging statements
3. Add proper exception wrapping
4. Add XML documentation

### Manual Approach

**Step 1**: Read the error handling guide

```bash
docs/best-practices/error-handling.md
```

**Step 2**: Inject ILogger in your service

```csharp
private readonly ILogger<CustomerService> _logger;

public CustomerService(ILogger<CustomerService> logger)
{
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
```

**Step 3**: Wrap operations in try-catch

```csharp
public async Task<Customer> CreateAsync(Customer customer)
{
    try
    {
        _logger.LogInformation("Creating customer: {Email}", customer.Email);

        // Validation
        if (customer == null)
            throw new ArgumentNullException(nameof(customer));

        ValidateCustomer(customer);

        // Business logic
        var result = await _repository.AddAsync(customer);

        _logger.LogInformation("Customer created successfully with ID: {Id}", result.Id);
        return result;
    }
    catch (ArgumentException ex)
    {
        _logger.LogWarning(ex, "Validation failed for customer");
        throw;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating customer");
        throw new InvalidOperationException("Failed to create customer.", ex);
    }
}
```

---

## 📝 Scenario 6: Setting Up DI for New Project

**Goal**: Configure Dependency Injection from scratch for a new WinForms project.

### Using Claude Code

**Command**:
```
/setup-di
```

Claude Code will:
1. Create a complete `Program.cs` with DI setup
2. Configure logging (Serilog)
3. Register services, repositories, forms
4. Set up EF Core DbContext
5. Add configuration (appsettings.json)

### Manual Approach

**Step 1**: Read the DI guide

```bash
docs/architecture/dependency-injection.md
docs/examples/di-example.md
```

**Step 2**: Install NuGet packages

```bash
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package Microsoft.Extensions.Logging
dotnet add package Serilog.Extensions.Logging
dotnet add package Serilog.Sinks.File
```

**Step 3**: Create `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

**Step 4**: Update `Program.cs`

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            ApplicationConfiguration.Initialize();

            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            // Build DI container
            var services = new ServiceCollection();
            ConfigureServices(services, configuration);
            var serviceProvider = services.BuildServiceProvider();

            // Run application
            var mainForm = serviceProvider.GetRequiredService<MainForm>();
            Application.Run(mainForm);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
            MessageBox.Show($"Fatal error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Configuration
        services.AddSingleton(configuration);

        // Logging
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog();
        });

        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        // Services
        services.AddScoped<ICustomerService, CustomerService>();

        // Forms
        services.AddTransient<MainForm>();
        services.AddTransient<CustomerForm>();
    }
}
```

---

## 📝 Scenario 7: Adding Data Binding to Forms

**Goal**: Use BindingSource for data binding instead of manual property setting.

### Using Claude Code

**Command**:
```
/add-data-binding CustomerForm.cs
```

Claude Code will:
1. Add BindingSource to your form
2. Configure data binding for controls
3. Update your presenter to work with BindingSource
4. Add INotifyPropertyChanged if needed

### Manual Approach

**Step 1**: Read the data binding guide

```bash
docs/ui-ux/data-binding.md
```

**Step 2**: Add BindingSource to your form

```csharp
private BindingSource _customerBindingSource = new BindingSource();
```

**Step 3**: Bind controls in InitializeComponent

```csharp
_txtName.DataBindings.Add(new Binding(
    "Text",
    _customerBindingSource,
    nameof(Customer.Name),
    formattingEnabled: true,
    DataSourceUpdateMode.OnPropertyChanged));

_txtEmail.DataBindings.Add(new Binding(
    "Text",
    _customerBindingSource,
    nameof(Customer.Email),
    formattingEnabled: true,
    DataSourceUpdateMode.OnPropertyChanged));
```

**Step 4**: Set DataSource in your view

```csharp
public Customer CurrentCustomer
{
    get => _customerBindingSource.Current as Customer;
    set => _customerBindingSource.DataSource = value ?? new Customer();
}
```

**Step 5**: Update in presenter

```csharp
private async void OnLoadRequested(object? sender, EventArgs e)
{
    var customer = await _service.GetByIdAsync(customerId);
    _view.CurrentCustomer = customer;
}
```

---

## 📚 Quick Reference

### All Available Slash Commands (Claude Code)

| Command | Description | When to Use |
|---------|-------------|-------------|
| `/create-form` | Generate new form with MVP | Starting a new form from scratch |
| `/review-code` | Review code against standards | Before committing changes |
| `/add-test` | Generate unit tests | After creating service/presenter |
| `/check-standards` | Quick compliance check | Quick validation |
| `/add-validation` | Add comprehensive validation | Form lacks validation |
| `/add-data-binding` | Setup data binding | Want to use BindingSource |
| `/fix-threading` | Fix cross-thread UI issues | Getting threading errors |
| `/refactor-to-mvp` | Refactor to MVP pattern | Legacy code needs refactoring |
| `/optimize-performance` | Optimize performance | Form is slow or laggy |
| `/add-error-handling` | Add error handling | Service lacks error handling |
| `/setup-di` | Setup Dependency Injection | New project needs DI |

### Documentation Quick Links

| Topic | Documentation |
|-------|---------------|
| **MVP Pattern** | [docs/architecture/mvp-pattern.md](docs/architecture/mvp-pattern.md) |
| **Dependency Injection** | [docs/architecture/dependency-injection.md](docs/architecture/dependency-injection.md) |
| **Input Validation** | [docs/ui-ux/input-validation.md](docs/ui-ux/input-validation.md) |
| **Data Binding** | [docs/ui-ux/data-binding.md](docs/ui-ux/data-binding.md) |
| **Async/Await** | [docs/best-practices/async-await.md](docs/best-practices/async-await.md) |
| **Error Handling** | [docs/best-practices/error-handling.md](docs/best-practices/error-handling.md) |
| **Thread Safety** | [docs/best-practices/thread-safety.md](docs/best-practices/thread-safety.md) |
| **Unit Testing** | [docs/testing/unit-testing.md](docs/testing/unit-testing.md) |
| **EF Core** | [docs/data-access/entity-framework.md](docs/data-access/entity-framework.md) |

### Templates

| Template | Path | Use For |
|----------|------|---------|
| **Form Template** | `templates/form-template.cs` | Creating new forms |
| **Service Template** | `templates/service-template.cs` | Creating business logic |
| **Repository Template** | `templates/repository-template.cs` | Creating data access |
| **Test Template** | `templates/test-template.cs` | Creating unit tests |

### Example Project

**Full Working Example**: [example-project/](example-project/)

Complete Customer Management application demonstrating:
- ✅ MVP Pattern
- ✅ Dependency Injection
- ✅ Entity Framework Core
- ✅ Repository Pattern
- ✅ Comprehensive Testing

---

## 💡 Tips & Best Practices

### 1. Always Start with Documentation

Before writing code, read the relevant documentation:
- Understand the **pattern** (MVP, MVVM)
- Learn the **best practices** for that feature
- See **working examples**

### 2. Use Templates as Starting Points

Don't start from scratch! Copy templates and modify:
```bash
cp templates/form-template.cs Forms/MyNewForm.cs
```

### 3. Follow the MVP Pattern

**DO**:
- ✅ Create IView interface
- ✅ Move logic to Presenter
- ✅ Keep Form thin (UI only)

**DON'T**:
- ❌ Put business logic in Form
- ❌ Access database from Form
- ❌ Complex logic in event handlers

### 4. Test as You Go

Write tests immediately after creating a component:
```bash
# After creating CustomerService
/add-test CustomerService.cs

# Run tests
dotnet test
```

### 5. Use Dependency Injection

Register everything in `Program.cs`:
```csharp
// Services (Scoped - one per operation)
services.AddScoped<ICustomerService, CustomerService>();

// Forms (Transient - new instance each time)
services.AddTransient<CustomerForm>();
```

### 6. Handle Errors Properly

Every service method should:
- ✅ Validate inputs
- ✅ Log operations
- ✅ Catch exceptions
- ✅ Wrap with meaningful errors

### 7. Use Async/Await

All I/O operations should be async:
```csharp
// ✅ Good
private async void btnLoad_Click(object sender, EventArgs e)
{
    var data = await _service.LoadDataAsync();
}

// ❌ Bad
private void btnLoad_Click(object sender, EventArgs e)
{
    var data = _service.LoadData(); // Blocks UI!
}
```

---

## 🎓 Learning Path

### For Beginners

1. **Start here**: [Scenario 1 - Login Form](#-scenario-1-creating-a-login-form-from-scratch)
2. Follow the step-by-step guide
3. Understand each layer (Model, View, Presenter, Service)
4. Run and debug the example

### For Intermediate Developers

1. Study the [Example Project](example-project/)
2. Try [Scenario 4 - Refactoring to MVP](#-scenario-4-refactoring-existing-code-to-mvp)
3. Add tests to your services
4. Explore advanced patterns

### For Teams

1. Review [CLAUDE.md](CLAUDE.md) together
2. Establish which patterns to use (MVP vs MVVM)
3. Set up DI container for your project
4. Create team-specific templates

---

## 📞 Need Help?

- **Stuck on a scenario?** Check the relevant documentation link
- **Code not working?** Review the [Example Project](example-project/)
- **Using Claude Code?** Try the relevant slash command
- **Still stuck?** Open an issue on GitHub

---

**Last Updated**: 2025-11-07
**Version**: 1.0
