# Project Structure & Organization

> **Quick Reference**: Standard folder organization for C# WinForms projects with clear separation of concerns.

---

## 📁 Standard Project Structure

```
/ProjectName
    ├── /Forms              # Windows Forms (UI Layer)
    ├── /Controls           # Custom user controls
    ├── /Models             # Business/data models
    ├── /Services           # Business logic
    ├── /Repositories       # Data access (DB, file, API)
    ├── /Utils              # Helper methods, extensions
    ├── /Resources          # Icons, strings, localization
    ├── Program.cs          # Application entry point
    └── App.config          # Configuration
```

---

## 📂 Folder Details

### `/Forms` - UI Layer
**Purpose**: Windows Forms for user interface

**Contains**:
- Form classes (`.cs` + `.Designer.cs`)
- UI event handlers
- Form initialization and layout code

**Rules**:
- ✅ Forms should contain **UI logic only** (event handling, validation display)
- ❌ No business logic in Forms
- ❌ No direct database calls
- ✅ Delegate to Services/Presenters for business operations

**Example**:
```csharp
// MainForm.cs
public partial class MainForm : Form
{
    private readonly ICustomerService _customerService;

    public MainForm(ICustomerService customerService)
    {
        InitializeComponent();
        _customerService = customerService;
    }

    private async void btnLoad_Click(object sender, EventArgs e)
    {
        // UI logic only - delegate to service
        var customers = await _customerService.GetAllCustomersAsync();
        dgvCustomers.DataSource = customers;
    }
}
```

---

### `/Controls` - Custom User Controls
**Purpose**: Reusable custom UI components

**Contains**:
- UserControl classes
- Composite controls
- Reusable UI widgets

**Example**:
```csharp
// CustomerSearchControl.cs
public partial class CustomerSearchControl : UserControl
{
    public event EventHandler<Customer> CustomerSelected;

    // Reusable search UI logic
}
```

---

### `/Models` - Domain Models
**Purpose**: Business and data entities

**Contains**:
- POCOs (Plain Old CLR Objects)
- DTOs (Data Transfer Objects)
- ViewModels (if using MVVM)

**Rules**:
- ✅ Simple properties only
- ✅ Data annotations for validation
- ❌ No business logic (use Services instead)
- ❌ No database code

**Example**:
```csharp
// Customer.cs
public class Customer
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    public DateTime CreatedDate { get; set; }
}
```

---

### `/Services` - Business Logic Layer
**Purpose**: Business rules and application logic

**Contains**:
- Service interfaces and implementations
- Business rule validation
- Orchestration of multiple operations

**Rules**:
- ✅ Contains all business logic
- ✅ Independent of UI (can be unit tested)
- ✅ Uses Repositories for data access
- ✅ Throws domain exceptions

**Example**:
```csharp
// ICustomerService.cs
public interface ICustomerService
{
    Task<List<Customer>> GetAllCustomersAsync();
    Task<Customer> GetCustomerByIdAsync(int id);
    Task<bool> SaveCustomerAsync(Customer customer);
    Task<bool> DeleteCustomerAsync(int id);
}

// CustomerService.cs
public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(ICustomerRepository repository, ILogger<CustomerService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> SaveCustomerAsync(Customer customer)
    {
        // Business validation
        if (string.IsNullOrWhiteSpace(customer.Email))
            throw new ValidationException("Email is required");

        // Check for duplicates (business rule)
        var existing = await _repository.FindByEmailAsync(customer.Email);
        if (existing != null && existing.Id != customer.Id)
            throw new DuplicateException("Email already exists");

        // Save via repository
        return await _repository.SaveAsync(customer);
    }
}
```

---

### `/Repositories` - Data Access Layer
**Purpose**: Database and external data access

**Contains**:
- Repository interfaces and implementations
- DbContext classes (Entity Framework)
- Data access code (SQL, API calls, file I/O)

**Rules**:
- ✅ Handles all data access
- ✅ Returns domain models
- ❌ No business logic
- ✅ Async operations for I/O

