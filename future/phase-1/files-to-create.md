# Phase 1: Files to Create

This document contains the COMPLETE CONTENT for all new files to create in Phase 1.

**Copy-paste ready**: Each section can be copied directly into the new file.

---

## File 1: `.claude/workflows/winforms-development-workflow.md`

**Purpose**: Main development workflow for WinForms applications

**Location**: `.claude/workflows/winforms-development-workflow.md`

**Content**:

```markdown
# WinForms Development Workflow

This workflow guides the development of WinForms applications following best practices.

---

## 1. Planning Phase

### Context Loading Strategy

**Before starting any task**, load the appropriate documentation:

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

### Create Implementation Plan

**For complex tasks** (3+ steps), create a plan in `plans/`:

1. Use appropriate template from `plans/templates/`
2. Fill in requirements, architecture, checklist
3. Save as `plans/YYMMDD-feature-name-plan.md`
4. Reference plan throughout implementation

---

## 2. Implementation Phase

### Always Use Templates

**NEVER generate code from scratch**. Always start with templates:

- `/templates/form-template.cs` - MVP pattern form
- `/templates/service-template.cs` - Business logic service
- `/templates/repository-template.cs` - Data access repository
- `/templates/test-template.cs` - Unit test structure

### Follow MVP/MVVM Pattern

**MVP Pattern** (Recommended):
```
Form (View) → IView interface → Presenter → Service → Repository
```

**MVVM Pattern** (.NET 8+):
```
Form (View) → ViewModel → Service → Repository
```

### Code Generation Patterns

#### Pattern 1: Creating a New Form

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

#### Pattern 2: Creating a Service

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

#### Pattern 3: Creating a Repository

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

### Implementation Rules

**When generating Forms:**
1. ✅ Start with `form-template.cs`
2. ✅ Implement MVP pattern (Form + IView + Presenter)
3. ✅ Async event handlers for data operations
4. ✅ Try-catch with user-friendly error messages
5. ✅ Dispose resources in Dispose() method
6. ✅ Set TabIndex for proper keyboard navigation
7. ✅ Use meaningful control names (not button1, textBox1)

**When generating Services:**
1. ✅ Start with `service-template.cs`
2. ✅ Constructor injection for all dependencies
3. ✅ Validate all inputs (ArgumentNullException, ArgumentException)
4. ✅ Async methods with proper cancellation token support
5. ✅ Log all operations (info, errors, warnings)
6. ✅ Wrap exceptions with meaningful messages
7. ✅ XML documentation on all public methods

**When generating Repositories:**
1. ✅ Start with `repository-template.cs`
2. ✅ Implement generic repository pattern
3. ✅ Use EF Core async methods (ToListAsync, FirstOrDefaultAsync, etc.)
4. ✅ Proper disposal of DbContext
5. ✅ Include soft-delete support if applicable
6. ✅ Error handling with data access exceptions

---

## 3. Testing Phase

### Test-Driven Approach

1. **Generate tests** using `templates/test-template.cs`
2. **Run tests**: `dotnet test`
3. **Fix failures** before proceeding
4. **Verify coverage** meets standards (80%+ for services)

### Testing Strategy

**For Services:**
- Unit tests with mocked dependencies (Moq)
- Test success paths, error cases, edge cases
- Naming: `MethodName_Scenario_ExpectedResult`

**For Repositories:**
- Integration tests with EF Core InMemory database
- Test CRUD operations
- Test queries and filters

**For Presenters:**
- Unit tests with mocked View and Service
- Test user interaction flows
- Test error handling

---

## 4. Code Review Phase

### Self-Review Checklist

Before committing, verify against [code-review-checklist.md](./code-review-checklist.md):

- [ ] Code compiles without warnings
- [ ] All tests pass
- [ ] No business logic in Forms
- [ ] Resources properly disposed
- [ ] Async/await used for I/O
- [ ] Input validated
- [ ] Errors handled with try-catch
- [ ] XML comments added
- [ ] No magic numbers
- [ ] Thread-safe UI updates
- [ ] Tests cover new code
- [ ] Follows naming conventions

### Agent-Assisted Review (Phase 2+)

When agents are available:
```
/review-code [files]
```

This will spawn `winforms-reviewer` agent to check:
- MVP/MVVM pattern adherence
- WinForms best practices
- Threading issues
- Performance concerns

---

## 5. Documentation Phase

### Update Documentation

**For new features:**
- Update XML comments in code
- Update relevant docs in `docs/`
- Add examples if applicable
- Update CHANGELOG.md

**For example project:**
- Keep `example-project/` in sync with standards
- Update if new pattern introduced

---

## 6. Commit Phase

### Git Workflow

**Only commit when user explicitly requests it.**

**When creating commits:**

1. Run `git status` and `git diff` to see changes
2. Draft commit message following repository style
3. Add relevant files to staging area
4. Create commit with co-authored-by footer:

```
Feature description here

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>
```

5. Run `git status` after commit to verify

**DO NOT:**
- ❌ Push unless explicitly requested
- ❌ Use `--no-verify` flag
- ❌ Force push to main/master
- ❌ Amend other developers' commits
- ❌ Commit secrets (.env, credentials, etc.)

---

## Common Code Snippets

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

**Last Updated**: 2025-11-08 (Phase 1)
**Version**: 1.0
```

