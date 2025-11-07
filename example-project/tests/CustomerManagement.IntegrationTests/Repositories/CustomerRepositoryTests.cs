using CustomerManagement.Data;
using CustomerManagement.Models;
using CustomerManagement.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CustomerManagement.IntegrationTests.Repositories;

/// <summary>
/// Integration tests for CustomerRepository.
/// Uses SQLite in-memory database for testing.
/// </summary>
public class CustomerRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly CustomerRepository _repository;

    public CustomerRepositoryTests()
    {
        // Create in-memory SQLite database for testing
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new AppDbContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        _repository = new CustomerRepository(_context);
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ReturnsAllCustomers_OrderedByName()
    {
        // Arrange
        await SeedCustomersAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Bob Johnson"); // Alphabetically first
        result[1].Name.Should().Be("Jane Smith");
        result[2].Name.Should().Be("John Doe");
    }

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ExistingCustomer_ReturnsCustomer()
    {
        // Arrange
        var customer = await SeedSingleCustomerAsync("John Doe", "john@example.com");

        // Act
        var result = await _repository.GetByIdAsync(customer.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(customer.Id);
        result.Name.Should().Be("John Doe");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingCustomer_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region AddAsync Tests

    [Fact]
    public async Task AddAsync_ValidCustomer_AddsToDatabase()
    {
        // Arrange
        var customer = new Customer
        {
            Name = "New Customer",
            Email = "new@example.com",
            Phone = "+1-555-0100",
            City = "New York",
            IsActive = true
        };

        // Act
        var result = await _repository.AddAsync(customer);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("New Customer");

        var fromDb = await _repository.GetByIdAsync(result.Id);
        fromDb.Should().NotBeNull();
        fromDb!.Email.Should().Be("new@example.com");
    }

    [Fact]
    public async Task AddAsync_NullCustomer_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _repository.AddAsync(null!));
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ExistingCustomer_UpdatesInDatabase()
    {
        // Arrange
        var customer = await SeedSingleCustomerAsync("John Doe", "john@example.com");

        // Update customer
        customer.Name = "John Doe Updated";
        customer.Phone = "+1-555-9999";

        // Act
        await _repository.UpdateAsync(customer);

        // Assert
        var fromDb = await _repository.GetByIdAsync(customer.Id);
        fromDb.Should().NotBeNull();
        fromDb!.Name.Should().Be("John Doe Updated");
        fromDb.Phone.Should().Be("+1-555-9999");
        fromDb.UpdatedAt.Should().NotBeNull();
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ExistingCustomer_RemovesFromDatabase()
    {
        // Arrange
        var customer = await SeedSingleCustomerAsync("John Doe", "john@example.com");

        // Act
        await _repository.DeleteAsync(customer);

        // Assert
        var fromDb = await _repository.GetByIdAsync(customer.Id);
        fromDb.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_CustomerWithOrders_CascadeDeletes()
    {
        // Arrange
        var customer = await SeedSingleCustomerAsync("John Doe", "john@example.com");
        var order = new Order
        {
            OrderNumber = "ORD-001",
            CustomerId = customer.Id,
            TotalAmount = 100.00m,
            Status = "Pending"
        };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(customer);

        // Assert
        var customerFromDb = await _repository.GetByIdAsync(customer.Id);
        customerFromDb.Should().BeNull();

        var ordersFromDb = await _context.Orders.Where(o => o.CustomerId == customer.Id).ToListAsync();
        ordersFromDb.Should().BeEmpty(); // Cascade delete
    }

    #endregion

    #region GetByEmailAsync Tests

    [Fact]
    public async Task GetByEmailAsync_ExistingEmail_ReturnsCustomer()
    {
        // Arrange
        await SeedSingleCustomerAsync("John Doe", "john@example.com");

        // Act
        var result = await _repository.GetByEmailAsync("john@example.com");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("john@example.com");
        result.Name.Should().Be("John Doe");
    }

    [Fact]
    public async Task GetByEmailAsync_NonExistingEmail_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByEmailAsync("nonexistent@example.com");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_EmptyEmail_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _repository.GetByEmailAsync(""));
    }

    #endregion

    #region GetWithOrdersAsync Tests

    [Fact]
    public async Task GetWithOrdersAsync_CustomerWithOrders_IncludesOrders()
    {
        // Arrange
        var customer = await SeedSingleCustomerAsync("John Doe", "john@example.com");
        var order1 = new Order
        {
            OrderNumber = "ORD-001",
            CustomerId = customer.Id,
            TotalAmount = 100.00m,
            Status = "Pending"
        };
        var order2 = new Order
        {
            OrderNumber = "ORD-002",
            CustomerId = customer.Id,
            TotalAmount = 200.00m,
            Status = "Completed"
        };
        _context.Orders.AddRange(order1, order2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetWithOrdersAsync(customer.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Orders.Should().HaveCount(2);
        result.Orders.Should().Contain(o => o.OrderNumber == "ORD-001");
        result.Orders.Should().Contain(o => o.OrderNumber == "ORD-002");
    }

    #endregion

    #region GetActiveCustomersAsync Tests

    [Fact]
    public async Task GetActiveCustomersAsync_ReturnsOnlyActiveCustomers()
    {
        // Arrange
        var active1 = await SeedSingleCustomerAsync("Active 1", "active1@example.com", isActive: true);
        var active2 = await SeedSingleCustomerAsync("Active 2", "active2@example.com", isActive: true);
        var inactive = await SeedSingleCustomerAsync("Inactive", "inactive@example.com", isActive: false);

        // Act
        var result = await _repository.GetActiveCustomersAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Id == active1.Id);
        result.Should().Contain(c => c.Id == active2.Id);
        result.Should().NotContain(c => c.Id == inactive.Id);
    }

    #endregion

    #region SearchByNameAsync Tests

    [Fact]
    public async Task SearchByNameAsync_MatchingName_ReturnsMatchingCustomers()
    {
        // Arrange
        await SeedCustomersAsync();

        // Act
        var result = await _repository.SearchByNameAsync("John");

        // Assert
        result.Should().HaveCount(2); // John Doe and Bob Johnson
        result.Should().Contain(c => c.Name == "John Doe");
        result.Should().Contain(c => c.Name == "Bob Johnson");
    }

    [Fact]
    public async Task SearchByNameAsync_CaseInsensitive_ReturnsMatchingCustomers()
    {
        // Arrange
        await SeedSingleCustomerAsync("John Doe", "john@example.com");

        // Act
        var result = await _repository.SearchByNameAsync("john");

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("John Doe");
    }

    [Fact]
    public async Task SearchByNameAsync_EmptySearchTerm_ReturnsAllCustomers()
    {
        // Arrange
        await SeedCustomersAsync();

        // Act
        var result = await _repository.SearchByNameAsync("");

        // Assert
        result.Should().HaveCount(3);
    }

    #endregion

    #region AnyAsync Tests

    [Fact]
    public async Task AnyAsync_MatchingPredicate_ReturnsTrue()
    {
        // Arrange
        await SeedSingleCustomerAsync("John Doe", "john@example.com");

        // Act
        var result = await _repository.AnyAsync(c => c.Email == "john@example.com");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task AnyAsync_NonMatchingPredicate_ReturnsFalse()
    {
        // Arrange
        await SeedSingleCustomerAsync("John Doe", "john@example.com");

        // Act
        var result = await _repository.AnyAsync(c => c.Email == "nonexistent@example.com");

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Helper Methods

    private async Task<Customer> SeedSingleCustomerAsync(string name, string email, bool isActive = true)
    {
        var customer = new Customer
        {
            Name = name,
            Email = email,
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear(); // Clear tracking for fresh reads

        return customer;
    }

    private async Task SeedCustomersAsync()
    {
        var customers = new[]
        {
            new Customer
            {
                Name = "John Doe",
                Email = "john@example.com",
                IsActive = true
            },
            new Customer
            {
                Name = "Jane Smith",
                Email = "jane@example.com",
                IsActive = true
            },
            new Customer
            {
                Name = "Bob Johnson",
                Email = "bob@example.com",
                IsActive = true
            }
        };

        _context.Customers.AddRange(customers);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    #endregion

    public void Dispose()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }
}
