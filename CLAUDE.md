# C# WinForms Coding Standards - Claude Code Guide

> **Project**: C# WinForms Coding Standards and Best Practices Documentation
> **Purpose**: Guidelines for building maintainable, scalable WinForms applications

---

## 📊 Project Status

**Repository Completion**: 34% (16/47 files)
**Last Updated**: 2025-11-07
**Version**: 2.0 (Modular structure)

### What's Complete ✅
- ✅ Architecture documentation (MVP, MVVM, DI, project structure)
- ✅ Coding conventions (naming, style, comments)
- ✅ Templates (form, service, repository, test)
- ✅ Configuration files (.gitignore, .editorconfig)
- ✅ Async/await and error handling best practices

### What's In Progress 🟡
- 🟡 Best practices documentation (2/8 files complete)
- 🟡 Code examples (1/4 files complete)

### What's Missing ⚠️
- ⚠️ UI/UX documentation (0/6 files) - **CRITICAL for WinForms!**
- ⚠️ Testing documentation (0/5 files) - **Essential for quality!**
- ⚠️ Data access documentation (0/3 files)
- ⚠️ Advanced topics (0/5 files)
- ⚠️ Working example project (planned)

📋 **Full details**: [MISSING_DOCS.md](MISSING_DOCS.md)
📝 **Review report**: [CODE_REVIEW_REPORT.md](CODE_REVIEW_REPORT.md)

---

## 📦 Tech Stack

- **.NET**: 8.0 (recommended) / .NET Framework 4.8
- **Language**: C# 12.0 / C# 10.0
- **UI Framework**: Windows Forms
- **ORM**: Entity Framework Core 8.0
- **Testing**: xUnit / NUnit
- **DI Container**: Microsoft.Extensions.DependencyInjection
- **Logging**: Serilog / NLog

---

## 🏗️ Project Structure

Standard WinForms project structure:

```
/ProjectName
    ├── /Forms              # UI Layer (minimal logic)
    ├── /Controls           # Custom user controls
    ├── /Models             # Business/data models
    ├── /Services           # Business logic
    ├── /Repositories       # Data access layer
    ├── /Utils              # Helpers, extensions
    ├── /Resources          # Icons, strings, localization
    ├── Program.cs
    └── App.config
```

**Key Principles**:
- ✅ Forms contain **UI handling only**, no business logic
- ✅ Business logic lives in **Services**
- ✅ Use **Dependency Injection** for loose coupling
- ✅ Follow **MVP** or **MVVM** pattern for larger apps

📖 **Detailed docs**: [docs/architecture/project-structure.md](docs/architecture/project-structure.md)

---

## 🎯 Coding Standards Quick Reference

### Architecture
- **Pattern**: MVP (recommended) or MVVM (.NET 8+)
- **Separation**: UI → Presenter/ViewModel → Service → Repository → Database
- 📖 [MVP Pattern](docs/architecture/mvp-pattern.md) | [MVVM Pattern](docs/architecture/mvvm-pattern.md)

### Naming Conventions
| Type | Convention | Example |
|------|-----------|---------|
| Class | PascalCase | `CustomerService`, `MainForm` |
| Method | PascalCase | `LoadCustomers()`, `SaveData()` |
| Variable | camelCase | `customerList`, `isActive` |
| Constant | UPPER_SNAKE_CASE | `MAX_RETRY_COUNT` |
| Control | prefix + PascalCase | `btnSave`, `txtName`, `dgvCustomers` |

📖 **Full conventions**: [docs/conventions/naming-conventions.md](docs/conventions/naming-conventions.md)

### Control Prefixes
```
btn → Button        lbl → Label         txt → TextBox
cbx → ComboBox      chk → CheckBox      dgv → DataGridView
grp → GroupBox      tab → TabControl    pnl → Panel
```

---

## ⚙️ Common Commands

```bash
# Build project
dotnet build

# Run tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true

# Run specific test
dotnet test --filter "FullyQualifiedName~ServiceTests"

# Clean and rebuild
dotnet clean && dotnet build
```

---

## 🤖 AI Assistant Rules (IMPORTANT!)

When writing code, **ALWAYS follow these rules**:

### ✅ DO:
1. **Separate concerns**: UI logic in Forms, business logic in Services
2. **Use async/await**: For all I/O operations (DB, file, network)
3. **Dispose resources**: Use `using` statements for IDisposable
4. **Validate input**: Always validate user input before processing
5. **Handle errors**: Use try-catch with proper logging
6. **Add XML comments**: For all public APIs
7. **Follow MVP/MVVM**: Don't mix UI and business logic
8. **Use DI**: Constructor injection for dependencies
9. **Write tests**: Unit tests for Services, integration tests for Repositories
10. **Thread-safe UI**: Use `Invoke`/`BeginInvoke` for cross-thread UI updates

### ❌ DON'T:
1. ❌ Put business logic in Forms
2. ❌ Use synchronous I/O (use async instead)
3. ❌ Leave resources undisposed (memory leaks)
4. ❌ Ignore exceptions silently
5. ❌ Use magic numbers/strings (use constants)
6. ❌ Create UI controls from background threads
7. ❌ Hardcode connection strings (use configuration)
8. ❌ Skip input validation
9. ❌ Write code without tests
10. ❌ Use Hungarian notation excessively

---

## 🧠 Claude Code Context Loading

When starting a new coding task, follow this context loading strategy:

### 1. **This File is Auto-Loaded**
CLAUDE.md is automatically loaded at the start of every session. You already have the core guidelines.

### 2. **Load Context Based on Task Type**

| Task Type | Files to Read |
|-----------|--------------|
| **Creating a new Form** | `templates/form-template.cs`, `docs/architecture/mvp-pattern.md` |
| **Creating a Service** | `templates/service-template.cs`, `docs/best-practices/async-await.md` |
| **Creating a Repository** | `templates/repository-template.cs`, *(data-access docs when available)* |
| **Writing Tests** | `templates/test-template.cs`, *(testing docs when available)* |
| **Data Binding** | *(UI/UX docs when available - see MISSING_DOCS.md)* |
| **Form Communication** | *(UI/UX docs when available - see MISSING_DOCS.md)* |
| **Error Handling** | `docs/best-practices/error-handling.md` |
| **General Questions** | `CODE_REVIEW_REPORT.md` for repository overview |

### 3. **Documentation Status Check**

⚠️ **IMPORTANT**: Many referenced docs don't exist yet (see [MISSING_DOCS.md](MISSING_DOCS.md))

**If a doc is missing**:
- Use the corresponding template file as reference
- Follow patterns from existing complete documentation
- Refer to the DO/DON'T rules above
- Ask user if clarification is needed

### 4. **Always Use Templates**

Templates are **production-ready** and follow all standards:
- `/templates/form-template.cs` - MVP pattern form
- `/templates/service-template.cs` - Business logic service
- `/templates/repository-template.cs` - Data access repository
- `/templates/test-template.cs` - Unit test structure

**Never generate code from scratch** - always start with templates!

---

## 🎨 Code Generation Patterns

### Pattern 1: Creating a New Form

**User Request**: "Create a CustomerForm to manage customers"

**Your Workflow**:
```
1. Read: templates/form-template.cs
2. Read: docs/architecture/mvp-pattern.md (if complex logic)
3. Generate:
   - CustomerForm.cs (Form implementation)
   - ICustomerView.cs (View interface)
   - CustomerPresenter.cs (Presentation logic)
4. Ensure:
   - MVP pattern properly implemented
   - Async/await for data loading
   - Proper error handling
   - Input validation
   - Resource disposal
5. Offer: Generate unit tests for the presenter
```

### Pattern 2: Creating a Service

**User Request**: "Create a service to handle customer operations"

**Your Workflow**:
```
1. Read: templates/service-template.cs
2. Generate: CustomerService.cs with:
   - Constructor injection for dependencies (ICustomerRepository, ILogger)
   - Async methods (LoadCustomersAsync, SaveCustomerAsync, etc.)
   - Input validation with ArgumentNullException
   - Try-catch with logging
   - XML documentation comments
3. Generate: ICustomerService interface
4. Offer: Generate unit tests with mocked dependencies
```

### Pattern 3: Creating a Repository

**User Request**: "Create a repository for customers"

