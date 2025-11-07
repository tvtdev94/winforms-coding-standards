# Unit Testing in WinForms

## 📋 Overview

Unit testing is the practice of testing individual components of your application in isolation from their dependencies. In WinForms applications, unit tests focus on testing business logic, presenters, services, and utility classes while mocking external dependencies like databases, file systems, and UI components.

This guide covers:
- What to unit test in WinForms applications
- Setting up xUnit testing framework
- Mocking dependencies with Moq
- Testing presenters, services, and ViewModels
- Best practices and common patterns
- Complete working examples

**Target audience**: Developers writing maintainable WinForms applications with proper separation of concerns (MVP/MVVM patterns).

---

## 🎯 Why This Matters

### Benefits of Unit Testing

#### 1. Regression Prevention
Unit tests catch bugs before they reach production. When you refactor code, tests verify that existing functionality still works.

```csharp
// Your test catches this bug immediately:
[Fact]
public void CalculateDiscount_NegativePrice_ThrowsException()
{
    // Test fails when someone changes validation logic incorrectly
    var service = new PricingService();
    Assert.Throws<ArgumentException>(() =>
        service.CalculateDiscount(-100, 0.1m));
}
```

#### 2. Design Feedback
If a class is hard to test, it's probably poorly designed. Tests encourage:
- Single Responsibility Principle
- Dependency Injection
- Loose coupling
- Clear interfaces

#### 3. Living Documentation
Tests document how your code should be used:

```csharp
[Fact]
public void SaveCustomer_ValidCustomer_ReturnsCustomerId()
{
    // This test documents the expected behavior clearly
    var service = new CustomerService(_mockRepo.Object, _mockLogger.Object);
    var customer = new Customer { Name = "John", Email = "john@test.com" };

    var result = await service.SaveAsync(customer);

    Assert.True(result > 0);
}
```

#### 4. Faster Development
- Catch bugs earlier (cheaper to fix)
- Refactor with confidence
- Less time debugging
- Faster feedback loop than manual testing

#### 5. Better Code Quality
Code written with tests in mind tends to be:
- More modular
- Better separated
- Easier to understand
- Less coupled

---

## What to Unit Test

### ✅ Presenters (MVP Pattern)

Presenters are **ideal** for unit testing because they:
- Contain business logic without UI dependencies
- Interact through interfaces (easy to mock)
- Are pure C# classes (no WinForms inheritance)

**What to test:**
- Business logic
- View updates
- Service calls
- Error handling
- Input validation

**Example:**

```csharp
public class CustomerPresenterTests
{
    private readonly Mock<ICustomerView> _mockView;
    private readonly Mock<ICustomerService> _mockService;
    private readonly CustomerPresenter _presenter;

    public CustomerPresenterTests()
    {
        _mockView = new Mock<ICustomerView>();
        _mockService = new Mock<ICustomerService>();
        _presenter = new CustomerPresenter(_mockView.Object, _mockService.Object);
    }

    [Fact]
    public async Task LoadCustomers_Success_UpdatesViewWithCustomers()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new Customer { Id = 1, Name = "Alice" },
            new Customer { Id = 2, Name = "Bob" }
        };
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(customers);

        // Act
        await _presenter.LoadCustomersAsync();

        // Assert
        _mockView.VerifySet(v => v.Customers = customers, Times.Once);
    }

    [Fact]
    public async Task LoadCustomers_ServiceThrows_DisplaysErrorMessage()
    {
        // Arrange
        _mockService.Setup(s => s.GetAllAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        await _presenter.LoadCustomersAsync();

        // Assert
        _mockView.Verify(v => v.ShowError("Failed to load customers: Database error"),
            Times.Once);
    }
}
```

### ✅ Services

Services contain business logic and are perfect for unit testing.

**What to test:**
- Business rules
- Validation logic
- Data transformation
- Exception handling
- Logging calls

**Example:**

```csharp
public class OrderServiceTests
{
    [Fact]
    public async Task CalculateTotal_WithDiscount_AppliesDiscountCorrectly()
    {
        // Arrange
        var mockRepo = new Mock<IOrderRepository>();
        var service = new OrderService(mockRepo.Object);
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new OrderItem { Price = 100, Quantity = 2 }, // $200
                new OrderItem { Price = 50, Quantity = 1 }   // $50
            },
            DiscountPercent = 10 // 10% discount
        };

        // Act
        var total = service.CalculateTotal(order);

        // Assert
        Assert.Equal(225m, total); // (200 + 50) * 0.9 = 225
    }

    [Fact]
    public void ValidateOrder_EmptyItems_ThrowsValidationException()
    {
        // Arrange
        var service = new OrderService(Mock.Of<IOrderRepository>());
        var order = new Order { Items = new List<OrderItem>() };

        // Act & Assert
        var ex = Assert.Throws<ValidationException>(() =>
            service.ValidateOrder(order));
        Assert.Contains("must have at least one item", ex.Message);
    }
}
```

### ✅ ViewModels (MVVM Pattern)

For .NET 8+ with CommunityToolkit.Mvvm:

**What to test:**
- Properties and property change notifications
- Command execution
- Command CanExecute logic
- Business logic in ViewModel

**Example:**

```csharp
public class CustomerViewModelTests
{
    [Fact]
    public void Name_WhenChanged_RaisesPropertyChanged()
    {
        // Arrange
        var viewModel = new CustomerViewModel();
        var propertyChanged = false;
        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(CustomerViewModel.Name))
                propertyChanged = true;
        };

        // Act
        viewModel.Name = "John";

        // Assert
        Assert.True(propertyChanged);
    }

    [Fact]
    public void SaveCommand_InvalidCustomer_CannotExecute()
    {
        // Arrange
        var viewModel = new CustomerViewModel { Name = "" }; // Invalid

        // Act
        var canExecute = viewModel.SaveCommand.CanExecute(null);

        // Assert
        Assert.False(canExecute);
    }
}
```

### ✅ Utility Classes

Helper classes, extensions, validators are great for unit testing.

**Example:**

```csharp
public class EmailValidatorTests
{
    [Theory]
    [InlineData("valid@email.com", true)]
    [InlineData("user@domain.co.uk", true)]
    [InlineData("invalid.email", false)]
    [InlineData("@nodomain.com", false)]
    [InlineData("", false)]
    public void IsValidEmail_VariousInputs_ReturnsExpected(string email, bool expected)
    {
        // Act
        var result = EmailValidator.IsValid(email);

        // Assert
        Assert.Equal(expected, result);
    }
}

public class StringExtensionsTests
{
    [Fact]
    public void TruncateWithEllipsis_LongString_TruncatesCorrectly()
    {
        // Arrange
        var longString = "This is a very long string that needs truncation";

        // Act
        var result = longString.TruncateWithEllipsis(20);

        // Assert
        Assert.Equal("This is a very lo...", result);
        Assert.Equal(20, result.Length);
    }
}
```

---

## What NOT to Unit Test

### ❌ Don't Test WinForms Controls

**Why**: You're testing the .NET Framework, not your code.