---

## File 2: `.claude/workflows/testing-workflow.md`

**Purpose**: Testing best practices and workflow

**Location**: `.claude/workflows/testing-workflow.md`

**Content**:

```markdown
# Testing Workflow

Guidelines for testing WinForms applications with xUnit/NUnit.

---

## Test-Driven Development (TDD)

### TDD Cycle

```
1. Write Test (Red)
   ↓
2. Write Code (Green)
   ↓
3. Refactor (Refactor)
   ↓
   Repeat
```

### When to Use TDD

**Recommended for:**
- ✅ Services (business logic)
- ✅ Repositories (data access)
- ✅ Presenters (MVP pattern)
- ✅ Utility classes
- ✅ Extensions

**Not practical for:**
- ❌ UI Designer code
- ❌ Simple DTOs/POCOs
- ❌ Configuration classes

---

## Test Generation

### Pattern 1: Generating Tests for Services

**Workflow:**

1. Read `templates/test-template.cs`
2. Read the service class to test
3. Generate `CustomerServiceTests.cs` with:
   - xUnit `[Fact]` attributes
   - Moq for mocking dependencies
   - Arrange-Act-Assert pattern
   - Test naming: `MethodName_Scenario_ExpectedResult`
   - Tests for success paths, error cases, edge cases

**Example:**

```csharp
public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _mockRepository;
    private readonly Mock<ILogger<CustomerService>> _mockLogger;
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        _mockRepository = new Mock<ICustomerRepository>();
        _mockLogger = new Mock<ILogger<CustomerService>>();
        _service = new CustomerService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_WhenCalled_ReturnsCustomerList()
    {
        // Arrange
        var expected = new List<Customer>
        {
            new Customer { Id = 1, Name = "John Doe" },
            new Customer { Id = 2, Name = "Jane Smith" }
        };
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(expected);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNullCustomer_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _service.CreateAsync(null));
    }

    [Fact]
    public async Task CreateAsync_WithValidCustomer_ReturnsCreatedCustomer()
    {
        // Arrange
        var customer = new Customer { Name = "New Customer", Email = "new@test.com" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Customer>()))
            .ReturnsAsync(new Customer { Id = 1, Name = customer.Name, Email = customer.Email });

        // Act
        var result = await _service.CreateAsync(customer);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        _mockRepository.Verify(r => r.AddAsync(customer), Times.Once);
    }
}
```

---

## Test Naming Conventions

### Format

```
MethodName_Scenario_ExpectedResult
```

### Examples

| Test Name | Purpose |
|-----------|---------|
| `GetAllAsync_WhenCalled_ReturnsCustomerList` | Success path |
| `CreateAsync_WithNullCustomer_ThrowsArgumentNullException` | Error case |
| `UpdateAsync_WithNonExistentId_ReturnsFalse` | Edge case |
| `DeleteAsync_WithValidId_RemovesCustomer` | Success path |
| `SearchAsync_WithEmptyQuery_ReturnsAllCustomers` | Edge case |

---

## Arrange-Act-Assert (AAA) Pattern

### Structure

```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedResult()
{
    // Arrange - Set up test data and mocks
    var mockRepo = new Mock<IRepository>();
    mockRepo.Setup(r => r.GetData()).ReturnsAsync(testData);
    var service = new Service(mockRepo.Object);

    // Act - Execute the method under test
    var result = await service.DoSomething();

    // Assert - Verify the results
    Assert.NotNull(result);
    Assert.Equal(expected, result);
    mockRepo.Verify(r => r.GetData(), Times.Once);
}
```

### Guidelines

**Arrange Section:**
- Create mocks
- Setup mock behavior
- Create test data
- Instantiate class under test

**Act Section:**
- Call the method being tested
- Should be ONE line (if more, extract to helper)

**Assert Section:**
- Verify return value
- Verify state changes
- Verify mock interactions

---

## Coverage Goals

### Target Coverage

| Layer | Coverage Goal | Type |
|-------|--------------|------|
| **Services** | 80%+ | Unit tests with mocks |
| **Repositories** | 70%+ | Integration tests (InMemory DB) |
| **Presenters** | 75%+ | Unit tests with mocks |
| **Models** | N/A | Simple POCOs, no logic |
| **Forms** | Manual | UI testing checklist |

### Running Coverage

```bash
# Install coverage tool
dotnet tool install --global dotnet-coverage

