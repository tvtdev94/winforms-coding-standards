# Connection Management in WinForms

> **Quick Reference**: Properly manage database connections for optimal performance, security, and resource efficiency in WinForms applications.

---

## 📋 Overview

**Connection Management** involves:
- Storing and securing connection strings
- Managing connection lifecycle (open/close)
- Leveraging connection pooling
- Handling connection failures gracefully
- Monitoring connection usage

**Key Principle**: Always use `using` statements to ensure connections are properly closed and returned to the pool.

---

## 🎯 Why This Matters

### Performance Impact
✅ **Connection Pooling** - Reusing connections is 10-100x faster than creating new ones
✅ **Resource Efficiency** - Database connections are expensive resources
✅ **Scalability** - Proper pooling enables more concurrent users

### Common Problems from Poor Management
❌ **Connection Exhaustion** - Pool runs out, app hangs
❌ **Memory Leaks** - Undisposed connections consume memory
❌ **Performance Degradation** - Creating connections is slow
❌ **Database Server Overload** - Too many connections

---

## 🔐 Connection String Management

### Storing Connection Strings

#### appsettings.json (Recommended for .NET 6+)

```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyApp;Integrated Security=true;TrustServerCertificate=true",
    "ReadOnlyConnection": "Server=localhost;Database=MyApp;Integrated Security=true;ApplicationIntent=ReadOnly"
  }
}
```

```csharp
// Reading from appsettings.json
public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not found");
    }
}
```

#### App.config (.NET Framework)

```xml
<!-- App.config -->
<configuration>
  <connectionStrings>
    <add name="DefaultConnection"
         connectionString="Server=localhost;Database=MyApp;Integrated Security=true"
         providerName="System.Data.SqlClient" />
  </connectionStrings>
</configuration>
```

```csharp
// Reading from App.config
using System.Configuration;

string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
```

#### Environment Variables (Production)

```csharp
// Reading from environment variables
string connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")
    ?? throw new InvalidOperationException("Connection string not configured");
```

#### User Secrets (Development)

```bash
# Set user secret
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=Dev;..."
```

```csharp
// Automatically loaded in development
var connectionString = configuration.GetConnectionString("DefaultConnection");
```

#### ❌ Wrong vs ✅ Correct

```csharp
// ❌ WRONG - Hardcoded connection string
public class CustomerRepository
{
    private const string ConnectionString = "Server=prod-sql;Database=Customers;User Id=sa;Password=Pass123!";

    public List<Customer> GetAll()
    {
        using var conn = new SqlConnection(ConnectionString); // Hardcoded!
        // ...
    }
}

// ✅ CORRECT - Injected from configuration
public class CustomerRepository
{
    private readonly string _connectionString;

    public CustomerRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentException("Connection string not found");
    }

    public async Task<List<Customer>> GetAllAsync()
    {
        using var conn = new SqlConnection(_connectionString);
        // ...
    }
}
```

---

### Connection String Security

#### Never Hardcode Credentials

```csharp
// ❌ DANGER - Credentials in source code
private const string ConnStr = "Server=sql;Database=DB;User Id=admin;Password=secret123";

// ✅ SECURE - From configuration
private readonly string _connectionString = _config.GetConnectionString("DefaultConnection");
```

#### Encryption Options

**Windows DPAPI (Local Applications)**

```csharp
using System.Security.Cryptography;

public static class ConnectionStringEncryption
{
    public static string Encrypt(string connectionString)
    {
        byte[] data = Encoding.UTF8.GetBytes(connectionString);
        byte[] encrypted = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(encrypted);
    }

    public static string Decrypt(string encryptedString)
    {
        byte[] encrypted = Convert.FromBase64String(encryptedString);
        byte[] decrypted = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
        return Encoding.UTF8.GetString(decrypted);
    }
}

// Usage
string encryptedConnStr = ConnectionStringEncryption.Encrypt(originalConnectionString);
// Store encryptedConnStr in config file

// When needed:
string connectionString = ConnectionStringEncryption.Decrypt(encryptedConnStr);
```

**Azure Key Vault (Cloud Applications)**

```csharp
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

public class SecureConnectionProvider
{
    private readonly SecretClient _keyVaultClient;

    public SecureConnectionProvider(string keyVaultUrl)
    {
        _keyVaultClient = new SecretClient(
            new Uri(keyVaultUrl),
            new DefaultAzureCredential());
    }

    public async Task<string> GetConnectionStringAsync()
    {
        KeyVaultSecret secret = await _keyVaultClient.GetSecretAsync("DatabaseConnectionString");
        return secret.Value;
    }
}
```