```csharp
// ❌ DON'T DO THIS
[Fact]
public void Button_WhenClicked_ChangesText()
{
    var button = new Button { Text = "Click me" };
    button.PerformClick();
    // This tests Button behavior, not your code
}
```

**Instead**: Test the presenter/ViewModel that handles the button click logic:

```csharp
// ✅ DO THIS
[Fact]
public async Task HandleSaveClick_ValidData_CallsSaveService()
{
    await _presenter.HandleSaveAsync();
    _mockService.Verify(s => s.SaveAsync(It.IsAny<Customer>()), Times.Once);
}
```

### ❌ Don't Test .NET Framework Code

```csharp
// ❌ DON'T TEST .NET's List<T>
[Fact]
public void List_Add_IncreasesCount()
{
    var list = new List<string>();
    list.Add("test");
    Assert.Equal(1, list.Count); // Testing framework, not your code
}
```

### ❌ Don't Test Third-Party Libraries

```csharp
// ❌ DON'T TEST Entity Framework
[Fact]
public void DbContext_SaveChanges_SavesData()
{
    var context = new MyDbContext();
    // Testing EF Core, not your code
}
```

**Instead**: Test your repository that uses EF Core (integration test) or mock it (unit test).

### ❌ Don't Test Simple Getters/Setters

```csharp
// ❌ WASTE OF TIME
[Fact]
public void Customer_Name_GetSet()
{
    var customer = new Customer();
    customer.Name = "John";
    Assert.Equal("John", customer.Name); // No value
}
```

**Exception**: Test getters/setters that have logic:

```csharp
// ✅ THIS IS WORTH TESTING
public class Customer
{
    private string _email;
    public string Email
    {
        get => _email;
        set => _email = value?.Trim().ToLowerInvariant() ?? "";
    }
}

[Fact]
public void Email_WithWhitespace_TrimsAndLowerCases()
{
    var customer = new Customer { Email = "  JOHN@TEST.COM  " };
    Assert.Equal("john@test.com", customer.Email);
}
```

### ❌ Don't Test Private Methods

**Why**: Test public API only. Private methods are implementation details.

```csharp
// ❌ DON'T USE REFLECTION TO TEST PRIVATE METHODS
[Fact]
public void PrivateHelperMethod_ReturnsCorrectValue()
{
    var service = new CustomerService();
    var method = typeof(CustomerService)
        .GetMethod("PrivateHelper", BindingFlags.NonPublic | BindingFlags.Instance);
    var result = method.Invoke(service, new object[] { 42 });
    // This is brittle and tests implementation details
}
```

**Instead**: Test through public methods:

```csharp
// ✅ PRIVATE METHOD TESTED INDIRECTLY
[Fact]
public void PublicMethod_UsesPrivateHelper_ReturnsExpected()
{
    var service = new CustomerService();
    var result = service.ProcessCustomer(customer); // Calls private method internally
    Assert.NotNull(result);
}
```

---

## Setting Up xUnit

### Installing xUnit

Create a test project:

```bash
dotnet new xunit -n YourApp.Tests
cd YourApp.Tests
dotnet add reference ../YourApp/YourApp.csproj
```

### Required NuGet Packages

```xml
<!-- YourApp.Tests.csproj -->
<ItemGroup>
  <!-- Testing Framework -->
  <PackageReference Include="xunit" Version="2.6.2" />
  <PackageReference Include="xunit.runner.visualstudio" Version="2.5.4">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
  </PackageReference>

  <!-- Mocking -->
  <PackageReference Include="Moq" Version="4.20.70" />

  <!-- Fluent Assertions (optional but recommended) -->
  <PackageReference Include="FluentAssertions" Version="6.12.0" />

  <!-- Code Coverage -->
  <PackageReference Include="coverlet.collector" Version="6.0.0">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
  </PackageReference>

  <!-- For testing async code -->
  <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
</ItemGroup>
```

### Test Project Structure

```
/YourApp.Tests
    ├── /Presenters
    │   ├── CustomerPresenterTests.cs
    │   ├── OrderPresenterTests.cs
    │   └── ProductPresenterTests.cs
    ├── /Services
    │   ├── CustomerServiceTests.cs
    │   ├── OrderServiceTests.cs
    │   └── PricingServiceTests.cs
    ├── /Repositories
    │   └── CustomerRepositoryTests.cs  (integration test)
    ├── /Utilities
    │   ├── ValidationHelperTests.cs
    │   └── StringExtensionsTests.cs
    ├── /ViewModels
    │   └── CustomerViewModelTests.cs
    └── TestFixtures/
        └── DatabaseFixture.cs
```

### Test Class Template

```csharp
using Xunit;
using Moq;
using FluentAssertions;
using YourApp.Services;
using YourApp.Models;

namespace YourApp.Tests.Services
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _mockRepository;
        private readonly Mock<ILogger<CustomerService>> _mockLogger;
        private readonly CustomerService _service;

        public CustomerServiceTests()
        {
            // Arrange - Common setup for all tests
            _mockRepository = new Mock<ICustomerRepository>();
            _mockLogger = new Mock<ILogger<CustomerService>>();
            _service = new CustomerService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task MethodName_Scenario_ExpectedResult()
        {
            // Arrange - Test-specific setup
            var customer = new Customer { Id = 1, Name = "Test" };
            _mockRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(customer);

            // Act
            var result = await _service.GetCustomerAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test", result.Name);
        }
    }
}
```

---

## Test Anatomy

### Arrange-Act-Assert Pattern

Every test should follow the **AAA pattern**:

```csharp
[Fact]
public async Task SaveCustomer_ValidCustomer_SavesSuccessfully()
{
    // ============ ARRANGE ============
    // Set up test data, mocks, and expectations
    var customer = new Customer
    {
        Name = "John Doe",
        Email = "john@test.com"
    };

    _mockRepository
        .Setup(r => r.SaveAsync(It.IsAny<Customer>()))
        .ReturnsAsync(123); // Simulated new ID

    // ============ ACT ============
    // Execute the method under test
    var customerId = await _service.SaveCustomerAsync(customer);

    // ============ ASSERT ============
    // Verify the expected outcome
    Assert.Equal(123, customerId);
    _mockRepository.Verify(
        r => r.SaveAsync(It.Is<Customer>(c => c.Name == "John Doe")),
        Times.Once
    );
}
```

**Why AAA?**
- Separates concerns clearly
- Makes tests easier to read
- Forces you to think about test structure
- Easier to maintain

### Test Naming Convention

Use the pattern: **`MethodName_Scenario_ExpectedResult`**

```csharp
// ✅ GOOD NAMES
public void SaveCustomer_ValidCustomer_ReturnsCustomerId()
public void SaveCustomer_NullCustomer_ThrowsArgumentNullException()
public void SaveCustomer_DuplicateEmail_ThrowsValidationException()
public void GetCustomer_NonExistentId_ReturnsNull()
public void CalculateDiscount_VIPCustomer_Returns20PercentDiscount()

// ❌ BAD NAMES
public void TestSave()  // What scenario? What's expected?
public void Test1()     // Meaningless
public void SaveWorks() // What's the scenario?
public void CustomerTest() // What about customers?
```

