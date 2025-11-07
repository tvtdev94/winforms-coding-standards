# Nullable Reference Types in WinForms

**Last Updated**: 2025-11-07
**Applies To**: C# 8.0+ (.NET Core 3.0+, .NET 5+, .NET Framework 4.8 with language version)

---

## 📋 Overview

Nullable Reference Types (NRT) is a C# 8.0+ feature that helps prevent `NullReferenceException` by making the compiler track which references can and cannot be null. This feature introduces compile-time null-safety without changing runtime behavior.

**Key Concepts**:
- Reference types are **non-nullable by default** when NRT is enabled
- Use `?` suffix to explicitly allow null (e.g., `string?`)
- Compiler warnings help catch potential null dereference errors
- No runtime overhead - purely compile-time feature

---

## 🎯 Why This Matters

### The Billion-Dollar Mistake

```csharp
// Classic NullReferenceException - the #1 runtime error in .NET
private void LoadCustomer(int id)
{
    Customer customer = _repository.GetCustomer(id); // May return null!
    txtName.Text = customer.Name; // 💥 Crash if customer is null
}
```

**Benefits of Nullable Reference Types**:
- ✅ **Catch null errors at compile-time** instead of runtime
- ✅ **Self-documenting code** - API signatures show intent
- ✅ **Refactoring safety** - compiler tracks null flow
- ✅ **Better IntelliSense** - IDE knows what can be null
- ✅ **Reduced defensive coding** - less unnecessary null checks

---

## Enabling Nullable Context

### Project-Wide Configuration

Edit your `.csproj` file:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>

    <!-- Enable nullable reference types -->
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

**Nullable Context Options**:

| Option | Description | Use Case |
|--------|-------------|----------|
| `enable` | Full nullability checks | New projects, fully migrated code |
| `disable` | No nullability checks (default) | Legacy code |
| `warnings` | Enables warnings but not annotations | Migration step 1 |
| `annotations` | Enables annotations but not warnings | Migration step 2 |

**Gradual Migration Example**:

```xml
<!-- Step 1: See warnings without breaking build -->
<Nullable>warnings</Nullable>

<!-- Step 2: Add annotations, refine warnings -->
<Nullable>annotations</Nullable>

<!-- Step 3: Full enforcement -->
<Nullable>enable</Nullable>
```

### File-Level Control

Use `#nullable` directive for granular control:

```csharp
#nullable enable // Enable for this file

using System;
using System.Windows.Forms;

namespace MyApp.Forms
{
    public partial class CustomerForm : Form
    {
        private string _customerName; // Non-nullable - must be initialized
        private string? _notes; // Nullable - can be null

        public CustomerForm()
        {
            InitializeComponent();
            _customerName = string.Empty; // Must initialize
        }
    }
}

#nullable disable // Disable after this point
```

**File-Level Directive Options**:

```csharp
#nullable enable          // Enable warnings and annotations
#nullable disable         // Disable (legacy behavior)
#nullable restore        // Restore project-level setting
#nullable enable warnings // Only warnings
#nullable enable annotations // Only annotations
```

---

## Nullable Annotations

### Nullable vs Non-Nullable Types

```csharp
// Non-nullable reference types (default with NRT enabled)
string name;           // Must never be null
Customer customer;     // Must never be null
List<int> numbers;     // Must never be null

// Nullable reference types (explicit)
string? optionalName;     // Can be null
Customer? maybeCustomer;  // Can be null
List<int>? maybeNumbers;  // Can be null

// Value types (unchanged behavior)
int count;            // Cannot be null (value type)
int? optionalCount;   // Can be null (Nullable<int>)
```

### Nullable Warnings

Common compiler warnings and their meanings:

#### CS8600: Converting null literal or possible null value to non-nullable type

```csharp
❌ // Warning CS8600
string? nullableName = GetNullableName();
string name = nullableName; // Converting nullable to non-nullable

✅ // Fix 1: Null check
string? nullableName = GetNullableName();
if (nullableName != null)
{
    string name = nullableName; // OK - compiler knows it's not null
}

✅ // Fix 2: Null-coalescing
string name = GetNullableName() ?? "Default";
```