---

### Connection String Builder

```csharp
using System.Data.SqlClient;

// ✅ Type-safe connection string construction
public static class ConnectionStringFactory
{
    public static string BuildConnectionString(
        string server,
        string database,
        bool integratedSecurity = true,
        int timeout = 30,
        int minPoolSize = 5,
        int maxPoolSize = 100)
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = server,
            InitialCatalog = database,
            IntegratedSecurity = integratedSecurity,
            ConnectTimeout = timeout,
            MinPoolSize = minPoolSize,
            MaxPoolSize = maxPoolSize,
            Pooling = true,
            MultipleActiveResultSets = false,
            TrustServerCertificate = true // For development only
        };

        if (!integratedSecurity)
        {
            builder.UserID = "app_user";
            builder.Password = GetPasswordFromSecureStorage();
        }

        return builder.ConnectionString;
    }

    private static string GetPasswordFromSecureStorage()
    {
        // Retrieve from secure storage (Key Vault, encrypted config, etc.)
        throw new NotImplementedException();
    }
}

// Usage
string connectionString = ConnectionStringFactory.BuildConnectionString(
    server: "localhost",
    database: "MyApp",
    integratedSecurity: true,
    maxPoolSize: 50
);
```

---

## 🏊 Connection Pooling

### How Connection Pooling Works

```
┌─────────────────────────────────────────┐
│         Connection Pool                 │
│                                         │
│  ┌────┐ ┌────┐ ┌────┐ ┌────┐ ┌────┐   │
│  │ C1 │ │ C2 │ │ C3 │ │ C4 │ │ C5 │   │ Available Connections
│  └────┘ └────┘ └────┘ └────┘ └────┘   │
│                                         │
│  ┌────┐ ┌────┐                         │
│  │ C6 │ │ C7 │                         │ In-Use Connections
│  └────┘ └────┘                         │
└─────────────────────────────────────────┘

Process:
1. Application requests connection
2. Pool returns existing connection if available
3. If none available, creates new (up to MaxPoolSize)
4. When app closes connection, returned to pool (not destroyed)
5. Pool maintains MinPoolSize connections always ready
```

### Pooling Configuration

```csharp
var builder = new SqlConnectionStringBuilder
{
    // Pool Settings
    Pooling = true,                    // Enable pooling (default: true)
    MinPoolSize = 5,                   // Minimum connections in pool (default: 0)
    MaxPoolSize = 100,                 // Maximum connections in pool (default: 100)
    ConnectionLifetime = 300,          // Max seconds connection can live (default: 0 = infinite)
    ConnectionTimeout = 15,            // Seconds to wait for connection (default: 15)

    // Database Settings
    DataSource = "localhost",
    InitialCatalog = "MyApp",
    IntegratedSecurity = true
};
```

**Parameter Explanations**:

| Parameter | Description | Recommended |
|-----------|-------------|-------------|
| `MinPoolSize` | Pre-created connections | 5-10 for desktop apps |
| `MaxPoolSize` | Maximum pool capacity | 50-100 for desktop apps |
| `ConnectionLifetime` | Max age before refresh (seconds) | 300 (5 min) or 0 (infinite) |
| `ConnectionTimeout` | Wait time for connection | 15-30 seconds |

---

### Pooling Best Practices

```csharp
// ✅ BEST PRACTICE - Quick open and close with using
public async Task<List<Customer>> GetAllCustomersAsync()
{
    var customers = new List<Customer>();

    // Connection opened here
    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        using var command = new SqlCommand("SELECT * FROM Customers", connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            customers.Add(MapCustomer(reader));
        }

    } // Connection closed and returned to pool immediately

    return customers;
}

// ❌ WRONG - Holding connection too long
public async Task<List<Customer>> GetAllCustomersWrong()
{
    using var connection = new SqlConnection(_connectionString);
    await connection.OpenAsync();

    using var command = new SqlCommand("SELECT * FROM Customers", connection);
    using var reader = await command.ExecuteReaderAsync();

    var customers = new List<Customer>();
    while (await reader.ReadAsync())
    {
        customers.Add(MapCustomer(reader));

        // ❌ WRONG - Processing while holding connection
        await Task.Delay(100); // Simulating slow processing
        await ProcessCustomerBusinessLogicAsync(customers.Last());
    }

    return customers; // Connection held during entire processing!
}

// ✅ CORRECT - Process after closing connection
public async Task<List<Customer>> GetAllCustomersCorrect()
{
    List<Customer> customers;

    // Get data quickly and close connection
    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();
        using var command = new SqlCommand("SELECT * FROM Customers", connection);
        using var reader = await command.ExecuteReaderAsync();

        customers = new List<Customer>();
        while (await reader.ReadAsync())
        {
            customers.Add(MapCustomer(reader));
        }
    } // Connection returned to pool

    // Process data without holding connection
    foreach (var customer in customers)
    {
        await ProcessCustomerBusinessLogicAsync(customer);
    }

    return customers;
}
```

