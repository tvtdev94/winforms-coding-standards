# Code Review Comment Templates

Reusable templates for common code review issues in WinForms projects.

---

## How to Use These Templates

1. **Copy the relevant template** for the issue you found
2. **Customize** with specific file paths, line numbers, and code examples
3. **Add context** specific to the PR being reviewed
4. **Be kind** - Remember there's a person receiving this feedback

**Format**:
- **Severity Level**: 🔴 Critical / 🟡 Major / 🟢 Minor
- **Location**: File path and line numbers
- **Issue**: What's wrong
- **Why**: Why it matters
- **Fix**: How to correct it
- **Reference**: Link to documentation

---

## Architecture Issues

### Template: Business Logic in Form

**Severity**: 🔴 Critical

**Location**: `Forms/CustomerForm.cs:45-60`

**Issue**:
Business logic (validation and data processing) is implemented directly in the Form. This violates the MVP pattern and makes the code difficult to test.

**Current Code**:
```csharp
private async void btnSave_Click(object sender, EventArgs e)
{
    // ❌ Business logic in Form
    if (string.IsNullOrWhiteSpace(txtName.Text))
    {
        MessageBox.Show("Name is required");
        return;
    }

    if (txtEmail.Text.Contains("@"))
    {
        MessageBox.Show("Invalid email");
        return;
    }

    var customer = new Customer
    {
        Name = txtName.Text,
        Email = txtEmail.Text
    };

    await _repository.AddAsync(customer); // Direct data access
}
```

**Why This Matters**:
- **Untestable**: Cannot unit test validation logic without creating actual Form
- **Maintainability**: Business rules scattered across Forms instead of centralized
- **Reusability**: Cannot reuse validation logic in other parts of the app
- **Coupling**: Form tightly coupled to data layer

**Recommended Fix**:

1. Move validation to Service layer:
```csharp
// CustomerService.cs
public async Task<Customer> CreateCustomerAsync(string name, string email)
{
    // Validation in Service (testable)
    if (string.IsNullOrWhiteSpace(name))
        throw new ValidationException("Name is required");

    if (!email.Contains("@"))
        throw new ValidationException("Invalid email");

    var customer = new Customer { Name = name, Email = email };
    return await _repository.AddAsync(customer);
}
```

2. Use Presenter to coordinate:
```csharp
// CustomerPresenter.cs
public async Task HandleSaveAsync()
{
    try
    {
        var customer = await _service.CreateCustomerAsync(
            _view.CustomerName,
            _view.CustomerEmail);

        _view.ShowSuccess("Customer saved successfully");
        _view.Close();
    }
    catch (ValidationException vex)
    {
        _view.ShowError(vex.Message);
    }
}
```

3. Form just wires UI:
```csharp
// CustomerForm.cs
private async void btnSave_Click(object sender, EventArgs e)
{
    // ✅ Minimal logic - delegate to Presenter
    await _presenter.HandleSaveAsync();
}
```

**Reference**:
- `docs/architecture/mvp-pattern.md`
- `docs/examples/mvp-example.md`

---

### Template: Missing Presenter (Direct Service Call from Form)

**Severity**: 🔴 Critical

**Location**: `Forms/OrderForm.cs:33`

**Issue**:
Form directly calls Service without going through a Presenter. This violates MVP pattern.

**Current Code**:
```csharp
public partial class OrderForm : Form
{
    private readonly IOrderService _service;

    public OrderForm(IOrderService service)
    {
        InitializeComponent();
        _service = service; // ❌ Form has direct Service dependency
    }

    private async void btnLoad_Click(object sender, EventArgs e)
    {
        var orders = await _service.GetOrdersAsync(); // ❌ Direct call
        dgvOrders.DataSource = orders;
    }
}
```

**Why This Matters**:
- Violates MVP pattern (no Presenter layer)
- Form has business logic knowledge
- Hard to unit test
- Cannot easily change business logic without touching Form

**Recommended Fix**:

1. Create IView interface:
```csharp
public interface IOrderView
{
    event Func<Task> LoadRequested;
    List<Order> Orders { set; }
    void ShowError(string message);
}
```

