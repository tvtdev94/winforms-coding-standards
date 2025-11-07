# 🚀 Usage Guide - Practical Examples

> **Step-by-step guide for using this repository to build WinForms applications**

This guide provides **practical, real-world examples** of how to use the coding standards, templates, and commands in this repository.

---

## 📋 Table of Contents

- [Prerequisites](#-prerequisites)
- [🆕 Scenario 0: Auto-Implement Complete Feature (FASTEST!)](#-scenario-0-auto-implement-complete-feature-fastest)
- [Scenario 1: Creating a Login Form](#-scenario-1-creating-a-login-form-from-scratch)
- [Scenario 2: Creating a Customer Management Form](#-scenario-2-creating-a-customer-management-form)
- [Scenario 3: Adding Validation to Existing Form](#-scenario-3-adding-validation-to-existing-form)
- [Scenario 4: Refactoring to MVP Pattern](#-scenario-4-refactoring-existing-code-to-mvp)
- [Scenario 5: Adding Error Handling](#-scenario-5-adding-error-handling-to-services)
- [Scenario 6: Setting Up DI for New Project](#-scenario-6-setting-up-di-for-new-project)
- [Scenario 7: Adding Data Binding](#-scenario-7-adding-data-binding-to-forms)
- [Scenario 8: Creating a Service Layer](#-scenario-8-creating-a-service-layer)
- [Scenario 9: Creating a Repository Layer](#-scenario-9-creating-a-repository-layer)
- [Scenario 10: Creating Dialog Forms](#-scenario-10-creating-dialog-forms)
- [Scenario 11: Creating Custom UserControls](#-scenario-11-creating-custom-usercontrols)
- [Scenario 12: Adding Logging to Application](#-scenario-12-adding-logging-to-application)
- [Scenario 13: Setting Up Configuration](#-scenario-13-setting-up-application-configuration)
- [Quick Reference](#-quick-reference)

---

## 🎯 Prerequisites

Before starting, ensure you have:
- ✅ **.NET 8.0 SDK** or **.NET Framework 4.8** installed
- ✅ **Visual Studio 2022** or **JetBrains Rider**
- ✅ **Claude Code** (optional, for slash commands)
- ✅ Basic knowledge of **C# and WinForms**

---

## 🆕 📝 Scenario 0: Auto-Implement Complete Feature (FASTEST!)

**Goal**: Implement a complete CRUD feature with ONE command instead of running multiple commands manually.

### 💡 The Problem

Traditionally, to create a CRUD feature, you need to run multiple commands:
```bash
/create-repository   # Step 1
/create-service      # Step 2
/create-form         # Step 3 (list)
/create-form         # Step 4 (edit)
/add-validation      # Step 5
/add-test            # Step 6
# Then manually register in DI...
```

**That's 6+ commands!** 😓

### ✨ The Solution: `/auto-implement`

Just describe what you want, and Claude will:
1. Analyze your requirements
2. Create an execution plan
3. Run all necessary commands automatically
4. Register everything in DI

**ONE command does it all!** 🚀

---

### Example 1: CRUD for Customer Entity

**You run:**
```
/auto-implement
```

**Claude asks:**
```
🎯 What feature would you like to implement?
```

**You answer:**
```
CRUD for Customer entity with Name, Email, Phone, and Address fields
```

**Claude analyzes and shows plan:**
```
📋 EXECUTION PLAN for "Customer CRUD"

🔍 Analysis:
- Feature type: CRUD
- Main entity: Customer
- Fields: Name, Email, Phone, Address
- Complexity: Medium

📝 Components to Create:
┌─────────────────────────────────────────────────────┐
│ 1. Entity Model                                     │
│    └─ Create Customer.cs in /Models                │
├─────────────────────────────────────────────────────┤
│ 2. Data Layer (Repository)                         │
│    ├─ Run: /create-repository                      │
│    ├─ Creates: ICustomerRepository.cs              │
│    ├─ Creates: CustomerRepository.cs               │
│    └─ Updates: AppDbContext.cs                     │
├─────────────────────────────────────────────────────┤
│ 3. Business Layer (Service)                        │
│    ├─ Run: /create-service                         │
│    ├─ Creates: ICustomerService.cs                 │
│    └─ Creates: CustomerService.cs                  │
├─────────────────────────────────────────────────────┤
│ 4. UI Layer - List Form                            │
│    ├─ Run: /create-form                            │
│    ├─ Creates: CustomerListForm.cs                 │
│    ├─ Creates: ICustomerListView.cs                │
│    └─ Creates: CustomerListPresenter.cs            │
├─────────────────────────────────────────────────────┤
│ 5. UI Layer - Edit Form                            │
│    ├─ Run: /create-form                            │
│    ├─ Creates: CustomerEditForm.cs                 │
│    ├─ Creates: ICustomerEditView.cs                │
│    └─ Creates: CustomerEditPresenter.cs            │
├─────────────────────────────────────────────────────┤
│ 6. Validation                                       │
│    ├─ Run: /add-validation                         │
│    └─ Apply to: CustomerEditForm                   │
├─────────────────────────────────────────────────────┤
│ 7. Testing                                          │
│    ├─ Run: /add-test (Service)                     │
│    └─ Run: /add-test (Repository)                  │
├─────────────────────────────────────────────────────┤
│ 8. DI Registration                                  │
│    └─ Update Program.cs with all registrations    │
└─────────────────────────────────────────────────────┘

⏱️  Estimated Time: 8-10 minutes
📁 Files to Create: 13 files
🔧 Commands to Run: 6 commands

❓ Proceed with this plan? (yes/no/customize)
```

**You answer:** `yes`

**Claude executes automatically:**
```
⏳ Step 1/8: Creating Customer entity model
✅ Created /Models/Customer.cs with properties: Id, Name, Email, Phone, Address

⏳ Step 2/8: Creating repository layer
🔧 Running: /create-repository
✅ Created IRepository.cs (generic)
✅ Created ICustomerRepository.cs (specific interface)
✅ Created CustomerRepository.cs (EF Core implementation)
✅ Updated AppDbContext.cs (added DbSet<Customer>)
✅ Added custom queries: SearchByName, SearchByEmail, GetActive

⏳ Step 3/8: Creating business service layer
🔧 Running: /create-service
✅ Created ICustomerService.cs (interface with CRUD methods)
✅ Created CustomerService.cs (implementation with validation & logging)
✅ Added business logic: Email validation, duplicate checking

⏳ Step 4/8: Creating list form (DataGridView)
🔧 Running: /create-form
✅ Created CustomerListForm.cs (MVP pattern)
✅ Created ICustomerListView.cs (view interface)
✅ Created CustomerListPresenter.cs (presentation logic)
✅ Features: Search, Sort, Delete with confirmation

⏳ Step 5/8: Creating edit form (Input fields)
🔧 Running: /create-form
✅ Created CustomerEditForm.cs (MVP pattern)
✅ Created ICustomerEditView.cs (view interface)
✅ Created CustomerEditPresenter.cs (presentation logic)
✅ Features: Create/Update with proper validation

⏳ Step 6/8: Adding comprehensive validation
🔧 Running: /add-validation
✅ Added ErrorProvider to CustomerEditForm
✅ Added validation rules: Required fields, email format, phone format
✅ Added visual feedback for errors

⏳ Step 7/8: Generating unit tests
🔧 Running: /add-test (CustomerService)
✅ Created CustomerServiceTests.cs (15 tests)
   - GetAllAsync_ReturnsAllCustomers
   - CreateAsync_ValidCustomer_CreatesSuccessfully
   - CreateAsync_DuplicateEmail_ThrowsException
   - UpdateAsync_ValidCustomer_UpdatesSuccessfully
   - DeleteAsync_ExistingId_DeletesSuccessfully
   - ... and 10 more tests

🔧 Running: /add-test (CustomerRepository)
✅ Created CustomerRepositoryTests.cs (8 integration tests)

⏳ Step 8/8: Registering in DI container
✅ Updated Program.cs:
   - services.AddDbContext<AppDbContext>(...)
   - services.AddScoped<ICustomerRepository, CustomerRepository>()
   - services.AddScoped<ICustomerService, CustomerService>()
   - services.AddTransient<CustomerListForm>()
   - services.AddTransient<CustomerEditForm>()

🎉 IMPLEMENTATION COMPLETE!

✅ Summary:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
✓ Entity: Customer (4 properties)
✓ Repository: CustomerRepository (CRUD + 3 custom queries)
✓ Service: CustomerService (CRUD + validation + logging)
✓ Forms: List form + Edit form (MVP pattern)
✓ Validation: Email, phone, required fields
✓ Tests: 23 tests total (100% passing)
✓ DI: All components registered
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

📁 Files Created (13 total):
┌─── Models
│   └── Customer.cs
├─── Repositories
│   ├── IRepository.cs
│   ├── ICustomerRepository.cs
│   └── CustomerRepository.cs
├─── Services
│   ├── ICustomerService.cs
│   └── CustomerService.cs
├─── Forms
│   ├── CustomerListForm.cs
│   └── CustomerEditForm.cs
├─── Views
│   ├── ICustomerListView.cs
│   └── ICustomerEditView.cs
├─── Presenters
│   ├── CustomerListPresenter.cs
│   └── CustomerEditPresenter.cs
└─── Tests
    ├── CustomerServiceTests.cs
    └── CustomerRepositoryTests.cs

📝 Next Steps:
1. ✅ Run migration: dotnet ef migrations add AddCustomerTable
2. ✅ Update database: dotnet ef database update
3. ✅ Run tests: dotnet test (expected: all pass)
4. ✅ Run application and open Customer menu
5. ✅ Test CRUD operations in UI
6. ✅ Review generated code and customize as needed
7. ✅ Commit: git add . && git commit -m "Add Customer CRUD feature"

🚀 Customer CRUD feature is ready to use!
⏱️  Total time: 9 minutes 32 seconds
```

**That's it!** From one command, you get a complete, working, tested feature! 🎉

---

### Example 2: User Authentication

**You:**
```
/auto-implement
User login system with remember me checkbox
```

**Claude's plan:**
```
📋 EXECUTION PLAN for "User Login System"

1. Create User entity (Username, PasswordHash, etc.)
2. /create-repository → UserRepository (FindByUsername)
3. /create-service → AuthenticationService (Login, Logout, HashPassword)
4. /create-dialog → LoginDialog (username, password, remember me)
5. /add-settings → Add JWT/session config to appsettings.json
6. /add-logging → Log authentication attempts
7. /add-test → AuthenticationServiceTests
8. DI registration + Program.cs integration

⏱️  Estimated Time: 7-8 minutes
📁 Files to Create: 10 files

Proceed? (yes/no)
```

---

### Example 3: Monthly Sales Report

**You:**
```
/auto-implement
Monthly sales report with chart showing revenue by product category
```

**Claude's plan:**
```
📋 EXECUTION PLAN for "Monthly Sales Report"

1. Create SalesReport model (Month, Category, Revenue, Count)
2. /create-service → SalesReportService (GetMonthlySales, GenerateReport)
3. /create-form → SalesReportForm (DatePicker + Chart + Export)
4. /create-custom-control → RevenueChart (custom chart control)
5. Add export to Excel functionality
6. /add-test → SalesReportServiceTests

⏱️  Estimated Time: 10-12 minutes
📁 Files to Create: 8 files

Proceed? (yes/no)
```

---

### When to Use `/auto-implement`

**✅ Use it when:**
- Creating standard CRUD features
- Implementing common patterns (login, reports, etc.)
- You want to save time
- You want complete, tested code immediately
- You trust automated workflows

**❌ Use manual commands when:**
- Very complex custom requirements
- Learning the architecture step-by-step
- Need fine-grained control over each component
- Implementing non-standard patterns

---

### Customizing the Plan

If you want to modify the execution plan:

**You:** `customize`

**Claude asks:**
```
Which steps would you like to modify?
- Remove steps? (enter numbers: 5,6)
- Add steps? (describe)
- Change order? (e.g., "move 6 before 5")
```

**You:** `Remove step 7 (I'll write tests manually later)`

**Claude:**
```
✅ Updated plan (step 7 removed)

New plan:
1. Entity Model
2. Repository
3. Service
4. List Form
5. Edit Form
6. Validation
8. DI Registration

Proceed with updated plan? (yes/no)
```

---

### Comparison: Manual vs Auto-Implement

**Manual approach (traditional):**
```bash
/create-repository      # 3 minutes
/create-service         # 3 minutes
/create-form            # 4 minutes (list)
/create-form            # 4 minutes (edit)
/add-validation         # 2 minutes
/add-test               # 3 minutes
# Manual DI registration  # 2 minutes
# Total: ~21 minutes + manual work
```

**Auto-implement approach:**
```bash
/auto-implement
"CRUD for Customer"
# Claude does everything automatically
# Total: ~9 minutes, all automated! ✨
```

**Time saved: ~12 minutes per feature!**

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

## 📝 Scenario 8: Creating a Service Layer

**Goal**: Create a CustomerService to handle business logic.

### Using Claude Code

**Command**:
```
/create-service
```

Claude Code will ask:
- Service name? → `CustomerService`
- What entity? → `Customer`
- Operations needed? → CRUD + business logic

Claude Code will generate:
1. `ICustomerService.cs` (interface)
2. `CustomerService.cs` (implementation with full error handling, logging, validation)
3. DI registration instructions

### Manual Approach

**Step 1**: Create interface

```csharp
public interface ICustomerService
{
    Task<List<Customer>> GetAllAsync(CancellationToken ct = default);
    Task<Customer?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Customer> CreateAsync(Customer customer, CancellationToken ct = default);
    Task UpdateAsync(Customer customer, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
```

**Step 2**: Create implementation (use `/create-service` or copy from template)

**Step 3**: Register in DI:
```csharp
services.AddScoped<ICustomerService, CustomerService>();
```

---

## 📝 Scenario 9: Creating a Repository Layer

**Goal**: Create repository for database access.

### Using Claude Code

**Command**:
```
/create-repository
```

Claude Code will ask:
- Entity name? → `Customer`
- Generic or specific? → `Both` (recommended)
- Special queries? → `SearchByName`, `GetActive`

Claude Code will generate:
1. `IRepository<T>.cs` (generic interface)
2. `ICustomerRepository.cs` (specific interface)
3. `CustomerRepository.cs` (implementation with EF Core)
4. Entity model (if needed)
5. DbContext updates

### Example Result

```csharp
public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<CustomerRepository> _logger;

    public async Task<List<Customer>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<List<Customer>> SearchByNameAsync(string name, CancellationToken ct = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .Where(c => c.Name.Contains(name))
            .ToListAsync(ct);
    }

    // ... other methods
}
```

**Register in DI**:
```csharp
services.AddScoped<ICustomerRepository, CustomerRepository>();
```

---

## 📝 Scenario 10: Creating Dialog Forms

**Goal**: Create a dialog to get user input.

### Using Claude Code

**Command**:
```
/create-dialog
```

Claude Code will ask:
- Dialog purpose? → `Input` (get customer name)
- Data to collect? → `Name` (string)
- Validation needed? → `Yes` (required, 3-100 chars)

Claude Code will generate complete dialog with:
- Validation using ErrorProvider
- OK/Cancel buttons with proper DialogResult
- Static helper method for easy usage

### Usage Example

```csharp
// Show the dialog
var customerName = CustomerNameDialog.ShowDialog(
    this,
    "Enter Customer Name",
    "Please enter the customer name:",
    initialValue: "");

if (customerName != null)
{
    // User clicked OK
    var customer = new Customer { Name = customerName };
    await _service.CreateAsync(customer);
}
// User clicked Cancel - do nothing
```

### Common Dialog Types Created

1. **Input Dialog**: Text, numbers, dates
2. **Confirmation Dialog**: Yes/No/Cancel
3. **Selection Dialog**: Choose from list
4. **Progress Dialog**: Show progress with cancel
5. **About Dialog**: Show app info

---

## 📝 Scenario 11: Creating Custom UserControls

**Goal**: Create reusable address entry control.

### Using Claude Code

**Command**:
```
/create-custom-control
```

Claude Code will ask:
- Control name? → `AddressControl`
- Purpose? → `Collect address information`
- Properties? → `Street, City, State, ZipCode`
- Events? → `DataChanged`

Claude Code will generate:
- Complete UserControl with all fields
- Properties with change events
- Validation method
- Clear() and Reset() methods
- Designer support with attributes

### Usage

```csharp
// In your form
var addressControl = new AddressControl
{
    Location = new Point(20, 100),
    Size = new Size(400, 150)
};

// Subscribe to events
addressControl.DataChanged += (s, e) =>
{
    _hasUnsavedChanges = true;
};

// Get data
var address = new Address
{
    Street = addressControl.Street,
    City = addressControl.City,
    State = addressControl.State,
    ZipCode = addressControl.ZipCode
};

// Validate
if (!addressControl.ValidateData())
{
    MessageBox.Show("Please check address fields");
    return;
}
```

---

## 📝 Scenario 12: Adding Logging to Application

**Goal**: Setup comprehensive logging with Serilog.

### Using Claude Code

**Command**:
```
/add-logging
```

Claude Code will ask:
- Framework? → `Serilog` (recommended)
- Log to? → `File` (rolling daily)
- Production log level? → `Information`
- Log location? → `Default` (LocalApplicationData)

Claude Code will:
1. Add NuGet packages
2. Configure Serilog in Program.cs
3. Create appsettings.json with log config
4. Add logging to existing services
5. Setup global exception handler with logging

### Result

**Program.cs** with complete Serilog setup:
```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("=== Application Starting ===");
    // ... app code
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated");
}
finally
{
    Log.CloseAndFlush();
}
```

**Service with logging**:
```csharp
public class CustomerService
{
    private readonly ILogger<CustomerService> _logger;

    public async Task<Customer> CreateAsync(Customer customer)
    {
        _logger.LogInformation("Creating customer: {Email}", customer.Email);

        try
        {
            var result = await _repository.AddAsync(customer);
            _logger.LogInformation("Customer created with ID: {Id}", result.Id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            throw;
        }
    }
}
```

**Logs location**:
- Development: `%LOCALAPPDATA%\YourCompany\YourApp\logs\app-2025-11-07.log`
- Production: Same location, auto-rotates daily

---

## 📝 Scenario 13: Setting Up Application Configuration

**Goal**: Setup configuration management with appsettings.json.

### Using Claude Code

**Command**:
```
/add-settings
```

Claude Code will ask:
- Config format? → `appsettings.json` (modern)
- Settings needed? → `App settings, Email, Features, Connection strings`
- Environment configs? → `Yes` (Development, Production)
- Encryption needed? → `Yes` (for passwords)

Claude Code will:
1. Create `appsettings.json`, `appsettings.Development.json`, `appsettings.Production.json`
2. Create strongly-typed settings classes
3. Configure in Program.cs with IOptions<T>
4. Show how to use in services/forms
5. Setup user settings for preferences

### Result

**appsettings.json**:
```json
{
  "AppSettings": {
    "AppName": "Customer Management",
    "Version": "1.0.0",
    "Theme": "Light",
    "AutoSaveInterval": 300
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app.db"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "EnableSsl": true
  }
}
```

**Strongly-typed class**:
```csharp
public class AppSettings
{
    public string AppName { get; set; }
    public string Version { get; set; }
    public string Theme { get; set; }
    public int AutoSaveInterval { get; set; }
}
```

**Usage in Form**:
```csharp
public class MainForm : Form
{
    private readonly AppSettings _settings;

    public MainForm(IOptions<AppSettings> settings)
    {
        InitializeComponent();
        _settings = settings.Value;

        // Use settings
        Text = $"{_settings.AppName} v{_settings.Version}";
        ApplyTheme(_settings.Theme);
    }
}
```

**Register in DI**:
```csharp
services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
```

---

## 📚 Quick Reference

### All Available Slash Commands (Claude Code)

| Command | Description | When to Use |
|---------|-------------|-------------|
| **Creating Code** | | |
| `/create-form` | Generate new form with MVP | Starting a new form from scratch |
| `/create-service` | Create service class | 🆕 Need business logic layer |
| `/create-repository` | Create repository class | 🆕 Need data access layer |
| `/create-dialog` | Create dialog form | 🆕 Need user input/confirmation |
| `/create-custom-control` | Create UserControl | 🆕 Need reusable UI component |
| **Adding Features** | | |
| `/add-validation` | Add comprehensive validation | Form lacks validation |
| `/add-data-binding` | Setup data binding | Want to use BindingSource |
| `/add-error-handling` | Add error handling | Service lacks error handling |
| `/add-logging` | Setup Serilog/NLog | 🆕 Need logging infrastructure |
| `/add-settings` | Setup configuration | 🆕 Need app settings management |
| **Improving Code** | | |
| `/fix-threading` | Fix cross-thread UI issues | Getting threading errors |
| `/refactor-to-mvp` | Refactor to MVP pattern | Legacy code needs refactoring |
| `/optimize-performance` | Optimize performance | Form is slow or laggy |
| **Testing & Review** | | |
| `/add-test` | Generate unit tests | After creating service/presenter |
| `/review-code` | Review code against standards | Before committing changes |
| `/check-standards` | Quick compliance check | Quick validation |
| **Project Setup** | | |
| `/setup-di` | Setup Dependency Injection | New project needs DI |

**Total Commands**: 17 (6 new ones marked with 🆕)

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