**Your Workflow**:
```
1. Read: templates/repository-template.cs
2. Generate:
   - CustomerRepository.cs implementing ICustomerRepository
   - Use EF Core DbContext
   - Async CRUD operations
   - Proper using/disposal
   - Error handling
3. Note: Remind user about DbContext configuration
4. Offer: Generate integration tests
```

### Pattern 4: Writing Tests

**User Request**: "Write tests for CustomerService"

**Your Workflow**:
```
1. Read: templates/test-template.cs
2. Read: The service class to test
3. Generate: CustomerServiceTests.cs with:
   - xUnit [Fact] attributes
   - Moq for mocking dependencies
   - Arrange-Act-Assert pattern
   - Test naming: MethodName_Scenario_ExpectedResult
   - Tests for success paths, error cases, edge cases
4. Run: dotnet test to verify
5. Report: Test results and coverage
```

### Pattern 5: Code Review

**User Request**: "Review this form code"

**Your Workflow**:
```
1. Read: The code file
2. Check against:
   - Pre-Commit Checklist (see below)
   - DO/DON'T rules (see above)
   - Relevant templates
   - Architecture patterns (MVP/MVVM)
3. Identify:
   - ✅ What's good
   - ⚠️ What needs improvement
   - ❌ What's wrong and must be fixed
4. Suggest: Specific code improvements
5. Offer: Refactor the code following standards
```

---

## 🎯 Code Generation Rules Summary

### When generating Forms:
1. ✅ Start with `form-template.cs`
2. ✅ Implement MVP pattern (Form + IView + Presenter)
3. ✅ Async event handlers for data operations
4. ✅ Try-catch with user-friendly error messages
5. ✅ Dispose resources in Dispose() method
6. ✅ Set TabIndex for proper keyboard navigation
7. ✅ Use meaningful control names (not button1, textBox1)

### When generating Services:
1. ✅ Start with `service-template.cs`
2. ✅ Constructor injection for all dependencies
3. ✅ Validate all inputs (ArgumentNullException, ArgumentException)
4. ✅ Async methods with proper cancellation token support
5. ✅ Log all operations (info, errors, warnings)
6. ✅ Wrap exceptions with meaningful messages
7. ✅ XML documentation on all public methods

### When generating Repositories:
1. ✅ Start with `repository-template.cs`
2. ✅ Implement generic repository pattern
3. ✅ Use EF Core async methods (ToListAsync, FirstOrDefaultAsync, etc.)
4. ✅ Proper disposal of DbContext
5. ✅ Include soft-delete support if applicable
6. ✅ Error handling with data access exceptions

### When generating Tests:
1. ✅ Start with `test-template.cs`
2. ✅ One test class per class under test
3. ✅ Use Moq for mocking dependencies
4. ✅ Arrange-Act-Assert structure
5. ✅ Test naming: `MethodName_Scenario_ExpectedResult`
6. ✅ Test both success and failure scenarios
7. ✅ Use Assert.Throws for exception testing

---

## 📚 Documentation Structure

### Core Documentation
- **[Overview](docs/00-overview.md)** - Full documentation index

### Architecture & Design
- [Project Structure](docs/architecture/project-structure.md)
- [MVP Pattern](docs/architecture/mvp-pattern.md)
- [MVVM Pattern](docs/architecture/mvvm-pattern.md)
- [Dependency Injection](docs/architecture/dependency-injection.md)

### Conventions
- [Naming Conventions](docs/conventions/naming-conventions.md)
- [Code Style](docs/conventions/code-style.md)
- [Comments & Docstrings](docs/conventions/comments-docstrings.md)

### UI & UX
- [Responsive Design](docs/ui-ux/responsive-design.md)
- [Form Communication](docs/ui-ux/form-communication.md)
- [Data Binding](docs/ui-ux/data-binding.md)
- [Input Validation](docs/ui-ux/input-validation.md)
- [DataGridView Best Practices](docs/ui-ux/datagridview-practices.md)

### Best Practices
- [Async/Await Pattern](docs/best-practices/async-await.md)
- [Resource Management](docs/best-practices/resource-management.md)
- [Error Handling & Logging](docs/best-practices/error-handling.md)
- [Thread Safety](docs/best-practices/thread-safety.md)
- [Performance Optimization](docs/best-practices/performance.md)
- [Security](docs/best-practices/security.md)
- [Configuration Management](docs/best-practices/configuration.md)

