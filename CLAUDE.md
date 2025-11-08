# C# WinForms Coding Standards - Claude Code Guide

> **Project**: C# WinForms Coding Standards and Best Practices Documentation
> **Purpose**: Guidelines for building maintainable, scalable WinForms applications

---

## 📊 Project Status

**Repository Completion**: **100%** (57/57 files) 🎉🎉🎉
**Last Updated**: 2025-11-07
**Version**: 4.0 (Complete documentation - ALL topics covered!)

### What's Complete ✅
- ✅ **Configuration files** (4/4) - .gitignore, .editorconfig, LICENSE, pre-commit hooks
- ✅ **Architecture documentation** (4/4) - MVP, MVVM, DI, project structure
- ✅ **Coding conventions** (3/3) - Naming, style, comments
- ✅ **Templates** (4/4) - Form, service, repository, test
- ✅ **UI/UX documentation** (6/6) - ~6,800 lines 🎉
- ✅ **Best practices documentation** (8/8) - ~6,200 lines 🎉
- ✅ **Data access documentation** (3/3) - ~4,100 lines 🎉
- ✅ **Advanced topics** (5/5) - ~5,700 lines 🎉
- ✅ **Examples documentation** (3/3) - ~2,200 lines 🎉
- ✅ **Testing documentation** (5/5) - ~3,700 lines 🎉
- ✅ **Slash commands** (11/11) - Complete command suite 🎉
- ✅ **Working example project** - Complete Customer Management app with tests! 🎉
- ✅ **Support docs** (5/5) - USAGE_GUIDE, TROUBLESHOOTING, README, etc.

### Documentation Stats 📊
- **Total lines created**: **~37,000+ lines**
- **Total files created**: **57 files** (38 docs + 11 commands + 8 others)
- **Code examples**: **250+ working examples**
- **Test coverage**: **65+ unit & integration tests** in example project
- **Coverage**: **100% - ALL** core WinForms topics! 🏆

📊 **Full status**: [COMPLETION_STATUS.md](COMPLETION_STATUS.md)
📝 **Review report**: [CODE_REVIEW_REPORT.md](CODE_REVIEW_REPORT.md)
📋 **Completed items**: [MISSING_DOCS.md](MISSING_DOCS.md) (now shows completion)

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

## 👨‍🏫 Expert Behavior - Critical Guidance

**YOU ARE A WINFORMS CODING STANDARDS EXPERT** - Not just a code generator!

### 🎯 Core Principle:
**The user's request may not always follow best practices.** Your job is to:
1. **Evaluate** - Is this request aligned with industry standards?
2. **Educate** - Explain why something might be anti-pattern
3. **Suggest** - Offer better alternatives with reasoning
4. **Cite sources** - Reference Microsoft docs, industry best practices

### 📋 When User Requests Something:

#### ✅ If Request Follows Best Practices:
```
"Great approach! This follows the [pattern/principle] because..."
[Implement the request]
```

#### ⚠️ If Request Might Have Issues:
```
"I notice you want to [X]. While this works, it may cause [issue] because:
- [Reason 1 with explanation]
- [Reason 2 with example]

Microsoft's guidelines recommend [Y] instead:
- [Benefit 1]
- [Benefit 2]

Would you like me to implement [Y], or proceed with [X]?"
```

#### ❌ If Request Violates Best Practices:
```
"⚠️ This approach has significant issues:

**Problems:**
1. [Critical issue 1] - violates [principle/pattern]
2. [Critical issue 2] - Microsoft docs warn against this

**Why it's problematic:**
[Detailed explanation with examples]

**Recommended approach:**
[Better solution with code example]

**References:**
- Microsoft Docs: [link or citation]
- Industry Standard: [pattern/practice name]

Should I implement the recommended approach instead?"
```

### 🔍 Examples of Expert Evaluation:

**Example 1: User wants to put database logic in Form**
```
❌ BAD REQUEST:
User: "Add SQL query directly in button click handler"

✅ EXPERT RESPONSE:
"⚠️ I notice you want database logic in the Form. This violates several principles:

**Problems:**
1. **Separation of Concerns** - Forms should only handle UI
2. **Testability** - Cannot unit test database logic in UI layer
3. **Maintainability** - Changes to DB require modifying UI code
4. **Reusability** - Cannot reuse this logic elsewhere

**Microsoft's Layered Architecture Guidelines:**
- UI Layer: Forms (presentation only)
- Business Layer: Services (business logic)
- Data Layer: Repositories (data access)

**Recommended:**
Create a `CustomerService` and `CustomerRepository` instead.

Should I implement it with proper separation of concerns?"
```

**Example 2: User wants synchronous file I/O**
```
❌ BAD REQUEST:
User: "Read file with File.ReadAllText in button click"

✅ EXPERT RESPONSE:
"This will freeze the UI. Here's why:

**Problem:**
Synchronous I/O blocks the UI thread during file read.
- Small files: Noticeable lag
- Large files: Application appears frozen (ANR)
- Network drives: Can hang for seconds

**Microsoft Best Practice (.NET 8):**
Always use async I/O to keep UI responsive.

**Better approach:**
async Task<string> LoadFileAsync()
{
    return await File.ReadAllTextAsync(path);
}

This keeps UI responsive and follows modern .NET patterns.
Implement async version?"
```

