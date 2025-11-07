# Testing Overview for WinForms

## 📋 Overview

Testing is a critical part of building maintainable, reliable WinForms applications. This guide covers testing strategies, tools, and best practices specifically tailored for Windows Forms development.

**What you'll learn:**
- The testing pyramid and how it applies to WinForms
- Different types of tests (unit, integration, UI)
- How to test MVP/MVVM patterns effectively
- Testing tools and frameworks
- Test-Driven Development (TDD) for WinForms
- Common testing challenges and solutions

**Target audience:**
- WinForms developers new to testing
- Teams establishing testing practices
- Developers refactoring legacy code

---

## 🎯 Why This Matters

### Quality and Reliability
- **Catch bugs early**: Tests catch issues before they reach production
- **Regression prevention**: Ensure fixes stay fixed
- **Confidence in changes**: Refactor without fear

### Maintainability
- **Living documentation**: Tests document how code should work
- **Faster debugging**: Tests pinpoint exact failure locations
- **Easier onboarding**: New developers understand code through tests

### Cost Savings
- **Reduce manual testing**: Automated tests run in seconds
- **Lower bug fix costs**: Bugs found early are cheaper to fix
- **Faster release cycles**: Confidence to deploy frequently

### Example Impact
```
Without Tests:
- Manual testing: 4 hours per release
- Bug fixes: 20% of sprint capacity
- Production bugs: 3-5 per month

With Tests (70%+ coverage):
- Automated testing: 5 minutes per commit
- Bug fixes: 5% of sprint capacity
- Production bugs: 0-1 per month
```

---

## Testing Pyramid for WinForms

### The Testing Pyramid

```
         /\
        /UI\          UI Tests (10%)
       /----\         - Slow, expensive, brittle
      / Int  \        Integration Tests (20%)
     /--------\       - Medium speed, moderate cost
    /   Unit   \      Unit Tests (70%)
   /------------\     - Fast, cheap, stable
```

**Recommended Distribution:**
- **70% Unit Tests**: Test business logic, presenters, services
- **20% Integration Tests**: Test database, external services
- **10% UI Tests**: Test critical user workflows

### Why This Distribution?

#### Speed of Execution
```
Unit Test:        < 1ms    ✅ Run thousands per second
Integration Test: 10-100ms ⚠️ Run hundreds per second
UI Test:          1-10s    ❌ Run a few per minute
```

#### Maintenance Cost
```
Unit Test:        Low      ✅ Isolated, rarely breaks
Integration Test: Medium   ⚠️ Requires test data setup
UI Test:          High     ❌ Breaks with UI changes
```

#### Feedback Loop
```
Unit Test:        Immediate    ✅ Know instantly what broke
Integration Test: Fast         ⚠️ Know which component failed
UI Test:          Delayed      ❌ Need to debug failure
```

#### Example
```csharp
// Unit Test - Tests business logic in isolation (70%)
[Fact]
public async Task CalculateDiscount_ReturnsCorrectAmount()
{
    var service = new PricingService();
    var discount = await service.CalculateDiscountAsync(100m, 0.1m);
    Assert.Equal(10m, discount);
}

// Integration Test - Tests DB operations (20%)
[Fact]
public async Task SaveCustomer_PersistsToDatabase()
{
    using var context = CreateTestContext();
    var repo = new CustomerRepository(context);
    await repo.AddAsync(new Customer { Name = "Test" });
    Assert.Equal(1, await context.Customers.CountAsync());
}

// UI Test - Tests complete user workflow (10%)
[Fact]
public void SaveCustomer_UpdatesGrid()
{
    // Launch app, click New, fill form, click Save
    // Verify grid contains new customer
    // Slow, brittle, expensive - use sparingly!
}
```

---

## Types of Tests in WinForms

### Unit Tests

**What to Test:**
- ✅ Presenters (MVP pattern)
- ✅ ViewModels (MVVM pattern)
- ✅ Services (business logic)
- ✅ Repositories (with mocked DbContext)
- ✅ Utility classes
- ✅ Validators

**What NOT to Test:**
- ❌ UI controls themselves (Button, TextBox, etc.)
- ❌ .NET Framework classes
- ❌ Third-party libraries
- ❌ Simple properties (auto-properties)

**Benefits:**
- Fast execution (milliseconds)
- Easy to write and maintain
- Pinpoint exact failures
- No external dependencies

**Tools:**
- **xUnit** (recommended - modern, extensible)
- **NUnit** (mature, feature-rich)
- **MSTest** (built into Visual Studio)

**Example:**
```csharp
public class CustomerServiceTests
{
    [Fact]
    public async Task GetCustomerAsync_ValidId_ReturnsCustomer()
    {
        // Arrange
        var mockRepo = new Mock<ICustomerRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Customer { Id = 1, Name = "John" });

        var service = new CustomerService(mockRepo.Object);

        // Act
        var result = await service.GetCustomerAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.Name);
    }
}
```

### Integration Tests