**Benefits of good naming:**
- Test failure tells you exactly what broke
- Tests serve as documentation
- No need to read test body to understand intent

### [Fact] vs [Theory]

#### [Fact] - Single Test Case

Use `[Fact]` for single scenario tests:

```csharp
[Fact]
public void CalculateTotal_EmptyOrder_ReturnsZero()
{
    var order = new Order { Items = new List<OrderItem>() };
    var total = _service.CalculateTotal(order);
    Assert.Equal(0m, total);
}
```

#### [Theory] - Parameterized Tests

Use `[Theory]` with `[InlineData]` to test multiple scenarios with same logic:

```csharp
[Theory]
[InlineData("valid@email.com", true)]
[InlineData("user@domain.co.uk", true)]
[InlineData("user+tag@domain.com", true)]
[InlineData("", false)]
[InlineData("invalid", false)]
[InlineData("@nodomain.com", false)]
[InlineData("no@domain", false)]
public void IsValidEmail_VariousInputs_ReturnsExpected(string email, bool expected)
{
    // Act
    var result = EmailValidator.IsValid(email);

    // Assert
    Assert.Equal(expected, result);
}
```

**Benefits of [Theory]:**
- Test multiple scenarios without duplication
- Easy to add new test cases (just add [InlineData])
- Clear which inputs produce which outputs

#### [Theory] with Complex Data

Use `[MemberData]` for complex objects:

```csharp
public class PricingTests
{
    [Theory]
    [MemberData(nameof(GetPricingScenarios))]
    public void CalculatePrice_VariousScenarios_ReturnsExpected(
        Order order, decimal expected)
    {
        var result = _service.CalculatePrice(order);
        Assert.Equal(expected, result);
    }

    public static IEnumerable<object[]> GetPricingScenarios()
    {
        yield return new object[]
        {
            new Order { Items = new[] { new OrderItem { Price = 100, Qty = 2 } } },
            200m
        };
        yield return new object[]
        {
            new Order
            {
                Items = new[] { new OrderItem { Price = 100, Qty = 2 } },
                Discount = 0.1m
            },
            180m
        };
    }
}
```

---

## Mocking with Moq

### Why Mock?

**Mocking** replaces real dependencies with test doubles to:
1. **Isolate** the unit under test
2. **Control** dependency behavior
3. **Verify** interactions
4. **Speed up** tests (no real database/network calls)

### Creating Mocks

```csharp
// Basic mock creation
var mockRepository = new Mock<ICustomerRepository>();
var mockLogger = new Mock<ILogger<CustomerService>>();

// Use mock in constructor
var service = new CustomerService(
    mockRepository.Object,  // .Object gets the actual interface instance
    mockLogger.Object
);
```

### Setup Method Calls

#### Simple Return Values

```csharp
// Setup method to return specific value
_mockRepository
    .Setup(r => r.GetByIdAsync(1))
    .ReturnsAsync(new Customer { Id = 1, Name = "John" });

// Setup with any parameter
_mockRepository
    .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
    .ReturnsAsync(new Customer { Id = 1, Name = "John" });

// Setup with parameter condition
_mockRepository
    .Setup(r => r.GetByIdAsync(It.Is<int>(id => id > 0)))
    .ReturnsAsync(new Customer { Id = 1, Name = "John" });
```

#### Different Returns for Different Inputs

```csharp
_mockRepository
    .Setup(r => r.GetByIdAsync(1))
    .ReturnsAsync(new Customer { Id = 1, Name = "Alice" });

_mockRepository
    .Setup(r => r.GetByIdAsync(2))
    .ReturnsAsync(new Customer { Id = 2, Name = "Bob" });

_mockRepository
    .Setup(r => r.GetByIdAsync(999))
    .ReturnsAsync((Customer)null); // Not found
```

#### Async Methods

```csharp
// ReturnsAsync for async methods
_mockService
    .Setup(s => s.SaveAsync(It.IsAny<Customer>()))
    .ReturnsAsync(123);

// Returns for sync methods
_mockService
    .Setup(s => s.Validate(It.IsAny<Customer>()))
    .Returns(true);
```

### Setup Properties

```csharp
// Setup property getter
var mockView = new Mock<ICustomerView>();
mockView.Setup(v => v.CustomerName).Returns("John");

// Setup property to track sets
mockView.SetupProperty(v => v.IsLoading);

// Later in test:
mockView.Object.IsLoading = true;
Assert.True(mockView.Object.IsLoading);
```

### Mock Exceptions

```csharp
// Throw synchronous exception
_mockRepository
    .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
    .Throws<InvalidOperationException>();

// Throw async exception
_mockRepository
    .Setup(r => r.SaveAsync(It.IsAny<Customer>()))
    .ThrowsAsync(new DbUpdateException("Database error"));

// Throw with message
_mockRepository
    .Setup(r => r.SaveAsync(It.IsAny<Customer>()))
    .Throws(new ValidationException("Email already exists"));
```

### Verify Method Calls

```csharp
// Verify method was called once
_mockRepository.Verify(
    r => r.SaveAsync(It.IsAny<Customer>()),
    Times.Once
);

// Verify method was never called
_mockRepository.Verify(
    r => r.DeleteAsync(It.IsAny<int>()),
    Times.Never
);

// Verify with specific arguments
_mockRepository.Verify(
    r => r.SaveAsync(It.Is<Customer>(c => c.Name == "John")),
    Times.Once
);

// Verify property was set
_mockView.VerifySet(v => v.Customers = It.IsAny<List<Customer>>(), Times.Once);

// Verify multiple calls
_mockLogger.Verify(
    l => l.LogInformation(It.IsAny<string>()),
    Times.Exactly(3)
);
```

### Verify Call Order (Sequential)

```csharp
[Fact]
public async Task LoadCustomers_CallsMethodsInCorrectOrder()
{
    var sequence = new MockSequence();

    _mockView.InSequence(sequence).Setup(v => v.ShowLoading(true));
    _mockService.InSequence(sequence).Setup(s => s.GetAllAsync())
        .ReturnsAsync(new List<Customer>());
    _mockView.InSequence(sequence).Setup(v => v.ShowLoading(false));

    await _presenter.LoadCustomersAsync();

    // If methods called out of order, test fails
}
```

---

## Testing Presenters

### Complete Presenter Test Class

