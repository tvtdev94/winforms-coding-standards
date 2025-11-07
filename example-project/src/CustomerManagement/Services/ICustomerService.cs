using CustomerManagement.Models;

namespace CustomerManagement.Services;

/// <summary>
/// Service interface for customer business logic.
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// Gets all customers.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of all customers.</returns>
    Task<List<Customer>> GetAllCustomersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a customer by identifier.
    /// </summary>
    /// <param name="customerId">The customer identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The customer if found; otherwise, null.</returns>
    Task<Customer?> GetCustomerByIdAsync(int customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a customer with their orders.
    /// </summary>
    /// <param name="customerId">The customer identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The customer with orders if found; otherwise, null.</returns>
    Task<Customer?> GetCustomerWithOrdersAsync(int customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets only active customers.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of active customers.</returns>
    Task<List<Customer>> GetActiveCustomersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches customers by name.
    /// </summary>
    /// <param name="searchTerm">The search term.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of matching customers.</returns>
    Task<List<Customer>> SearchCustomersAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new customer.
    /// </summary>
    /// <param name="customer">The customer to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created customer.</returns>
    /// <exception cref="InvalidOperationException">Thrown when email already exists.</exception>
    Task<Customer> CreateCustomerAsync(Customer customer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing customer.
    /// </summary>
    /// <param name="customer">The customer to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when customer not found or email already exists.</exception>
    Task UpdateCustomerAsync(Customer customer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a customer.
    /// </summary>
    /// <param name="customerId">The customer identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when customer not found.</exception>
    Task DeleteCustomerAsync(int customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates customer data.
    /// </summary>
    /// <param name="customer">The customer to validate.</param>
    /// <returns>A dictionary of validation errors (empty if valid).</returns>
    Dictionary<string, string> ValidateCustomer(Customer customer);

    /// <summary>
    /// Checks if an email is already in use.
    /// </summary>
    /// <param name="email">The email to check.</param>
    /// <param name="excludeCustomerId">Optional customer ID to exclude (for updates).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if email exists; otherwise, false.</returns>
    Task<bool> EmailExistsAsync(string email, int? excludeCustomerId = null, CancellationToken cancellationToken = default);
}