# Run tests with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Generate HTML report (optional)
reportgenerator -reports:coverage.opencover.xml -targetdir:coverage-report
```

---

## Integration Testing

### Repository Integration Tests

**Use EF Core InMemory database:**

```csharp
public class CustomerRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly CustomerRepository _repository;

    public CustomerRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new CustomerRepository(_context);
    }

    [Fact]
    public async Task AddAsync_WithValidCustomer_AddsToDatabase()
    {
        // Arrange
        var customer = new Customer { Name = "Test Customer", Email = "test@test.com" };

        // Act
        var result = await _repository.AddAsync(customer);
        await _context.SaveChangesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(1, await _context.Customers.CountAsync());
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
```

---

## UI Testing

### Manual Testing Checklist

**For each Form:**

- [ ] Form loads without errors
- [ ] All controls visible and properly sized
- [ ] Tab order is logical
- [ ] Keyboard shortcuts work (Alt+key)
- [ ] Validation shows appropriate errors
- [ ] Save/Cancel buttons work
- [ ] Data loads asynchronously (UI not frozen)
- [ ] Error messages are user-friendly
- [ ] Form disposes properly (no memory leaks)
- [ ] Responsive layout (resize window)

### Automated UI Testing (Optional)

For critical flows, use FlaUI or similar:

```csharp
[Fact]
public void CustomerForm_SaveButton_SavesCustomer()
{
    using var app = Application.Launch("MyApp.exe");
    var window = app.GetMainWindow(Automation);

    var nameBox = window.FindFirstDescendant(cf => cf.ByAutomationId("txtName")).AsTextBox();
    var saveButton = window.FindFirstDescendant(cf => cf.ByAutomationId("btnSave")).AsButton();

    nameBox.Text = "Test Customer";
    saveButton.Click();

    // Assert confirmation message appears
    Assert.True(window.FindFirstDescendant(cf => cf.ByText("Saved successfully")) != null);
}
```

---

## Running Tests

### Command Line

```bash
# Run all tests
dotnet test

# Run specific test
dotnet test --filter "FullyQualifiedName~CustomerServiceTests"

# Run with verbosity
dotnet test --verbosity detailed

# Run with coverage
dotnet test /p:CollectCoverage=true
```

### Visual Studio

- **Test Explorer**: View → Test Explorer
- **Run All**: Ctrl+R, A
- **Debug Test**: Right-click → Debug Test
- **Live Unit Testing**: Test → Live Unit Testing → Start

---

## Test Organization

### File Structure

```
/Tests
    ├── /Services
    │   ├── CustomerServiceTests.cs
    │   └── OrderServiceTests.cs
    ├── /Repositories
    │   ├── CustomerRepositoryTests.cs
    │   └── OrderRepositoryTests.cs
    ├── /Presenters
    │   ├── CustomerPresenterTests.cs
    │   └── OrderPresenterTests.cs
    └── /Helpers
        └── TestHelper.cs
```

### Test Class Naming

```
{ClassUnderTest}Tests.cs

Examples:
- CustomerService → CustomerServiceTests.cs
- OrderRepository → OrderRepositoryTests.cs
- MainFormPresenter → MainFormPresenterTests.cs
```

---

**Last Updated**: 2025-11-08 (Phase 1)
**Version**: 1.0
```

---

## File 3: `.claude/workflows/code-review-checklist.md`

**Purpose**: Pre-commit code review checklist

**Location**: `.claude/workflows/code-review-checklist.md`

**Content**:

```markdown
# Code Review Checklist

Use this checklist before committing code to ensure quality and adherence to standards.

---

## ✅ Pre-Commit Checklist

### 1. Compilation & Build

- [ ] **Code compiles** without errors
- [ ] **No build warnings** (treat warnings as errors)
- [ ] **All projects in solution build** successfully

**Verify**:
```bash
dotnet build
```

---

### 2. Testing

- [ ] **All existing tests pass**
- [ ] **New tests added** for new code
- [ ] **Tests cover edge cases** and error scenarios
- [ ] **Test coverage meets standards** (80%+ for services)

**Verify**:
```bash
dotnet test
dotnet test /p:CollectCoverage=true
```

---

### 3. Architecture & Patterns

- [ ] **No business logic in Forms** - Moved to Services or Presenters
- [ ] **MVP/MVVM pattern followed** - Proper separation of concerns
- [ ] **Services use dependency injection** - Constructor injection
- [ ] **Repositories use EF Core correctly** - Async methods, proper disposal
- [ ] **No direct database access from Forms** - Use Services → Repositories

**Check**:
- Forms only handle UI updates
- Presenters coordinate between View and Service
- Services contain business logic
- Repositories handle data access

---

### 4. Resource Management

- [ ] **All IDisposable resources disposed** - Use `using` statements or Dispose()
- [ ] **Form.Dispose() implemented correctly** - Dispose timers, event subscriptions
- [ ] **No memory leaks** - Unsubscribe from events
- [ ] **Database connections closed** - DbContext disposed properly

**Common issues**:
```csharp
// ❌ BAD - Memory leak
private Timer _timer = new Timer();

// ✅ GOOD - Proper disposal
private Timer _timer;
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        _timer?.Dispose();
    }
    base.Dispose(disposing);
}
```

---

### 5. Async/Await

- [ ] **Async/await used for all I/O** - Database, file, network operations
- [ ] **Async methods named with Async suffix** - e.g., `LoadDataAsync()`
- [ ] **Async all the way** - Don't block on async (no `.Result` or `.Wait()`)
- [ ] **CancellationToken support** (for long-running operations)

**Check**:
```csharp
// ❌ BAD - Blocking async
var data = _service.GetDataAsync().Result;

