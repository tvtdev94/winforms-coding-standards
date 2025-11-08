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