#### CS8602: Dereference of a possibly null reference

```csharp
❌ // Warning CS8602
string? name = GetNullableName();
int length = name.Length; // Dereferencing possibly null

✅ // Fix: Null check
string? name = GetNullableName();
int length = name?.Length ?? 0; // Safe navigation
```

#### CS8603: Possible null reference return

```csharp
❌ // Warning CS8603
public string GetName()
{
    return _name; // _name is string?, but return type is string
}

✅ // Fix 1: Change return type
public string? GetName()
{
    return _name; // OK - return type matches
}

✅ // Fix 2: Guarantee non-null
public string GetName()
{
    return _name ?? "Unknown"; // Never returns null
}
```

#### CS8618: Non-nullable field/property is uninitialized

```csharp
❌ // Warning CS8618
public class CustomerForm : Form
{
    private ICustomerService _service; // Not initialized

    public CustomerForm()
    {
        InitializeComponent();
        // _service never assigned!
    }
}

✅ // Fix 1: Initialize in constructor
public class CustomerForm : Form
{
    private ICustomerService _service;

    public CustomerForm(ICustomerService service)
    {
        InitializeComponent();
        _service = service; // Initialized
    }
}

✅ // Fix 2: Initialize at declaration
public class CustomerForm : Form
{
    private ICustomerService _service = new CustomerService();
}

✅ // Fix 3: Make nullable if it can be null
public class CustomerForm : Form
{
    private ICustomerService? _service; // Explicitly nullable
}
```

---

## WinForms-Specific Scenarios

### Control Properties

Many WinForms controls have properties that can be null:

```csharp
❌ // Unsafe - SelectedItem can be null
private void cmbCategories_SelectedIndexChanged(object sender, EventArgs e)
{
    var category = (Category)cmbCategories.SelectedItem;
    LoadProducts(category.Id); // 💥 NullReferenceException if no selection
}

✅ // Safe - Check for null
private void cmbCategories_SelectedIndexChanged(object sender, EventArgs e)
{
    if (cmbCategories.SelectedItem is Category category)
    {
        LoadProducts(category.Id);
    }
}

✅ // Alternative - pattern matching with null check
private void cmbCategories_SelectedIndexChanged(object sender, EventArgs e)
{
    var category = cmbCategories.SelectedItem as Category;
    if (category is not null)
    {
        LoadProducts(category.Id);
    }
}
```

### FindControl Patterns

```csharp
❌ // Unsafe - Controls.Find can return empty array
private void ConfigureButton()
{
    var button = (Button)this.Controls.Find("btnSave", true)[0];
    button.Enabled = true; // 💥 IndexOutOfRangeException or NullReferenceException
}

✅ // Safe - Check array length
private void ConfigureButton()
{
    var controls = this.Controls.Find("btnSave", true);
    if (controls.Length > 0 && controls[0] is Button button)
    {
        button.Enabled = true;
    }
}

✅ // Safe - LINQ FirstOrDefault
private void ConfigureButton()
{
    var button = this.Controls.Find("btnSave", true)
        .OfType<Button>()
        .FirstOrDefault();

    if (button is not null)
    {
        button.Enabled = true;
    }
}
```

### Event Args

Event handler signatures with nullable awareness:

```csharp
// Standard event handler signature
private void btnSave_Click(object? sender, EventArgs e)
{
    // sender is nullable - can be null per .NET event pattern
    if (sender is Button button)
    {
        button.Enabled = false;
    }

    SaveData();
}

// Custom event args
public class CustomerChangedEventArgs : EventArgs
{
    public Customer? Customer { get; }
    public string? ChangeReason { get; }

    public CustomerChangedEventArgs(Customer? customer, string? reason = null)
    {
        Customer = customer;
        ChangeReason = reason;
    }
}

// Handler for custom event
private void OnCustomerChanged(object? sender, CustomerChangedEventArgs e)
{
    if (e.Customer is not null)
    {
        txtName.Text = e.Customer.Name;
        txtEmail.Text = e.Customer.Email ?? string.Empty;
    }
}
```

