# DevExpress Data Binding Patterns

> **Purpose**: Complete guide to data binding with DevExpress controls
> **Audience**: WinForms developers using DevExpress components

---

## 📋 Table of Contents

1. [Data Binding Overview](#data-binding-overview)
2. [GridControl Data Binding](#gridcontrol-data-binding)
3. [LookUpEdit Data Binding](#lookupedit-data-binding)
4. [Form Data Binding](#form-data-binding)
5. [BindingSource Pattern](#bindingsource-pattern)
6. [Master-Detail Binding](#master-detail-binding)
7. [Best Practices](#best-practices)

---

## Data Binding Overview

DevExpress controls support multiple data binding approaches:

| Approach | Use Case | Performance |
|----------|----------|-------------|
| **Direct Binding** | Simple scenarios | Good |
| **BindingSource** | Forms with validation | ✅ Best |
| **BindingList** | Observable collections | Good |
| **INotifyPropertyChanged** | Real-time updates | Medium |

---

## GridControl Data Binding

### Simple List Binding

```csharp
public partial class CustomerListForm : XtraForm
{
    private readonly ICustomerService _customerService;

    public async Task LoadDataAsync()
    {
        try
        {
            // Load data from service
            var customers = await _customerService.GetAllAsync();

            // ✅ Direct binding
            gridControl1.DataSource = customers;

            // Configure columns after binding
            ConfigureGrid();
        }
        catch (Exception ex)
        {
            XtraMessageBox.Show($"Failed to load customers: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ConfigureGrid()
    {
        var gridView = gridControl1.MainView as GridView;

        // Auto-size columns
        gridView.BestFitColumns();

        // Hide technical columns
        gridView.Columns["Id"].Visible = false;
        gridView.Columns["CreatedDate"].Visible = false;

        // Set display names
        gridView.Columns["Name"].Caption = "Customer Name";
        gridView.Columns["Email"].Caption = "Email Address";
    }
}
```

### Async Data Loading Pattern

```csharp
public partial class CustomerListForm : XtraForm
{
    private async void CustomerListForm_Load(object sender, EventArgs e)
    {
        await LoadDataWithProgressAsync();
    }

    private async Task LoadDataWithProgressAsync()
    {
        try
        {
            // Show loading indicator
            gridControl1.UseWaitCursor = true;
            overlayWindowOptions1.ShowProgressPanel();

            // Load data asynchronously
            var customers = await _customerService.GetAllAsync();

            // Bind to grid
            gridControl1.DataSource = customers;

            ConfigureGrid();
        }
        catch (Exception ex)
        {
            XtraMessageBox.Show($"Error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            overlayWindowOptions1.HideProgressPanel();
            gridControl1.UseWaitCursor = false;
        }
    }
}
```

### Refresh Grid Data

```csharp
public async Task RefreshGridAsync()
{
    // Save current selection
    var gridView = gridControl1.MainView as GridView;
    var focusedRowHandle = gridView.FocusedRowHandle;
    var selectedId = gridView.GetFocusedRowCellValue("Id");

    // Reload data
    var customers = await _customerService.GetAllAsync();
    gridControl1.DataSource = customers;

    // Restore selection
    if (selectedId != null)
    {
        int rowHandle = gridView.LocateByValue("Id", selectedId);
        if (rowHandle != GridControl.InvalidRowHandle)
        {
            gridView.FocusedRowHandle = rowHandle;
        }
    }

    gridView.BestFitColumns();
}
```

---

## LookUpEdit Data Binding

### Basic LookUpEdit Setup

```csharp
public partial class CustomerEditForm : XtraForm
{
    public void SetupLookUps()
    {
        // Load customer types
        var customerTypes = _referenceDataService.GetCustomerTypes();

        // Bind to LookUpEdit
        lkeCustomerType.Properties.DataSource = customerTypes;
        lkeCustomerType.Properties.DisplayMember = "Name";
        lkeCustomerType.Properties.ValueMember = "Id";

        // Configure display columns
        lkeCustomerType.Properties.Columns.Clear();
        lkeCustomerType.Properties.Columns.Add(new LookUpColumnInfo("Name", "Type"));
        lkeCustomerType.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));

        // Set dropdown size
        lkeCustomerType.Properties.PopupWidth = 400;

        // Enable search/filter
        lkeCustomerType.Properties.SearchMode = SearchMode.AutoFilter;
        lkeCustomerType.Properties.AutoSearchColumnIndex = 0;

        // Allow null selection
        lkeCustomerType.Properties.NullText = "-- Select Type --";
    }

    // Get selected value
    public int? GetSelectedCustomerType()
    {
        return lkeCustomerType.EditValue as int?;
    }

    // Set selected value
    public void SetSelectedCustomerType(int? typeId)
    {
        lkeCustomerType.EditValue = typeId;
    }
}
```

### Cascading LookUpEdits

```csharp
// Country -> State -> City cascade

private void lkeCountry_EditValueChanged(object sender, EventArgs e)
{
    int? countryId = lkeCountry.EditValue as int?;

    if (countryId.HasValue)
    {
        // Load states for selected country
        var states = _referenceDataService.GetStatesByCountry(countryId.Value);
        lkeState.Properties.DataSource = states;
        lkeState.Enabled = true;
    }
    else
    {
        lkeState.Properties.DataSource = null;
        lkeState.EditValue = null;
        lkeState.Enabled = false;

        lkeCity.Properties.DataSource = null;
        lkeCity.EditValue = null;
        lkeCity.Enabled = false;
    }
}

private void lkeState_EditValueChanged(object sender, EventArgs e)
{
    int? stateId = lkeState.EditValue as int?;

    if (stateId.HasValue)
    {
        var cities = _referenceDataService.GetCitiesByState(stateId.Value);
        lkeCity.Properties.DataSource = cities;
        lkeCity.Enabled = true;
    }
    else
    {
        lkeCity.Properties.DataSource = null;
        lkeCity.EditValue = null;
        lkeCity.Enabled = false;
    }
}
```

---

## Form Data Binding

### Manual Two-Way Binding

```csharp
public partial class CustomerEditForm : XtraForm, ICustomerEditView
{
    private Customer _currentCustomer;

    // Load customer data into form
    public void SetCustomer(Customer customer)
    {
        _currentCustomer = customer;

        // Bind to controls
        txtCustomerName.EditValue = customer.Name;
        txtEmail.EditValue = customer.Email;
        txtPhone.EditValue = customer.Phone;
        lkeCustomerType.EditValue = customer.CustomerTypeId;
        chkIsActive.Checked = customer.IsActive;
        dteCreatedDate.EditValue = customer.CreatedDate;
        memoNotes.EditValue = customer.Notes;
    }

    // Get customer data from form
    public Customer GetCustomer()
    {
        _currentCustomer.Name = txtCustomerName.Text;
        _currentCustomer.Email = txtEmail.Text;
        _currentCustomer.Phone = txtPhone.Text;
        _currentCustomer.CustomerTypeId = lkeCustomerType.EditValue as int?;
        _currentCustomer.IsActive = chkIsActive.Checked;
        _currentCustomer.CreatedDate = dteCreatedDate.EditValue as DateTime? ?? DateTime.Now;
        _currentCustomer.Notes = memoNotes.Text;

        return _currentCustomer;
    }
}
```

---

## BindingSource Pattern

### Using BindingSource (Recommended)

```csharp
public partial class CustomerEditForm : XtraForm
{
    private BindingSource _customerBindingSource;
    private Customer _currentCustomer;

    public CustomerEditForm()
    {
        InitializeComponent();
        SetupDataBinding();
    }

    private void SetupDataBinding()
    {
        _customerBindingSource = new BindingSource();

        // Bind controls to BindingSource
        txtCustomerName.DataBindings.Add("EditValue", _customerBindingSource, "Name", true,
            DataSourceUpdateMode.OnPropertyChanged);

        txtEmail.DataBindings.Add("EditValue", _customerBindingSource, "Email", true,
            DataSourceUpdateMode.OnPropertyChanged);

        txtPhone.DataBindings.Add("EditValue", _customerBindingSource, "Phone", true,
            DataSourceUpdateMode.OnPropertyChanged);

        lkeCustomerType.DataBindings.Add("EditValue", _customerBindingSource, "CustomerTypeId", true,
            DataSourceUpdateMode.OnPropertyChanged);

        chkIsActive.DataBindings.Add("Checked", _customerBindingSource, "IsActive", true,
            DataSourceUpdateMode.OnPropertyChanged);

        dteCreatedDate.DataBindings.Add("EditValue", _customerBindingSource, "CreatedDate", true,
            DataSourceUpdateMode.OnPropertyChanged);

        memoNotes.DataBindings.Add("EditValue", _customerBindingSource, "Notes", true,
            DataSourceUpdateMode.OnPropertyChanged);
    }

    public void SetCustomer(Customer customer)
    {
        _currentCustomer = customer;
        _customerBindingSource.DataSource = _currentCustomer;
    }

    public Customer GetCustomer()
    {
        // Data is already synchronized via BindingSource
        return _currentCustomer;
    }
}
```

### Benefits of BindingSource

✅ **Automatic synchronization** - No manual GetCustomer() needed
✅ **Validation support** - Works with data annotations
✅ **Change tracking** - Knows when data is modified
✅ **Less code** - No manual property assignments

---

## Master-Detail Binding

### Grid Master-Detail

```csharp
public partial class OrderListForm : XtraForm
{
    private void SetupMasterDetail()
    {
        // Master: Orders
        var orders = await _orderService.GetAllAsync();
        gridControlOrders.DataSource = orders;

        // Detail: Order Items
        var gridViewOrders = gridControlOrders.MainView as GridView;

        // Add detail view
        var detailView = new GridView();
        detailView.Name = "OrderItemsDetailView";
        gridControlOrders.LevelTree.Nodes.Add("OrderItems", detailView);

        // Configure detail
        detailView.OptionsView.ShowGroupPanel = false;
        detailView.Columns["OrderId"].Visible = false;

        // Bind detail
        gridViewOrders.MasterRowGetChildList += GridViewOrders_MasterRowGetChildList;
    }

    private void GridViewOrders_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
    {
        var gridView = sender as GridView;
        var order = gridView.GetRow(e.RowHandle) as Order;

        if (order != null)
        {
            e.ChildList = order.OrderItems;
        }
    }
}
```

### Form Master-Detail

```csharp
public partial class CustomerOrdersForm : XtraForm
{
    // Grid selection changes detail panel
    private void gridViewCustomers_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
    {
        var gridView = sender as GridView;
        var customer = gridView.GetFocusedRow() as Customer;

        if (customer != null)
        {
            LoadCustomerDetails(customer);
        }
    }

    private async void LoadCustomerDetails(Customer customer)
    {
        // Update detail panel
        txtCustomerName.Text = customer.Name;
        txtEmail.Text = customer.Email;

        // Load related orders
        var orders = await _orderService.GetOrdersByCustomerAsync(customer.Id);
        gridControlOrders.DataSource = orders;
    }
}
```

---

## Best Practices

### ✅ DO

1. **Use BindingSource** for form data binding
2. **Load data asynchronously** with async/await
3. **Show loading indicators** during data load
4. **Configure columns after binding**
5. **Use AsNoTracking()** for read-only grids
6. **Handle null values** appropriately
7. **Refresh grid after updates**

```csharp
// ✅ GOOD: Async loading with indicator
private async Task LoadDataAsync()
{
    ShowLoadingIndicator();
    try
    {
        var data = await _service.GetAllAsync();
        gridControl.DataSource = data;
        ConfigureGrid();
    }
    finally
    {
        HideLoadingIndicator();
    }
}
```

### ❌ DON'T

1. ❌ Bind to IQueryable (use List)
2. ❌ Load large datasets synchronously
3. ❌ Forget to handle binding errors
4. ❌ Mix binding approaches in same form
5. ❌ Bind directly to database entities (use DTOs/ViewModels)

```csharp
// ❌ BAD: Synchronous blocking load
private void LoadData()
{
    var data = _service.GetAll(); // Blocks UI!
    gridControl.DataSource = data;
}

// ❌ BAD: Binding to IQueryable
gridControl.DataSource = _context.Customers; // Don't do this!
```

---

## Common Patterns

### Pattern 1: Load-Edit-Save

```csharp
public partial class CustomerEditForm : XtraForm
{
    private BindingSource _bindingSource;
    private Customer _customer;

    // 1. Load
    public async Task LoadCustomerAsync(int customerId)
    {
        _customer = await _customerService.GetByIdAsync(customerId);
        _bindingSource.DataSource = _customer;
    }

    // 2. Edit (automatic via data binding)

    // 3. Save
    private async void btnSave_Click(object sender, EventArgs e)
    {
        if (!ValidateForm())
            return;

        try
        {
            await _customerService.UpdateAsync(_customer);
            XtraMessageBox.Show("Customer saved successfully", "Success");
            this.DialogResult = DialogResult.OK;
        }
        catch (Exception ex)
        {
            XtraMessageBox.Show($"Save failed: {ex.Message}", "Error");
        }
    }
}
```

### Pattern 2: Grid with CRUD Operations

```csharp
public partial class CustomerListForm : XtraForm
{
    // Create
    private void btnNew_Click(object sender, EventArgs e)
    {
        var form = _formFactory.Create<CustomerEditForm>();
        if (form.ShowDialog() == DialogResult.OK)
        {
            RefreshGridAsync();
        }
    }

    // Read (already loaded in grid)

    // Update
    private void btnEdit_Click(object sender, EventArgs e)
    {
        var customer = GetSelectedCustomer();
        if (customer == null) return;

        var form = _formFactory.Create<CustomerEditForm>();
        form.LoadCustomerAsync(customer.Id);
        if (form.ShowDialog() == DialogResult.OK)
        {
            RefreshGridAsync();
        }
    }

    // Delete
    private async void btnDelete_Click(object sender, EventArgs e)
    {
        var customer = GetSelectedCustomer();
        if (customer == null) return;

        var result = XtraMessageBox.Show(
            $"Delete customer '{customer.Name}'?",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            await _customerService.DeleteAsync(customer.Id);
            await RefreshGridAsync();
        }
    }

    private Customer GetSelectedCustomer()
    {
        var gridView = gridControl1.MainView as GridView;
        return gridView.GetFocusedRow() as Customer;
    }
}
```

---

## Summary

**Key Takeaways**:

1. **Use BindingSource** for form data binding (best practice)
2. **Load data asynchronously** to avoid blocking UI
3. **Configure grids after binding** data
4. **Handle null values** and errors gracefully
5. **Use DTOs/ViewModels** instead of direct entity binding
6. **Follow MVP pattern** - binding in View, logic in Presenter

---

## Next Steps

- **Grid Patterns** → [devexpress-grid-patterns.md](devexpress-grid-patterns.md)
- **Responsive Design** → [devexpress-responsive-design.md](devexpress-responsive-design.md)
- **Controls Guide** → [devexpress-controls.md](devexpress-controls.md)

---

## Resources

- **Official Docs**: https://docs.devexpress.com/WindowsForms/1461/controls-and-libraries/data-grid
- **Binding Examples**: DevExpress Demo Center

---

**Last Updated**: 2025-11-17
**DevExpress Version**: 24.1+
