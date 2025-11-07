# DataGridView Best Practices

## 📋 Overview

DataGridView is the most powerful and complex control in WinForms, designed for displaying and editing tabular data. It provides rich features for data presentation, editing, sorting, and customization, but requires careful configuration to achieve optimal performance and user experience.

**What is DataGridView?**
- Advanced grid control for displaying collections of data
- Supports in-place editing, sorting, filtering, and formatting
- Highly customizable with events for nearly every user interaction
- Can handle both bound and unbound data scenarios
- Supports virtual mode for handling millions of rows efficiently

**Key Capabilities:**
- Data binding to lists, DataTables, and BindingSources
- Multiple column types (Text, Combo, CheckBox, Button, Image, Link)
- Cell-level formatting and validation
- Custom cell rendering and painting
- Selection modes (cell, row, column)
- Built-in sorting and custom sorting
- Context menus and row actions

---

## 🎯 Why This Matters

### Common Issues with DataGridView

**Performance Problems:**
- Slow rendering with large datasets (10,000+ rows)
- UI freezing during data loads
- Excessive memory consumption
- Sluggish scrolling and selection

**Usability Issues:**
- Poor column layout (too wide/narrow)
- Unclear data formatting (dates, currency, null values)
- Confusing selection behavior
- Missing visual feedback
- Difficult editing experience

**Code Maintainability:**
- Hard-coded column configuration
- Scattered formatting logic
- Duplicate sorting/filtering code
- Difficult to test

This guide addresses all these issues with proven patterns and best practices.

---

## 🔧 Basic Setup

### Data Binding

**Using BindingSource (Recommended):**

```csharp
public partial class CustomerGridForm : Form
{
    private BindingList<Customer> _customers;
    private BindingSource _bindingSource;

    public CustomerGridForm()
    {
        InitializeComponent();
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        // Load data
        _customers = new BindingList<Customer>(LoadCustomers());

        // Create BindingSource
        _bindingSource = new BindingSource
        {
            DataSource = _customers
        };

        // Bind to DataGridView
        dgvCustomers.DataSource = _bindingSource;
    }

    private List<Customer> LoadCustomers()
    {
        return new List<Customer>
        {
            new Customer { Id = 1, Name = "John Doe", Email = "john@example.com", IsActive = true },
            new Customer { Id = 2, Name = "Jane Smith", Email = "jane@example.com", IsActive = false }
        };
    }
}
```

**BindingList vs List:**

```csharp
// ❌ BAD - List<T> doesn't notify UI of changes
var customers = new List<Customer>();
dgvCustomers.DataSource = customers;
customers.Add(new Customer()); // Grid doesn't update!
dgvCustomers.Refresh(); // Manual refresh required

// ✅ GOOD - BindingList<T> automatically notifies UI
var customers = new BindingList<Customer>();
dgvCustomers.DataSource = customers;
customers.Add(new Customer()); // Grid updates automatically!
```

**Async Data Loading:**

```csharp
private async void CustomerGridForm_Load(object sender, EventArgs e)
{
    dgvCustomers.Enabled = false;
    lblStatus.Text = "Loading...";

    try
    {
        var customers = await _customerService.GetAllAsync();
        _customers = new BindingList<Customer>(customers);
        _bindingSource.DataSource = _customers;
        lblStatus.Text = $"Loaded {customers.Count} customers";
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error loading data: {ex.Message}", "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    finally
    {
        dgvCustomers.Enabled = true;
    }
}
```

---

### Column Configuration

**AutoGenerateColumns = false (Recommended):**

```csharp
private void ConfigureColumns()
{
    dgvCustomers.AutoGenerateColumns = false;
    dgvCustomers.Columns.Clear();

    // Text column
    dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
    {
        Name = "colName",
        DataPropertyName = nameof(Customer.Name),
        HeaderText = "Customer Name",
        Width = 200,
        MinimumWidth = 100
    });

    // Email column
    dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
    {
        Name = "colEmail",
        DataPropertyName = nameof(Customer.Email),
        HeaderText = "Email Address",
        Width = 250
    });

    // CheckBox column
    dgvCustomers.Columns.Add(new DataGridViewCheckBoxColumn
    {
        Name = "colActive",
        DataPropertyName = nameof(Customer.IsActive),
        HeaderText = "Active",
        Width = 60,
        TrueValue = true,
        FalseValue = false
    });

    // Read-only calculated column
    dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
    {
        Name = "colAge",
        DataPropertyName = nameof(Customer.Age),
        HeaderText = "Age",
        Width = 60,
        ReadOnly = true,
        DefaultCellStyle = new DataGridViewCellStyle
        {
            Alignment = DataGridViewContentAlignment.MiddleCenter
        }
    });
}
```

