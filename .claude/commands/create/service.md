---
description: Create a new service class with dependency injection and best practices
---

You are tasked with creating a new service class following WinForms best practices.

## Workflow

1. **Ask for Service Information**
   - Service name (e.g., "CustomerService", "OrderService", "AuthenticationService")
   - What entity/domain does it manage?
   - What operations are needed? (CRUD, business logic, etc.)

2. **Read Templates and Documentation**
   - Read `templates/service-template.cs` as the base
   - Reference `docs/best-practices/async-await.md` for async patterns
   - Reference `docs/best-practices/error-handling.md` for error handling

3. **Create Service Files**

   Generate **TWO files**:

   ### File 1: Interface (`I{ServiceName}.cs`)
   ```csharp
   /// <summary>
   /// Interface for {ServiceName}
   /// </summary>
   public interface I{ServiceName}
   {
       // Define all public methods as async
       Task<List<{Entity}>> GetAllAsync(CancellationToken cancellationToken = default);
       Task<{Entity}?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
       Task<{Entity}> CreateAsync({Entity} entity, CancellationToken cancellationToken = default);
       Task UpdateAsync({Entity} entity, CancellationToken cancellationToken = default);
       Task DeleteAsync(int id, CancellationToken cancellationToken = default);
       // Add custom business logic methods
   }
   ```

   ### File 2: Implementation (`{ServiceName}.cs`)

   **MUST include:**
   - ✅ Namespace matching project structure
   - ✅ XML documentation comments on all public methods
   - ✅ Constructor with dependency injection (ILogger, IRepository)
   - ✅ Null checks with ArgumentNullException
   - ✅ All methods are async (Task/Task<T>)
   - ✅ Try-catch blocks with logging
   - ✅ Input validation before processing
   - ✅ CancellationToken support
   - ✅ Meaningful error messages

   **MUST NOT:**
   - ❌ Use synchronous methods for I/O operations
   - ❌ Catch exceptions without logging
   - ❌ Return null (use nullable reference types)
   - ❌ Have business logic in constructors
   - ❌ Use magic strings or numbers

4. **Generate Complete Code**

