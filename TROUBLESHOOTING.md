# 🔧 Troubleshooting Guide

> Common issues and solutions for WinForms development with these coding standards

**Last Updated**: 2025-11-07

---

## 📋 Table of Contents

- [Setup & Configuration Issues](#-setup--configuration-issues)
- [Dependency Injection Issues](#-dependency-injection-issues)
- [MVP Pattern Issues](#-mvp-pattern-issues)
- [Threading & UI Issues](#-threading--ui-issues)
- [Entity Framework Issues](#-entity-framework-issues)
- [Data Binding Issues](#-data-binding-issues)
- [Performance Issues](#-performance-issues)
- [Testing Issues](#-testing-issues)
- [Build & Deployment Issues](#-build--deployment-issues)
- [Claude Code Specific Issues](#-claude-code-specific-issues)

---

## 🔧 Setup & Configuration Issues

### Issue #1: Application Won't Start - "Entry Point Not Found"

**Error Message**:
```
Application could not be started. Entry point not found.
```

**Root Cause**: Missing or incorrect `[STAThread]` attribute on Main method, or Program.cs not configured correctly.

**Solution**:
```csharp
// ✅ Correct Program.cs structure
internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
```

**Related**: [docs/architecture/project-structure.md](docs/architecture/project-structure.md)

---

### Issue #2: Configuration File Not Found

**Error Message**:
```
Could not find file 'appsettings.json'
```

**Root Cause**: `appsettings.json` not copied to output directory.

**Solution**:

1. **Check .csproj file**:
```xml
<ItemGroup>
  <None Update="appsettings.json">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

2. **Or in Visual Studio**: Right-click `appsettings.json` → Properties → Copy to Output Directory → "Copy if newer"

**Related**: [docs/best-practices/configuration.md](docs/best-practices/configuration.md)

---

### Issue #3: Missing NuGet Packages After Clone

**Error Message**:
```
The type or namespace name 'Microsoft' could not be found
```

**Root Cause**: NuGet packages not restored.

**Solution**:
```bash
# Restore packages
dotnet restore

# Or in Visual Studio
Right-click solution → Restore NuGet Packages
```

---

## 💉 Dependency Injection Issues

### Issue #4: "Unable to Resolve Service for Type"

**Error Message**:
```
System.InvalidOperationException: Unable to resolve service for type 'ICustomerService'
while attempting to activate 'CustomerForm'.
```

**Root Cause**: Service not registered in DI container.

**Solution**:

Check `Program.cs` and ensure service is registered:

```csharp
// ❌ WRONG - Service not registered
var services = new ServiceCollection();
services.AddTransient<CustomerForm>(); // CustomerForm needs ICustomerService

// ✅ CORRECT - Register dependencies first
var services = new ServiceCollection();
services.AddScoped<ICustomerRepository, CustomerRepository>();
services.AddScoped<ICustomerService, CustomerService>();
services.AddTransient<CustomerForm>(); // Now CustomerForm can resolve dependencies
```

**Claude Code Command**: `/setup-di` to regenerate DI configuration

**Related**: [docs/architecture/dependency-injection.md](docs/architecture/dependency-injection.md)

---

### Issue #5: Circular Dependency Detected

**Error Message**:
```
System.InvalidOperationException: A circular dependency was detected
```

**Root Cause**: Services depend on each other in a loop.

**Solution**:

Identify the circular dependency and refactor:

```csharp
// ❌ WRONG - Circular dependency
public class ServiceA
{
    public ServiceA(ServiceB serviceB) { } // A depends on B
}

public class ServiceB
{
    public ServiceB(ServiceA serviceA) { } // B depends on A
}

// ✅ CORRECT - Extract common logic to a third service
public class SharedService
{
    // Common logic here
}

public class ServiceA
{
    public ServiceA(SharedService shared) { }
}

public class ServiceB
{
    public ServiceB(SharedService shared) { }
}
```

**Related**: [docs/architecture/dependency-injection.md](docs/architecture/dependency-injection.md)

---

### Issue #6: "No Service for Type IServiceProvider"

**Error Message**:
```
Cannot resolve 'IServiceProvider' from root provider
```

**Root Cause**: Trying to inject `IServiceProvider` without registering it.

**Solution**:

```csharp
// ✅ Register IServiceProvider explicitly
var services = new ServiceCollection();
var serviceProvider = services.BuildServiceProvider();
services.AddSingleton<IServiceProvider>(serviceProvider);
```

**Better Approach**: Avoid injecting `IServiceProvider` (service locator anti-pattern). Use constructor injection instead.

---

## 🎯 MVP Pattern Issues

### Issue #7: Presenter Not Updating View

**Symptom**: Changes in presenter don't reflect in the UI.

**Root Cause**: View properties not wired to UI controls correctly.

**Solution**:

Check your view implementation:

```csharp
// ❌ WRONG - Property doesn't update control
public string CustomerName { get; set; } // Just a field

// ✅ CORRECT - Property wired to control
public string CustomerName
{
    get => txtCustomerName.Text;
    set => txtCustomerName.Text = value;
}
```

**Claude Code Command**: `/review-code YourForm.cs` to check MVP implementation

**Related**: [docs/architecture/mvp-pattern.md](docs/architecture/mvp-pattern.md)

---

### Issue #8: Events Not Firing in Presenter

**Symptom**: Button clicks don't trigger presenter logic.

**Root Cause**: Events not wired correctly in form constructor.

**Solution**:

```csharp
// ❌ WRONG - Event not wired
public CustomerForm(CustomerPresenter presenter)
{
    InitializeComponent();
    _presenter = presenter;
    // Missing: Wire button click to event
}

// ✅ CORRECT - Events properly wired
public CustomerForm(CustomerPresenter presenter)
{
    InitializeComponent();
    _presenter = presenter;

    // Wire UI events to View events
    btnSave.Click += (s, e) => SaveRequested?.Invoke(this, EventArgs.Empty);
    btnLoad.Click += (s, e) => LoadRequested?.Invoke(this, EventArgs.Empty);
}
```

**Related**: [docs/examples/mvp-example.md](docs/examples/mvp-example.md)

---

### Issue #9: Memory Leak - Presenter Not Disposed

**Symptom**: Application memory grows over time when opening/closing forms.

**Root Cause**: Presenter events not unsubscribed in Form.Dispose().

**Solution**:

```csharp
// ✅ Always detach presenter in Dispose
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        _presenter?.DetachView(); // Unsubscribe events
        components?.Dispose();
    }
    base.Dispose(disposing);
}

// In Presenter
public void DetachView()
{
    if (_view != null)
    {
        _view.LoadRequested -= OnLoadRequested;
        _view.SaveRequested -= OnSaveRequested;
        _view = null;
    }
}
```

**Related**: [docs/best-practices/resource-management.md](docs/best-practices/resource-management.md)

---

## 🧵 Threading & UI Issues

### Issue #10: Cross-Thread Operation Error

**Error Message**:
```
System.InvalidOperationException: Cross-thread operation not valid:
Control 'txtStatus' accessed from a thread other than the thread it was created on.
```

**Root Cause**: Trying to update UI controls from a background thread.

**Solution**:

```csharp
// ❌ WRONG - Direct UI update from background thread
private async Task LoadDataAsync()
{
    var data = await _service.GetDataAsync();
    lblStatus.Text = "Loaded"; // ERROR! Cross-thread access
}

// ✅ CORRECT - Use Invoke or InvokeRequired
private async Task LoadDataAsync()
{
    var data = await _service.GetDataAsync();

    if (lblStatus.InvokeRequired)
    {
        lblStatus.Invoke(new Action(() => lblStatus.Text = "Loaded"));
    }
    else
    {
        lblStatus.Text = "Loaded";
    }
}

// ✅ BETTER - Helper method
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

**Claude Code Command**: `/fix-threading YourForm.cs`

**Related**: [docs/best-practices/thread-safety.md](docs/best-practices/thread-safety.md)

---

### Issue #11: UI Freezes During Long Operations

**Symptom**: Application becomes unresponsive during data loading.

**Root Cause**: Synchronous I/O operations blocking UI thread.

**Solution**:

```csharp
// ❌ WRONG - Synchronous blocking call
private void btnLoad_Click(object sender, EventArgs e)
{
    var data = _service.GetData(); // Blocks UI thread
    dgvCustomers.DataSource = data;
}

// ✅ CORRECT - Async/await pattern
private async void btnLoad_Click(object sender, EventArgs e)
{
    try
    {
        btnLoad.Enabled = false;
        Cursor = Cursors.WaitCursor;

        var data = await _service.GetDataAsync(); // Non-blocking
        dgvCustomers.DataSource = data;
    }
    finally
    {
        btnLoad.Enabled = true;
        Cursor = Cursors.Default;
    }
}
```

**Related**: [docs/best-practices/async-await.md](docs/best-practices/async-await.md)

---

### Issue #12: BackgroundWorker RunWorkerCompleted Not Firing

**Root Cause**: Exception thrown in DoWork without proper handling.

**Solution**:

```csharp
// ✅ Always handle exceptions in BackgroundWorker
private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
{
    try
    {
        // Your work here
        e.Result = ProcessData();
    }
    catch (Exception ex)
    {
        e.Result = ex; // Store exception to handle in RunWorkerCompleted
    }
}

private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
{
    if (e.Result is Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}");
    }
    else
    {
        // Handle success
    }
}
```

**Related**: [docs/best-practices/thread-safety.md](docs/best-practices/thread-safety.md)

---

## 💾 Entity Framework Issues

### Issue #13: "No Database Provider Configured"

**Error Message**:
```
System.InvalidOperationException: No database provider has been configured for this DbContext
```

**Root Cause**: DbContext not registered in DI container or connection string missing.

**Solution**:

1. **Check appsettings.json**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=myapp.db"
  }
}
```

2. **Check Program.cs**:
```csharp
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
```

**Related**: [docs/data-access/entity-framework.md](docs/data-access/entity-framework.md)

---

### Issue #14: Migration Not Applied - "No Such Table"

**Error Message**:
```
Microsoft.Data.Sqlite.SqliteException: SQLite Error 1: 'no such table: Customers'
```

**Root Cause**: Database migrations not created or applied.

**Solution**:

```bash
# 1. Create initial migration
dotnet ef migrations add InitialCreate

# 2. Apply migration to database
dotnet ef database update

# 3. Verify database file exists
ls *.db
```

**Alternative**: Use `Database.EnsureCreated()` for development:
```csharp
public static void InitializeDatabase(AppDbContext context)
{
    context.Database.EnsureCreated(); // Creates DB if not exists
}
```

**Related**: [docs/data-access/entity-framework.md](docs/data-access/entity-framework.md)

---

### Issue #15: "A second operation was started on this context"

**Error Message**:
```
System.InvalidOperationException: A second operation was started on this context
instance before a previous operation completed.
```

**Root Cause**: Multiple async operations using same DbContext instance concurrently.

**Solution**:

```csharp
// ❌ WRONG - Sharing DbContext between async operations
public async Task ProcessAsync()
{
    var task1 = _context.Customers.ToListAsync();
    var task2 = _context.Orders.ToListAsync(); // ERROR! Same context
    await Task.WhenAll(task1, task2);
}

// ✅ CORRECT - Use separate DbContext instances
public async Task ProcessAsync()
{
    using var context1 = new AppDbContext();
    using var context2 = new AppDbContext();

    var task1 = context1.Customers.ToListAsync();
    var task2 = context2.Orders.ToListAsync();
    await Task.WhenAll(task1, task2);
}

// ✅ BETTER - Sequential operations with single context
public async Task ProcessAsync()
{
    var customers = await _context.Customers.ToListAsync();
    var orders = await _context.Orders.ToListAsync();
}
```

**Related**: [docs/data-access/entity-framework.md](docs/data-access/entity-framework.md)

---

## 🔗 Data Binding Issues

### Issue #16: DataGridView Not Updating After Data Change

**Symptom**: DataGridView doesn't reflect changes after modifying data source.

**Root Cause**: Data source doesn't implement `INotifyPropertyChanged` or `BindingList<T>` not used.

**Solution**:

```csharp
// ❌ WRONG - Regular List doesn't notify changes
dgvCustomers.DataSource = new List<Customer>(customers);

// ✅ CORRECT - Use BindingList
dgvCustomers.DataSource = new BindingList<Customer>(customers);

// ✅ BETTER - Use BindingSource for advanced features
var bindingSource = new BindingSource();
bindingSource.DataSource = new BindingList<Customer>(customers);
dgvCustomers.DataSource = bindingSource;

// Now changes are tracked
bindingSource.Add(new Customer { Name = "John" }); // Automatically updates UI
```

**Related**: [docs/ui-ux/data-binding.md](docs/ui-ux/data-binding.md)

---

### Issue #17: BindingSource Current is Null

**Error Message**:
```
System.NullReferenceException: Object reference not set to an instance of an object.
```

**Root Cause**: BindingSource.Current accessed when no row is selected or list is empty.

**Solution**:

```csharp
// ❌ WRONG - No null check
var customer = (Customer)_bindingSource.Current;
customer.Name = "Updated"; // NullReferenceException if no selection

// ✅ CORRECT - Always check for null
if (_bindingSource.Current is Customer customer)
{
    customer.Name = "Updated";
}
else
{
    MessageBox.Show("Please select a customer first.");
}

// ✅ BETTER - Check position
if (_bindingSource.Position >= 0 && _bindingSource.Current != null)
{
    var customer = (Customer)_bindingSource.Current;
    customer.Name = "Updated";
}
```

**Related**: [docs/ui-ux/data-binding.md](docs/ui-ux/data-binding.md)

---

### Issue #18: TextBox Not Updating When Data Changes

**Symptom**: Bound TextBox doesn't show updated values.

**Root Cause**: Data source doesn't implement `INotifyPropertyChanged`.

**Solution**:

Implement `INotifyPropertyChanged` in your model:

```csharp
// ✅ Implement INotifyPropertyChanged
public class Customer : INotifyPropertyChanged
{
    private string _name;

    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// Binding
txtName.DataBindings.Add("Text", _bindingSource, "Name",
    formattingEnabled: true,
    DataSourceUpdateMode.OnPropertyChanged);
```

**Related**: [docs/ui-ux/data-binding.md](docs/ui-ux/data-binding.md)

---

## ⚡ Performance Issues

### Issue #19: DataGridView Slow with Large Dataset

**Symptom**: DataGridView takes 5-10 seconds to load 10,000+ rows.

**Root Cause**: Not using Virtual Mode for large datasets.

**Solution**:

```csharp
// ✅ Enable Virtual Mode for large datasets
dgvCustomers.VirtualMode = true;
dgvCustomers.CellValueNeeded += OnCellValueNeeded;

private List<Customer> _customers;

private void OnCellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
{
    if (e.RowIndex >= 0 && e.RowIndex < _customers.Count)
    {
        var customer = _customers[e.RowIndex];
        e.Value = e.ColumnIndex switch
        {
            0 => customer.Id,
            1 => customer.Name,
            2 => customer.Email,
            _ => null
        };
    }
}

// Set row count
dgvCustomers.RowCount = _customers.Count;
```

**Performance Comparison**:
- Normal mode: 10,000 rows → 8-10 seconds
- Virtual mode: 100,000 rows → < 1 second

**Claude Code Command**: `/optimize-performance`

**Related**: [docs/ui-ux/datagridview-practices.md](docs/ui-ux/datagridview-practices.md)

---

### Issue #20: Application Memory Leak

**Symptom**: Application memory usage grows continuously.

**Root Cause**: Event handlers not unsubscribed or disposable objects not disposed.

**Solution**:

```csharp
// ✅ Always unsubscribe events in Dispose
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        // Unsubscribe events
        _service.DataChanged -= OnDataChanged;
        _timer.Tick -= OnTimerTick;

        // Dispose managed resources
        _timer?.Dispose();
        _backgroundWorker?.Dispose();
        components?.Dispose();
    }
    base.Dispose(disposing);
}
```

**Use Memory Profiler**:
```bash
# Visual Studio: Debug → Performance Profiler → .NET Object Allocation Tracking
# Or use dotMemory, PerfView
```

**Related**: [docs/best-practices/memory-management.md](docs/best-practices/memory-management.md)

---

### Issue #21: Slow Form Load Time

**Symptom**: Form takes 3-5 seconds to appear.

**Root Cause**: Heavy operations in Form constructor or Form_Load.

**Solution**:

```csharp
// ❌ WRONG - Heavy operation in constructor
public CustomerForm()
{
    InitializeComponent();
    LoadData(); // Blocks UI initialization
}

// ✅ CORRECT - Defer heavy operations
public CustomerForm()
{
    InitializeComponent();

    // Defer loading until form is shown
    this.Shown += async (s, e) => await LoadDataAsync();
}

private async Task LoadDataAsync()
{
    // Show loading indicator
    pnlLoading.Visible = true;

    try
    {
        var data = await _service.GetDataAsync();
        dgvCustomers.DataSource = data;
    }
    finally
    {
        pnlLoading.Visible = false;
    }
}
```

**Related**: [docs/best-practices/performance.md](docs/best-practices/performance.md)

---

## 🧪 Testing Issues

### Issue #22: "Object reference not set" in Unit Tests

**Error in Tests**:
```
System.NullReferenceException: Object reference not set to an instance of an object
```

**Root Cause**: Mock not set up correctly or dependency not injected.

**Solution**:

```csharp
// ✅ Properly setup mocks
[Fact]
public async Task LoadCustomers_ShouldLoadData()
{
    // Arrange
    var mockService = new Mock<ICustomerService>();
    var mockView = new Mock<ICustomerListView>();

    // Setup mock to return data
    mockService
        .Setup(s => s.GetAllCustomersAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(new List<Customer> { new Customer { Name = "John" } });

    var presenter = new CustomerListPresenter(mockView.Object, mockService.Object);

    // Act
    mockView.Raise(v => v.LoadRequested += null, EventArgs.Empty);
    await Task.Delay(100); // Allow async to complete

    // Assert
    mockView.VerifySet(v => v.Customers = It.IsAny<List<Customer>>(), Times.Once);
}
```

**Related**: [docs/testing/unit-testing.md](docs/testing/unit-testing.md)

---

### Issue #23: Integration Tests Fail - Database Locked

**Error Message**:
```
Microsoft.Data.Sqlite.SqliteException: SQLite Error 5: 'database is locked'
```

**Root Cause**: Previous test didn't dispose DbContext properly.

**Solution**:

```csharp
// ✅ Properly dispose DbContext in tests
public class CustomerRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly CustomerRepository _repository;

    public CustomerRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new AppDbContext(options);
        _context.Database.OpenConnection(); // Keep connection open
        _context.Database.EnsureCreated();

        _repository = new CustomerRepository(_context);
    }

    public void Dispose()
    {
        _context?.Database.CloseConnection();
        _context?.Dispose();
    }
}
```

**Related**: [docs/testing/integration-testing.md](docs/testing/integration-testing.md)

---

### Issue #24: Tests Pass Locally But Fail in CI/CD

**Root Cause**: Tests depend on specific environment or timing.

**Solution**:

```csharp
// ❌ WRONG - Time-dependent test
[Fact]
public async Task Test_ShouldComplete()
{
    await Task.Delay(100); // Might fail on slow CI servers
    Assert.True(completed);
}

// ✅ CORRECT - Use proper synchronization
[Fact]
public async Task Test_ShouldComplete()
{
    var tcs = new TaskCompletionSource<bool>();
    _service.Completed += (s, e) => tcs.SetResult(true);

    await _service.StartAsync();

    var completed = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(5));
    Assert.True(completed);
}
```

**Related**: [docs/testing/unit-testing.md](docs/testing/unit-testing.md)

---

## 🏗️ Build & Deployment Issues

### Issue #25: Build Succeeds But App Won't Run

**Symptom**: Build completes without errors, but executable crashes or doesn't start.

**Root Cause**: Missing runtime dependencies or config files.

**Solution**:

Check publish settings:
```bash
# Publish as self-contained
dotnet publish -c Release -r win-x64 --self-contained true

# Verify output directory has all required files:
# - YourApp.exe
# - appsettings.json
# - All .dll dependencies
# - runtimes/ folder (if using SQLite)
```

**Related**: [docs/deployment/packaging.md](docs/deployment/packaging.md) (if exists)

---

### Issue #26: "Could not load file or assembly"

**Error Message**:
```
System.IO.FileNotFoundException: Could not load file or assembly 'System.Text.Json'
```

**Root Cause**: Version mismatch or missing binding redirect.

**Solution**:

1. **Update all packages**:
```bash
dotnet list package --outdated
dotnet add package System.Text.Json
```

2. **Or add binding redirect** (for .NET Framework):
```xml
<runtime>
  <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
    <dependentAssembly>
      <assemblyIdentity name="System.Text.Json" />
      <bindingRedirect oldVersion="0.0.0.0-8.0.0.0" newVersion="8.0.0.0" />
    </dependentAssembly>
  </assemblyBinding>
</runtime>
```

---

## 🤖 Claude Code Specific Issues

### Issue #27: Claude Code Generates Code Not Following Standards

**Symptom**: Generated code uses patterns not in your documentation.

**Root Cause**: CLAUDE.md not loaded or context not provided.

**Solution**:

1. **Verify CLAUDE.md exists in repository root**
2. **Explicitly reference standards**:
```
User: "Create a CustomerForm following the MVP pattern in docs/architecture/mvp-pattern.md"
```

3. **Use slash commands**:
```
/create-form Customer
```

4. **Provide context**:
```
Read templates/form-template.cs then create CustomerForm
```

---

### Issue #28: Claude Code Suggests Sync Instead of Async

**Symptom**: Generated code uses synchronous I/O operations.

**Solution**:

Explicitly request async:
```
User: "Create a service with ASYNC methods following docs/best-practices/async-await.md"
```

Or use templates:
```
User: "Use templates/service-template.cs as base to create CustomerService"
```

**Related**: [CLAUDE.md](CLAUDE.md) - See DO/DON'T rules

---

### Issue #29: Generated Code Missing XML Documentation

**Symptom**: Claude Code creates classes without XML comments.

**Solution**:

Remind about documentation standards:
```
User: "Add XML documentation comments following docs/conventions/comments-docstrings.md"
```

Or use command:
```
/review-code CustomerService.cs
```

This will check for missing documentation and suggest additions.

---

### Issue #30: Claude Code Doesn't Use DI Container

**Symptom**: Generated code creates dependencies with `new` keyword.

**Solution**:

Use the DI setup command:
```
/setup-di
```

Or explicitly request:
```
User: "Create CustomerService using constructor injection following docs/architecture/dependency-injection.md"
```

**Related**: [docs/examples/di-example.md](docs/examples/di-example.md)

---

## 📞 Getting More Help

### Still Stuck?

1. **Check Documentation**:
   - [Full Documentation Index](docs/00-overview.md)
   - [Usage Guide with Examples](USAGE_GUIDE.md)
   - [Example Project](example-project/)

2. **Use Claude Code Commands**:
   ```
   /review-code YourFile.cs    # Check for issues
   /check-standards YourFile.cs # Validate against standards
   ```

3. **Search Issues**:
   - Check if similar issue reported in GitHub Issues
   - Search in USAGE_GUIDE.md for scenario examples

4. **Ask for Help**:
   - Open a GitHub Issue with:
     - Error message
     - What you tried
     - Minimal code to reproduce

### Contributing

Found a common issue not listed here? Please contribute!

1. Open an issue describing the problem and solution
2. Or submit a PR updating this TROUBLESHOOTING.md

---

**Last Updated**: 2025-11-07
**Version**: 1.0
**Feedback**: Please report issues or suggest improvements via GitHub Issues