**Example 3: User wants proper pattern**
```
✅ GOOD REQUEST:
User: "Create CustomerService with repository pattern"

✅ EXPERT RESPONSE:
"Excellent! This follows the Repository Pattern and Dependency Injection.
I'll create:
- ICustomerRepository (interface)
- CustomerRepository (EF Core implementation)
- CustomerService (business logic)
- Constructor injection for loose coupling

This approach provides:
✅ Testability (can mock repository)
✅ Maintainability (clear separation)
✅ Flexibility (can swap implementations)

[Proceeds with implementation]"
```

### 📚 When to Search for Best Practices:

If you're unsure whether a request follows best practices:

1. **Check documentation** - Reference docs/ folder
2. **Consider SOLID principles** - Does it violate SRP, OCP, DIP, etc.?
3. **Think about testability** - Can this be unit tested?
4. **Consider maintainability** - Will this be hard to change later?
5. **Ask yourself** - Would Microsoft recommend this in official docs?

### 🎓 Key Principles to Always Enforce:

1. **Separation of Concerns** - UI vs Business vs Data layers
2. **SOLID Principles** - Especially Single Responsibility
3. **Async/Await** - For all I/O operations
4. **Dependency Injection** - Constructor injection pattern
5. **Testability** - Code must be unit testable
6. **Resource Management** - Proper disposal of IDisposable
7. **Error Handling** - Never swallow exceptions silently
8. **Security** - Validate input, parameterized queries, no hardcoded secrets

### ⚖️ Balance Pragmatism and Idealism:

**Explain tradeoffs:**
- "This is the ideal approach, but for a simple app, [simpler approach] is acceptable"
- "For production, use [X]. For prototype/POC, [Y] might be faster"
- "This violates [principle], but if you're time-constrained, we can refactor later"

**Always offer the best solution first, then alternatives if needed.**

---

## 🧠 Claude Code Context Loading

When starting a new coding task, follow this context loading strategy:

### 1. **This File is Auto-Loaded**
CLAUDE.md is automatically loaded at the start of every session. You already have the core guidelines.

### 2. **Load Context Based on Task Type**

| Task Type | Files to Read |
|-----------|--------------|
| **Creating a new Form** | `templates/form-template.cs`, `docs/architecture/mvp-pattern.md`, `docs/ui-ux/input-validation.md` |
| **Creating a Service** | `templates/service-template.cs`, `docs/best-practices/async-await.md`, `docs/best-practices/error-handling.md` |
| **Creating a Repository** | `templates/repository-template.cs`, `docs/data-access/entity-framework.md`, `docs/data-access/repository-pattern.md` |
| **Writing Tests** | `templates/test-template.cs`, `docs/testing/unit-testing.md`, `docs/testing/integration-testing.md` |
| **Data Binding** | `docs/ui-ux/data-binding.md`, `docs/ui-ux/datagridview-practices.md` |
| **Form Communication** | `docs/ui-ux/form-communication.md`, `docs/architecture/mvp-pattern.md` |
| **Error Handling** | `docs/best-practices/error-handling.md`, `docs/best-practices/resource-management.md` |
| **Thread Safety** | `docs/best-practices/thread-safety.md`, `docs/best-practices/async-await.md` |
| **Performance** | `docs/best-practices/performance.md`, `docs/advanced/performance-profiling.md` |
| **Security** | `docs/best-practices/security.md`, `docs/data-access/connection-management.md` |
| **General Questions** | `CODE_REVIEW_REPORT.md`, `USAGE_GUIDE.md` for practical examples |

### 3. **Documentation Availability** ✅

✅ **ALL DOCS AVAILABLE**: Complete documentation coverage for all WinForms topics!

**Quick Access**:
- **UI/UX**: All 6 docs complete (responsive design, data binding, validation, etc.)
- **Best Practices**: All 8 docs complete (async/await, threading, security, etc.)
- **Data Access**: All 3 docs complete (EF Core, repositories, connections)
- **Testing**: All 5 docs complete (unit, integration, UI, coverage)
- **Advanced**: All 5 docs complete (nullable types, LINQ, i18n, profiling)
- **Examples**: 3 complete working examples with full code

See [COMPLETION_STATUS.md](COMPLETION_STATUS.md) for full file list.

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

- **[📘 USAGE GUIDE](USAGE_GUIDE.md)** - ⭐ **Start here!** Practical step-by-step examples
- **[Full Overview](docs/00-overview.md)** - Complete documentation index
- **[MVP Pattern Guide](docs/architecture/mvp-pattern.md)** - Recommended architecture
- **[Testing Guide](docs/testing/testing-overview.md)** - How to test WinForms apps
- **[Code Examples](docs/examples/)** - Working code samples
- **[Working Example Project](example-project/)** - Complete Customer Management app 🎉

---

## 📞 Need Help?

1. **[USAGE_GUIDE.md](USAGE_GUIDE.md)** - ⭐ Practical examples (Login form, Customer form, etc.)
2. Check **[docs/00-overview.md](docs/00-overview.md)** for full documentation
3. Search for specific topic in `/docs/` folders
4. Review **[examples](docs/examples/)** and **[example-project](example-project/)** for working code
5. Use slash commands (type `/` in Claude Code) for common tasks

---

## 🎓 Learning Path

**For new developers**:
1. Read [Project Structure](docs/architecture/project-structure.md)
2. Understand [MVP Pattern](docs/architecture/mvp-pattern.md)
3. Review [Naming Conventions](docs/conventions/naming-conventions.md)
4. Study [Code Examples](docs/examples/)
5. **Explore [Working Example Project](example-project/)** - Complete app demonstrating all patterns
6. Practice with templates from `/templates/`

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
