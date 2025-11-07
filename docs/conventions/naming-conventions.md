# Naming Conventions

> **Quick Reference**: Standard naming rules for C# WinForms projects to ensure consistency and readability.

---

## 📋 Naming Rules Summary

| Type | Convention | Example |
|------|-----------|---------|
| **Class** | PascalCase | `CustomerService`, `MainForm` |
| **Interface** | I + PascalCase | `ICustomerService`, `IRepository` |
| **Method** | PascalCase | `LoadCustomers()`, `SaveData()` |
| **Property** | PascalCase | `FirstName`, `IsActive` |
| **Variable (local)** | camelCase | `customerList`, `isValid` |
| **Field (private)** | _camelCase | `_customerService`, `_logger` |
| **Constant** | UPPER_SNAKE_CASE | `MAX_RETRY_COUNT`, `DEFAULT_TIMEOUT` |
| **Event** | PascalCase | `CustomerSaved`, `DataLoaded` |
| **Event Handler** | Target_Event | `btnSave_Click`, `txtName_TextChanged` |
| **Control Name** | prefix + PascalCase | `btnSave`, `txtName`, `dgvCustomers` |
| **Namespace** | Company.Project.Module | `MyApp.UI.Forms`, `MyApp.Services` |

---

## 🎯 Control Naming Prefixes

```csharp
// Common Controls
btn  -> Button              // btnSave, btnCancel, btnAddCustomer
lbl  -> Label               // lblTitle, lblStatus, lblErrorMessage
txt  -> TextBox             // txtName, txtEmail, txtPhoneNumber
cbx  -> ComboBox            // cbxCountry, cbxStatus, cbxCustomerType
chk  -> CheckBox            // chkIsActive, chkAgreeToTerms
dgv  -> DataGridView        // dgvCustomers, dgvOrders, dgvProducts

// Containers
grp  -> GroupBox            // grpCustomerInfo, grpSettings
tab  -> TabControl          // tabMain, tabSettings
pnl  -> Panel               // pnlHeader, pnlFooter, pnlContent
tlp  -> TableLayoutPanel    // tlpMainLayout, tlpFormFields
flp  -> FlowLayoutPanel     // flpButtons, flpTags
spl  -> SplitContainer      // splMainContent, splLeftRight

// Menus & Toolbars
mnu  -> MenuStrip           // mnuMain, mnuContext
ctx  -> ContextMenuStrip    // ctxCustomerMenu, ctxGridMenu
sts  -> StatusStrip         // stsMain, stsProgress
tls  -> ToolStrip           // tlsMain, tlsEdit

// Other Controls
pic  -> PictureBox          // picLogo, picUserAvatar
prg  -> ProgressBar         // prgLoading, prgUpload
lsv  -> ListView            // lsvCustomers, lsvFiles
tvw  -> TreeView            // tvwFolders, tvwCategories
bds  -> BindingSource       // bdsCustomers, bdsOrders
err  -> ErrorProvider       // errValidation, errInput
```

---

## 📝 Examples

### Classes & Interfaces

```csharp
// ✅ GOOD
public class CustomerService : ICustomerService { }
public class OrderRepository : IOrderRepository { }
public class MainForm : Form { }

// ❌ BAD
public class customer_service { }  // Wrong case
public class Customers { }         // Too generic
public class CS { }                // Abbreviation unclear
```

### Methods & Properties

```csharp
// ✅ GOOD
public async Task<List<Customer>> GetAllCustomersAsync() { }
public bool ValidateEmail(string email) { }
public string FirstName { get; set; }
public bool IsActive { get; set; }

// ❌ BAD
public async Task<List<Customer>> get_customers() { }  // Wrong case
public bool Val(string s) { }                          // Unclear abbreviation
public string first_name { get; set; }                 // Wrong case
```

### Variables & Fields

```csharp
// ✅ GOOD - Private fields
private readonly ICustomerService _customerService;
private readonly ILogger<MainForm> _logger;
private bool _isLoading;

// ✅ GOOD - Local variables
var customerList = await _service.GetAllAsync();
int totalCount = customers.Count;
bool isValid = ValidateForm();

// ❌ BAD
private ICustomerService CustomerService;  // Should be _camelCase
var list = GetData();                      // Too generic
bool b = true;                             // Single letter
```

### Constants

```csharp
// ✅ GOOD
public const int MAX_RETRY_COUNT = 3;
public const string DEFAULT_CONNECTION_STRING = "...";
private const int PAGE_SIZE = 50;

// ❌ BAD
public const int MaxRetryCount = 3;     // Should be UPPER_SNAKE_CASE
public const int max = 3;               // Too short/generic
```

### Events & Event Handlers

```csharp
// ✅ GOOD - Events
public event EventHandler<CustomerEventArgs> CustomerSaved;
public event EventHandler DataLoaded;

// ✅ GOOD - Event Handlers
private void btnSave_Click(object sender, EventArgs e) { }
private void txtName_TextChanged(object sender, EventArgs e) { }
private void dgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e) { }

// ❌ BAD
public event EventHandler Event1;       // Generic name
private void Button1_Click() { }        // Default designer name
private void SaveClickHandler() { }     // Doesn't follow convention
```

### Control Names

```csharp
// ✅ GOOD
private Button btnSave;
private TextBox txtCustomerName;
private DataGridView dgvCustomers;
private Label lblTotalRecords;
private ComboBox cbxCountry;

// ❌ BAD
private Button button1;           // Default designer name
private TextBox customerName;     // Missing prefix
private DataGridView DGVCustomers; // Wrong case (prefix should be lowercase)
```

### Namespaces

```csharp
// ✅ GOOD
namespace MyCompany.MyApp.UI.Forms { }
namespace MyCompany.MyApp.Services { }
namespace MyCompany.MyApp.Data.Repositories { }

// ❌ BAD
namespace myapp.forms { }          // Wrong case
namespace Forms { }                // Too generic
namespace MyCompany_MyApp { }      // Use dots, not underscores
```

---

## ✅ Best Practices

### DO:
✅ Use descriptive, meaningful names
✅ Be consistent with casing conventions
✅ Use full words (avoid abbreviations)
✅ Use prefixes for controls consistently
✅ Name event handlers with pattern: `controlName_EventName`
✅ Use singular for entity names: `Customer`, not `Customers`
✅ Use plural for collections: `List<Customer> customers`

### DON'T:
❌ Don't use single-letter variable names (except `i`, `j` for loops)
❌ Don't use Hungarian notation (e.g., `strName`, `intAge`)
❌ Don't abbreviate excessively (`Cust` instead of `Customer`)
❌ Don't use underscores in method/property names (except private fields)
❌ Don't leave default designer names (`button1`, `textBox1`)
❌ Don't use generic names (`data`, `info`, `temp`)

---

## 🎯 Quick Checklist

Before committing code, verify:

- [ ] All classes use PascalCase
- [ ] All methods use PascalCase
- [ ] All variables use camelCase
- [ ] All private fields use _camelCase
- [ ] All constants use UPPER_SNAKE_CASE
- [ ] All controls have prefixes (btn, txt, dgv, etc.)
- [ ] All controls renamed from designer defaults
- [ ] No abbreviations (unless very common: Id, UI, URL)
- [ ] Names are descriptive and meaningful

---

## 🔗 Related Topics

- [Code Style](code-style.md) - Formatting and style guidelines
- [Comments & Docstrings](comments-docstrings.md) - Documentation standards

---

## 📚 References

- [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [.NET Framework Design Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines)

---

**Last Updated**: 2025-11-07