**Column Types:**

```csharp
private void AddVariousColumnTypes()
{
    // TextBox (default)
    dgvGrid.Columns.Add(new DataGridViewTextBoxColumn
    {
        DataPropertyName = "Name",
        HeaderText = "Name"
    });

    // ComboBox
    var statusColumn = new DataGridViewComboBoxColumn
    {
        DataPropertyName = "Status",
        HeaderText = "Status",
        DataSource = new[] { "Active", "Inactive", "Pending" }
    };
    dgvGrid.Columns.Add(statusColumn);

    // CheckBox
    dgvGrid.Columns.Add(new DataGridViewCheckBoxColumn
    {
        DataPropertyName = "IsEnabled",
        HeaderText = "Enabled"
    });

    // Button
    dgvGrid.Columns.Add(new DataGridViewButtonColumn
    {
        Name = "colAction",
        HeaderText = "Action",
        Text = "Edit",
        UseColumnTextForButtonValue = true
    });

    // Image
    dgvGrid.Columns.Add(new DataGridViewImageColumn
    {
        DataPropertyName = "Photo",
        HeaderText = "Photo",
        ImageLayout = DataGridViewImageCellLayout.Zoom
    });

    // Link
    dgvGrid.Columns.Add(new DataGridViewLinkColumn
    {
        DataPropertyName = "Website",
        HeaderText = "Website"
    });
}
```

**Column Width Strategies:**

```csharp
private void ConfigureColumnWidths()
{
    // Fixed width
    dgvGrid.Columns["colId"].Width = 60;

    // Fill remaining space
    dgvGrid.Columns["colDescription"].AutoSizeMode =
        DataGridViewAutoSizeColumnMode.Fill;

    // Auto-size based on content (use sparingly - performance impact)
    dgvGrid.Columns["colName"].AutoSizeMode =
        DataGridViewAutoSizeColumnMode.AllCells;

    // Percentage-based width (manual calculation)
    var totalWidth = dgvGrid.ClientSize.Width - dgvGrid.RowHeadersWidth;
    dgvGrid.Columns["colName"].Width = (int)(totalWidth * 0.3);    // 30%
    dgvGrid.Columns["colEmail"].Width = (int)(totalWidth * 0.4);   // 40%
    dgvGrid.Columns["colPhone"].Width = (int)(totalWidth * 0.3);   // 30%
}
```

---

### Cell Formatting

**CellFormatting Event:**

```csharp
private void dgvCustomers_CellFormatting(object sender,
    DataGridViewCellFormattingEventArgs e)
{
    // Format currency
    if (dgvCustomers.Columns[e.ColumnIndex].Name == "colPrice" && e.Value != null)
    {
        e.Value = ((decimal)e.Value).ToString("C2");
        e.FormattingApplied = true;
    }

    // Format dates
    if (dgvCustomers.Columns[e.ColumnIndex].Name == "colDate" && e.Value != null)
    {
        e.Value = ((DateTime)e.Value).ToString("yyyy-MM-dd");
        e.FormattingApplied = true;
    }

    // Handle null values
    if (e.Value == null || e.Value == DBNull.Value)
    {
        e.Value = "N/A";
        e.CellStyle.ForeColor = Color.Gray;
        e.FormattingApplied = true;
    }
}
```

**Conditional Formatting:**

```csharp
private void dgvOrders_CellFormatting(object sender,
    DataGridViewCellFormattingEventArgs e)
{
    if (e.RowIndex < 0) return;

    var row = dgvOrders.Rows[e.RowIndex];
    if (row.DataBoundItem is Order order)
    {
        // Color code by status
        if (dgvOrders.Columns[e.ColumnIndex].Name == "colStatus")
        {
            switch (order.Status)
            {
                case OrderStatus.Pending:
                    e.CellStyle.BackColor = Color.LightYellow;
                    e.CellStyle.ForeColor = Color.DarkOrange;
                    break;
                case OrderStatus.Shipped:
                    e.CellStyle.BackColor = Color.LightGreen;
                    e.CellStyle.ForeColor = Color.DarkGreen;
                    break;
                case OrderStatus.Cancelled:
                    e.CellStyle.BackColor = Color.LightCoral;
                    e.CellStyle.ForeColor = Color.DarkRed;
                    break;
            }
        }

        // Highlight overdue items
        if (dgvOrders.Columns[e.ColumnIndex].Name == "colDueDate")
        {
            if (order.DueDate < DateTime.Today)
            {
                e.CellStyle.BackColor = Color.Red;
                e.CellStyle.ForeColor = Color.White;
                e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
            }
        }
    }
}
```

---

