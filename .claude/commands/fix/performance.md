---
description: Optimize WinForms application performance
---

You are tasked with optimizing the performance of a WinForms application.

## Workflow

1. **Ask the user**:
   - What performance issues are you experiencing?
   - Which forms/controls are slow?
   - What operations take too long?

2. **Analyze the code** for common performance issues

3. **Apply Optimizations**:

### Optimization 1: Suspend/Resume Layout

❌ **Slow - Layout recalculated on every change**:
```csharp
private void LoadData()
{
    foreach (var item in items)
    {
        var label = new Label { Text = item.Name };
        var textBox = new TextBox { Text = item.Value };
        panel.Controls.Add(label);
        panel.Controls.Add(textBox);
        // Layout recalculated twice per iteration!
    }
}
```

✅ **Fast - Layout suspended during bulk operations**:
```csharp
private void LoadData()
{
    panel.SuspendLayout();
    try
    {
        foreach (var item in items)
        {
            var label = new Label { Text = item.Name };
            var textBox = new TextBox { Text = item.Value };
            panel.Controls.Add(label);
            panel.Controls.Add(textBox);
        }
    }
    finally
    {
        panel.ResumeLayout();
    }
}
```

### Optimization 2: DataGridView Performance

❌ **Slow - Real-time updates**:
```csharp
private void LoadDataGridView()
{
    dgv.DataSource = null;
    foreach (var item in largeDataSet)
    {
        dgv.Rows.Add(item.Col1, item.Col2, item.Col3);
        // Redraws after each row!
    }
}
```

✅ **Fast - Batch updates**:
```csharp
private void LoadDataGridView()
{
    dgv.SuspendLayout();
    try
    {
        // Use data binding instead of adding rows manually
        var bindingList = new BindingList<DataItem>(largeDataSet);
        dgv.DataSource = bindingList;

        // Or use BeginUpdate/EndUpdate for manual rows
        // dgv.BeginUpdate();
        // ... add rows ...
        // dgv.EndUpdate();
    }
    finally
    {
        dgv.ResumeLayout();
    }
}
```

### Optimization 3: Virtual Mode for Large Datasets

✅ **Virtual Mode - Load data on demand**:
```csharp
private List<DataItem> _data;
private const int PAGE_SIZE = 100;

private void InitializeDataGridView()
{
    dgv.VirtualMode = true;
    dgv.RowCount = _data.Count;
    dgv.CellValueNeeded += Dgv_CellValueNeeded;
}

private void Dgv_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
{
    // Only load visible data
    if (e.RowIndex < _data.Count)
    {
        var item = _data[e.RowIndex];
        switch (dgv.Columns[e.ColumnIndex].Name)
        {
            case "colName":
                e.Value = item.Name;
                break;
            case "colValue":
                e.Value = item.Value;
                break;
        }
    }
}
```

### Optimization 4: Control Caching

❌ **Slow - Creating controls repeatedly**:
```csharp
private void UpdateDisplay(DataItem item)
{
    panel.Controls.Clear();
    panel.Controls.Add(new Label { Text = item.Name });
    panel.Controls.Add(new TextBox { Text = item.Value });
    // Creates new controls every time!
}
```

✅ **Fast - Reuse existing controls**:
```csharp
private Label _lblName;
private TextBox _txtValue;

private void InitializeControls()
{
    _lblName = new Label();
    _txtValue = new TextBox();
    panel.Controls.Add(_lblName);
    panel.Controls.Add(_txtValue);
}

private void UpdateDisplay(DataItem item)
{
    _lblName.Text = item.Name;
    _txtValue.Text = item.Value;
    // Just update text - much faster!
}
```

### Optimization 5: Image Handling

❌ **Slow - Loading images every time**:
```csharp
private void DisplayImage(string imagePath)
{
    pictureBox.Image = Image.FromFile(imagePath);
    // Loads from disk every time!
}
```

✅ **Fast - Image caching**:
```csharp
private Dictionary<string, Image> _imageCache = new Dictionary<string, Image>();

private void DisplayImage(string imagePath)
{
    if (!_imageCache.ContainsKey(imagePath))
    {
        _imageCache[imagePath] = Image.FromFile(imagePath);
    }
    pictureBox.Image = _imageCache[imagePath];
}

// Don't forget to dispose cached images
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        foreach (var image in _imageCache.Values)
        {
            image?.Dispose();
        }
        _imageCache.Clear();
    }
    base.Dispose(disposing);
}
```