// ✅ GOOD - Proper async
var data = await _service.GetDataAsync();
```

---

### 6. Input Validation

- [ ] **User input validated** - Before processing
- [ ] **ErrorProvider used** (WinForms) or validation logic
- [ ] **Null checks** - ArgumentNullException for required parameters
- [ ] **Business rule validation** - In Service layer

**Example**:
```csharp
// ✅ GOOD - Proper validation
public async Task CreateCustomerAsync(Customer customer)
{
    ArgumentNullException.ThrowIfNull(customer);

    if (string.IsNullOrWhiteSpace(customer.Name))
        throw new ValidationException("Customer name is required");

    // Proceed with creation
}
```

---

### 7. Error Handling & Logging

- [ ] **Try-catch blocks** - Around risky operations
- [ ] **Exceptions logged** - Using ILogger
- [ ] **User-friendly error messages** - No technical jargon in UI
- [ ] **Exceptions not swallowed** - Log or re-throw
- [ ] **Specific exceptions caught** - Avoid `catch (Exception)` unless necessary

**Example**:
```csharp
// ✅ GOOD - Proper error handling
try
{
    await _service.SaveDataAsync(data);
}
catch (ValidationException vex)
{
    _logger.LogWarning(vex, "Validation failed");
    MessageBox.Show(vex.Message, "Validation Error");
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error saving data");
    MessageBox.Show("An error occurred. Please try again.", "Error");
}
```

---

### 8. Thread Safety

- [ ] **UI updates on UI thread** - Use `Invoke` or `BeginInvoke`
- [ ] **No UI controls accessed from background threads**
- [ ] **Async methods used instead of BackgroundWorker** (modern .NET)

**Example**:
```csharp
// ✅ GOOD - Thread-safe UI update
private void UpdateStatus(string message)
{
    if (InvokeRequired)
    {
        Invoke(new Action(() => UpdateStatus(message)));
        return;
    }
    lblStatus.Text = message;
}
```

---

### 9. Code Quality

- [ ] **No magic numbers or strings** - Use constants
- [ ] **XML documentation comments** - On all public APIs
- [ ] **Follows naming conventions** - PascalCase, camelCase, etc.
- [ ] **Code is readable** - Clear variable names, logical structure
- [ ] **No commented-out code** - Remove or add explanation
- [ ] **No TODO comments without Jira ticket** - Track technical debt

**Example**:
```csharp
// ❌ BAD - Magic numbers
if (customer.Age < 18) { }

