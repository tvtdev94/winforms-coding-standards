using CustomerManagement.Data;
using CustomerManagement.Models;
using CustomerManagement.Repositories;
using CustomerManagement.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CustomerManagement.Tests.Services;

/// <summary>
/// Unit tests for CustomerService.
/// Tests business logic and validation.
/// </summary>
public class CustomerServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICustomerRepository> _mockRepository;
    private readonly Mock<ILogger<CustomerService>> _mockLogger;
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockRepository = new Mock<ICustomerRepository>();
        _mockLogger = new Mock<ILogger<CustomerService>>();

        // Setup Unit of Work to return the mock repository
        _mockUnitOfWork.Setup(u => u.Customers).Returns(_mockRepository.Object);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _service = new CustomerService(_mockUnitOfWork.Object, _mockLogger.Object);
    }

    #region GetAllCustomersAsync Tests

    [Fact]
    public async Task GetAllCustomersAsync_ReturnsAllCustomers()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new() { Id = 1, Name = "John Doe", Email = "john@example.com" },
            new() { Id = 2, Name = "Jane Smith", Email = "jane@example.com" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(customers);

        // Act
        var result = await _service.GetAllCustomersAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(customers);
        _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllCustomersAsync_RepositoryThrowsException_ThrowsInvalidOperationException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.GetAllCustomersAsync());
    }

    #endregion

    #region CreateCustomerAsync Tests

    [Fact]
    public async Task CreateCustomerAsync_ValidCustomer_ReturnsCreatedCustomer()
    {
        // Arrange
        var customer = new Customer
        {
            Name = "John Doe",
            Email = "john@example.com",
            Phone = "+1-555-0100"
        };

        _mockRepository.Setup(r => r.GetByEmailAsync(customer.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer c, CancellationToken ct) => { c.Id = 1; return c; });

        // Act
        var result = await _service.CreateCustomerAsync(customer);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateCustomerAsync_NullCustomer_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _service.CreateCustomerAsync(null!));
    }

    [Fact]
    public async Task CreateCustomerAsync_EmptyName_ThrowsInvalidOperationException()
    {
        // Arrange
        var customer = new Customer
        {
            Name = "",
            Email = "john@example.com"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.CreateCustomerAsync(customer));
        exception.Message.Should().Contain("Name is required");
    }

    [Fact]
    public async Task CreateCustomerAsync_EmptyEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var customer = new Customer
        {
            Name = "John Doe",
            Email = ""
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.CreateCustomerAsync(customer));
        exception.Message.Should().Contain("Email is required");
    }

    [Fact]
    public async Task CreateCustomerAsync_InvalidEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var customer = new Customer
        {
            Name = "John Doe",
            Email = "invalid-email"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.CreateCustomerAsync(customer));
        exception.Message.Should().Contain("Email format is invalid");
    }

    [Fact]
    public async Task CreateCustomerAsync_DuplicateEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var customer = new Customer
        {
            Name = "John Doe",
            Email = "john@example.com"
        };

        var existingCustomer = new Customer
        {
            Id = 1,
            Name = "Existing Customer",
            Email = "john@example.com"
        };

        _mockRepository.Setup(r => r.GetByEmailAsync(customer.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCustomer);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.CreateCustomerAsync(customer));
        exception.Message.Should().Contain("already exists");
    }

    #endregion

    #region UpdateCustomerAsync Tests

    [Fact]
    public async Task UpdateCustomerAsync_ValidCustomer_UpdatesCustomer()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            Name = "John Doe Updated",
            Email = "john@example.com",
            Phone = "+1-555-0100"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        _mockRepository.Setup(r => r.GetByEmailAsync(customer.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateCustomerAsync(customer);

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCustomerAsync_CustomerNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 999,
            Name = "John Doe",
            Email = "john@example.com"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.UpdateCustomerAsync(customer));
        exception.Message.Should().Contain("not found");
    }

    #endregion

    #region DeleteCustomerAsync Tests

    [Fact]
    public async Task DeleteCustomerAsync_ExistingCustomer_DeletesCustomer()
    {
        // Arrange
        var customerId = 1;
        var customer = new Customer
        {
            Id = customerId,
            Name = "John Doe",
            Email = "john@example.com"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteCustomerAsync(customerId);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteCustomerAsync_CustomerNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var customerId = 999;

        _mockRepository.Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.DeleteCustomerAsync(customerId));
        exception.Message.Should().Contain("not found");
    }

    #endregion

    #region ValidateCustomer Tests

    [Fact]
    public void ValidateCustomer_ValidCustomer_ReturnsNoErrors()
    {
        // Arrange
        var customer = new Customer
        {
            Name = "John Doe",
            Email = "john@example.com",
            Phone = "+1-555-0100"
        };

        // Act
        var errors = _service.ValidateCustomer(customer);

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateCustomer_NameTooLong_ReturnsError()
    {
        // Arrange
        var customer = new Customer
        {
            Name = new string('A', 101), // 101 characters
            Email = "john@example.com"
        };

        // Act
        var errors = _service.ValidateCustomer(customer);

        // Assert
        errors.Should().ContainKey(nameof(Customer.Name));
        errors[nameof(Customer.Name)].Should().Contain("100 characters");
    }

    [Fact]
    public void ValidateCustomer_InvalidPhone_ReturnsError()
    {
        // Arrange
        var customer = new Customer
        {
            Name = "John Doe",
            Email = "john@example.com",
            Phone = "invalid_phone!"
        };

        // Act
        var errors = _service.ValidateCustomer(customer);

        // Assert
        errors.Should().ContainKey(nameof(Customer.Phone));
        errors[nameof(Customer.Phone)].Should().Contain("format is invalid");
    }

    #endregion

    #region EmailExistsAsync Tests

    [Fact]
    public async Task EmailExistsAsync_EmailExists_ReturnsTrue()
    {
        // Arrange
        var email = "john@example.com";
        var customer = new Customer { Id = 1, Email = email, Name = "John" };

        _mockRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        // Act
        var result = await _service.EmailExistsAsync(email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task EmailExistsAsync_EmailDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var email = "john@example.com";

        _mockRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _service.EmailExistsAsync(email);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task EmailExistsAsync_ExcludeCustomerId_ReturnsFalseForSameCustomer()
    {
        // Arrange
        var email = "john@example.com";
        var customerId = 1;
        var customer = new Customer { Id = customerId, Email = email, Name = "John" };

        _mockRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        // Act
        var result = await _service.EmailExistsAsync(email, customerId);

        // Assert
        result.Should().BeFalse(); // Same customer, so email is available for this customer
    }

    #endregion
}
