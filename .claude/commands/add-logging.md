---
description: Setup comprehensive logging with Serilog or NLog
---

You are tasked with setting up comprehensive logging for a WinForms application.

## Workflow

1. **Ask for Logging Preferences**
   - Which logging framework? (Serilog recommended, or NLog)
   - Where to log? (File, Console, Database, External service)
   - Log level for production? (Information, Warning, Error)
   - Log file location? (Default: logs/ folder)
   - Do you need structured logging? (JSON format)

2. **Read Documentation**
   - Reference `docs/best-practices/error-handling.md` for logging patterns

3. **Install NuGet Packages**

### For Serilog (Recommended):
```bash
dotnet add package Serilog
dotnet add package Serilog.Extensions.Logging
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.Debug
```

### For NLog (Alternative):
```bash
dotnet add package NLog
dotnet add package NLog.Extensions.Logging
```

4. **Configure Logging in Program.cs**

### Option 1: Serilog Configuration (Recommended)

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Windows.Forms;

namespace YourApp;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        // Configure Serilog FIRST (before anything else)
        ConfigureSerilog();

        try
        {
            Log.Information("=== Application Starting ===");
            Log.Information("Application: {AppName}, Version: {Version}",
                Application.ProductName,
                Application.ProductVersion);

            ApplicationConfiguration.Initialize();

            // Build configuration
            var configuration = BuildConfiguration();

            // Setup DI container with logging
            var services = new ServiceCollection();
            ConfigureServices(services, configuration);
            var serviceProvider = services.BuildServiceProvider();

            // Log DI setup complete
            Log.Information("Dependency Injection configured successfully");

            // Run application
            var mainForm = serviceProvider.GetRequiredService<MainForm>();
            Log.Information("Main form created, starting message loop");

            Application.Run(mainForm);

            Log.Information("=== Application Shutdown (Normal) ===");
        }
        catch (Exception ex)
        {
            // Log fatal errors
            Log.Fatal(ex, "Application terminated unexpectedly");

            MessageBox.Show(
                $"A fatal error occurred:\n\n{ex.Message}\n\nPlease check the log file for details.",
                "Fatal Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            // Ensure all logs are flushed
            Log.CloseAndFlush();
        }
    }

    private static void ConfigureSerilog()
    {
        // Define log file path
        var logsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            Application.CompanyName ?? "YourCompany",
            Application.ProductName ?? "YourApp",
            "logs");

        // Ensure logs directory exists
        Directory.CreateDirectory(logsPath);

        var logFilePath = Path.Combine(logsPath, "app-.log");

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            // Minimum level
            .MinimumLevel.Debug()

            // Override specific namespaces
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)

            // Enrich logs with additional info
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()

            // Write to file (rolling daily)
            .WriteTo.File(
                logFilePath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30, // Keep 30 days
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                shared: true) // Allow multiple processes

            // Write to debug output (for Visual Studio)
            .WriteTo.Debug(
                outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")

#if DEBUG
            // Write to console in debug mode
            .WriteTo.Console(
                outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
#endif

            .CreateLogger();

        Log.Information("Serilog configured. Log file: {LogFilePath}", logFilePath);
    }

    private static IConfiguration BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Production"}.json",
                optional: true);

        return builder.Build();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Add configuration
        services.AddSingleton(configuration);

        // Add logging (Serilog)
        services.AddLogging(builder =>
        {
            builder.ClearProviders(); // Remove default providers
            builder.AddSerilog(dispose: true); // Add Serilog
        });

        // Register your services, repositories, forms here
        // ...

        Log.Debug("Services configuration complete");
    }
}
```

### Option 2: NLog Configuration

```csharp
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.Windows.Forms;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        // Configure NLog
        var logger = ConfigureNLog();

        try
        {
            logger.Info("=== Application Starting ===");

            ApplicationConfiguration.Initialize();

            // ... rest of your code

            Application.Run(new MainForm());

            logger.Info("=== Application Shutdown (Normal) ===");
        }
        catch (Exception ex)
        {
            logger.Fatal(ex, "Application terminated unexpectedly");
            MessageBox.Show($"Fatal error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            LogManager.Shutdown();
        }
    }

    private static Logger ConfigureNLog()
    {
        var config = new NLog.Config.LoggingConfiguration();

        // File target
        var logfile = new NLog.Targets.FileTarget("logfile")
        {
            FileName = "${specialfolder:folder=LocalApplicationData}/${appname}/logs/app-${shortdate}.log",
            Layout = "${longdate} [${level:uppercase=true}] [${logger}] ${message} ${exception:format=tostring}",
            ArchiveEvery = NLog.Targets.FileArchivePeriod.Day,
            MaxArchiveFiles = 30
        };

        // Console target (debug)
        var logconsole = new NLog.Targets.ConsoleTarget("logconsole")
        {
            Layout = "${time} [${level:uppercase=true}] ${message} ${exception:format=message}"
        };

        // Rules
        config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, logconsole);
        config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logfile);

        LogManager.Configuration = config;
        return LogManager.GetCurrentClassLogger();
    }
}
```

5. **Create appsettings.json for Serilog**

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "logs/app-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30,
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithThreadId" ]
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app.db"
  }
}
```

