# C# WinForms Coding Standards Guide

> **Purpose**: Complete reference for naming conventions, code style, and formatting standards
> **Audience**: AI assistants and developers working on WinForms projects

---

## 📋 Table of Contents

1. [Naming Conventions](#naming-conventions)
2. [Control Prefixes](#control-prefixes)
3. [Code Style](#code-style)
4. [File Organization](#file-organization)
5. [Comments & Documentation](#comments--documentation)

---

## Naming Conventions

### General Rules

| Type | Convention | Example |
|------|-----------|---------|
| **Class** | PascalCase | `CustomerService`, `MainForm`, `ProductRepository` |
| **Interface** | I + PascalCase | `ICustomerService`, `IFormFactory`, `IUnitOfWork` |
| **Method** | PascalCase | `LoadCustomers()`, `SaveData()`, `ValidateInput()` |
| **Property** | PascalCase | `CustomerName`, `IsActive`, `TotalAmount` |
| **Field (private)** | _camelCase | `_customerService`, `_unitOfWork`, `_logger` |
| **Field (const)** | UPPER_SNAKE_CASE | `MAX_RETRY_COUNT`, `DEFAULT_TIMEOUT` |
| **Variable (local)** | camelCase | `customerList`, `isValid`, `totalCount` |
| **Parameter** | camelCase | `customerId`, `userName`, `cancellationToken` |
| **Enum** | PascalCase | `OrderStatus`, `CustomerType` |
| **Enum Value** | PascalCase | `Pending`, `Approved`, `Rejected` |

### WinForms Control Naming

Controls should use **prefix + PascalCase**:

| Control | Prefix | Example |
|---------|--------|---------|
| **Button** | btn | `btnSave`, `btnCancel`, `btnSearch` |
| **Label** | lbl | `lblCustomerName`, `lblTotal`, `lblStatus` |
| **TextBox** | txt | `txtCustomerName`, `txtEmail`, `txtPhone` |
| **ComboBox** | cbx | `cbxCustomerType`, `cbxCountry` |
| **CheckBox** | chk | `chkIsActive`, `chkAgreeTerms` |
| **DataGridView** | dgv | `dgvCustomers`, `dgvOrders` |
| **GroupBox** | grp | `grpCustomerInfo`, `grpPayment` |
| **TabControl** | tab | `tabMain`, `tabDetails` |
| **TabPage** | tabp | `tabpGeneral`, `tabpAdvanced` |
| **Panel** | pnl | `pnlHeader`, `pnlFooter` |
| **ListBox** | lst | `lstCustomers`, `lstProducts` |
| **DateTimePicker** | dtp | `dtpStartDate`, `dtpEndDate` |
| **NumericUpDown** | nud | `nudQuantity`, `nudPrice` |
| **PictureBox** | pic | `picLogo`, `picProduct` |
| **ProgressBar** | prg | `prgLoading` |
| **MenuStrip** | mnu | `mnuMain` |
| **ToolStrip** | tsp | `tspMain` |
| **StatusStrip** | sts | `stsMain` |
| **ContextMenuStrip** | cms | `cmsCustomer` |

---

## Control Prefixes

### Quick Reference

```
Common Controls:
btn → Button        lbl → Label         txt → TextBox
cbx → ComboBox      chk → CheckBox      dgv → DataGridView
grp → GroupBox      tab → TabControl    pnl → Panel

Input Controls:
txt → TextBox       nud → NumericUpDown  dtp → DateTimePicker
mtx → MaskedTextBox rtx → RichTextBox    cbx → ComboBox

Lists & Grids:
lst → ListBox       clb → CheckedListBox dgv → DataGridView
lsv → ListView      trv → TreeView

Containers:
pnl → Panel         grp → GroupBox      tab → TabControl
spc → SplitContainer

Menus & Toolbars:
mnu → MenuStrip     tsp → ToolStrip     sts → StatusStrip
cms → ContextMenuStrip
```

---

## Code Style

### Indentation & Spacing

```csharp
// ✅ CORRECT: 4 spaces for indentation
public class CustomerService
{
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Customer> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid customer ID");

        return await _unitOfWork.Customers.GetByIdAsync(id);
    }
}

// ❌ WRONG: Tabs or 2 spaces
public class CustomerService
{
  private readonly IUnitOfWork _unitOfWork;

  public async Task<Customer> GetByIdAsync(int id)
  {
    return await _unitOfWork.Customers.GetByIdAsync(id);
  }
}
```

### Braces

```csharp
// ✅ CORRECT: Braces on new line (Allman style)
public class CustomerService
{
    public void DoSomething()
    {
        if (condition)
        {
            // code
        }
    }
}

// ❌ WRONG: Braces on same line (K&R style)
public class CustomerService {
    public void DoSomething() {
        if (condition) {
            // code
        }
    }
}
```

### Line Length

- **Recommended**: Max 120 characters per line
- **Hard limit**: 150 characters
- Break long lines at logical points

```csharp
// ✅ CORRECT: Break long method calls
var customer = await _unitOfWork.Customers
    .GetByEmailAsync(email, cancellationToken);

// ❌ WRONG: Too long
var customer = await _unitOfWork.Customers.GetByEmailAsync(email, includeOrders: true, includeAddress: true, cancellationToken);
```

### Using Statements

```csharp
// ✅ CORRECT: System first, then alphabetical, then custom namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectName.Models;
using ProjectName.Services;

// ❌ WRONG: Random order
using ProjectName.Models;
using System;
using Microsoft.EntityFrameworkCore;
using ProjectName.Services;
```

---

## File Organization

### File Structure

```csharp
// 1. Using statements
using System;
using System.Linq;

// 2. Namespace
namespace ProjectName.Services
{
    // 3. Class/Interface documentation
    /// <summary>
    /// Service for managing customer operations
    /// </summary>
    public class CustomerService : ICustomerService
    {
        // 4. Fields (private, readonly first)
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CustomerService> _logger;

        // 5. Constructor
        public CustomerService(IUnitOfWork unitOfWork, ILogger<CustomerService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // 6. Public properties
        public int TotalCustomers { get; private set; }

        // 7. Public methods
        public async Task<List<Customer>> GetAllAsync()
        {
            // ...
        }

        // 8. Private methods
        private void ValidateCustomer(Customer customer)
        {
            // ...
        }
    }
}
```

### Class Member Order

1. **Fields** (private, readonly first)
2. **Constructors**
3. **Properties** (public, then private)
4. **Public methods**
5. **Protected methods**
6. **Private methods**
7. **Nested types**

---

## Comments & Documentation

### XML Documentation

```csharp
/// <summary>
/// Retrieves a customer by their unique identifier
/// </summary>
/// <param name="id">The customer's ID</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>The customer if found, null otherwise</returns>
/// <exception cref="ArgumentException">Thrown when id is invalid</exception>
public async Task<Customer?> GetByIdAsync(
    int id,
    CancellationToken cancellationToken = default)
{
    // Implementation
}
```

### Inline Comments

```csharp
// ✅ GOOD: Explain WHY, not WHAT
// Use lazy loading to avoid N+1 queries
var customers = await _unitOfWork.Customers
    .AsNoTracking()
    .ToListAsync();

// ❌ BAD: Obvious comments
// Get all customers
var customers = await _unitOfWork.Customers.GetAllAsync();
```

### TODO Comments

```csharp
// TODO: Add caching for frequently accessed customers
// HACK: Temporary workaround for EF Core bug #12345
// FIXME: This breaks when customer has no orders
// NOTE: This method is called from multiple threads
```

---

## Constants vs Magic Numbers

```csharp
// ✅ CORRECT: Use named constants
private const int MAX_LOGIN_ATTEMPTS = 3;
private const string DEFAULT_COUNTRY = "Vietnam";
private const decimal TAX_RATE = 0.10m;

public bool ValidateLoginAttempts(int attempts)
{
    return attempts < MAX_LOGIN_ATTEMPTS;
}

// ❌ WRONG: Magic numbers
public bool ValidateLoginAttempts(int attempts)
{
    return attempts < 3; // What is 3?
}
```

---

## Async/Await Naming

```csharp
// ✅ CORRECT: Async methods end with "Async"
public async Task<Customer> GetCustomerAsync(int id)
public async Task SaveCustomerAsync(Customer customer)
public async Task<bool> DeleteCustomerAsync(int id)

// ❌ WRONG: Missing "Async" suffix
public async Task<Customer> GetCustomer(int id)
```

---

## Boolean Naming

```csharp
// ✅ CORRECT: Use "Is", "Has", "Can", "Should"
public bool IsActive { get; set; }
public bool HasOrders { get; set; }
public bool CanEdit { get; set; }
public bool ShouldValidate { get; set; }

// ❌ WRONG: Unclear meaning
public bool Active { get; set; }
public bool Orders { get; set; }
```

---

## Collection Naming

```csharp
// ✅ CORRECT: Plural names for collections
public List<Customer> Customers { get; set; }
public IEnumerable<Order> Orders { get; set; }
public Dictionary<int, Product> ProductsById { get; set; }

// ❌ WRONG: Singular or with "List" suffix
public List<Customer> CustomerList { get; set; }
public IEnumerable<Order> Order { get; set; }
```

---

## Summary

**Key Takeaways**:

1. ✅ **PascalCase** for classes, methods, properties
2. ✅ **camelCase** for variables, parameters
3. ✅ **_camelCase** for private fields
4. ✅ **Prefixes** for all WinForms controls (btn, txt, dgv, etc.)
5. ✅ **Async suffix** for async methods
6. ✅ **XML documentation** for all public APIs
7. ✅ **Named constants** instead of magic numbers
8. ✅ **4 spaces** for indentation
9. ✅ **Braces on new line** (Allman style)
10. ✅ **Organize** using statements (System first, alphabetical)

---

**See also**:
- [Code Style](../../docs/conventions/code-style.md) - Full code style documentation
- [Naming Conventions](../../docs/conventions/naming-conventions.md) - Detailed naming rules
- [Comments & Docstrings](../../docs/conventions/comments-docstrings.md) - Documentation standards