// ✅ GOOD - Named constant
private const int MINIMUM_AGE = 18;
if (customer.Age < MINIMUM_AGE) { }
```

---

### 10. Documentation

- [ ] **XML comments on public methods** - /// summary, param, returns
- [ ] **README updated** (if applicable)
- [ ] **CHANGELOG updated** (if applicable)
- [ ] **Code examples updated** (if pattern changed)

---

### 11. Security

- [ ] **No hardcoded credentials** - Use configuration
- [ ] **SQL injection prevented** - Use parameterized queries (EF Core handles this)
- [ ] **User input sanitized** - Prevent XSS, command injection
- [ ] **Sensitive data not logged** - No passwords, credit cards in logs
- [ ] **No secrets in source control** - Use .env, Azure Key Vault, etc.

**Example**:
```csharp
// ❌ BAD - Hardcoded connection string
var conn = "Server=prod;User=admin;Password=password123";

// ✅ GOOD - From configuration
var conn = _configuration.GetConnectionString("DefaultConnection");
```

---

### 12. Performance

- [ ] **No N+1 queries** - Use `.Include()` for related data
- [ ] **Large lists virtualized** - Use DataGridView virtualization
- [ ] **Images optimized** - Correct size, compressed
- [ ] **No unnecessary database calls** - Cache where appropriate

---

## 🔴 Critical Issues (Must Fix)

These issues MUST be fixed before committing:

1. **Code doesn't compile**
2. **Tests are failing**
3. **Business logic in Forms** (violates architecture)
4. **Memory leaks** (undisposed resources)
5. **Security vulnerabilities** (hardcoded credentials, SQL injection)
6. **Data loss risk** (no error handling on save operations)

---

## 🟡 Important Issues (Should Fix)

These should be fixed before committing:

1. Missing async/await on I/O operations
2. Missing input validation
3. Missing error handling/logging
4. Thread safety issues
5. Poor naming conventions

---

## 🟢 Minor Issues (Nice to Have)

These can be addressed later:

1. Missing XML documentation
2. Code formatting inconsistencies
3. Performance optimizations
4. Refactoring opportunities

---

## Automated Checks (Phase 2+)

When `winforms-reviewer` agent is available, run automated review:

```
/review-code [files]
```

This will check all items above automatically and generate a report.

---

**Last Updated**: 2025-11-08 (Phase 1)
**Version**: 1.0
```

