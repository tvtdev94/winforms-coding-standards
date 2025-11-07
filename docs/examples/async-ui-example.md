# Async/Await UI Example

> **Complete Working Examples**: Keep your WinForms UI responsive with async/await patterns.

See [Async/Await Pattern Guide](../best-practices/async-await.md) for detailed explanation.

---

## 📋 Overview

This guide provides complete, runnable examples of async/await in WinForms applications. Copy and adapt these examples to keep your UI responsive during I/O operations.

---

## 🎯 What You'll Learn

- Async button click handlers
- Progress reporting with `Progress<T>`
- Cancellation support with `CancellationToken`
- Error handling in async code
- Updating UI from background work
- Parallel async operations
- DataGridView async loading

---

## Prerequisites

- **Framework**: .NET Framework 4.5+ or .NET 6+
- **Understanding**: Basic knowledge of async/await
- **Pattern**: Recommended with MVP or MVVM

---

## Example 1: Simple Async Data Loading

### The Problem

❌ **Blocking UI with synchronous code:**

```csharp
// BAD - This freezes the UI during loading
private void btnLoadCustomers_Click(object sender, EventArgs e)
{
    lblStatus.Text = "Loading...";

    // This blocks the UI thread for 3 seconds!
    Thread.Sleep(3000);
    var customers = _service.GetAllCustomers();

    dgvCustomers.DataSource = customers;
    lblStatus.Text = $"Loaded {customers.Count} customers";
}
```

**Result**: UI freezes, "Not Responding" in title bar, terrible UX.

---

### The Solution

✅ **Async/await pattern:**

```csharp
// GOOD - UI remains responsive
private async void btnLoadCustomers_Click(object sender, EventArgs e)
{
    // Disable button to prevent double-clicks
    btnLoadCustomers.Enabled = false;
    lblStatus.Text = "Loading...";
    prgLoading.Visible = true;
    prgLoading.Style = ProgressBarStyle.Marquee;

    try
    {
        // Async operation - UI remains responsive
        var customers = await _service.GetAllCustomersAsync();

        // Update UI (automatically on UI thread)
        dgvCustomers.DataSource = customers;
        lblStatus.Text = $"Loaded {customers.Count} customers";
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error loading customers: {ex.Message}",
            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        lblStatus.Text = "Error loading data";
    }
    finally
    {
        // Re-enable button and hide loading indicator
        btnLoadCustomers.Enabled = true;
        prgLoading.Visible = false;
    }
}
```

---

### Complete Code

```csharp
// CustomerLoadForm.cs
public partial class CustomerLoadForm : Form
{
    private readonly ICustomerService _service;

    public CustomerLoadForm(ICustomerService service)
    {
        _service = service;
        InitializeComponent();
    }

    private async void CustomerLoadForm_Load(object sender, EventArgs e)
    {
        // Load data when form opens
        await LoadCustomersAsync();
    }

    private async void btnLoadCustomers_Click(object sender, EventArgs e)
    {
        await LoadCustomersAsync();
    }

    private async Task LoadCustomersAsync()
    {
        btnLoadCustomers.Enabled = false;
        lblStatus.Text = "Loading customers...";
        prgLoading.Visible = true;
        prgLoading.Style = ProgressBarStyle.Marquee;

        try
        {
            var customers = await _service.GetAllCustomersAsync();
            dgvCustomers.DataSource = customers;
            lblStatus.Text = $"Loaded {customers.Count} customers";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            lblStatus.Text = "Failed to load customers";
        }
        finally
        {
            btnLoadCustomers.Enabled = true;
            prgLoading.Visible = false;
        }
    }
}

// ICustomerService.cs
public interface ICustomerService
{
    Task<List<Customer>> GetAllCustomersAsync();
}

// CustomerService.cs (example implementation)
public class CustomerService : ICustomerService
{
    public async Task<List<Customer>> GetAllCustomersAsync()
    {
        // Simulate network delay
        await Task.Delay(2000);

        // Return sample data
        return new List<Customer>
        {
            new Customer { Id = 1, Name = "John Doe", Email = "john@example.com" },
            new Customer { Id = 2, Name = "Jane Smith", Email = "jane@example.com" }
        };
    }
}
```

---

## Example 2: Progress Reporting

### The Problem