---

### Disabling Pooling

**Rarely needed** - only disable pooling for specific scenarios:

```csharp
// When to disable pooling (rare cases):
// 1. Using SQL Server Express LocalDB
// 2. Very infrequent database access
// 3. Specific security requirements

var builder = new SqlConnectionStringBuilder
{
    DataSource = "localhost",
    InitialCatalog = "MyApp",
    IntegratedSecurity = true,
    Pooling = false  // Disable pooling
};

// Or in connection string:
"Server=localhost;Database=MyApp;Integrated Security=true;Pooling=false"
```

---

## ⏱️ Connection Lifetime

### Opening Connections - Late as Possible

```csharp
// ❌ WRONG - Opening connection too early
public async Task<bool> SaveCustomerWrong(Customer customer)
{
    using var connection = new SqlConnection(_connectionString);
    await connection.OpenAsync(); // Opened early!

    // Validation logic while holding connection
    if (string.IsNullOrEmpty(customer.Name))
        return false;

    if (!IsEmailValid(customer.Email))
        return false;

    // Heavy processing
    await CalculateCustomerScoreAsync(customer);

    // Finally use connection
    using var cmd = new SqlCommand("INSERT INTO...", connection);
    await cmd.ExecuteNonQueryAsync();

    return true;
}

// ✅ CORRECT - Open late, close early
public async Task<bool> SaveCustomerCorrect(Customer customer)
{
    // Validation without connection
    if (string.IsNullOrEmpty(customer.Name))
        return false;

    if (!IsEmailValid(customer.Email))
        return false;

    // Heavy processing
    await CalculateCustomerScoreAsync(customer);

    // Open connection only when needed
    using var connection = new SqlConnection(_connectionString);
    await connection.OpenAsync();

    using var cmd = new SqlCommand("INSERT INTO Customers...", connection);
    cmd.Parameters.AddWithValue("@Name", customer.Name);
    await cmd.ExecuteNonQueryAsync();

    return true; // Connection auto-closed here
}
```

---

### Connection States

```csharp
using System.Data;

public void DemonstrateConnectionStates()
{
    using var connection = new SqlConnection(_connectionString);

    Console.WriteLine(connection.State); // Closed

    connection.Open();
    Console.WriteLine(connection.State); // Open

    // Checking state before operations
    if (connection.State == ConnectionState.Open)
    {
        // Execute commands
    }

    connection.Close();
    Console.WriteLine(connection.State); // Closed
}

// ✅ Handling broken connections
public async Task<bool> ExecuteWithStateCheckAsync(string sql)
{
    using var connection = new SqlConnection(_connectionString);

    try
    {
        await connection.OpenAsync();

        // Check connection is actually open
        if (connection.State != ConnectionState.Open)
        {
            _logger.LogError("Connection failed to open");
            return false;
        }

        using var cmd = new SqlCommand(sql, connection);
        await cmd.ExecuteNonQueryAsync();

        return true;
    }
    catch (SqlException ex) when (connection.State == ConnectionState.Broken)
    {
        _logger.LogError(ex, "Connection broken during operation");
        return false;
    }
}
```

**Connection States**:
- `Closed` - Not connected
- `Open` - Active and ready
- `Connecting` - Opening in progress
- `Executing` - Executing command
- `Fetching` - Retrieving data
- `Broken` - Connection lost, must close and reopen

---

### Connection Timeouts