6. **Add Logging to Services**

Show how to use logging in services and forms:

```csharp
using Microsoft.Extensions.Logging;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(
        ICustomerRepository repository,
        ILogger<CustomerService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<Customer>> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving all customers");

            var customers = await _repository.GetAllAsync();

            _logger.LogInformation("Retrieved {Count} customers", customers.Count);
            return customers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers");
            throw;
        }
    }

    public async Task<Customer> CreateAsync(Customer customer)
    {
        try
        {
            _logger.LogInformation("Creating customer: {Email}", customer.Email);

            // Validation
            if (string.IsNullOrWhiteSpace(customer.Name))
            {
                _logger.LogWarning("Validation failed: Name is required");
                throw new ArgumentException("Name is required");
            }

            var created = await _repository.AddAsync(customer);

            _logger.LogInformation("Customer created with ID: {Id}", created.Id);
            return created;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer: {Email}", customer.Email);
            throw;
        }
    }
}
```

7. **Add Logging to Forms**

```csharp
using Microsoft.Extensions.Logging;

public partial class MainForm : Form
{
    private readonly ILogger<MainForm> _logger;
    private readonly ICustomerService _customerService;

    public MainForm(
        ICustomerService customerService,
        ILogger<MainForm> logger)
    {
        InitializeComponent();

        _customerService = customerService;
        _logger = logger;

        _logger.LogDebug("MainForm constructor called");
    }

    private async void MainForm_Load(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("MainForm loading");

            await LoadDataAsync();

            _logger.LogInformation("MainForm loaded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading MainForm");
            MessageBox.Show($"Error loading form: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task LoadDataAsync()
    {
        _logger.LogDebug("Loading customer data");

        var customers = await _customerService.GetAllAsync();
        dgvCustomers.DataSource = customers;

        _logger.LogDebug("Loaded {Count} customers into grid", customers.Count);
    }
}
```

8. **Logging Best Practices Guide**

Create a quick reference:

```csharp
// ✅ GOOD: Structured logging with parameters
_logger.LogInformation("User {UserId} logged in from {IpAddress}", userId, ipAddress);

// ❌ BAD: String interpolation
_logger.LogInformation($"User {userId} logged in from {ipAddress}");

// ✅ GOOD: Log levels appropriately
_logger.LogDebug("Detailed diagnostic info");      // Development only
_logger.LogInformation("Normal operations");        // Production
_logger.LogWarning("Abnormal but handled");         // Issues that recovered
_logger.LogError(ex, "Operation failed");           // Errors with exception
_logger.LogCritical(ex, "System failure");          // Fatal errors

// ✅ GOOD: Log scope for context
using (_logger.BeginScope("OrderId: {OrderId}", orderId))
{
    _logger.LogInformation("Processing order");
    // All logs in this scope will include OrderId
}

// ✅ GOOD: Exception logging
try
{
    await DoSomethingAsync();
}
catch (Exception ex)
{
    _logger.LogError(ex, "Failed to do something for customer {CustomerId}", customerId);
    throw; // Re-throw after logging
}
```