2. Create Presenter:
```csharp
public class OrderPresenter
{
    private readonly IOrderView _view;
    private readonly IOrderService _service;

    public OrderPresenter(IOrderView view, IOrderService service)
    {
        _view = view;
        _service = service;
    }

    public async Task HandleLoadAsync()
    {
        try
        {
            var orders = await _service.GetOrdersAsync();
            _view.Orders = orders;
        }
        catch (Exception ex)
        {
            _view.ShowError($"Failed to load orders: {ex.Message}");
        }
    }
}
```

3. Update Form:
```csharp
public partial class OrderForm : Form, IOrderView
{
    private readonly OrderPresenter _presenter;

    public OrderForm(OrderPresenter presenter)
    {
        InitializeComponent();
        _presenter = presenter;
    }

    public List<Order> Orders
    {
        set => dgvOrders.DataSource = value;
    }

    private async void btnLoad_Click(object sender, EventArgs e)
    {
        await _presenter.HandleLoadAsync();
    }
}
```

**Reference**: `docs/architecture/mvp-pattern.md`

---

## Async/Await Issues

### Template: Blocking on Async

**Severity**: 🔴 Critical

**Location**: `Services/CustomerService.cs:28`

**Issue**:
Blocking on async operation using `.Result` or `.Wait()`. This can cause deadlocks in UI applications.

**Current Code**:
```csharp
public Customer GetCustomer(int id)
{
    // ❌ Blocking on async - can cause deadlock
    return _repository.GetByIdAsync(id).Result;
}
```

**Why This Matters**:
- **Deadlock Risk**: In UI applications, this can freeze the entire app
- **Performance**: Blocks thread instead of allowing async continuation
- **Best Practice**: "Async all the way" - don't mix sync and async

**Recommended Fix**:
```csharp
public async Task<Customer> GetCustomerAsync(int id)
{
    // ✅ Async all the way
    return await _repository.GetByIdAsync(id);
}
```

**Reference**: `docs/best-practices/async-await.md`

---

### Template: Missing Async on I/O

**Severity**: 🟡 Major

**Location**: `Repositories/OrderRepository.cs:15-20`

**Issue**:
Synchronous database operations used instead of async. This blocks threads unnecessarily.

**Current Code**:
```csharp
public List<Order> GetAll()
{
    // ❌ Synchronous query
    return _context.Orders.ToList();
}
```

**Why This Matters**:
- Blocks thread during I/O
- Poor scalability
- WinForms UI will freeze during operation

**Recommended Fix**:
```csharp
public async Task<List<Order>> GetAllAsync()
{
    // ✅ Async query
    return await _context.Orders.ToListAsync();
}
```

**Reference**: `docs/best-practices/async-await.md`

---

### Template: Missing Async Suffix

**Severity**: 🟢 Minor

**Location**: `Services/ProductService.cs:42`

**Issue**:
Async method doesn't have `Async` suffix. This violates naming conventions.

**Current Code**:
```csharp
// ❌ Missing Async suffix
public async Task<Product> GetProduct(int id)
{
    return await _repository.GetByIdAsync(id);
}
```

**Why This Matters**:
- Violates C# naming conventions
- Makes it unclear that method is async
- Harder to maintain

**Recommended Fix**:
```csharp
// ✅ Has Async suffix
public async Task<Product> GetProductAsync(int id)
{
    return await _repository.GetByIdAsync(id);
}
```

**Reference**: `docs/conventions/naming-conventions.md`

---

## Resource Management Issues

### Template: Undisposed Resource

**Severity**: 🔴 Critical

**Location**: `Forms/ReportForm.cs:55-60`

**Issue**:
IDisposable resource (`FileStream`) is not disposed. This causes resource leak.

**Current Code**:
```csharp
private void ExportData()
{
    // ❌ Not disposed - resource leak
    var stream = File.OpenWrite("export.txt");
    var writer = new StreamWriter(stream);
    writer.Write(data);
}
```

**Why This Matters**:
- **Resource Leak**: File handles remain open
- **Crashes**: Can hit OS limits on open files
- **Data Loss**: Data may not be flushed to disk

**Recommended Fix**:
```csharp
private void ExportData()
{
    // ✅ Using statement ensures disposal
    using var stream = File.OpenWrite("export.txt");
    using var writer = new StreamWriter(stream);
    writer.Write(data);
} // Automatically disposed here
```

**Reference**: `docs/best-practices/resource-management.md`

---

### Template: Missing DetachView in Dispose

**Severity**: 🟡 Major

