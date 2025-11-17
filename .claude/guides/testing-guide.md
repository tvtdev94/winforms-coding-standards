# WinForms Testing Guide

> **Purpose**: Complete guide for unit testing, integration testing, and UI testing
> **Audience**: AI assistants and developers writing tests for WinForms apps

---

## 📋 Table of Contents

1. [Testing Overview](#testing-overview)
2. [Unit Testing](#unit-testing)
3. [Integration Testing](#integration-testing)
4. [Test Patterns](#test-patterns)
5. [Mocking with Moq](#mocking-with-moq)
6. [Common Test Scenarios](#common-test-scenarios)

---

## Testing Overview

### Testing Strategy

**Test Pyramid**:
```
        ┌─────────┐
        │   UI    │  ◄── Few (manual or automated UI tests)
        └─────────┘
      ┌─────────────┐
      │ Integration │  ◄── Some (test repository + DB)
      └─────────────┘
    ┌─────────────────┐
    │   Unit Tests    │  ◄── Many (test services, presenters)
    └─────────────────┘
```

### Test Types

| Type | What to Test | Tools |
|------|--------------|-------|
| **Unit Tests** | Services, Presenters, Business Logic | xUnit, Moq |
| **Integration Tests** | Repositories + Database | xUnit, EF In-Memory |
| **UI Tests** | Forms, User Interactions | Manual or FlaUI |

### Coverage Goals

- **Unit Tests**: 80%+ code coverage
- **Integration Tests**: All repository methods
- **UI Tests**: Critical user flows

---

## Unit Testing

### What to Test

✅ **DO test**:
- Services (business logic)
- Presenters (UI logic)
- Validation logic
- Exception handling
- Edge cases

❌ **DON'T test**:
- Forms directly (test presenters instead)
- EF Core internals
- Third-party libraries

### Test Structure (AAA Pattern)

```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedResult()
{
    // Arrange - Setup test data and mocks
    var customer = new Customer { Name = "John" };
    _mockRepository.Setup(r => r.GetByIdAsync(1))
        .ReturnsAsync(customer);

    // Act - Execute the method under test
    var result = await _service.GetByIdAsync(1);

    // Assert - Verify the results
    Assert.NotNull(result);
    Assert.Equal("John", result.Name);
}
```

### Test Class Template

```csharp
public class CustomerServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<ILogger<CustomerService>> _loggerMock;
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        // Initialize mocks
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _loggerMock = new Mock<ILogger<CustomerService>>();

        // Setup Unit of Work to return repository
        _unitOfWorkMock.Setup(u => u.Customers)
            .Returns(_customerRepositoryMock.Object);

        // Create service under test
        _service = new CustomerService(
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task AddAsync_ValidCustomer_ReturnsCustomer()
    {
        // ... test implementation
    }
}
```

---

## Integration Testing

### What to Test

✅ **Test with real database**:
- Repository methods (CRUD operations)
- EF Core queries
- Database constraints
- Transactions

### Setup with In-Memory Database

```csharp
public class CustomerRepositoryIntegrationTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly CustomerRepository _repository;

    public CustomerRepositoryIntegrationTests()
    {
        // Setup In-Memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new CustomerRepository(_context);

        // Seed test data
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        _context.Customers.AddRange(
            new Customer { Id = 1, Name = "John", Email = "john@example.com" },
            new Customer { Id = 2, Name = "Jane", Email = "jane@example.com" }
        );
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCustomers()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsCustomer()
    {
        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.Name);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
```

---

## Test Patterns

### Test Naming Convention

**Format**: `MethodName_Scenario_ExpectedResult`

```csharp
// ✅ GOOD test names
AddAsync_ValidCustomer_ReturnsCustomer()
AddAsync_NullCustomer_ThrowsArgumentNullException()
GetByIdAsync_NonExistentId_ReturnsNull()
UpdateAsync_ValidCustomer_UpdatesSuccessfully()

// ❌ BAD test names
Test1()
TestAddCustomer()
CustomerTest()
```

### Testing Exceptions

```csharp
[Fact]
public async Task AddAsync_NullCustomer_ThrowsArgumentNullException()
{
    // Act & Assert
    await Assert.ThrowsAsync<ArgumentNullException>(
        () => _service.AddAsync(null));
}

[Fact]
public async Task AddAsync_InvalidEmail_ThrowsArgumentException()
{
    // Arrange
    var customer = new Customer
    {
        Name = "John",
        Email = "invalid-email" // Invalid format
    };

    // Act & Assert
    var exception = await Assert.ThrowsAsync<ArgumentException>(
        () => _service.AddAsync(customer));

    Assert.Contains("Invalid email", exception.Message);
}
```

### Testing with Theory (Parameterized Tests)

```csharp
[Theory]
[InlineData("")]
[InlineData(" ")]
[InlineData(null)]
public async Task AddAsync_InvalidName_ThrowsArgumentException(string name)
{
    // Arrange
    var customer = new Customer
    {
        Name = name,
        Email = "test@example.com"
    };

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(
        () => _service.AddAsync(customer));
}

[Theory]
[InlineData("test@example.com", true)]
[InlineData("invalid", false)]
[InlineData("", false)]
public void IsValidEmail_VariousInputs_ReturnsExpected(string email, bool expected)
{
    // Act
    var result = _validator.IsValidEmail(email);

    // Assert
    Assert.Equal(expected, result);
}
```

---

## Mocking with Moq

### Setup Mock Returns

```csharp
// Return a value
_mockRepository.Setup(r => r.GetByIdAsync(1))
    .ReturnsAsync(new Customer { Id = 1, Name = "John" });

// Return based on parameter
_mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
    .ReturnsAsync((int id) => new Customer { Id = id, Name = $"Customer{id}" });

// Return null
_mockRepository.Setup(r => r.GetByIdAsync(999))
    .ReturnsAsync((Customer)null);

// Return Task.CompletedTask
_mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Customer>()))
    .Returns(Task.CompletedTask);
```

### Setup Mock Throws

```csharp
// Throw exception
_mockRepository.Setup(r => r.GetByIdAsync(-1))
    .ThrowsAsync(new ArgumentException("Invalid ID"));

// Throw on any call
_mockRepository.Setup(r => r.SaveChangesAsync(default))
    .ThrowsAsync(new DbUpdateException("Database error"));
```

### Verify Mock Calls

```csharp
// Verify method was called once
_mockRepository.Verify(
    r => r.AddAsync(It.IsAny<Customer>(), default),
    Times.Once);

// Verify method was called with specific parameter
_mockRepository.Verify(
    r => r.GetByIdAsync(1, default),
    Times.Once);

// Verify method was never called
_mockRepository.Verify(
    r => r.DeleteAsync(It.IsAny<int>(), default),
    Times.Never);

// Verify all setups were called
_mockRepository.VerifyAll();

// Verify no other calls were made
_mockRepository.VerifyNoOtherCalls();
```

### Verify Property Access

```csharp
// Verify property getter was accessed
_unitOfWorkMock.VerifyGet(u => u.Customers, Times.Once);

// Verify property setter was called
_mockView.VerifySet(v => v.CustomerName = "John", Times.Once);
```

---

## Common Test Scenarios

### Testing Service Methods

```csharp
public class CustomerServiceTests
{
    [Fact]
    public async Task AddAsync_ValidCustomer_CallsRepositoryAndSaves()
    {
        // Arrange
        var customer = new Customer { Name = "John", Email = "john@test.com" };

        _customerRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Customer>(), default))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        var result = await _service.AddAsync(customer);

        // Assert
        Assert.NotNull(result);
        _customerRepositoryMock.Verify(
            r => r.AddAsync(customer, default),
            Times.Once);
        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(default),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingCustomer_ReturnsCustomer()
    {
        // Arrange
        var expected = new Customer { Id = 1, Name = "John" };
        _customerRepositoryMock
            .Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.Id, result.Id);
        Assert.Equal(expected.Name, result.Name);
    }

    [Fact]
    public async Task DeleteAsync_ExistingCustomer_CallsRepositoryAndSaves()
    {
        // Arrange
        _customerRepositoryMock
            .Setup(r => r.DeleteAsync(1, default))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _customerRepositoryMock.Verify(
            r => r.DeleteAsync(1, default),
            Times.Once);
        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(default),
            Times.Once);
    }
}
```

### Testing Presenters

```csharp
public class CustomerPresenterTests
{
    private readonly Mock<ICustomerService> _serviceMock;
    private readonly Mock<ICustomerView> _viewMock;
    private readonly CustomerPresenter _presenter;

    public CustomerPresenterTests()
    {
        _serviceMock = new Mock<ICustomerService>();
        _viewMock = new Mock<ICustomerView>();
        _presenter = new CustomerPresenter(_serviceMock.Object);
        _presenter.SetView(_viewMock.Object);
    }

    [Fact]
    public async Task OnLoadRequested_Success_UpdatesView()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new Customer { Id = 1, Name = "John" },
            new Customer { Id = 2, Name = "Jane" }
        };

        _serviceMock.Setup(s => s.GetAllAsync(default))
            .ReturnsAsync(customers);

        // Act
        _viewMock.Raise(v => v.LoadRequested += null, EventArgs.Empty);

        // Wait for async operation
        await Task.Delay(100);

        // Assert
        _viewMock.Verify(
            v => v.SetCustomerList(It.Is<List<Customer>>(list => list.Count == 2)),
            Times.Once);
    }

    [Fact]
    public async Task OnSaveRequested_ServiceThrows_ShowsError()
    {
        // Arrange
        _viewMock.SetupGet(v => v.CustomerName).Returns("John");
        _viewMock.SetupGet(v => v.Email).Returns("john@test.com");

        _serviceMock.Setup(s => s.AddAsync(It.IsAny<Customer>(), default))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act
        _viewMock.Raise(v => v.SaveRequested += null, EventArgs.Empty);

        // Wait for async operation
        await Task.Delay(100);

        // Assert
        _viewMock.Verify(
            v => v.ShowError(It.Is<string>(msg => msg.Contains("Database error"))),
            Times.Once);
    }
}
```

### Testing Validation Logic

```csharp
public class CustomerValidatorTests
{
    private readonly CustomerValidator _validator;

    public CustomerValidatorTests()
    {
        _validator = new CustomerValidator();
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("John", true)]
    public void ValidateName_VariousInputs_ReturnsExpected(string name, bool expected)
    {
        // Act
        var result = _validator.ValidateName(name);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("invalid", false)]
    [InlineData("@example.com", false)]
    [InlineData("test@", false)]
    public void ValidateEmail_VariousFormats_ReturnsExpected(string email, bool expected)
    {
        // Act
        var result = _validator.ValidateEmail(email);

        // Assert
        Assert.Equal(expected, result);
    }
}
```

---

## Summary

**Key Testing Principles**:

1. ✅ **AAA Pattern**: Arrange, Act, Assert
2. ✅ **One assertion per test** (or related assertions)
3. ✅ **Clear test names**: `MethodName_Scenario_ExpectedResult`
4. ✅ **Test both success and failure cases**
5. ✅ **Use mocks for dependencies** (Moq)
6. ✅ **Verify mock interactions**
7. ✅ **Use Theory for parameterized tests**
8. ✅ **Integration tests for repositories**
9. ✅ **Unit tests for services and presenters**
10. ✅ **80%+ code coverage goal**

**Test Checklist**:
- [x] ✅ Start with `test-template.cs`
- [x] ✅ One test class per class under test
- [x] ✅ Use Moq for mocking dependencies
- [x] ✅ Arrange-Act-Assert structure
- [x] ✅ Test naming: `MethodName_Scenario_ExpectedResult`
- [x] ✅ Test both success and failure scenarios
- [x] ✅ Use Assert.Throws for exception testing
- [x] ✅ Verify mocks were called correctly
- [x] ✅ Clean up resources in Dispose()

---

**See also**:
- [Testing Overview](../../docs/testing/testing-overview.md) - Complete testing documentation
- [Unit Testing](../../docs/testing/unit-testing.md) - Detailed unit testing guide
- [Integration Testing](../../docs/testing/integration-testing.md) - Integration testing patterns
- [Test Template](../../templates/test-template.cs) - Production-ready test template
