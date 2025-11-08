# Code Review Checklist

Use this checklist before committing code to ensure quality and adherence to standards.

---

## ✅ Pre-Commit Checklist

### 1. Compilation & Build

- [ ] **Code compiles** without errors
- [ ] **No build warnings** (treat warnings as errors)
- [ ] **All projects in solution build** successfully

**Verify**:
```bash
dotnet build
```

---

### 2. Testing

- [ ] **All existing tests pass**
- [ ] **New tests added** for new code
- [ ] **Tests cover edge cases** and error scenarios
- [ ] **Test coverage meets standards** (80%+ for services)

**Verify**:
```bash
dotnet test
dotnet test /p:CollectCoverage=true
```

---

### 3. Architecture & Patterns

- [ ] **No business logic in Forms** - Moved to Services or Presenters
- [ ] **MVP/MVVM pattern followed** - Proper separation of concerns
- [ ] **Services use dependency injection** - Constructor injection
- [ ] **Repositories use EF Core correctly** - Async methods, proper disposal
- [ ] **No direct database access from Forms** - Use Services → Repositories

**Check**:
- Forms only handle UI updates
- Presenters coordinate between View and Service
- Services contain business logic
- Repositories handle data access

---

### 4. Resource Management

- [ ] **All IDisposable resources disposed** - Use `using` statements or Dispose()
- [ ] **Form.Dispose() implemented correctly** - Dispose timers, event subscriptions
- [ ] **No memory leaks** - Unsubscribe from events
- [ ] **Database connections closed** - DbContext disposed properly

**Common issues**:
```csharp
// ❌ BAD - Memory leak
private Timer _timer = new Timer();

// ✅ GOOD - Proper disposal
private Timer _timer;
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        _timer?.Dispose();
    }
    base.Dispose(disposing);
}
```

---

### 5. Async/Await

- [ ] **Async/await used for all I/O** - Database, file, network operations
- [ ] **Async methods named with Async suffix** - e.g., `LoadDataAsync()`
- [ ] **Async all the way** - Don't block on async (no `.Result` or `.Wait()`)
- [ ] **CancellationToken support** (for long-running operations)

**Check**:
```csharp
// ❌ BAD - Blocking async
var data = _service.GetDataAsync().Result;

// ✅ GOOD - Proper async
var data = await _service.GetDataAsync();
```

---

### 6. Input Validation

- [ ] **User input validated** - Before processing
- [ ] **ErrorProvider used** (WinForms) or validation logic
- [ ] **Null checks** - ArgumentNullException for required parameters
- [ ] **Business rule validation** - In Service layer

**Example**:
```csharp
// ✅ GOOD - Proper validation
public async Task CreateCustomerAsync(Customer customer)
{
    ArgumentNullException.ThrowIfNull(customer);

    if (string.IsNullOrWhiteSpace(customer.Name))
        throw new ValidationException("Customer name is required");

    // Proceed with creation
}
```

---

### 7. Error Handling & Logging

- [ ] **Try-catch blocks** - Around risky operations
- [ ] **Exceptions logged** - Using ILogger
- [ ] **User-friendly error messages** - No technical jargon in UI
- [ ] **Exceptions not swallowed** - Log or re-throw
- [ ] **Specific exceptions caught** - Avoid `catch (Exception)` unless necessary

**Example**:
```csharp
// ✅ GOOD - Proper error handling
try
{
    await _service.SaveDataAsync(data);
}
catch (ValidationException vex)
{
    _logger.LogWarning(vex, "Validation failed");
    MessageBox.Show(vex.Message, "Validation Error");
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error saving data");
    MessageBox.Show("An error occurred. Please try again.", "Error");
}
```

---

### 8. Thread Safety

- [ ] **UI updates on UI thread** - Use `Invoke` or `BeginInvoke`
- [ ] **No UI controls accessed from background threads**
- [ ] **Async methods used instead of BackgroundWorker** (modern .NET)

**Example**:
```csharp
// ✅ GOOD - Thread-safe UI update
private void UpdateStatus(string message)
{
    if (InvokeRequired)
    {
        Invoke(new Action(() => UpdateStatus(message)));
        return;
    }
    lblStatus.Text = message;
}
```

---

### 9. Code Quality

- [ ] **No magic numbers or strings** - Use constants
- [ ] **XML documentation comments** - On all public APIs
- [ ] **Follows naming conventions** - PascalCase, camelCase, etc.
- [ ] **Code is readable** - Clear variable names, logical structure
- [ ] **No commented-out code** - Remove or add explanation
- [ ] **No TODO comments without Jira ticket** - Track technical debt

**Example**:
```csharp
// ❌ BAD - Magic numbers
if (customer.Age < 18) { }

// ✅ GOOD - Named constant
private const int MINIMUM_AGE = 18;
if (customer.Age < MINIMUM_AGE) { }
```

---

### 10. Documentation

- [ ] **XML comments on public methods** - /// summary, param, returns
- [ ] **README updated** (if applicable)
- [ ] **CHANGELOG updated** (if applicable)
- [ ] **Code examples updated** (if pattern changed)

---

### 11. Security

- [ ] **No hardcoded credentials** - Use configuration
- [ ] **SQL injection prevented** - Use parameterized queries (EF Core handles this)
- [ ] **User input sanitized** - Prevent XSS, command injection
- [ ] **Sensitive data not logged** - No passwords, credit cards in logs
- [ ] **No secrets in source control** - Use .env, Azure Key Vault, etc.

**Example**:
```csharp
// ❌ BAD - Hardcoded connection string
var conn = "Server=prod;User=admin;Password=password123";

// ✅ GOOD - From configuration
var conn = _configuration.GetConnectionString("DefaultConnection");
```

---

### 12. Performance

- [ ] **No N+1 queries** - Use `.Include()` for related data
- [ ] **Large lists virtualized** - Use DataGridView virtualization
- [ ] **Images optimized** - Correct size, compressed
- [ ] **No unnecessary database calls** - Cache where appropriate

---

## 🔴 Critical Issues (Must Fix)

These issues MUST be fixed before committing:

1. **Code doesn't compile**
2. **Tests are failing**
3. **Business logic in Forms** (violates architecture)
4. **Memory leaks** (undisposed resources)
5. **Security vulnerabilities** (hardcoded credentials, SQL injection)
6. **Data loss risk** (no error handling on save operations)

---

## 🟡 Important Issues (Should Fix)

These should be fixed before committing:

1. Missing async/await on I/O operations
2. Missing input validation
3. Missing error handling/logging
4. Thread safety issues
5. Poor naming conventions

---

## 🟢 Minor Issues (Nice to Have)

These can be addressed later:

1. Missing XML documentation
2. Code formatting inconsistencies
3. Performance optimizations
4. Refactoring opportunities

---

## Automated Checks (Phase 2+)

When `winforms-reviewer` agent is available, run automated review:

```
/review-code [files]
```

This will check all items above automatically and generate a report.

---

**Last Updated**: 2025-11-08 (Phase 1)
**Version**: 1.0
