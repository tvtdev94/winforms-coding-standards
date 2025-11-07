# Async/Await Pattern for WinForms

> **Quick Reference**: Use async/await for non-blocking UI operations in WinForms applications.

---

## 🎯 Why Async/Await?

✅ **Responsive UI** - UI doesn't freeze during I/O operations
✅ **Better UX** - Users can interact while loading
✅ **Modern C#** - Standard pattern for all I/O operations

---

## 💻 Basic Pattern

```csharp
// ❌ BAD - Synchronous (freezes UI)
private void btnLoad_Click(object sender, EventArgs e)
{
    var customers = _service.GetAllCustomers(); // Blocks UI!
    dgvCustomers.DataSource = customers;
}

// ✅ GOOD - Asynchronous (non-blocking)
private async void btnLoad_Click(object sender, EventArgs e)
{
    lblLoading.Visible = true;
    btnLoad.Enabled = false;

    try
    {
        var customers = await _service.GetAllCustomersAsync();
        dgvCustomers.DataSource = customers;
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}");
    }
    finally
    {
        lblLoading.Visible = false;
        btnLoad.Enabled = true;
    }
}
```

---

## 📋 Complete Example

```csharp
public partial class CustomerForm : Form
{
    private readonly ICustomerService _service;
    private CancellationTokenSource? _cts;

    public CustomerForm(ICustomerService service)
    {
        _service = service;
        InitializeComponent();
    }

    private async void CustomerForm_Load(object sender, EventArgs e)
    {
        await LoadCustomersAsync();
    }

    private async Task LoadCustomersAsync()
    {
        // Create cancellation token
        _cts = new CancellationTokenSource();

        // Show loading UI
        prgLoading.Visible = true;
        prgLoading.Style = ProgressBarStyle.Marquee;
        btnLoad.Enabled = false;

        try
        {
            // Async operation with cancellation support
            var customers = await _service.GetAllCustomersAsync(_cts.Token);

            // Update UI (automatically on UI thread)
            dgvCustomers.DataSource = customers;
            lblStatus.Text = $"Loaded {customers.Count} customers";
        }
        catch (OperationCanceledException)
        {
            lblStatus.Text = "Operation cancelled";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading customers: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            // Hide loading UI
            prgLoading.Visible = false;
            btnLoad.Enabled = true;
            _cts?.Dispose();
            _cts = null;
        }
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        // Cancel ongoing operation
        _cts?.Cancel();
    }
}
```

---

## 🚫 Common Mistakes

### 1. Blocking with .Result or .Wait()
```csharp
// ❌ BAD - Deadlock risk!
private void btnLoad_Click(object sender, EventArgs e)
{
    var result = _service.GetDataAsync().Result; // Deadlock!
}

// ✅ GOOD
private async void btnLoad_Click(object sender, EventArgs e)
{
    var result = await _service.GetDataAsync();
}
```

### 2. Async Void (except event handlers)
```csharp
// ❌ BAD - Can't catch exceptions
private async void LoadData()
{
    await _service.GetDataAsync();
}

// ✅ GOOD - Return Task
private async Task LoadDataAsync()
{
    await _service.GetDataAsync();
}

// ✅ OK - Event handlers can be async void
private async void btnLoad_Click(object sender, EventArgs e)
{
    await LoadDataAsync();
}
```

### 3. Forgetting ConfigureAwait
```csharp
// ⚠️ Library code - use ConfigureAwait(false)
public async Task<List<Customer>> GetAllAsync()
{
    using var client = new HttpClient();
    var response = await client.GetAsync(url).ConfigureAwait(false);
    // ...
}

// ✅ UI code - ConfigureAwait(true) or omit (default)
private async void btnLoad_Click(object sender, EventArgs e)
{
    var customers = await _service.GetAllAsync(); // Continues on UI thread
    dgvCustomers.DataSource = customers; // Safe to update UI
}
```

---

## ✅ Best Practices

### DO:
✅ Use async/await for all I/O operations (DB, file, network)
✅ Show loading indicators during async operations
✅ Disable buttons during operations to prevent double-clicks
✅ Support cancellation with CancellationToken
✅ Handle exceptions properly
✅ Use async suffix for method names: `LoadDataAsync()`

### DON'T:
❌ Don't use .Result or .Wait() (deadlock risk)
❌ Don't use async void (except event handlers)
❌ Don't forget to show loading state
❌ Don't update UI from background threads without Invoke

---

## 🔗 Related Topics

- [Thread Safety](thread-safety.md) - Cross-thread UI updates
- [Error Handling](error-handling.md) - Exception handling patterns

---

**Last Updated**: 2025-11-07