### Optimization 6: Reduce Repaints

❌ **Slow - Multiple repaints**:
```csharp
private void UpdateUI()
{
    lblStatus.Text = "Processing...";
    lblStatus.Refresh(); // Forces repaint
    progressBar.Value = 50;
    progressBar.Refresh(); // Forces repaint
    lblResult.Text = "Done";
    lblResult.Refresh(); // Forces repaint
}
```

✅ **Fast - Batch UI updates**:
```csharp
private void UpdateUI()
{
    // Update all properties first
    lblStatus.Text = "Processing...";
    progressBar.Value = 50;
    lblResult.Text = "Done";

    // Single repaint at the end
    // Or just let WinForms handle it automatically
}
```

### Optimization 7: Asynchronous Loading

❌ **Slow - Synchronous loading blocks UI**:
```csharp
private void Form_Load(object sender, EventArgs e)
{
    // UI freezes during load
    var data = LoadDataFromDatabase(); // Takes 5 seconds
    DisplayData(data);
}
```

✅ **Fast - Async loading keeps UI responsive**:
```csharp
private async void Form_Load(object sender, EventArgs e)
{
    try
    {
        ShowLoadingIndicator(true);

        // Load in background - UI stays responsive
        var data = await LoadDataFromDatabaseAsync();
        DisplayData(data);
    }
    catch (Exception ex)
    {
        ShowError(ex.Message);
    }
    finally
    {
        ShowLoadingIndicator(false);
    }
}

private async Task<List<DataItem>> LoadDataFromDatabaseAsync()
{
    return await Task.Run(() => LoadDataFromDatabase());
}
```

### Optimization 8: Lazy Loading

✅ **Load data only when needed**:
```csharp
private List<Customer> _customers;
private bool _customersLoaded = false;

private async Task<List<Customer>> GetCustomersAsync()
{
    if (!_customersLoaded)
    {
        _customers = await _service.LoadCustomersAsync();
        _customersLoaded = true;
    }
    return _customers;
}

private async void tabCustomers_Enter(object sender, EventArgs e)
{
    // Only load when tab is opened
    if (!_customersLoaded)
    {
        var customers = await GetCustomersAsync();
        dgvCustomers.DataSource = new BindingList<Customer>(customers);
    }
}
```

### Optimization 9: Use StringBuilder for Text

❌ **Slow - String concatenation**:
```csharp
private string BuildReport()
{
    string report = "";
    foreach (var item in items)
    {
        report += item.Name + ": " + item.Value + "\n";
        // Creates new string every iteration!
    }
    return report;
}
```

✅ **Fast - StringBuilder**:
```csharp
private string BuildReport()
{
    var sb = new StringBuilder();
    foreach (var item in items)
    {
        sb.AppendLine($"{item.Name}: {item.Value}");
    }
    return sb.ToString();
}
```

### Optimization 10: Double Buffering

✅ **Enable double buffering to reduce flicker**:
```csharp
public MyForm()
{
    InitializeComponent();

    // Enable double buffering for smoother rendering
    this.DoubleBuffered = true;

    // Or for specific controls
    typeof(DataGridView).InvokeMember("DoubleBuffered",
        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
        null, dgv, new object[] { true });
}
```

4. **Performance Checklist**:
   - [ ] Use SuspendLayout/ResumeLayout for bulk control operations
   - [ ] Use data binding instead of manual row addition
   - [ ] Enable virtual mode for large DataGridView datasets
   - [ ] Cache controls instead of creating new ones
   - [ ] Cache images and resources
   - [ ] Avoid unnecessary Refresh() calls
   - [ ] Use async/await for I/O operations
   - [ ] Implement lazy loading for tabs/panels
   - [ ] Use StringBuilder for string concatenation
   - [ ] Enable double buffering to reduce flicker
   - [ ] Profile with performance tools to find bottlenecks

5. **Profiling Tools**:
   - Visual Studio Profiler
   - dotMemory (JetBrains)
   - PerfView
   - ANTS Performance Profiler

6. **Show the user**:
   - Optimized code with explanations
   - Before/After comparisons
   - Expected performance improvements
   - Additional optimization suggestions
   - Profiling recommendations if needed