### Designer-Generated Code

The WinForms designer generates code that may trigger nullable warnings:

```csharp
// CustomerForm.Designer.cs
partial class CustomerForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null; // ⚠️ CS8618

    ❌ // Generated fields without initialization
    private Button btnSave;
    private TextBox txtName;

    ✅ // Fix: Use null-forgiving operator (designer guarantees initialization)
    private Button btnSave = null!;
    private TextBox txtName = null!;

    // Or suppress in InitializeComponent
    private void InitializeComponent()
    {
        this.btnSave = new Button();
        this.txtName = new TextBox();
        // ... rest of designer code
    }
}
```

**Best Practice for Designer Code**:

```csharp
// CustomerForm.cs
#nullable enable

public partial class CustomerForm : Form
{
    private readonly ICustomerService _service;

    public CustomerForm(ICustomerService service)
    {
        _service = service;
        InitializeComponent(); // Designer fields initialized here
    }

    private void btnSave_Click(object? sender, EventArgs e)
    {
        // Safe - btnSave initialized by designer
        btnSave.Enabled = false;
    }
}

// CustomerForm.Designer.cs
#nullable disable // Disable for designer file to avoid warnings

partial class CustomerForm
{
    private Button btnSave;
    private TextBox txtName;
    // Designer doesn't understand NRT
}
```

### Data Binding

```csharp
// Model with nullable properties
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // Required
    public string? Email { get; set; } // Optional
    public string? Phone { get; set; } // Optional
    public DateTime? LastPurchase { get; set; } // Optional
}

// Binding to DataGridView
private void LoadCustomers()
{
    var customers = _repository.GetAllCustomers();

    dgvCustomers.AutoGenerateColumns = false;
    dgvCustomers.DataSource = customers;

    // Handle nullable columns in CellFormatting
}

private void dgvCustomers_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
{
    if (dgvCustomers.Columns[e.ColumnIndex].Name == "Email")
    {
        // Handle null email
        e.Value = e.Value ?? "(No email)";
    }

    if (dgvCustomers.Columns[e.ColumnIndex].Name == "LastPurchase"
        && e.Value is DateTime lastPurchase)
    {
        e.Value = lastPurchase.ToShortDateString();
    }
    else if (e.Value is null)
    {
        e.Value = "Never";
    }
}
```

---

## Nullable Operators

### Null-Coalescing Operator (??)

Provides default values for null:

```csharp
// Basic usage
string? nullableName = GetName();
string displayName = nullableName ?? "Guest";

// WinForms examples
txtEmail.Text = customer.Email ?? string.Empty;
lblStatus.Text = errorMessage ?? "No errors";

// Chaining
string name = customer.PreferredName
    ?? customer.FirstName
    ?? customer.Username
    ?? "Unknown";

// With method calls
var service = _cachedService ?? CreateService();
```

### Null-Conditional Operator (?.)

Safe member access:

```csharp
// Basic usage
int? length = name?.Length;
string? upper = name?.ToUpper();

// Chaining
string? city = customer?.Address?.City;

// With method calls
int count = customers?.Count ?? 0;

// WinForms examples
var selectedCategory = (cmbCategories.SelectedItem as Category)?.Name;
int? selectedIndex = dgvCustomers.SelectedRows?.Count > 0
    ? dgvCustomers.SelectedRows[0]?.Index
    : null;

// Event invocation
CustomerChanged?.Invoke(this, new CustomerChangedEventArgs(customer));
```

### Null-Forgiving Operator (!)

Tells compiler "I know this isn't null":

```csharp
⚠️ // Use sparingly - you're overriding the compiler's safety!

// When you KNOW it's not null (but compiler doesn't)
private void Form_Load(object sender, EventArgs e)
{
    // Designer guarantees btnSave is initialized
    btnSave!.Click += btnSave_Click;
}

// After null check the compiler doesn't understand
string? input = txtInput.Text;
if (string.IsNullOrEmpty(input))
{
    return;
}
// Compiler doesn't know input is not null here (false positive)
ProcessInput(input!); // Suppress warning

❌ // DON'T use to suppress real warnings
string? name = GetNullableName();
ProcessName(name!); // 💥 Dangerous - can still be null at runtime!

✅ // DO check first
string? name = GetNullableName();
if (name is not null)
{
    ProcessName(name); // No need for !, compiler knows it's not null
}
```

