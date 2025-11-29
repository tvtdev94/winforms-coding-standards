using CustomerManagement.Models;
using CustomerManagement.Validators;
using FluentAssertions;
using Xunit;

namespace CustomerManagement.Tests.Validators;

/// <summary>
/// Unit tests for CustomerValidator.
/// Tests validation rules for Customer entity.
/// </summary>
public class CustomerValidatorTests
{
    private readonly CustomerValidator _validator;

    public CustomerValidatorTests()
    {
        _validator = new CustomerValidator();
    }

    #region Valid Customer Tests

    [Fact]
    public void Validate_ValidCustomer_ReturnsNoErrors()
    {
        // Arrange
        var customer = CreateValidCustomer();

        // Act
        var errors = _validator.Validate(customer);

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ValidCustomerWithOptionalFields_ReturnsNoErrors()
    {
        // Arrange
        var customer = new Customer
        {
            Name = "John Doe",
            Email = "john@example.com",
            Phone = null,      // Optional
            Address = null,    // Optional
            City = null,       // Optional
            Country = null     // Optional
        };

        // Act
        var errors = _validator.Validate(customer);

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public void IsValid_ValidCustomer_ReturnsTrue()
    {
        // Arrange
        var customer = CreateValidCustomer();

        // Act
        var result = _validator.IsValid(customer);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Name Validation Tests

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_EmptyName_ReturnsError(string? name)
    {
        // Arrange
        var customer = CreateValidCustomer();
        customer.Name = name!;

        // Act
        var errors = _validator.Validate(customer);

        // Assert
        errors.Should().ContainKey(nameof(Customer.Name));
        errors[nameof(Customer.Name)].Should().Contain("required");
    }

    [Fact]
    public void Validate_NameTooLong_ReturnsError()
    {
        // Arrange
        var customer = CreateValidCustomer();
        customer.Name = new string('A', 101); // 101 characters

        // Act
        var errors = _validator.Validate(customer);

        // Assert
        errors.Should().ContainKey(nameof(Customer.Name));
        errors[nameof(Customer.Name)].Should().Contain("100 characters");
    }

    [Fact]
    public void Validate_NameExactlyMaxLength_ReturnsNoError()
    {
        // Arrange
        var customer = CreateValidCustomer();
        customer.Name = new string('A', 100); // Exactly 100 characters

        // Act
        var errors = _validator.Validate(customer);

        // Assert
        errors.Should().NotContainKey(nameof(Customer.Name));
    }

    #endregion

    #region Email Validation Tests

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_EmptyEmail_ReturnsError(string? email)
    {
        // Arrange
        var customer = CreateValidCustomer();
        customer.Email = email!;

        // Act
        var errors = _validator.Validate(customer);

        // Assert
        errors.Should().ContainKey(nameof(Customer.Email));
        errors[nameof(Customer.Email)].Should().Contain("required");
    }

    [Fact]
    public void Validate_EmailTooLong_ReturnsError()
    {
        // Arrange
        var customer = CreateValidCustomer();
        // Need > 100 chars: 92 'a' + '@test.com' (9 chars) = 101 chars
        customer.Email = new string('a', 92) + "@test.com";

        // Act
        var errors = _validator.Validate(customer);

        // Assert
        errors.Should().ContainKey(nameof(Customer.Email));
        errors[nameof(Customer.Email)].Should().Contain("100 characters");
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing@domain")]
    [InlineData("@nodomain.com")]
    [InlineData("spaces in@email.com")]
    public void Validate_InvalidEmailFormat_ReturnsError(string email)
    {
        // Arrange
        var customer = CreateValidCustomer();
        customer.Email = email;

        // Act
        var errors = _validator.Validate(customer);

        // Assert
        errors.Should().ContainKey(nameof(Customer.Email));
        errors[nameof(Customer.Email)].Should().Contain("format");
    }

    [Theory]
    [InlineData("valid@example.com")]
    [InlineData("user.name@domain.org")]
    [InlineData("test+tag@company.co.uk")]
    public void Validate_ValidEmailFormat_ReturnsNoError(string email)
    {
        // Arrange
        var customer = CreateValidCustomer();
        customer.Email = email;

        // Act
        var errors = _validator.Validate(customer);

        // Assert
        errors.Should().NotContainKey(nameof(Customer.Email));
    }

    #endregion

    #region Phone Validation Tests

    [Fact]
    public void Validate_NullPhone_ReturnsNoError()
    {
        // Arrange
        var customer = CreateValidCustomer();
        customer.Phone = null;

        // Act
        var errors = _validator.Validate(customer);

        // Assert
        errors.Should().NotContainKey(nameof(Customer.Phone));
    }

    [Fact]
    public void Validate_EmptyPhone_ReturnsNoError()
    {
        // Arrange
        var customer = CreateValidCustomer();
        customer.Phone = "";

        // Act
        var errors = _validator.Validate(customer);

        // Assert
        errors.Should().NotContainKey(nameof(Customer.Phone));
    }

    [Fact]
    public void Validate_PhoneTooLong_ReturnsError()
    {
        // Arrange
        var customer = CreateValidCustomer();
        customer.Phone = new string('1', 21); // 21 characters

        // Act
        var errors = _validator.Validate(customer);

        // Assert
        errors.Should().ContainKey(nameof(Customer.Phone));
        errors[nameof(Customer.Phone)].Should().Contain("20 characters");
    }

    [Theory]
    [InlineData("abc123")]
    [InlineData("phone@number")]
    [InlineData("123-456-CALL")]
    public void Validate_InvalidPhoneFormat_ReturnsError(string phone)
    {
        // Arrange
        var customer = CreateValidCustomer();
        customer.Phone = phone;

        // Act
        var errors = _validator.Validate(customer);

        // Assert
        errors.Should().ContainKey(nameof(Customer.Phone));
        errors[nameof(Customer.Phone)].Should().Contain("format");
    }

    [Theory]
    [InlineData("1234567890")]
    [InlineData("+1-555-123-4567")]
    [InlineData("(555) 123-4567")]
    [InlineData("+84 123 456 789")]
    public void Validate_ValidPhoneFormat_ReturnsNoError(string phone)
    {
        // Arrange
        var customer = CreateValidCustomer();
        customer.Phone = phone;

        // Act
        var errors = _validator.Validate(customer);

        // Assert
        errors.Should().NotContainKey(nameof(Customer.Phone));
    }

    #endregion

    #region Address Validation Tests

    [Fact]
    public void Validate_AddressTooLong_ReturnsError()
    {
        // Arrange
        var customer = CreateValidCustomer();
        customer.Address = new string('A', 201); // 201 characters

        // Act
        var errors = _validator.Validate(customer);

        // Assert
        errors.Should().ContainKey(nameof(Customer.Address));
        errors[nameof(Customer.Address)].Should().Contain("200 characters");
    }

    [Fact]
    public void Validate_AddressExactlyMaxLength_ReturnsNoError()
    {
        // Arrange
        var customer = CreateValidCustomer();
        customer.Address = new string('A', 200);

        // Act
        var errors = _validator.Validate(customer);

        // Assert
        errors.Should().NotContainKey(nameof(Customer.Address));
    }

    #endregion

    #region City Validation Tests

    [Fact]
    public void Validate_CityTooLong_ReturnsError()
    {
        // Arrange
        var customer = CreateValidCustomer();
        customer.City = new string('A', 51); // 51 characters

        // Act
        var errors = _validator.Validate(customer);

        // Assert
        errors.Should().ContainKey(nameof(Customer.City));
        errors[nameof(Customer.City)].Should().Contain("50 characters");
    }

    #endregion

    #region Country Validation Tests

    [Fact]
    public void Validate_CountryTooLong_ReturnsError()
    {
        // Arrange
        var customer = CreateValidCustomer();
        customer.Country = new string('A', 51); // 51 characters

        // Act
        var errors = _validator.Validate(customer);

        // Assert
        errors.Should().ContainKey(nameof(Customer.Country));
        errors[nameof(Customer.Country)].Should().Contain("50 characters");
    }

    #endregion

    #region Multiple Errors Tests

    [Fact]
    public void Validate_MultipleErrors_ReturnsAllErrors()
    {
        // Arrange
        var customer = new Customer
        {
            Name = "",           // Error: required
            Email = "invalid",   // Error: format
            Phone = "abc"        // Error: format
        };

        // Act
        var errors = _validator.Validate(customer);

        // Assert
        errors.Should().HaveCount(3);
        errors.Should().ContainKey(nameof(Customer.Name));
        errors.Should().ContainKey(nameof(Customer.Email));
        errors.Should().ContainKey(nameof(Customer.Phone));
    }

    [Fact]
    public void IsValid_InvalidCustomer_ReturnsFalse()
    {
        // Arrange
        var customer = new Customer { Name = "", Email = "" };

        // Act
        var result = _validator.IsValid(customer);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Extension Methods Tests

    [Fact]
    public void MapToViewProperties_MapsCorrectly()
    {
        // Arrange
        var errors = new Dictionary<string, string>
        {
            { "Name", "Name is required" },
            { "Email", "Email is invalid" }
        };

        // Act
        var mapped = errors.MapToViewProperties("Customer");

        // Assert
        mapped.Should().ContainKey("CustomerName");
        mapped.Should().ContainKey("CustomerEmail");
        mapped["CustomerName"].Should().Be("Name is required");
        mapped["CustomerEmail"].Should().Be("Email is invalid");
    }

    [Fact]
    public void ToErrorMessage_WithErrors_ReturnsFormattedMessage()
    {
        // Arrange
        var errors = new Dictionary<string, string>
        {
            { "Name", "Name is required" },
            { "Email", "Email is invalid" }
        };

        // Act
        var message = errors.ToErrorMessage();

        // Assert
        message.Should().NotBeNull();
        message.Should().Contain("Name");
        message.Should().Contain("Email");
        message.Should().Contain("required");
        message.Should().Contain("invalid");
    }

    [Fact]
    public void ToErrorMessage_NoErrors_ReturnsNull()
    {
        // Arrange
        var errors = new Dictionary<string, string>();

        // Act
        var message = errors.ToErrorMessage();

        // Assert
        message.Should().BeNull();
    }

    #endregion

    #region Helper Methods

    private static Customer CreateValidCustomer()
    {
        return new Customer
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Phone = "+1-555-123-4567",
            Address = "123 Main St",
            City = "New York",
            Country = "USA",
            IsActive = true
        };
    }

    #endregion
}
