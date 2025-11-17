// Template: Repository Pattern with EF Core (Unit of Work Pattern)
// Replace: YourEntity, YourRepository, AppDbContext
// Note: Repository does NOT call SaveChangesAsync - that's handled by Unit of Work

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace YourNamespace.Data.Repositories
{
    /// <summary>
    /// Generic repository interface for common CRUD operations.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Repository interface for YourEntity-specific operations.
    /// Extends the generic repository with custom methods.
    /// </summary>
    public interface IYourRepository : IRepository<YourEntity>
    {
        // Add entity-specific methods here
        Task<YourEntity?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<List<YourEntity>> GetActiveAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// EF Core implementation of YourEntity repository.
    /// IMPORTANT: Does NOT call SaveChangesAsync - managed by Unit of Work.
    /// </summary>
    public class YourRepository : IYourRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="YourRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public YourRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc/>
        public async Task<List<YourEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.YourEntities
                .AsNoTracking()
                .OrderBy(e => e.Name) // Adjust ordering as needed
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<YourEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.YourEntities
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<List<YourEntity>> FindAsync(Expression<Func<YourEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.YourEntities
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<YourEntity> AddAsync(YourEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await _context.YourEntities.AddAsync(entity, cancellationToken);
            // Note: SaveChanges is handled by Unit of Work
            return entity;
        }

        /// <inheritdoc/>
        public Task UpdateAsync(YourEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _context.YourEntities.Update(entity);
            // Note: SaveChanges is handled by Unit of Work
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task DeleteAsync(YourEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _context.YourEntities.Remove(entity);
            // Note: SaveChanges is handled by Unit of Work
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task<int> CountAsync(Expression<Func<YourEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
            {
                return await _context.YourEntities.CountAsync(cancellationToken);
            }

            return await _context.YourEntities.CountAsync(predicate, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<bool> AnyAsync(Expression<Func<YourEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.YourEntities.AnyAsync(predicate, cancellationToken);
        }

        // Entity-specific methods

        /// <inheritdoc/>
        public async Task<YourEntity?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            return await _context.YourEntities
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Name == name, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<List<YourEntity>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return await _context.YourEntities
                .AsNoTracking()
                .Where(e => e.IsActive)
                .OrderBy(e => e.Name)
                .ToListAsync(cancellationToken);
        }
    }
}
