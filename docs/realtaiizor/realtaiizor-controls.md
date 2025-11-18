# ReaLTaiizor Controls Guide

> **Purpose**: Comprehensive guide to commonly used ReaLTaiizor controls
> **Audience**: WinForms developers using ReaLTaiizor components

---

## 📋 Table of Contents

1. [Material Theme Controls](#material-theme-controls)
2. [Metro Theme Controls](#metro-theme-controls)
3. [Poison Theme Controls](#poison-theme-controls)
4. [Common Control Patterns](#common-control-patterns)
5. [Quick Reference](#quick-reference)

---

## Material Theme Controls

### MaterialButton

```csharp
using ReaLTaiizor.Controls;

// Basic button
var btnSave = new MaterialButton();
btnSave.Text = "Save";
btnSave.Type = MaterialButton.MaterialButtonType.Contained;
btnSave.Click += BtnSave_Click;

// Outlined button
var btnCancel = new MaterialButton();
btnCancel.Text = "Cancel";
btnCancel.Type = MaterialButton.MaterialButtonType.Outlined;
```

### MaterialTextBox

```csharp
// Text input with hint
var txtName = new MaterialTextBox();
txtName.Hint = "Enter customer name...";
txtName.UseSystemPasswordChar = false;

// Password input
var txtPassword = new MaterialTextBox();
txtPassword.Hint = "Enter password...";
txtPassword.UseSystemPasswordChar = true;
txtPassword.Password = true;

// Multiline input
var txtNotes = new MaterialTextBox();
txtNotes.Multiline = true;
txtNotes.Hint = "Enter notes...";
```

### MaterialComboBox

```csharp
// Dropdown
var cboType = new MaterialComboBox();
cboType.Hint = "Select type...";
cboType.Items.AddRange(new object[] { "Type 1", "Type 2", "Type 3" });
cboType.SelectedIndexChanged += CboType_SelectedIndexChanged;

// Get/Set value
string selected = cboType.SelectedItem?.ToString();
cboType.SelectedIndex = 0;
```

### MaterialListView

```csharp
// List view for data display
var lvCustomers = new MaterialListView();
lvCustomers.View = View.Details;
lvCustomers.FullRowSelect = true;
lvCustomers.GridLines = true;

// Add columns
lvCustomers.Columns.Add("ID", 50);
lvCustomers.Columns.Add("Name", 200);
lvCustomers.Columns.Add("Email", 250);

// Add items
var item = new ListViewItem(new[] { "1", "John Doe", "john@example.com" });
lvCustomers.Items.Add(item);
```

### MaterialCheckBox

```csharp
// Checkbox
var chkAgree = new MaterialCheckBox();
chkAgree.Text = "I agree to terms and conditions";
chkAgree.Checked = false;
chkAgree.CheckedChanged += ChkAgree_CheckedChanged;
```

### MaterialRadioButton

```csharp
// Radio buttons
var radioMale = new MaterialRadioButton();
radioMale.Text = "Male";
radioMale.Group = 1;

var radioFemale = new MaterialRadioButton();
radioFemale.Text = "Female";
radioFemale.Group = 1;
```

### MaterialProgressBar

```csharp
// Progress bar
var progressBar = new MaterialProgressBar();
progressBar.Value = 50;
progressBar.Maximum = 100;

// Update progress
progressBar.Value = 75;
```

### MaterialLabel

```csharp
// Label
var lblTitle = new MaterialLabel();
lblTitle.Text = "Customer Information";
lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
```

---

## Metro Theme Controls

### MetroButton

```csharp
using ReaLTaiizor.Controls;

// Metro button
var btnAction = new MetroButton();
btnAction.Text = "Action";
btnAction.Theme = ReaLTaiizor.Enum.Metro.ThemeStyle.Light;
btnAction.Click += BtnAction_Click;
```

### MetroTextBox

```csharp
// Metro text input
var txtInput = new MetroTextBox();
txtInput.WatermarkText = "Enter text...";
txtInput.UseCustomBackColor = true;
txtInput.UseCustomForeColor = true;
```

### MetroComboBox

```csharp
// Metro dropdown
var cboOptions = new MetroComboBox();
cboOptions.Items.AddRange(new object[] { "Option 1", "Option 2", "Option 3" });
cboOptions.Theme = ReaLTaiizor.Enum.Metro.ThemeStyle.Light;
```

### MetroGrid

```csharp
// Metro data grid
var grid = new MetroGrid();
grid.ColumnHeadersVisible = true;
grid.RowHeadersVisible = false;
grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

// Add columns
grid.Columns.Add("colId", "ID");
grid.Columns.Add("colName", "Name");
grid.Columns.Add("colEmail", "Email");

// Add rows
grid.Rows.Add(1, "John Doe", "john@example.com");
```

### MetroCheckBox

```csharp
// Metro checkbox
var chkOption = new MetroCheckBox();
chkOption.Text = "Enable feature";
chkOption.Checked = false;
```

### MetroProgressBar

```csharp
// Metro progress bar
var progress = new MetroProgressBar();
progress.Value = 60;
progress.ProgressBarStyle = ProgressBarStyle.Continuous;
```

---

## Poison Theme Controls

### PoisonButton

```csharp
using ReaLTaiizor.Controls;

// Poison button
var btnSubmit = new PoisonButton();
btnSubmit.Text = "Submit";
btnSubmit.Click += BtnSubmit_Click;
```

### PoisonTextBox

```csharp
// Poison text input
var txtData = new PoisonTextBox();
txtData.PromptText = "Enter data...";
txtData.UseCustomBackColor = true;
```

### PoisonComboBox

```csharp
// Poison dropdown
var cboList = new PoisonComboBox();
cboList.Items.AddRange(new object[] { "Item 1", "Item 2", "Item 3" });
cboList.PromptText = "Select an item...";
```

### PoisonCheckBox

```csharp
// Poison checkbox
var chkEnabled = new PoisonCheckBox();
chkEnabled.Text = "Enabled";
chkEnabled.Checked = true;
```

---

## Common Control Patterns

### Form with Material Controls

```csharp
using ReaLTaiizor.Forms;
using ReaLTaiizor.Controls;

public partial class CustomerEditForm : MaterialForm
{
    // Controls
    private MaterialTextBox txtName;
    private MaterialTextBox txtEmail;
    private MaterialComboBox cboType;
    private MaterialCheckBox chkActive;
    private MaterialButton btnSave;
    private MaterialButton btnCancel;

    public CustomerEditForm()
    {
        InitializeComponent();
        InitializeControls();
    }

    private void InitializeControls()
    {
        // Name
        txtName = new MaterialTextBox
        {
            Location = new Point(20, 80),
            Size = new Size(400, 40),
            Hint = "Customer Name"
        };
        this.Controls.Add(txtName);

        // Email
        txtEmail = new MaterialTextBox
        {
            Location = new Point(20, 130),
            Size = new Size(400, 40),
            Hint = "Email Address"
        };
        this.Controls.Add(txtEmail);

        // Type
        cboType = new MaterialComboBox
        {
            Location = new Point(20, 180),
            Size = new Size(400, 40),
            Hint = "Customer Type"
        };
        cboType.Items.AddRange(new[] { "Regular", "Premium", "VIP" });
        this.Controls.Add(cboType);

        // Active
        chkActive = new MaterialCheckBox
        {
            Location = new Point(20, 230),
            Size = new Size(200, 40),
            Text = "Active"
        };
        this.Controls.Add(chkActive);

        // Save Button
        btnSave = new MaterialButton
        {
            Location = new Point(320, 290),
            Size = new Size(100, 40),
            Text = "Save",
            Type = MaterialButton.MaterialButtonType.Contained
        };
        btnSave.Click += BtnSave_Click;
        this.Controls.Add(btnSave);

        // Cancel Button
        btnCancel = new MaterialButton
        {
            Location = new Point(210, 290),
            Size = new Size(100, 40),
            Text = "Cancel",
            Type = MaterialButton.MaterialButtonType.Outlined
        };
        btnCancel.Click += (s, e) => this.Close();
        this.Controls.Add(btnCancel);
    }

    private void BtnSave_Click(object sender, EventArgs e)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            MessageBox.Show("Name is required", "Validation Error");
            return;
        }

        // Save logic...
    }
}
```

### ListView with Data

```csharp
public void LoadCustomersToListView(List<Customer> customers)
{
    var lvCustomers = new MaterialListView();
    lvCustomers.View = View.Details;
    lvCustomers.FullRowSelect = true;

    // Columns
    lvCustomers.Columns.Add("ID", 60);
    lvCustomers.Columns.Add("Name", 200);
    lvCustomers.Columns.Add("Email", 250);
    lvCustomers.Columns.Add("Type", 100);
    lvCustomers.Columns.Add("Active", 80);

    // Data
    foreach (var customer in customers)
    {
        var item = new ListViewItem(new[]
        {
            customer.Id.ToString(),
            customer.Name,
            customer.Email,
            customer.Type,
            customer.IsActive ? "Yes" : "No"
        });
        lvCustomers.Items.Add(item);
    }
}
```

### ComboBox with Objects

```csharp
public class CustomerType
{
    public int Id { get; set; }
    public string Name { get; set; }

    public override string ToString() => Name;
}

// Setup ComboBox
public void SetupCustomerTypeComboBox()
{
    var types = new List<CustomerType>
    {
        new CustomerType { Id = 1, Name = "Regular" },
        new CustomerType { Id = 2, Name = "Premium" },
        new CustomerType { Id = 3, Name = "VIP" }
    };

    cboCustomerType.DataSource = types;
    cboCustomerType.DisplayMember = "Name";
    cboCustomerType.ValueMember = "Id";
}

// Get selected value
var selectedType = (CustomerType)cboCustomerType.SelectedItem;
int selectedId = (int)cboCustomerType.SelectedValue;
```

---

## Quick Reference

### Control Naming Conventions

| Control | Prefix | Example |
|---------|--------|---------|
| MaterialButton | `btn` | `btnSave`, `btnCancel` |
| MetroButton | `btn` | `btnAction`, `btnSubmit` |
| MaterialTextBox | `txt` | `txtName`, `txtEmail` |
| MetroTextBox | `txt` | `txtInput`, `txtData` |
| MaterialComboBox | `cbo` | `cboType`, `cboStatus` |
| MetroComboBox | `cbo` | `cboOptions`, `cboList` |
| MaterialListView | `lv` | `lvCustomers`, `lvOrders` |
| MetroGrid | `grid` | `gridData`, `gridCustomers` |
| MaterialCheckBox | `chk` | `chkActive`, `chkAgree` |
| MetroCheckBox | `chk` | `chkEnabled`, `chkOption` |
| MaterialLabel | `lbl` | `lblTitle`, `lblStatus` |
| MetroLabel | `lbl` | `lblInfo`, `lblMessage` |

📖 **Full naming conventions**: [realtaiizor-naming-conventions.md](realtaiizor-naming-conventions.md)

### Common Properties

```csharp
// Enable/Disable
control.Enabled = false;

// Show/Hide
control.Visible = false;

// Text
button.Text = "Click Me";
textBox.Text = "Some text";

// Focus
textBox.Focus();

// Clear
textBox.Text = string.Empty;
comboBox.SelectedIndex = -1;
```

---

## Best Practices Summary

### ✅ DO

1. **Use controls from ONE theme** consistently
2. **Set hints/prompts** for better UX
3. **Validate input** before processing
4. **Handle events** properly
5. **Follow naming conventions**
6. **Dispose resources** when done
7. **Follow MVP pattern**

### ❌ DON'T

1. ❌ Mix controls from different themes
2. ❌ Mix ReaLTaiizor with standard WinForms controls
3. ❌ Forget to set hints for text boxes
4. ❌ Skip validation
5. ❌ Put business logic in forms

---

## Next Steps

- **Themes** → [realtaiizor-themes.md](realtaiizor-themes.md)
- **Forms** → [realtaiizor-forms.md](realtaiizor-forms.md)
- **Data Binding** → [realtaiizor-data-binding.md](realtaiizor-data-binding.md)
- **Naming** → [realtaiizor-naming-conventions.md](realtaiizor-naming-conventions.md)

---

## Resources

- **Official GitHub**: https://github.com/Taiizor/ReaLTaiizor
- **Sample Apps**: Check repository for examples
- **Community**: GitHub Issues and Discussions

---

**Last Updated**: 2025-11-17
**ReaLTaiizor Version**: 3.8.0.5+