```csharp
using Xunit;
using Moq;
using FluentAssertions;
using YourApp.Presenters;
using YourApp.Views;
using YourApp.Services;
using YourApp.Models;

namespace YourApp.Tests.Presenters
{
    public class CustomerPresenterTests
    {
        private readonly Mock<ICustomerView> _mockView;
        private readonly Mock<ICustomerService> _mockService;
        private readonly Mock<ILogger<CustomerPresenter>> _mockLogger;
        private readonly CustomerPresenter _presenter;

        public CustomerPresenterTests()
        {
            _mockView = new Mock<ICustomerView>();
            _mockService = new Mock<ICustomerService>();
            _mockLogger = new Mock<ILogger<CustomerPresenter>>();

            _presenter = new CustomerPresenter(
                _mockView.Object,
                _mockService.Object,
                _mockLogger.Object
            );
        }

        #region LoadAsync Tests

        [Fact]
        public async Task LoadAsync_Success_DisplaysCustomers()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "Alice", Email = "alice@test.com" },
                new Customer { Id = 2, Name = "Bob", Email = "bob@test.com" }
            };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(customers);

            // Act
            await _presenter.LoadAsync();

            // Assert
            _mockView.VerifySet(v => v.Customers = customers, Times.Once);
            _mockView.VerifySet(v => v.IsLoading = true, Times.Once);
            _mockView.VerifySet(v => v.IsLoading = false, Times.Once);
        }

        [Fact]
        public async Task LoadAsync_ServiceThrowsException_DisplaysErrorMessage()
        {
            // Arrange
            var exception = new Exception("Database connection failed");
            _mockService.Setup(s => s.GetAllAsync()).ThrowsAsync(exception);

            // Act
            await _presenter.LoadAsync();

            // Assert
            _mockView.Verify(
                v => v.ShowError("Failed to load customers: Database connection failed"),
                Times.Once
            );
            _mockView.VerifySet(v => v.IsLoading = false, Times.Once);
            _mockLogger.Verify(
                l => l.LogError(exception, It.IsAny<string>()),
                Times.Once
            );
        }

        [Fact]
        public async Task LoadAsync_NoCustomers_DisplaysEmptyList()
        {
            // Arrange
            _mockService.Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<Customer>());

            // Act
            await _presenter.LoadAsync();

            // Assert
            _mockView.VerifySet(v => v.Customers = It.Is<List<Customer>>(
                list => list.Count == 0),
                Times.Once
            );
        }

        #endregion

        #region SaveAsync Tests

        [Fact]
        public async Task SaveAsync_ValidCustomer_SavesAndReloads()
        {
            // Arrange
            var customer = new Customer
            {
                Name = "John Doe",
                Email = "john@test.com"
            };
            _mockView.Setup(v => v.GetCustomerData()).Returns(customer);
            _mockService.Setup(s => s.SaveAsync(customer)).ReturnsAsync(123);
            _mockService.Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<Customer> { customer });

            // Act
            await _presenter.SaveAsync();

            // Assert
            _mockService.Verify(s => s.SaveAsync(customer), Times.Once);
            _mockView.Verify(v => v.ShowSuccess("Customer saved successfully"), Times.Once);
            _mockView.Verify(v => v.ClearForm(), Times.Once);
            _mockService.Verify(s => s.GetAllAsync(), Times.Once); // Reloaded
        }

        [Fact]
        public async Task SaveAsync_InvalidCustomer_DisplaysValidationError()
        {
            // Arrange
            var customer = new Customer { Name = "", Email = "invalid" };
            _mockView.Setup(v => v.GetCustomerData()).Returns(customer);
            _mockService.Setup(s => s.SaveAsync(customer))
                .ThrowsAsync(new ValidationException("Name is required"));

            // Act
            await _presenter.SaveAsync();

            // Assert
            _mockView.Verify(
                v => v.ShowValidationError("Name is required"),
                Times.Once
            );
            _mockService.Verify(s => s.GetAllAsync(), Times.Never); // Not reloaded
        }

        [Fact]
        public async Task SaveAsync_NullCustomer_ThrowsArgumentNullException()
        {
            // Arrange
            _mockView.Setup(v => v.GetCustomerData()).Returns((Customer)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _presenter.SaveAsync()
            );
        }

        [Fact]
        public async Task SaveAsync_DuplicateEmail_DisplaysErrorMessage()
        {
            // Arrange
            var customer = new Customer
            {
                Name = "John",
                Email = "existing@test.com"
            };
            _mockView.Setup(v => v.GetCustomerData()).Returns(customer);
            _mockService.Setup(s => s.SaveAsync(customer))
                .ThrowsAsync(new DuplicateEmailException("Email already exists"));

            // Act
            await _presenter.SaveAsync();

            // Assert
            _mockView.Verify(
                v => v.ShowError("Email already exists"),
                Times.Once
            );
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_UserConfirms_DeletesCustomer()
        {
            // Arrange
            var customerId = 123;
            _mockView.Setup(v => v.ConfirmDelete()).Returns(true);

            // Act
            await _presenter.DeleteAsync(customerId);

            // Assert
            _mockService.Verify(s => s.DeleteAsync(customerId), Times.Once);
            _mockView.Verify(v => v.ShowSuccess("Customer deleted"), Times.Once);
            _mockService.Verify(s => s.GetAllAsync(), Times.Once); // Reloaded
        }

        [Fact]
        public async Task DeleteAsync_UserCancels_DoesNotDelete()
        {
            // Arrange
            var customerId = 123;
            _mockView.Setup(v => v.ConfirmDelete()).Returns(false);

            // Act
            await _presenter.DeleteAsync(customerId);

            // Assert
            _mockService.Verify(s => s.DeleteAsync(It.IsAny<int>()), Times.Never);
            _mockService.Verify(s => s.GetAllAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ServiceFails_DisplaysError()
        {
            // Arrange
            var customerId = 123;
            _mockView.Setup(v => v.ConfirmDelete()).Returns(true);
            _mockService.Setup(s => s.DeleteAsync(customerId))
                .ThrowsAsync(new Exception("Cannot delete customer with orders"));

            // Act
            await _presenter.DeleteAsync(customerId);

            // Assert
            _mockView.Verify(
                v => v.ShowError(It.Is<string>(msg =>
                    msg.Contains("Cannot delete customer with orders"))),
                Times.Once
            );
        }

        #endregion

        #region SearchAsync Tests

        [Fact]
        public async Task SearchAsync_WithSearchTerm_FiltersCustomers()
        {
            // Arrange
            var searchTerm = "Alice";
            var allCustomers = new List<Customer>
            {
                new Customer { Id = 1, Name = "Alice" },
                new Customer { Id = 2, Name = "Bob" },
                new Customer { Id = 3, Name = "Alice Cooper" }
            };
            var filteredCustomers = allCustomers
                .Where(c => c.Name.Contains(searchTerm))
                .ToList();

            _mockService.Setup(s => s.SearchAsync(searchTerm))
                .ReturnsAsync(filteredCustomers);

            // Act
            await _presenter.SearchAsync(searchTerm);

            // Assert
            _mockView.VerifySet(v => v.Customers = It.Is<List<Customer>>(
                list => list.Count == 2 && list.All(c => c.Name.Contains("Alice"))),
                Times.Once
            );
        }

        [Fact]
        public async Task SearchAsync_EmptySearchTerm_LoadsAllCustomers()
        {
            // Arrange
            var allCustomers = new List<Customer>
            {
                new Customer { Id = 1, Name = "Alice" },
                new Customer { Id = 2, Name = "Bob" }
            };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(allCustomers);

            // Act
            await _presenter.SearchAsync("");

            // Assert
            _mockService.Verify(s => s.GetAllAsync(), Times.Once);
            _mockService.Verify(s => s.SearchAsync(It.IsAny<string>()), Times.Never);
        }

        #endregion
    }
}
```

