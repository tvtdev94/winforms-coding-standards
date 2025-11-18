# DevExpress WinForms Controls - Overview

> **Purpose**: Introduction to DevExpress controls, setup, and getting started
> **Audience**: WinForms developers using DevExpress components

---

## 📋 Table of Contents

1. [What is DevExpress?](#what-is-devexpress)
2. [Why DevExpress?](#why-devexpress)
3. [Setup & Installation](#setup--installation)
4. [NuGet Packages](#nuget-packages)
5. [License Configuration](#license-configuration)
6. [Getting Started](#getting-started)

---

## What is DevExpress?

**DevExpress WinForms Controls** is a comprehensive suite of professional UI controls for Windows Forms applications.

### Key Components

| Component | Purpose |
|-----------|---------|
| **XtraGrid** | Advanced data grid with filtering, grouping, search |
| **XtraEditors** | Rich input controls (LookUpEdit, TextEdit, DateEdit, etc.) |
| **XtraLayout** | Responsive layout management |
| **XtraNavigation** | Navigation controls (TreeList, NavBar, etc.) |
| **XtraReports** | Professional reporting solution |
| **XtraCharts** | Data visualization and charting |

---

## Why DevExpress?

### ✅ Advantages

1. **Professional UI** - Modern, polished look out of the box
2. **Rich Features** - Advanced filtering, grouping, search, export
3. **Responsive Design** - Built-in support for responsive layouts
4. **Data Binding** - Excellent data binding support
5. **Customization** - Highly customizable appearance and behavior
6. **Performance** - Optimized for large datasets
7. **Documentation** - Extensive documentation and examples
8. **Support** - Professional support from DevExpress team

### ⚠️ Considerations

1. **Cost** - Commercial license required
2. **Learning Curve** - More complex than standard WinForms controls
3. **Package Size** - Larger deployment size
4. **Version Updates** - Need to manage version updates carefully

---

## Setup & Installation

### Prerequisites

- **.NET 8.0** or **.NET 6.0** (recommended)
- **.NET Framework 4.8** (legacy projects)
- **Visual Studio 2022** or **JetBrains Rider**
- **DevExpress License** (trial or commercial)

### Installation Methods

#### Method 1: Using init-project.ps1 (Recommended)

When creating a new project with our init script:

```powershell
# Run the interactive script
.\scripts\init-project.ps1

# Select DevExpress when prompted:
# 4. UI Framework
#    [1] Standard WinForms (default controls)
#    [2] DevExpress (XtraGrid, XtraEditors, XtraLayout, XtraReports)
#    Select UI framework (1-2): 2
```

The script will automatically:
- ✅ Add required DevExpress NuGet packages
- ✅ Configure project for DevExpress
- ✅ Set up proper folder structure

#### Method 2: Manual NuGet Installation

For existing projects:

```bash
# Install core DevExpress packages
dotnet add package DevExpress.WindowsDesktop.Win.Grid
dotnet add package DevExpress.WindowsDesktop.Win.Editors
dotnet add package DevExpress.WindowsDesktop.Win.Layout
dotnet add package DevExpress.WindowsDesktop.Win.Navigation
dotnet add package DevExpress.WindowsDesktop.Win.Reporting
```

---

## NuGet Packages

### Core Packages

| Package | Purpose | When to Use |
|---------|---------|-------------|
| `DevExpress.WindowsDesktop.Win.Grid` | XtraGrid, GridView, GridControl | ✅ Always (data grids) |
| `DevExpress.WindowsDesktop.Win.Editors` | TextEdit, LookUpEdit, DateEdit, etc. | ✅ Always (input controls) |
| `DevExpress.WindowsDesktop.Win.Layout` | LayoutControl, Accordions | ✅ Always (responsive design) |
| `DevExpress.WindowsDesktop.Win.Navigation` | TreeList, NavBar | Use for navigation |
| `DevExpress.WindowsDesktop.Win.Reporting` | XtraReports | Use for reporting features |

### Optional Packages

| Package | Purpose |
|---------|---------|
| `DevExpress.WindowsDesktop.Win.Charts` | XtraCharts for data visualization |
| `DevExpress.WindowsDesktop.Win.Gauges` | Gauges and indicators |
| `DevExpress.WindowsDesktop.Win.PivotGrid` | Pivot tables and OLAP |
| `DevExpress.WindowsDesktop.Win.RichEdit` | Rich text editor |
| `DevExpress.WindowsDesktop.Win.Spreadsheet` | Excel-like spreadsheet |

### Version Management

```xml
<!-- Recommended: Pin to specific version in .csproj -->
<PackageReference Include="DevExpress.WindowsDesktop.Win.Grid" Version="24.1.3" />
<PackageReference Include="DevExpress.WindowsDesktop.Win.Editors" Version="24.1.3" />
<PackageReference Include="DevExpress.WindowsDesktop.Win.Layout" Version="24.1.3" />
```

**⚠️ Important**: Keep all DevExpress packages on the same version to avoid compatibility issues.

---

## License Configuration

### Development License

During development, DevExpress will use your Visual Studio license automatically.

### Deployment License

For deployment, you need to register your license key in code:

```csharp
// Program.cs - Add BEFORE Application.Run()
using DevExpress.Utils;

namespace YourApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Register DevExpress license
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(YourApp));

            // Your existing code
            ApplicationConfiguration.Initialize();
            // ... rest of Program.cs
        }
    }
}
```

### License Key (if required)

Some deployment scenarios require a license key:

```csharp
// Add to Program.cs before any DevExpress controls are created
DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("Office 2019 Colorful");
```

---

## Getting Started

### 1. Create Your First DevExpress Form

```csharp
using DevExpress.XtraEditors;

namespace YourApp.Forms
{
    public partial class CustomerForm : XtraForm  // ✅ Inherit from XtraForm
    {
        public CustomerForm()
        {
            InitializeComponent();
        }
    }
}
```

### 2. Add DevExpress Controls

In the Form Designer:

1. Open **Toolbox**
2. Find **DX.24.1: Common Controls** section
3. Drag controls onto your form:
   - `GridControl` for data grids
   - `TextEdit` for text input
   - `LookUpEdit` for dropdowns
   - `DateEdit` for dates
   - `SimpleButton` for buttons

### 3. Apply DevExpress Theme (Optional)

```csharp
// Program.cs - Before Application.Run()
DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("Office 2019 Colorful");

// Or per-form
public partial class CustomerForm : XtraForm
{
    public CustomerForm()
    {
        InitializeComponent();
        this.LookAndFeel.SetSkinStyle("Office 2019 Colorful");
    }
}
```

### Available Skins

Popular skins for professional apps:
- `Office 2019 Colorful` (Modern, colorful)
- `Office 2019 White` (Clean, minimalist)
- `Office 2019 Black` (Dark theme)
- `Visual Studio 2013 Blue`
- `The Bezier`

---

## Project Structure with DevExpress

```
/ProjectName
├── /Forms              # DevExpress XtraForms
│   ├── MainForm.cs     # Inherit from XtraForm
│   └── CustomerForm.cs
├── /Presenters         # MVP Presenters (same as before)
├── /Services           # Business logic (same as before)
├── /Repositories       # Data access (same as before)
├── /Data               # DbContext (same as before)
└── Program.cs          # DevExpress license registration
```

**Key Change**: Forms inherit from `XtraForm` instead of `Form`.

---

## Quick Start Checklist

When starting a DevExpress project:

- [ ] ✅ Install DevExpress NuGet packages (all same version)
- [ ] ✅ Inherit forms from `XtraForm` instead of `Form`
- [ ] ✅ Register DevExpress license in Program.cs
- [ ] ✅ Apply consistent skin/theme across the app
- [ ] ✅ Use DevExpress controls from Toolbox (DX.24.1 section)
- [ ] ✅ Follow MVP pattern (same as standard WinForms)
- [ ] ✅ Use IFormFactory pattern for dependency injection

---

## Common First Steps

### 1. Create a Grid Form

```csharp
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;

public partial class CustomerListForm : XtraForm
{
    private readonly CustomerPresenter _presenter;

    public CustomerListForm(CustomerPresenter presenter)
    {
        InitializeComponent();
        _presenter = presenter;
    }

    public void SetCustomers(List<Customer> customers)
    {
        gridControl1.DataSource = customers;
    }
}
```

### 2. Configure the Grid

```csharp
private void CustomerListForm_Load(object sender, EventArgs e)
{
    // Configure grid view
    var view = gridControl1.MainView as DevExpress.XtraGrid.Views.Grid.GridView;

    view.OptionsBehavior.Editable = false;
    view.OptionsView.ShowGroupPanel = false;
    view.OptionsFind.AlwaysVisible = true;  // ✅ Built-in search!

    // Hide ID column
    view.Columns["Id"].Visible = false;
}
```

---

## Best Practices

### ✅ DO

1. **Use XtraForm** for all forms (better than standard Form)
2. **Apply consistent skin** across the entire app
3. **Use GridControl** for all data grids
4. **Use LookUpEdit** for dropdowns (better than ComboBox)
5. **Enable built-in search** with `OptionsFind.AlwaysVisible = true`
6. **Follow MVP pattern** (same as standard WinForms)
7. **Keep DevExpress versions in sync**

### ❌ DON'T

1. ❌ Mix standard WinForms controls with DevExpress (inconsistent UI)
2. ❌ Use different DevExpress versions in the same project
3. ❌ Forget to register license for deployment
4. ❌ Skip responsive design (use LayoutControl)
5. ❌ Put business logic in forms (use MVP pattern)

---

## Next Steps

Now that you understand DevExpress basics:

1. **Learn Controls** → [devexpress-controls.md](devexpress-controls.md)
2. **Data Binding** → [devexpress-data-binding.md](devexpress-data-binding.md)
3. **Grid Patterns** → [devexpress-grid-patterns.md](devexpress-grid-patterns.md)
4. **Responsive Design** → [devexpress-responsive-design.md](devexpress-responsive-design.md)
5. **Naming Conventions** → [devexpress-naming-conventions.md](devexpress-naming-conventions.md)

---

## Resources

- **Official Docs**: https://docs.devexpress.com/WindowsForms/
- **Support**: https://supportcenter.devexpress.com/
- **Examples**: Built into DevExpress installation
- **YouTube**: DevExpress WinForms Channel

---

**Last Updated**: 2025-11-17
**DevExpress Version**: 24.1+ (latest stable)
