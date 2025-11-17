using CustomerManagement.Data;
using CustomerManagement.Factories;
using CustomerManagement.Forms;
using CustomerManagement.Repositories;
using CustomerManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CustomerManagement;

/// <summary>
/// Main program entry point with Dependency Injection setup.
/// </summary>
internal static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Configure Serilog for logging
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(
                path: "logs/app-.log",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        try
        {
            Log.Information("Application starting...");

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Build configuration
            var configuration = BuildConfiguration();

            // Build dependency injection container
            var services = new ServiceCollection();
            ConfigureServices(services, configuration);
            var serviceProvider = services.BuildServiceProvider();

            // Initialize database
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
                DbInitializer.InitializeAsync(dbContext, logger).Wait();
            }

            // Run the application with the main form
            var mainForm = serviceProvider.GetRequiredService<MainForm>();
            Application.Run(mainForm);

            Log.Information("Application exiting normally");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
            MessageBox.Show(
                $"A fatal error occurred: {ex.Message}\n\nPlease check the logs for details.",
                "Fatal Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    /// <summary>
    /// Builds the application configuration from appsettings.json.
    /// </summary>
    /// <returns>The configuration root.</returns>
    private static IConfiguration BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        return builder.Build();
    }

    /// <summary>
    /// Configures services for dependency injection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Configuration
        services.AddSingleton(configuration);

        // Logging
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog();
        });

        // Database Context
        services.AddDbContext<AppDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlite(connectionString);

            // Enable sensitive data logging in development (disable in production!)
            #if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            #endif
        });

        // Unit of Work (Scoped - one instance per request/scope)
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<ICustomerService, CustomerService>();

        // Factory Pattern (Singleton - one instance for entire app lifetime)
        services.AddSingleton<IFormFactory, FormFactory>();

        // Forms (Transient - new instance each time)
        services.AddTransient<MainForm>();
        services.AddTransient<CustomerListForm>();
        services.AddTransient<CustomerEditForm>();

        Log.Information("Services configured successfully");
    }

    /// <summary>
    /// Configures global exception handlers.
    /// </summary>
    private static void ConfigureExceptionHandlers()
    {
        // Handle unhandled exceptions in UI thread
        Application.ThreadException += (sender, e) =>
        {
            Log.Error(e.Exception, "Unhandled UI thread exception");
            MessageBox.Show(
                $"An unexpected error occurred: {e.Exception.Message}\n\nPlease check the logs for details.",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        };

        // Handle unhandled exceptions in non-UI threads
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            var exception = e.ExceptionObject as Exception;
            Log.Fatal(exception, "Unhandled non-UI thread exception");

            if (e.IsTerminating)
            {
                MessageBox.Show(
                    $"A fatal error occurred: {exception?.Message}\n\nThe application will now close.",
                    "Fatal Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        };
    }
}