---

## Testing Services

### Complete Service Test Class

```csharp
using Xunit;
using Moq;
using FluentAssertions;
using YourApp.Services;
using YourApp.Repositories;
using YourApp.Models;

namespace YourApp.Tests.Services
{
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

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ReturnsAllCustomers()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "Alice" },
                new Customer { Id = 2, Name = "Bob" }
            };
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(customers);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(c => c.Name == "Alice");
            _mockLogger.Verify(
                l => l.LogInformation("Retrieved {Count} customers", 2),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAllAsync_RepositoryThrows_LogsAndRethrows()
        {
            // Arrange
            var exception = new Exception("Database error");
            _mockRepository.Setup(r => r.GetAllAsync()).ThrowsAsync(exception);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.GetAllAsync());
            _mockLogger.Verify(
                l => l.LogError(exception, It.IsAny<string>()),
                Times.Once
            );
        }

        #endregion

        #region SaveAsync Tests

        [Fact]
        public async Task SaveAsync_ValidCustomer_SavesSuccessfully()
        {
            // Arrange
            var customer = new Customer
            {
                Name = "John Doe",
                Email = "john@test.com",
                Phone = "555-1234"
            };
            _mockRepository.Setup(r => r.SaveAsync(customer)).ReturnsAsync(123);

            // Act
            var customerId = await _service.SaveAsync(customer);

            // Assert
            Assert.Equal(123, customerId);
            _mockRepository.Verify(r => r.SaveAsync(customer), Times.Once);
            _mockLogger.Verify(
                l => l.LogInformation("Saved customer: {CustomerName}", "John Doe"),
                Times.Once
            );
        }

        [Fact]
        public async Task SaveAsync_NullCustomer_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _service.SaveAsync(null)
            );
            _mockRepository.Verify(r => r.SaveAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task SaveAsync_EmptyName_ThrowsValidationException()
        {
            // Arrange
            var customer = new Customer
            {
                Name = "",
                Email = "test@test.com"
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ValidationException>(
                () => _service.SaveAsync(customer)
            );
            Assert.Contains("Name is required", ex.Message);
        }

        [Fact]
        public async Task SaveAsync_InvalidEmail_ThrowsValidationException()
        {
            // Arrange
            var customer = new Customer
            {
                Name = "John",
                Email = "invalid-email"
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ValidationException>(
                () => _service.SaveAsync(customer)
            );
            Assert.Contains("Invalid email format", ex.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task SaveAsync_InvalidName_ThrowsValidationException(string name)
        {
            // Arrange
            var customer = new Customer
            {
                Name = name,
                Email = "test@test.com"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(
                () => _service.SaveAsync(customer)
            );
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ExistingCustomer_DeletesSuccessfully()
        {
            // Arrange
            var customerId = 123;
            _mockRepository.Setup(r => r.GetByIdAsync(customerId))
                .ReturnsAsync(new Customer { Id = customerId });
            _mockRepository.Setup(r => r.DeleteAsync(customerId))
                .Returns(Task.CompletedTask);

            // Act
            await _service.DeleteAsync(customerId);

            // Assert
            _mockRepository.Verify(r => r.DeleteAsync(customerId), Times.Once);
            _mockLogger.Verify(
                l => l.LogInformation("Deleted customer: {CustomerId}", customerId),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteAsync_NonExistentCustomer_ThrowsNotFoundException()
        {
            // Arrange
            var customerId = 999;
            _mockRepository.Setup(r => r.GetByIdAsync(customerId))
                .ReturnsAsync((Customer)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => _service.DeleteAsync(customerId)
            );
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_CustomerWithOrders_ThrowsBusinessException()
        {
            // Arrange
            var customerId = 123;
            var customer = new Customer
            {
                Id = customerId,
                Orders = new List<Order> { new Order() } // Has orders
            };
            _mockRepository.Setup(r => r.GetByIdAsync(customerId))
                .ReturnsAsync(customer);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<BusinessRuleException>(
                () => _service.DeleteAsync(customerId)
            );
            Assert.Contains("Cannot delete customer with existing orders", ex.Message);
        }

        #endregion

        #region Business Logic Tests

        [Fact]
        public void CalculateDiscount_RegularCustomer_Returns5Percent()
        {
            // Arrange
            var customer = new Customer
            {
                TotalOrders = 5,
                CustomerType = CustomerType.Regular
            };

            // Act
            var discount = _service.CalculateDiscount(customer);

            // Assert
            Assert.Equal(0.05m, discount);
        }

        [Fact]
        public void CalculateDiscount_VIPCustomer_Returns20Percent()
        {
            // Arrange
            var customer = new Customer
            {
                TotalOrders = 50,
                CustomerType = CustomerType.VIP
            };

            // Act
            var discount = _service.CalculateDiscount(customer);

            // Assert
            Assert.Equal(0.20m, discount);
        }

        [Theory]
        [InlineData(0, 0)]      // No orders = no discount
        [InlineData(5, 0.05)]   // 5 orders = 5%
        [InlineData(10, 0.10)]  // 10 orders = 10%
        [InlineData(25, 0.15)]  // 25 orders = 15%
        [InlineData(50, 0.20)]  // 50+ orders = 20% (max)
        [InlineData(100, 0.20)] // 100 orders = 20% (max)
        public void CalculateDiscountByOrders_VariousOrderCounts_ReturnsExpectedDiscount(
            int orderCount, decimal expectedDiscount)
        {
            // Arrange
            var customer = new Customer { TotalOrders = orderCount };

            // Act
            var discount = _service.CalculateDiscountByOrders(customer);

            // Assert
            Assert.Equal(expectedDiscount, discount);
        }

        #endregion
    }
}
```

---

## Advanced Testing Techniques

### Testing with FluentAssertions

FluentAssertions provides more readable assertions:

```csharp
// xUnit basic assertions
Assert.Equal(3, list.Count);
Assert.True(customer.IsActive);
Assert.Contains("Error", message);

// FluentAssertions - more readable!
list.Should().HaveCount(3);
customer.IsActive.Should().BeTrue();
message.Should().Contain("Error");

// Complex assertions
customers.Should()
    .HaveCount(5)
    .And.OnlyContain(c => c.IsActive)
    .And.Contain(c => c.Name == "Alice");

// Exception assertions
Action act = () => service.SaveCustomer(null);
act.Should().Throw<ArgumentNullException>()
    .WithMessage("*customer*");

// Collection assertions
result.Should().BeEquivalentTo(expected, options =>
    options.Excluding(c => c.CreatedDate));
```

### Testing Events