**What to Test:**
- ✅ Database operations (EF Core)
- ✅ Repository implementations
- ✅ External service integrations
- ✅ Configuration loading
- ✅ File I/O operations

**In-Memory Databases:**
```csharp
// SQLite in-memory for testing
private AppDbContext CreateTestContext()
{
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlite("DataSource=:memory:")
        .Options;

    var context = new AppDbContext(options);
    context.Database.OpenConnection(); // Keep in-memory DB alive
    context.Database.EnsureCreated();
    return context;
}
```

**Benefits:**
- Test real database operations
- Catch SQL/LINQ errors
- Verify transactions work
- Test database constraints

**Example:**
```csharp
[Fact]
public async Task AddCustomer_WithValidData_SavesSuccessfully()
{
    // Arrange
    using var context = CreateTestContext();
    var repository = new CustomerRepository(context);
    var customer = new Customer { Name = "Jane", Email = "jane@test.com" };

    // Act
    await repository.AddAsync(customer);
    await context.SaveChangesAsync();

    // Assert
    var saved = await context.Customers.FirstOrDefaultAsync();
    Assert.NotNull(saved);
    Assert.Equal("Jane", saved.Name);
}
```

### UI Tests

**What to Test:**
- ✅ Critical user workflows (login, checkout, etc.)
- ✅ Complex form interactions
- ✅ Multi-form navigation
- ✅ Keyboard shortcuts

**Challenges in WinForms:**
- Slow execution
- Brittle tests
- Hard to maintain
- Requires running application

**Tools:**
- **FlaUI** (modern, .NET-based)
- **WinAppDriver** (Microsoft-supported)
- **White** (legacy, less maintained)

**When to Use:**
- **Sparingly!** Only for critical workflows
- When unit/integration tests insufficient
- For smoke tests before releases
- For testing third-party controls

**Example (FlaUI):**
```csharp
[Fact]
public void LoginForm_ValidCredentials_OpensMainForm()
{
    // Arrange
    using var app = Application.Launch("MyApp.exe");
    using var automation = new UIA3Automation();
    var window = app.GetMainWindow(automation);

    // Act
    var txtUsername = window.FindFirstDescendant(cf => cf.ByAutomationId("txtUsername"));
    var txtPassword = window.FindFirstDescendant(cf => cf.ByAutomationId("txtPassword"));
    var btnLogin = window.FindFirstDescendant(cf => cf.ByAutomationId("btnLogin"));

    txtUsername.AsTextBox().Text = "admin";
    txtPassword.AsTextBox().Text = "password";
    btnLogin.Click();

    // Assert
    var mainWindow = app.GetMainWindow(automation);
    Assert.Equal("Main Form", mainWindow.Title);
}
```

### Manual Testing

**Still Important:**
- Visual appearance
- Usability and UX
- Edge cases
- Exploratory testing

**Checklist Approach:**
```markdown
## Pre-Release Manual Testing Checklist

### UI/UX
- [ ] All forms display correctly at different resolutions
- [ ] Tab order is logical
- [ ] Keyboard shortcuts work
- [ ] Icons and images load properly

### Functionality
- [ ] Happy path workflows complete successfully
- [ ] Error messages are user-friendly
- [ ] Validation prevents invalid input
- [ ] Data persists correctly

### Edge Cases
- [ ] Slow network/database
- [ ] Large datasets (1000+ records)
- [ ] Special characters in input
- [ ] Concurrent users (if applicable)
```

---

## Testing Strategy for WinForms

### With MVP Pattern

**Test Presenters (Most Important):**

