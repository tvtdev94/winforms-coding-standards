using CustomerManagement.Models;
using CustomerManagement.Presenters;
using CustomerManagement.Services;
using CustomerManagement.Views;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CustomerManagement.Tests.Presenters;

/// <summary>
/// Unit tests for CustomerEditPresenter.
/// Tests presenter logic for creating and editing customers.
/// </summary>
public class CustomerEditPresenterTests : IDisposable
{
    private readonly Mock<ICustomerEditView> _mockView;
    private readonly Mock<ICustomerService> _mockService;
    private readonly Mock<ILogger<CustomerEditPresenter>> _mockLogger;
    private readonly CustomerEditPresenter _presenter;

    public CustomerEditPresenterTests()
    {
        _mockView = new Mock<ICustomerEditView>();
        _mockService = new Mock<ICustomerService>();
        _mockLogger = new Mock<ILogger<CustomerEditPresenter>>();

        _presenter = new CustomerEditPresenter(
            _mockView.Object,
            _mockService.Object,
            _mockLogger.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_NullView_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new CustomerEditPresenter(null!, _mockService.Object, _mockLogger.Object));

        exception.ParamName.Should().Be("view");
    }

    [Fact]
    public void Constructor_NullService_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new CustomerEditPresenter(_mockView.Object, null!, _mockLogger.Object));

        exception.ParamName.Should().Be("customerService");
    }

    [Fact]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new CustomerEditPresenter(_mockView.Object, _mockService.Object, null!));

        exception.ParamName.Should().Be("logger");
    }

    #endregion

    #region LoadRequested Tests - Create Mode

    [Fact]
    public async Task LoadRequested_CreateMode_SetsIsEditModeToFalse()
    {
        // Arrange
        _mockView.Setup(v => v.CustomerId).Returns(0);

        // Act
        _mockView.Raise(v => v.LoadRequested += null, EventArgs.Empty);

        // Wait for async operation
        await Task.Delay(100);

        // Assert
        _mockView.VerifySet(v => v.IsEditMode = false, Times.Once);
        _mockService.Verify(s => s.GetCustomerByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region LoadRequested Tests - Edit Mode

    [Fact]
    public async Task LoadRequested_EditMode_LoadsCustomerSuccessfully()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com",
            Phone = "+1-555-0100",
            Address = "123 Main St",
            City = "New York",
            Country = "USA",
            IsActive = true
        };

        _mockView.Setup(v => v.CustomerId).Returns(customer.Id);
        _mockService.Setup(s => s.GetCustomerByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        // Act
        _mockView.Raise(v => v.LoadRequested += null, EventArgs.Empty);

        // Wait for async operation
        await Task.Delay(100);

        // Assert
        _mockView.VerifySet(v => v.IsEditMode = true, Times.Once);
        _mockView.VerifySet(v => v.IsLoading = true, Times.Once);
        _mockView.VerifySet(v => v.IsLoading = false, Times.Once);

        _mockView.VerifySet(v => v.CustomerName = customer.Name, Times.Once);
        _mockView.VerifySet(v => v.CustomerEmail = customer.Email, Times.Once);
        _mockView.VerifySet(v => v.CustomerPhone = customer.Phone, Times.Once);
        _mockView.VerifySet(v => v.CustomerAddress = customer.Address, Times.Once);
        _mockView.VerifySet(v => v.CustomerCity = customer.City, Times.Once);
        _mockView.VerifySet(v => v.CustomerCountry = customer.Country, Times.Once);
        _mockView.VerifySet(v => v.IsActive = customer.IsActive, Times.Once);
    }

    [Fact]
    public async Task LoadRequested_CustomerNotFound_ShowsErrorAndCloses()
    {
        // Arrange
        var customerId = 999;
        _mockView.Setup(v => v.CustomerId).Returns(customerId);
        _mockService.Setup(s => s.GetCustomerByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        // Act
        _mockView.Raise(v => v.LoadRequested += null, EventArgs.Empty);

        // Wait for async operation
        await Task.Delay(100);

        // Assert
        _mockView.Verify(v => v.ShowError(It.Is<string>(msg =>
            msg.Contains(customerId.ToString()) && msg.Contains("not found"))), Times.Once);
        _mockView.Verify(v => v.CloseWithResult(false), Times.Once);
    }

    [Fact]
    public async Task LoadRequested_ServiceThrowsException_ShowsErrorAndCloses()
    {
        // Arrange
        var customerId = 1;
        _mockView.Setup(v => v.CustomerId).Returns(customerId);
        _mockService.Setup(s => s.GetCustomerByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        _mockView.Raise(v => v.LoadRequested += null, EventArgs.Empty);

        // Wait for async operation
        await Task.Delay(100);

        // Assert
        _mockView.Verify(v => v.ShowError(It.Is<string>(msg =>
            msg.Contains("Failed to load"))), Times.Once);
        _mockView.Verify(v => v.CloseWithResult(false), Times.Once);
        _mockView.VerifySet(v => v.IsLoading = false, Times.Once);
    }

    [Fact]
    public async Task LoadRequested_CustomerWithNullFields_HandlesNullsCorrectly()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com",
            Phone = null,  // Null optional fields
            Address = null,
            City = null,
            Country = null,
            IsActive = true
        };

        _mockView.Setup(v => v.CustomerId).Returns(customer.Id);
        _mockService.Setup(s => s.GetCustomerByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        // Act
        _mockView.Raise(v => v.LoadRequested += null, EventArgs.Empty);

        // Wait for async operation
        await Task.Delay(100);

        // Assert - Nulls should be converted to empty strings
        _mockView.VerifySet(v => v.CustomerPhone = string.Empty, Times.Once);
        _mockView.VerifySet(v => v.CustomerAddress = string.Empty, Times.Once);
        _mockView.VerifySet(v => v.CustomerCity = string.Empty, Times.Once);
        _mockView.VerifySet(v => v.CustomerCountry = string.Empty, Times.Once);
    }

    #endregion

    #region SaveRequested Tests - Create Customer

    [Fact]
    public async Task SaveRequested_CreateMode_ValidCustomer_CreatesSuccessfully()
    {
        // Arrange
        _mockView.Setup(v => v.IsEditMode).Returns(false);
        _mockView.Setup(v => v.CustomerName).Returns("John Doe");
        _mockView.Setup(v => v.CustomerEmail).Returns("john@example.com");
        _mockView.Setup(v => v.CustomerPhone).Returns("+1-555-0100");
        _mockView.Setup(v => v.CustomerAddress).Returns("123 Main St");
        _mockView.Setup(v => v.CustomerCity).Returns("New York");
        _mockView.Setup(v => v.CustomerCountry).Returns("USA");
        _mockView.Setup(v => v.IsActive).Returns(true);

        _mockService.Setup(s => s.ValidateCustomer(It.IsAny<Customer>()))
            .Returns(new Dictionary<string, string>()); // No validation errors

        _mockService.Setup(s => s.CreateCustomerAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        _mockView.Raise(v => v.SaveRequested += null, EventArgs.Empty);

        // Wait for async operation
        await Task.Delay(100);

        // Assert
        _mockView.Verify(v => v.ClearAllErrors(), Times.Once);
        _mockView.VerifySet(v => v.IsLoading = true, Times.Once);
        _mockView.VerifySet(v => v.IsLoading = false, Times.Once);

        _mockService.Verify(s => s.CreateCustomerAsync(
            It.Is<Customer>(c =>
                c.Name == "John Doe" &&
                c.Email == "john@example.com" &&
                c.Phone == "+1-555-0100" &&
                c.IsActive == true),
            It.IsAny<CancellationToken>()),
            Times.Once);

        _mockView.Verify(v => v.ShowSuccess(It.Is<string>(msg =>
            msg.Contains("created successfully"))), Times.Once);
        _mockView.Verify(v => v.CloseWithResult(true), Times.Once);
    }

    [Fact]
    public async Task SaveRequested_CreateMode_TrimsWhitespace()
    {
        // Arrange
        _mockView.Setup(v => v.IsEditMode).Returns(false);
        _mockView.Setup(v => v.CustomerName).Returns("  John Doe  ");  // Leading/trailing spaces
        _mockView.Setup(v => v.CustomerEmail).Returns("  john@example.com  ");
        _mockView.Setup(v => v.CustomerPhone).Returns("  +1-555-0100  ");

        _mockService.Setup(s => s.ValidateCustomer(It.IsAny<Customer>()))
            .Returns(new Dictionary<string, string>());

        _mockService.Setup(s => s.CreateCustomerAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        _mockView.Raise(v => v.SaveRequested += null, EventArgs.Empty);

        // Wait for async operation
        await Task.Delay(100);

        // Assert - Whitespace should be trimmed
        _mockService.Verify(s => s.CreateCustomerAsync(
            It.Is<Customer>(c =>
                c.Name == "John Doe" &&
                c.Email == "john@example.com" &&
                c.Phone == "+1-555-0100"),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SaveRequested_CreateMode_EmptyOptionalFields_SetsToNull()
    {
        // Arrange
        _mockView.Setup(v => v.IsEditMode).Returns(false);
        _mockView.Setup(v => v.CustomerName).Returns("John Doe");
        _mockView.Setup(v => v.CustomerEmail).Returns("john@example.com");
        _mockView.Setup(v => v.CustomerPhone).Returns("   ");  // Whitespace only
        _mockView.Setup(v => v.CustomerAddress).Returns("");   // Empty string
        _mockView.Setup(v => v.CustomerCity).Returns((string?)null);  // Null

        _mockService.Setup(s => s.ValidateCustomer(It.IsAny<Customer>()))
            .Returns(new Dictionary<string, string>());

        _mockService.Setup(s => s.CreateCustomerAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        _mockView.Raise(v => v.SaveRequested += null, EventArgs.Empty);

        // Wait for async operation
        await Task.Delay(100);

        // Assert - Empty/whitespace optional fields should become null
        _mockService.Verify(s => s.CreateCustomerAsync(
            It.Is<Customer>(c =>
                c.Phone == null &&
                c.Address == null &&
                c.City == null),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region SaveRequested Tests - Update Customer

    [Fact]
    public async Task SaveRequested_EditMode_ValidCustomer_UpdatesSuccessfully()
    {
        // Arrange
        var existingCustomer = new Customer
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com"
        };

        _mockView.Setup(v => v.IsEditMode).Returns(true);
        _mockView.Setup(v => v.CustomerId).Returns(existingCustomer.Id);
        _mockView.Setup(v => v.CustomerName).Returns("Jane Smith");  // Changed
        _mockView.Setup(v => v.CustomerEmail).Returns("jane@example.com");  // Changed

        // Simulate loading the customer first
        _mockService.Setup(s => s.GetCustomerByIdAsync(existingCustomer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCustomer);

        _mockService.Setup(s => s.ValidateCustomer(It.IsAny<Customer>()))
            .Returns(new Dictionary<string, string>());

        _mockService.Setup(s => s.UpdateCustomerAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Load customer first
        _mockView.Raise(v => v.LoadRequested += null, EventArgs.Empty);
        await Task.Delay(100);

        // Act - Save changes
        _mockView.Raise(v => v.SaveRequested += null, EventArgs.Empty);
        await Task.Delay(100);

        // Assert
        _mockService.Verify(s => s.UpdateCustomerAsync(
            It.Is<Customer>(c =>
                c.Id == existingCustomer.Id &&
                c.Name == "Jane Smith" &&
                c.Email == "jane@example.com"),
            It.IsAny<CancellationToken>()),
            Times.Once);

        _mockView.Verify(v => v.ShowSuccess(It.Is<string>(msg =>
            msg.Contains("updated successfully"))), Times.Once);
        _mockView.Verify(v => v.CloseWithResult(true), Times.Once);
    }

    #endregion

    #region SaveRequested Tests - Validation

    [Fact]
    public async Task SaveRequested_ValidationFails_ShowsErrorsAndDoesNotSave()
    {
        // Arrange
        _mockView.Setup(v => v.IsEditMode).Returns(false);
        _mockView.Setup(v => v.CustomerName).Returns("");  // Invalid - empty
        _mockView.Setup(v => v.CustomerEmail).Returns("invalid-email");  // Invalid - bad format

        var validationErrors = new Dictionary<string, string>
        {
            { "Name", "Name is required" },
            { "Email", "Invalid email format" }
        };

        _mockService.Setup(s => s.ValidateCustomer(It.IsAny<Customer>()))
            .Returns(validationErrors);

        // Act
        _mockView.Raise(v => v.SaveRequested += null, EventArgs.Empty);

        // Wait for async operation
        await Task.Delay(100);

        // Assert
        _mockView.Verify(v => v.SetFieldError("Name", "Name is required"), Times.Once);
        _mockView.Verify(v => v.SetFieldError("Email", "Invalid email format"), Times.Once);
        _mockView.Verify(v => v.ShowError(It.Is<string>(msg =>
            msg.Contains("validation errors"))), Times.Once);

        // Should NOT call create or update
        _mockService.Verify(s => s.CreateCustomerAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockService.Verify(s => s.UpdateCustomerAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);

        // Should NOT close
        _mockView.Verify(v => v.CloseWithResult(It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task SaveRequested_MultipleValidationErrors_SetsAllErrors()
    {
        // Arrange
        _mockView.Setup(v => v.IsEditMode).Returns(false);

        var validationErrors = new Dictionary<string, string>
        {
            { "Name", "Name is required" },
            { "Email", "Email is required" },
            { "Phone", "Invalid phone format" }
        };

        _mockService.Setup(s => s.ValidateCustomer(It.IsAny<Customer>()))
            .Returns(validationErrors);

        // Act
        _mockView.Raise(v => v.SaveRequested += null, EventArgs.Empty);
        await Task.Delay(100);

        // Assert - All errors should be set
        _mockView.Verify(v => v.SetFieldError("Name", "Name is required"), Times.Once);
        _mockView.Verify(v => v.SetFieldError("Email", "Email is required"), Times.Once);
        _mockView.Verify(v => v.SetFieldError("Phone", "Invalid phone format"), Times.Once);
    }

    #endregion

    #region SaveRequested Tests - Error Handling

    [Fact]
    public async Task SaveRequested_CreateThrowsInvalidOperationException_ShowsBusinessError()
    {
        // Arrange
        _mockView.Setup(v => v.IsEditMode).Returns(false);
        _mockView.Setup(v => v.CustomerName).Returns("John Doe");
        _mockView.Setup(v => v.CustomerEmail).Returns("john@example.com");

        _mockService.Setup(s => s.ValidateCustomer(It.IsAny<Customer>()))
            .Returns(new Dictionary<string, string>());

        _mockService.Setup(s => s.CreateCustomerAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Customer with this email already exists"));

        // Act
        _mockView.Raise(v => v.SaveRequested += null, EventArgs.Empty);
        await Task.Delay(100);

        // Assert
        _mockView.Verify(v => v.ShowError("Customer with this email already exists"), Times.Once);
        _mockView.Verify(v => v.CloseWithResult(It.IsAny<bool>()), Times.Never);
        _mockView.VerifySet(v => v.IsLoading = false, Times.Once);
    }

    [Fact]
    public async Task SaveRequested_CreateThrowsGenericException_ShowsGenericError()
    {
        // Arrange
        _mockView.Setup(v => v.IsEditMode).Returns(false);
        _mockView.Setup(v => v.CustomerName).Returns("John Doe");
        _mockView.Setup(v => v.CustomerEmail).Returns("john@example.com");

        _mockService.Setup(s => s.ValidateCustomer(It.IsAny<Customer>()))
            .Returns(new Dictionary<string, string>());

        _mockService.Setup(s => s.CreateCustomerAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        _mockView.Raise(v => v.SaveRequested += null, EventArgs.Empty);
        await Task.Delay(100);

        // Assert
        _mockView.Verify(v => v.ShowError(It.Is<string>(msg =>
            msg.Contains("Failed to save") && msg.Contains("Database connection failed"))),
            Times.Once);
        _mockView.VerifySet(v => v.IsLoading = false, Times.Once);
    }

    #endregion

    #region CancelRequested Tests

    [Fact]
    public void CancelRequested_ClosesFormWithFalseResult()
    {
        // Act
        _mockView.Raise(v => v.CancelRequested += null, EventArgs.Empty);

        // Assert
        _mockView.Verify(v => v.CloseWithResult(false), Times.Once);
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_UnsubscribesFromAllEvents()
    {
        // Arrange
        var mockView = new Mock<ICustomerEditView>();
        var presenter = new CustomerEditPresenter(
            mockView.Object,
            _mockService.Object,
            _mockLogger.Object);

        // Act
        presenter.Dispose();

        // Try to raise events - they should not trigger any calls
        mockView.Raise(v => v.LoadRequested += null, EventArgs.Empty);
        mockView.Raise(v => v.SaveRequested += null, EventArgs.Empty);
        mockView.Raise(v => v.CancelRequested += null, EventArgs.Empty);

        // Assert - No interactions should happen after dispose
        mockView.Verify(v => v.ShowError(It.IsAny<string>()), Times.Never);
        mockView.Verify(v => v.ShowSuccess(It.IsAny<string>()), Times.Never);
    }

    #endregion

    public void Dispose()
    {
        _presenter?.Dispose();
    }
}