**Location**: `Forms/CustomerForm.cs:100-105`

**Issue**:
Form doesn't call `DetachView()` in `Dispose()`. This prevents Presenter from unsubscribing from View events, causing memory leak.

**Current Code**:
```csharp
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        components?.Dispose();
        // ❌ Missing _presenter.DetachView()
    }
    base.Dispose(disposing);
}
```

**Why This Matters**:
- **Memory Leak**: Presenter still holds reference to View
- **Event Leaks**: View events not unsubscribed
- **GC Cannot Collect**: Form cannot be garbage collected

**Recommended Fix**:
```csharp
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        _presenter?.DetachView(); // ✅ Detach before disposal
        components?.Dispose();
    }
    base.Dispose(disposing);
}
```

**Reference**: `docs/architecture/mvp-pattern.md`

---

## Validation Issues

### Template: Missing Input Validation

**Severity**: 🟡 Major

**Location**: `Services/OrderService.cs:33`

**Issue**:
No validation of input parameters. This can lead to invalid data being saved or exceptions.

**Current Code**:
```csharp
public async Task CreateOrderAsync(Order order)
{
    // ❌ No validation
    return await _repository.AddAsync(order);
}
```

**Why This Matters**:
- Invalid data can be saved to database
- Unclear error messages for users
- Business rules not enforced

**Recommended Fix**:
```csharp
public async Task CreateOrderAsync(Order order)
{
    // ✅ Validate inputs
    ArgumentNullException.ThrowIfNull(order);

    if (order.CustomerId <= 0)
        throw new ValidationException("Customer is required");

    if (order.Items == null || !order.Items.Any())
        throw new ValidationException("Order must have at least one item");

    if (order.TotalAmount <= 0)
        throw new ValidationException("Order total must be positive");

    return await _repository.AddAsync(order);
}
```

**Reference**: `docs/ui-ux/input-validation.md`

---

### Template: Missing Null Check

**Severity**: 🟡 Major

**Location**: `Services/CustomerService.cs:18`

**Issue**:
Missing null check on parameter. Can cause NullReferenceException.

**Current Code**:
```csharp
public async Task UpdateCustomerAsync(Customer customer)
{
    // ❌ No null check
    customer.UpdatedAt = DateTime.UtcNow;
    await _repository.UpdateAsync(customer);
}
```

**Why This Matters**:
- Runtime NullReferenceException
- Unclear error for caller
- Violates defensive programming

**Recommended Fix**:
```csharp
public async Task UpdateCustomerAsync(Customer customer)
{
    // ✅ Check for null
    ArgumentNullException.ThrowIfNull(customer);

    customer.UpdatedAt = DateTime.UtcNow;
    await _repository.UpdateAsync(customer);
}
```

**Reference**: `docs/best-practices/error-handling.md`

---

## Error Handling Issues

### Template: Swallowed Exception

**Severity**: 🔴 Critical

**Location**: `Forms/MainForm.cs:78-82`

**Issue**:
Exception is caught but not logged or handled. Silent failures make debugging impossible.

**Current Code**:
```csharp
try
{
    await _service.SaveDataAsync(data);
}
catch
{
    // ❌ Exception swallowed - no logging, no user feedback
}
```

**Why This Matters**:
- **Silent Failures**: User doesn't know if operation succeeded
- **Debugging Nightmare**: No logs to investigate issues
- **Data Loss**: User thinks data was saved but wasn't

**Recommended Fix**:
```csharp
try
{
    await _service.SaveDataAsync(data);
    MessageBox.Show("Data saved successfully", "Success");
}
catch (ValidationException vex)
{
    _logger.LogWarning(vex, "Validation failed");
    MessageBox.Show(vex.Message, "Validation Error",
        MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Failed to save data");
    MessageBox.Show("An error occurred while saving. Please try again.",
        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
```

**Reference**: `docs/best-practices/error-handling.md`

---

### Template: Generic Exception Catch

**Severity**: 🟡 Major

**Location**: `Services/ProductService.cs:55-60`

**Issue**:
Catching generic `Exception` without handling specific exceptions first. Makes error handling imprecise.

**Current Code**:
```csharp
try
{
    return await _repository.GetByIdAsync(id);
}
catch (Exception ex)
{
    // ❌ Too generic - ValidationException, DbException, etc. all handled the same
    _logger.LogError(ex, "Error");
    throw new ServiceException("Error occurred");
}
```