```csharp
// ICustomerView.cs
public interface ICustomerView
{
    string CustomerName { get; set; }
    string Email { get; set; }
    void ShowError(string message);
    void ShowSuccess(string message);
}

// CustomerPresenter.cs
public class CustomerPresenter
{
    private readonly ICustomerView _view;
    private readonly ICustomerService _service;

    public CustomerPresenter(ICustomerView view, ICustomerService service)
    {
        _view = view;
        _service = service;
    }

    public async Task SaveCustomerAsync()
    {
        // Validation
        if (string.IsNullOrWhiteSpace(_view.CustomerName))
        {
            _view.ShowError("Name is required");
            return;
        }

        // Business logic
        try
        {
            var customer = new Customer
            {
                Name = _view.CustomerName,
                Email = _view.Email
            };

            await _service.SaveCustomerAsync(customer);
            _view.ShowSuccess("Customer saved successfully");
        }
        catch (Exception ex)
        {
            _view.ShowError($"Error: {ex.Message}");
        }
    }
}

// CustomerPresenterTests.cs
public class CustomerPresenterTests
{
    [Fact]
    public async Task SaveCustomer_EmptyName_ShowsError()
    {
        // Arrange
        var mockView = new Mock<ICustomerView>();
        mockView.Setup(v => v.CustomerName).Returns("");

        var mockService = new Mock<ICustomerService>();
        var presenter = new CustomerPresenter(mockView.Object, mockService.Object);

        // Act
        await presenter.SaveCustomerAsync();

        // Assert
        mockView.Verify(v => v.ShowError("Name is required"), Times.Once);
        mockService.Verify(s => s.SaveCustomerAsync(It.IsAny<Customer>()), Times.Never);
    }

    [Fact]
    public async Task SaveCustomer_ValidData_CallsServiceAndShowsSuccess()
    {
        // Arrange
        var mockView = new Mock<ICustomerView>();
        mockView.Setup(v => v.CustomerName).Returns("John");
        mockView.Setup(v => v.Email).Returns("john@test.com");

        var mockService = new Mock<ICustomerService>();
        var presenter = new CustomerPresenter(mockView.Object, mockService.Object);

        // Act
        await presenter.SaveCustomerAsync();

        // Assert
        mockService.Verify(s => s.SaveCustomerAsync(It.Is<Customer>(
            c => c.Name == "John" && c.Email == "john@test.com")), Times.Once);
        mockView.Verify(v => v.ShowSuccess("Customer saved successfully"), Times.Once);
    }

    [Fact]
    public async Task SaveCustomer_ServiceThrowsException_ShowsError()
    {
        // Arrange
        var mockView = new Mock<ICustomerView>();
        mockView.Setup(v => v.CustomerName).Returns("John");

        var mockService = new Mock<ICustomerService>();
        mockService.Setup(s => s.SaveCustomerAsync(It.IsAny<Customer>()))
            .ThrowsAsync(new Exception("Database error"));

        var presenter = new CustomerPresenter(mockView.Object, mockService.Object);

        // Act
        await presenter.SaveCustomerAsync();

        // Assert
        mockView.Verify(v => v.ShowError("Error: Database error"), Times.Once);
    }
}
```

**Benefits:**
- ✅ No UI dependency - tests run fast
- ✅ Easy to mock dependencies
- ✅ Complete code coverage possible
- ✅ Tests all logic paths

### With MVVM Pattern

**Test ViewModels:**

```csharp
public class CustomerViewModelTests
{
    [Fact]
    public async Task LoadCustomers_UpdatesCustomersProperty()
    {
        // Arrange
        var mockService = new Mock<ICustomerService>();
        mockService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(new List<Customer>
            {
                new Customer { Id = 1, Name = "John" },
                new Customer { Id = 2, Name = "Jane" }
            });

        var viewModel = new CustomerViewModel(mockService.Object);

        // Act
        await viewModel.LoadCustomersAsync();

        // Assert
        Assert.Equal(2, viewModel.Customers.Count);
        Assert.Equal("John", viewModel.Customers[0].Name);
    }

    [Fact]
    public void CustomerName_WhenChanged_RaisesPropertyChanged()
    {
        // Arrange
        var mockService = new Mock<ICustomerService>();
        var viewModel = new CustomerViewModel(mockService.Object);
        var propertyChanged = false;

        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(viewModel.CustomerName))
                propertyChanged = true;
        };

        // Act
        viewModel.CustomerName = "New Name";

        // Assert
        Assert.True(propertyChanged);
    }

    [Fact]
    public void SaveCommand_CanExecute_WhenNameIsNotEmpty()
    {
        // Arrange
        var mockService = new Mock<ICustomerService>();
        var viewModel = new CustomerViewModel(mockService.Object);

        // Act & Assert
        viewModel.CustomerName = "";
        Assert.False(viewModel.SaveCommand.CanExecute(null));

        viewModel.CustomerName = "John";
        Assert.True(viewModel.SaveCommand.CanExecute(null));
    }
}
```

### Without Pattern (Legacy Code)

**Challenges:**
- ❌ Hard to test - logic tightly coupled to UI
- ❌ Requires form instantiation
- ❌ Slow tests

**Refactoring Approach:**
```csharp
// Before - Untestable
public partial class CustomerForm : Form
{
    private void btnSave_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            MessageBox.Show("Name is required");
            return;
        }

        using var context = new AppDbContext();
        context.Customers.Add(new Customer { Name = txtName.Text });
        context.SaveChanges();
        MessageBox.Show("Saved!");
    }
}

// After - Testable (Extract Method)
public partial class CustomerForm : Form
{
    private readonly CustomerService _service;

    public CustomerForm(CustomerService service)
    {
        InitializeComponent();
        _service = service;
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        await SaveCustomerAsync();
    }

    // Now testable!
    internal async Task SaveCustomerAsync()
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            MessageBox.Show("Name is required");
            return;
        }

        var customer = new Customer { Name = txtName.Text };
        await _service.SaveCustomerAsync(customer);
        MessageBox.Show("Saved!");
    }
}

// Test
[Fact]
public async Task SaveCustomerAsync_EmptyName_ShowsMessageBox()
{
    // Still not ideal, but better than nothing
    var mockService = new Mock<ICustomerService>();
    var form = new CustomerForm(mockService.Object);
    form.Show(); // Required for controls to be initialized

    await form.SaveCustomerAsync();

    mockService.Verify(s => s.SaveCustomerAsync(It.IsAny<Customer>()), Times.Never);
    form.Close();
}
```

