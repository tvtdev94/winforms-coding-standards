# DevExpress WinForms Controls Guide

> **Purpose**: Comprehensive guide to commonly used DevExpress controls
> **Audience**: WinForms developers using DevExpress components

---

## 📋 Table of Contents

1. [XtraGrid (Data Grid)](#xtragrid-data-grid)
2. [XtraEditors (Input Controls)](#xtraeditors-input-controls)
3. [XtraLayout (Responsive Layout)](#xtralayout-responsive-layout)
4. [XtraNavigation (Navigation)](#xtranavigation-navigation)
5. [XtraReports (Reporting)](#xtrareports-reporting)
6. [Buttons & Actions](#buttons--actions)
7. [Quick Reference](#quick-reference)

---

## XtraGrid (Data Grid)

### GridControl + GridView

The most commonly used DevExpress control for displaying tabular data.

**Components**:
- `GridControl` - Container control
- `GridView` - Main view for grid-style display
- `GridColumn` - Individual columns

### Basic Setup

```csharp
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

public partial class CustomerListForm : XtraForm
{
    private void InitializeGrid()
    {
        // Get the GridView
        var gridView = gridControl1.MainView as GridView;

        // Basic configuration
        gridView.OptionsBehavior.Editable = false;  // Read-only
        gridView.OptionsView.ShowGroupPanel = false; // Hide group panel
        gridView.OptionsSelection.MultiSelect = false; // Single selection

        // Enable built-in search
        gridView.OptionsFind.AlwaysVisible = true;
        gridView.OptionsFind.FindNullPrompt = "Search...";
    }
}
```

### Data Binding

```csharp
// Simple binding
gridControl1.DataSource = customerList;

// Or with BindingSource
var bindingSource = new BindingSource();
bindingSource.DataSource = customerList;
gridControl1.DataSource = bindingSource;
```

### Column Configuration

```csharp
private void ConfigureColumns()
{
    var gridView = gridControl1.MainView as GridView;

    // Hide columns
    gridView.Columns["Id"].Visible = false;
    gridView.Columns["CreatedDate"].Visible = false;

    // Set column headers
    gridView.Columns["Name"].Caption = "Customer Name";
    gridView.Columns["Email"].Caption = "Email Address";

    // Set column widths
    gridView.Columns["Name"].Width = 200;
    gridView.Columns["Email"].Width = 250;

    // Set column order
    gridView.Columns["Name"].VisibleIndex = 0;
    gridView.Columns["Email"].VisibleIndex = 1;
    gridView.Columns["Phone"].VisibleIndex = 2;

    // Read-only specific columns
    gridView.Columns["Id"].OptionsColumn.AllowEdit = false;

    // Format columns
    gridView.Columns["CreatedDate"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
    gridView.Columns["CreatedDate"].DisplayFormat.FormatString = "yyyy-MM-dd HH:mm";
}
```

### Built-in Features

```csharp
private void EnableAdvancedFeatures()
{
    var gridView = gridControl1.MainView as GridView;

    // ✅ Filtering
    gridView.OptionsView.ShowAutoFilterRow = true;

    // ✅ Grouping
    gridView.OptionsView.ShowGroupPanel = true;
    gridView.GroupSummary.Add(DevExpress.Data.SummaryItemType.Count, "Name", null, "(Count: {0})");

    // ✅ Export
    // Add buttons for export (Excel, PDF, CSV)
}
```

### Handling Events

```csharp
private void gridView1_RowClick(object sender, RowClickEventArgs e)
{
    if (e.Clicks == 2) // Double-click
    {
        var customer = gridView1.GetFocusedRow() as Customer;
        if (customer != null)
        {
            OpenCustomerForm(customer.Id);
        }
    }
}

private void gridView1_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
{
    var customer = gridView1.GetFocusedRow() as Customer;
    UpdateDetailsPanel(customer);
}
```

### Best Practices

**✅ DO**:
- Use `AsNoTracking()` when loading data for read-only grids
- Enable built-in search (`OptionsFind.AlwaysVisible = true`)
- Hide technical columns (Id, CreatedDate, etc.)
- Set meaningful column captions
- Use `BestFitColumns()` after binding data

**❌ DON'T**:
- Don't allow editing directly in grid (use edit forms instead)
- Don't bind to IQueryable (use List or BindingSource)
- Don't forget to handle null values

---

## XtraEditors (Input Controls)

### TextEdit - Text Input

Replacement for standard TextBox.

```csharp
// Basic setup
txtCustomerName.Properties.NullText = "Enter customer name...";
txtCustomerName.Properties.MaxLength = 100;

// Validation
txtCustomerName.Properties.Appearance.BackColor = Color.Yellow; // Highlight required

// Mask input (e.g., phone)
txtPhone.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Simple;
txtPhone.Properties.Mask.EditMask = "(000) 000-0000";
txtPhone.Properties.Mask.UseMaskAsDisplayFormat = true;
```

### LookUpEdit - Dropdown (ComboBox Replacement)

Much better than standard ComboBox.

```csharp
// Setup data source
lkeCustomerType.Properties.DataSource = customerTypes;
lkeCustomerType.Properties.DisplayMember = "Name";
lkeCustomerType.Properties.ValueMember = "Id";

// Configure columns
lkeCustomerType.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Type"));
lkeCustomerType.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Description"));

// Allow null selection
lkeCustomerType.Properties.NullText = "-- Select Type --";

// Enable search
lkeCustomerType.Properties.SearchMode = SearchMode.AutoFilter;

// Get/Set value
int selectedTypeId = (int)lkeCustomerType.EditValue;
lkeCustomerType.EditValue = customerId;
```

### DateEdit - Date Picker

```csharp
// Basic setup
dteCreatedDate.Properties.DisplayFormat.FormatString = "yyyy-MM-dd";
dteCreatedDate.Properties.EditFormat.FormatString = "yyyy-MM-dd";
dteCreatedDate.Properties.Mask.EditMask = "yyyy-MM-dd";

// Allow null dates
dteCreatedDate.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;

// Set min/max dates
dteCreatedDate.Properties.MinValue = DateTime.Today.AddYears(-1);
dteCreatedDate.Properties.MaxValue = DateTime.Today;

// Get/Set value
DateTime? selectedDate = dteCreatedDate.EditValue as DateTime?;
dteCreatedDate.EditValue = DateTime.Now;
```

### CheckEdit - Checkbox

```csharp
// Basic setup
chkIsActive.Text = "Is Active";
chkIsActive.Checked = true;

// Get/Set value
bool isActive = chkIsActive.Checked;
chkIsActive.Checked = customer.IsActive;
```

### MemoEdit - Multi-line Text

```csharp
// Setup multi-line text input
memoNotes.Properties.MaxLength = 1000;
memoNotes.Properties.ScrollBars = ScrollBars.Vertical;

// Get/Set value
string notes = memoNotes.Text;
memoNotes.Text = customer.Notes;
```

### SpinEdit - Numeric Input

```csharp
// Integer input
spinAge.Properties.MinValue = 0;
spinAge.Properties.MaxValue = 150;
spinAge.Properties.IsFloatValue = false;

// Decimal input (e.g., price)
spinPrice.Properties.MinValue = 0;
spinPrice.Properties.MaxValue = 999999;
spinPrice.Properties.IsFloatValue = true;
spinPrice.Properties.DisplayFormat.FormatString = "c2"; // Currency
```

---

## XtraLayout (Responsive Layout)

### LayoutControl - Responsive Container

Professional responsive layout management.

```csharp
// Basic setup
layoutControl1.AllowCustomization = false; // Prevent runtime customization

// Access layout items
var layoutItemName = layoutControl1.Root.Items.FindByName("layoutItemCustomerName");
layoutItemName.Text = "Customer Name:";
layoutItemName.TextLocation = DevExpress.Utils.Locations.Left;

// Required field indicator
layoutItemName.AppearanceItemCaption.ForeColor = Color.Red;
layoutItemName.Text = "Customer Name:*";

// Hide/show items
layoutItemName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
```

### Form Layout Example

```csharp
// Typical form layout with LayoutControl
// In Designer:
// 1. Add LayoutControl to form
// 2. Add controls (TextEdit, LookUpEdit, etc.) to LayoutControl
// 3. Layout items auto-created
// 4. Use Layout Designer to arrange

// Code access:
private void ConfigureLayout()
{
    // Group related items
    var groupPersonalInfo = new LayoutControlGroup();
    groupPersonalInfo.Text = "Personal Information";
    layoutControl1.Root.AddGroup(groupPersonalInfo);

    // Move items to group
    groupPersonalInfo.Add(layoutItemName);
    groupPersonalInfo.Add(layoutItemEmail);
}
```

---

## XtraNavigation (Navigation)

### TreeList - Hierarchical Data

```csharp
// Setup hierarchical data
treeList1.DataSource = categories;
treeList1.KeyFieldName = "Id";
treeList1.ParentFieldName = "ParentId";

// Configure columns
treeList1.Columns["Id"].Visible = false;
treeList1.Columns["Name"].Caption = "Category";

// Expand all nodes
treeList1.ExpandAll();

// Get selected node
var focusedNode = treeList1.FocusedNode;
var category = treeList1.GetDataRecordByNode(focusedNode) as Category;
```

### NavBarControl - Navigation Panel

```csharp
// Add groups and items programmatically
var group = new NavBarGroup("Customers");
navBarControl1.Groups.Add(group);

var item = new NavBarItem("Customer List");
group.ItemLinks.Add(item);

// Handle click
private void navBarControl1_LinkClicked(object sender, NavBarLinkEventArgs e)
{
    if (e.Link.Caption == "Customer List")
    {
        OpenCustomerList();
    }
}
```

---

## XtraReports (Reporting)

### Creating a Report

```csharp
// Create report class
using DevExpress.XtraReports.UI;

public partial class CustomerReport : XtraReport
{
    public CustomerReport()
    {
        InitializeComponent();
    }

    public void SetDataSource(List<Customer> customers)
    {
        this.DataSource = customers;
    }
}

// Show report
private void ShowReport()
{
    var report = new CustomerReport();
    report.SetDataSource(customers);

    // Preview
    var tool = new DevExpress.XtraReports.UI.ReportPrintTool(report);
    tool.ShowPreview();

    // Or export to PDF
    report.ExportToPdf("customers.pdf");
}
```

---

## Buttons & Actions

### SimpleButton

```csharp
// Better than standard Button
btnSave.Text = "Save";
btnSave.ImageOptions.Image = Properties.Resources.save_icon;
btnSave.ImageOptions.ImageToTextAlignment = ImageAlignToText.LeftCenter;

// Styling
btnSave.Appearance.BackColor = Color.FromArgb(0, 122, 204);
btnSave.Appearance.ForeColor = Color.White;
```

### BarManager - Toolbars & Menus

```csharp
// Add toolbar items
var barManager = new DevExpress.XtraBars.BarManager();
barManager.Form = this;

var bar = new DevExpress.XtraBars.Bar();
bar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;

var btnNew = new DevExpress.XtraBars.BarButtonItem();
btnNew.Caption = "New";
btnNew.ItemClick += BtnNew_ItemClick;
```

---

## Quick Reference

### Control Naming Conventions

| Control | Prefix | Example |
|---------|--------|---------|
| GridControl | `grc` | `grcCustomers` |
| GridView | `grv` | `grvCustomers` |
| TextEdit | `txt` | `txtCustomerName` |
| LookUpEdit | `lke` | `lkeCustomerType` |
| DateEdit | `dte` | `dteCreatedDate` |
| CheckEdit | `chk` | `chkIsActive` |
| MemoEdit | `memo` | `memoNotes` |
| SpinEdit | `spin` | `spinAge` |
| SimpleButton | `btn` | `btnSave` |
| LayoutControl | `lc` | `lcMain` |
| LayoutControlItem | `lci` | `lciCustomerName` |
| TreeList | `tree` | `treeCategories` |

📖 **Full naming conventions**: [devexpress-naming-conventions.md](devexpress-naming-conventions.md)

### Common Properties

```csharp
// Enable/Disable
control.Enabled = false;

// Show/Hide
control.Visible = false;

// Read-only
textEdit1.Properties.ReadOnly = true;

// Focus
textEdit1.Focus();

// Clear
textEdit1.EditValue = null;

// Validate
if (string.IsNullOrWhiteSpace(txtName.Text))
{
    dxErrorProvider1.SetError(txtName, "Name is required");
}
```

---

## Best Practices Summary

### ✅ DO

1. **Use DevExpress equivalents** instead of standard controls
2. **Enable built-in features** (search, filter, export)
3. **Apply consistent naming** (use prefixes)
4. **Configure columns** (hide ID, set captions, widths)
5. **Use LayoutControl** for responsive forms
6. **Handle validation** with error providers
7. **Follow MVP pattern** (no business logic in forms)

### ❌ DON'T

1. ❌ Mix standard WinForms controls with DevExpress
2. ❌ Allow inline editing in grids (use edit forms)
3. ❌ Forget to set NullText for better UX
4. ❌ Skip column configuration
5. ❌ Put business logic in control event handlers

---

## Next Steps

- **Data Binding** → [devexpress-data-binding.md](devexpress-data-binding.md)
- **Grid Patterns** → [devexpress-grid-patterns.md](devexpress-grid-patterns.md)
- **Responsive Design** → [devexpress-responsive-design.md](devexpress-responsive-design.md)
- **Naming Conventions** → [devexpress-naming-conventions.md](devexpress-naming-conventions.md)

---

## Resources

- **Official Docs**: https://docs.devexpress.com/WindowsForms/114566/controls-and-libraries
- **Control Examples**: Built into DevExpress Demo Center
- **YouTube**: DevExpress Controls Tutorials

---

**Last Updated**: 2025-11-17
**DevExpress Version**: 24.1+
