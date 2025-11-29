---
description: Create a new repository class with Entity Framework Core
---

You are tasked with creating a new repository class following the Repository pattern with Entity Framework Core.

---

## 🔥 STEP 0: MANDATORY Context Loading (DO THIS FIRST!)

**Before ANY code generation, you MUST:**

### 1. Read Project Configuration
```
READ: .claude/project-context.md
```
Extract: `UI_FRAMEWORK`, `DATABASE`, `PATTERN`, `FRAMEWORK`

### 2. Load Required Templates & Guides
- `templates/repository-template.cs` → Repository structure
- `templates/unitofwork-template.cs` → UoW integration
- `docs/data-access/unit-of-work-pattern.md` → UoW pattern
- `docs/data-access/repository-pattern.md` → Repository rules

### 3. Critical Rules

| 🚫 NEVER | ✅ ALWAYS |
|----------|----------|
| SaveChanges in Repository | SaveChanges in UoW only |
| Return IQueryable | Return List/concrete types |
| Skip AsNoTracking | AsNoTracking for reads |
| Generate without template | Start from template |

**⚠️ If project-context.md doesn't exist**: Ask user for database preference.

---

## Workflow

1. **Ask for Repository Information**
   - Entity name (e.g., "Customer", "Order", "Product")
   - Does it need generic repository or specific repository?
   - What special queries are needed beyond basic CRUD?

2. **Read Templates and Documentation**
   - Read `templates/repository-template.cs` as the base
   - Reference `docs/data-access/entity-framework.md` for EF Core patterns
   - Reference `docs/data-access/repository-pattern.md` for repository patterns

3. **Create Repository Files**

   Generate **TWO or THREE files** depending on needs:

   ### Option 1: Generic Repository (Recommended for simple CRUD)

   **File 1: Generic Interface (`IRepository.cs`)**
   ```csharp
   /// <summary>
   /// Generic repository interface for common CRUD operations
   /// </summary>
   public interface IRepository<T> where T : class
   {
       Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
       Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
       Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
       Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
       Task DeleteAsync(int id, CancellationToken cancellationToken = default);
       Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
   }
   ```

   **File 2: Specific Interface (`I{Entity}Repository.cs`)**
   ```csharp
   /// <summary>
   /// Repository interface for {Entity} entity
   /// </summary>
   public interface I{Entity}Repository : IRepository<{Entity}>
   {
       // Add custom query methods here
       Task<List<{Entity}>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
       Task<List<{Entity}>> GetActiveAsync(CancellationToken cancellationToken = default);
       // Add more custom methods as needed
   }
   ```

   ### Option 2: Specific Repository Only (For complex entities)

   **File: I{Entity}Repository.cs**
   ```csharp
   /// <summary>
   /// Repository interface for {Entity} entity
   /// </summary>
   public interface I{Entity}Repository
   {
       // All CRUD methods explicitly defined
       Task<List<{Entity}>> GetAllAsync(CancellationToken cancellationToken = default);
       Task<{Entity}?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
       Task<{Entity}> AddAsync({Entity} entity, CancellationToken cancellationToken = default);
       Task UpdateAsync({Entity} entity, CancellationToken cancellationToken = default);
       Task DeleteAsync(int id, CancellationToken cancellationToken = default);

       // Custom methods
       Task<List<{Entity}>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
   }
   ```

4. **Generate Repository Implementation**

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace YourApp.Repositories;

/// <summary>
/// Repository implementation for {Entity} entity
/// </summary>
public class {Entity}Repository : I{Entity}Repository
{
    private readonly AppDbContext _context;
    private readonly ILogger<{Entity}Repository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="{Entity}Repository"/> class.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="logger">The logger instance</param>
    /// <exception cref="ArgumentNullException">Thrown when context or logger is null</exception>
    public {Entity}Repository(
        AppDbContext context,
        ILogger<{Entity}Repository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
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
            _logger.LogDebug("Retrieving all {entities} from database");

            var entities = await _context.{Entities}
                .AsNoTracking()
                // .Include(x => x.RelatedEntity) // Add includes if needed
                .ToListAsync(cancellationToken);

            _logger.LogDebug("Retrieved {Count} {entities}", entities.Count, "{entities}");
            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all {entities} from database");
            throw new InvalidOperationException("Failed to retrieve {entities} from database.", ex);
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

            _logger.LogDebug("Retrieving {entity} with ID: {Id}", "{entity}", id);

            var entity = await _context.{Entities}
                .AsNoTracking()
                // .Include(x => x.RelatedEntity) // Add includes if needed
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            if (entity == null)
            {
                _logger.LogDebug("{Entity} with ID {Id} not found", "{Entity}", id);
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
            throw new InvalidOperationException($"Failed to retrieve {entity} from database.", ex);
        }
    }