---

## Test-Driven Development (TDD)

### TDD in WinForms

**Process:**
1. **Write test first** - Define expected behavior
2. **Watch it fail** - Verify test catches the issue
3. **Implement code** - Write minimum code to pass
4. **Watch it pass** - Verify implementation works
5. **Refactor** - Improve code quality
6. **Repeat** - Next feature

**Benefits for WinForms:**
- Forces good architecture (MVP/MVVM)
- Prevents untestable code
- Better API design
- Complete test coverage

**Example Workflow:**
```csharp
// 1. Write Test First (RED)
[Fact]
public async Task GetActiveCustomers_ReturnsOnlyActiveCustomers()
{
    // Arrange
    var mockRepo = new Mock<ICustomerRepository>();
    mockRepo.Setup(r => r.GetAllAsync())
        .ReturnsAsync(new List<Customer>
        {
            new Customer { Id = 1, Name = "Active", IsActive = true },
            new Customer { Id = 2, Name = "Inactive", IsActive = false }
        });

    var service = new CustomerService(mockRepo.Object);

    // Act
    var result = await service.GetActiveCustomersAsync();

    // Assert
    Assert.Single(result);
    Assert.Equal("Active", result.First().Name);
}

// 2. Run Test - It FAILS (GetActiveCustomersAsync doesn't exist)

// 3. Implement Code (GREEN)
public async Task<List<Customer>> GetActiveCustomersAsync()
{
    var all = await _repository.GetAllAsync();
    return all.Where(c => c.IsActive).ToList();
}

// 4. Run Test - It PASSES

// 5. Refactor (if needed)
public async Task<List<Customer>> GetActiveCustomersAsync()
{
    return (await _repository.GetAllAsync())
        .Where(c => c.IsActive)
        .ToList();
}

// 6. Run Test Again - Still PASSES
```

### Red-Green-Refactor

```
┌─────────────────────────────────────┐
│  🔴 RED: Write Failing Test         │
│  - Test describes desired behavior  │
│  - Test fails (code doesn't exist)  │
└─────────────────────────────────────┘
              ↓
┌─────────────────────────────────────┐
│  🟢 GREEN: Make Test Pass           │
│  - Write minimum code to pass       │
│  - Don't worry about perfection     │
└─────────────────────────────────────┘
              ↓
┌─────────────────────────────────────┐
│  🔵 REFACTOR: Improve Code          │
│  - Remove duplication               │
│  - Improve naming                   │
│  - Extract methods                  │
│  - Tests still pass!                │
└─────────────────────────────────────┘
              ↓
          (Repeat)
```

---

## Testing Tools Ecosystem

### Test Frameworks

| Framework | Pros | Cons | When to Use |
|-----------|------|------|-------------|
| **xUnit** | Modern, extensible, parallel | Less familiar | New projects (recommended) |
| **NUnit** | Mature, feature-rich | Slightly older syntax | Legacy projects |
| **MSTest** | Built-in VS support | Less flexible | Enterprise/MS shops |

**Installation:**
```bash
# xUnit (recommended)
dotnet add package xunit
dotnet add package xunit.runner.visualstudio

# NUnit
dotnet add package NUnit
dotnet add package NUnit3TestAdapter

# MSTest
dotnet add package MSTest.TestFramework
dotnet add package MSTest.TestAdapter
```

### Mocking Frameworks

| Framework | Syntax | Learning Curve | When to Use |
|-----------|--------|----------------|-------------|
| **Moq** | Fluent, LINQ-based | Easy | Most projects (recommended) |
| **NSubstitute** | Natural, minimal | Very easy | Readability focus |
| **FakeItEasy** | Natural language | Easy | Enterprise |

**Moq Example:**
```csharp
var mock = new Mock<ICustomerRepository>();
mock.Setup(r => r.GetByIdAsync(1))
    .ReturnsAsync(new Customer { Id = 1, Name = "John" });
```

**NSubstitute Example:**
```csharp
var repo = Substitute.For<ICustomerRepository>();
repo.GetByIdAsync(1).Returns(new Customer { Id = 1, Name = "John" });
```

### Assertion Libraries

| Library | Style | When to Use |
|---------|-------|-------------|
| **FluentAssertions** | Natural language | Highly recommended |
| **Shouldly** | Natural syntax | Alternative to Fluent |
| **Assert (built-in)** | Traditional | Simple tests |

**FluentAssertions Example:**
```csharp
// Traditional Assert
Assert.Equal(5, list.Count);
Assert.True(customer.IsActive);
Assert.NotNull(result);

// FluentAssertions
list.Should().HaveCount(5);
customer.IsActive.Should().BeTrue();
result.Should().NotBeNull();

// Better error messages
// Expected list to have count 5, but found 3:
// {Customer1, Customer2, Customer3}
```

---

## Best Practices

### ✅ DO:

1. **Follow AAA Pattern**
```csharp
[Fact]
public async Task Test()
{
    // Arrange - Setup test data and mocks
    var mock = new Mock<IRepository>();
    var service = new Service(mock.Object);

    // Act - Execute the operation
    var result = await service.DoSomethingAsync();

    // Assert - Verify the result
    Assert.NotNull(result);
}
```

2. **Use Descriptive Test Names**
```csharp
// ✅ Good
[Fact]
public async Task SaveCustomer_WithNullName_ThrowsArgumentNullException()

// ❌ Bad
[Fact]
public async Task Test1()
```

3. **Test One Thing Per Test**
```csharp
// ✅ Good
[Fact]
public void Validator_NullEmail_ReturnsError() { }

[Fact]
public void Validator_InvalidEmailFormat_ReturnsError() { }

// ❌ Bad
[Fact]
public void Validator_AllCases_WorksCorrectly() { /* tests 10 things */ }
```

4. **Use Test Fixtures for Shared Setup**
```csharp
public class CustomerServiceTests : IDisposable
{
    private readonly Mock<ICustomerRepository> _mockRepo;
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        _mockRepo = new Mock<ICustomerRepository>();
        _service = new CustomerService(_mockRepo.Object);
    }

    public void Dispose()
    {
        // Cleanup if needed
    }
}
```

5. **Test Both Success and Failure Paths**
```csharp
[Fact]
public async Task GetCustomer_ValidId_ReturnsCustomer() { }

[Fact]
public async Task GetCustomer_InvalidId_ThrowsNotFoundException() { }

[Fact]
public async Task GetCustomer_DatabaseError_ThrowsException() { }
```

6. **Use Theory for Data-Driven Tests**
```csharp
[Theory]
[InlineData("", false)]
[InlineData("invalid", false)]
[InlineData("test@example.com", true)]
public void EmailValidator_ValidatesCorrectly(string email, bool expected)
{
    var result = EmailValidator.IsValid(email);
    Assert.Equal(expected, result);
}
```

7. **Mock External Dependencies**
```csharp
// ✅ Good
var mockRepo = new Mock<ICustomerRepository>();
var service = new CustomerService(mockRepo.Object);

// ❌ Bad - Don't use real database in unit tests
var context = new AppDbContext();
var service = new CustomerService(context);
```

8. **Verify Method Calls**
```csharp
[Fact]
public async Task DeleteCustomer_CallsRepositoryDelete()
{
    var mockRepo = new Mock<ICustomerRepository>();
    var service = new CustomerService(mockRepo.Object);

    await service.DeleteCustomerAsync(1);

    mockRepo.Verify(r => r.DeleteAsync(1), Times.Once);
}
```

9. **Keep Tests Independent**
```csharp
// ✅ Good - Each test is self-contained
[Fact]
public void Test1() { var service = new Service(); /* test */ }

[Fact]
public void Test2() { var service = new Service(); /* test */ }

// ❌ Bad - Tests depend on execution order
private Service _sharedService = new Service();

[Fact]
public void Test1() { _sharedService.DoSomething(); }

[Fact]
public void Test2() { _sharedService.DoSomethingElse(); /* assumes Test1 ran */ }
```

10. **Use Async Tests for Async Code**
```csharp
// ✅ Good
[Fact]
public async Task LoadData_ReturnsData()
{
    var result = await service.LoadDataAsync();
    Assert.NotNull(result);
}

// ❌ Bad - Don't block on async code
[Fact]
public void LoadData_ReturnsData()
{
    var result = service.LoadDataAsync().Result; // Deadlock risk!
    Assert.NotNull(result);
}
```

### ❌ DON'T:

1. **Don't Test Private Methods**
```csharp
// ❌ Bad - Test through public API only
[Fact]
public void PrivateCalculate_ReturnsCorrectValue()
{
    var method = typeof(Service).GetMethod("Calculate",
        BindingFlags.NonPublic | BindingFlags.Instance);
    var result = method.Invoke(_service, new object[] { 5 });
    Assert.Equal(10, result);
}
```

2. **Don't Use Thread.Sleep in Tests**
```csharp
// ❌ Bad
[Fact]
public async Task Test()
{
    await service.StartBackgroundTask();
    Thread.Sleep(1000); // Flaky!
    Assert.True(service.IsComplete);
}

// ✅ Good
[Fact]
public async Task Test()
{
    await service.StartBackgroundTaskAsync();
    await service.WaitForCompletionAsync();
    Assert.True(service.IsComplete);
}
```

3. **Don't Assert on MessageBox/UI Directly**
```csharp
// ❌ Bad - Can't reliably test UI
[Fact]
public void ButtonClick_ShowsMessageBox()
{
    form.btnSave.PerformClick();
    // How do you assert MessageBox was shown?
}

// ✅ Good - Test presenter/view model
[Fact]
public async Task SaveCustomer_ShowsSuccessMessage()
{
    mockView.Verify(v => v.ShowSuccess("Saved!"), Times.Once);
}
```

