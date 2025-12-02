# Performance & Optimization

> Part of Production UI Standards

---

## Async Loading Pattern

```csharp
public async Task LoadDataAsync()
{
    ShowLoadingOverlay();
    statusBar.SetBusy("Loading data...");
    dgvData.Enabled = false;

    try
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var data = await _service.GetAllAsync(cts.Token);

        dgvData.DataSource = data;
        statusBar.SetRecordCount(data.Count);
    }
    catch (OperationCanceledException)
    {
        ToastNotification.Show("Operation timed out", ToastType.Warning);
    }
    catch (Exception ex)
    {
        ToastNotification.Show("Failed to load data", ToastType.Error);
        Log.Error(ex, "Failed to load data");
    }
    finally
    {
        HideLoadingOverlay();
        statusBar.SetReady();
        dgvData.Enabled = true;
    }
}
```

---

## Virtual Mode for Large Data

```csharp
public class VirtualDataGridView
{
    private DataGridView dgv;
    private List<Customer> cachedData = new();
    private int pageSize = 100;

    public void EnableVirtualMode(DataGridView grid)
    {
        dgv = grid;
        dgv.VirtualMode = true;
        dgv.CellValueNeeded += Dgv_CellValueNeeded;
    }

    public async Task LoadDataAsync(int totalRows)
    {
        dgv.RowCount = totalRows;
    }

    private void Dgv_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
    {
        int pageIndex = e.RowIndex / pageSize;
        if (!IsPageCached(pageIndex))
        {
            LoadPage(pageIndex);
        }

        var item = cachedData[e.RowIndex % pageSize];
        e.Value = dgv.Columns[e.ColumnIndex].DataPropertyName switch
        {
            "Name" => item.Name,
            "Email" => item.Email,
            _ => null
        };
    }
}
```

---

## Double Buffering

```csharp
public static class ControlExtensions
{
    public static void EnableDoubleBuffering(this Control control)
    {
        typeof(Control).GetProperty("DoubleBuffered",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance)
            ?.SetValue(control, true);
    }
}

// Usage - reduces flickering
dgvCustomers.EnableDoubleBuffering();
panelContent.EnableDoubleBuffering();
```

---

## Suspending Layout

```csharp
// When adding many controls
panel.SuspendLayout();
try
{
    for (int i = 0; i < 100; i++)
    {
        panel.Controls.Add(new Label { Text = $"Item {i}" });
    }
}
finally
{
    panel.ResumeLayout(true);
}
```

---

## Background Operations

```csharp
// Don't block UI thread
// ❌ WRONG
var data = _service.GetAllAsync().Result; // Blocks UI!

// ✅ CORRECT
var data = await _service.GetAllAsync(); // Non-blocking
```

---

## Checklist

- [ ] Async/await for all I/O operations
- [ ] Virtual mode for 1000+ rows
- [ ] Double buffering enabled on grids/panels
- [ ] No .Result or .Wait() calls (blocking)
- [ ] Timeout on long operations
- [ ] SuspendLayout when adding many controls
- [ ] Loading indicators during operations
