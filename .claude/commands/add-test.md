# Add Unit Tests

You are asked to add unit tests for a class. Follow TDD best practices:

## Steps

1. **Identify class to test**
   - Confirm class name and file location
   - Check if it's testable (Services, Presenters - YES; Forms - NO)

2. **Check existing tests**
   - Look for existing test project
   - Check for existing tests for this class

3. **Create test project if needed**
   ```bash
   dotnet new xunit -n ProjectName.Tests
   dotnet add ProjectName.Tests reference ProjectName
   dotnet add package Moq
   dotnet add package FluentAssertions
   ```

4. **Create test class**
   - Name: `ClassNameTests.cs`
   - One test class per production class
   - Follow docs/testing/unit-testing.md

5. **Write tests for each method**
   - At least one happy path test
   - One test per error condition
   - Edge cases (null, empty, boundary values)
   - Follow AAA pattern (Arrange-Act-Assert)

6. **Mock dependencies**
   - Use Moq for interface mocking
   - Mock IServices, IRepositories
   - Verify method calls with Moq

7. **Run and verify tests**
   ```bash
   dotnet test
   ```

## Test Template

```csharp
public class CustomerServiceTests
{
    private Mock<ICustomerRepository> _mockRepository;
    private Mock<ILogger<CustomerService>> _mockLogger;
    private CustomerService _service;

    public CustomerServiceTests()
    {
        _mockRepository = new Mock<ICustomerRepository>();
        _mockLogger = new Mock<ILogger<CustomerService>>();
        _service = new CustomerService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsCustomer()
    {
        // Arrange
        var expectedCustomer = new Customer { Id = 1, Name = "John" };
        _mockRepository
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(expectedCustomer);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("John", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_InvalidId_ReturnsNull()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SaveAsync_ValidCustomer_CallsRepository()
    {
        // Arrange
        var customer = new Customer { Name = "John", Email = "john@example.com" };
        _mockRepository
            .Setup(r => r.SaveAsync(It.IsAny<Customer>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.SaveAsync(customer);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.SaveAsync(customer), Times.Once);
    }

    [Fact]
    public async Task SaveAsync_NullCustomer_ThrowsException()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.SaveAsync(null!));
    }
}
```

## Checklist

- [ ] Test project exists and references main project
- [ ] Moq and xUnit packages installed
- [ ] Test class created with Tests suffix
- [ ] All public methods have tests
- [ ] Happy path tested
- [ ] Error conditions tested
- [ ] Edge cases tested
- [ ] Dependencies mocked
- [ ] Tests follow AAA pattern
- [ ] Tests have descriptive names (Method_Scenario_ExpectedResult)
- [ ] All tests pass

## Coverage Target

- Aim for 80%+ code coverage
- Run with coverage:
  ```bash
  dotnet test /p:CollectCoverage=true
  ```

## References

- [Testing Overview](../docs/testing/testing-overview.md)
- [Unit Testing Guide](../docs/testing/unit-testing.md)
