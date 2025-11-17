using CustomerManagement.Data;
using CustomerManagement.Models;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace CustomerManagement.Services;

/// <summary>
/// Service implementation for customer business logic.
/// </summary>
public partial class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CustomerService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="logger">The logger.</param>
    public CustomerService(
        IUnitOfWork unitOfWork,
        ILogger<CustomerService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<List<Customer>> GetAllCustomersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting all customers");
            var customers = await _unitOfWork.Customers.GetAllAsync(cancellationToken);
            _logger.LogInformation("Retrieved {Count} customers", customers.Count);
            return customers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all customers");
            throw new InvalidOperationException("Failed to retrieve customers.", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<Customer?> GetCustomerByIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting customer with ID: {CustomerId}", customerId);
            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId, cancellationToken);

            if (customer == null)
            {
                _logger.LogWarning("Customer with ID {CustomerId} not found", customerId);
            }

            return customer;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer with ID: {CustomerId}", customerId);
            throw new InvalidOperationException($"Failed to retrieve customer with ID {customerId}.", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<Customer?> GetCustomerWithOrdersAsync(int customerId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting customer with orders, ID: {CustomerId}", customerId);
            var customer = await _unitOfWork.Customers.GetWithOrdersAsync(customerId, cancellationToken);

            if (customer == null)
            {
                _logger.LogWarning("Customer with ID {CustomerId} not found", customerId);
            }
            else
            {
                _logger.LogInformation("Retrieved customer {CustomerId} with {OrderCount} orders",
                    customerId, customer.Orders.Count);
            }

            return customer;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer with orders, ID: {CustomerId}", customerId);
            throw new InvalidOperationException($"Failed to retrieve customer with orders, ID {customerId}.", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<List<Customer>> GetActiveCustomersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting active customers");
            var customers = await _unitOfWork.Customers.GetActiveCustomersAsync(cancellationToken);
            _logger.LogInformation("Retrieved {Count} active customers", customers.Count);
            return customers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active customers");
            throw new InvalidOperationException("Failed to retrieve active customers.", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<List<Customer>> SearchCustomersAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching customers with term: {SearchTerm}", searchTerm);
            var customers = await _unitOfWork.Customers.SearchByNameAsync(searchTerm, cancellationToken);
            _logger.LogInformation("Found {Count} customers matching '{SearchTerm}'", customers.Count, searchTerm);
            return customers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching customers with term: {SearchTerm}", searchTerm);
            throw new InvalidOperationException($"Failed to search customers with term '{searchTerm}'.", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<Customer> CreateCustomerAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        if (customer == null)
        {
            throw new ArgumentNullException(nameof(customer));
        }

        try
        {
            _logger.LogInformation("Creating customer: {Email}", customer.Email);

            // Validate customer data
            var validationErrors = ValidateCustomer(customer);
            if (validationErrors.Any())
            {
                var errorMessage = string.Join("; ", validationErrors.Values);
                _logger.LogWarning("Validation failed for customer: {Errors}", errorMessage);
                throw new InvalidOperationException($"Validation failed: {errorMessage}");
            }

            // Check if email already exists
            var emailExists = await EmailExistsAsync(customer.Email, null, cancellationToken);
            if (emailExists)
            {
                _logger.LogWarning("Email already exists: {Email}", customer.Email);
                throw new InvalidOperationException($"A customer with email '{customer.Email}' already exists.");
            }

            // Set creation timestamp
            customer.CreatedAt = DateTime.UtcNow;
            customer.UpdatedAt = null;

            // Add customer and save changes via Unit of Work
            var createdCustomer = await _unitOfWork.Customers.AddAsync(customer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Customer created successfully with ID: {CustomerId}", createdCustomer.Id);

            return createdCustomer;
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer: {Email}", customer.Email);
            throw new InvalidOperationException("Failed to create customer.", ex);
        }
    }

    /// <inheritdoc/>
    public async Task UpdateCustomerAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        if (customer == null)
        {
            throw new ArgumentNullException(nameof(customer));
        }

        try
        {
            _logger.LogInformation("Updating customer with ID: {CustomerId}", customer.Id);

            // Check if customer exists
            var existingCustomer = await _unitOfWork.Customers.GetByIdAsync(customer.Id, cancellationToken);
            if (existingCustomer == null)
            {
                _logger.LogWarning("Customer with ID {CustomerId} not found", customer.Id);
                throw new InvalidOperationException($"Customer with ID {customer.Id} not found.");
            }

            // Validate customer data
            var validationErrors = ValidateCustomer(customer);
            if (validationErrors.Any())
            {
                var errorMessage = string.Join("; ", validationErrors.Values);
                _logger.LogWarning("Validation failed for customer ID {CustomerId}: {Errors}",
                    customer.Id, errorMessage);
                throw new InvalidOperationException($"Validation failed: {errorMessage}");
            }

            // Check if email already exists (excluding current customer)
            var emailExists = await EmailExistsAsync(customer.Email, customer.Id, cancellationToken);
            if (emailExists)
            {
                _logger.LogWarning("Email already exists: {Email}", customer.Email);
                throw new InvalidOperationException($"A customer with email '{customer.Email}' already exists.");
            }

            // Update customer and save changes via Unit of Work
            await _unitOfWork.Customers.UpdateAsync(customer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Customer with ID {CustomerId} updated successfully", customer.Id);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer with ID: {CustomerId}", customer.Id);
            throw new InvalidOperationException($"Failed to update customer with ID {customer.Id}.", ex);
        }
    }

    /// <inheritdoc/>
    public async Task DeleteCustomerAsync(int customerId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting customer with ID: {CustomerId}", customerId);

            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId, cancellationToken);
            if (customer == null)
            {
                _logger.LogWarning("Customer with ID {CustomerId} not found", customerId);
                throw new InvalidOperationException($"Customer with ID {customerId} not found.");
            }

            // Delete customer and save changes via Unit of Work
            await _unitOfWork.Customers.DeleteAsync(customer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Customer with ID {CustomerId} deleted successfully", customerId);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer with ID: {CustomerId}", customerId);
            throw new InvalidOperationException($"Failed to delete customer with ID {customerId}.", ex);
        }
    }

    /// <inheritdoc/>
    public Dictionary<string, string> ValidateCustomer(Customer customer)
    {
        var errors = new Dictionary<string, string>();

        // Validate Name
        if (string.IsNullOrWhiteSpace(customer.Name))
        {
            errors[nameof(customer.Name)] = "Name is required.";
        }
        else if (customer.Name.Length > 100)
        {
            errors[nameof(customer.Name)] = "Name cannot exceed 100 characters.";
        }

        // Validate Email
        if (string.IsNullOrWhiteSpace(customer.Email))
        {
            errors[nameof(customer.Email)] = "Email is required.";
        }
        else if (customer.Email.Length > 100)
        {
            errors[nameof(customer.Email)] = "Email cannot exceed 100 characters.";
        }
        else if (!IsValidEmail(customer.Email))
        {
            errors[nameof(customer.Email)] = "Email format is invalid.";
        }

        // Validate Phone (optional, but must be valid if provided)
        if (!string.IsNullOrWhiteSpace(customer.Phone))
        {
            if (customer.Phone.Length > 20)
            {
                errors[nameof(customer.Phone)] = "Phone cannot exceed 20 characters.";
            }
            else if (!IsValidPhone(customer.Phone))
            {
                errors[nameof(customer.Phone)] = "Phone format is invalid.";
            }
        }

        // Validate Address
        if (!string.IsNullOrWhiteSpace(customer.Address) && customer.Address.Length > 200)
        {
            errors[nameof(customer.Address)] = "Address cannot exceed 200 characters.";
        }

        // Validate City
        if (!string.IsNullOrWhiteSpace(customer.City) && customer.City.Length > 50)
        {
            errors[nameof(customer.City)] = "City cannot exceed 50 characters.";
        }

        // Validate Country
        if (!string.IsNullOrWhiteSpace(customer.Country) && customer.Country.Length > 50)
        {
            errors[nameof(customer.Country)] = "Country cannot exceed 50 characters.";
        }

        return errors;
    }

    /// <inheritdoc/>
    public async Task<bool> EmailExistsAsync(string email, int? excludeCustomerId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var customer = await _unitOfWork.Customers.GetByEmailAsync(email, cancellationToken);

        if (customer == null)
        {
            return false;
        }

        // If excluding a customer ID (for updates), check if it's a different customer
        if (excludeCustomerId.HasValue)
        {
            return customer.Id != excludeCustomerId.Value;
        }

        return true;
    }

    /// <summary>
    /// Validates email format using regex.
    /// </summary>
    private static bool IsValidEmail(string email)
    {
        return EmailRegex().IsMatch(email);
    }

    /// <summary>
    /// Validates phone format (allows various international formats).
    /// </summary>
    private static bool IsValidPhone(string phone)
    {
        return PhoneRegex().IsMatch(phone);
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"^[\d\s\-\+\(\)]+$")]
    private static partial Regex PhoneRegex();
}