4. **Don't Use Production Database**
```csharp
// ❌ Bad
var context = new AppDbContext("ProductionConnectionString");

// ✅ Good
var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite("DataSource=:memory:")
    .Options;
var context = new AppDbContext(options);
```

5. **Don't Write Tests Without Assertions**
```csharp
// ❌ Bad
[Fact]
public async Task SaveCustomer_Works()
{
    await service.SaveCustomerAsync(customer);
    // No assertion! What are we testing?
}

// ✅ Good
[Fact]
public async Task SaveCustomer_PersistsData()
{
    await service.SaveCustomerAsync(customer);

    var saved = await service.GetCustomerAsync(customer.Id);
    Assert.NotNull(saved);
    Assert.Equal(customer.Name, saved.Name);
}
```

6. **Don't Ignore Test Failures**
```csharp
// ❌ Bad
[Fact(Skip = "Fails sometimes, will fix later")]
public void FlakeyTest() { }

// ✅ Good - Fix or remove
[Fact]
public void ReliableTest() { }
```

7. **Don't Mix Unit and Integration Tests**
```csharp
// ❌ Bad - Same test class for different test types
public class CustomerTests
{
    [Fact] public void UnitTest() { /* mocked */ }

    [Fact] public void IntegrationTest() { /* real DB */ }
}

// ✅ Good - Separate test projects/classes
public class CustomerServiceTests { /* unit tests */ }
public class CustomerRepositoryIntegrationTests { /* integration tests */ }
```

8. **Don't Test Framework/Library Code**
```csharp
// ❌ Bad - Testing LINQ itself
[Fact]
public void Linq_Where_Filters()
{
    var list = new List<int> { 1, 2, 3 };
    var result = list.Where(x => x > 1);
    Assert.Equal(2, result.Count());
}

// ✅ Good - Test your business logic
[Fact]
public void GetActiveCustomers_FiltersCorrectly()
{
    var result = service.GetActiveCustomers();
    Assert.All(result, c => Assert.True(c.IsActive));
}
```

---

## Testing Workflow

### Development Workflow

```
1. 📝 Write failing test
   ↓
2. ▶️ Run test (RED)
   ↓
3. 💻 Write implementation
   ↓
4. ▶️ Run test (GREEN)
   ↓
5. ♻️ Refactor code
   ↓
6. ▶️ Run test (still GREEN)
   ↓
7. ✅ Commit changes
   ↓
   (Repeat)
```

**Commands:**
```bash
# Run all tests
dotnet test

# Run specific test
dotnet test --filter "FullyQualifiedName~SaveCustomer"

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run with coverage
dotnet test /p:CollectCoverage=true
```

### CI/CD Integration

**GitHub Actions Example:**
```yaml
name: CI

on: [push, pull_request]

jobs:
  test:
    runs-on: windows-latest

    steps:
    - uses: actions/checkout@v3

    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 8.0.x

    - name: Restore dependencies
      run: dotnet restore

    - name: Build
      run: dotnet build --no-restore

    - name: Test
      run: dotnet test --no-build --verbosity normal

    - name: Test with Coverage
      run: dotnet test /p:CollectCoverage=true /p:CoverageThreshold=70
```

**Fail Build on Test Failures:**
- All tests must pass before merge
- No exceptions!
- Fix failures immediately

**Code Coverage Gates:**
```xml
<!-- Directory.Build.props -->
<PropertyGroup>
  <CoverageThreshold>70</CoverageThreshold>
  <CoverageThresholdType>line</CoverageThresholdType>
</PropertyGroup>
```

---

## Common Testing Challenges

### Challenge 1: Testing Forms Directly

**Problem:**
```csharp
// Hard to test
public partial class CustomerForm : Form
{
    private void btnSave_Click(object sender, EventArgs e)
    {
        // Business logic mixed with UI
        if (txtName.Text.Length > 0)
        {
            using var context = new AppDbContext();
            context.Customers.Add(new Customer { Name = txtName.Text });
            context.SaveChanges();
        }
    }
}
```

**Solution: Use MVP Pattern**
```csharp
// Easy to test
public class CustomerPresenter
{
    private readonly ICustomerView _view;
    private readonly ICustomerService _service;

    public async Task SaveCustomerAsync()
    {
        if (string.IsNullOrWhiteSpace(_view.CustomerName))
        {
            _view.ShowError("Name required");
            return;
        }

        await _service.SaveCustomerAsync(new Customer
        {
            Name = _view.CustomerName
        });
    }
}

// Test
[Fact]
public async Task SaveCustomer_ValidData_CallsService()
{
    var mockView = new Mock<ICustomerView>();
    mockView.Setup(v => v.CustomerName).Returns("John");
    var mockService = new Mock<ICustomerService>();

    var presenter = new CustomerPresenter(mockView.Object, mockService.Object);
    await presenter.SaveCustomerAsync();

    mockService.Verify(s => s.SaveCustomerAsync(It.IsAny<Customer>()), Times.Once);
}
```

