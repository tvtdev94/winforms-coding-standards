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
/// Unit tests for CustomerListPresenter.
/// Tests presenter logic and view interaction.
/// </summary>
public class CustomerListPresenterTests : IDisposable
{
    private readonly Mock<ICustomerListView> _mockView;
    private readonly Mock<ICustomerService> _mockService;
    private readonly Mock<ILogger<CustomerListPresenter>> _mockLogger;
    private readonly CustomerListPresenter _presenter;

    public CustomerListPresenterTests()
    {
        _mockView = new Mock<ICustomerListView>();
        _mockService = new Mock<ICustomerService>();
        _mockLogger = new Mock<ILogger<CustomerListPresenter>>();

        _presenter = new CustomerListPresenter(
            _mockView.Object,
            _mockService.Object,
            _mockLogger.Object);
    }

    #region LoadRequested Tests

    [Fact]
    public async Task LoadRequested_LoadsCustomersSuccessfully()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new() { Id = 1, Name = "John Doe", Email = "john@example.com" },
            new() { Id = 2, Name = "Jane Smith", Email = "jane@example.com" }
        };

        _mockService.Setup(s => s.GetAllCustomersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(customers);

        var viewCustomers = new List<Customer>();
        _mockView.SetupSet(v => v.Customers = It.IsAny<List<Customer>>())
            .Callback<List<Customer>>(c => viewCustomers = c);

        // Act
        _mockView.Raise(v => v.LoadRequested += null, EventArgs.Empty);

        // Wait a bit for async operation
        await Task.Delay(100);

        // Assert
        viewCustomers.Should().HaveCount(2);
        viewCustomers.Should().BeEquivalentTo(customers);
        _mockView.VerifySet(v => v.IsLoading = true, Times.Once);
        _mockView.VerifySet(v => v.IsLoading = false, Times.Once);
    }

    [Fact]
    public async Task LoadRequested_ServiceThrowsException_ShowsError()
    {
        // Arrange
        _mockService.Setup(s => s.GetAllCustomersAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        _mockView.Raise(v => v.LoadRequested += null, EventArgs.Empty);

        // Wait for async operation
        await Task.Delay(100);

        // Assert
        _mockView.Verify(v => v.ShowError(It.Is<string>(msg => msg.Contains("Failed to load"))), Times.Once);
        _mockView.VerifySet(v => v.IsLoading = false, Times.Once);
    }

    #endregion

    #region SearchRequested Tests

    [Fact]
    public async Task SearchRequested_SearchesCustomersSuccessfully()
    {
        // Arrange
        var searchTerm = "John";
        var customers = new List<Customer>
        {
            new() { Id = 1, Name = "John Doe", Email = "john@example.com" }
        };

        _mockService.Setup(s => s.SearchCustomersAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customers);

        var viewCustomers = new List<Customer>();
        _mockView.SetupSet(v => v.Customers = It.IsAny<List<Customer>>())
            .Callback<List<Customer>>(c => viewCustomers = c);

        // Act
        _mockView.Raise(v => v.SearchRequested += null, searchTerm);

        // Wait for debounce + async operation
        await Task.Delay(400);

        // Assert
        viewCustomers.Should().HaveCount(1);
        viewCustomers[0].Name.Should().Be("John Doe");
    }

    [Fact]
    public async Task SearchRequested_MultipleSearches_OnlyLastSearchExecuted()
    {
        // Arrange
        var searchTerm1 = "John";
        var searchTerm2 = "Jane";
        var customers = new List<Customer>
        {
            new() { Id = 2, Name = "Jane Smith", Email = "jane@example.com" }
        };

        _mockService.Setup(s => s.SearchCustomersAsync(searchTerm2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customers);

        // Act
        _mockView.Raise(v => v.SearchRequested += null, searchTerm1);
        await Task.Delay(100); // Less than debounce time
        _mockView.Raise(v => v.SearchRequested += null, searchTerm2);

        // Wait for debounce + async operation
        await Task.Delay(400);

        // Assert - should only search for "Jane"
        _mockService.Verify(s => s.SearchCustomersAsync(searchTerm2, It.IsAny<CancellationToken>()), Times.Once);
        _mockService.Verify(s => s.SearchCustomersAsync(searchTerm1, It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region DeleteRequested Tests

    [Fact]
    public async Task DeleteRequested_UserConfirms_DeletesCustomer()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com"
        };

        _mockView.Setup(v => v.SelectedCustomer).Returns(customer);
        _mockView.Setup(v => v.ShowConfirmation(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        _mockService.Setup(s => s.DeleteCustomerAsync(customer.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockService.Setup(s => s.GetAllCustomersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Customer>());

        // Act
        _mockView.Raise(v => v.DeleteRequested += null, customer.Id);

        // Wait for async operation
        await Task.Delay(100);

        // Assert
        _mockService.Verify(s => s.DeleteCustomerAsync(customer.Id, It.IsAny<CancellationToken>()), Times.Once);
        _mockView.Verify(v => v.ShowSuccess(It.Is<string>(msg => msg.Contains("deleted successfully"))), Times.Once);
    }

    [Fact]
    public async Task DeleteRequested_UserCancels_DoesNotDelete()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com"
        };

        _mockView.Setup(v => v.SelectedCustomer).Returns(customer);
        _mockView.Setup(v => v.ShowConfirmation(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        // Act
        _mockView.Raise(v => v.DeleteRequested += null, customer.Id);

        // Wait a bit
        await Task.Delay(100);

        // Assert
        _mockService.Verify(s => s.DeleteCustomerAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteRequested_NoCustomerSelected_ShowsError()
    {
        // Arrange
        _mockView.Setup(v => v.SelectedCustomer).Returns((Customer?)null);

        // Act
        _mockView.Raise(v => v.DeleteRequested += null, 1);

        // Wait a bit
        await Task.Delay(100);

        // Assert
        _mockView.Verify(v => v.ShowError(It.Is<string>(msg => msg.Contains("select a customer"))), Times.Once);
        _mockService.Verify(s => s.DeleteCustomerAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region ViewDetailsRequested Tests

    [Fact]
    public async Task ViewDetailsRequested_LoadsCustomerWithOrders()
    {
        // Arrange
        var customerId = 1;
        var customer = new Customer
        {
            Id = customerId,
            Name = "John Doe",
            Email = "john@example.com",
            Phone = "+1-555-0100",
            Orders = new List<Order>
            {
                new() { Id = 1, OrderNumber = "ORD-001" },
                new() { Id = 2, OrderNumber = "ORD-002" }
            }
        };

        _mockService.Setup(s => s.GetCustomerWithOrdersAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        // Act
        _mockView.Raise(v => v.ViewDetailsRequested += null, customerId);

        // Wait for async operation
        await Task.Delay(100);

        // Assert
        _mockService.Verify(s => s.GetCustomerWithOrdersAsync(customerId, It.IsAny<CancellationToken>()), Times.Once);
        _mockView.Verify(v => v.ShowSuccess(It.Is<string>(msg =>
            msg.Contains(customer.Name) && msg.Contains("2"))), Times.Once);
    }

    #endregion

    public void Dispose()
    {
        _presenter?.Dispose();
    }
}
