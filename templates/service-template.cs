// Template: Service Layer with Unit of Work Pattern
// Replace: YourEntity, YourService, IUnitOfWork
// IMPORTANT: Use IUnitOfWork instead of IRepository, call SaveChangesAsync after modifications

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using YourNamespace.Data;

namespace YourNamespace.Services
{
    /// <summary>
    /// Service interface for YourEntity business operations.
    /// </summary>
    public interface IYourService
    {
        /// <summary>
        /// Gets all YourEntity items.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<List<YourEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a YourEntity by ID.
        /// </summary>
        /// <param name="id">The entity ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<YourEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new YourEntity.
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<YourEntity> CreateAsync(YourEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing YourEntity.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task UpdateAsync(YourEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a YourEntity.
        /// </summary>
        /// <param name="id">The entity ID to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates a YourEntity.
        /// </summary>
        /// <param name="entity">The entity to validate.</param>
        /// <returns>Dictionary of validation errors (empty if valid).</returns>
        Dictionary<string, string> Validate(YourEntity entity);
    }

    /// <summary>
    /// Implementation of YourEntity business operations.
    /// Uses Unit of Work pattern for transaction management.
    /// </summary>
    public class YourService : IYourService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<YourService> _logger;

        /// <summary>
        /// Initializes a new instance of the YourService class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="logger">The logger.</param>
        public YourService(IUnitOfWork unitOfWork, ILogger<YourService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<List<YourEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting all YourEntity items");
                var entities = await _unitOfWork.YourEntities.GetAllAsync(cancellationToken);
                _logger.LogInformation("Retrieved {Count} YourEntity items", entities.Count);
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all YourEntity items");
                throw new InvalidOperationException("Failed to retrieve YourEntity items.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<YourEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting YourEntity with ID: {Id}", id);
                var entity = await _unitOfWork.YourEntities.GetByIdAsync(id, cancellationToken);

                if (entity == null)
                {
                    _logger.LogWarning("YourEntity with ID {Id} not found", id);
                }

                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving YourEntity with ID: {Id}", id);
                throw new InvalidOperationException($"Failed to retrieve YourEntity with ID {id}.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<YourEntity> CreateAsync(YourEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                _logger.LogInformation("Creating YourEntity");

                // Validate entity data
                var validationErrors = Validate(entity);
                if (validationErrors.Any())
                {
                    var errorMessage = string.Join("; ", validationErrors.Values);
                    _logger.LogWarning("Validation failed for YourEntity: {Errors}", errorMessage);
                    throw new InvalidOperationException($"Validation failed: {errorMessage}");
                }

                // Add entity and save changes via Unit of Work
                var createdEntity = await _unitOfWork.YourEntities.AddAsync(entity, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("YourEntity created successfully with ID: {Id}", createdEntity.Id);

                return createdEntity;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating YourEntity");
                throw new InvalidOperationException("Failed to create YourEntity.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(YourEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                _logger.LogInformation("Updating YourEntity with ID: {Id}", entity.Id);

                // Check if entity exists
                var existingEntity = await _unitOfWork.YourEntities.GetByIdAsync(entity.Id, cancellationToken);
                if (existingEntity == null)
                {
                    _logger.LogWarning("YourEntity with ID {Id} not found", entity.Id);
                    throw new InvalidOperationException($"YourEntity with ID {entity.Id} not found.");
                }

                // Validate entity data
                var validationErrors = Validate(entity);
                if (validationErrors.Any())
                {
                    var errorMessage = string.Join("; ", validationErrors.Values);
                    _logger.LogWarning("Validation failed for YourEntity ID {Id}: {Errors}",
                        entity.Id, errorMessage);
                    throw new InvalidOperationException($"Validation failed: {errorMessage}");
                }

                // Update entity and save changes via Unit of Work
                await _unitOfWork.YourEntities.UpdateAsync(entity, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("YourEntity with ID {Id} updated successfully", entity.Id);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating YourEntity with ID: {Id}", entity.Id);
                throw new InvalidOperationException($"Failed to update YourEntity with ID {entity.Id}.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Deleting YourEntity with ID: {Id}", id);

                var entity = await _unitOfWork.YourEntities.GetByIdAsync(id, cancellationToken);
                if (entity == null)
                {
                    _logger.LogWarning("YourEntity with ID {Id} not found", id);
                    throw new InvalidOperationException($"YourEntity with ID {id} not found.");
                }

                // Delete entity and save changes via Unit of Work
                await _unitOfWork.YourEntities.DeleteAsync(entity, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("YourEntity with ID {Id} deleted successfully", id);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting YourEntity with ID: {Id}", id);
                throw new InvalidOperationException($"Failed to delete YourEntity with ID {id}.", ex);
            }
        }

        /// <inheritdoc/>
        public Dictionary<string, string> Validate(YourEntity entity)
        {
            var errors = new Dictionary<string, string>();

            // Add business validation rules here
            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                errors[nameof(entity.Name)] = "Name is required.";
            }
            else if (entity.Name.Length > 100)
            {
                errors[nameof(entity.Name)] = "Name cannot exceed 100 characters.";
            }

            // Add more validation rules as needed

            return errors;
        }
    }
}