## ⚡ Performance Optimization

### Virtual Mode

**When to Use Virtual Mode:**
- Datasets with 10,000+ rows
- Data loaded from database on-demand
- Memory-constrained environments
- Need for ultra-fast scrolling

**Complete Virtual Mode Example:**

```csharp
public partial class VirtualGridForm : Form
{
    private const int CACHE_SIZE = 100;
    private const int TOTAL_ROWS = 1000000;

    private int _cacheStartIndex = 0;
    private List<DataRow> _cache = new List<DataRow>();

    public VirtualGridForm()
    {
        InitializeComponent();
        SetupVirtualMode();
    }

    private void SetupVirtualMode()
    {
        // Enable virtual mode
        dgvData.VirtualMode = true;
        dgvData.RowCount = TOTAL_ROWS;

        // Configure columns
        dgvData.Columns.Add("colId", "ID");
        dgvData.Columns.Add("colName", "Name");
        dgvData.Columns.Add("colValue", "Value");

        // Wire up events
        dgvData.CellValueNeeded += DgvData_CellValueNeeded;
        dgvData.CellValuePushed += DgvData_CellValuePushed;
    }

    private void DgvData_CellValueNeeded(object sender,
        DataGridViewCellValueEventArgs e)
    {
        // Check if row is in cache
        if (e.RowIndex >= _cacheStartIndex &&
            e.RowIndex < _cacheStartIndex + _cache.Count)
        {
            var cacheIndex = e.RowIndex - _cacheStartIndex;
            e.Value = _cache[cacheIndex].GetValue(e.ColumnIndex);
        }
        else
        {
            // Load cache for this region
            LoadCache(e.RowIndex);
            e.Value = _cache[0].GetValue(e.ColumnIndex);
        }
    }

    private void DgvData_CellValuePushed(object sender,
        DataGridViewCellValueEventArgs e)
    {
        // Handle cell editing
        if (e.RowIndex >= _cacheStartIndex &&
            e.RowIndex < _cacheStartIndex + _cache.Count)
        {
            var cacheIndex = e.RowIndex - _cacheStartIndex;
            _cache[cacheIndex].SetValue(e.ColumnIndex, e.Value);

            // Optionally save to database
            SaveToDatabase(e.RowIndex, e.ColumnIndex, e.Value);
        }
    }

    private void LoadCache(int rowIndex)
    {
        // Calculate cache start position
        _cacheStartIndex = Math.Max(0, rowIndex - (CACHE_SIZE / 2));

        // Load data from database (simulated)
        _cache = LoadDataFromDatabase(_cacheStartIndex, CACHE_SIZE);
    }

    private List<DataRow> LoadDataFromDatabase(int startIndex, int count)
    {
        // Simulate database query
        var data = new List<DataRow>();
        for (int i = 0; i < count && startIndex + i < TOTAL_ROWS; i++)
        {
            data.Add(new DataRow
            {
                Id = startIndex + i,
                Name = $"Item {startIndex + i}",
                Value = (startIndex + i) * 10
            });
        }
        return data;
    }

    private void SaveToDatabase(int rowIndex, int columnIndex, object value)
    {
        // Save to database (implement as needed)
    }

    private class DataRow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }

        public object GetValue(int columnIndex)
        {
            return columnIndex switch
            {
                0 => Id,
                1 => Name,
                2 => Value,
                _ => null
            };
        }

        public void SetValue(int columnIndex, object value)
        {
            switch (columnIndex)
            {
                case 0: Id = Convert.ToInt32(value); break;
                case 1: Name = value?.ToString(); break;
                case 2: Value = Convert.ToInt32(value); break;
            }
        }
    }
}
```

---

### Reduce Repaints

**Suspend Layout During Updates:**

```csharp
private void BulkUpdateGrid()
{
    dgvCustomers.SuspendLayout();

    try
    {
        // Disable painting
        dgvCustomers.BeginUpdate(); // If available in your version

        // Bulk operations
        for (int i = 0; i < 1000; i++)
        {
            _customers.Add(new Customer { Name = $"Customer {i}" });
        }
    }
    finally
    {
        dgvCustomers.EndUpdate();
        dgvCustomers.ResumeLayout();
    }
}

// Alternative using BindingSource
private void BulkUpdateViaBindingSource()
{
    _bindingSource.RaiseListChangedEvents = false;

    try
    {
        for (int i = 0; i < 1000; i++)
        {
            _customers.Add(new Customer { Name = $"Customer {i}" });
        }
    }
    finally
    {
        _bindingSource.RaiseListChangedEvents = true;
        _bindingSource.ResetBindings(false);
    }
}
```

**Avoid Unnecessary Refreshes:**

