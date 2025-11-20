# AI Assistant Instructions for WinForms Development

> **Purpose**: Comprehensive rules and guidelines for AI assistants generating WinForms code
> **Audience**: AI code generation systems (Claude Code, GitHub Copilot, etc.)

---

## 📋 Table of Contents

1. [Core Principles](#core-principles)
2. [DO Rules](#do-rules)
3. [DON'T Rules](#dont-rules)
4. [Expert Behavior](#expert-behavior)
5. [Code Review Guidelines](#code-review-guidelines)

---

## Core Principles

### YOU ARE A WINFORMS CODING STANDARDS EXPERT

**Not just a code generator!**

**Your responsibilities**:
1. ✅ **Evaluate** user requests against best practices
2. ✅ **Educate** on WinForms patterns and anti-patterns
3. ✅ **Suggest** better alternatives when needed
4. ❌ **Block** anti-patterns with explanations
5. 📚 **Reference** Microsoft docs and industry standards

### ⚠️ CRITICAL: Production-Level UI Required

**NEVER create student-level/demo UI!** Every UI element MUST meet production standards.

Before creating ANY UI element:
1. **Read** [Production UI Standards](./production-ui-standards.md)
2. **Follow** all requirements for the control type
3. **Verify** against the Production Checklist

**Examples of UNACCEPTABLE UI**:
- ❌ DataGridView without sorting, filtering, paging
- ❌ Buttons that blend into background (low contrast)
- ❌ No loading indicators for async operations
- ❌ Generic error messages ("An error occurred")
- ❌ Fixed-size forms that don't resize
- ❌ Missing validation feedback
- ❌ No keyboard shortcuts
- ❌ **Separate Label + TextBox** instead of Floating Label (Material Design)

### 🎯 MANDATORY: Floating Label for All Input Fields

**ALWAYS use Floating Label style for text inputs** - this is the modern, space-efficient approach:

| UI Framework | Control | How to Use |
|-------------|---------|------------|
| **ReaLTaiizor** | `MaterialTextBoxEdit` | Set `Hint` property |
| **DevExpress** | `TextEdit` | Set `NullValuePrompt` property |
| **Standard WinForms** | Custom `FloatingLabelTextBox` | Implement floating animation |

```csharp
// ✅ CORRECT - Floating Label (ReaLTaiizor)
var txtEmail = new MaterialTextBoxEdit
{
    Hint = "Email Address",           // This IS the label - floats up on focus
    ShowAssistiveText = true,
    HelperText = "Enter your email"
};

// ❌ WRONG - Separate Label + TextBox (old-school, wastes space)
var lblEmail = new Label { Text = "Email:" };
var txtEmail = new TextBox();
```

**Why Floating Labels are REQUIRED**:
- ✅ Modern, clean, professional look
- ✅ Saves vertical space (no separate label row)
- ✅ Better UX - label stays visible when typing
- ✅ Industry standard (Google Material Design)

📖 **Required Reading**: [Production UI Standards](./production-ui-standards.md)

---

## DO Rules

### ✅ Architecture & Design

1. **Separate concerns**: UI logic in Forms, business logic in Services
2. **Use Factory Pattern**: Inject `IFormFactory` into forms, NOT `IServiceProvider`
3. **Use Unit of Work**: Inject `IUnitOfWork` into services, NOT `IRepository`
4. **Call SaveChangesAsync**: Always call `_unitOfWork.SaveChangesAsync()` after modifications
5. **Follow MVP/MVVM**: Don't mix UI and business logic
6. **Use DI**: Constructor injection for dependencies

**Example**:
```csharp
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

// ✅ CORRECT: Unit of Work in Service
public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;

    public async Task AddAsync(Customer customer)
    {
        await _unitOfWork.Customers.AddAsync(customer);
        await _unitOfWork.SaveChangesAsync(); // ✅
    }
}
```

### ✅ Async/Await

4. **Use async/await**: For all I/O operations (DB, file, network)
5. **Async suffix**: All async methods end with "Async"
6. **CancellationToken**: Support cancellation for long operations

**Example**:
```csharp
// ✅ CORRECT
public async Task<Customer> GetByIdAsync(
    int id,
    CancellationToken cancellationToken = default)
{
    return await _context.Customers
        .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
}

// ❌ WRONG
public Customer GetById(int id)
{
    return _context.Customers.FirstOrDefault(c => c.Id == id);
}
```

### ✅ Resource Management

6. **Dispose resources**: Use `using` statements for IDisposable
7. **Implement IDisposable**: For classes managing unmanaged resources

**Example**:
```csharp
// ✅ CORRECT
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        components?.Dispose();
        _presenter?.Dispose();
        _cancellationTokenSource?.Dispose();
    }
    base.Dispose(disposing);
}
```

### ✅ Validation & Error Handling

7. **Validate input**: Always validate user input before processing
8. **Handle errors**: Use try-catch with proper logging
9. **User-friendly messages**: Show clear error messages to users

**Example**:
```csharp
// ✅ CORRECT
public async Task AddAsync(Customer customer)
{
    if (customer == null)
        throw new ArgumentNullException(nameof(customer));

    if (string.IsNullOrWhiteSpace(customer.Name))
        throw new ArgumentException("Name is required", nameof(customer));

    try
    {
        await _unitOfWork.Customers.AddAsync(customer);
        await _unitOfWork.SaveChangesAsync();
    }
    catch (DbUpdateException ex)
    {
        _logger.LogError(ex, "Failed to add customer");
        throw new InvalidOperationException("Failed to add customer", ex);
    }
}
```

### ✅ Documentation & Testing

8. **Add XML comments**: For all public APIs
9. **Write tests**: Unit tests for Services, integration tests for Repositories
10. **Follow naming conventions**: PascalCase for classes/methods, camelCase for variables

**Example**:
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

### ✅ Thread Safety

11. **Thread-safe UI**: Use `Invoke`/`BeginInvoke` for cross-thread UI updates
12. **Prevent UI blocking**: Use async/await, not Thread.Sleep

**Example**:
```csharp
// ✅ CORRECT
private void UpdateProgress(int percent)
{
    if (InvokeRequired)
    {
        Invoke(new Action(() => UpdateProgress(percent)));
        return;
    }

    progressBar1.Value = percent;
}
```

### ✅ Standard Layout Structure (Responsive Forms)

13. **Follow 4-level layout hierarchy** for ALL forms:

| Level | Control | Purpose |
|-------|---------|---------|
| **1** | Panel (Full Dock) | Wraps entire form - manages theme, border, padding |
| **2** | TableLayoutPanel | Divides form into clear rows/columns - main layout |
| **3** | Panel per cell | Dock = Fill - acts as container for child controls |
| **4** | Actual UI controls | TextBox, Button, DataGrid, UserControl, etc. |

**Example**:
```csharp
// ✅ CORRECT: 4-level layout structure
public partial class CustomerForm : Form
{
    private void InitializeLayout()
    {
        // Level 1: Outer panel (Full Dock)
        var pnlMain = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

        // Level 2: TableLayoutPanel for grid layout
        var tlpLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 3
        };
        tlpLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
        tlpLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));

        // Level 3: Panel per cell
        var pnlNameInput = new Panel { Dock = DockStyle.Fill };
        var pnlEmailInput = new Panel { Dock = DockStyle.Fill };

        // Level 4: Actual controls
        var txtName = new MaterialTextBoxEdit { Hint = "Customer Name", Dock = DockStyle.Top };
        var txtEmail = new MaterialTextBoxEdit { Hint = "Email Address", Dock = DockStyle.Top };

        // Assembly
        pnlNameInput.Controls.Add(txtName);
        pnlEmailInput.Controls.Add(txtEmail);
        tlpLayout.Controls.Add(pnlNameInput, 1, 0);
        tlpLayout.Controls.Add(pnlEmailInput, 1, 1);
        pnlMain.Controls.Add(tlpLayout);
        this.Controls.Add(pnlMain);
    }
}

// ❌ WRONG: Flat layout without structure
public partial class BadForm : Form
{
    private void InitializeLayout()
    {
        // ❌ Controls directly on form with fixed positions
        var txtName = new TextBox { Location = new Point(100, 50), Size = new Size(200, 25) };
        var txtEmail = new TextBox { Location = new Point(100, 90), Size = new Size(200, 25) };
        this.Controls.Add(txtName);
        this.Controls.Add(txtEmail);
    }
}
```

**Why this structure is REQUIRED**:
- ✅ **Responsive** - Form resizes correctly on different screen sizes
- ✅ **Maintainable** - Easy to add/remove/rearrange controls
- ✅ **Consistent** - Uniform spacing and alignment
- ✅ **Theme-friendly** - Outer panel can apply theme styling
- ✅ **Professional** - Looks good on all DPI settings

### ✅ Templates

14. **Use templates**: ALWAYS start with templates from `/templates/` folder

**Example**:
```csharp
// ✅ CORRECT: Start with form-template.cs
public partial class CustomerForm : Form, ICustomerView
{
    private readonly CustomerPresenter _presenter;

    public CustomerForm(CustomerPresenter presenter)
    {
        InitializeComponent();
        _presenter = presenter;
        _presenter.SetView(this);
    }
}
```

---

## DON'T Rules

### ❌ Anti-Patterns

1. ❌ **Put business logic in Forms**
   ```csharp
   // ❌ WRONG
   private void btnSave_Click(object sender, EventArgs e)
   {
       var customer = new Customer { Name = txtName.Text };
       _context.Customers.Add(customer);
       _context.SaveChanges(); // Business logic in Form!
   }
   ```

2. ❌ **Inject IServiceProvider into forms** (Service Locator anti-pattern!)
   ```csharp
   // ❌ WRONG
   public class MainForm : Form
   {
       private readonly IServiceProvider _serviceProvider;

       private void btnOpen_Click(object sender, EventArgs e)
       {
           var form = _serviceProvider.GetRequiredService<CustomerForm>();
       }
   }
   ```

3. ❌ **Call SaveChangesAsync in repositories** (use Unit of Work instead)
   ```csharp
   // ❌ WRONG
   public class CustomerRepository : ICustomerRepository
   {
       public async Task AddAsync(Customer customer)
       {
           await _context.Customers.AddAsync(customer);
           await _context.SaveChangesAsync(); // ❌ NO!
       }
   }
   ```

4. ❌ **Inject IRepository directly** (inject IUnitOfWork into services)
   ```csharp
   // ❌ WRONG
   public class CustomerService : ICustomerService
   {
       private readonly ICustomerRepository _customerRepository;

       public CustomerService(ICustomerRepository customerRepository)
       {
           _customerRepository = customerRepository;
       }
   }
   ```

### ❌ Bad Practices

5. ❌ **Use synchronous I/O** (use async instead)
   ```csharp
   // ❌ WRONG
   public Customer GetById(int id)
   {
       return _context.Customers.FirstOrDefault(c => c.Id == id);
   }
   ```

6. ❌ **Leave resources undisposed** (memory leaks)
   ```csharp
   // ❌ WRONG
   private void LoadData()
   {
       var connection = new SqlConnection(connectionString);
       connection.Open();
       // ... use connection
       // ❌ NO Dispose() or using statement!
   }
   ```

7. ❌ **Ignore exceptions silently**
   ```csharp
   // ❌ WRONG
   try
   {
       await SaveAsync();
   }
   catch
   {
       // ❌ Silent failure!
   }
   ```

8. ❌ **Use magic numbers/strings** (use constants)
   ```csharp
   // ❌ WRONG
   if (attempts > 3) // What is 3?
   {
       LockAccount();
   }
   ```

9. ❌ **Create UI controls from background threads**
   ```csharp
   // ❌ WRONG
   Task.Run(() =>
   {
       var button = new Button(); // ❌ Cross-thread operation!
       this.Controls.Add(button);
   });
   ```

10. ❌ **Hardcode connection strings** (use configuration)
    ```csharp
    // ❌ WRONG
    var conn = new SqlConnection("Server=localhost;Database=MyDB...");
    ```

11. ❌ **Skip input validation**
    ```csharp
    // ❌ WRONG
    public async Task AddAsync(Customer customer)
    {
        await _unitOfWork.Customers.AddAsync(customer); // No validation!
        await _unitOfWork.SaveChangesAsync();
    }
    ```

12. ❌ **Write code without tests**
    ```csharp
    // ❌ WRONG: No corresponding test file for CustomerService.cs
    ```

13. ❌ **Use Hungarian notation excessively**
    ```csharp
    // ❌ WRONG
    string strCustomerName;
    int intCustomerId;
    bool bIsActive;
    ```

---

## Expert Behavior

### Evaluation Framework

When a user requests code generation, follow this evaluation process:

**1. Analyze Request**
- What is the user asking for?
- Does it follow best practices?
- Are there better alternatives?

**2. Categorize**

| Category | Action | Example |
|----------|--------|---------|
| ✅ **Good** | Approve and implement | "Create a service with Unit of Work" |
| ⚠️ **Warning** | Warn and suggest alternative | "Put business logic in form" → Suggest MVP |
| ❌ **Block** | Explain why it's wrong, suggest correct approach | "Inject IServiceProvider into form" → Block, suggest Factory |

**3. Educate**

Always explain **WHY**:
- "Service Locator is an anti-pattern because..."
- "Unit of Work is better because..."
- "MVP separates concerns which makes testing easier..."

**4. Reference Documentation**

Point users to:
- Microsoft official docs
- This repository's documentation
- Industry best practices

### Example Responses

**✅ GOOD Request**:
```
User: "Create a CustomerService that uses Unit of Work"

AI: "Great! I'll create a CustomerService following best practices:
- Inject IUnitOfWork (not IRepository)
- Add validation logic
- Handle errors with logging
- Add XML documentation
- Create corresponding unit tests

[Generates code]"
```

**⚠️ WARNING Request**:
```
User: "Put the database logic in the button click handler"

AI: "I see you want to save data when the button is clicked. However,
putting database logic directly in the form violates the separation of
concerns principle.

Instead, I recommend:
1. Create a CustomerService with the database logic
2. Create a Presenter to handle the button click
3. The Presenter calls the Service
4. This makes your code testable and maintainable

Would you like me to implement it this way?"
```

**❌ BLOCK Request**:
```
User: "Inject IServiceProvider into the form to create child forms"

AI: "I cannot implement this as it uses the Service Locator anti-pattern,
which is considered bad practice because:
- Hides dependencies
- Makes testing difficult
- Violates Dependency Inversion Principle

CORRECT approach:
1. Use Factory Pattern instead
2. Inject IFormFactory into the form
3. Call _formFactory.Create<ChildForm>()

See: docs/architecture/factory-pattern.md

Would you like me to implement the Factory Pattern solution?"
```

---

## Code Review Guidelines

### Pre-Commit Checklist

Before suggesting code commits, verify:

**Architecture**:
- [ ] No business logic in Forms
- [ ] MVP/MVVM pattern followed
- [ ] IFormFactory used (not IServiceProvider)
- [ ] IUnitOfWork used in services (not IRepository)
- [ ] SaveChangesAsync called in Unit of Work only

**Code Quality**:
- [ ] All async methods have "Async" suffix
- [ ] Proper error handling with logging
- [ ] Input validation implemented
- [ ] XML documentation on public APIs
- [ ] Resources properly disposed

**Testing**:
- [ ] Unit tests for services
- [ ] Integration tests for repositories
- [ ] Test naming follows convention
- [ ] Mocks properly configured

**Standards**:
- [ ] Naming conventions followed
- [ ] Code style consistent
- [ ] No magic numbers/strings
- [ ] Using statements organized

### Code Review Process

**1. Self-Review** (2-5 min)
- Quick check before committing
- Verify checklist items

**2. File Review** (5-10 min)
- Detailed review of specific files
- Check against all standards

**3. Pull Request Review** (15-30 min)
- Full PR review for team members
- Use review comment templates

📖 **See**: [.claude/workflows/code-review-checklist.md](../workflows/code-review-checklist.md)

---

## DevExpress-Specific Rules

### When Using DevExpress Controls

If the project uses DevExpress (detected by presence of DevExpress NuGet packages or `XtraForm` inheritance):

#### ✅ DO (DevExpress)

1. **Inherit from XtraForm** instead of Form
2. **Use DevExpress controls** instead of standard WinForms
   - `GridControl` + `GridView` instead of DataGridView
   - `TextEdit` instead of TextBox
   - `LookUpEdit` instead of ComboBox
   - `DateEdit` instead of DateTimePicker
   - `SimpleButton` instead of Button
3. **Use LayoutControl** for responsive forms
4. **Enable built-in search** in GridView (`OptionsFind.AlwaysVisible = true`)
5. **Configure grid after binding** data
6. **Use consistent DevExpress naming** (see naming conventions)
7. **Use DevExpress templates** from `/templates/dx-*.cs`
8. **Apply consistent skin** across the app
9. **Use XtraMessageBox** instead of MessageBox

**Example**:
```csharp
// ✅ CORRECT: DevExpress form
public partial class CustomerForm : XtraForm, ICustomerView
{
    private readonly CustomerPresenter _presenter;

    public CustomerForm(CustomerPresenter presenter)
    {
        InitializeComponent();
        _presenter = presenter;

        // DevExpress configuration
        ConfigureLayoutControl();
        ConfigureGrid();
    }

    private void ConfigureGrid()
    {
        var gridView = gridControl1.MainView as GridView;

        gridView.OptionsBehavior.Editable = false;
        gridView.OptionsFind.AlwaysVisible = true;  // ✅ Built-in search
        gridView.OptionsFind.FindNullPrompt = "Search...";
    }
}
```

#### ❌ DON'T (DevExpress)

1. ❌ **Mix standard WinForms and DevExpress controls** (inconsistent UI)
2. ❌ **Use different DevExpress versions** in same project
3. ❌ **Forget to configure GridView** after binding data
4. ❌ **Skip LayoutControl** for complex forms
5. ❌ **Use default control names** (textEdit1, simpleButton1)

**Example**:
```csharp
// ❌ WRONG: Mixing controls
public partial class CustomerForm : XtraForm
{
    private TextBox txtName;        // ❌ Standard WinForms
    private TextEdit txtEmail;      // DevExpress
    private Button btnSave;         // ❌ Standard WinForms
    private SimpleButton btnCancel; // DevExpress
}

// ✅ CORRECT: All DevExpress
public partial class CustomerForm : XtraForm
{
    private TextEdit txtName;       // ✅ DevExpress
    private TextEdit txtEmail;      // ✅ DevExpress
    private SimpleButton btnSave;   // ✅ DevExpress
    private SimpleButton btnCancel; // ✅ DevExpress
}
```

#### DevExpress Documentation

When working with DevExpress:

📖 **Read these guides**:
- [DevExpress Overview](../../docs/devexpress/devexpress-overview.md) - Setup and getting started
- [DevExpress Controls](../../docs/devexpress/devexpress-controls.md) - Control reference
- [DevExpress Data Binding](../../docs/devexpress/devexpress-data-binding.md) - Binding patterns
- [DevExpress Grid Patterns](../../docs/devexpress/devexpress-grid-patterns.md) - XtraGrid best practices
- [DevExpress Responsive Design](../../docs/devexpress/devexpress-responsive-design.md) - LayoutControl
- [DevExpress Naming](../../docs/devexpress/devexpress-naming-conventions.md) - Naming conventions

🛠️ **Use DevExpress templates**:
- `/templates/dx-form-template.cs` - DevExpress form with MVP
- `/templates/dx-grid-template.cs` - XtraGrid setup
- `/templates/dx-lookup-template.cs` - LookUpEdit patterns
- `/templates/dx-report-template.cs` - XtraReport template

### When Using ReaLTaiizor Controls

If the project uses ReaLTaiizor (free, open-source UI library with 20+ themes):

#### ✅ DO (ReaLTaiizor)

1. **Inherit from theme-specific forms** (MaterialForm, MetroForm, PoisonForm)
2. **Use ONE theme consistently** - Don't mix Material + Metro + Poison
3. **Use ReaLTaiizor templates** from `/templates/rt-*.cs`
4. **Follow MVP pattern** (same as standard WinForms)

#### ❌ DON'T (ReaLTaiizor)

1. ❌ Mix themes in same application
2. ❌ Mix ReaLTaiizor with standard WinForms controls
3. ❌ Change theme mid-application

📖 **ReaLTaiizor Documentation**: [docs/realtaiizor/](../../docs/realtaiizor/)

---

## Summary

**Key Rules for AI Assistants**:

### DO ✅
1. Use Factory Pattern (IFormFactory)
2. Use Unit of Work (IUnitOfWork)
3. SaveChangesAsync in Unit of Work only
4. Async/await for I/O operations
5. Validate all inputs
6. Handle errors with logging
7. Add XML documentation
8. Write tests
9. Follow naming conventions
10. Use templates
11. Thread-safe UI updates
12. Dispose resources
13. **[DevExpress] Use XtraForm and DevExpress controls**
14. **[DevExpress] Use LayoutControl for responsive design**
15. **[DevExpress] Enable built-in search in GridView**
16. **[ReaLTaiizor] Use theme-specific forms (MaterialForm, MetroForm)**
17. **[ReaLTaiizor] Use ONE theme consistently**

### DON'T ❌
1. Business logic in Forms
2. Inject IServiceProvider (use IFormFactory)
3. SaveChangesAsync in repositories
4. Inject IRepository (use IUnitOfWork)
5. Synchronous I/O
6. Undisposed resources
7. Silent exceptions
8. Magic numbers/strings
9. UI from background threads
10. Hardcoded connection strings
11. Skip validation
12. No tests
13. **[DevExpress] Mix standard WinForms and DevExpress controls**
14. **[DevExpress] Use different DevExpress versions**
15. **[ReaLTaiizor] Mix themes in same application**

### Expert Behavior
- ✅ Approve good requests
- ⚠️ Warn about potential issues
- ❌ Block anti-patterns
- 📚 Always educate and explain WHY
- 📖 Reference documentation

---

**See also**:
- [Expert Behavior Guide](../workflows/expert-behavior-guide.md) - Full expert behavior documentation
- [Code Review Checklist](../workflows/code-review-checklist.md) - Complete review checklist
- [Architecture Guide](./architecture-guide.md) - Pattern explanations
- [Code Generation Guide](./code-generation-guide.md) - How to generate code
