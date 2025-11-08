---
description: Add comprehensive error handling to WinForms code
---

You are tasked with adding comprehensive error handling to WinForms code.

## Workflow

1. **Ask the user**:
   - Which file needs error handling?
   - What type of operations does it perform? (database, file I/O, network, etc.)

2. **Read the code** to identify unhandled scenarios

3. **Add error handling** using these patterns:

### Pattern 1: Global Exception Handling

✅ **Setup global handlers in Program.cs**:
```csharp
static class Program
{
    [STAThread]
    static void Main()
    {
        // Setup global exception handlers
        Application.ThreadException += Application_ThreadException;
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledExceptionException;

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }

    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
        LogException(e.Exception);
        MessageBox.Show($"An unexpected error occurred:\n\n{e.Exception.Message}",
            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private static void CurrentDomain_UnhandledExceptionException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;
        LogException(exception);
        MessageBox.Show($"A critical error occurred:\n\n{exception?.Message}",
            "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private static void LogException(Exception ex)
    {
        // Log to file, database, or logging service
        try
        {
            string logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "YourApp", "Logs", "errors.log");

            Directory.CreateDirectory(Path.GetDirectoryName(logPath));

            File.AppendAllText(logPath,
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex}\n\n");
        }
        catch
        {
            // Silently fail if logging fails
        }
    }
}
```

### Pattern 2: Form-Level Error Handling

✅ **Consistent error handling in forms**:
```csharp
public partial class CustomerForm : Form
{
    private readonly ILogger<CustomerForm> _logger;

    private async void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            await ExecuteWithErrorHandlingAsync(async () =>
            {
                await SaveCustomerAsync();
            });
        }
        catch (Exception ex)
        {
            // Already handled in ExecuteWithErrorHandlingAsync
        }
    }

    /// <summary>
    /// Executes an action with comprehensive error handling
    /// </summary>
    private async Task ExecuteWithErrorHandlingAsync(Func<Task> action)
    {
        try
        {
            SetLoadingState(true);
            await action();
        }
        catch (ValidationException vex)
        {
            // User input errors
            _logger.LogWarning(vex, "Validation error");
            MessageBox.Show(vex.Message, "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (UnauthorizedAccessException uex)
        {
            // Permission errors
            _logger.LogWarning(uex, "Unauthorized access");
            MessageBox.Show("You don't have permission to perform this action.",
                "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (DbUpdateException dbEx)
        {
            // Database errors
            _logger.LogError(dbEx, "Database error");
            MessageBox.Show("An error occurred while saving to database. Please try again.",
                "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (HttpRequestException httpEx)
        {
            // Network errors
            _logger.LogError(httpEx, "Network error");
            MessageBox.Show("Network connection error. Please check your internet connection.",
                "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (OperationCanceledException)
        {
            // User cancelled operation
            _logger.LogInformation("Operation cancelled by user");
        }
        catch (Exception ex)
        {
            // Unexpected errors
            _logger.LogError(ex, "Unexpected error");
            MessageBox.Show($"An unexpected error occurred:\n\n{ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            SetLoadingState(false);
        }
    }

    private void SetLoadingState(bool isLoading)
    {
        btnSave.Enabled = !isLoading;
        Cursor = isLoading ? Cursors.WaitCursor : Cursors.Default;
    }
}
```

### Pattern 3: Service Layer Error Handling

✅ **Wrap exceptions with context**:
```csharp
public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<CustomerService> _logger;

    public async Task<Customer> GetCustomerAsync(int id)
    {
        try
        {
            if (id <= 0)
                throw new ArgumentException("Customer ID must be greater than zero", nameof(id));

            _logger.LogInformation("Loading customer {CustomerId}", id);

            var customer = await _repository.GetByIdAsync(id);

            if (customer == null)
            {
                _logger.LogWarning("Customer {CustomerId} not found", id);
                throw new NotFoundException($"Customer with ID {id} not found");
            }

            return customer;
        }
        catch (NotFoundException)
        {
            // Re-throw domain exceptions
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading customer {CustomerId}", id);
            throw new ServiceException($"Failed to load customer {id}", ex);
        }
    }

    public async Task CreateCustomerAsync(Customer customer)
    {
        try
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            // Validation
            ValidateCustomer(customer);

            _logger.LogInformation("Creating customer {CustomerName}", customer.Name);

            await _repository.AddAsync(customer);

            _logger.LogInformation("Customer {CustomerId} created successfully", customer.Id);
        }
        catch (ValidationException)
        {
            // Re-throw validation exceptions
            throw;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error creating customer");
            throw new ServiceException("Failed to create customer due to database error", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating customer");
            throw new ServiceException("An unexpected error occurred while creating customer", ex);
        }
    }

    private void ValidateCustomer(Customer customer)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(customer.Name))
            errors.Add("Customer name is required");

        if (customer.Name?.Length < 3)
            errors.Add("Customer name must be at least 3 characters");

        if (string.IsNullOrWhiteSpace(customer.Email))
            errors.Add("Email is required");

        if (!IsValidEmail(customer.Email))
            errors.Add("Invalid email format");

        if (errors.Any())
        {
            throw new ValidationException(string.Join("\n", errors));
        }
    }
}
```

