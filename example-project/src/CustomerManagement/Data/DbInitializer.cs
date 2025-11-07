using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CustomerManagement.Data;

/// <summary>
/// Helper class for database initialization.
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Initializes the database, creating it if it doesn't exist and applying migrations.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">Optional logger for diagnostics.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task InitializeAsync(AppDbContext context, ILogger? logger = null)
    {
        try
        {
            // Ensure database is created
            var created = await context.Database.EnsureCreatedAsync();

            if (created)
            {
                logger?.LogInformation("Database created successfully.");
            }
            else
            {
                logger?.LogInformation("Database already exists.");
            }

            // Alternative: Use migrations in production
            // await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    /// <summary>
    /// Drops and recreates the database. Use with caution!
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">Optional logger for diagnostics.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task RecreateAsync(AppDbContext context, ILogger? logger = null)
    {
        try
        {
            logger?.LogWarning("Dropping database...");
            await context.Database.EnsureDeletedAsync();

            logger?.LogInformation("Creating database...");
            await context.Database.EnsureCreatedAsync();

            logger?.LogInformation("Database recreated successfully.");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "An error occurred while recreating the database.");
            throw;
        }
    }
}