```csharp
// Connection Timeout vs Command Timeout
var builder = new SqlConnectionStringBuilder
{
    DataSource = "localhost",
    InitialCatalog = "MyApp",

    // Connection Timeout: How long to wait for connection to open
    ConnectTimeout = 30  // 30 seconds (default: 15)
};

using var connection = new SqlConnection(builder.ConnectionString);
await connection.OpenAsync(); // Will wait up to 30 seconds

// Command Timeout: How long command can run
using var command = new SqlCommand("EXEC LongRunningProc", connection);
command.CommandTimeout = 120; // 120 seconds (default: 30)

await command.ExecuteNonQueryAsync(); // Will wait up to 120 seconds

// ✅ Handling timeout exceptions
public async Task<List<Customer>> GetCustomersWithTimeoutHandling()
{
    try
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(); // ConnectTimeout applies

        using var command = new SqlCommand("SELECT * FROM Customers", connection);
        command.CommandTimeout = 30;

        using var reader = await command.ExecuteReaderAsync();
        return MapCustomers(reader);
    }
    catch (SqlException ex) when (ex.Number == -2) // Timeout error
    {
        _logger.LogError(ex, "Query timeout exceeded");
        throw new TimeoutException("Database query took too long", ex);
    }
}
```

---

## 🔌 ADO.NET Connection Management

### SqlConnection Best Practices

```csharp
public class CustomerRepository
{
    private readonly string _connectionString;
    private readonly ILogger<CustomerRepository> _logger;

    public CustomerRepository(IConfiguration config, ILogger<CustomerRepository> logger)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentException("Connection string not found");
        _logger = logger;
    }

    // ✅ PATTERN: using statement for automatic disposal
    public async Task<Customer?> GetByIdAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(
            "SELECT Id, Name, Email FROM Customers WHERE Id = @Id",
            connection);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Customer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2)
            };
        }

        return null;
    }

    // ✅ PATTERN: Transaction with proper error handling
    public async Task<bool> TransferOrderAsync(int orderId, int newCustomerId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        try
        {
            // Update order
            using (var cmd = new SqlCommand(
                "UPDATE Orders SET CustomerId = @NewId WHERE Id = @OrderId",
                connection, transaction))
            {
                cmd.Parameters.AddWithValue("@NewId", newCustomerId);
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                await cmd.ExecuteNonQueryAsync();
            }

            // Log history
            using (var cmd = new SqlCommand(
                "INSERT INTO OrderHistory (OrderId, Action, Date) VALUES (@Id, 'Transferred', GETDATE())",
                connection, transaction))
            {
                cmd.Parameters.AddWithValue("@Id", orderId);
                await cmd.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to transfer order {OrderId}", orderId);
            await transaction.RollbackAsync();
            return false;
        }
    }
}
```

---

### Multiple Connections (Parallel Operations)

```csharp
// ✅ Parallel database operations
public async Task<(List<Customer> customers, List<Order> orders)> GetDataParallelAsync()
{
    var customersTask = GetCustomersAsync();
    var ordersTask = GetOrdersAsync();

    await Task.WhenAll(customersTask, ordersTask);

    return (await customersTask, await ordersTask);
}

private async Task<List<Customer>> GetCustomersAsync()
{
    using var connection = new SqlConnection(_connectionString); // Own connection
    await connection.OpenAsync();

    using var command = new SqlCommand("SELECT * FROM Customers", connection);
    using var reader = await command.ExecuteReaderAsync();

    return MapCustomers(reader);
}

private async Task<List<Order>> GetOrdersAsync()
{
    using var connection = new SqlConnection(_connectionString); // Own connection
    await connection.OpenAsync();

    using var command = new SqlCommand("SELECT * FROM Orders", connection);
    using var reader = await command.ExecuteReaderAsync();

    return MapOrders(reader);
}
```

---

## 🎯 EF Core Connection Management

### DbContext Lifetime

```csharp
// ✅ RECOMMENDED - Scoped lifetime in DI
services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
}, ServiceLifetime.Scoped); // One instance per operation

// ✅ Per-operation context (desktop apps)
public async Task<List<Customer>> GetCustomersAsync()
{
    using var context = new AppDbContext(_options);
    return await context.Customers.ToListAsync();
}

// ❌ ANTI-PATTERN - Singleton DbContext
services.AddSingleton<AppDbContext>(); // DON'T DO THIS!

// DbContext is NOT thread-safe and should not be shared
```

---

### DbContext Pooling

```csharp
// ✅ DbContext pooling for better performance
services.AddDbContextPool<AppDbContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
}, poolSize: 128); // Pool up to 128 contexts

// When to use DbContext pooling:
// ✅ High-throughput applications
// ✅ Short-lived contexts
// ✅ Contexts without much state

// When NOT to use:
// ❌ Long-lived contexts
// ❌ Contexts with complex initialization
// ❌ Desktop apps with infrequent database access
```