**Why This Matters**:
- Loses specific error information
- Cannot handle different errors appropriately
- User gets generic error message

**Recommended Fix**:
```csharp
try
{
    return await _repository.GetByIdAsync(id);
}
catch (ValidationException vex)
{
    // ✅ Handle specific exceptions first
    _logger.LogWarning(vex, "Validation failed for ID {Id}", id);
    throw; // Re-throw to caller
}
catch (DbException dbEx)
{
    _logger.LogError(dbEx, "Database error getting product {Id}", id);
    throw new ServiceException("Database error occurred", dbEx);
}
catch (Exception ex)
{
    // Generic handler last
    _logger.LogError(ex, "Unexpected error getting product {Id}", id);
    throw new ServiceException("Unexpected error occurred", ex);
}
```

**Reference**: `docs/best-practices/error-handling.md`

---

## Thread Safety Issues

### Template: Cross-Thread UI Access

**Severity**: 🔴 Critical

**Location**: `Forms/DashboardForm.cs:45-50`

**Issue**:
UI controls accessed from background thread without Invoke. This will cause `InvalidOperationException`.

**Current Code**:
```csharp
private async void btnRefresh_Click(object sender, EventArgs e)
{
    await Task.Run(() =>
    {
        var data = _service.GetData();
        lblStatus.Text = "Loaded"; // ❌ Cross-thread violation
        dgvData.DataSource = data; // ❌ Cross-thread violation
    });
}
```

**Why This Matters**:
- **Crashes**: InvalidOperationException at runtime
- **Corruption**: UI state can become corrupted
- **Race Conditions**: Unpredictable behavior

**Recommended Fix**:

Option 1 - Use async/await (preferred):
```csharp
private async void btnRefresh_Click(object sender, EventArgs e)
{
    // ✅ Async method - automatically marshals back to UI thread
    var data = await _service.GetDataAsync();
    lblStatus.Text = "Loaded"; // Safe - back on UI thread
    dgvData.DataSource = data;
}
```

Option 2 - Use Invoke if needed:
```csharp
private void UpdateStatus(string message)
{
    if (InvokeRequired)
    {
        Invoke(() => UpdateStatus(message));
        return;
    }
    lblStatus.Text = message; // Safe - on UI thread
}
```

**Reference**: `docs/best-practices/thread-safety.md`

---

## Testing Issues

### Template: Missing Tests

**Severity**: 🟡 Major

**Location**: `Services/PaymentService.cs`

**Issue**:
New service added without unit tests. This makes it difficult to verify correctness and prevent regressions.

**Why This Matters**:
- Cannot verify business logic works correctly
- Regressions may go unnoticed
- Refactoring becomes risky

**Recommended Fix**:

Create test file `PaymentServiceTests.cs`:
```csharp
public class PaymentServiceTests
{
    [Fact]
    public async Task ProcessPaymentAsync_ValidPayment_ReturnsSuccess()
    {
        // Arrange
        var mockRepo = new Mock<IPaymentRepository>();
        var mockLogger = new Mock<ILogger<PaymentService>>();
        var service = new PaymentService(mockRepo.Object, mockLogger.Object);

        var payment = new Payment { Amount = 100, CustomerId = 1 };
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Payment>()))
            .ReturnsAsync(payment);

        // Act
        var result = await service.ProcessPaymentAsync(payment);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PaymentStatus.Success, result.Status);
    }

    [Fact]
    public async Task ProcessPaymentAsync_NegativeAmount_ThrowsValidationException()
    {
        // Arrange
        var mockRepo = new Mock<IPaymentRepository>();
        var mockLogger = new Mock<ILogger<PaymentService>>();
        var service = new PaymentService(mockRepo.Object, mockLogger.Object);

        var payment = new Payment { Amount = -100 };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => service.ProcessPaymentAsync(payment));
    }
}
```

**Reference**: `docs/testing/unit-testing.md`

---

### Template: Poor Test Naming

**Severity**: 🟢 Minor

**Location**: `Tests/CustomerServiceTests.cs:25`

**Issue**:
Test name doesn't follow convention: `MethodName_Scenario_ExpectedResult`