    /// <summary>
    /// Adds a new {entity} to the database asynchronously
    /// </summary>
    /// <param name="entity">The {entity} to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added {entity} with generated ID</returns>
    /// <exception cref="ArgumentNullException">Thrown when entity is null</exception>
    public async Task<{Entity}> AddAsync({Entity} entity, CancellationToken cancellationToken = default)
    {
        try
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _logger.LogDebug("Adding new {entity} to database");

            await _context.{Entities}.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("{Entity} added successfully with ID: {Id}", "{Entity}", entity.Id);
            return entity;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while adding {entity}", "{entity}");
            throw new InvalidOperationException("Failed to add {entity} to database.", ex);
        }
        catch (ArgumentNullException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding {entity} to database", "{entity}");
            throw new InvalidOperationException("Failed to add {entity}.", ex);
        }
    }

    /// <summary>
    /// Updates an existing {entity} in the database asynchronously
    /// </summary>
    /// <param name="entity">The {entity} to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="ArgumentNullException">Thrown when entity is null</exception>
    /// <exception cref="ArgumentException">Thrown when entity ID is invalid</exception>
    public async Task UpdateAsync({Entity} entity, CancellationToken cancellationToken = default)
    {
        try
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id <= 0)
                throw new ArgumentException("Invalid entity ID.", nameof(entity));

            _logger.LogDebug("Updating {entity} with ID: {Id}", "{entity}", entity.Id);

            // Check if entity exists
            var exists = await ExistsAsync(entity.Id, cancellationToken);
            if (!exists)
                throw new ArgumentException($"{Entity} with ID {entity.Id} not found.");

            _context.{Entities}.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("{Entity} with ID {Id} updated successfully", "{Entity}", entity.Id);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency error updating {entity} with ID: {Id}", "{entity}", entity.Id);
            throw new InvalidOperationException("The {entity} was modified by another user. Please refresh and try again.", ex);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while updating {entity} with ID: {Id}", "{entity}", entity.Id);
            throw new InvalidOperationException("Failed to update {entity} in database.", ex);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating {entity} with ID: {Id}", "{entity}", entity.Id);
            throw new InvalidOperationException("Failed to update {entity}.", ex);
        }
    }

    /// <summary>
    /// Deletes a {entity} from the database asynchronously
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

            _logger.LogDebug("Deleting {entity} with ID: {Id}", "{entity}", id);

            var entity = await _context.{Entities}
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            if (entity == null)
                throw new ArgumentException($"{Entity} with ID {id} not found.");

            _context.{Entities}.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("{Entity} with ID {Id} deleted successfully", "{Entity}", id);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while deleting {entity} with ID: {Id}", "{entity}", id);
            throw new InvalidOperationException("Failed to delete {entity} from database. It may be referenced by other records.", ex);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting {entity} with ID: {Id}", "{entity}", id);
            throw new InvalidOperationException("Failed to delete {entity}.", ex);
        }
    }

    /// <summary>
    /// Checks if a {entity} exists by ID asynchronously
    /// </summary>
    /// <param name="id">The {entity} ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if exists, false otherwise</returns>
    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id <= 0)
                return false;

            return await _context.{Entities}
                .AnyAsync(e => e.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if {entity} exists with ID: {Id}", "{entity}", id);
            throw new InvalidOperationException("Failed to check {entity} existence.", ex);
        }
    }

    #region Custom Query Methods

    /// <summary>
    /// Searches {entities} by name asynchronously
    /// </summary>
    /// <param name="name">The name to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching {entities}</returns>
    public async Task<List<{Entity}>> SearchByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Search name cannot be empty.", nameof(name));

            _logger.LogDebug("Searching {entities} by name: {Name}", "{entities}", name);

            var entities = await _context.{Entities}
                .AsNoTracking()
                .Where(e => e.Name.Contains(name))
                .ToListAsync(cancellationToken);

            _logger.LogDebug("Found {Count} {entities} matching '{Name}'", entities.Count, "{entities}", name);
            return entities;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching {entities} by name: {Name}", "{entities}", name);
            throw new InvalidOperationException("Failed to search {entities}.", ex);
        }
    }

    /// <summary>
    /// Gets all active {entities} asynchronously
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active {entities}</returns>
    public async Task<List<{Entity}>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Retrieving active {entities}");

            var entities = await _context.{Entities}
                .AsNoTracking()
                .Where(e => e.IsActive) // Assumes entity has IsActive property
                .ToListAsync(cancellationToken);

            _logger.LogDebug("Retrieved {Count} active {entities}", entities.Count, "{entities}");
            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active {entities}");
            throw new InvalidOperationException("Failed to retrieve active {entities}.", ex);
        }
    }

    #endregion
}
```

5. **Register in DI Container**

   Show the user how to register in `Program.cs`:
   ```csharp
   // In ConfigureServices method:

   // Register DbContext first
   services.AddDbContext<AppDbContext>(options =>
       options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

   // Register repository
   services.AddScoped<I{Entity}Repository, {Entity}Repository>();
   ```

6. **Create Entity Model (if needed)**

   If entity doesn't exist, offer to create it:
   ```csharp
   namespace YourApp.Models;

   /// <summary>
   /// Represents a {entity}
   /// </summary>
   public class {Entity}
   {
       /// <summary>
       /// Gets or sets the {entity} ID
       /// </summary>
       public int Id { get; set; }

       /// <summary>
       /// Gets or sets the {entity} name
       /// </summary>
       public string Name { get; set; } = string.Empty;

       /// <summary>
       /// Gets or sets whether the {entity} is active
       /// </summary>
       public bool IsActive { get; set; } = true;

       /// <summary>
       /// Gets or sets the created date
       /// </summary>
       public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

       /// <summary>
       /// Gets or sets the last modified date
       /// </summary>
       public DateTime? ModifiedDate { get; set; }
   }
   ```

7. **Add to DbContext**

   Show how to add DbSet to AppDbContext:
   ```csharp
   public class AppDbContext : DbContext
   {
       // ... existing code ...

       public DbSet<{Entity}> {Entities} { get; set; } = null!;

       // In OnModelCreating:
       protected override void OnModelCreating(ModelBuilder modelBuilder)
       {
           base.OnModelCreating(modelBuilder);

           modelBuilder.Entity<{Entity}>(entity =>
           {
               entity.HasKey(e => e.Id);
               entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
               entity.HasIndex(e => e.Name); // Add index for search performance

               // Add more configuration as needed
           });
       }
   }
   ```

8. **Suggest Next Steps**

   Ask: "Would you like me to:
   - Create a migration for this entity? (`dotnet ef migrations add Add{Entity}Table`)
   - Generate a service that uses this repository? (`/create-service`)
   - Generate unit tests? (`/add-test`)"

## Best Practices Checklist

Before finishing, verify:
- ✅ Interface and implementation created
- ✅ All methods are async with CancellationToken support
- ✅ Uses AsNoTracking() for read operations
- ✅ Proper dependency injection (DbContext, ILogger)
- ✅ XML documentation on all public members
- ✅ Try-catch with logging
- ✅ Handles DbUpdateException and DbUpdateConcurrencyException
- ✅ Existence checks before update/delete
- ✅ Custom query methods use proper filtering
- ✅ DI registration instructions provided

## Common Repository Patterns

### Generic + Specific (Recommended)
```
IRepository<T> → I{Entity}Repository : IRepository<{Entity}> → {Entity}Repository
```

### Specific Only (For complex entities)
```
I{Entity}Repository → {Entity}Repository
```

### Include Related Entities
```csharp
.Include(e => e.Orders)
.ThenInclude(o => o.OrderItems)
```

### Pagination Support
```csharp
Task<PagedResult<{Entity}>> GetPagedAsync(int page, int pageSize, CancellationToken ct);
```

## Example Output Structure

```
/Models
    └── {Entity}.cs                  (Entity model)
/Repositories
    ├── IRepository.cs               (Generic interface - optional)
    ├── I{Entity}Repository.cs       (Specific interface)
    └── {Entity}Repository.cs        (Implementation)
/Data
    └── AppDbContext.cs              (Updated with DbSet)
```

## Performance Tips

- Use `AsNoTracking()` for read-only queries
- Add indexes for frequently queried columns
- Use `Include()` carefully to avoid N+1 queries
- Consider using compiled queries for frequently used queries
- Use pagination for large result sets
- Use `AnyAsync()` instead of `CountAsync() > 0`

## Notes

- **Always use async EF methods** (ToListAsync, FirstOrDefaultAsync, etc.)
- **Log at Debug level** for repository operations (use Info in services)
- **Handle concurrency** with DbUpdateConcurrencyException
- **Check constraints** when deleting (may have foreign key references)
- **Use transactions** for complex multi-table operations
- **Connection is managed by DbContext** - don't manually open/close
