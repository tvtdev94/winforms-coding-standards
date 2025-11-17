using CustomerManagement.Data;
using CustomerManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CustomerManagement.Repositories;

/// <summary>
/// Repository implementation for Customer entity.
/// </summary>
public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public CustomerRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public async Task<List<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<Customer>> FindAsync(Expression<Func<Customer, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Customer> AddAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        await _context.Customers.AddAsync(entity, cancellationToken);
        // Note: SaveChanges is now handled by Unit of Work
        return entity;
    }

    /// <inheritdoc/>
    public Task UpdateAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        _context.Customers.Update(entity);
        // Note: SaveChanges is now handled by Unit of Work
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task DeleteAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        _context.Customers.Remove(entity);
        // Note: SaveChanges is now handled by Unit of Work
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task<int> CountAsync(Expression<Func<Customer, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
        {
            return await _context.Customers.CountAsync(cancellationToken);
        }

        return await _context.Customers.CountAsync(predicate, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> AnyAsync(Expression<Func<Customer, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Customers.AnyAsync(predicate, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));
        }

        return await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Customer?> GetWithOrdersAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<Customer>> GetActiveCustomersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<Customer>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllAsync(cancellationToken);
        }

        var lowerSearchTerm = searchTerm.ToLower();

        return await _context.Customers
            .AsNoTracking()
            .Where(c => c.Name.ToLower().Contains(lowerSearchTerm))
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<Customer>> GetByCityAsync(string city, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("City cannot be null or empty.", nameof(city));
        }

        return await _context.Customers
            .AsNoTracking()
            .Where(c => c.City == city)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }
}
