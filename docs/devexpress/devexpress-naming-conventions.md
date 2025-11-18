# DevExpress Naming Conventions

> **Purpose**: Standard naming conventions for DevExpress WinForms controls
> **Audience**: WinForms developers using DevExpress components

---

## 📋 Table of Contents

1. [Naming Principles](#naming-principles)
2. [Control Prefixes](#control-prefixes)
3. [Layout Controls](#layout-controls)
4. [Common Examples](#common-examples)
5. [Best Practices](#best-practices)

---

## Naming Principles

### General Rules

1. **Use prefixes** to identify control types
2. **Use PascalCase** for control names (after prefix)
3. **Be descriptive** but concise
4. **Group related controls** with consistent naming

### Format

```
[prefix][EntityName][Property/Purpose]
```

**Examples**:
- `txtCustomerName` - TextEdit for customer name
- `lkeCustomerType` - LookUpEdit for customer type
- `grcCustomers` - GridControl for customers
- `btnSave` - Button for saving

---

## Control Prefixes

### Data Display Controls

| Control | Prefix | Example |
|---------|--------|---------|
| **GridControl** | `grc` | `grcCustomers`, `grcOrders` |
| **GridView** | `grv` | `grvCustomers`, `grvOrders` |
| **TreeList** | `tree` | `treeCategories`, `treeOrgChart` |
| **PivotGrid** | `pivot` | `pivotSales`, `pivotInventory` |
| **ChartControl** | `chart` | `chartSales`, `chartRevenue` |

### Input Controls

| Control | Prefix | Example |
|---------|--------|---------|
| **TextEdit** | `txt` | `txtCustomerName`, `txtEmail` |
| **LookUpEdit** | `lke` | `lkeCustomerType`, `lkeCountry` |
| **DateEdit** | `dte` | `dteCreatedDate`, `dteDueDate` |
| **CheckEdit** | `chk` | `chkIsActive`, `chkAgreeTerms` |
| **SpinEdit** | `spin` | `spinQuantity`, `spinAge` |
| **MemoEdit** | `memo` | `memoNotes`, `memoDescription` |
| **ComboBoxEdit** | `cbo` | `cboStatus`, `cboCategory` |
| **RadioGroup** | `radio` | `radioGender`, `radioPriority` |
| **ImageEdit** | `img` | `imgProfile`, `imgLogo` |
| **TimeEdit** | `time` | `timeStart`, `timeEnd` |
| **ColorEdit** | `color` | `colorBackground`, `colorText` |

### Buttons & Actions

| Control | Prefix | Example |
|---------|--------|---------|
| **SimpleButton** | `btn` | `btnSave`, `btnCancel`, `btnDelete` |
| **DropDownButton** | `ddBtn` | `ddBtnMore`, `ddBtnActions` |
| **BarButtonItem** | `bbi` | `bbiNew`, `bbiSave`, `bbiDelete` |

### Layout Controls

| Control | Prefix | Example |
|---------|--------|---------|
| **LayoutControl** | `lc` | `lcMain`, `lcCustomerEdit` |
| **LayoutControlGroup** | `lcg` | `lcgPersonalInfo`, `lcgAddress` |
| **LayoutControlItem** | `lci` | `lciCustomerName`, `lciEmail` |
| **EmptySpaceItem** | `esi` | `esiSpacer1`, `esiFooter` |
| **SplitterItem** | `spi` | `spiVertical`, `spiHorizontal` |
| **TabbedGroup** | `tab` | `tabMain`, `tabDetails` |
| **LayoutGroup** | `lg` | `lgButtons`, `lgFilters` |

### Navigation Controls

| Control | Prefix | Example |
|---------|--------|---------|
| **NavBarControl** | `nav` | `navMain`, `navSidebar` |
| **NavBarGroup** | `navg` | `navgCustomers`, `navgReports` |
| **NavBarItem** | `navi` | `naviCustomerList`, `naviNewOrder` |
| **TabControl** | `tab` | `tabMain`, `tabCustomer` |
| **TabPage** | `page` | `pageGeneral`, `pageAddress` |
| **AccordionControl** | `acc` | `accMenu`, `accFilters` |

### Container Controls

| Control | Prefix | Example |
|---------|--------|---------|
| **XtraForm** | N/A | `CustomerListForm`, `OrderEditForm` |
| **XtraUserControl** | N/A | `CustomerDetailsControl` |
| **PanelControl** | `panel` | `panelTop`, `panelButtons` |
| **GroupControl** | `group` | `groupPersonalInfo`, `groupAddress` |
| **SplitContainerControl** | `split` | `splitMain`, `splitLeftRight` |

### Advanced Controls

| Control | Prefix | Example |
|---------|--------|---------|
| **RichEditControl** | `rich` | `richEditor`, `richNotes` |
| **SpreadsheetControl** | `sheet` | `sheetData`, `sheetReport` |
| **PdfViewer** | `pdf` | `pdfDocument`, `pdfReport` |
| **GaugeControl** | `gauge` | `gaugeSpeed`, `gaugeProgress` |
| **ProgressBarControl** | `progress` | `progressLoad`, `progressExport` |

### Reporting

| Control | Prefix | Example |
|---------|--------|---------|
| **XtraReport** | N/A | `CustomerReport`, `InvoiceReport` |
| **DetailBand** | `band` | `bandDetail`, `bandGroupHeader` |
| **XRLabel** | `lbl` | `lblCustomerName`, `lblTotal` |
| **XRTable** | `tbl` | `tblOrderItems`, `tblSummary` |

---

## Layout Controls

### Detailed LayoutControl Naming

```csharp
// Form
public partial class CustomerEditForm : XtraForm
{
    // Layout structure
    private LayoutControl lcMain;                   // Main layout
    private LayoutControlGroup lcgRoot;             // Root group

    // Groups
    private LayoutControlGroup lcgPersonalInfo;     // Personal info group
    private LayoutControlGroup lcgAddress;          // Address group
    private LayoutControlGroup lcgButtons;          // Button group

    // Layout items
    private LayoutControlItem lciCustomerName;      // Customer name item
    private LayoutControlItem lciEmail;             // Email item
    private LayoutControlItem lciPhone;             // Phone item
    private LayoutControlItem lciStreet;            // Street item
    private LayoutControlItem lciCity;              // City item
    private LayoutControlItem lciState;             // State item
    private LayoutControlItem lciNotes;             // Notes item

    // Buttons
    private LayoutControlItem lciBtnSave;           // Save button item
    private LayoutControlItem lciBtnCancel;         // Cancel button item

    // Spacers
    private EmptySpaceItem esiFooter;               // Footer spacer
    private EmptySpaceItem esiButtons;              // Button spacer

    // Controls (actual input controls)
    private TextEdit txtCustomerName;
    private TextEdit txtEmail;
    private TextEdit txtPhone;
    private MemoEdit memoNotes;
    private SimpleButton btnSave;
    private SimpleButton btnCancel;
}
```

### Example Hierarchy

```
lcMain (LayoutControl)
└── lcgRoot (LayoutControlGroup)
    ├── lcgPersonalInfo (Group)
    │   ├── lciCustomerName (Item → txtCustomerName)
    │   ├── lciEmail (Item → txtEmail)
    │   └── lciPhone (Item → txtPhone)
    ├── lcgAddress (Group)
    │   ├── lciStreet (Item → txtStreet)
    │   ├── lciCity (Item → txtCity)
    │   └── lciState (Item → lkeState)
    ├── lciNotes (Item → memoNotes)
    └── lcgButtons (Group)
        ├── esiButtons (EmptySpaceItem)
        ├── lciBtnCancel (Item → btnCancel)
        └── lciBtnSave (Item → btnSave)
```

---

## Common Examples

### Customer Edit Form

```csharp
public partial class CustomerEditForm : XtraForm
{
    // Layout
    private LayoutControl lcMain;

    // Personal Info
    private TextEdit txtCustomerName;
    private TextEdit txtFirstName;
    private TextEdit txtLastName;
    private TextEdit txtEmail;
    private TextEdit txtPhone;
    private LookUpEdit lkeCustomerType;
    private CheckEdit chkIsActive;
    private DateEdit dteCreatedDate;

    // Address
    private TextEdit txtStreet;
    private TextEdit txtCity;
    private LookUpEdit lkeState;
    private TextEdit txtZipCode;
    private LookUpEdit lkeCountry;

    // Notes
    private MemoEdit memoNotes;

    // Buttons
    private SimpleButton btnSave;
    private SimpleButton btnCancel;
    private SimpleButton btnDelete;
}
```

### Customer List Form

```csharp
public partial class CustomerListForm : XtraForm
{
    // Grid
    private GridControl grcCustomers;
    private GridView grvCustomers;

    // Filters
    private TextEdit txtSearch;
    private LookUpEdit lkeFilterType;
    private CheckEdit chkShowInactive;
    private DateEdit dteFilterFrom;
    private DateEdit dteFilterTo;

    // Buttons
    private SimpleButton btnNew;
    private SimpleButton btnEdit;
    private SimpleButton btnDelete;
    private SimpleButton btnRefresh;
    private SimpleButton btnExport;

    // Status
    private LabelControl lblTotalRecords;
    private ProgressBarControl progressLoad;
}
```

### Order Entry Form

```csharp
public partial class OrderEditForm : XtraForm
{
    // Header
    private LookUpEdit lkeCustomer;
    private DateEdit dteOrderDate;
    private TextEdit txtOrderNumber;
    private LookUpEdit lkeStatus;

    // Items Grid
    private GridControl grcOrderItems;
    private GridView grvOrderItems;

    // Item Entry
    private LookUpEdit lkeProduct;
    private SpinEdit spinQuantity;
    private SpinEdit spinUnitPrice;
    private TextEdit txtDiscount;
    private SimpleButton btnAddItem;

    // Totals
    private TextEdit txtSubtotal;
    private TextEdit txtTax;
    private TextEdit txtTotal;

    // Notes
    private MemoEdit memoNotes;

    // Actions
    private SimpleButton btnSave;
    private SimpleButton btnCancel;
    private SimpleButton btnPrint;
}
```

### Dashboard Form

```csharp
public partial class DashboardForm : XtraForm
{
    // Charts
    private ChartControl chartSales;
    private ChartControl chartRevenue;
    private GaugeControl gaugePerformance;

    // Grids
    private GridControl grcRecentOrders;
    private GridView grvRecentOrders;
    private GridControl grcTopCustomers;
    private GridView grvTopCustomers;

    // KPIs
    private LabelControl lblTotalSales;
    private LabelControl lblTotalOrders;
    private LabelControl lblAverageOrder;
    private LabelControl lblGrowth;

    // Filters
    private DateEdit dtePeriodFrom;
    private DateEdit dtePeriodTo;
    private SimpleButton btnRefresh;
}
```

---

## Best Practices

### ✅ DO

1. **Use consistent prefixes** across the entire project
2. **Be descriptive** - `txtCustomerEmail` not `txt1`
3. **Group related controls** with consistent naming
4. **Match control and layout item** names:
   - Control: `txtCustomerName`
   - Layout Item: `lciCustomerName`
5. **Use meaningful abbreviations**:
   - `Customer` not `Cust` or `C`
   - `Description` not `Desc`
6. **Follow form naming**: `[Entity][Action]Form`
   - `CustomerEditForm`
   - `OrderListForm`
   - `ReportViewerForm`

```csharp
// ✅ GOOD
private TextEdit txtCustomerName;
private LayoutControlItem lciCustomerName;

// ✅ GOOD
public partial class CustomerEditForm : XtraForm { }
public partial class OrderListForm : XtraForm { }

// ✅ GOOD
private GridControl grcCustomers;
private GridView grvCustomers;
```

### ❌ DON'T

1. ❌ Use default names (`textEdit1`, `simpleButton1`)
2. ❌ Mix naming conventions
3. ❌ Use unclear abbreviations
4. ❌ Forget prefixes
5. ❌ Use Hungarian notation excessively

```csharp
// ❌ BAD
private TextEdit textEdit1;  // Default name
private TextEdit strName;    // Hungarian notation
private TextEdit txt1;       // Not descriptive

// ❌ BAD
public partial class Form1 : XtraForm { }  // Default name

// ❌ BAD - Inconsistent
private GridControl customerGrid;  // No prefix
private GridView grvOrders;        // Has prefix
```

---

## Quick Reference Table

### Most Common Controls

| Control | Prefix | Example |
|---------|--------|---------|
| GridControl | `grc` | `grcCustomers` |
| GridView | `grv` | `grvCustomers` |
| TextEdit | `txt` | `txtCustomerName` |
| LookUpEdit | `lke` | `lkeCustomerType` |
| DateEdit | `dte` | `dteCreatedDate` |
| CheckEdit | `chk` | `chkIsActive` |
| MemoEdit | `memo` | `memoNotes` |
| SimpleButton | `btn` | `btnSave` |
| LayoutControl | `lc` | `lcMain` |
| LayoutControlItem | `lci` | `lciCustomerName` |
| LayoutControlGroup | `lcg` | `lcgPersonalInfo` |

---

## Summary

**Key Rules**:

1. **Always use prefixes** for DevExpress controls
2. **Be consistent** across the project
3. **Be descriptive** - avoid `txt1`, `btn1`
4. **Match layout items** to controls
5. **Use PascalCase** after prefix

**Benefits**:
- ✅ Easy to identify control types
- ✅ Better IntelliSense
- ✅ Easier code maintenance
- ✅ Team consistency

---

## Next Steps

- **Controls Guide** → [devexpress-controls.md](devexpress-controls.md)
- **Layout Guide** → [devexpress-responsive-design.md](devexpress-responsive-design.md)
- **Standard Conventions** → [../../best-practices/naming-conventions.md](../../best-practices/naming-conventions.md)

---

**Last Updated**: 2025-11-17
**DevExpress Version**: 24.1+