### Null-Coalescing Assignment (??=)

Lazy initialization:

```csharp
// Basic usage
private List<Customer>? _customerCache;

public List<Customer> GetCustomers()
{
    // Initialize only if null
    _customerCache ??= LoadCustomersFromDatabase();
    return _customerCache;
}

// WinForms examples
private Form? _settingsForm;

private void ShowSettings()
{
    _settingsForm ??= new SettingsForm();
    _settingsForm.ShowDialog();
}

// Configuration
private AppSettings? _settings;

private AppSettings Settings
{
    get
    {
        _settings ??= AppSettings.Load();
        return _settings;
    }
}
```

---

## Attributes for Nullability

### [NotNull], [MaybeNull], [AllowNull]

These attributes provide additional hints to the compiler:

```csharp
using System.Diagnostics.CodeAnalysis;

public class CustomerService
{
    // Property that accepts null but never returns null
    [AllowNull]
    public string SearchFilter
    {
        get => _searchFilter;
        set => _searchFilter = value ?? string.Empty;
    }
    private string _searchFilter = string.Empty;

    // Method that may return null even though type is non-nullable
    [return: MaybeNull]
    public T GetValue<T>(string key)
    {
        if (_cache.TryGetValue(key, out var value))
        {
            return (T)value;
        }
        return default(T); // May be null for reference types
    }

    // Method that ensures non-null output
    [return: NotNull]
    public string? GetCustomerName(int id)
    {
        return _repository.GetCustomerName(id) ?? "Unknown";
    }
}
```

### [NotNullWhen], [NotNullIfNotNull]

Advanced flow analysis attributes:

```csharp
using System.Diagnostics.CodeAnalysis;

public class ValidationHelper
{
    // Tells compiler: if return is true, value is not null
    public static bool TryGetCustomer(
        int id,
        [NotNullWhen(true)] out Customer? customer)
    {
        customer = _repository.FindCustomer(id);
        return customer is not null;
    }
}

// Usage
if (ValidationHelper.TryGetCustomer(customerId, out var customer))
{
    // Compiler knows customer is not null here
    Console.WriteLine(customer.Name); // No warning
}

// NotNullIfNotNull example
public class StringHelper
{
    // If input is not null, output is not null
    [return: NotNullIfNotNull(nameof(input))]
    public static string? Sanitize(string? input)
    {
        return input?.Trim().Replace("\n", " ");
    }
}
```

---

## Migration Strategy

### Enabling in Existing Projects

**Step-by-Step Approach**:

```xml
<!-- Phase 1: Assess - Enable warnings only -->
<Nullable>warnings</Nullable>
```

1. **Build and review warnings** - understand the scope
2. **Fix critical warnings** - actual bugs found
3. **Categorize remaining warnings** - false positives vs real issues

```xml
<!-- Phase 2: Annotate - Add annotations -->
<Nullable>annotations</Nullable>
```

4. **Add nullable annotations to APIs** - public methods, properties
5. **Update signatures** - mark what can be null with `?`
6. **Add null checks** - defensive coding where needed

```xml
<!-- Phase 3: Enforce - Full nullability -->
<Nullable>enable</Nullable>
```

7. **Fix all warnings** - achieve zero warnings
8. **Test thoroughly** - ensure no runtime breaks
9. **Monitor** - watch for new warnings

**Incremental Migration Example**:

```csharp
// Before - Legacy code
public class CustomerService
{
    private IRepository _repo;

    public Customer GetCustomer(int id)
    {
        return _repo.GetCustomer(id); // May return null
    }

    public void UpdateCustomer(Customer customer, string notes)
    {
        customer.Notes = notes; // notes may be null
        _repo.Update(customer);
    }
}

// After - With nullable annotations
#nullable enable

public class CustomerService
{
    private readonly IRepository _repo;

    public CustomerService(IRepository repo)
    {
        _repo = repo; // Must be initialized
    }

    // Explicitly show this can return null
    public Customer? GetCustomer(int id)
    {
        return _repo.GetCustomer(id);
    }

    // Show notes is optional
    public void UpdateCustomer(Customer customer, string? notes)
    {
        customer.Notes = notes ?? string.Empty;
        _repo.Update(customer);
    }
}
```

### Suppressing Warnings

Sometimes you need to suppress warnings:

```csharp
// File-level suppression
#nullable disable
// Legacy code here
#nullable restore

// Pragma suppression for specific lines
#pragma warning disable CS8602 // Dereference of a possibly null reference
var name = customer.Name; // Known to be safe in this context
#pragma warning restore CS8602

// Attribute suppression
[SuppressMessage("Microsoft.Usage", "CS8602:Dereference of a possibly null reference")]
private void ProcessCustomer(Customer? customer)
{
    // Special case where we know customer is not null
    var name = customer.Name;
}
```

**When Suppression is Appropriate**:
- ✅ Designer-generated code
- ✅ Third-party library interop without NRT support
- ✅ Complex control flow compiler can't analyze
- ❌ Avoiding fixing real bugs
- ❌ Laziness in new code

---

## Best Practices

### ✅ DO:

**1. Be explicit about nullability in public APIs**

```csharp
✅ // Clear contract
public interface ICustomerService
{
    Customer? FindCustomer(int id); // Can return null
    Customer GetCustomer(int id); // Never returns null (or throws)
    void UpdateEmail(int id, string? email); // Email can be null
}
```

**2. Use pattern matching for null checks**

```csharp
✅ // Modern C# syntax
if (customer is not null)
{
    ProcessCustomer(customer);
}

if (cmbCategories.SelectedItem is Category category)
{
    LoadProducts(category);
}
```

**3. Initialize non-nullable fields**

```csharp
✅ // All non-nullable fields initialized
public class CustomerForm : Form
{
    private readonly ICustomerService _service;
    private string _currentFilter = string.Empty;

    public CustomerForm(ICustomerService service)
    {
        _service = service;
        InitializeComponent();
    }
}
```

**4. Use null-coalescing for defaults**

```csharp
✅ // Clean default value handling
txtEmail.Text = customer.Email ?? string.Empty;
lblLastPurchase.Text = customer.LastPurchase?.ToShortDateString() ?? "Never";
var pageSize = userSettings?.PageSize ?? 25;
```

**5. Make intent clear with nullable value types**

```csharp
✅ // Clear when value is optional
public class SearchCriteria
{
    public string Name { get; set; } = string.Empty;
    public int? MinAge { get; set; } // Optional filter
    public int? MaxAge { get; set; } // Optional filter
    public DateTime? RegisteredAfter { get; set; } // Optional filter
}
```

**6. Use [NotNull] attributes for complex scenarios**

```csharp
✅ // Help the compiler understand your code
public bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
{
    if (_dictionary.ContainsKey(key))
    {
        value = _dictionary[key];
        return true;
    }
    value = null;
    return false;
}
```

**7. Validate external data**

```csharp
✅ // Never trust external input
public void ImportCustomer(string jsonData)
{
    var customer = JsonSerializer.Deserialize<Customer>(jsonData);

    if (customer is null)
    {
        throw new ArgumentException("Invalid customer data", nameof(jsonData));
    }

    if (string.IsNullOrWhiteSpace(customer.Name))
    {
        throw new ArgumentException("Customer name is required", nameof(jsonData));
    }

    _repository.Add(customer);
}
```

