# Error Handling & Logging

> **Quick Reference**: Proper exception handling and logging patterns for WinForms applications.

---

## 🎯 Exception Handling Pattern

```csharp
private async void btnSave_Click(object sender, EventArgs e)
{
    try
    {
        // Validate first
        if (!ValidateForm())
            return;

        // Show loading
        SetLoadingState(true);

        // Perform operation
        var customer = GetCustomerFromForm();
        await _service.SaveAsync(customer);

        // Success
        _logger.LogInformation("Customer saved: {CustomerId}", customer.Id);
        MessageBox.Show("Customer saved successfully", "Success",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        this.DialogResult = DialogResult.OK;
    }
    catch (ValidationException vex)
    {
        // Expected business exception
        _logger.LogWarning(vex, "Validation failed");
        MessageBox.Show(vex.Message, "Validation Error",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
    catch (DbUpdateException dbEx)
    {
        // Database-specific exception
        _logger.LogError(dbEx, "Database error saving customer");
        MessageBox.Show("Failed to save to database. Please try again.",
            "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    catch (Exception ex)
    {
        // Unexpected exception
        _logger.LogError(ex, "Unexpected error saving customer");
        MessageBox.Show("An unexpected error occurred. Please contact support.",
            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    finally
    {
        // Always cleanup
        SetLoadingState(false);
    }
}
```

---

## 📝 Logging with Serilog

### Setup (Program.cs)
```csharp
services.AddLogging(configure =>
{
    var logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
        .WriteTo.Console()
        .CreateLogger();

    configure.AddSerilog(logger);
});
```

### Usage
```csharp
public class CustomerService
{
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(ILogger<CustomerService> logger)
    {
        _logger = logger;
    }

    public async Task<Customer> GetByIdAsync(int id)
    {
        _logger.LogInformation("Loading customer {CustomerId}", id);

        try
        {
            var customer = await _repository.GetByIdAsync(id);

            if (customer == null)
            {
                _logger.LogWarning("Customer {CustomerId} not found", id);
                return null;
            }

            _logger.LogDebug("Customer loaded: {@Customer}", customer);
            return customer;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading customer {CustomerId}", id);
            throw;
        }
    }
}
```

---

## 🌍 Global Exception Handler

```csharp
// Program.cs
static void Main()
{
    Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
    Application.ThreadException += Application_ThreadException;
    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

    // ... rest of setup
}

private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
{
    LogException(e.Exception);
    MessageBox.Show("An unexpected error occurred. The application will continue.",
        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
}

private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
{
    LogException(e.ExceptionObject as Exception);
    MessageBox.Show("A fatal error occurred. The application will now close.",
        "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
}

private static void LogException(Exception? ex)
{
    // Log to file/service
    File.AppendAllText("crash.log",
        $"[{DateTime.Now}] {ex?.ToString()}\n\n");
}
```

---

## ✅ Best Practices

### DO:
✅ Catch specific exceptions first (ValidationException, DbException)
✅ Log all exceptions with context
✅ Show user-friendly messages
✅ Use finally for cleanup
✅ Set up global exception handler

### DON'T:
❌ Don't swallow exceptions silently
❌ Don't expose technical details to users
❌ Don't catch Exception unless necessary
❌ Don't use exceptions for flow control

---

**Last Updated**: 2025-11-07