---

## File 4: `.claude/workflows/expert-behavior-guide.md`

**Purpose**: Expert evaluation and guidance for AI assistants

**Location**: `.claude/workflows/expert-behavior-guide.md`

**Content**:

```markdown
# Expert Behavior Guide

**YOU ARE A WINFORMS CODING STANDARDS EXPERT** - Not just a code generator!

This guide defines how AI assistants should evaluate user requests and provide expert guidance.

---

## 🎯 Core Principle

**The user's request may not always follow best practices.**

Your job is to:
1. **Evaluate** - Is this request aligned with industry standards?
2. **Educate** - Explain why something might be anti-pattern
3. **Suggest** - Offer better alternatives with reasoning
4. **Cite sources** - Reference Microsoft docs, industry best practices

---

## 📋 Request Evaluation Framework

### ✅ If Request Follows Best Practices

**Response Pattern**:
```
"Great approach! This follows the [pattern/principle] because..."
[Implement the request]
```

**Example**:
```
User: "Create CustomerService with repository pattern and DI"

Response:
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

---

### ⚠️ If Request Might Have Issues

**Response Pattern**:
```
"I notice you want to [X]. While this works, it may cause [issue] because:
- [Reason 1 with explanation]
- [Reason 2 with example]

Microsoft's guidelines recommend [Y] instead:
- [Benefit 1]
- [Benefit 2]

Would you like me to implement [Y], or proceed with [X]?"
```

**Example**:
```
User: "Read file with File.ReadAllText in button click"

Response:
"I notice you want to read the file synchronously. While this works,
it will freeze the UI because:
- Synchronous I/O blocks the UI thread during file read
- Small files: Noticeable lag
- Large files: Application appears frozen (ANR)
- Network drives: Can hang for seconds

Microsoft Best Practice (.NET 8):
Always use async I/O to keep UI responsive.

Better approach:
```csharp
private async void btnLoad_Click(object sender, EventArgs e)
{
    var content = await File.ReadAllTextAsync(path);
}
```

This keeps UI responsive and follows modern .NET patterns.
Should I implement the async version instead?"
```

---

### ❌ If Request Violates Best Practices

**Response Pattern**:
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

**Example**:
```
User: "Add SQL query directly in button click handler"

Response:
"⚠️ This approach has significant issues:

**Problems:**
1. **Separation of Concerns** - Forms should only handle UI
2. **Testability** - Cannot unit test database logic in UI layer
3. **Maintainability** - Changes to DB require modifying UI code
4. **Reusability** - Cannot reuse this logic elsewhere

**Why it's problematic:**
Mixing data access with UI creates tightly coupled code that's:
- Hard to test (requires UI automation)
- Hard to maintain (changes ripple across layers)
- Insecure (SQL injection risk if not careful)
- Not reusable (locked in Form)

**Microsoft's Layered Architecture Guidelines:**
- UI Layer: Forms (presentation only)
- Business Layer: Services (business logic)
- Data Layer: Repositories (data access)

**Recommended approach:**
Create a CustomerService and CustomerRepository:

```csharp
// Repository
public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;
    public CustomerRepository(AppDbContext context) => _context = context;

    public async Task<List<Customer>> GetAllAsync()
        => await _context.Customers.ToListAsync();
}

// Service
public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;
    public CustomerService(ICustomerRepository repository)
        => _repository = repository;

    public async Task<List<Customer>> GetAllAsync()
        => await _repository.GetAllAsync();
}