**Long operation with no feedback:**

Users don't know if the operation is progressing or frozen.

---

### The Solution

**Use `Progress<T>` for reporting progress:**

```csharp
// DataImportForm.cs
public partial class DataImportForm : Form
{
    private readonly IImportService _importService;

    public DataImportForm(IImportService importService)
    {
        _importService = importService;
        InitializeComponent();
    }

    private async void btnImport_Click(object sender, EventArgs e)
    {
        btnImport.Enabled = false;
        prgImport.Value = 0;
        prgImport.Visible = true;

        // Create progress reporter
        var progress = new Progress<int>(percentage =>
        {
            // This runs on UI thread automatically
            prgImport.Value = percentage;
            lblProgress.Text = $"Importing... {percentage}%";
        });

        try
        {
            var result = await _importService.ImportDataAsync(progress);

            MessageBox.Show($"Import complete! {result.RecordsImported} records imported.",
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            lblProgress.Text = "Import complete";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Import failed: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            lblProgress.Text = "Import failed";
        }
        finally
        {
            btnImport.Enabled = true;
            prgImport.Visible = false;
        }
    }
}

// IImportService.cs
public interface IImportService
{
    Task<ImportResult> ImportDataAsync(IProgress<int> progress);
}

// ImportService.cs
public class ImportService : IImportService
{
    public async Task<ImportResult> ImportDataAsync(IProgress<int> progress)
    {
        const int totalRecords = 100;
        int imported = 0;

        for (int i = 0; i < totalRecords; i++)
        {
            // Simulate importing a record
            await Task.Delay(50);

            imported++;

            // Report progress (0-100%)
            int percentage = (imported * 100) / totalRecords;
            progress?.Report(percentage);
        }

        return new ImportResult { RecordsImported = imported };
    }
}

public class ImportResult
{
    public int RecordsImported { get; set; }
}
```

---

## Example 3: Cancellation Support

### The Problem

**Can't cancel long operations:**

Users are forced to wait for completion.

---

### The Solution

**Use `CancellationTokenSource`:**

```csharp
// DownloadForm.cs
public partial class DownloadForm : Form
{
    private readonly IDownloadService _downloadService;
    private CancellationTokenSource? _cts;

    public DownloadForm(IDownloadService downloadService)
    {
        _downloadService = downloadService;
        InitializeComponent();
    }

    private async void btnStartDownload_Click(object sender, EventArgs e)
    {
        // Create cancellation token source
        _cts = new CancellationTokenSource();

        btnStartDownload.Enabled = false;
        btnCancelDownload.Enabled = true;
        prgDownload.Value = 0;
        prgDownload.Visible = true;

        var progress = new Progress<int>(percentage =>
        {
            prgDownload.Value = percentage;
            lblStatus.Text = $"Downloading... {percentage}%";
        });

        try
        {
            await _downloadService.DownloadFileAsync(
                "https://example.com/largefile.zip",
                progress,
                _cts.Token);

            lblStatus.Text = "Download complete!";
            MessageBox.Show("Download completed successfully!",
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (OperationCanceledException)
        {
            lblStatus.Text = "Download cancelled by user";
            MessageBox.Show("Download was cancelled.",
                "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Download failed";
            MessageBox.Show($"Download failed: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnStartDownload.Enabled = true;
            btnCancelDownload.Enabled = false;
            prgDownload.Visible = false;
            _cts?.Dispose();
            _cts = null;
        }
    }

    private void btnCancelDownload_Click(object sender, EventArgs e)
    {
        // Request cancellation
        _cts?.Cancel();
        btnCancelDownload.Enabled = false;
        lblStatus.Text = "Cancelling...";
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cts?.Cancel();
            _cts?.Dispose();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}

// IDownloadService.cs
public interface IDownloadService
{
    Task DownloadFileAsync(string url, IProgress<int> progress, CancellationToken cancellationToken);
}

// DownloadService.cs
public class DownloadService : IDownloadService
{
    public async Task DownloadFileAsync(string url, IProgress<int> progress, CancellationToken cancellationToken)
    {
        const int totalChunks = 100;

        for (int i = 0; i < totalChunks; i++)
        {
            // Check for cancellation
            cancellationToken.ThrowIfCancellationRequested();

            // Simulate downloading a chunk
            await Task.Delay(100, cancellationToken);

            // Report progress
            int percentage = ((i + 1) * 100) / totalChunks;
            progress?.Report(percentage);
        }
    }
}
```