---

### Multiple DbContexts

```csharp
// Different databases
public class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
        : base(options) { }

    public DbSet<Order> Orders { get; set; }
}

public class CustomersDbContext : DbContext
{
    public CustomersDbContext(DbContextOptions<CustomersDbContext> options)
        : base(options) { }

    public DbSet<Customer> Customers { get; set; }
}

// Registration
services.AddDbContext<OrdersDbContext>(options =>
    options.UseSqlServer(config.GetConnectionString("OrdersDatabase")));

services.AddDbContext<CustomersDbContext>(options =>
    options.UseSqlServer(config.GetConnectionString("CustomersDatabase")));

// Usage
public class MultiDatabaseService
{
    private readonly OrdersDbContext _ordersContext;
    private readonly CustomersDbContext _customersContext;

    public MultiDatabaseService(
        OrdersDbContext ordersContext,
        CustomersDbContext customersContext)
    {
        _ordersContext = ordersContext;
        _customersContext = customersContext;
    }

    public async Task<CustomerOrderInfo> GetInfoAsync(int customerId)
    {
        var customer = await _customersContext.Customers
            .FindAsync(customerId);

        var orders = await _ordersContext.Orders
            .Where(o => o.CustomerId == customerId)
            .ToListAsync();

        return new CustomerOrderInfo { Customer = customer, Orders = orders };
    }
}
```

---

## 🔄 Connection Resilience

### Retry Policies with Polly

```csharp
// Install: dotnet add package Polly

using Polly;
using Polly.Retry;

public class ResilientDatabaseService
{
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly string _connectionString;

    public ResilientDatabaseService(string connectionString)
    {
        _connectionString = connectionString;

        // Exponential backoff retry policy
        _retryPolicy = Policy
            .Handle<SqlException>(ex => IsTransient(ex))
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine($"Retry {retryCount} after {timeSpan.TotalSeconds}s due to: {exception.Message}");
                });
    }

    private static bool IsTransient(SqlException ex)
    {
        // Transient error codes
        int[] transientErrors = { -2, -1, 2, 20, 64, 233, 10053, 10054, 10060, 40197, 40501, 40613 };
        return transientErrors.Contains(ex.Number);
    }

    public async Task<List<Customer>> GetCustomersAsync()
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("SELECT * FROM Customers", connection);
            using var reader = await command.ExecuteReaderAsync();

            return MapCustomers(reader);
        });
    }
}
```

---

### Connection Resiliency in EF Core

```csharp
services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            // ✅ Enable retry on failure
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);

            // ✅ Command timeout
            sqlOptions.CommandTimeout(60);
        });
});

// Custom retry strategy
services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        connectionString,
        sqlOptions =>
        {
            sqlOptions.ExecutionStrategy(dependencies =>
                new CustomExecutionStrategy(dependencies, maxRetryCount: 10));
        });
});
```

---

## 📊 Monitoring and Diagnostics

### Connection Pool Monitoring

```csharp
using System.Diagnostics;

public class ConnectionPoolMonitor
{
    public void MonitorConnectionPool()
    {
        // Performance counters for SQL Server
        var counters = new[]
        {
            new PerformanceCounter(".NET Data Provider for SqlServer",
                "NumberOfActiveConnectionPools",
                Process.GetCurrentProcess().ProcessName),
            new PerformanceCounter(".NET Data Provider for SqlServer",
                "NumberOfActiveConnections",
                Process.GetCurrentProcess().ProcessName),
            new PerformanceCounter(".NET Data Provider for SqlServer",
                "NumberOfPooledConnections",
                Process.GetCurrentProcess().ProcessName),
            new PerformanceCounter(".NET Data Provider for SqlServer",
                "NumberOfFreeConnections",
                Process.GetCurrentProcess().ProcessName)
        };

        foreach (var counter in counters)
        {
            Console.WriteLine($"{counter.CounterName}: {counter.NextValue()}");
        }
    }
}
```

---

### Logging Connection Events

