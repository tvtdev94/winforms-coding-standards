// Template: Service Layer with Interface
// Replace: YourEntity, YourService, YourRepository

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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
        Task<List<YourEntity>> GetAllAsync();

        /// <summary>
        /// Gets a YourEntity by ID.
        /// </summary>
        /// <param name="id">The entity ID.</param>
        Task<YourEntity?> GetByIdAsync(int id);

        /// <summary>
        /// Saves a YourEntity.
        /// </summary>
        /// <param name="entity">The entity to save.</param>
        Task<bool> SaveAsync(YourEntity entity);

        /// <summary>
        /// Deletes a YourEntity.
        /// </summary>
        /// <param name="id">The entity ID to delete.</param>
        Task<bool> DeleteAsync(int id);
    }

    /// <summary>
    /// Implementation of YourEntity business operations.
    /// </summary>
    public class YourService : IYourService
    {
        private readonly IYourRepository _repository;
        private readonly ILogger<YourService> _logger;

        /// <summary>
        /// Initializes a new instance of the YourService class.
        /// </summary>
        public YourService(IYourRepository repository, ILogger<YourService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<YourEntity>> GetAllAsync()
        {
            _logger.LogInformation("Getting all YourEntity items");

            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all YourEntity items");
                throw;
            }
        }

        public async Task<YourEntity?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Getting YourEntity with ID: {Id}", id);

            try
            {
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting YourEntity with ID: {Id}", id);
                throw;
            }
        }

        public async Task<bool> SaveAsync(YourEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _logger.LogInformation("Saving YourEntity: {Id}", entity.Id);

            try
            {
                // Business validation
                ValidateEntity(entity);

                // Save via repository
                return await _repository.SaveAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving YourEntity: {Id}", entity.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting YourEntity: {Id}", id);

            try
            {
                return await _repository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting YourEntity: {Id}", id);
                throw;
            }
        }

        private void ValidateEntity(YourEntity entity)
        {
            // Add business validation rules here
            // throw new ValidationException("...") if invalid
        }
    }
}
