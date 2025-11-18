# ReaLTaiizor Data Binding Guide

> **Purpose**: Data binding patterns with ReaLTaiizor controls
> **Audience**: WinForms developers using ReaLTaiizor

---

## ListView Data Binding

### Material ListView

```csharp
using ReaLTaiizor.Controls;

public void LoadCustomers(List<Customer> customers)
{
    var lvCustomers = new MaterialListView();
    lvCustomers.View = View.Details;
    lvCustomers.FullRowSelect = true;

    // Columns
    lvCustomers.Columns.Add("ID", 60);
    lvCustomers.Columns.Add("Name", 200);
    lvCustomers.Columns.Add("Email", 250);

    // Data
    lvCustomers.Items.Clear();
    foreach (var customer in customers)
    {
        var item = new ListViewItem(new[]
        {
            customer.Id.ToString(),
            customer.Name,
            customer.Email
        });
        lvCustomers.Items.Add(item);
    }
}
```

---

## ComboBox Data Binding

### Material ComboBox

```csharp
public void SetupCustomerTypeComboBox(List<CustomerType> types)
{
    var cboType = new MaterialComboBox();
    cboType.DataSource = types;
    cboType.DisplayMember = "Name";
    cboType.ValueMember = "Id";
    cboType.Hint = "Select type...";
}

// Get selected value
var selectedType = (CustomerType)cboType.SelectedItem;
int selectedId = (int)cboType.SelectedValue;
```

---

## Metro Grid Data Binding

### Metro Grid

```csharp
using ReaLTaiizor.Controls;

public void LoadCustomersToGrid(List<Customer> customers)
{
    var grid = new MetroGrid();

    // Setup
    grid.AllowUserToAddRows = false;
    grid.AllowUserToDeleteRows = false;
    grid.ReadOnly = true;
    grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

    // Bind data
    grid.DataSource = customers;

    // Hide ID column
    grid.Columns["Id"].Visible = false;

    // Configure columns
    grid.Columns["Name"].HeaderText = "Customer Name";
    grid.Columns["Email"].HeaderText = "Email Address";
}
```

---

## Best Practices

### ✅ DO

1. **Use async loading** for large datasets
2. **Clear items** before rebinding
3. **Configure columns** after binding
4. **Handle null values**

### ❌ DON'T

1. ❌ Bind to IQueryable
2. ❌ Load synchronously
3. ❌ Forget to dispose

---

**Last Updated**: 2025-11-17