// Form
private async void btnLoad_Click(object sender, EventArgs e)
{
    var customers = await _presenter.LoadCustomersAsync();
}
```

**References:**
- Microsoft: Layered Architecture in ASP.NET
- Pattern: Repository Pattern (Martin Fowler)
- Docs: docs/architecture/mvp-pattern.md

Should I implement it with proper separation of concerns?"
```

---

## 🔍 Common Anti-Patterns to Flag

### 1. Business Logic in Forms

**❌ Bad**:
```csharp
private void btnSave_Click(object sender, EventArgs e)
{
    using var db = new AppDbContext();
    db.Customers.Add(new Customer { Name = txtName.Text });
    db.SaveChanges();
}
```

**Fix**: Move to Service layer
**Reference**: docs/architecture/mvp-pattern.md

---

### 2. Synchronous I/O

**❌ Bad**:
```csharp
var data = File.ReadAllText(path);
```

**Fix**: Use async
**Reference**: docs/best-practices/async-await.md

---

### 3. Missing Resource Disposal

**❌ Bad**:
```csharp
var timer = new Timer();
// Never disposed
```

**Fix**: Implement Dispose()
**Reference**: docs/best-practices/resource-management.md

---

### 4. Cross-Thread UI Access

**❌ Bad**:
```csharp
Task.Run(() => {
    lblStatus.Text = "Done"; // Exception!
});
```

**Fix**: Use Invoke
**Reference**: docs/best-practices/thread-safety.md

---

### 5. No Input Validation

**❌ Bad**:
```csharp
public void SaveCustomer(Customer customer)
{
    _repository.Add(customer); // What if null?
}
```

**Fix**: Add validation
**Reference**: docs/ui-ux/input-validation.md

---

### 6. Swallowing Exceptions

**❌ Bad**:
```csharp
try { /*...*/ }
catch { /* Silent failure */ }
```

**Fix**: Log and handle properly
**Reference**: docs/best-practices/error-handling.md

---

### 7. Magic Numbers/Strings

**❌ Bad**:
```csharp
if (customer.Age < 18) { }
```

**Fix**: Use constants
**Reference**: docs/conventions/code-style.md

---

### 8. No Async/Await

**❌ Bad**:
```csharp
public List<Customer> GetAll()
{
    return _context.Customers.ToList();
}
```

**Fix**: Make async
**Reference**: docs/best-practices/async-await.md

---

## 🎓 Key Principles to Always Enforce

### 1. Separation of Concerns
- UI layer: Forms (presentation only)
- Business layer: Services
- Data layer: Repositories

### 2. SOLID Principles
- **S**ingle Responsibility
- **O**pen/Closed
- **L**iskov Substitution
- **I**nterface Segregation
- **D**ependency Inversion

### 3. Async/Await
- All I/O operations must be async
- Database, file, network calls
- Keeps UI responsive

### 4. Dependency Injection
- Constructor injection pattern
- Interface-based dependencies
- Enables testing and flexibility

### 5. Testability
- Code must be unit testable
- Services and Repositories fully tested
- Presenters tested with mocked dependencies

### 6. Resource Management
- Dispose all IDisposable resources
- Unsubscribe from events
- No memory leaks

### 7. Error Handling
- Never swallow exceptions
- Log all errors
- User-friendly messages in UI

### 8. Security
- Validate all input
- Parameterized queries (EF Core)
- No hardcoded secrets

---

## ⚖️ Balancing Pragmatism and Idealism

### Explain Tradeoffs

**Pattern**:
```
"This is the ideal approach: [X]
For a simple app/prototype, [Y] is acceptable as a shortcut.
For production, I recommend [X] because [reasons]."
```

**Example**:
```
"Ideally, implement full MVP pattern with interfaces and DI.
For a small utility app, a simple Form with direct service calls is acceptable.
For production/enterprise app, MVP provides better testability and maintainability."
```

### When to Be Flexible

**Allow shortcuts for:**
- Prototypes/POCs
- Small utility apps (<5 forms)
- Learning/educational projects
- Time-constrained demos