```csharp
[Fact]
public void CustomerChanged_WhenCustomerSaved_RaisesEvent()
{
    // Arrange
    var eventRaised = false;
    Customer eventCustomer = null;

    _service.CustomerChanged += (sender, customer) =>
    {
        eventRaised = true;
        eventCustomer = customer;
    };

    var customer = new Customer { Name = "John" };

    // Act
    _service.SaveCustomer(customer);

    // Assert
    Assert.True(eventRaised);
    Assert.Same(customer, eventCustomer);
}

// Using FluentAssertions for events
[Fact]
public void PropertyChanged_WhenNameSet_RaisesEvent()
{
    // Arrange
    var viewModel = new CustomerViewModel();
    using var monitor = viewModel.Monitor();

    // Act
    viewModel.Name = "John";

    // Assert
    monitor.Should().RaisePropertyChangeFor(vm => vm.Name);
}
```

### Testing IDisposable

```csharp
[Fact]
public void Dispose_DisposesAllResources()
{
    // Arrange
    var mockDisposableService = new Mock<IDisposableService>();
    var presenter = new CustomerPresenter(
        Mock.Of<ICustomerView>(),
        mockDisposableService.Object
    );

    // Act
    presenter.Dispose();

    // Assert
    mockDisposableService.Verify(s => s.Dispose(), Times.Once);
}

[Fact]
public void Dispose_CalledMultipleTimes_OnlyDisposesOnce()
{
    // Arrange
    var mockService = new Mock<IDisposableService>();
    var presenter = new CustomerPresenter(
        Mock.Of<ICustomerView>(),
        mockService.Object
    );

    // Act
    presenter.Dispose();
    presenter.Dispose(); // Second call

    // Assert
    mockService.Verify(s => s.Dispose(), Times.Once); // Only once
}
```

### AutoFixture for Test Data

AutoFixture generates test data automatically:

```csharp
// Install: Install-Package AutoFixture.Xunit2

[Theory, AutoData]
public void SaveCustomer_ValidCustomer_SavesSuccessfully(
    Customer customer,  // AutoFixture generates this
    int expectedId)     // And this
{
    // Arrange
    _mockRepository.Setup(r => r.SaveAsync(customer))
        .ReturnsAsync(expectedId);

    // Act
    var result = await _service.SaveAsync(customer);

    // Assert
    Assert.Equal(expectedId, result);
}

// Customize generation
[Theory, AutoData]
public void ProcessOrder_ValidOrder_ProcessesSuccessfully(
    [Frozen] Mock<IOrderRepository> mockRepo,  // Frozen = used everywhere
    OrderService service,                      // Uses mockRepo
    Order order)
{
    // AutoFixture injects the mock into the service
    mockRepo.Setup(r => r.SaveAsync(order)).ReturnsAsync(123);

    var result = await service.ProcessAsync(order);

    Assert.Equal(123, result);
}
```

---

## Best Practices

### ✅ DO: Test One Thing Per Test

```csharp
// ✅ GOOD - Each test has single responsibility
[Fact]
public void SaveCustomer_ValidCustomer_CallsRepository()
{
    _service.SaveCustomer(customer);
    _mockRepo.Verify(r => r.Save(customer), Times.Once);
}

[Fact]
public void SaveCustomer_ValidCustomer_LogsInformation()
{
    _service.SaveCustomer(customer);
    _mockLogger.Verify(l => l.LogInformation(It.IsAny<string>()), Times.Once);
}

// ❌ BAD - Testing multiple things
[Fact]
public void SaveCustomer_DoesEverything()
{
    _service.SaveCustomer(customer);
    _mockRepo.Verify(r => r.Save(customer), Times.Once);
    _mockLogger.Verify(l => l.LogInformation(It.IsAny<string>()), Times.Once);
    Assert.True(_service.HasChanges);
    // Too much in one test!
}
```

### ✅ DO: Use Descriptive Test Names

```csharp
// ✅ GOOD
[Fact]
public void GetCustomer_NonExistentId_ReturnsNull()

[Fact]
public void SaveCustomer_DuplicateEmail_ThrowsValidationException()

// ❌ BAD
[Fact]
public void Test1()

[Fact]
public void TestCustomer()
```

### ✅ DO: Test Both Success and Failure Paths

```csharp
// ✅ Test success
[Fact]
public async Task SaveAsync_ValidCustomer_ReturnsId()
{
    var result = await _service.SaveAsync(validCustomer);
    Assert.True(result > 0);
}

// ✅ Test validation failure
[Fact]
public async Task SaveAsync_InvalidEmail_ThrowsValidationException()
{
    await Assert.ThrowsAsync<ValidationException>(
        () => _service.SaveAsync(invalidCustomer));
}

// ✅ Test infrastructure failure
[Fact]
public async Task SaveAsync_DatabaseError_ThrowsException()
{
    _mockRepo.Setup(r => r.SaveAsync(It.IsAny<Customer>()))
        .ThrowsAsync(new DbException());
    await Assert.ThrowsAsync<DbException>(
        () => _service.SaveAsync(validCustomer));
}
```

### ✅ DO: Keep Tests Independent

```csharp
// ✅ GOOD - Each test sets up its own data
public class CustomerServiceTests
{
    [Fact]
    public void Test1()
    {
        var customer = new Customer { Name = "Test1" };
        // Test using customer
    }

    [Fact]
    public void Test2()
    {
        var customer = new Customer { Name = "Test2" };
        // Independent from Test1
    }
}

// ❌ BAD - Tests depend on shared state
public class CustomerServiceTests
{
    private Customer _sharedCustomer = new Customer(); // Shared!

    [Fact]
    public void Test1()
    {
        _sharedCustomer.Name = "Changed"; // Affects Test2!
    }

    [Fact]
    public void Test2()
    {
        // Depends on Test1 not running first
        Assert.Null(_sharedCustomer.Name);
    }
}
```

### ✅ DO: Use Theory for Multiple Similar Tests

```csharp
// ✅ GOOD - DRY principle
[Theory]
[InlineData("", false)]
[InlineData("invalid", false)]
[InlineData("test@test.com", true)]
public void IsValidEmail_VariousInputs_ReturnsExpected(string email, bool expected)
{
    var result = EmailValidator.IsValid(email);
    Assert.Equal(expected, result);
}

// ❌ BAD - Repetitive
[Fact]
public void IsValidEmail_EmptyString_ReturnsFalse() { /* ... */ }

[Fact]
public void IsValidEmail_InvalidFormat_ReturnsFalse() { /* ... */ }

[Fact]
public void IsValidEmail_ValidEmail_ReturnsTrue() { /* ... */ }
```

### ✅ DO: Mock External Dependencies

```csharp
// ✅ GOOD - Mock database
_mockRepository.Setup(r => r.GetAllAsync())
    .ReturnsAsync(testData);

// ❌ BAD - Real database
var context = new MyDbContext(); // Real connection!
var repository = new CustomerRepository(context);
```

### ✅ DO: Verify Important Interactions

```csharp
// ✅ Verify critical operations
_mockRepository.Verify(r => r.SaveAsync(customer), Times.Once);
_mockLogger.Verify(l => l.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);

// ❌ Don't over-verify
_mockLogger.Verify(l => l.LogDebug(It.IsAny<string>()), Times.Exactly(37));
// Too brittle - breaks with implementation changes
```

### ✅ DO: Use Constructor for Common Setup