**8. Use required keyword (C# 11+)**

```csharp
✅ // Enforce initialization
public class Customer
{
    public required int Id { get; init; }
    public required string Name { get; set; }
    public string? Email { get; set; }
}

// Must set required properties
var customer = new Customer
{
    Id = 1,
    Name = "John" // Required
};
```

### ❌ DON'T:

**1. Don't overuse null-forgiving operator**

```csharp
❌ // Dangerous - defeats the purpose of NRT
var customer = GetCustomer(id)!;
ProcessCustomer(customer!.Name!);

✅ // Proper null checking
var customer = GetCustomer(id);
if (customer is not null && customer.Name is not null)
{
    ProcessCustomer(customer.Name);
}
```

**2. Don't return null from non-nullable methods**

```csharp
❌ // Lying to the compiler
public string GetCustomerName(int id)
{
    var customer = _repository.FindCustomer(id);
    return customer?.Name; // 💥 Can return null!
}

✅ // Be honest
public string? GetCustomerName(int id)
{
    var customer = _repository.FindCustomer(id);
    return customer?.Name;
}

✅ // Or guarantee non-null
public string GetCustomerName(int id)
{
    var customer = _repository.FindCustomer(id);
    return customer?.Name ?? "Unknown";
}
```

**3. Don't ignore nullable warnings**

```csharp
❌ // Ignoring the problem
#pragma warning disable CS8602
var name = customer.Name;
#pragma warning restore CS8602

✅ // Fix the root cause
var name = customer?.Name ?? "Unknown";
```

**4. Don't use null for missing state**

```csharp
❌ // Null has ambiguous meaning
public Customer? GetCustomer(int id)
{
    // Does null mean "not found" or "error"?
    return null;
}

✅ // Use Optional/Result pattern
public Result<Customer> GetCustomer(int id)
{
    var customer = _repository.FindCustomer(id);
    return customer is not null
        ? Result<Customer>.Success(customer)
        : Result<Customer>.NotFound();
}
```

**5. Don't mix nullable and non-nullable carelessly**

```csharp
❌ // Inconsistent API design
public void ProcessData(string name, string? email, string address, string? phone)
{
    // Hard to remember which can be null
}

✅ // Group with DTOs or use clear patterns
public class CustomerContact
{
    public string Name { get; set; } = string.Empty; // Required
    public string Address { get; set; } = string.Empty; // Required
    public string? Email { get; set; } // Optional
    public string? Phone { get; set; } // Optional
}

public void ProcessData(CustomerContact contact) { }
```

**6. Don't disable nullable for new code**

```csharp
❌ // Writing new code without NRT
#nullable disable
public class NewFeatureForm : Form
{
    private string _userName; // Should be nullable or initialized
    private Customer _customer; // Should be nullable or initialized
}

✅ // Enable NRT for all new code
#nullable enable
public class NewFeatureForm : Form
{
    private string _userName = string.Empty;
    private Customer? _customer;
}
```

---

## Common Patterns

### Null Checks - Modern Syntax

```csharp
// Pattern matching (C# 9+)
if (customer is null) { /* handle null */ }
if (customer is not null) { /* use customer */ }

// Traditional (still valid)
if (customer == null) { /* handle null */ }
if (customer != null) { /* use customer */ }

// Property pattern
if (customer is { Name: not null })
{
    // Customer exists and has name
}

// Combined patterns
if (customer is { Id: > 0, Email: not null } c)
{
    SendEmail(c.Email);
}
```

### Initializing Fields

```csharp
public class CustomerForm : Form
{
    // Pattern 1: Initialize at declaration
    private string _filter = string.Empty;
    private List<Customer> _customers = new();

    // Pattern 2: Initialize in constructor
    private readonly ICustomerService _service;
    private readonly ILogger _logger;

    // Pattern 3: Nullable for optional
    private Customer? _selectedCustomer;
    private SettingsForm? _settingsDialog;

    // Pattern 4: Lazy initialization
    private CustomerValidator? _validator;
    private CustomerValidator Validator => _validator ??= new CustomerValidator();

    // Pattern 5: Designer controls (use null-forgiving)
    private Button btnSave = null!;
    private TextBox txtName = null!;

    public CustomerForm(ICustomerService service, ILogger logger)
    {
        _service = service;
        _logger = logger;
        InitializeComponent();
    }
}
```

### Optional Parameters

```csharp
// Nullable parameters with defaults
public void LoadCustomers(
    string? searchTerm = null,
    int? pageSize = null,
    DateTime? startDate = null)
{
    var query = _repository.GetCustomersQuery();

    if (searchTerm is not null)
    {
        query = query.Where(c => c.Name.Contains(searchTerm));
    }

    var size = pageSize ?? 25;
    query = query.Take(size);

    if (startDate is not null)
    {
        query = query.Where(c => c.CreatedDate >= startDate);
    }

    return query.ToList();
}

// Usage
LoadCustomers(); // All defaults
LoadCustomers(searchTerm: "John"); // Override one
LoadCustomers(pageSize: 50, startDate: DateTime.Today); // Override multiple
```

---

## Complete Working Examples

### Example 1: Form with Nullable-Aware Controls

```csharp
#nullable enable

using System;
using System.Windows.Forms;

namespace MyApp.Forms
{
    public partial class CustomerEditForm : Form
    {
        private readonly ICustomerService _service;
        private readonly ILogger _logger;
        private Customer? _currentCustomer;

        // Designer controls - initialized by InitializeComponent
        private TextBox txtName = null!;
        private TextBox txtEmail = null!;
        private TextBox txtPhone = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;

        public CustomerEditForm(ICustomerService service, ILogger logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            InitializeComponent();
        }

        public void LoadCustomer(int? customerId)
        {
            if (customerId is null or <= 0)
            {
                // New customer
                _currentCustomer = null;
                ClearForm();
                return;
            }

            try
            {
                _currentCustomer = _service.GetCustomer(customerId.Value);

                if (_currentCustomer is null)
                {
                    MessageBox.Show("Customer not found", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                PopulateForm(_currentCustomer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load customer {Id}", customerId);
                MessageBox.Show($"Error loading customer: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateForm(Customer customer)
        {
            txtName.Text = customer.Name;
            txtEmail.Text = customer.Email ?? string.Empty;
            txtPhone.Text = customer.Phone ?? string.Empty;
        }

        private void ClearForm()
        {
            txtName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtPhone.Text = string.Empty;
        }

        private async void btnSave_Click(object? sender, EventArgs e)
        {
            if (!ValidateInput(out var validationError))
            {
                MessageBox.Show(validationError, "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var customer = _currentCustomer ?? new Customer();
                customer.Name = txtName.Text.Trim();
                customer.Email = string.IsNullOrWhiteSpace(txtEmail.Text)
                    ? null
                    : txtEmail.Text.Trim();
                customer.Phone = string.IsNullOrWhiteSpace(txtPhone.Text)
                    ? null
                    : txtPhone.Text.Trim();

                if (_currentCustomer is null)
                {
                    await _service.CreateCustomerAsync(customer);
                }
                else
                {
                    await _service.UpdateCustomerAsync(customer);
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save customer");
                MessageBox.Show($"Error saving customer: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput(out string? errorMessage)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                errorMessage = "Customer name is required";
                return false;
            }

            var email = txtEmail.Text.Trim();
            if (!string.IsNullOrEmpty(email) && !IsValidEmail(email))
            {
                errorMessage = "Invalid email format";
                return false;
            }

            errorMessage = null;
            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
```

### Example 2: Service with Nullable Return Types

```csharp
#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyApp.Services
{
    public interface ICustomerService
    {
        // Explicit nullability in contract
        Customer? FindCustomer(int id);
        Customer GetCustomer(int id); // Throws if not found
        Task<Customer?> FindCustomerAsync(int id);
        List<Customer> SearchCustomers(string? searchTerm);
        Task CreateCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);
        bool TryGetCustomer(int id, out Customer? customer);
    }

    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly ILogger _logger;

        public CustomerService(ICustomerRepository repository, ILogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Returns null if not found
        public Customer? FindCustomer(int id)
        {
            _logger.LogDebug("Finding customer {Id}", id);
            return _repository.GetById(id);
        }

        // Throws if not found - guaranteed non-null
        public Customer GetCustomer(int id)
        {
            var customer = FindCustomer(id);
            if (customer is null)
            {
                throw new CustomerNotFoundException($"Customer {id} not found");
            }
            return customer;
        }

        public async Task<Customer?> FindCustomerAsync(int id)
        {
            _logger.LogDebug("Finding customer async {Id}", id);
            return await _repository.GetByIdAsync(id);
        }

        public List<Customer> SearchCustomers(string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return _repository.GetAll();
            }

            return _repository.Search(searchTerm);
        }

        public async Task CreateCustomerAsync(Customer customer)
        {
            if (customer is null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            if (string.IsNullOrWhiteSpace(customer.Name))
            {
                throw new ArgumentException("Customer name is required", nameof(customer));
            }

            _logger.LogInformation("Creating customer {Name}", customer.Name);
            await _repository.AddAsync(customer);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            if (customer is null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            var existing = await FindCustomerAsync(customer.Id);
            if (existing is null)
            {
                throw new CustomerNotFoundException($"Customer {customer.Id} not found");
            }

            _logger.LogInformation("Updating customer {Id}", customer.Id);
            await _repository.UpdateAsync(customer);
        }

        public bool TryGetCustomer(int id, out Customer? customer)
        {
            customer = FindCustomer(id);
            return customer is not null;
        }
    }

    public class CustomerNotFoundException : Exception
    {
        public CustomerNotFoundException(string message) : base(message) { }
    }
}
```

### Example 3: Repository with Null Handling

```csharp
#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Data
{
    public interface ICustomerRepository
    {
        Customer? GetById(int id);
        Task<Customer?> GetByIdAsync(int id);
        List<Customer> GetAll();
        List<Customer> Search(string searchTerm);
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(int id);
    }

    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Customer? GetById(int id)
        {
            return _context.Customers.Find(id);
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public List<Customer> GetAll()
        {
            return _context.Customers.ToList();
        }

        public List<Customer> Search(string searchTerm)
        {
            // searchTerm is non-nullable here, but we're defensive
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<Customer>();
            }

            return _context.Customers
                .Where(c => c.Name.Contains(searchTerm)
                    || (c.Email != null && c.Email.Contains(searchTerm)))
                .ToList();
        }

        public async Task AddAsync(Customer customer)
        {
            if (customer is null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Customer customer)
        {
            if (customer is null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await GetByIdAsync(id);
            if (customer is not null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }
    }
}
```

---

## Troubleshooting

### Common Issues and Solutions

| Warning | Cause | Solution |
|---------|-------|----------|
| CS8618 | Non-nullable field not initialized | Initialize in constructor or make nullable |
| CS8600 | Converting null to non-nullable | Add null check or use `??` operator |
| CS8602 | Dereferencing possibly null | Use `?.` or null check |
| CS8603 | Returning null from non-nullable | Change return type to `T?` or guarantee non-null |
| CS8604 | Passing null to non-nullable parameter | Check argument before passing or change parameter to `T?` |
| CS8625 | Cannot convert null to non-nullable | Use default value or make type nullable |

### Debugging Tips

```csharp
// Enable nullable warnings in specific file
#nullable enable warnings

// Check current nullable context
#if NULLABLE
    // Code when nullable is enabled
#endif

// Use [NotNullWhen] to help flow analysis
public bool IsValid([NotNullWhen(true)] out string? error)
{
    if (_hasError)
    {
        error = _errorMessage;
        return false;
    }
    error = null;
    return true;
}

// Use Debug.Assert for development-time checks
Debug.Assert(customer is not null, "Customer should not be null here");
```

---

## Related Topics

- **[Code Style](../conventions/code-style.md)** - General coding conventions
- **[Error Handling & Logging](../best-practices/error-handling.md)** - Exception handling patterns
- **[LINQ Best Practices](linq-practices.md)** - LINQ with nullable types
- **[Input Validation](../ui-ux/input-validation.md)** - Validating nullable user input
- **[Dependency Injection](../architecture/dependency-injection.md)** - DI with nullable awareness

---

**Summary**: Nullable Reference Types are a powerful tool for preventing null reference exceptions at compile time. Enable them for all new WinForms projects, be explicit about nullability in your APIs, and use modern C# patterns for null checking and handling.