---

## Example 4: Multiple Parallel Operations

### The Problem

**Need to load data from multiple sources:**

Loading sequentially takes too long.

---

### The Solution

**Use `Task.WhenAll` for parallel operations:**

```csharp
// DashboardForm.cs
public partial class DashboardForm : Form
{
    private readonly ICustomerService _customerService;
    private readonly IOrderService _orderService;
    private readonly IProductService _productService;

    public DashboardForm(
        ICustomerService customerService,
        IOrderService orderService,
        IProductService productService)
    {
        _customerService = customerService;
        _orderService = orderService;
        _productService = productService;
        InitializeComponent();
    }

    private async void DashboardForm_Load(object sender, EventArgs e)
    {
        await LoadDashboardDataAsync();
    }

    private async Task LoadDashboardDataAsync()
    {
        lblStatus.Text = "Loading dashboard...";
        prgLoading.Visible = true;
        prgLoading.Style = ProgressBarStyle.Marquee;

        try
        {
            // Start all operations in parallel
            var customersTask = _customerService.GetCustomerCountAsync();
            var ordersTask = _orderService.GetTodayOrdersAsync();
            var productsTask = _productService.GetLowStockProductsAsync();

            // Wait for all to complete
            await Task.WhenAll(customersTask, ordersTask, productsTask);

            // Get results (all are already complete)
            int customerCount = await customersTask;
            var orders = await ordersTask;
            var lowStockProducts = await productsTask;

            // Update UI
            lblCustomerCount.Text = customerCount.ToString();
            lblOrderCount.Text = orders.Count.ToString();
            lblLowStockCount.Text = lowStockProducts.Count.ToString();

            dgvRecentOrders.DataSource = orders;
            dgvLowStock.DataSource = lowStockProducts;

            lblStatus.Text = "Dashboard loaded successfully";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading dashboard: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            lblStatus.Text = "Error loading dashboard";
        }
        finally
        {
            prgLoading.Visible = false;
        }
    }
}

// Service interfaces
public interface ICustomerService
{
    Task<int> GetCustomerCountAsync();
}

public interface IOrderService
{
    Task<List<Order>> GetTodayOrdersAsync();
}

public interface IProductService
{
    Task<List<Product>> GetLowStockProductsAsync();
}
```

---

## Example 5: DataGridView with Async Loading

### The Problem

**Loading large dataset freezes UI:**

```csharp
// BAD - Blocks UI
private void btnLoadProducts_Click(object sender, EventArgs e)
{
    var products = _service.GetAllProducts(); // Blocks for 5 seconds!
    dgvProducts.DataSource = products;
}
```

---

### The Solution

**Async loading with pagination:**

```csharp
// ProductListForm.cs
public partial class ProductListForm : Form
{
    private readonly IProductService _productService;
    private int _currentPage = 1;
    private const int PAGE_SIZE = 50;
    private bool _isLoading = false;

    public ProductListForm(IProductService productService)
    {
        _productService = productService;
        InitializeComponent();
    }

    private async void ProductListForm_Load(object sender, EventArgs e)
    {
        await LoadProductsAsync();
    }

    private async void btnNextPage_Click(object sender, EventArgs e)
    {
        _currentPage++;
        await LoadProductsAsync();
    }

    private async void btnPreviousPage_Click(object sender, EventArgs e)
    {
        if (_currentPage > 1)
        {
            _currentPage--;
            await LoadProductsAsync();
        }
    }

    private async Task LoadProductsAsync()
    {
        if (_isLoading) return;

        _isLoading = true;
        btnNextPage.Enabled = false;
        btnPreviousPage.Enabled = false;
        prgLoading.Visible = true;
        prgLoading.Style = ProgressBarStyle.Marquee;

        try
        {
            var result = await _productService.GetProductsPagedAsync(_currentPage, PAGE_SIZE);

            dgvProducts.DataSource = result.Products;
            lblPageInfo.Text = $"Page {_currentPage} of {result.TotalPages} ({result.TotalRecords} total)";

            btnPreviousPage.Enabled = _currentPage > 1;
            btnNextPage.Enabled = _currentPage < result.TotalPages;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading products: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            prgLoading.Visible = false;
            _isLoading = false;
        }
    }
}

// IProductService.cs
public interface IProductService
{
    Task<PagedResult<Product>> GetProductsPagedAsync(int page, int pageSize);
}

// ProductService.cs
public class ProductService : IProductService
{
    public async Task<PagedResult<Product>> GetProductsPagedAsync(int page, int pageSize)
    {
        // Simulate database query
        await Task.Delay(1000);

        // Generate sample data
        var allProducts = Enumerable.Range(1, 500)
            .Select(i => new Product
            {
                Id = i,
                Name = $"Product {i}",
                Price = i * 10.50m,
                Stock = i % 100
            })
            .ToList();

        var totalRecords = allProducts.Count;
        var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

        var products = allProducts
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<Product>
        {
            Products = products,
            CurrentPage = page,
            TotalPages = totalPages,
            TotalRecords = totalRecords
        };
    }
}

public class PagedResult<T>
{
    public List<T> Products { get; set; } = new();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
```