```csharp
public class LoggingConnectionWrapper
{
    private readonly string _connectionString;
    private readonly ILogger _logger;

    public async Task<T> ExecuteAsync<T>(Func<SqlConnection, Task<T>> operation)
    {
        using var connection = new SqlConnection(_connectionString);

        try
        {
            _logger.LogDebug("Opening database connection");
            var sw = Stopwatch.StartNew();

            await connection.OpenAsync();

            sw.Stop();
            _logger.LogDebug("Connection opened in {ElapsedMs}ms", sw.ElapsedMilliseconds);

            var result = await operation(connection);

            _logger.LogDebug("Operation completed successfully");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database operation failed");
            throw;
        }
        finally
        {
            _logger.LogDebug("Closing database connection");
        }
    }
}

// EF Core logging
services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString)
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging() // Only in development!
        .EnableDetailedErrors();
});
```

---

### Detecting Connection Leaks

```csharp
// Symptoms of connection leaks:
// - Application slows down over time
// - "Connection pool exhausted" errors
// - High memory usage

// ✅ Finding leaks with diagnostics
public class ConnectionLeakDetector
{
    private static int _connectionsOpened = 0;
    private static int _connectionsClosed = 0;

    public static SqlConnection CreateMonitoredConnection(string connectionString)
    {
        var connection = new SqlConnection(connectionString);

        connection.StateChange += (sender, args) =>
        {
            if (args.CurrentState == ConnectionState.Open)
            {
                Interlocked.Increment(ref _connectionsOpened);
                Console.WriteLine($"Connection opened. Total: {_connectionsOpened}");
            }
            else if (args.CurrentState == ConnectionState.Closed)
            {
                Interlocked.Increment(ref _connectionsClosed);
                Console.WriteLine($"Connection closed. Total: {_connectionsClosed}");
            }
        };

        return connection;
    }

    public static void ReportLeaks()
    {
        int leakedConnections = _connectionsOpened - _connectionsClosed;
        if (leakedConnections > 0)
        {
            Console.WriteLine($"WARNING: {leakedConnections} connections may be leaked!");
        }
    }
}
```

---

## ✅ Best Practices

### DO:
✅ **Always use `using` statements** for SqlConnection and DbContext
✅ **Store connection strings in configuration files**, never hardcode
✅ **Use connection pooling** (enabled by default)
✅ **Open connections late**, close them early
✅ **Use async/await** for all database operations
✅ **Implement retry logic** for transient failures
✅ **Log connection errors** for troubleshooting
✅ **Use parameterized queries** to prevent SQL injection
✅ **Configure appropriate timeouts** for your scenarios
✅ **Monitor connection pool usage** in production
✅ **Encrypt sensitive connection strings**
✅ **Use separate connection strings** for read and write operations if needed

### DON'T:
❌ **Don't hold connections open** longer than necessary
❌ **Don't create connections** without disposing them
❌ **Don't hardcode connection strings** in source code
❌ **Don't use singleton DbContexts** (not thread-safe)
❌ **Don't disable pooling** without good reason
❌ **Don't ignore connection errors** silently
❌ **Don't reuse connections** across threads
❌ **Don't store passwords** in plain text
❌ **Don't forget to close connections** in exception handlers
❌ **Don't use synchronous database calls** in UI thread

---

## 🚀 Performance Tips

**Connection Pooling Optimization**:
- Set `MinPoolSize` to avoid warm-up delays
- Set `MaxPoolSize` appropriate to your workload
- Use `ConnectionLifetime` to refresh connections periodically

**Minimize Connection Time**:
```csharp
// ✅ Load data, then close connection before processing
using (var connection = new SqlConnection(_connectionString))
{
    await connection.OpenAsync();
    data = await LoadDataAsync(connection);
} // Connection returned to pool

// Process data without holding connection
ProcessData(data);
```

**Async Operations**:
```csharp
// ✅ Use async to avoid blocking threads
await connection.OpenAsync();
await command.ExecuteNonQueryAsync();

// ❌ Don't block async calls
connection.OpenAsync().Wait(); // DON'T DO THIS
```

**Checklist**:
- [ ] Using statement for all connections
- [ ] Async methods for I/O operations
- [ ] Connection opened only when needed
- [ ] No business logic while connection is open
- [ ] Appropriate pool size configured
- [ ] Connection timeout set appropriately
- [ ] Retry logic for transient errors
- [ ] Logging for connection failures

---

## 🔗 Related Topics

- [Entity Framework Core](ef-core-best-practices.md) - ORM best practices
- [Repository Pattern](repository-pattern.md) - Data access abstraction
- [Performance Optimization](../best-practices/performance.md) - Overall performance
- [Error Handling](../best-practices/error-handling.md) - Exception handling
- [Security](../best-practices/security.md) - Securing connections

---

**Last Updated**: 2025-11-07