**Example**:
```csharp
// ICustomerRepository.cs
public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync();
    Task<Customer> GetByIdAsync(int id);
    Task<Customer> FindByEmailAsync(string email);
    Task<bool> SaveAsync(Customer customer);
    Task<bool> DeleteAsync(int id);
}

// CustomerRepository.cs
public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Customer>> GetAllAsync()
    {
        return await _context.Customers
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> SaveAsync(Customer customer)
    {
        if (customer.Id == 0)
            _context.Customers.Add(customer);
        else
            _context.Customers.Update(customer);

        return await _context.SaveChangesAsync() > 0;
    }
}
```

---

### `/Utils` - Helper Classes
**Purpose**: Utility functions and extension methods

**Contains**:
- Extension methods
- Helper classes
- Constants
- Enums

**Example**:
```csharp
// StringExtensions.cs
public static class StringExtensions
{
    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
}

// Constants.cs
public static class Constants
{
    public const int MAX_NAME_LENGTH = 100;
    public const int DEFAULT_PAGE_SIZE = 50;
    public const string DATE_FORMAT = "yyyy-MM-dd";
}
```

---

### `/Resources` - Application Resources
**Purpose**: Non-code assets

**Contains**:
- Images, icons
- String resources (.resx files)
- Localization files
- Embedded resources

**Example**:
```csharp
// Accessing resources
var icon = Resources.AppIcon;
var message = Resources.WelcomeMessage;
```

---

## 🎯 Key Principles

### 1. Separation of Concerns
Each folder/layer has a single responsibility:

```
UI (Forms) → Presenter/Service → Repository → Database
```

- **Forms**: Handle UI events, display data
- **Services**: Business logic, validation, orchestration
- **Repositories**: Data access only

### 2. Dependency Flow
Dependencies flow **inward** (toward business logic):

```
Forms → Services → Repositories → Database
  ↓        ↓           ↓
 Uses    Uses      Uses
```

- ✅ Forms depend on Services (via interfaces)
- ✅ Services depend on Repositories (via interfaces)
- ❌ Services should NOT depend on Forms
- ❌ Repositories should NOT depend on Services

### 3. Testability
- **Services**: Easy to unit test (no UI dependencies)
- **Repositories**: Test with in-memory database
- **Forms**: Test with UI automation (FlaUI) if needed

---

## 🏛️ Architecture Pattern Recommendations

### Small Apps (1-5 forms)
```
Forms → Services → Repositories
```
- Simple 3-layer architecture
- No need for MVP/MVVM
- Use Dependency Injection

### Medium Apps (5-20 forms)
```
Forms (View) → Presenter → Services → Repositories
```
- **MVP Pattern** recommended
- Presenters contain presentation logic
- Views are passive (implement interfaces)

### Large Apps (20+ forms)
```
Forms (View) ⇄ ViewModel → Services → Repositories
```
- **MVVM Pattern** (.NET 8+)
- Two-way data binding
- Observable properties

### Enterprise Apps
```
API Layer → Services → Repositories → Database
         ↑
WinForms Client (MVP/MVVM)
```
- Client-server architecture
- WinForms as thin client
- API for business logic

---

## 📋 Project Organization Checklist

When starting a new project:

- [ ] Create folder structure as shown above
- [ ] Set up Dependency Injection container
- [ ] Define service interfaces in `/Services`
- [ ] Define repository interfaces in `/Repositories`
- [ ] Configure DbContext (if using EF Core)
- [ ] Set up logging framework
- [ ] Create base classes (BaseForm, BaseService, etc.)
- [ ] Configure App.config or appsettings.json
- [ ] Set up unit test project
- [ ] Document architecture decisions

---

## 🔗 Related Topics

- [MVP Pattern](mvp-pattern.md) - Recommended for medium apps
- [MVVM Pattern](mvvm-pattern.md) - For .NET 8+ with data binding
- [Dependency Injection](dependency-injection.md) - DI setup guide
- [Repository Pattern](../data-access/repository-pattern.md) - Data access details

---

## 📚 References

- [Microsoft - N-tier architecture](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---

**Last Updated**: 2025-11-07