---

## Best Practices Demonstrated

✅ **async void only for event handlers** - All other async methods return `Task`
✅ **ConfigureAwait(false) in library code** - Not shown but recommended for services
✅ **Progress<T> for UI updates** - Thread-safe progress reporting
✅ **CancellationToken for cancellable operations** - Always support cancellation
✅ **Try-catch for async error handling** - Wrap await calls in try-catch
✅ **Disable buttons during operations** - Prevent double-clicks
✅ **Show loading indicators** - Always give visual feedback
✅ **Finally blocks for cleanup** - Ensure UI state is restored

---

## Common Mistakes Avoided

❌ **async void methods** (except event handlers) - Use `Task` instead
❌ **.Result or .Wait() calls** - Causes deadlocks
❌ **Updating UI from background threads** - Use `Progress<T>` or `Invoke`
❌ **Not handling cancellation** - Always support `CancellationToken`
❌ **Not showing progress** - Users need feedback
❌ **Forgetting finally blocks** - UI state gets stuck
❌ **Not disabling controls** - Double-click issues

---

## Testing Async Code

```csharp
// ProductServiceTests.cs
public class ProductServiceTests
{
    [Fact]
    public async Task GetProductsPagedAsync_ReturnsCorrectPage()
    {
        // Arrange
        var service = new ProductService();

        // Act
        var result = await service.GetProductsPagedAsync(page: 2, pageSize: 50);

        // Assert
        Assert.Equal(2, result.CurrentPage);
        Assert.Equal(50, result.Products.Count);
        Assert.Equal(51, result.Products[0].Id); // First item of page 2
    }

    [Fact]
    public async Task DownloadFileAsync_SupportsCancellation()
    {
        // Arrange
        var service = new DownloadService();
        var progress = new Progress<int>();
        var cts = new CancellationTokenSource();

        // Act - cancel after 100ms
        var downloadTask = service.DownloadFileAsync("test", progress, cts.Token);
        await Task.Delay(100);
        cts.Cancel();

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await downloadTask);
    }
}
```

---

## Performance Considerations

### When to use Task.Run

```csharp
// ❌ DON'T use Task.Run for naturally async operations
private async Task<List<Customer>> GetCustomersAsync()
{
    // BAD - EF Core is already async
    return await Task.Run(() => _dbContext.Customers.ToList());
}

// ✅ DO use Task.Run for CPU-bound work
private async Task<byte[]> CompressDataAsync(byte[] data)
{
    // GOOD - Compression is CPU-bound
    return await Task.Run(() => GZip.Compress(data));
}

// ✅ DO use natural async
private async Task<List<Customer>> GetCustomersAsync()
{
    // GOOD - Use EF Core's async methods
    return await _dbContext.Customers.ToListAsync();
}
```

---

## Related Concepts

- [Thread Safety](../best-practices/thread-safety.md) - Cross-thread UI updates
- [Error Handling & Logging](../best-practices/error-handling.md) - Exception patterns
- [Performance Optimization](../best-practices/performance.md) - Async performance
- [MVP Pattern](mvp-example.md) - Integrating async with MVP

---

**Last Updated**: 2025-11-07