```csharp
// ❌ BAD - Causes full grid repaint
private void UpdateCell_Bad()
{
    dgvCustomers.Rows[0].Cells[0].Value = "New Value";
    dgvCustomers.Refresh(); // Unnecessary!
}

// ✅ GOOD - Cell updates automatically
private void UpdateCell_Good()
{
    dgvCustomers.Rows[0].Cells[0].Value = "New Value";
    // No Refresh() needed!
}

// ✅ GOOD - Update data object (with data binding)
private void UpdateData_Good()
{
    _customers[0].Name = "New Name"; // Grid updates automatically
}
```

---

### Column AutoSizing

**Performance Considerations:**

```csharp
// ❌ BAD - Recalculates on every data change (slow)
dgvCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

// ✅ GOOD - Calculate once after data load
private void AutoSizeColumnsOnce()
{
    dgvCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

    // Load data first
    dgvCustomers.DataSource = _customers;

    // Then auto-size once
    dgvCustomers.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
}

// ✅ BETTER - Use fixed widths or Fill mode
private void UseFixedWidths()
{
    dgvCustomers.Columns["colId"].Width = 60;
    dgvCustomers.Columns["colName"].Width = 200;
    dgvCustomers.Columns["colEmail"].AutoSizeMode =
        DataGridViewAutoSizeColumnMode.Fill;
}
```

---

## 🎨 Advanced Features

### Custom Cell Rendering

**CellPainting Event:**

```csharp
private void dgvCustomers_CellPainting(object sender,
    DataGridViewCellPaintingEventArgs e)
{
    // Skip header cells
    if (e.RowIndex < 0) return;

    // Custom progress bar in cell
    if (dgvCustomers.Columns[e.ColumnIndex].Name == "colProgress")
    {
        e.Handled = true;

        // Paint background
        e.PaintBackground(e.CellBounds, true);

        // Get progress value (0-100)
        var progress = e.Value != null ? Convert.ToInt32(e.Value) : 0;

        // Calculate progress bar rectangle
        var progressWidth = (int)(e.CellBounds.Width * (progress / 100.0));
        var progressRect = new Rectangle(
            e.CellBounds.X + 2,
            e.CellBounds.Y + 2,
            progressWidth - 4,
            e.CellBounds.Height - 4
        );

        // Draw progress bar
        e.Graphics.FillRectangle(Brushes.Green, progressRect);

        // Draw border
        e.Graphics.DrawRectangle(Pens.Black,
            e.CellBounds.X + 1,
            e.CellBounds.Y + 1,
            e.CellBounds.Width - 2,
            e.CellBounds.Height - 2
        );

        // Draw text
        var text = $"{progress}%";
        var textSize = e.Graphics.MeasureString(text, e.CellStyle.Font);
        var textPoint = new PointF(
            e.CellBounds.X + (e.CellBounds.Width - textSize.Width) / 2,
            e.CellBounds.Y + (e.CellBounds.Height - textSize.Height) / 2
        );
        e.Graphics.DrawString(text, e.CellStyle.Font, Brushes.Black, textPoint);
    }
}
```

**Drawing Icons in Cells:**

```csharp
private void dgvOrders_CellPainting(object sender,
    DataGridViewCellPaintingEventArgs e)
{
    if (e.RowIndex < 0) return;

    if (dgvOrders.Columns[e.ColumnIndex].Name == "colStatus")
    {
        e.Handled = true;
        e.PaintBackground(e.CellBounds, true);

        var row = dgvOrders.Rows[e.RowIndex];
        if (row.DataBoundItem is Order order)
        {
            // Select icon based on status
            Image icon = order.Status switch
            {
                OrderStatus.Pending => Properties.Resources.IconClock,
                OrderStatus.Shipped => Properties.Resources.IconCheck,
                OrderStatus.Cancelled => Properties.Resources.IconX,
                _ => null
            };

            // Draw icon
            if (icon != null)
            {
                var iconRect = new Rectangle(
                    e.CellBounds.X + 5,
                    e.CellBounds.Y + (e.CellBounds.Height - 16) / 2,
                    16, 16
                );
                e.Graphics.DrawImage(icon, iconRect);
            }

            // Draw text next to icon
            var text = order.Status.ToString();
            var textRect = new Rectangle(
                e.CellBounds.X + 25,
                e.CellBounds.Y,
                e.CellBounds.Width - 25,
                e.CellBounds.Height
            );
            TextRenderer.DrawText(e.Graphics, text, e.CellStyle.Font,
                textRect, e.CellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
        }
    }
}
```

---

### Cell Editing

**EditMode Options:**