**Current Code**:
```csharp
[Fact]
public async Task Test1() // ❌ Unclear what is being tested
{
    var result = await _service.CreateCustomerAsync(customer);
    Assert.NotNull(result);
}
```

**Why This Matters**:
- Hard to understand what test does
- Failures are confusing
- Test organization suffers

**Recommended Fix**:
```csharp
[Fact]
public async Task CreateCustomerAsync_ValidCustomer_ReturnsCreatedCustomer()
{
    // ✅ Clear naming: Method_Scenario_ExpectedResult
    var result = await _service.CreateCustomerAsync(customer);
    Assert.NotNull(result);
}
```

**Reference**: `docs/testing/unit-testing.md`

---

## Security Issues

### Template: Hardcoded Credentials

**Severity**: 🔴 Critical

**Location**: `Data/AppDbContext.cs:15`

**Issue**:
Database credentials hardcoded in source code. This is a serious security vulnerability.

**Current Code**:
```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    // ❌ Hardcoded credentials - NEVER do this
    optionsBuilder.UseSqlServer(
        "Server=prod;Database=app;User=admin;Password=P@ssw0rd123");
}
```

**Why This Matters**:
- **Security Breach**: Credentials exposed in source control
- **Compliance**: Violates security policies
- **Attack Vector**: Anyone with repo access has DB access

**Recommended Fix**:

1. Use configuration file:
```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod;Database=app;User=admin;Password=..."
  }
}
```

2. Inject connection string:
```csharp
// Program.cs
services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection")));
```

3. Use environment variables or Azure Key Vault for production

**Reference**: `docs/best-practices/security.md`

---

### Template: SQL Injection Risk

**Severity**: 🔴 Critical

**Location**: `Repositories/ProductRepository.cs:44`

**Issue**:
Raw SQL query with string concatenation. This is vulnerable to SQL injection.

**Current Code**:
```csharp
public async Task<List<Product>> SearchAsync(string name)
{
    // ❌ SQL Injection vulnerability
    var sql = $"SELECT * FROM Products WHERE Name LIKE '%{name}%'";
    return await _context.Products.FromSqlRaw(sql).ToListAsync();
}
```

**Why This Matters**:
- **Critical Security Flaw**: Attacker can execute arbitrary SQL
- **Data Breach**: Can read entire database
- **Data Loss**: Can delete or modify data

**Recommended Fix**:

Option 1 - Use parameterized query:
```csharp
public async Task<List<Product>> SearchAsync(string name)
{
    // ✅ Parameterized query - safe from injection
    return await _context.Products
        .FromSqlRaw("SELECT * FROM Products WHERE Name LIKE {0}", $"%{name}%")
        .ToListAsync();
}
```

Option 2 - Use LINQ (preferred):
```csharp
public async Task<List<Product>> SearchAsync(string name)
{
    // ✅ LINQ - EF Core handles parameterization
    return await _context.Products
        .Where(p => p.Name.Contains(name))
        .ToListAsync();
}
```

**Reference**: `docs/best-practices/security.md`

---

## Performance Issues

### Template: N+1 Query Problem

**Severity**: 🟡 Major

**Location**: `Repositories/OrderRepository.cs:22-28`

**Issue**:
N+1 query problem - loading related entities in a loop. This causes excessive database roundtrips.

**Current Code**:
```csharp
public async Task<List<Order>> GetOrdersWithCustomersAsync()
{
    var orders = await _context.Orders.ToListAsync();

    // ❌ N+1 problem - one query per order
    foreach (var order in orders)
    {
        order.Customer = await _context.Customers
            .FindAsync(order.CustomerId);
    }

    return orders;
}
```

**Why This Matters**:
- **Performance**: 1 + N database queries instead of 1
- **Scalability**: Gets exponentially worse with more data
- **User Experience**: Slow loading times

**Recommended Fix**:
```csharp
public async Task<List<Order>> GetOrdersWithCustomersAsync()
{
    // ✅ Single query with Include - no N+1 problem
    return await _context.Orders
        .Include(o => o.Customer)
        .ToListAsync();
}
```

**Reference**: `docs/best-practices/performance.md`

---

## Code Quality Issues

### Template: Magic Numbers

**Severity**: 🟢 Minor

**Location**: `Services/DiscountService.cs:18`

**Issue**:
Magic numbers used instead of named constants. Reduces code readability.