```csharp
// ✅ GOOD - Setup in constructor
public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _mockRepo;
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        _mockRepo = new Mock<ICustomerRepository>();
        _service = new CustomerService(_mockRepo.Object);
    }

    [Fact]
    public void Test1() { /* Use _service */ }

    [Fact]
    public void Test2() { /* Use _service */ }
}
```

### ❌ DON'T: Test Implementation Details

```csharp
// ❌ BAD - Testing private method
[Fact]
public void PrivateHelper_ReturnsCorrectValue()
{
    var method = typeof(Service).GetMethod("PrivateHelper",
        BindingFlags.NonPublic | BindingFlags.Instance);
    var result = method.Invoke(_service, new object[] { 42 });
    // Brittle and tests implementation
}

// ✅ GOOD - Test public API
[Fact]
public void PublicMethod_CallsPrivateHelperIndirectly_ReturnsExpected()
{
    var result = _service.PublicMethod(42);
    Assert.Equal(expected, result);
}
```

### ❌ DON'T: Use Real External Resources

```csharp
// ❌ BAD
[Fact]
public void SaveCustomer_SavesToRealDatabase()
{
    var context = new ProductionDbContext(); // Real DB!
    var repo = new CustomerRepository(context);
    // Slow, brittle, requires DB setup
}

// ❌ BAD
[Fact]
public void DownloadFile_DownloadsFromRealServer()
{
    var service = new FileService();
    var file = service.Download("http://real-server.com/file"); // Real HTTP!
    // Slow, requires network, brittle
}
```

### ❌ DON'T: Test Framework Code

```csharp
// ❌ DON'T TEST .NET
[Fact]
public void List_Add_IncreasesCount()
{
    var list = new List<int>();
    list.Add(1);
    Assert.Equal(1, list.Count);
}

// ❌ DON'T TEST ENTITY FRAMEWORK
[Fact]
public void DbContext_SaveChanges_PersistsData()
{
    var context = new MyDbContext();
    context.SaveChanges(); // Testing EF Core
}
```

### ❌ DON'T: Write Flaky Tests

```csharp
// ❌ BAD - Timing dependent
[Fact]
public async Task ProcessAsync_CompletesInTime()
{
    var task = _service.ProcessAsync();
    await Task.Delay(100); // Flaky!
    Assert.True(task.IsCompleted); // Might fail randomly
}

// ✅ GOOD - Wait properly
[Fact]
public async Task ProcessAsync_Completes()
{
    await _service.ProcessAsync();
    Assert.True(true); // If we get here, it completed
}
```

### ❌ DON'T: Catch Exceptions in Tests

```csharp
// ❌ BAD
[Fact]
public void SaveCustomer_InvalidData_ThrowsException()
{
    try
    {
        _service.SaveCustomer(invalid);
        Assert.True(false, "Should have thrown");
    }
    catch (ValidationException)
    {
        Assert.True(true);
    }
}

// ✅ GOOD
[Fact]
public void SaveCustomer_InvalidData_ThrowsException()
{
    Assert.Throws<ValidationException>(() =>
        _service.SaveCustomer(invalid));
}
```

---

## Complete Working Examples

### Example 1: Full Presenter Test Suite

```csharp
namespace YourApp.Tests.Presenters
{
    /// <summary>
    /// Complete test suite for OrderPresenter showing all test patterns
    /// </summary>
    public class OrderPresenterTests : IDisposable
    {
        private readonly Mock<IOrderView> _mockView;
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly Mock<ICustomerService> _mockCustomerService;
        private readonly Mock<ILogger<OrderPresenter>> _mockLogger;
        private readonly OrderPresenter _presenter;

        public OrderPresenterTests()
        {
            _mockView = new Mock<IOrderView>();
            _mockOrderService = new Mock<IOrderService>();
            _mockCustomerService = new Mock<ICustomerService>();
            _mockLogger = new Mock<ILogger<OrderPresenter>>();

            _presenter = new OrderPresenter(
                _mockView.Object,
                _mockOrderService.Object,
                _mockCustomerService.Object,
                _mockLogger.Object
            );
        }

        #region Initialize Tests

        [Fact]
        public async Task Initialize_LoadsCustomersAndOrders()
        {
            // Arrange
            var customers = GetTestCustomers();
            var orders = GetTestOrders();
            _mockCustomerService.Setup(s => s.GetAllAsync()).ReturnsAsync(customers);
            _mockOrderService.Setup(s => s.GetAllAsync()).ReturnsAsync(orders);

            // Act
            await _presenter.InitializeAsync();

            // Assert
            _mockView.VerifySet(v => v.Customers = customers, Times.Once);
            _mockView.VerifySet(v => v.Orders = orders, Times.Once);
            _mockView.VerifySet(v => v.IsLoading = true, Times.Once);
            _mockView.VerifySet(v => v.IsLoading = false, Times.Once);
        }

        #endregion

        #region CreateOrder Tests

        [Fact]
        public async Task CreateOrder_ValidOrder_CreatesSuccessfully()
        {
            // Arrange
            var order = new Order
            {
                CustomerId = 1,
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 2, Price = 50 }
                }
            };
            _mockView.Setup(v => v.GetOrderData()).Returns(order);
            _mockOrderService.Setup(s => s.CreateAsync(order)).ReturnsAsync(100);
            _mockOrderService.Setup(s => s.GetAllAsync()).ReturnsAsync(GetTestOrders());

            // Act
            await _presenter.CreateOrderAsync();

            // Assert
            _mockOrderService.Verify(s => s.CreateAsync(order), Times.Once);
            _mockView.Verify(v => v.ShowSuccess("Order created successfully"), Times.Once);
            _mockView.Verify(v => v.ClearForm(), Times.Once);
        }

        [Fact]
        public async Task CreateOrder_EmptyItems_ShowsValidationError()
        {
            // Arrange
            var order = new Order
            {
                CustomerId = 1,
                Items = new List<OrderItem>() // Empty!
            };
            _mockView.Setup(v => v.GetOrderData()).Returns(order);

            // Act
            await _presenter.CreateOrderAsync();

            // Assert
            _mockView.Verify(v => v.ShowValidationError(
                It.Is<string>(msg => msg.Contains("at least one item"))),
                Times.Once);
            _mockOrderService.Verify(s => s.CreateAsync(It.IsAny<Order>()), Times.Never);
        }

        #endregion

        #region CalculateTotal Tests

        [Fact]
        public void CalculateTotal_UpdatesViewWithCorrectTotal()
        {
            // Arrange
            var items = new List<OrderItem>
            {
                new OrderItem { Quantity = 2, Price = 50 },   // $100
                new OrderItem { Quantity = 1, Price = 30 }    // $30
            };
            _mockView.Setup(v => v.GetOrderItems()).Returns(items);

            // Act
            _presenter.CalculateTotal();

            // Assert
            _mockView.VerifySet(v => v.Total = 130m, Times.Once);
        }

        [Theory]
        [MemberData(nameof(GetTotalCalculationScenarios))]
        public void CalculateTotal_VariousScenarios_CalculatesCorrectly(
            List<OrderItem> items,
            decimal discount,
            decimal expectedTotal)
        {
            // Arrange
            _mockView.Setup(v => v.GetOrderItems()).Returns(items);
            _mockView.Setup(v => v.DiscountPercent).Returns(discount);

            // Act
            _presenter.CalculateTotal();

            // Assert
            _mockView.VerifySet(v => v.Total = expectedTotal, Times.Once);
        }

        public static IEnumerable<object[]> GetTotalCalculationScenarios()
        {
            // No discount
            yield return new object[]
            {
                new List<OrderItem>
                {
                    new OrderItem { Quantity = 2, Price = 50 }
                },
                0m,
                100m
            };

            // With 10% discount
            yield return new object[]
            {
                new List<OrderItem>
                {
                    new OrderItem { Quantity = 2, Price = 50 }
                },
                0.1m,
                90m
            };

            // Multiple items with discount
            yield return new object[]
            {
                new List<OrderItem>
                {
                    new OrderItem { Quantity = 2, Price = 50 },
                    new OrderItem { Quantity = 1, Price = 30 }
                },
                0.1m,
                117m  // (100 + 30) * 0.9
            };
        }

        #endregion

        // Helper methods
        private List<Customer> GetTestCustomers()
        {
            return new List<Customer>
            {
                new Customer { Id = 1, Name = "Alice" },
                new Customer { Id = 2, Name = "Bob" }
            };
        }

        private List<Order> GetTestOrders()
        {
            return new List<Order>
            {
                new Order { Id = 1, CustomerId = 1, Total = 100 },
                new Order { Id = 2, CustomerId = 2, Total = 200 }
            };
        }

        public void Dispose()
        {
            _presenter?.Dispose();
        }
    }
}
```

