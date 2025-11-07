using CustomerManagement.Models;

namespace CustomerManagement.Repositories;

/// <summary>
/// Repository interface for Customer-specific operations.
/// </summary>
public interface ICustomerRepository : IRepository<Customer>
{
    /// <summary>
    /// Gets a customer by email address.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The customer if found; otherwise, null.</returns>
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a customer with their orders included.
    /// </summary>
    /// <param name="customerId">The customer identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The customer with orders if found; otherwise, null.</returns>
    Task<Customer?> GetWithOrdersAsync(int customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active customers.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of active customers.</returns>
    Task<List<Customer>> GetActiveCustomersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches customers by name (case-insensitive).
    /// </summary>
    /// <param name="searchTerm">The search term.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of matching customers.</returns>
    Task<List<Customer>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets customers by city.
    /// </summary>
    /// <param name="city">The city name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of customers in the specified city.</returns>
    Task<List<Customer>> GetByCity Async(string city, CancellationToken cancellationToken = default);
}
