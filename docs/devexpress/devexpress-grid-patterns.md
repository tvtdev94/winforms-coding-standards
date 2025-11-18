# DevExpress XtraGrid Patterns & Best Practices

> **Purpose**: Advanced patterns and best practices for DevExpress XtraGrid
> **Audience**: WinForms developers building data-intensive applications

---

## 📋 Table of Contents

1. [Grid Configuration Patterns](#grid-configuration-patterns)
2. [Search & Filter](#search--filter)
3. [Export Features](#export-features)
4. [Custom Columns](#custom-columns)
5. [Performance Optimization](#performance-optimization)
6. [User Experience](#user-experience)
7. [Common Scenarios](#common-scenarios)

---

## Grid Configuration Patterns

### Standard Read-Only Grid

```csharp
public void ConfigureReadOnlyGrid(GridView gridView)
{
    // Basic configuration
    gridView.OptionsBehavior.Editable = false;
    gridView.OptionsBehavior.ReadOnly = true;
    gridView.OptionsView.ShowGroupPanel = false;
    gridView.OptionsSelection.MultiSelect = false;
    gridView.OptionsSelection.EnableAppearanceFocusedCell = false;

    // Row appearance
    gridView.Appearance.FocusedRow.BackColor = Color.FromArgb(0, 122, 204);
    gridView.Appearance.FocusedRow.ForeColor = Color.White;
    gridView.OptionsView.EnableAppearanceEvenRow = true;
    gridView.Appearance.EvenRow.BackColor = Color.FromArgb(250, 250, 250);

    // Enable built-in search
    gridView.OptionsFind.AlwaysVisible = true;
    gridView.OptionsFind.FindNullPrompt = "Search...";
    gridView.OptionsFind.ShowCloseButton = false;

    // Double-click to open
    gridView.DoubleClick += GridView_DoubleClick;
}

private void GridView_DoubleClick(object sender, EventArgs e)
{
    var gridView = sender as GridView;
    var hitInfo = gridView.CalcHitInfo((sender as Control).PointToClient(Cursor.Position));

    if (hitInfo.InRow && hitInfo.RowHandle >= 0)
    {
        var row = gridView.GetRow(hitInfo.RowHandle);
        OpenEditForm(row);
    }
}
```

### Editable Grid (Inline Editing)

```csharp
public void ConfigureEditableGrid(GridView gridView)
{
    // Enable editing
    gridView.OptionsBehavior.Editable = true;
    gridView.OptionsBehavior.ReadOnly = false;

    // Validation
    gridView.ValidateRow += GridView_ValidateRow;
    gridView.InvalidRowException += GridView_InvalidRowException;

    // Make specific columns read-only
    gridView.Columns["Id"].OptionsColumn.AllowEdit = false;
    gridView.Columns["CreatedDate"].OptionsColumn.AllowEdit = false;

    // Cell value changed
    gridView.CellValueChanged += GridView_CellValueChanged;
}

private void GridView_ValidateRow(object sender, ValidateRowEventArgs e)
{
    var gridView = sender as GridView;
    var name = gridView.GetRowCellValue(e.RowHandle, "Name")?.ToString();

    if (string.IsNullOrWhiteSpace(name))
    {
        e.Valid = false;
        e.ErrorText = "Name is required";
    }
}

private void GridView_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
{
    e.ExceptionMode = ExceptionMode.DisplayError;
}

private void GridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
{
    var gridView = sender as GridView;
    var row = gridView.GetRow(e.RowHandle);

    // Mark as modified
    _modifiedRows.Add(row);
}
```

### Multi-Select Grid

```csharp
public void ConfigureMultiSelectGrid(GridView gridView)
{
    // Enable multi-select
    gridView.OptionsSelection.MultiSelect = true;
    gridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;

    // Show checkboxes
    gridView.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DefaultBoolean.True;
    gridView.OptionsSelection.ShowCheckBoxSelectorInGroupRow = DefaultBoolean.True;
}

// Get selected rows
private List<Customer> GetSelectedCustomers()
{
    var gridView = gridControl1.MainView as GridView;
    var selectedRows = new List<Customer>();

    int[] selectedRowHandles = gridView.GetSelectedRows();
    foreach (int rowHandle in selectedRowHandles)
    {
        if (rowHandle >= 0)
        {
            var customer = gridView.GetRow(rowHandle) as Customer;
            if (customer != null)
            {
                selectedRows.Add(customer);
            }
        }
    }

    return selectedRows;
}
```

---

## Search & Filter

### Built-in Search Panel

```csharp
public void EnableSearchPanel(GridView gridView)
{
    // Enable search panel
    gridView.OptionsFind.AlwaysVisible = true;
    gridView.OptionsFind.FindNullPrompt = "Search customers...";
    gridView.OptionsFind.ShowCloseButton = false;
    gridView.OptionsFind.ShowClearButton = true;

    // Search in specific columns
    gridView.OptionsFind.FindFilterColumns = "Name;Email;Phone";

    // Case-insensitive search
    gridView.OptionsFind.FindMode = FindMode.Always;
}
```

### Auto Filter Row

```csharp
public void EnableAutoFilter(GridView gridView)
{
    // Show filter row
    gridView.OptionsView.ShowAutoFilterRow = true;

    // Configure filter behavior
    gridView.OptionsFilter.AllowFilterEditor = true;
    gridView.OptionsFilter.AllowColumnMRUFilterList = true;
    gridView.OptionsFilter.ShowAllTableValuesInFilterPopup = true;
}
```

### Custom Filter

```csharp
private void btnFilterActive_Click(object sender, EventArgs e)
{
    var gridView = gridControl1.MainView as GridView;

    // Apply custom filter
    gridView.ActiveFilterString = "[IsActive] = True";
}

private void btnClearFilter_Click(object sender, EventArgs e)
{
    var gridView = gridControl1.MainView as GridView;
    gridView.ClearColumnsFilter();
    gridView.ActiveFilterString = string.Empty;
}

// Filter with multiple conditions
private void ApplyAdvancedFilter()
{
    var gridView = gridControl1.MainView as GridView;

    // IsActive = true AND CreatedDate >= last month
    var lastMonth = DateTime.Today.AddMonths(-1);
    gridView.ActiveFilterString = $"[IsActive] = True AND [CreatedDate] >= #{lastMonth:yyyy-MM-dd}#";
}
```

---

## Export Features

### Export to Excel

```csharp
using DevExpress.XtraPrinting;
using DevExpress.XtraGrid.Views.Grid;

private void btnExportExcel_Click(object sender, EventArgs e)
{
    var gridView = gridControl1.MainView as GridView;

    var saveFileDialog = new SaveFileDialog
    {
        Filter = "Excel Files|*.xlsx",
        FileName = $"Customers_{DateTime.Now:yyyyMMdd}.xlsx"
    };

    if (saveFileDialog.ShowDialog() == DialogResult.OK)
    {
        gridView.ExportToXlsx(saveFileDialog.FileName);
        XtraMessageBox.Show("Export completed successfully", "Success",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
```

### Export to PDF

```csharp
private void btnExportPdf_Click(object sender, EventArgs e)
{
    var gridView = gridControl1.MainView as GridView;

    var saveFileDialog = new SaveFileDialog
    {
        Filter = "PDF Files|*.pdf",
        FileName = $"Customers_{DateTime.Now:yyyyMMdd}.pdf"
    };

    if (saveFileDialog.ShowDialog() == DialogResult.OK)
    {
        gridView.ExportToPdf(saveFileDialog.FileName);
        XtraMessageBox.Show("Export completed successfully", "Success",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
```

### Export to CSV

```csharp
private void btnExportCsv_Click(object sender, EventArgs e)
{
    var gridView = gridControl1.MainView as GridView;

    var saveFileDialog = new SaveFileDialog
    {
        Filter = "CSV Files|*.csv",
        FileName = $"Customers_{DateTime.Now:yyyyMMdd}.csv"
    };

    if (saveFileDialog.ShowDialog() == DialogResult.OK)
    {
        gridView.ExportToCsv(saveFileDialog.FileName);
        XtraMessageBox.Show("Export completed successfully", "Success",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
```

---

## Custom Columns

### Unbound Columns (Calculated)

```csharp
private void SetupUnboundColumns()
{
    var gridView = gridControl1.MainView as GridView;

    // Add unbound column
    var colFullName = new GridColumn
    {
        FieldName = "FullName",
        Caption = "Full Name",
        UnboundDataType = typeof(string),
        Visible = true
    };
    gridView.Columns.Add(colFullName);

    // Handle custom value
    gridView.CustomUnboundColumnData += GridView_CustomUnboundColumnData;
}

private void GridView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
{
    if (e.Column.FieldName == "FullName" && e.IsGetData)
    {
        var customer = gridView1.GetRow(e.ListSourceRowIndex) as Customer;
        if (customer != null)
        {
            e.Value = $"{customer.FirstName} {customer.LastName}";
        }
    }
}
```

### Custom Cell Formatting

```csharp
private void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
{
    var gridView = sender as GridView;

    // Highlight inactive customers
    if (e.Column.FieldName == "IsActive")
    {
        bool isActive = (bool)gridView.GetRowCellValue(e.RowHandle, "IsActive");
        if (!isActive)
        {
            e.Appearance.BackColor = Color.LightCoral;
            e.Appearance.ForeColor = Color.White;
        }
    }

    // Highlight overdue
    if (e.Column.FieldName == "DueDate")
    {
        DateTime? dueDate = gridView.GetRowCellValue(e.RowHandle, "DueDate") as DateTime?;
        if (dueDate.HasValue && dueDate.Value < DateTime.Today)
        {
            e.Appearance.BackColor = Color.Yellow;
        }
    }
}
```

### Custom Display Text

```csharp
private void gridView1_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
{
    // Format currency
    if (e.Column.FieldName == "TotalAmount" && e.Value != null)
    {
        decimal amount = (decimal)e.Value;
        e.DisplayText = amount.ToString("C2");
    }

    // Format status
    if (e.Column.FieldName == "Status" && e.Value != null)
    {
        int status = (int)e.Value;
        e.DisplayText = status switch
        {
            0 => "Pending",
            1 => "Active",
            2 => "Completed",
            3 => "Cancelled",
            _ => "Unknown"
        };
    }
}
```

---

## Performance Optimization

### Virtual Mode for Large Datasets

```csharp
public void ConfigureVirtualMode(GridView gridView)
{
    // Enable virtual mode
    gridView.OptionsView.ColumnAutoWidth = false;

    // Disable features that hurt performance
    gridView.OptionsView.EnableAppearanceEvenRow = false;
    gridView.OptionsView.EnableAppearanceOddRow = false;

    // Use instant feedback for filtering
    gridView.OptionsView.ShowIndicator = false;
}
```

### Lazy Loading

```csharp
private async void gridView1_RowCountChanged(object sender, EventArgs e)
{
    var gridView = sender as GridView;

    // Load next page when scrolling near bottom
    if (gridView.IsLastVisibleRow)
    {
        await LoadNextPageAsync();
    }
}

private async Task LoadNextPageAsync()
{
    if (_isLoading) return;

    _isLoading = true;
    try
    {
        var nextPage = await _customerService.GetPageAsync(_currentPage + 1, _pageSize);
        _customers.AddRange(nextPage);
        _currentPage++;

        gridControl1.RefreshDataSource();
    }
    finally
    {
        _isLoading = false;
    }
}
```

### Async Data Loading

```csharp
private async Task LoadDataOptimizedAsync()
{
    var gridView = gridControl1.MainView as GridView;

    // Begin update
    gridView.BeginUpdate();
    try
    {
        // Load data with AsNoTracking for read-only
        var customers = await _customerService.GetAllAsync(useTracking: false);

        // Bind
        gridControl1.DataSource = customers;

        // Configure
        ConfigureGridOptimized(gridView);
    }
    finally
    {
        // End update (refresh once)
        gridView.EndUpdate();
    }
}

private void ConfigureGridOptimized(GridView gridView)
{
    // Batch updates
    gridView.BeginSort();
    try
    {
        gridView.Columns["Name"].SortOrder = ColumnSortOrder.Ascending;
    }
    finally
    {
        gridView.EndSort();
    }

    // Best fit columns
    gridView.BestFitColumns();
}
```

---

## User Experience

### Remember User Preferences

```csharp
// Save grid layout
private void SaveGridLayout()
{
    var gridView = gridControl1.MainView as GridView;
    gridView.SaveLayoutToXml("grid_layout.xml");
}

// Load grid layout
private void LoadGridLayout()
{
    var gridView = gridControl1.MainView as GridView;
    if (File.Exists("grid_layout.xml"))
    {
        gridView.RestoreLayoutFromXml("grid_layout.xml");
    }
}

// Save on form close
private void CustomerListForm_FormClosing(object sender, FormClosingEventArgs e)
{
    SaveGridLayout();
}

// Load on form load
private void CustomerListForm_Load(object sender, EventArgs e)
{
    LoadGridLayout();
}
```

### Context Menu

```csharp
private void SetupContextMenu()
{
    var contextMenu = new ContextMenuStrip();

    contextMenu.Items.Add("Edit", null, (s, e) => EditSelectedRow());
    contextMenu.Items.Add("Delete", null, (s, e) => DeleteSelectedRow());
    contextMenu.Items.Add("-"); // Separator
    contextMenu.Items.Add("Refresh", null, (s, e) => RefreshGridAsync());
    contextMenu.Items.Add("Export to Excel", null, (s, e) => ExportToExcel());

    gridControl1.ContextMenuStrip = contextMenu;
}
```

### Status Information

```csharp
private void UpdateStatusBar()
{
    var gridView = gridControl1.MainView as GridView;

    int totalRows = gridView.DataRowCount;
    int visibleRows = gridView.VisibleRowCount;
    int selectedRows = gridView.SelectedRowsCount;

    lblStatus.Text = $"Total: {totalRows} | Visible: {visibleRows} | Selected: {selectedRows}";
}

private void gridView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
{
    UpdateStatusBar();
}

private void gridView1_ColumnFilterChanged(object sender, EventArgs e)
{
    UpdateStatusBar();
}
```

---

## Common Scenarios

### CRUD Grid Pattern

```csharp
public partial class CustomerListForm : XtraForm
{
    private readonly ICustomerService _customerService;
    private readonly IFormFactory _formFactory;
    private GridView gridView => gridControl1.MainView as GridView;

    // Create
    private void btnNew_Click(object sender, EventArgs e)
    {
        var form = _formFactory.Create<CustomerEditForm>();
        if (form.ShowDialog() == DialogResult.OK)
        {
            RefreshGridAsync();
        }
    }

    // Read (in grid)

    // Update
    private void btnEdit_Click(object sender, EventArgs e)
    {
        var customer = gridView.GetFocusedRow() as Customer;
        if (customer == null) return;

        var form = _formFactory.Create<CustomerEditForm>();
        form.LoadCustomer(customer.Id);
        if (form.ShowDialog() == DialogResult.OK)
        {
            RefreshGridAsync();
        }
    }

    // Delete
    private async void btnDelete_Click(object sender, EventArgs e)
    {
        var customer = gridView.GetFocusedRow() as Customer;
        if (customer == null) return;

        var result = XtraMessageBox.Show(
            $"Delete '{customer.Name}'?",
            "Confirm",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            await _customerService.DeleteAsync(customer.Id);
            await RefreshGridAsync();
        }
    }

    private async Task RefreshGridAsync()
    {
        var customers = await _customerService.GetAllAsync();
        gridControl1.DataSource = customers;
        gridView.BestFitColumns();
    }
}
```

---

## Best Practices Summary

### ✅ DO

1. **Configure after binding** data
2. **Use AsNoTracking()** for read-only grids
3. **Enable built-in search** (`OptionsFind.AlwaysVisible`)
4. **Hide technical columns** (Id, timestamps)
5. **Use BestFitColumns()** after binding
6. **Save/restore user layout**
7. **Provide export features** (Excel, PDF, CSV)
8. **Show status information** (row counts)
9. **Use async loading** with progress indicators
10. **Handle null values** in custom columns

### ❌ DON'T

1. ❌ Allow inline editing for complex forms (use edit dialogs)
2. ❌ Load large datasets synchronously
3. ❌ Bind to IQueryable
4. ❌ Forget to call BeginUpdate/EndUpdate for batch operations
5. ❌ Mix DevExpress and standard controls

---

## Next Steps

- **Data Binding** → [devexpress-data-binding.md](devexpress-data-binding.md)
- **Responsive Design** → [devexpress-responsive-design.md](devexpress-responsive-design.md)
- **Controls Guide** → [devexpress-controls.md](devexpress-controls.md)

---

## Resources

- **Official Docs**: https://docs.devexpress.com/WindowsForms/3455/controls-and-libraries/data-grid
- **Performance Tips**: https://docs.devexpress.com/WindowsForms/7360/controls-and-libraries/data-grid/performance-improvement

---

**Last Updated**: 2025-11-17
**DevExpress Version**: 24.1+