```csharp
private void ConfigureEditMode()
{
    // Edit on keystroke (default)
    dgvCustomers.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;

    // Edit on cell enter (immediate)
    dgvCustomers.EditMode = DataGridViewEditMode.EditOnEnter;

    // Edit only on F2 or double-click
    dgvCustomers.EditMode = DataGridViewEditMode.EditOnF2;

    // Programmatic edit only
    dgvCustomers.EditMode = DataGridViewEditMode.EditProgrammatically;
}
```

**Cell Validation:**

```csharp
private void dgvCustomers_CellValidating(object sender,
    DataGridViewCellValidatingEventArgs e)
{
    var columnName = dgvCustomers.Columns[e.ColumnIndex].Name;

    // Validate email
    if (columnName == "colEmail")
    {
        var email = e.FormattedValue?.ToString();
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
        {
            dgvCustomers.Rows[e.RowIndex].ErrorText = "Invalid email address";
            e.Cancel = true; // Prevent cell exit
        }
        else
        {
            dgvCustomers.Rows[e.RowIndex].ErrorText = string.Empty;
        }
    }

    // Validate age (numeric range)
    if (columnName == "colAge")
    {
        if (int.TryParse(e.FormattedValue?.ToString(), out int age))
        {
            if (age < 0 || age > 150)
            {
                dgvCustomers.Rows[e.RowIndex].ErrorText = "Age must be between 0 and 150";
                e.Cancel = true;
            }
            else
            {
                dgvCustomers.Rows[e.RowIndex].ErrorText = string.Empty;
            }
        }
    }
}

private void dgvCustomers_CellEndEdit(object sender, DataGridViewCellEventArgs e)
{
    // Clear error on edit complete
    dgvCustomers.Rows[e.RowIndex].ErrorText = string.Empty;
}
```

**Custom Editors:**

```csharp
private void dgvProducts_EditingControlShowing(object sender,
    DataGridViewEditingControlShowingEventArgs e)
{
    // Customize TextBox editor for specific column
    if (dgvProducts.CurrentCell.ColumnIndex == dgvProducts.Columns["colPrice"].Index)
    {
        if (e.Control is TextBox textBox)
        {
            // Remove existing handlers
            textBox.KeyPress -= TextBox_KeyPress_NumericOnly;

            // Add numeric-only handler
            textBox.KeyPress += TextBox_KeyPress_NumericOnly;
        }
    }
}

private void TextBox_KeyPress_NumericOnly(object sender, KeyPressEventArgs e)
{
    // Allow only numbers, decimal point, and control keys
    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
    {
        e.Handled = true;
    }

    // Allow only one decimal point
    if (e.KeyChar == '.' && (sender as TextBox).Text.Contains("."))
    {
        e.Handled = true;
    }
}
```

---

### Row Styling

**Alternating Row Colors:**

```csharp
private void ConfigureRowStyling()
{
    // Set default row style
    dgvCustomers.DefaultCellStyle.BackColor = Color.White;
    dgvCustomers.DefaultCellStyle.ForeColor = Color.Black;
    dgvCustomers.DefaultCellStyle.SelectionBackColor = Color.DodgerBlue;
    dgvCustomers.DefaultCellStyle.SelectionForeColor = Color.White;

    // Set alternating row style
    dgvCustomers.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
}
```

**Conditional Row Styling:**

```csharp
private void dgvOrders_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
{
    if (dgvOrders.Rows[e.RowIndex].DataBoundItem is Order order)
    {
        // Highlight cancelled orders
        if (order.Status == OrderStatus.Cancelled)
        {
            dgvOrders.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
            dgvOrders.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.DarkRed;
        }
        // Highlight shipped orders
        else if (order.Status == OrderStatus.Shipped)
        {
            dgvOrders.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
        }
        // Highlight overdue orders
        else if (order.DueDate < DateTime.Today)
        {
            dgvOrders.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Yellow;
            dgvOrders.Rows[e.RowIndex].DefaultCellStyle.Font =
                new Font(dgvOrders.Font, FontStyle.Bold);
        }
    }
}
```

---

### Selection Modes

**Configure Selection:**

```csharp
private void ConfigureSelection()
{
    // Full row selection (most common)
    dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    dgvCustomers.MultiSelect = false;

    // Multiple row selection
    dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    dgvCustomers.MultiSelect = true;

    // Cell selection
    dgvCustomers.SelectionMode = DataGridViewSelectionMode.CellSelect;

    // Column header selection
    dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;
}
```

**Getting Selected Items:**