**Current Code**:
```csharp
public decimal CalculateDiscount(decimal amount, string tier)
{
    // ❌ Magic numbers - what do 0.1, 0.2, 0.3 mean?
    return tier switch
    {
        "Bronze" => amount * 0.1m,
        "Silver" => amount * 0.2m,
        "Gold" => amount * 0.3m,
        _ => 0
    };
}
```

**Why This Matters**:
- Hard to understand what numbers represent
- Difficult to maintain (change in one place)
- Error-prone (easy to mistype)

**Recommended Fix**:
```csharp
// ✅ Named constants - clear and maintainable
private const decimal BRONZE_DISCOUNT_RATE = 0.1m;
private const decimal SILVER_DISCOUNT_RATE = 0.2m;
private const decimal GOLD_DISCOUNT_RATE = 0.3m;

public decimal CalculateDiscount(decimal amount, string tier)
{
    return tier switch
    {
        "Bronze" => amount * BRONZE_DISCOUNT_RATE,
        "Silver" => amount * SILVER_DISCOUNT_RATE,
        "Gold" => amount * GOLD_DISCOUNT_RATE,
        _ => 0
    };
}
```

**Reference**: `docs/conventions/code-style.md`

---

### Template: Missing XML Documentation

**Severity**: 🟢 Minor

**Location**: `Services/EmailService.cs:15`

**Issue**:
Public API method missing XML documentation comments.

**Current Code**:
```csharp
// ❌ No XML documentation
public async Task SendEmailAsync(string to, string subject, string body)
{
    // ...
}
```

**Why This Matters**:
- IntelliSense doesn't show method documentation
- Harder for other developers to understand usage
- No parameter descriptions

**Recommended Fix**:
```csharp
/// <summary>
/// Sends an email asynchronously.
/// </summary>
/// <param name="to">The recipient email address.</param>
/// <param name="subject">The email subject line.</param>
/// <param name="body">The email body content.</param>
/// <exception cref="ArgumentException">
/// Thrown when email address is invalid.
/// </exception>
/// <returns>A task representing the async operation.</returns>
public async Task SendEmailAsync(string to, string subject, string body)
{
    // ...
}
```

**Reference**: `docs/conventions/comments-docstrings.md`

---

## Positive Feedback Templates

### Template: Excellent Test Coverage

✅ **Excellent test coverage!**

I really appreciate the comprehensive test suite you've added:
- Tests cover both success and failure scenarios
- Edge cases are tested (null inputs, boundary values)
- AAA pattern consistently applied
- Good use of mocking with Moq

Test coverage for `CustomerService` is at 95% - fantastic work!

---

### Template: Clean MVP Implementation

✅ **Clean MVP implementation!**

This is a textbook example of the MVP pattern:
- Form has minimal code (< 30 lines)
- All business logic in Presenter
- IView interface properly defined
- DetachView called in Dispose()
- Excellent separation of concerns

Well done! 🎉

---

### Template: Good Error Handling

✅ **Excellent error handling!**

I like how you:
- Catch specific exceptions first
- Log all errors with appropriate levels
- Provide user-friendly error messages
- Don't swallow exceptions

The try-catch structure in `OrderService.cs:45-65` is exemplary!

---

## Quick Response Templates

### Approve with Minor Suggestions

```markdown
✅ **APPROVED**

Great work overall! Code quality is excellent. I have a couple of minor suggestions below, but nothing blocking merge.

**Strengths**:
- Clean architecture
- Good test coverage
- Well-documented

**Optional Improvements**:
- [Minor suggestion 1]
- [Minor suggestion 2]

Feel free to address these in a follow-up PR. Approved! 🚀
```

### Request Changes

```markdown
⚠️ **REQUESTING CHANGES**

Thanks for the PR! I found a few issues that need to be addressed:

**Critical**:
1. [Issue 1 with file:line reference]

**Major**:
1. [Issue 1]
2. [Issue 2]

**Positive**:
- [Something done well]

Please fix the critical items and I'll re-review. Happy to discuss any questions!
```

### Comment (Non-blocking)

```markdown
💬 **COMMENTS**

Code looks good! I have some suggestions that might improve it:

1. [Suggestion 1]
2. [Suggestion 2]

These are non-blocking. Feel free to address now or in a future PR. Your call!
```

---

**Last Updated**: 2025-11-08
**Version**: 1.0