### Challenge 2: Slow Tests

**Problem:**
```csharp
// Slow test - hits real database
[Fact]
public async Task GetCustomers_ReturnsAll()
{
    using var context = new AppDbContext(); // Real DB connection
    var repo = new CustomerRepository(context);
    var result = await repo.GetAllAsync();
    Assert.NotEmpty(result);
}
// Execution time: 500ms+ per test
```

**Solution: Use In-Memory Database or Mocks**
```csharp
// Fast test - in-memory database
[Fact]
public async Task GetCustomers_ReturnsAll()
{
    using var context = CreateInMemoryContext();
    context.Customers.Add(new Customer { Name = "Test" });
    await context.SaveChangesAsync();

    var repo = new CustomerRepository(context);
    var result = await repo.GetAllAsync();

    Assert.Single(result);
}
// Execution time: <10ms

// Faster test - mocked repository
[Fact]
public async Task LoadCustomers_UpdatesView()
{
    var mockRepo = new Mock<ICustomerRepository>();
    mockRepo.Setup(r => r.GetAllAsync())
        .ReturnsAsync(new List<Customer> { new Customer { Name = "Test" } });

    var service = new CustomerService(mockRepo.Object);
    var result = await service.GetAllCustomersAsync();

    Assert.Single(result);
}
// Execution time: <1ms
```

### Challenge 3: Flaky UI Tests

**Problem:**
```csharp
// Flaky - timing issues
[Fact]
public void LoadData_PopulatesGrid()
{
    form.btnLoad.PerformClick();
    Thread.Sleep(100); // Race condition!
    Assert.True(form.dgvCustomers.Rows.Count > 0);
}
```

**Solutions:**
```csharp
// 1. Use proper waits
[Fact]
public async Task LoadData_PopulatesGrid()
{
    form.btnLoad.PerformClick();

    // Wait for data to load
    var timeout = TimeSpan.FromSeconds(5);
    var stopwatch = Stopwatch.StartNew();

    while (form.dgvCustomers.Rows.Count == 0 && stopwatch.Elapsed < timeout)
    {
        await Task.Delay(100);
        Application.DoEvents();
    }

    Assert.True(form.dgvCustomers.Rows.Count > 0);
}

// 2. Better: Test presenter instead
[Fact]
public async Task LoadData_CallsViewUpdate()
{
    var mockView = new Mock<ICustomerView>();
    var mockService = new Mock<ICustomerService>();
    mockService.Setup(s => s.GetAllAsync())
        .ReturnsAsync(new List<Customer> { new Customer() });

    var presenter = new CustomerPresenter(mockView.Object, mockService.Object);
    await presenter.LoadCustomersAsync();

    mockView.Verify(v => v.UpdateCustomers(It.IsAny<List<Customer>>()), Times.Once);
}
```

---

## Getting Started

### Setting Up Testing Project

**1. Create Test Project:**
```bash
# Navigate to solution directory
cd /path/to/solution

# Create test project
dotnet new xunit -n MyApp.Tests

# Add reference to main project
cd MyApp.Tests
dotnet add reference ../MyApp/MyApp.csproj

# Add necessary packages
dotnet add package Moq
dotnet add package FluentAssertions
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

**2. Project Structure:**
```
/Solution
    /MyApp
        /Forms
        /Services
        /Repositories
    /MyApp.Tests
        /Services           # Service tests
        /Repositories       # Repository tests
        /Presenters         # Presenter tests
        /Integration        # Integration tests
        /Helpers            # Test helpers
```

**3. Test Project Configuration:**
```xml
<!-- MyApp.Tests.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0-windows</TargetFramework>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="xunit" Version="2.6.0" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.0" />
    <PackageReference Include="Moq" Version="4.20.0" />
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\MyApp\MyApp.csproj" />
  </ItemGroup>
</Project>
```

### Your First Test

**Step-by-Step:**

**1. Create Test Class:**
```csharp
// MyApp.Tests/Services/CustomerServiceTests.cs
using Xunit;
using Moq;
using FluentAssertions;

namespace MyApp.Tests.Services;

public class CustomerServiceTests
{
    [Fact]
    public void FirstTest_Always_Passes()
    {
        // Arrange
        var expected = true;

        // Act
        var actual = true;

        // Assert
        Assert.Equal(expected, actual);
        // Or with FluentAssertions:
        actual.Should().Be(expected);
    }
}
```

**2. Run Test:**
```bash
# Command line
dotnet test