---

## Running Tests

### Visual Studio Test Explorer

1. **Open Test Explorer**: `Test` → `Test Explorer` (or `Ctrl+E, T`)
2. **Run All Tests**: Click "Run All" button
3. **Run Specific Test**: Right-click test → "Run"
4. **Debug Test**: Right-click test → "Debug"
5. **Group Tests**: By class, namespace, or outcome

### Command Line

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run specific test class
dotnet test --filter "FullyQualifiedName~CustomerPresenterTests"

# Run specific test method
dotnet test --filter "FullyQualifiedName~CustomerPresenterTests.LoadAsync_Success_DisplaysCustomers"

# Run tests matching name pattern
dotnet test --filter "Name~LoadAsync"

# Run with code coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Run tests in parallel
dotnet test --parallel
```

### Continuous Integration (GitHub Actions)

```yaml
# .github/workflows/tests.yml
name: Run Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore

      - name: Test
        run: dotnet test --no-build --verbosity normal --logger "trx" /p:CollectCoverage=true

      - name: Upload test results
        uses: actions/upload-artifact@v3
        if: always()
        with:
          name: test-results
          path: '**/*.trx'
```

---

## Troubleshooting

### Problem: Tests Pass Individually but Fail Together

**Cause**: Shared state between tests

**Solution**:
```csharp
// Ensure each test has independent setup
public CustomerServiceTests()
{
    // Create NEW instances for each test
    _mockRepo = new Mock<ICustomerRepository>();
    _service = new CustomerService(_mockRepo.Object);
}
```

### Problem: Async Tests Hanging

**Cause**: Deadlock or missing await

**Solution**:
```csharp
// ❌ DON'T
[Fact]
public void Test()
{
    var result = _service.GetDataAsync().Result; // Deadlock!
}

// ✅ DO
[Fact]
public async Task Test()
{
    var result = await _service.GetDataAsync();
}
```

### Problem: Mock Not Working

**Cause**: Not using mock.Object or setup not matching

**Solution**:
```csharp
// ❌ Wrong
var mockRepo = new Mock<IRepository>();
var service = new Service(mockRepo); // Missing .Object!

// ✅ Correct
var mockRepo = new Mock<IRepository>();
var service = new Service(mockRepo.Object);

// Ensure setup matches call
_mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);
// Call must use ID = 1
```

### Problem: Verification Fails

**Cause**: Method not called or called with different parameters

**Solution**:
```csharp
// Debug what was actually called
_mockRepo.Invocations.Should().HaveCount(1);
Console.WriteLine(_mockRepo.Invocations[0].Method.Name);

// Use It.IsAny if exact parameters don't matter
_mockRepo.Verify(r => r.Save(It.IsAny<Customer>()), Times.Once);
```

---

## Performance Tips

### 1. Minimize Test Setup

```csharp
// ❌ SLOW - Complex setup
public CustomerServiceTests()
{
    var connection = new SqlConnection(...);
    var context = new DbContext(connection);
    _repository = new CustomerRepository(context);
    // Slow initialization
}

// ✅ FAST - Mock dependencies
public CustomerServiceTests()
{
    _mockRepo = new Mock<ICustomerRepository>();
    _service = new CustomerService(_mockRepo.Object);
}
```

### 2. Use Parallel Test Execution

```xml
<!-- xunit.runner.json -->
{
  "parallelizeTestCollections": true,
  "maxParallelThreads": -1
}
```

### 3. Avoid Thread.Sleep in Tests

```csharp
// ❌ SLOW
[Fact]
public async Task Test()
{
    Task.Run(() => _service.Process());
    Thread.Sleep(1000); // Slow!
    Assert.True(_service.IsComplete);
}

// ✅ FAST
[Fact]
public async Task Test()
{
    await _service.ProcessAsync();
    Assert.True(_service.IsComplete);
}
```

---

## Related Topics

- **[Testing Overview](testing-overview.md)** - Full testing strategy
- **[Integration Testing](integration-testing.md)** - Testing with real dependencies
- **[MVP Pattern](../architecture/mvp-pattern.md)** - Understanding what to unit test
- **[Dependency Injection](../architecture/dependency-injection.md)** - Enabling testability
- **[Mocking Guide](mocking-guide.md)** - Deep dive into Moq

---

## Summary

**Key Takeaways:**

1. ✅ **Unit test presenters, services, and business logic** - they're isolated and fast to test
2. ❌ **Don't unit test WinForms controls** - they're hard to test and you're testing the framework
3. ✅ **Use MVP/MVVM patterns** - they make your code testable by separating concerns
4. ✅ **Mock external dependencies** - database, file system, network calls
5. ✅ **Follow AAA pattern** - Arrange, Act, Assert
6. ✅ **Name tests descriptively** - `MethodName_Scenario_ExpectedResult`
7. ✅ **Test success AND failure paths** - don't just test the happy path
8. ✅ **Use [Theory]** for multiple similar test cases
9. ✅ **Keep tests independent** - no shared state between tests
10. ✅ **Verify important interactions** - but don't over-verify implementation details

**Testing makes your code:**
- More reliable (catches bugs early)
- Better designed (forces good architecture)
- Easier to refactor (safety net)
- Well documented (tests show usage)

Start testing today and your future self will thank you!

---

**Last Updated**: 2025-11-07
**Version**: 1.0