**Enforce standards for:**
- Production applications
- Team projects
- Long-term maintained apps
- Enterprise software

**Always offer the best solution first, then alternatives if needed.**

---

## 📚 When to Search for Best Practices

If you're unsure whether a request follows best practices:

1. **Check documentation** - Reference `docs/` folder
2. **Consider SOLID principles** - Does it violate SRP, OCP, DIP, etc.?
3. **Think about testability** - Can this be unit tested?
4. **Consider maintainability** - Will this be hard to change later?
5. **Ask yourself** - Would Microsoft recommend this in official docs?

---

## 📝 Documentation References

When citing best practices, reference these docs:

| Topic | Reference |
|-------|-----------|
| Architecture | docs/architecture/mvp-pattern.md, mvvm-pattern.md |
| Async/Await | docs/best-practices/async-await.md |
| Error Handling | docs/best-practices/error-handling.md |
| Thread Safety | docs/best-practices/thread-safety.md |
| Resource Management | docs/best-practices/resource-management.md |
| Security | docs/best-practices/security.md |
| Testing | docs/testing/unit-testing.md |
| Data Access | docs/data-access/entity-framework.md |

---

**Last Updated**: 2025-11-08 (Phase 1)
**Version**: 1.0
```

---

## File 5: `.claude/metadata.json`

**Purpose**: Project metadata and version tracking

**Location**: `.claude/metadata.json`

**Content**:

```json
{
  "version": "4.1.0",
  "name": "WinForms Coding Standards",
  "description": "C# WinForms coding standards, documentation, and project templates for building maintainable, scalable applications",
  "type": "coding-standards-kit",
  "lastUpdated": "2025-11-08",
  "phase": "Phase 1 - Restructure Complete",
  "stats": {
    "documentationFiles": 38,
    "codeTemplates": 4,
    "slashCommands": 18,
    "workflows": 4,
    "agents": 0,
    "skills": 0,
    "exampleProjects": 1,
    "totalLines": 37000,
    "completionStatus": "100%"
  },
  "repository": {
    "type": "git",
    "url": "https://github.com/YOUR_USERNAME/winforms-coding-standards",
    "branch": "main"
  },
  "targets": {
    "dotnetVersion": "8.0",
    "dotnetFrameworkVersion": "4.8",
    "csharpVersion": "12.0",
    "efCoreVersion": "8.0"
  },
  "technologies": {
    "uiFramework": "Windows Forms",
    "orm": "Entity Framework Core 8.0",
    "testing": ["xUnit", "NUnit"],
    "diContainer": "Microsoft.Extensions.DependencyInjection",
    "logging": ["Serilog", "NLog"]
  },
  "roadmap": {
    "phase1": {
      "name": "Restructure Project",
      "status": "Complete",
      "completedDate": "2025-11-08"
    },
    "phase2": {
      "name": "WinForms-Specific Agents",
      "status": "Planned",
      "estimatedDuration": "1-2 days"
    },
    "phase3": {
      "name": "Plan Templates & Scaffolding",
      "status": "Planned",
      "estimatedDuration": "4-6 hours"
    },
    "phase4": {
      "name": "Enhance init-project.ps1",
      "status": "Planned",
      "estimatedDuration": "1 day"
    },
    "phase5": {
      "name": "Skills & Auto-Documentation",
      "status": "Planned",
      "estimatedDuration": "2-3 days"
    }
  }
}
```

---

## File 6: `plans/templates/.gitkeep`

**Purpose**: Keep templates directory in git

**Location**: `plans/templates/.gitkeep`

**Content**: (Empty file)

---

## File 7: `plans/reports/.gitkeep`

**Purpose**: Keep reports directory in git

**Location**: `plans/reports/.gitkeep`

**Content**: (Empty file)

---

**Total Files to Create**: 7 files

**Next Step**: See `files-to-edit.md` for modifications to existing files.

---

**Last Updated**: 2025-11-08
**Phase**: 1