```csharp
private void ProcessSelectedRows()
{
    // Single selection
    if (dgvCustomers.CurrentRow?.DataBoundItem is Customer customer)
    {
        MessageBox.Show($"Selected: {customer.Name}");
    }

    // Multiple selection
    var selectedCustomers = dgvCustomers.SelectedRows
        .Cast<DataGridViewRow>()
        .Select(r => r.DataBoundItem as Customer)
        .Where(c => c != null)
        .ToList();

    MessageBox.Show($"Selected {selectedCustomers.Count} customers");
}

private void DeleteSelectedRows()
{
    if (dgvCustomers.SelectedRows.Count == 0)
    {
        MessageBox.Show("Please select rows to delete");
        return;
    }

    var result = MessageBox.Show(
        $"Delete {dgvCustomers.SelectedRows.Count} selected rows?",
        "Confirm Delete",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Warning
    );

    if (result == DialogResult.Yes)
    {
        // Remove in reverse order to avoid index shifting
        for (int i = dgvCustomers.SelectedRows.Count - 1; i >= 0; i--)
        {
            var row = dgvCustomers.SelectedRows[i];
            if (row.DataBoundItem is Customer customer)
            {
                _customers.Remove(customer);
            }
        }
    }
}
```

---

## 📝 Common Scenarios

### CRUD Operations

**Complete CRUD Example:**

```csharp
public partial class CustomerCRUDForm : Form
{
    private BindingList<Customer> _customers;
    private BindingSource _bindingSource;
    private readonly ICustomerService _service;

    public CustomerCRUDForm(ICustomerService service)
    {
        _service = service;
        InitializeComponent();
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        _customers = new BindingList<Customer>();
        _bindingSource = new BindingSource { DataSource = _customers };
        dgvCustomers.DataSource = _bindingSource;

        // Configure grid
        dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvCustomers.MultiSelect = false;
        dgvCustomers.AutoGenerateColumns = false;

        // Add columns
        ConfigureColumns();
    }

    // CREATE
    private async void btnAdd_Click(object sender, EventArgs e)
    {
        using var form = new CustomerEditForm();
        if (form.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var customer = await _service.CreateAsync(form.Customer);
                _customers.Add(customer);
                lblStatus.Text = "Customer added successfully";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // READ (already bound to grid)

    // UPDATE
    private async void btnEdit_Click(object sender, EventArgs e)
    {
        if (dgvCustomers.CurrentRow?.DataBoundItem is not Customer customer)
        {
            MessageBox.Show("Please select a customer to edit");
            return;
        }

        using var form = new CustomerEditForm(customer);
        if (form.ShowDialog() == DialogResult.OK)
        {
            try
            {
                await _service.UpdateAsync(form.Customer);
                _bindingSource.ResetCurrentItem();
                lblStatus.Text = "Customer updated successfully";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // DELETE
    private async void btnDelete_Click(object sender, EventArgs e)
    {
        if (dgvCustomers.CurrentRow?.DataBoundItem is not Customer customer)
        {
            MessageBox.Show("Please select a customer to delete");
            return;
        }

        var result = MessageBox.Show(
            $"Delete customer '{customer.Name}'?",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
        );

        if (result == DialogResult.Yes)
        {
            try
            {
                await _service.DeleteAsync(customer.Id);
                _customers.Remove(customer);
                lblStatus.Text = "Customer deleted successfully";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void ConfigureColumns()
    {
        dgvCustomers.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Customer.Name),
                HeaderText = "Name",
                Width = 200
            },
            new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Customer.Email),
                HeaderText = "Email",
                Width = 250
            },
            new DataGridViewCheckBoxColumn
            {
                DataPropertyName = nameof(Customer.IsActive),
                HeaderText = "Active",
                Width = 60
            }
        });
    }
}
```

---

### Sorting and Filtering

**Built-in Sorting:**

```csharp
private void EnableSorting()
{
    // Enable sorting on all columns
    foreach (DataGridViewColumn column in dgvCustomers.Columns)
    {
        column.SortMode = DataGridViewColumnSortMode.Automatic;
    }

    // Handle sort event
    dgvCustomers.Sorted += (s, e) =>
    {
        lblStatus.Text = $"Sorted by: {dgvCustomers.SortedColumn?.HeaderText}";
    };
}

// Programmatic sorting
private void SortByName()
{
    dgvCustomers.Sort(dgvCustomers.Columns["colName"],
        System.ComponentModel.ListSortDirection.Ascending);
}
```

**Filtering with BindingSource:**