# Or use Test Explorer in Visual Studio
# View -> Test Explorer -> Run All
```

**3. See Results:**
```
Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:     1, Skipped:     0, Total:     1
```

**4. Add Real Test:**
```csharp
[Fact]
public async Task GetCustomerAsync_ValidId_ReturnsCustomer()
{
    // Arrange
    var mockRepo = new Mock<ICustomerRepository>();
    mockRepo.Setup(r => r.GetByIdAsync(1))
        .ReturnsAsync(new Customer { Id = 1, Name = "John" });

    var service = new CustomerService(mockRepo.Object);

    // Act
    var result = await service.GetCustomerAsync(1);

    // Assert
    result.Should().NotBeNull();
    result.Name.Should().Be("John");
}
```

---

## Complete Testing Example

```csharp
// Service Implementation
public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(
        ICustomerRepository repository,
        ILogger<CustomerService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Customer> GetCustomerAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("ID must be positive", nameof(id));

        _logger.LogInformation("Getting customer {CustomerId}", id);

        var customer = await _repository.GetByIdAsync(id);

        if (customer == null)
        {
            _logger.LogWarning("Customer {CustomerId} not found", id);
            throw new NotFoundException($"Customer {id} not found");
        }

        return customer;
    }

    public async Task<Customer> SaveCustomerAsync(Customer customer)
    {
        if (customer == null)
            throw new ArgumentNullException(nameof(customer));

        if (string.IsNullOrWhiteSpace(customer.Name))
            throw new ArgumentException("Name is required", nameof(customer.Name));

        _logger.LogInformation("Saving customer {CustomerName}", customer.Name);

        if (customer.Id == 0)
            await _repository.AddAsync(customer);
        else
            await _repository.UpdateAsync(customer);

        return customer;
    }
}

// Complete Test Suite
public class CustomerServiceTests : IDisposable
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
    public void Constructor_NullRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new CustomerService(null!, _mockLogger.Object));

        ex.ParamName.Should().Be("repository");
    }

    [Fact]
    public async Task GetCustomerAsync_ValidId_ReturnsCustomer()
    {
        // Arrange
        var expected = new Customer { Id = 1, Name = "John" };
        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.GetCustomerAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("John");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetCustomerAsync_InvalidId_ThrowsArgumentException(int id)
    {
        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.GetCustomerAsync(id));

        ex.Message.Should().Contain("ID must be positive");
    }

    [Fact]
    public async Task GetCustomerAsync_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Customer?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.GetCustomerAsync(999));

        ex.Message.Should().Contain("Customer 999 not found");
    }

    [Fact]
    public async Task SaveCustomerAsync_NewCustomer_CallsAdd()
    {
        // Arrange
        var customer = new Customer { Id = 0, Name = "New Customer" };

        // Act
        await _service.SaveCustomerAsync(customer);

        // Assert
        _mockRepository.Verify(r => r.AddAsync(customer), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Never);
    }

    [Fact]
    public async Task SaveCustomerAsync_ExistingCustomer_CallsUpdate()
    {
        // Arrange
        var customer = new Customer { Id = 1, Name = "Existing" };

        // Act
        await _service.SaveCustomerAsync(customer);

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(customer), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Never);
    }

    [Fact]
    public async Task SaveCustomerAsync_NullCustomer_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.SaveCustomerAsync(null!));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SaveCustomerAsync_EmptyName_ThrowsArgumentException(string name)
    {
        // Arrange
        var customer = new Customer { Name = name };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.SaveCustomerAsync(customer));

        ex.Message.Should().Contain("Name is required");
    }

    public void Dispose()
    {
        // Cleanup if needed
    }
}
```

---

## Testing Checklist

```markdown
## Unit Testing Checklist

### Test Coverage
- [ ] All public methods have tests
- [ ] Success paths covered
- [ ] Error paths covered
- [ ] Edge cases covered
- [ ] Null/empty input tested

### Test Quality
- [ ] Tests follow AAA pattern (Arrange, Act, Assert)
- [ ] Test names describe scenario and expected result
- [ ] One assertion concept per test
- [ ] Tests are independent
- [ ] No hardcoded values (use constants)

### Mocking
- [ ] External dependencies mocked
- [ ] No real database calls in unit tests
- [ ] No network calls in unit tests
- [ ] Mock setup is clear and minimal

### Assertions
- [ ] Use FluentAssertions for readability
- [ ] Verify method calls with Moq
- [ ] Check exception messages, not just types
- [ ] Assert on all relevant properties

### Performance
- [ ] Tests run in < 100ms each
- [ ] No Thread.Sleep() used
- [ ] Async tests use await properly
- [ ] Tests can run in parallel

### Maintainability
- [ ] Test code is clean and readable
- [ ] Shared setup in constructor/fixture
- [ ] Test helpers for common operations
- [ ] Comments only where necessary
```

---

## Related Topics

- **[Unit Testing](unit-testing.md)** - Detailed guide to unit testing WinForms
- **[Integration Testing](integration-testing.md)** - Testing with databases and external services
- **[UI Testing](ui-testing.md)** - Automated UI testing strategies
- **[Test Coverage](test-coverage.md)** - Measuring and improving coverage
- **[MVP Pattern](../architecture/mvp-pattern.md)** - Testable architecture
- **[MVVM Pattern](../architecture/mvvm-pattern.md)** - Alternative testable pattern

---

**Last Updated**: 2025-11-07
**Version**: 1.0