9. **Create Logging Helper Class (Optional)**

```csharp
using Serilog;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace YourApp.Utils;

/// <summary>
/// Helper class for performance logging
/// </summary>
public class PerformanceLogger : IDisposable
{
    private readonly string _operationName;
    private readonly Stopwatch _stopwatch;
    private readonly ILogger _logger;

    public PerformanceLogger(
        ILogger logger,
        [CallerMemberName] string operationName = "")
    {
        _logger = logger;
        _operationName = operationName;
        _stopwatch = Stopwatch.StartNew();

        _logger.Debug("Started: {Operation}", _operationName);
    }

    public void Dispose()
    {
        _stopwatch.Stop();
        _logger.Information("Completed: {Operation} in {ElapsedMs}ms",
            _operationName, _stopwatch.ElapsedMilliseconds);
    }
}

// Usage:
using (new PerformanceLogger(_logger))
{
    await LoadLargeDatasetAsync();
} // Automatically logs execution time
```

10. **Verify Installation**

Show the user a checklist:

```
✅ NuGet packages installed
✅ Serilog/NLog configured in Program.cs
✅ appsettings.json created (for Serilog)
✅ Logging added to DI container
✅ ILogger<T> injected in services/forms
✅ Log statements added to key operations
✅ Global exception handler logs errors
✅ Log file location verified
✅ Application runs and logs are created
```

## Best Practices Checklist

Before finishing, verify:
- ✅ Serilog/NLog installed via NuGet
- ✅ Configured in Program.cs before any other code
- ✅ Log file location in LocalApplicationData
- ✅ Rolling daily files with retention policy
- ✅ Different log levels for different environments
- ✅ Structured logging (not string interpolation)
- ✅ ILogger<T> injected via DI
- ✅ Exceptions logged with full details
- ✅ Log.CloseAndFlush() in finally block
- ✅ Sensitive data not logged (passwords, tokens)

## Log Levels Guide

| Level | When to Use | Example |
|-------|-------------|---------|
| **Trace** | Very detailed diagnostic info | Method entry/exit, variable values |
| **Debug** | Diagnostic info for development | SQL queries, detailed flow |
| **Information** | Normal operation flow | User logged in, order created |
| **Warning** | Abnormal but expected | Validation failed, retry attempted |
| **Error** | Operation failed | Exception caught, operation aborted |
| **Critical/Fatal** | System failure | Database down, unrecoverable error |

## Common Sinks (Output Targets)

### File (Most Common)
```csharp
.WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day)
```

### Console (Development)
```csharp
.WriteTo.Console()
```

### Debug Window (Visual Studio)
```csharp
.WriteTo.Debug()
```

### Database (Production)
```csharp
.WriteTo.MSSqlServer(connectionString, tableName)
```

### Seq (Structured Log Server)
```csharp
.WriteTo.Seq("http://localhost:5341")
```

### Email (Critical Errors)
```csharp
.WriteTo.Email(emailSettings)
```

## Performance Considerations

### ✅ Good Practices
- Use structured logging (parameters, not interpolation)
- Log at appropriate levels (don't log everything at Info)
- Use async sinks for high-volume logging
- Set retention policies to avoid disk fill

### ❌ Avoid
- Logging in tight loops
- Logging large objects (serialize to string first)
- Logging sensitive data
- String concatenation in log messages

## Troubleshooting

### Logs not appearing?
1. Check log file path exists
2. Verify write permissions
3. Check MinimumLevel configuration
4. Ensure Log.CloseAndFlush() is called

### Performance issues?
1. Reduce log level in production
2. Use async sinks
3. Consider sampling for high-frequency logs
4. Check disk I/O

### Log file too large?
1. Reduce retention period
2. Use rolling files
3. Archive old logs
4. Consider external log aggregation

## Notes

- **Serilog is recommended** for WinForms due to simplicity and features
- **Log to LocalApplicationData** to avoid permission issues
- **Always log exceptions** with the exception object, not just message
- **Use structured logging** for better searchability
- **Don't log sensitive data** (passwords, credit cards, tokens)
- **Flush on shutdown** to ensure all logs are written
- **Consider log aggregation** (Seq, ELK) for production apps