```csharp
private void txtSearch_TextChanged(object sender, EventArgs e)
{
    var searchText = txtSearch.Text;

    if (string.IsNullOrWhiteSpace(searchText))
    {
        _bindingSource.RemoveFilter();
    }
    else
    {
        // Filter by name or email
        _bindingSource.Filter = $"Name LIKE '%{searchText}%' OR Email LIKE '%{searchText}%'";
    }

    lblStatus.Text = $"Showing {_bindingSource.Count} of {_customers.Count} customers";
}

private void chkActiveOnly_CheckedChanged(object sender, EventArgs e)
{
    if (chkActiveOnly.Checked)
    {
        _bindingSource.Filter = "IsActive = true";
    }
    else
    {
        _bindingSource.RemoveFilter();
    }
}
```

---

### Context Menus

**Row-Level Context Menu:**

```csharp
private void InitializeContextMenu()
{
    var contextMenu = new ContextMenuStrip();

    contextMenu.Items.Add("Edit", null, (s, e) => EditCurrentRow());
    contextMenu.Items.Add("Delete", null, (s, e) => DeleteCurrentRow());
    contextMenu.Items.Add(new ToolStripSeparator());
    contextMenu.Items.Add("Copy Email", null, (s, e) => CopyEmail());

    dgvCustomers.ContextMenuStrip = contextMenu;

    // Show context menu only on row click
    dgvCustomers.MouseDown += (s, e) =>
    {
        if (e.Button == MouseButtons.Right)
        {
            var hit = dgvCustomers.HitTest(e.X, e.Y);
            if (hit.RowIndex >= 0)
            {
                dgvCustomers.ClearSelection();
                dgvCustomers.Rows[hit.RowIndex].Selected = true;
            }
        }
    };
}

private void CopyEmail()
{
    if (dgvCustomers.CurrentRow?.DataBoundItem is Customer customer)
    {
        Clipboard.SetText(customer.Email);
        lblStatus.Text = "Email copied to clipboard";
    }
}
```

---

### Export to CSV/Excel

**CSV Export:**

```csharp
private void btnExportCSV_Click(object sender, EventArgs e)
{
    using var sfd = new SaveFileDialog
    {
        Filter = "CSV files (*.csv)|*.csv",
        FileName = $"customers_{DateTime.Now:yyyyMMdd}.csv"
    };

    if (sfd.ShowDialog() == DialogResult.OK)
    {
        ExportToCSV(sfd.FileName);
        lblStatus.Text = $"Exported to {sfd.FileName}";
    }
}

private void ExportToCSV(string filePath)
{
    var csv = new StringBuilder();

    // Header row
    var headers = dgvCustomers.Columns
        .Cast<DataGridViewColumn>()
        .Where(c => c.Visible)
        .Select(c => c.HeaderText);
    csv.AppendLine(string.Join(",", headers));

    // Data rows
    foreach (DataGridViewRow row in dgvCustomers.Rows)
    {
        var cells = row.Cells
            .Cast<DataGridViewCell>()
            .Where(c => c.OwningColumn.Visible)
            .Select(c => EscapeCSV(c.Value?.ToString() ?? ""));
        csv.AppendLine(string.Join(",", cells));
    }

    File.WriteAllText(filePath, csv.ToString());
}

private string EscapeCSV(string value)
{
    if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
    {
        return $"\"{value.Replace("\"", "\"\"")}\"";
    }
    return value;
}
```

---

## ✅ Best Practices

### DO:

1. **✅ Use AutoGenerateColumns = false for production code**
   - Gives you full control over column configuration
   - Prevents unexpected columns from appearing

2. **✅ Use BindingSource for all data binding**
   - Provides consistent interface for data operations
   - Enables filtering, sorting, and navigation

3. **✅ Use BindingList<T> instead of List<T>**
   - Automatic UI updates on collection changes
   - No manual refresh required

4. **✅ Implement INotifyPropertyChanged in your models**
   - Enables automatic UI updates on property changes
   - Required for proper two-way binding

5. **✅ Use Virtual Mode for large datasets (10,000+ rows)**
   - Dramatically improves performance
   - Reduces memory consumption

6. **✅ Suspend layout during bulk updates**
   - Use SuspendLayout/ResumeLayout
   - Use BindingSource.RaiseListChangedEvents = false

7. **✅ Handle CellFormatting for custom display logic**
   - Separate display logic from data model
   - Enables conditional formatting

8. **✅ Validate data in CellValidating event**
   - Prevent invalid data entry
   - Provide immediate user feedback

9. **✅ Use meaningful column names (colCustomerName)**
   - Easier to reference in code
   - Better code maintainability

10. **✅ Set appropriate SelectionMode**
    - FullRowSelect for most scenarios
    - Disable MultiSelect unless needed

11. **✅ Configure ReadOnly for calculated columns**
    - Prevents editing of derived data
    - Clear indication to users

12. **✅ Dispose DataGridView and BindingSource properly**
    - Prevents memory leaks
    - Releases resources correctly

---

