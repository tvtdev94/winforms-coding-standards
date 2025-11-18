# DevExpress Responsive Design Guide

> **Purpose**: Creating responsive and adaptive WinForms layouts with DevExpress
> **Audience**: WinForms developers building professional, responsive applications

---

## 📋 Table of Contents

1. [Why Responsive Design?](#why-responsive-design)
2. [LayoutControl Basics](#layoutcontrol-basics)
3. [Responsive Layout Patterns](#responsive-layout-patterns)
4. [Anchoring & Docking](#anchoring--docking)
5. [Dynamic Resizing](#dynamic-resizing)
6. [Best Practices](#best-practices)

---

## Why Responsive Design?

Modern WinForms applications need to work on:
- **Different screen sizes** (1920x1080, 1366x768, 4K displays)
- **Different DPI settings** (100%, 125%, 150%, 200%)
- **Resizable windows** (users expect to resize forms)

**Standard WinForms Problems**:
❌ Fixed pixel positions
❌ Overlapping controls when resized
❌ Horizontal scrollbars appearing
❌ Text cutoff on different DPI

**DevExpress Solution**:
✅ LayoutControl - Professional responsive layout
✅ Automatic DPI scaling
✅ Flexible sizing and spacing
✅ Clean, professional appearance

---

## LayoutControl Basics

### What is LayoutControl?

`LayoutControl` is DevExpress's container for creating responsive, auto-sizing layouts.

**Key Concepts**:
- **LayoutControl** - The container
- **LayoutControlGroup** - Groups items together
- **LayoutControlItem** - Wraps individual controls
- **EmptySpaceItem** - Creates spacing

### Basic Setup

```csharp
using DevExpress.XtraLayout;

public partial class CustomerEditForm : XtraForm
{
    public CustomerEditForm()
    {
        InitializeComponent();
        ConfigureLayout();
    }

    private void ConfigureLayout()
    {
        // Disable customization at runtime
        layoutControl1.AllowCustomization = false;

        // Root group settings
        layoutControl1.Root.GroupBordersVisible = false;
        layoutControl1.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(10);
    }
}
```

### Adding Controls

**In Designer**:
1. Add `LayoutControl` to form
2. Add controls (TextEdit, LookUpEdit, etc.) **to LayoutControl**
3. DevExpress auto-creates LayoutControlItems
4. Use Layout Designer to arrange

**In Code**:
```csharp
private void AddControlToLayout()
{
    // Create control
    var txtName = new TextEdit();

    // Create layout item
    var layoutItem = layoutControl1.Root.AddItem();
    layoutItem.Control = txtName;
    layoutItem.Text = "Customer Name:";
    layoutItem.TextLocation = DevExpress.Utils.Locations.Left;
    layoutItem.TextAlignMode = TextAlignModeItem.CustomSize;
    layoutItem.TextSize = new Size(120, 0);
}
```

---

## Responsive Layout Patterns

### Pattern 1: Simple Form Layout

```
┌──────────────────────────────────┐
│ Customer Name:  [____________]   │
│ Email:          [____________]   │
│ Phone:          [____________]   │
│ Address:        [____________]   │
│                                  │
│           [Cancel]  [Save]       │
└──────────────────────────────────┘
```

**Setup**:
```csharp
private void ConfigureSimpleFormLayout()
{
    var root = layoutControl1.Root;

    // Set label width for all items
    foreach (BaseLayoutItem item in root.Items)
    {
        if (item is LayoutControlItem lci)
        {
            lci.TextLocation = DevExpress.Utils.Locations.Left;
            lci.TextAlignMode = TextAlignModeItem.CustomSize;
            lci.TextSize = new Size(120, 0);  // Fixed label width
        }
    }

    // Make text fields fill available width
    lciCustomerName.SizeConstraintsType = SizeConstraintsType.Default;
    lciEmail.SizeConstraintsType = SizeConstraintsType.Default;
    lciPhone.SizeConstraintsType = SizeConstraintsType.Default;

    // Button row
    var buttonGroup = new LayoutControlGroup();
    buttonGroup.GroupBordersVisible = false;
    buttonGroup.LayoutMode = LayoutMode.Flow;
    buttonGroup.FlowDirection = FlowDirection.RightToLeft;

    root.AddGroup(buttonGroup);
    buttonGroup.Move(lciBtnSave, buttonGroup, 0);
    buttonGroup.Move(lciBtnCancel, buttonGroup, 1);
}
```

### Pattern 2: Two-Column Layout

```
┌──────────────────────────────────┐
│ First Name:  [_____]  Last Name: [_____] │
│ Email:       [_____]  Phone:     [_____] │
│ City:        [_____]  State:     [_____] │
└──────────────────────────────────┘
```

**Setup**:
```csharp
private void ConfigureTwoColumnLayout()
{
    var root = layoutControl1.Root;
    root.LayoutMode = LayoutMode.Table;

    // Define 2 columns
    var col1 = root.OptionsTableLayoutGroup.AddColumn();
    col1.Width = new ColumnDefinition { SizeType = SizeType.Percent, Width = 50 };

    var col2 = root.OptionsTableLayoutGroup.AddColumn();
    col2.Width = new ColumnDefinition { SizeType = SizeType.Percent, Width = 50 };

    // Position items
    lciFirstName.Move(lciFirstName, root, 0, 0); // Row 0, Col 0
    lciLastName.Move(lciLastName, root, 0, 1);   // Row 0, Col 1
    lciEmail.Move(lciEmail, root, 1, 0);         // Row 1, Col 0
    lciPhone.Move(lciPhone, root, 1, 1);         // Row 1, Col 1
}
```

### Pattern 3: Grouped Sections

```
┌──────────────────────────────────┐
│ ┌─ Personal Information ───────┐ │
│ │ Name:    [____________]      │ │
│ │ Email:   [____________]      │ │
│ └──────────────────────────────┘ │
│                                  │
│ ┌─ Address ────────────────────┐ │
│ │ Street:  [____________]      │ │
│ │ City:    [____________]      │ │
│ └──────────────────────────────┘ │
└──────────────────────────────────┘
```

**Setup**:
```csharp
private void ConfigureGroupedLayout()
{
    var root = layoutControl1.Root;

    // Personal Info Group
    var groupPersonal = new LayoutControlGroup();
    groupPersonal.Text = "Personal Information";
    groupPersonal.GroupBordersVisible = true;
    root.AddGroup(groupPersonal);

    // Move items to group
    groupPersonal.Move(lciName, groupPersonal);
    groupPersonal.Move(lciEmail, groupPersonal);

    // Address Group
    var groupAddress = new LayoutControlGroup();
    groupAddress.Text = "Address";
    groupAddress.GroupBordersVisible = true;
    root.AddGroup(groupAddress);

    groupAddress.Move(lciStreet, groupAddress);
    groupAddress.Move(lciCity, groupAddress);
}
```

### Pattern 4: Master-Detail Layout

```
┌──────────────────────────────────┐
│ ┌─ Customers ──────────────────┐ │
│ │ [Grid showing customers]     │ │
│ │                              │ │
│ └──────────────────────────────┘ │
│                                  │
│ ┌─ Details ────────────────────┐ │
│ │ Name:  [____________]        │ │
│ │ Email: [____________]        │ │
│ └──────────────────────────────┘ │
└──────────────────────────────────┘
```

**Setup**:
```csharp
private void ConfigureMasterDetailLayout()
{
    var root = layoutControl1.Root;
    root.LayoutMode = LayoutMode.Table;

    // 2 rows: 70% for grid, 30% for details
    var row1 = root.OptionsTableLayoutGroup.AddRow();
    row1.Height = new RowDefinition { SizeType = SizeType.Percent, Height = 70 };

    var row2 = root.OptionsTableLayoutGroup.AddRow();
    row2.Height = new RowDefinition { SizeType = SizeType.Percent, Height = 30 };

    // Master group (grid)
    var groupMaster = new LayoutControlGroup();
    groupMaster.Text = "Customers";
    root.AddGroup(groupMaster, 0, 0); // Row 0
    groupMaster.Move(lciGrid, groupMaster);

    // Detail group
    var groupDetail = new LayoutControlGroup();
    groupDetail.Text = "Details";
    root.AddGroup(groupDetail, 1, 0); // Row 1
    groupDetail.Move(lciName, groupDetail);
    groupDetail.Move(lciEmail, groupDetail);
}
```

---

## Anchoring & Docking

### Anchoring Controls

```csharp
// Stretch horizontally
lciCustomerName.SizeConstraintsType = SizeConstraintsType.Default;

// Fixed width
lciCustomerName.SizeConstraintsType = SizeConstraintsType.Custom;
lciCustomerName.MaxSize = new Size(300, 0);
lciCustomerName.MinSize = new Size(200, 0);

// Fill available space
lciMemoNotes.SizeConstraintsType = SizeConstraintsType.Default;
txtNotes.Dock = DockStyle.Fill;
```

### Docking Panels

```csharp
// Left panel (fixed width)
var panelLeft = new Panel();
panelLeft.Dock = DockStyle.Left;
panelLeft.Width = 250;

// Fill remaining space
var panelMain = new Panel();
panelMain.Dock = DockStyle.Fill;

// Bottom panel (fixed height)
var panelBottom = new Panel();
panelBottom.Dock = DockStyle.Bottom;
panelBottom.Height = 50;

// Add in order: Bottom, Left, Fill
this.Controls.Add(panelMain);   // Last = Fill
this.Controls.Add(panelLeft);   // Second
this.Controls.Add(panelBottom); // First
```

---

## Dynamic Resizing

### Responsive Grid Height

```csharp
private void CustomerListForm_Resize(object sender, EventArgs e)
{
    // Grid takes 70% of available height
    int availableHeight = this.ClientSize.Height - panelTop.Height - panelBottom.Height;
    gridControl1.Height = (int)(availableHeight * 0.7);

    // Details panel takes remaining 30%
    panelDetails.Height = availableHeight - gridControl1.Height;
}
```

### Auto-Resize Columns

```csharp
private void ConfigureAutoResizeGrid()
{
    var gridView = gridControl1.MainView as GridView;

    // Auto-width columns
    gridView.OptionsView.ColumnAutoWidth = true;

    // Or best fit
    gridView.BestFitColumns();

    // Or specific columns fill
    gridView.Columns["Name"].Width = 200;
    gridView.Columns["Email"].Width = 250;
    gridView.Columns["Notes"].MinWidth = 100;
    gridView.Columns["Notes"].MaxWidth = 500;
}
```

### Splitter Control

```csharp
// Add splitter between panels
var splitter = new Splitter();
splitter.Dock = DockStyle.Left;
splitter.Width = 3;

// Order: Left Panel -> Splitter -> Fill Panel
this.Controls.Add(panelFill);
this.Controls.Add(splitter);
this.Controls.Add(panelLeft);
```

---

## Best Practices

### ✅ DO

1. **Use LayoutControl** for forms with multiple inputs
2. **Set consistent label widths** (120-150px)
3. **Group related fields** with LayoutControlGroup
4. **Use table layout** for multi-column forms
5. **Test on different DPI** (100%, 125%, 150%)
6. **Make grids fill available space**
7. **Set minimum form size**

```csharp
// ✅ GOOD: Set minimum size
this.MinimumSize = new Size(800, 600);

// ✅ GOOD: Use LayoutControl
var layoutControl = new LayoutControl();
layoutControl.Dock = DockStyle.Fill;

// ✅ GOOD: Consistent label width
foreach (var item in layoutControl.Root.Items.OfType<LayoutControlItem>())
{
    item.TextSize = new Size(120, 0);
}
```

### ❌ DON'T

1. ❌ Use fixed pixel positions
2. ❌ Mix LayoutControl with manual positioning
3. ❌ Forget to test on different screen sizes
4. ❌ Allow forms to be too small
5. ❌ Use too many nested groups

```csharp
// ❌ BAD: Fixed position
txtName.Location = new Point(100, 50);
txtName.Size = new Size(200, 20);

// ❌ BAD: No minimum size
// Form can be resized to 1x1 pixel!

// ❌ BAD: Not responsive
txtName.Width = 200; // Fixed width
```

---

## Complete Example

### Responsive Customer Edit Form

```csharp
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;

public partial class CustomerEditForm : XtraForm
{
    public CustomerEditForm()
    {
        InitializeComponent();
        InitializeLayout();
    }

    private void InitializeLayout()
    {
        // Set form minimum size
        this.MinimumSize = new Size(600, 400);
        this.Size = new Size(800, 500);

        // Configure layout control
        layoutControl1.Dock = DockStyle.Fill;
        layoutControl1.AllowCustomization = false;

        var root = layoutControl1.Root;
        root.GroupBordersVisible = false;
        root.Padding = new DevExpress.XtraLayout.Utils.Padding(10);

        // Set consistent label width
        foreach (var item in root.Items.OfType<LayoutControlItem>())
        {
            item.TextLocation = DevExpress.Utils.Locations.Left;
            item.TextAlignMode = TextAlignModeItem.CustomSize;
            item.TextSize = new Size(120, 0);
        }

        // Create groups
        var groupPersonal = new LayoutControlGroup();
        groupPersonal.Text = "Personal Information";
        groupPersonal.GroupBordersVisible = true;
        root.AddGroup(groupPersonal);

        groupPersonal.Move(lciName, groupPersonal);
        groupPersonal.Move(lciEmail, groupPersonal);
        groupPersonal.Move(lciPhone, groupPersonal);

        var groupAddress = new LayoutControlGroup();
        groupAddress.Text = "Address";
        groupAddress.GroupBordersVisible = true;
        root.AddGroup(groupAddress);

        groupAddress.Move(lciStreet, groupAddress);
        groupAddress.Move(lciCity, groupAddress);
        groupAddress.Move(lciState, groupAddress);

        // Notes (full width)
        lciNotes.TextLocation = DevExpress.Utils.Locations.Top;
        txtNotes.Properties.ScrollBars = ScrollBars.Vertical;
        lciNotes.SizeConstraintsType = SizeConstraintsType.Custom;
        lciNotes.MinSize = new Size(0, 100);
        lciNotes.MaxSize = new Size(0, 200);

        // Button panel
        CreateButtonPanel();
    }

    private void CreateButtonPanel()
    {
        var buttonPanel = new Panel();
        buttonPanel.Dock = DockStyle.Bottom;
        buttonPanel.Height = 50;
        buttonPanel.Padding = new Padding(10);

        var btnSave = new SimpleButton { Text = "Save", Width = 100 };
        btnSave.Dock = DockStyle.Right;
        btnSave.Click += BtnSave_Click;

        var btnCancel = new SimpleButton { Text = "Cancel", Width = 100 };
        btnCancel.Dock = DockStyle.Right;
        btnCancel.Click += BtnCancel_Click;

        buttonPanel.Controls.Add(btnSave);
        buttonPanel.Controls.Add(btnCancel);

        this.Controls.Add(buttonPanel);
    }
}
```

---

## Testing Responsive Design

### Test Checklist

- [ ] Test on 1920x1080 (100% DPI)
- [ ] Test on 1366x768 (100% DPI)
- [ ] Test on 125% DPI
- [ ] Test on 150% DPI
- [ ] Resize form to minimum size
- [ ] Resize form to maximum size
- [ ] Check all text is visible
- [ ] Check no horizontal scrollbars
- [ ] Check button alignment
- [ ] Check field alignment

### DPI Testing

```csharp
// Enable DPI awareness in Program.cs
[STAThread]
static void Main()
{
    Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
    ApplicationConfiguration.Initialize();

    // ... rest of code
}
```

---

## Summary

**Key Takeaways**:

1. **Use LayoutControl** for responsive forms
2. **Test on multiple screen sizes and DPI**
3. **Set minimum form size**
4. **Use table layout** for multi-column forms
5. **Group related fields** for better organization
6. **Make grids fill available space**

---

## Next Steps

- **Controls Guide** → [devexpress-controls.md](devexpress-controls.md)
- **Grid Patterns** → [devexpress-grid-patterns.md](devexpress-grid-patterns.md)
- **Data Binding** → [devexpress-data-binding.md](devexpress-data-binding.md)

---

## Resources

- **Official Docs**: https://docs.devexpress.com/WindowsForms/116682/controls-and-libraries/forms-and-user-controls/layout-control
- **DPI Awareness**: https://docs.devexpress.com/WindowsForms/118717/common-features/high-dpi-support

---

**Last Updated**: 2025-11-17
**DevExpress Version**: 24.1+