```csharp
using Microsoft.Extensions.Logging;

namespace YourApp.Services;

/// <summary>
/// Service for managing {Entity} operations
/// </summary>
public class {ServiceName} : I{ServiceName}
{
    private readonly I{Entity}Repository _repository;
    private readonly ILogger<{ServiceName}> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="{ServiceName}"/> class.
    /// </summary>
    /// <param name="repository">The {entity} repository</param>
    /// <param name="logger">The logger instance</param>
    /// <exception cref="ArgumentNullException">Thrown when repository or logger is null</exception>
    public {ServiceName}(
        I{Entity}Repository repository,
        ILogger<{ServiceName}> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets all {entities} asynchronously
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all {entities}</returns>
    public async Task<List<{Entity}>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all {entities}");

            var entities = await _repository.GetAllAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} {entities}", entities.Count, "{entities}");
            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all {entities}");
            throw new InvalidOperationException("Failed to retrieve {entities}.", ex);
        }
    }

    /// <summary>
    /// Gets a {entity} by ID asynchronously
    /// </summary>
    /// <param name="id">The {entity} ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The {entity} if found, null otherwise</returns>
    public async Task<{Entity}?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id <= 0)
                throw new ArgumentException("ID must be greater than zero.", nameof(id));

            _logger.LogInformation("Retrieving {entity} with ID: {Id}", "{entity}", id);

            var entity = await _repository.GetByIdAsync(id, cancellationToken);

            if (entity == null)
            {
                _logger.LogWarning("{Entity} with ID {Id} not found", "{Entity}", id);
            }
            else
            {
                _logger.LogInformation("{Entity} with ID {Id} retrieved successfully", "{Entity}", id);
            }

            return entity;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving {entity} with ID: {Id}", "{entity}", id);
            throw new InvalidOperationException($"Failed to retrieve {entity} with ID {id}.", ex);
        }
    }

    /// <summary>
    /// Creates a new {entity} asynchronously
    /// </summary>
    /// <param name="entity">The {entity} to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created {entity}</returns>
    /// <exception cref="ArgumentNullException">Thrown when entity is null</exception>
    /// <exception cref="ArgumentException">Thrown when validation fails</exception>
    public async Task<{Entity}> CreateAsync({Entity} entity, CancellationToken cancellationToken = default)
    {
        try
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _logger.LogInformation("Creating new {entity}");

            // Validate entity
            Validate{Entity}(entity);

            // Business logic validation
            await ValidateBusinessRulesAsync(entity, cancellationToken);

            var created = await _repository.AddAsync(entity, cancellationToken);

            _logger.LogInformation("{Entity} created successfully with ID: {Id}", "{Entity}", created.Id);
            return created;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating {entity}");
            throw new InvalidOperationException("Failed to create {entity}.", ex);
        }
    }

    /// <summary>
    /// Updates an existing {entity} asynchronously
    /// </summary>
    /// <param name="entity">The {entity} to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="ArgumentNullException">Thrown when entity is null</exception>
    /// <exception cref="ArgumentException">Thrown when validation fails</exception>
    public async Task UpdateAsync({Entity} entity, CancellationToken cancellationToken = default)
    {
        try
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id <= 0)
                throw new ArgumentException("Invalid entity ID.", nameof(entity));

            _logger.LogInformation("Updating {entity} with ID: {Id}", "{entity}", entity.Id);

            // Check if exists
            var existing = await _repository.GetByIdAsync(entity.Id, cancellationToken);
            if (existing == null)
                throw new ArgumentException($"{Entity} with ID {entity.Id} not found.");

            // Validate entity
            Validate{Entity}(entity);

            // Business logic validation
            await ValidateBusinessRulesAsync(entity, cancellationToken);

            await _repository.UpdateAsync(entity, cancellationToken);

            _logger.LogInformation("{Entity} with ID {Id} updated successfully", "{Entity}", entity.Id);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating {entity} with ID: {Id}", "{entity}", entity.Id);
            throw new InvalidOperationException($"Failed to update {entity}.", ex);
        }
    }

    /// <summary>
    /// Deletes a {entity} by ID asynchronously
    /// </summary>
    /// <param name="id">The {entity} ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="ArgumentException">Thrown when ID is invalid or entity not found</exception>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id <= 0)
                throw new ArgumentException("ID must be greater than zero.", nameof(id));

            _logger.LogInformation("Deleting {entity} with ID: {Id}", "{entity}", id);

            // Check if exists
            var existing = await _repository.GetByIdAsync(id, cancellationToken);
            if (existing == null)
                throw new ArgumentException($"{Entity} with ID {id} not found.");

            // Business logic validation (e.g., check if can be deleted)
            await ValidateCanDeleteAsync(id, cancellationToken);

            await _repository.DeleteAsync(id, cancellationToken);

            _logger.LogInformation("{Entity} with ID {Id} deleted successfully", "{Entity}", id);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting {entity} with ID: {Id}", "{entity}", id);
            throw new InvalidOperationException($"Failed to delete {entity}.", ex);
        }
    }

    #region Private Helper Methods

    /// <summary>
    /// Validates the {entity} entity
    /// </summary>
    private void Validate{Entity}({Entity} entity)
    {
        // Add validation logic here
        // Example:
        // if (string.IsNullOrWhiteSpace(entity.Name))
        //     throw new ArgumentException("Name is required.", nameof(entity));

        // if (entity.Name.Length > 100)
        //     throw new ArgumentException("Name cannot exceed 100 characters.", nameof(entity));
    }

    /// <summary>
    /// Validates business rules for the {entity}
    /// </summary>
    private async Task ValidateBusinessRulesAsync({Entity} entity, CancellationToken cancellationToken)
    {
        // Add business logic validation here
        // Example:
        // - Check for duplicates
        // - Validate relationships
        // - Check business constraints

        await Task.CompletedTask; // Remove when implementing actual validation
    }

    /// <summary>
    /// Validates if a {entity} can be deleted
    /// </summary>
    private async Task ValidateCanDeleteAsync(int id, CancellationToken cancellationToken)
    {
        // Add deletion validation logic here
        // Example:
        // - Check if entity has related records
        // - Check business rules for deletion

        await Task.CompletedTask; // Remove when implementing actual validation
    }

    #endregion
}
```

5. **Register in DI Container**

   Show the user how to register in `Program.cs`:
   ```csharp
   // In ConfigureServices method:
   services.AddScoped<I{ServiceName}, {ServiceName}>();
   ```

6. **Suggest Unit Tests**

   Ask: "Would you like me to generate unit tests for this service using /add-test?"

## Best Practices Checklist

Before finishing, verify:
- ✅ Interface and implementation created
- ✅ All methods are async with CancellationToken support
- ✅ Proper dependency injection
- ✅ XML documentation on all public members
- ✅ Input validation with meaningful errors
- ✅ Try-catch with logging
- ✅ No magic strings or numbers
- ✅ Follows service-template.cs structure
- ✅ DI registration instructions provided

## Common Service Types

### CRUD Service
```csharp
GetAllAsync, GetByIdAsync, CreateAsync, UpdateAsync, DeleteAsync
```

### Authentication Service
```csharp
LoginAsync, LogoutAsync, ValidateTokenAsync, RefreshTokenAsync, ChangePasswordAsync
```

### Report Service
```csharp
GenerateReportAsync, ExportToPdfAsync, ExportToExcelAsync, GetReportDataAsync
```

### Notification Service
```csharp
SendEmailAsync, SendSmsAsync, CreateNotificationAsync, GetNotificationsAsync
```

### File Service
```csharp
UploadFileAsync, DownloadFileAsync, DeleteFileAsync, GetFileInfoAsync
```

## Example Output Structure

```
/Services
    ├── ICustomerService.cs          (Interface)
    ├── CustomerService.cs           (Implementation)
    └── [Register in Program.cs]
```

## Notes

- **Always use async/await** for I/O operations
- **Log everything**: Info for operations, Error for exceptions, Warning for business rule violations
- **Validate early**: Check inputs at the start of each method
- **Use CancellationToken**: Allow operations to be cancelled
- **Throw meaningful exceptions**: Help developers understand what went wrong
- **Follow Single Responsibility**: One service per domain/entity