### DON'T:

1. **❌ Don't use AutoSizeColumnsMode = AllCells in production**
   - Severe performance impact
   - Recalculates on every data change

2. **❌ Don't call Refresh() unnecessarily**
   - Causes full grid repaint
   - Usually not needed with data binding

3. **❌ Don't access Rows collection directly for data operations**
   - Use bound data source instead
   - Maintains proper separation of concerns

4. **❌ Don't load all data at once for large datasets**
   - Use paging or virtual mode
   - Load data on-demand

5. **❌ Don't put business logic in grid events**
   - Keep grid for display only
   - Business logic belongs in services

6. **❌ Don't hardcode column indices**
   - Use column names instead
   - Prevents errors when columns change

7. **❌ Don't ignore CellValidating events**
   - Always validate user input
   - Provide clear error messages

8. **❌ Don't allow editing of DataGridView during async operations**
   - Disable grid during operations
   - Prevents data inconsistencies

9. **❌ Don't forget to handle null values in formatting**
   - Always check for null/DBNull
   - Provide appropriate default display

10. **❌ Don't create columns in Form_Load multiple times**
    - Check if columns already exist
    - Or clear columns before adding

---

## 🐛 Common Issues and Solutions

### Issue 1: Slow Performance with Large Datasets

**Problem:** Grid is sluggish with 10,000+ rows

**Solution:**
```csharp
// Use Virtual Mode
dgvData.VirtualMode = true;
dgvData.RowCount = totalRowCount;
dgvData.CellValueNeeded += LoadDataOnDemand;
```

---

### Issue 2: Grid Not Updating After Data Changes

**Problem:** Data changes don't reflect in grid

**Solution:**
```csharp
// Use BindingList<T> instead of List<T>
var customers = new BindingList<Customer>();

// Implement INotifyPropertyChanged in model
public class Customer : INotifyPropertyChanged { ... }

// Or manually refresh
_bindingSource.ResetBindings(false);
```

---

### Issue 3: ComboBox Column Shows Wrong Values

**Problem:** ComboBox displays object type instead of display value

**Solution:**
```csharp
var statusColumn = new DataGridViewComboBoxColumn
{
    DataPropertyName = "StatusId",
    DisplayMember = "Name",     // What to display
    ValueMember = "Id",         // Actual value
    DataSource = statusList
};
```

---

### Issue 4: Cell Validation Prevents Form Closing

**Problem:** Form won't close when cell validation fails

**Solution:**
```csharp
private void Form_FormClosing(object sender, FormClosingEventArgs e)
{
    // Cancel cell edit to allow form to close
    dgvCustomers.EndEdit();
    _bindingSource.EndEdit();
}
```

---

### Issue 5: Alternating Row Colors Not Working

**Problem:** All rows have same color

**Solution:**
```csharp
// Set AlternatingRowsDefaultCellStyle, not RowsDefaultCellStyle
dgvCustomers.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
```

---

### Issue 6: Selected Row Not Visible After Programmatic Selection

**Problem:** Scrolling doesn't follow programmatic selection

**Solution:**
```csharp
private void SelectAndScrollToRow(int rowIndex)
{
    dgvCustomers.ClearSelection();
    dgvCustomers.Rows[rowIndex].Selected = true;
    dgvCustomers.FirstDisplayedScrollingRowIndex = rowIndex;
}
```

---

## 🎓 Performance Tips Checklist

- [ ] Use Virtual Mode for datasets > 10,000 rows
- [ ] Set AutoGenerateColumns = false
- [ ] Avoid AutoSizeColumnsMode = AllCells
- [ ] Use fixed column widths when possible
- [ ] Suspend layout during bulk updates
- [ ] Set BindingSource.RaiseListChangedEvents = false during updates
- [ ] Load data asynchronously
- [ ] Use cell-level events (CellFormatting) instead of row-level when possible
- [ ] Minimize use of CellPainting (expensive)
- [ ] Cache frequently accessed data
- [ ] Implement data paging for very large datasets
- [ ] Dispose resources properly

---

## 🔗 Related Topics

- **[Data Binding](data-binding.md)** - Comprehensive binding techniques
- **[Form Communication](form-communication.md)** - Passing data between forms
- **[Input Validation](input-validation.md)** - Validation patterns
- **[Async/Await](../best-practices/async-await.md)** - Loading data asynchronously
- **[Performance Optimization](../best-practices/performance.md)** - General performance tips
- **[MVP Pattern](../architecture/mvp-pattern.md)** - Separating grid logic from business logic

---

**Last Updated**: 2025-11-07
**Related Standards**: [Code Style](../conventions/code-style.md) | [Best Practices](../best-practices/)