### Pattern 4: Custom Exception Types

✅ **Create custom exceptions for different scenarios**:
```csharp
// Base exception for application
public class AppException : Exception
{
    public AppException(string message) : base(message) { }
    public AppException(string message, Exception innerException)
        : base(message, innerException) { }
}

// Validation errors
public class ValidationException : AppException
{
    public ValidationException(string message) : base(message) { }
}

// Not found errors
public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message) { }
}

// Service layer errors
public class ServiceException : AppException
{
    public ServiceException(string message, Exception innerException)
        : base(message, innerException) { }
}

// Unauthorized access
public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message) : base(message) { }
}
```

### Pattern 5: Retry Logic for Transient Errors

✅ **Implement retry for network/database operations**:
```csharp
private async Task<T> ExecuteWithRetryAsync<T>(
    Func<Task<T>> operation,
    int maxRetries = 3,
    int delayMs = 1000)
{
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            return await operation();
        }
        catch (Exception ex) when (IsTransientError(ex) && attempt < maxRetries)
        {
            _logger.LogWarning(ex,
                "Transient error on attempt {Attempt}/{MaxRetries}. Retrying...",
                attempt, maxRetries);

            await Task.Delay(delayMs * attempt); // Exponential backoff
        }
    }

    // Last attempt without catching
    return await operation();
}

private bool IsTransientError(Exception ex)
{
    return ex is HttpRequestException ||
           ex is TimeoutException ||
           (ex is SqlException sqlEx && IsTransientSqlError(sqlEx));
}

private bool IsTransientSqlError(SqlException ex)
{
    // SQL Server transient error codes
    int[] transientErrorNumbers = { -2, -1, 2, 20, 64, 233, 10053, 10054, 10060, 40197, 40501, 40613 };
    return transientErrorNumbers.Contains(ex.Number);
}

// Usage
var customer = await ExecuteWithRetryAsync(async () =>
    await _repository.GetByIdAsync(customerId));
```

### Pattern 6: User-Friendly Error Messages

✅ **Show appropriate messages based on error type**:
```csharp
private void ShowErrorMessage(Exception ex)
{
    string title;
    string message;
    MessageBoxIcon icon;

    switch (ex)
    {
        case ValidationException vex:
            title = "Validation Error";
            message = vex.Message;
            icon = MessageBoxIcon.Warning;
            break;

        case NotFoundException nex:
            title = "Not Found";
            message = nex.Message;
            icon = MessageBoxIcon.Information;
            break;

        case UnauthorizedException uex:
            title = "Access Denied";
            message = "You don't have permission to perform this action.";
            icon = MessageBoxIcon.Warning;
            break;

        case DbUpdateException:
            title = "Database Error";
            message = "An error occurred while saving. Please try again.";
            icon = MessageBoxIcon.Error;
            break;

        case HttpRequestException:
            title = "Connection Error";
            message = "Unable to connect to the server. Please check your connection.";
            icon = MessageBoxIcon.Error;
            break;

        default:
            title = "Error";
            message = $"An unexpected error occurred:\n\n{ex.Message}";
            icon = MessageBoxIcon.Error;
            break;
    }

    MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
}
```

4. **Error Handling Checklist**:
   - [ ] Global exception handlers in Program.cs
   - [ ] Try-catch blocks around all I/O operations
   - [ ] Appropriate exception types (custom or built-in)
   - [ ] User-friendly error messages
   - [ ] Logging all errors with context
   - [ ] Retry logic for transient errors
   - [ ] Finally blocks for cleanup
   - [ ] Don't swallow exceptions silently
   - [ ] Re-throw exceptions when appropriate
   - [ ] Dispose resources even on error

5. **Show the user**:
   - Updated code with comprehensive error handling
   - Custom exception classes if created
   - Explanation of error handling strategy
   - Logging implementation
   - Offer to add more error handling scenarios