### Testing
- [Testing Overview](docs/testing/testing-overview.md)
- [Unit Testing](docs/testing/unit-testing.md)
- [Integration Testing](docs/testing/integration-testing.md)
- [UI Testing](docs/testing/ui-testing.md)

### Advanced Topics
- [Nullable Reference Types](docs/advanced/nullable-reference-types.md)
- [LINQ Best Practices](docs/advanced/linq-practices.md)
- [Localization (i18n)](docs/advanced/localization-i18n.md)

### Examples
- [MVP Example](docs/examples/mvp-example.md)
- [DI Example](docs/examples/di-example.md)
- [Async UI Example](docs/examples/async-ui-example.md)

---

## 🔧 Code Templates

Use templates from `/templates/` folder:
- `form-template.cs` - Standard Form with MVP pattern
- `service-template.cs` - Service layer template
- `repository-template.cs` - Repository pattern template
- `test-template.cs` - Unit test template

---

## ✅ Pre-Commit Checklist

Before committing code, verify:

- [ ] **Code compiles** without warnings
- [ ] **All tests pass** (`dotnet test`)
- [ ] **No business logic in Forms** - moved to Services
- [ ] **Resources properly disposed** - using statements
- [ ] **Async/await used** for I/O operations
- [ ] **Input validated** with ErrorProvider or validation logic
- [ ] **Errors handled** with try-catch and logging
- [ ] **XML comments added** for public APIs
- [ ] **No magic numbers** - constants defined
- [ ] **Thread-safe UI updates** - Invoke/BeginInvoke used
- [ ] **Tests cover new code** - adequate test coverage
- [ ] **Code follows naming conventions**

---

## 🔗 Quick Links

- **[Full Overview](docs/00-overview.md)** - Complete documentation index
- **[MVP Pattern Guide](docs/architecture/mvp-pattern.md)** - Recommended architecture
- **[Testing Guide](docs/testing/testing-overview.md)** - How to test WinForms apps
- **[Code Examples](docs/examples/)** - Working code samples

---

## 📞 Need Help?

1. Check **[docs/00-overview.md](docs/00-overview.md)** for full documentation
2. Search for specific topic in `/docs/` folders
3. Review **[examples](docs/examples/)** for working code
4. Use slash commands (type `/` in Claude Code) for common tasks

---

## 🎓 Learning Path

**For new developers**:
1. Read [Project Structure](docs/architecture/project-structure.md)
2. Understand [MVP Pattern](docs/architecture/mvp-pattern.md)
3. Review [Naming Conventions](docs/conventions/naming-conventions.md)
4. Study [Code Examples](docs/examples/)
5. Practice with templates from `/templates/`

**For AI assistants**:
1. Load this file first (automatic)
2. **Check project status** - Know what docs exist vs missing
3. Reference specific docs as needed for deep dives
4. **Always use templates** - Never generate code from scratch
5. Follow code generation patterns above
6. Follow pre-commit checklist before suggesting commits
7. Validate against DO/DON'T rules before responding

---

## 📝 Common Code Snippets

### Async Button Click Handler
```csharp
private async void btnLoad_Click(object sender, EventArgs e)
{
    try
    {
        btnLoad.Enabled = false;
        Cursor = Cursors.WaitCursor;

        var data = await _presenter.LoadDataAsync();
        // Update UI with data
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}", "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    finally
    {
        btnLoad.Enabled = true;
        Cursor = Cursors.Default;
    }
}
```

### Thread-Safe UI Update
```csharp
private void UpdateUIFromBackgroundThread(string message)
{
    if (InvokeRequired)
    {
        Invoke(new Action(() => UpdateUIFromBackgroundThread(message)));
        return;
    }

    // Safe to update UI here
    lblStatus.Text = message;
}
```

### Proper Resource Disposal
```csharp
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        // Dispose managed resources
        _timer?.Dispose();
        _backgroundWorker?.Dispose();
        components?.Dispose();

        // Unsubscribe from events
        _service.DataChanged -= OnDataChanged;
    }
    base.Dispose(disposing);
}
```

---

**Last Updated**: 2025-11-07
**Version**: 2.1 (Enhanced for Claude Code optimization)
**Changes**: Added project status, context loading, code generation patterns
