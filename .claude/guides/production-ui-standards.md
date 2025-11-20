# Production-Level UI Standards for WinForms

> **Purpose**: Ensure AI generates production-quality UI, not student-level demos
> **Version**: 1.0.0
> **Last Updated**: 2025-01-20

---

## ⚠️ CRITICAL: This is NOT Optional

Every UI element you create MUST follow these standards. A DataGridView without sorting/filtering/paging is **UNACCEPTABLE**. A button that blends into the background is **UNACCEPTABLE**.

**Before completing ANY UI task, verify against the checklist at the end of this document.**

### 🎨 DEFAULT UI FRAMEWORK: Material Design (ReaLTaiizor)

**ALWAYS use Material Design theme by default** (unless project specifies otherwise):

```csharp
// ✅ DEFAULT - Use Material theme
using ReaLTaiizor.Forms;
using ReaLTaiizor.Controls;

public class CustomerForm : MaterialForm
{
    private MaterialTextBoxEdit txtName;    // With floating label
    private MaterialButton btnSave;
    private MaterialComboBox cboStatus;
}
```

**Why Material Design:**
- ✅ Modern, clean, professional look
- ✅ Floating labels save space
- ✅ Consistent color system
- ✅ Good animations and feedback
- ✅ Free and open-source (ReaLTaiizor)

📖 **Docs**: [docs/ui/realtaiizor/material-theme.md](../../docs/ui/realtaiizor/material-theme.md)

### 🚨 CRITICAL RULE: UI Control Consistency

**NEVER mix different UI control libraries in the same application!**

| Project Type | Use ONLY | ❌ NEVER Mix With |
|-------------|----------|-------------------|
| **Standard WinForms** | TextBox, Button, DataGridView, ComboBox | DevExpress, ReaLTaiizor |
| **DevExpress** | TextEdit, SimpleButton, GridControl, LookUpEdit | Standard WinForms, ReaLTaiizor |
| **ReaLTaiizor** | MaterialTextBox, MaterialButton, PoisonDataGridView | Standard WinForms, DevExpress |

```csharp
// ❌ WRONG - Mixed controls
public partial class CustomerForm : Form
{
    private TextBox txtName;           // Standard WinForms
    private SimpleButton btnSave;      // DevExpress
    private MaterialTextBox txtEmail;  // ReaLTaiizor
}

// ✅ CORRECT - All DevExpress
public partial class CustomerForm : XtraForm
{
    private TextEdit txtName;          // DevExpress
    private SimpleButton btnSave;      // DevExpress
    private TextEdit txtEmail;         // DevExpress
}
```

**Why this matters**:
- ❌ Inconsistent look and feel
- ❌ Different styling/theming systems conflict
- ❌ Confusing user experience
- ❌ Maintenance nightmare

---

## Table of Contents

1. [Color Palette & Theming](#1-color-palette--theming)
2. [Data Display Controls](#2-data-display-controls)
3. [Input Controls](#3-input-controls)
4. [Buttons and Actions](#4-buttons-and-actions)
5. [Form Layout & Responsive Design](#5-form-layout--responsive-design)
6. [Feedback & Status Communication](#6-feedback--status-communication)
7. [Validation & Error Handling](#7-validation--error-handling)
8. [Accessibility (WCAG)](#8-accessibility-wcag)
9. [Performance & Optimization](#9-performance--optimization)
10. [Internationalization](#10-internationalization)
11. [Professional Polish](#11-professional-polish)
12. [Navigation & Workflow](#12-navigation--workflow)
13. [Security & Data Protection](#13-security--data-protection)
14. [Production Checklist](#14-production-checklist)

---

## 1. Color Palette & Theming

### 1.1 Professional Color Schemes

**NEVER use random colors. ALWAYS use a defined palette.**

#### Recommended Palettes

**Modern Professional (Default)**
```csharp
public static class AppColors
{
    // Primary colors
    public static readonly Color Primary = Color.FromArgb(41, 128, 185);        // #2980B9 - Professional blue
    public static readonly Color PrimaryDark = Color.FromArgb(31, 97, 141);     // #1F618D
    public static readonly Color PrimaryLight = Color.FromArgb(174, 214, 241);  // #AED6F1

    // Secondary colors
    public static readonly Color Secondary = Color.FromArgb(155, 89, 182);      // #9B59B6 - Purple accent
    public static readonly Color SecondaryLight = Color.FromArgb(215, 189, 226); // #D7BDE2

    // Semantic colors
    public static readonly Color Success = Color.FromArgb(39, 174, 96);         // #27AE60 - Green
    public static readonly Color Warning = Color.FromArgb(241, 196, 15);        // #F1C40F - Yellow
    public static readonly Color Danger = Color.FromArgb(231, 76, 60);          // #E74C3C - Red
    public static readonly Color Info = Color.FromArgb(52, 152, 219);           // #3498DB - Light blue

    // Neutral colors
    public static readonly Color Background = Color.FromArgb(248, 249, 250);    // #F8F9FA - Light gray
    public static readonly Color Surface = Color.FromArgb(255, 255, 255);       // #FFFFFF - White
    public static readonly Color Border = Color.FromArgb(222, 226, 230);        // #DEE2E6
    public static readonly Color TextPrimary = Color.FromArgb(33, 37, 41);      // #212529 - Dark gray
    public static readonly Color TextSecondary = Color.FromArgb(108, 117, 125); // #6C757D - Medium gray
    public static readonly Color TextMuted = Color.FromArgb(173, 181, 189);     // #ADB5BD - Light gray

    // States
    public static readonly Color Hover = Color.FromArgb(233, 236, 239);         // #E9ECEF
    public static readonly Color Selected = Color.FromArgb(209, 236, 241);      // #D1ECF1
    public static readonly Color Disabled = Color.FromArgb(206, 212, 218);      // #CED4DA
}
```

**Soft Pastel Theme**
```csharp
public static class PastelColors
{
    // Primary pastel
    public static readonly Color Primary = Color.FromArgb(162, 210, 255);       // #A2D2FF - Soft blue
    public static readonly Color PrimaryDark = Color.FromArgb(118, 180, 236);   // #76B4EC

    // Secondary pastel
    public static readonly Color Secondary = Color.FromArgb(205, 180, 219);     // #CDB4DB - Soft purple
    public static readonly Color Tertiary = Color.FromArgb(255, 202, 212);      // #FFCAD4 - Soft pink

    // Semantic pastels
    public static readonly Color Success = Color.FromArgb(183, 228, 199);       // #B7E4C7 - Soft green
    public static readonly Color Warning = Color.FromArgb(255, 230, 179);       // #FFE6B3 - Soft yellow
    public static readonly Color Danger = Color.FromArgb(255, 179, 186);        // #FFB3BA - Soft red
    public static readonly Color Info = Color.FromArgb(189, 224, 254);          // #BDE0FE - Soft cyan

    // Neutrals for pastel theme
    public static readonly Color Background = Color.FromArgb(253, 253, 253);    // #FDFDFD
    public static readonly Color Surface = Color.FromArgb(255, 255, 255);       // #FFFFFF
    public static readonly Color Border = Color.FromArgb(233, 233, 233);        // #E9E9E9
    public static readonly Color TextPrimary = Color.FromArgb(64, 64, 64);      // #404040
    public static readonly Color TextSecondary = Color.FromArgb(128, 128, 128); // #808080
}
```

**Dark Theme**
```csharp
public static class DarkColors
{
    // Primary colors
    public static readonly Color Primary = Color.FromArgb(100, 181, 246);       // #64B5F6 - Bright blue
    public static readonly Color PrimaryDark = Color.FromArgb(66, 165, 245);    // #42A5F5

    // Background layers
    public static readonly Color Background = Color.FromArgb(18, 18, 18);       // #121212
    public static readonly Color Surface = Color.FromArgb(30, 30, 30);          // #1E1E1E
    public static readonly Color SurfaceLight = Color.FromArgb(45, 45, 45);     // #2D2D2D
    public static readonly Color Border = Color.FromArgb(66, 66, 66);           // #424242

    // Text colors
    public static readonly Color TextPrimary = Color.FromArgb(255, 255, 255);   // #FFFFFF
    public static readonly Color TextSecondary = Color.FromArgb(179, 179, 179); // #B3B3B3
    public static readonly Color TextMuted = Color.FromArgb(128, 128, 128);     // #808080

    // Semantic colors (brighter for dark theme)
    public static readonly Color Success = Color.FromArgb(129, 199, 132);       // #81C784
    public static readonly Color Warning = Color.FromArgb(255, 213, 79);        // #FFD54F
    public static readonly Color Danger = Color.FromArgb(229, 115, 115);        // #E57373
    public static readonly Color Info = Color.FromArgb(79, 195, 247);           // #4FC3F7
}
```

### 1.2 Color Contrast Requirements (WCAG 2.1)

**MANDATORY minimum contrast ratios:**

| Element Type | Minimum Ratio | Example |
|-------------|---------------|---------|
| Normal text (< 18pt) | 4.5:1 | Dark gray on white |
| Large text (≥ 18pt or 14pt bold) | 3:1 | - |
| UI components (buttons, inputs) | 3:1 | Button border on background |
| Focus indicators | 3:1 | - |

```csharp
// Helper method to check contrast
public static double GetContrastRatio(Color foreground, Color background)
{
    double L1 = GetRelativeLuminance(foreground);
    double L2 = GetRelativeLuminance(background);
    return (Math.Max(L1, L2) + 0.05) / (Math.Min(L1, L2) + 0.05);
}

private static double GetRelativeLuminance(Color color)
{
    double r = color.R / 255.0;
    double g = color.G / 255.0;
    double b = color.B / 255.0;

    r = r <= 0.03928 ? r / 12.92 : Math.Pow((r + 0.055) / 1.055, 2.4);
    g = g <= 0.03928 ? g / 12.92 : Math.Pow((g + 0.055) / 1.055, 2.4);
    b = b <= 0.03928 ? b / 12.92 : Math.Pow((b + 0.055) / 1.055, 2.4);

    return 0.2126 * r + 0.7152 * g + 0.0722 * b;
}
```

### 1.3 Color Usage Rules

#### ✅ DO:
1. **Use semantic colors consistently** - Success always green, Danger always red
2. **Provide sufficient contrast** - Text must be readable
3. **Use color + icon** - Don't rely on color alone (colorblind users)
4. **Test with grayscale** - UI should be usable without color
5. **Use 60-30-10 rule** - 60% neutral, 30% secondary, 10% accent
6. **Define color constants** - Never hardcode hex values inline

#### ❌ DON'T:
1. ❌ Button same color as background
2. ❌ Light gray text on white background
3. ❌ Red text on dark background (low contrast)
4. ❌ Random colors for each screen
5. ❌ Too many colors (max 5-6 in palette)
6. ❌ Neon/fluorescent colors (eye strain)

### 1.4 Control-Specific Color Guidelines

```csharp
// Button color guidelines
public void ApplyButtonStyles(Button btn, ButtonType type)
{
    btn.FlatStyle = FlatStyle.Flat;
    btn.FlatAppearance.BorderSize = 1;
    btn.Cursor = Cursors.Hand;

    switch (type)
    {
        case ButtonType.Primary:
            btn.BackColor = AppColors.Primary;
            btn.ForeColor = Color.White;
            btn.FlatAppearance.BorderColor = AppColors.PrimaryDark;
            break;

        case ButtonType.Secondary:
            btn.BackColor = AppColors.Surface;
            btn.ForeColor = AppColors.TextPrimary;
            btn.FlatAppearance.BorderColor = AppColors.Border;
            break;

        case ButtonType.Danger:
            btn.BackColor = AppColors.Danger;
            btn.ForeColor = Color.White;
            btn.FlatAppearance.BorderColor = Color.FromArgb(192, 57, 43);
            break;

        case ButtonType.Success:
            btn.BackColor = AppColors.Success;
            btn.ForeColor = Color.White;
            btn.FlatAppearance.BorderColor = Color.FromArgb(30, 132, 73);
            break;
    }
}

// DataGridView color guidelines
public void ApplyGridStyles(DataGridView dgv)
{
    dgv.BackgroundColor = AppColors.Surface;
    dgv.GridColor = AppColors.Border;

    // Header style
    dgv.ColumnHeadersDefaultCellStyle.BackColor = AppColors.Primary;
    dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
    dgv.ColumnHeadersDefaultCellStyle.Font = new Font(dgv.Font, FontStyle.Bold);
    dgv.EnableHeadersVisualStyles = false;

    // Alternating rows
    dgv.AlternatingRowsDefaultCellStyle.BackColor = AppColors.Background;
    dgv.RowsDefaultCellStyle.BackColor = AppColors.Surface;

    // Selection
    dgv.DefaultCellStyle.SelectionBackColor = AppColors.Selected;
    dgv.DefaultCellStyle.SelectionForeColor = AppColors.TextPrimary;
}
```

---

## 2. Data Display Controls

### 2.1 DataGridView - MANDATORY Features

**⚠️ CRITICAL: Grid MUST fill available space - NO empty gap below grid!**

```csharp
// ✅ CORRECT - Grid fills remaining space
dgvData.Dock = DockStyle.Fill;  // ALWAYS use Dock.Fill for main grid

// Layout structure:
// ┌─────────────────────────┐
// │ Filter Panel (Top)      │  ← Dock.Top
// ├─────────────────────────┤
// │                         │
// │   DataGridView          │  ← Dock.Fill (fills ALL remaining space)
// │                         │
// ├─────────────────────────┤
// │ Paging Panel (Bottom)   │  ← Dock.Bottom
// └─────────────────────────┘

// ❌ WRONG - Fixed height leaves gap
dgvData.Height = 400;  // NEVER use fixed height for main grid
```

**Every DataGridView MUST have:**

```csharp
public void ConfigureProductionGrid(DataGridView dgv)
{
    // ========== VISUAL STYLING ==========
    dgv.BorderStyle = BorderStyle.None;
    dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
    dgv.EnableHeadersVisualStyles = false;
    dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

    // Header styling
    dgv.ColumnHeadersDefaultCellStyle.BackColor = AppColors.Primary;
    dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
    dgv.ColumnHeadersDefaultCellStyle.Font = new Font(dgv.Font.FontFamily, 9f, FontStyle.Bold);
    dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 4, 8, 4);
    dgv.ColumnHeadersHeight = 40;
    dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

    // Alternating rows
    dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
    dgv.RowsDefaultCellStyle.BackColor = Color.White;

    // Selection styling
    dgv.DefaultCellStyle.SelectionBackColor = AppColors.Selected;
    dgv.DefaultCellStyle.SelectionForeColor = AppColors.TextPrimary;

    // ========== SORTING ==========
    foreach (DataGridViewColumn col in dgv.Columns)
    {
        col.SortMode = DataGridViewColumnSortMode.Automatic;
    }

    // ========== COLUMN BEHAVIOR ==========
    dgv.AllowUserToResizeColumns = true;
    dgv.AllowUserToOrderColumns = true;
    dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

    // ========== ROW BEHAVIOR ==========
    dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    dgv.MultiSelect = false;
    dgv.AllowUserToAddRows = false;
    dgv.AllowUserToDeleteRows = false;
    dgv.ReadOnly = true;
    dgv.RowTemplate.Height = 35;

    // ========== SCROLLING ==========
    dgv.ScrollBars = ScrollBars.Both;
    dgv.DoubleBuffered(true); // Extension method for flicker-free
}
```

### 2.2 Grid Filtering

```csharp
// Filter panel above grid
public class GridFilterPanel : Panel
{
    private TextBox txtSearch;
    private ComboBox cboStatus;
    private DateTimePicker dtpFrom;
    private DateTimePicker dtpTo;
    private Button btnFilter;
    private Button btnClear;

    public event EventHandler<FilterEventArgs> FilterApplied;

    public GridFilterPanel()
    {
        InitializeComponents();
        Height = 50;
        BackColor = AppColors.Background;
        Padding = new Padding(10);
    }

    private void InitializeComponents()
    {
        // Search textbox with placeholder
        txtSearch = new TextBox
        {
            PlaceholderText = "Search...",
            Width = 200
        };

        // Status filter
        cboStatus = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 120
        };
        cboStatus.Items.AddRange(new[] { "All", "Active", "Inactive", "Pending" });
        cboStatus.SelectedIndex = 0;

        // Date range
        dtpFrom = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 100 };
        dtpTo = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 100 };

        // Buttons
        btnFilter = new Button { Text = "Filter", Width = 75 };
        btnClear = new Button { Text = "Clear", Width = 75 };

        // Layout using FlowLayoutPanel
        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            AutoSize = true
        };

        flow.Controls.AddRange(new Control[] {
            new Label { Text = "Search:", AutoSize = true, Margin = new Padding(0, 6, 5, 0) },
            txtSearch,
            new Label { Text = "Status:", AutoSize = true, Margin = new Padding(10, 6, 5, 0) },
            cboStatus,
            new Label { Text = "From:", AutoSize = true, Margin = new Padding(10, 6, 5, 0) },
            dtpFrom,
            new Label { Text = "To:", AutoSize = true, Margin = new Padding(10, 6, 5, 0) },
            dtpTo,
            btnFilter,
            btnClear
        });

        Controls.Add(flow);
    }
}
```

### 2.3 Grid Paging

```csharp
public class GridPagingPanel : Panel
{
    private Button btnFirst;
    private Button btnPrev;
    private Button btnNext;
    private Button btnLast;
    private Label lblPageInfo;
    private ComboBox cboPageSize;

    public int CurrentPage { get; private set; } = 1;
    public int PageSize { get; private set; } = 25;
    public int TotalRecords { get; private set; }
    public int TotalPages => (int)Math.Ceiling(TotalRecords / (double)PageSize);

    public event EventHandler PageChanged;

    public GridPagingPanel()
    {
        Height = 40;
        BackColor = AppColors.Background;

        InitializeComponents();
    }

    private void InitializeComponents()
    {
        btnFirst = CreateNavButton("⏮", "First page");
        btnPrev = CreateNavButton("◀", "Previous page");
        btnNext = CreateNavButton("▶", "Next page");
        btnLast = CreateNavButton("⏭", "Last page");

        lblPageInfo = new Label
        {
            AutoSize = true,
            Text = "Page 1 of 1 (0 records)",
            Margin = new Padding(10, 8, 10, 0)
        };

        cboPageSize = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 60
        };
        cboPageSize.Items.AddRange(new object[] { 10, 25, 50, 100 });
        cboPageSize.SelectedItem = 25;
        cboPageSize.SelectedIndexChanged += (s, e) =>
        {
            PageSize = (int)cboPageSize.SelectedItem;
            CurrentPage = 1;
            PageChanged?.Invoke(this, EventArgs.Empty);
        };

        // Layout
        var flow = new FlowLayoutPanel { Dock = DockStyle.Fill };
        flow.Controls.AddRange(new Control[] {
            btnFirst, btnPrev, lblPageInfo, btnNext, btnLast,
            new Label { Text = "Page size:", AutoSize = true, Margin = new Padding(20, 8, 5, 0) },
            cboPageSize
        });

        Controls.Add(flow);
    }

    public void UpdatePageInfo(int totalRecords)
    {
        TotalRecords = totalRecords;
        lblPageInfo.Text = $"Page {CurrentPage} of {TotalPages} ({TotalRecords:N0} records)";

        btnFirst.Enabled = btnPrev.Enabled = CurrentPage > 1;
        btnNext.Enabled = btnLast.Enabled = CurrentPage < TotalPages;
    }

    private Button CreateNavButton(string text, string tooltip)
    {
        var btn = new Button
        {
            Text = text,
            Width = 35,
            Height = 30,
            FlatStyle = FlatStyle.Flat
        };
        new ToolTip().SetToolTip(btn, tooltip);
        return btn;
    }
}
```

### 2.4 Empty State & Loading

```csharp
// Empty state overlay
public class EmptyStateOverlay : Panel
{
    public EmptyStateOverlay(string message, string icon = "📭")
    {
        BackColor = AppColors.Surface;
        Dock = DockStyle.Fill;

        var container = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3
        };

        // Icon
        container.Controls.Add(new Label
        {
            Text = icon,
            Font = new Font("Segoe UI", 48),
            AutoSize = true,
            Anchor = AnchorStyles.None
        });

        // Message
        container.Controls.Add(new Label
        {
            Text = message,
            Font = new Font("Segoe UI", 12),
            ForeColor = AppColors.TextSecondary,
            AutoSize = true,
            Anchor = AnchorStyles.None
        });

        // Action button (optional)
        var btnAction = new Button
        {
            Text = "Add New Item",
            Padding = new Padding(20, 8, 20, 8),
            Anchor = AnchorStyles.None
        };
        container.Controls.Add(btnAction);

        Controls.Add(container);
    }
}

// Loading overlay
public class LoadingOverlay : Panel
{
    private System.Windows.Forms.Timer animationTimer;
    private int dotCount = 0;
    private Label lblLoading;

    public LoadingOverlay(string message = "Loading")
    {
        BackColor = Color.FromArgb(200, 255, 255, 255);
        Dock = DockStyle.Fill;

        lblLoading = new Label
        {
            Text = message,
            Font = new Font("Segoe UI", 12),
            AutoSize = true,
            Anchor = AnchorStyles.None
        };

        var container = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 1
        };
        container.Controls.Add(lblLoading);
        Controls.Add(container);

        // Animate dots
        animationTimer = new System.Windows.Forms.Timer { Interval = 500 };
        animationTimer.Tick += (s, e) =>
        {
            dotCount = (dotCount + 1) % 4;
            lblLoading.Text = message + new string('.', dotCount);
        };
        animationTimer.Start();
    }

    protected override void Dispose(bool disposing)
    {
        animationTimer?.Stop();
        animationTimer?.Dispose();
        base.Dispose(disposing);
    }
}
```

### 2.5 Context Menu for Grid

```csharp
public ContextMenuStrip CreateGridContextMenu(DataGridView dgv)
{
    var menu = new ContextMenuStrip();

    menu.Items.Add(new ToolStripMenuItem("View Details", null, (s, e) => ViewSelected()));
    menu.Items.Add(new ToolStripMenuItem("Edit", null, (s, e) => EditSelected()) { ShortcutKeys = Keys.F2 });
    menu.Items.Add(new ToolStripSeparator());
    menu.Items.Add(new ToolStripMenuItem("Delete", null, (s, e) => DeleteSelected())
    {
        ShortcutKeys = Keys.Delete,
        Image = SystemIcons.Warning.ToBitmap()
    });
    menu.Items.Add(new ToolStripSeparator());

    // Export submenu
    var exportMenu = new ToolStripMenuItem("Export");
    exportMenu.DropDownItems.Add("Export to Excel", null, (s, e) => ExportToExcel());
    exportMenu.DropDownItems.Add("Export to CSV", null, (s, e) => ExportToCsv());
    exportMenu.DropDownItems.Add("Export to PDF", null, (s, e) => ExportToPdf());
    menu.Items.Add(exportMenu);

    menu.Items.Add(new ToolStripSeparator());
    menu.Items.Add(new ToolStripMenuItem("Refresh", null, (s, e) => RefreshData()) { ShortcutKeys = Keys.F5 });

    dgv.ContextMenuStrip = menu;
    return menu;
}
```

---

## 3. Input Controls

### 3.0 Preferred Input Style: Floating Label (Material Design)

**⭐ RECOMMENDED for modern, clean UI:**

Floating Label (còn gọi là Material Text Field, Inline Label) là kiểu input hiện đại:
- Label nằm trong input như placeholder/hint
- Khi focus hoặc có text → label "float" lên trên
- Không cần label riêng bên ngoài → tiết kiệm không gian

**ReaLTaiizor**: Dùng `MaterialTextBoxEdit` với property `Hint`

```csharp
// ReaLTaiizor - MaterialTextBoxEdit
var txtEmail = new MaterialTextBoxEdit
{
    Hint = "Email Address",           // Floating label text
    UseAccent = true,
    ShowAssistiveText = true,
    HelperText = "Enter your email"   // Helper text below
};

var txtPassword = new MaterialTextBoxEdit
{
    Hint = "Password",
    Password = true,
    ShowAssistiveText = true,
    HelperText = "Minimum 8 characters"
};
```

**Standard WinForms - Custom Floating Label**:

```csharp
public class FloatingLabelTextBox : UserControl
{
    private TextBox textBox;
    private Label floatingLabel;
    private bool isFloating;

    public string Hint { get; set; } = "Label";

    public FloatingLabelTextBox()
    {
        textBox = new TextBox { BorderStyle = BorderStyle.None };
        floatingLabel = new Label
        {
            ForeColor = AppColors.TextMuted,
            Font = new Font("Segoe UI", 9.5f)
        };

        textBox.GotFocus += (s, e) => AnimateLabelUp();
        textBox.LostFocus += (s, e) =>
        {
            if (string.IsNullOrEmpty(textBox.Text))
                AnimateLabelDown();
        };

        Controls.Add(textBox);
        Controls.Add(floatingLabel);
    }

    private void AnimateLabelUp()
    {
        // Animate label to top, smaller font, accent color
        floatingLabel.Font = new Font("Segoe UI", 7.5f);
        floatingLabel.ForeColor = AppColors.Primary;
        floatingLabel.Top = 0;
        isFloating = true;
    }

    private void AnimateLabelDown()
    {
        // Animate label back to center, normal font
        floatingLabel.Font = new Font("Segoe UI", 9.5f);
        floatingLabel.ForeColor = AppColors.TextMuted;
        floatingLabel.Top = textBox.Top;
        isFloating = false;
    }
}
```

**DevExpress**: Dùng `TextEdit` với `Properties.NullValuePrompt`

```csharp
var txtEmail = new TextEdit();
txtEmail.Properties.NullValuePrompt = "Email Address";
txtEmail.Properties.NullValuePromptShowForEmptyValue = true;
```

---

### 3.1 TextBox - Production Standards

```csharp
public class ProductionTextBox : TextBox
{
    private Label lblCharCount;
    private ErrorProvider errorProvider;

    public int? MaxLength { get; set; }
    public bool ShowCharacterCount { get; set; } = true;
    public bool Required { get; set; }

    public ProductionTextBox()
    {
        // Styling
        BorderStyle = BorderStyle.FixedSingle;
        Font = new Font("Segoe UI", 9.5f);
        Padding = new Padding(5);

        // Placeholder support (.NET 5+)
        // For older .NET, implement custom placeholder

        // Events
        TextChanged += OnTextChanged;
        Enter += (s, e) => BackColor = AppColors.Selected;
        Leave += (s, e) => BackColor = SystemColors.Window;
    }

    private void OnTextChanged(object sender, EventArgs e)
    {
        if (ShowCharacterCount && MaxLength.HasValue)
        {
            UpdateCharacterCount();
        }
    }

    private void UpdateCharacterCount()
    {
        // Show "45/100" below textbox
        if (lblCharCount != null)
        {
            lblCharCount.Text = $"{Text.Length}/{MaxLength}";
            lblCharCount.ForeColor = Text.Length > MaxLength * 0.9
                ? AppColors.Warning
                : AppColors.TextSecondary;
        }
    }
}
```

### 3.2 ComboBox - Production Standards

```csharp
public class ProductionComboBox : ComboBox
{
    private const string PlaceholderText = "-- Select --";

    public ProductionComboBox()
    {
        DropDownStyle = ComboBoxStyle.DropDownList;
        Font = new Font("Segoe UI", 9.5f);

        // Add placeholder as first item
        Items.Add(PlaceholderText);
        SelectedIndex = 0;
    }

    public void LoadItems<T>(IEnumerable<T> items, string displayMember, string valueMember)
    {
        Items.Clear();
        Items.Add(PlaceholderText);

        foreach (var item in items)
        {
            Items.Add(item);
        }

        DisplayMember = displayMember;
        ValueMember = valueMember;
        SelectedIndex = 0;
    }

    public bool HasSelection => SelectedIndex > 0 && SelectedItem?.ToString() != PlaceholderText;

    public T GetSelectedValue<T>()
    {
        if (!HasSelection) return default;
        return (T)SelectedValue;
    }
}

// Searchable ComboBox for large lists
public class SearchableComboBox : ComboBox
{
    private List<object> allItems = new List<object>();

    public SearchableComboBox()
    {
        DropDownStyle = ComboBoxStyle.DropDown;
        AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        AutoCompleteSource = AutoCompleteSource.ListItems;
    }

    public void LoadItems(IEnumerable<object> items)
    {
        allItems = items.ToList();
        Items.Clear();
        Items.AddRange(allItems.ToArray());
    }
}
```

### 3.3 DateTimePicker - Production Standards

```csharp
// Nullable DateTimePicker
public class NullableDateTimePicker : DateTimePicker
{
    private bool isNull = true;
    private DateTimePickerFormat originalFormat;
    private string originalCustomFormat;

    public new DateTime? Value
    {
        get => isNull ? null : base.Value;
        set
        {
            if (value.HasValue)
            {
                base.Value = value.Value;
                isNull = false;
                Format = originalFormat;
                CustomFormat = originalCustomFormat;
            }
            else
            {
                isNull = true;
                Format = DateTimePickerFormat.Custom;
                CustomFormat = " "; // Blank display
            }
        }
    }

    public NullableDateTimePicker()
    {
        originalFormat = Format;
        originalCustomFormat = CustomFormat;

        // Clear button via keyboard
        KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                Value = null;
                e.Handled = true;
            }
        };

        ValueChanged += (s, e) =>
        {
            if (isNull)
            {
                isNull = false;
                Format = originalFormat;
            }
        };
    }
}

// Date range validator
public class DateRangePicker : UserControl
{
    public NullableDateTimePicker FromDate { get; private set; }
    public NullableDateTimePicker ToDate { get; private set; }

    public event EventHandler RangeChanged;

    public DateRangePicker()
    {
        FromDate = new NullableDateTimePicker();
        ToDate = new NullableDateTimePicker();

        // Validate range
        FromDate.ValueChanged += ValidateRange;
        ToDate.ValueChanged += ValidateRange;
    }

    private void ValidateRange(object sender, EventArgs e)
    {
        if (FromDate.Value.HasValue && ToDate.Value.HasValue)
        {
            if (FromDate.Value > ToDate.Value)
            {
                // Swap or show error
                MessageBox.Show("From date cannot be after To date",
                    "Invalid Range", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        RangeChanged?.Invoke(this, EventArgs.Empty);
    }
}
```

---

## 4. Buttons and Actions

### 4.1 Button Styling Standards

```csharp
public enum ButtonType { Primary, Secondary, Success, Danger, Warning, Link }

public static class ButtonStyles
{
    public static void ApplyStyle(Button btn, ButtonType type)
    {
        // Base styling
        btn.FlatStyle = FlatStyle.Flat;
        btn.Cursor = Cursors.Hand;
        btn.Font = new Font("Segoe UI", 9f);
        btn.Padding = new Padding(15, 8, 15, 8);
        btn.MinimumSize = new Size(80, 32);
        btn.FlatAppearance.BorderSize = 1;

        switch (type)
        {
            case ButtonType.Primary:
                btn.BackColor = AppColors.Primary;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderColor = AppColors.PrimaryDark;
                btn.FlatAppearance.MouseOverBackColor = AppColors.PrimaryDark;
                break;

            case ButtonType.Secondary:
                btn.BackColor = AppColors.Surface;
                btn.ForeColor = AppColors.TextPrimary;
                btn.FlatAppearance.BorderColor = AppColors.Border;
                btn.FlatAppearance.MouseOverBackColor = AppColors.Hover;
                break;

            case ButtonType.Success:
                btn.BackColor = AppColors.Success;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderColor = Color.FromArgb(30, 132, 73);
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 132, 73);
                break;

            case ButtonType.Danger:
                btn.BackColor = AppColors.Danger;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderColor = Color.FromArgb(192, 57, 43);
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 57, 43);
                break;

            case ButtonType.Warning:
                btn.BackColor = AppColors.Warning;
                btn.ForeColor = AppColors.TextPrimary;
                btn.FlatAppearance.BorderColor = Color.FromArgb(243, 156, 18);
                break;

            case ButtonType.Link:
                btn.BackColor = Color.Transparent;
                btn.ForeColor = AppColors.Primary;
                btn.FlatAppearance.BorderSize = 0;
                btn.Cursor = Cursors.Hand;
                // Add underline on hover
                break;
        }
    }

    // Disabled state styling
    public static void ApplyDisabledState(Button btn)
    {
        btn.BackColor = AppColors.Disabled;
        btn.ForeColor = AppColors.TextMuted;
        btn.FlatAppearance.BorderColor = AppColors.Border;
        btn.Cursor = Cursors.No;
    }
}
```

### 4.2 Loading State for Buttons

```csharp
public static class ButtonExtensions
{
    private static readonly Dictionary<Button, (string Text, bool Enabled)> OriginalStates = new();

    public static void SetLoading(this Button btn, bool isLoading, string loadingText = "Loading...")
    {
        if (isLoading)
        {
            // Save original state
            OriginalStates[btn] = (btn.Text, btn.Enabled);

            btn.Text = loadingText;
            btn.Enabled = false;
            btn.Cursor = Cursors.WaitCursor;
        }
        else
        {
            // Restore original state
            if (OriginalStates.TryGetValue(btn, out var state))
            {
                btn.Text = state.Text;
                btn.Enabled = state.Enabled;
                btn.Cursor = Cursors.Hand;
                OriginalStates.Remove(btn);
            }
        }
    }
}

// Usage
private async void btnSave_Click(object sender, EventArgs e)
{
    btnSave.SetLoading(true, "Saving...");
    try
    {
        await _presenter.SaveAsync();
    }
    finally
    {
        btnSave.SetLoading(false);
    }
}
```

### 4.3 Button Action Standards

```csharp
// Double-click prevention
public class SafeButton : Button
{
    private DateTime lastClick = DateTime.MinValue;
    private readonly int clickDelayMs = 500;

    protected override void OnClick(EventArgs e)
    {
        if ((DateTime.Now - lastClick).TotalMilliseconds < clickDelayMs)
            return;

        lastClick = DateTime.Now;
        base.OnClick(e);
    }
}

// Confirmation for destructive actions
public static async Task<bool> ConfirmDangerousAction(string message, string title = "Confirm Action")
{
    var result = MessageBox.Show(
        message,
        title,
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Warning,
        MessageBoxDefaultButton.Button2); // Default to No

    return result == DialogResult.Yes;
}

// Usage
private async void btnDelete_Click(object sender, EventArgs e)
{
    if (!await ConfirmDangerousAction("Are you sure you want to delete this item? This action cannot be undone."))
        return;

    // Proceed with deletion
}
```

---

## 5. Form Layout & Responsive Design

### 5.1 Responsive Layout Requirements

**Every form MUST:**

```csharp
public partial class ProductionForm : Form
{
    public ProductionForm()
    {
        InitializeComponent();

        // REQUIRED: Set minimum size
        MinimumSize = new Size(800, 600);

        // REQUIRED: Start maximized by default
        StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Maximized;  // ⭐ ALWAYS start maximized

        // REQUIRED: Remember window state
        Load += (s, e) => RestoreWindowState();
        FormClosing += (s, e) => SaveWindowState();
    }

    private void SaveWindowState()
    {
        Properties.Settings.Default.WindowState = WindowState;
        if (WindowState == FormWindowState.Normal)
        {
            Properties.Settings.Default.WindowLocation = Location;
            Properties.Settings.Default.WindowSize = Size;
        }
        Properties.Settings.Default.Save();
    }

    private void RestoreWindowState()
    {
        if (Properties.Settings.Default.WindowSize != Size.Empty)
        {
            Location = Properties.Settings.Default.WindowLocation;
            Size = Properties.Settings.Default.WindowSize;
        }
        WindowState = Properties.Settings.Default.WindowState;
    }
}
```

### 5.2 Layout Patterns

```csharp
// Master-detail layout
public void SetupMasterDetailLayout()
{
    var splitContainer = new SplitContainer
    {
        Dock = DockStyle.Fill,
        Orientation = Orientation.Vertical,
        SplitterDistance = 300,
        SplitterWidth = 6,
        Panel1MinSize = 200,
        Panel2MinSize = 400
    };

    // Left panel - List
    var dgvList = new DataGridView { Dock = DockStyle.Fill };
    splitContainer.Panel1.Controls.Add(dgvList);

    // Right panel - Details
    var detailPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
    splitContainer.Panel2.Controls.Add(detailPanel);

    Controls.Add(splitContainer);
}

// Form with toolbar, content, and status bar
public void SetupStandardLayout()
{
    // Toolbar at top
    var toolStrip = new ToolStrip { Dock = DockStyle.Top };

    // Status bar at bottom
    var statusStrip = new StatusStrip { Dock = DockStyle.Bottom };
    statusStrip.Items.Add(new ToolStripStatusLabel("Ready"));

    // Main content fills remaining space
    var contentPanel = new Panel { Dock = DockStyle.Fill };

    // Add in correct order (reverse of visual order)
    Controls.Add(contentPanel);
    Controls.Add(statusStrip);
    Controls.Add(toolStrip);
}
```

### 5.3 Spacing Standards

```csharp
public static class LayoutConstants
{
    // Margins
    public const int FormPadding = 15;
    public const int GroupPadding = 10;
    public const int ControlSpacing = 8;
    public const int SectionSpacing = 20;

    // Control sizes
    public const int TextBoxHeight = 25;
    public const int ButtonHeight = 32;
    public const int ButtonMinWidth = 80;
    public const int LabelWidth = 120;
    public const int InputWidth = 200;
}

// Apply consistent spacing
public void ApplyStandardSpacing(Control parent)
{
    parent.Padding = new Padding(LayoutConstants.FormPadding);

    foreach (Control control in parent.Controls)
    {
        control.Margin = new Padding(LayoutConstants.ControlSpacing);
    }
}
```

---

## 6. Feedback & Status Communication

### 6.1 Toast Notifications

```csharp
public enum ToastType { Success, Error, Warning, Info }

public class ToastNotification : Form
{
    private System.Windows.Forms.Timer closeTimer;

    public ToastNotification(string message, ToastType type, int durationMs = 3000)
    {
        // Form setup
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        TopMost = true;
        Size = new Size(300, 60);
        StartPosition = FormStartPosition.Manual;

        // Position bottom-right
        var screen = Screen.PrimaryScreen.WorkingArea;
        Location = new Point(screen.Right - Width - 20, screen.Bottom - Height - 20);

        // Styling based on type
        BackColor = type switch
        {
            ToastType.Success => AppColors.Success,
            ToastType.Error => AppColors.Danger,
            ToastType.Warning => AppColors.Warning,
            ToastType.Info => AppColors.Info,
            _ => AppColors.Info
        };

        // Message label
        var lbl = new Label
        {
            Text = message,
            ForeColor = type == ToastType.Warning ? AppColors.TextPrimary : Color.White,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 10f)
        };
        Controls.Add(lbl);

        // Auto-close timer
        closeTimer = new System.Windows.Forms.Timer { Interval = durationMs };
        closeTimer.Tick += (s, e) => Close();
        closeTimer.Start();

        // Click to close
        Click += (s, e) => Close();
        lbl.Click += (s, e) => Close();
    }

    public static void Show(string message, ToastType type = ToastType.Info)
    {
        var toast = new ToastNotification(message, type);
        toast.Show();
    }
}

// Usage
ToastNotification.Show("Record saved successfully!", ToastType.Success);
ToastNotification.Show("Failed to connect to server", ToastType.Error);
```

### 6.2 Status Bar Updates

```csharp
public class StatusBarManager
{
    private readonly StatusStrip statusStrip;
    private readonly ToolStripStatusLabel lblStatus;
    private readonly ToolStripProgressBar progressBar;
    private readonly ToolStripStatusLabel lblRecordCount;

    public StatusBarManager(StatusStrip strip)
    {
        statusStrip = strip;

        lblStatus = new ToolStripStatusLabel
        {
            Text = "Ready",
            Spring = true,
            TextAlign = ContentAlignment.MiddleLeft
        };

        progressBar = new ToolStripProgressBar
        {
            Visible = false,
            Width = 100
        };

        lblRecordCount = new ToolStripStatusLabel
        {
            Text = "0 records",
            BorderSides = ToolStripStatusLabelBorderSides.Left
        };

        statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus, progressBar, lblRecordCount });
    }

    public void SetStatus(string message)
    {
        lblStatus.Text = message;
    }

    public void SetRecordCount(int count)
    {
        lblRecordCount.Text = $"{count:N0} records";
    }

    public void ShowProgress(bool show, int value = 0)
    {
        progressBar.Visible = show;
        progressBar.Value = value;
    }

    public void SetBusy(string message)
    {
        lblStatus.Text = message;
        progressBar.Visible = true;
        progressBar.Style = ProgressBarStyle.Marquee;
    }

    public void SetReady()
    {
        lblStatus.Text = "Ready";
        progressBar.Visible = false;
        progressBar.Style = ProgressBarStyle.Blocks;
    }
}
```

---

## 7. Validation & Error Handling

### 7.1 Validation Framework

```csharp
public class FormValidator
{
    private readonly ErrorProvider errorProvider;
    private readonly Dictionary<Control, List<Func<string>>> validationRules = new();

    public FormValidator(Form form)
    {
        errorProvider = new ErrorProvider(form)
        {
            BlinkStyle = ErrorBlinkStyle.NeverBlink
        };
    }

    public void AddRule(Control control, Func<string> rule)
    {
        if (!validationRules.ContainsKey(control))
        {
            validationRules[control] = new List<Func<string>>();

            // Validate on leave
            control.Leave += (s, e) => ValidateControl(control);
        }

        validationRules[control].Add(rule);
    }

    public void AddRequired(Control control, string fieldName)
    {
        AddRule(control, () =>
        {
            var value = control switch
            {
                TextBox tb => tb.Text,
                ComboBox cb => cb.SelectedIndex > 0 ? "selected" : "",
                _ => control.Text
            };

            return string.IsNullOrWhiteSpace(value)
                ? $"{fieldName} is required"
                : null;
        });
    }

    public void AddEmail(TextBox textBox)
    {
        AddRule(textBox, () =>
        {
            if (string.IsNullOrEmpty(textBox.Text)) return null;

            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return !Regex.IsMatch(textBox.Text, emailRegex)
                ? "Invalid email format"
                : null;
        });
    }

    public void AddRange(NumericUpDown nud, decimal min, decimal max)
    {
        AddRule(nud, () =>
        {
            if (nud.Value < min || nud.Value > max)
                return $"Value must be between {min} and {max}";
            return null;
        });
    }

    private void ValidateControl(Control control)
    {
        var errors = validationRules[control]
            .Select(rule => rule())
            .Where(error => error != null)
            .ToList();

        var errorMessage = string.Join("\n", errors);
        errorProvider.SetError(control, errorMessage);

        // Visual feedback
        control.BackColor = string.IsNullOrEmpty(errorMessage)
            ? SystemColors.Window
            : Color.FromArgb(255, 240, 240);
    }

    public bool ValidateAll()
    {
        var isValid = true;

        foreach (var control in validationRules.Keys)
        {
            ValidateControl(control);
            if (!string.IsNullOrEmpty(errorProvider.GetError(control)))
                isValid = false;
        }

        return isValid;
    }

    public List<string> GetAllErrors()
    {
        return validationRules.Keys
            .Select(c => errorProvider.GetError(c))
            .Where(e => !string.IsNullOrEmpty(e))
            .ToList();
    }
}

// Usage
var validator = new FormValidator(this);
validator.AddRequired(txtName, "Name");
validator.AddEmail(txtEmail);
validator.AddRange(nudAge, 0, 120);

if (!validator.ValidateAll())
{
    var errors = validator.GetAllErrors();
    MessageBox.Show(string.Join("\n", errors), "Validation Errors");
    return;
}
```

### 7.2 Global Exception Handler

```csharp
public static class GlobalExceptionHandler
{
    public static void Initialize()
    {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += Application_ThreadException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
        HandleException(e.Exception);
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        HandleException(e.ExceptionObject as Exception);
    }

    private static void HandleException(Exception ex)
    {
        // Log the error
        Log.Error(ex, "Unhandled exception occurred");

        // Show user-friendly dialog
        var dialog = new ErrorDialog(ex);
        dialog.ShowDialog();
    }
}

public class ErrorDialog : Form
{
    public ErrorDialog(Exception ex)
    {
        Text = "An Error Occurred";
        Size = new Size(500, 300);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20),
            RowCount = 4,
            ColumnCount = 1
        };

        // Icon and title
        var lblTitle = new Label
        {
            Text = "😔 Something went wrong",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            AutoSize = true
        };
        panel.Controls.Add(lblTitle);

        // User-friendly message
        var lblMessage = new Label
        {
            Text = "We apologize for the inconvenience. The error has been logged and we'll look into it.",
            AutoSize = true,
            MaximumSize = new Size(440, 0)
        };
        panel.Controls.Add(lblMessage);

        // Technical details (collapsible)
        var txtDetails = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Text = ex.ToString(),
            Dock = DockStyle.Fill
        };

        var grpDetails = new GroupBox
        {
            Text = "Technical Details",
            Dock = DockStyle.Fill
        };
        grpDetails.Controls.Add(txtDetails);
        panel.Controls.Add(grpDetails);

        // Buttons
        var buttonPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            Dock = DockStyle.Fill
        };

        var btnClose = new Button { Text = "Close", DialogResult = DialogResult.OK };
        var btnCopy = new Button { Text = "Copy Details" };
        btnCopy.Click += (s, e) => Clipboard.SetText(ex.ToString());

        buttonPanel.Controls.AddRange(new Control[] { btnClose, btnCopy });
        panel.Controls.Add(buttonPanel);

        Controls.Add(panel);
        AcceptButton = btnClose;
    }
}
```

---

## 8. Accessibility (WCAG)

### 8.1 Keyboard Navigation

```csharp
public static class AccessibilityHelper
{
    // Set logical tab order
    public static void SetTabOrder(params Control[] controls)
    {
        for (int i = 0; i < controls.Length; i++)
        {
            controls[i].TabIndex = i;
        }
    }

    // Add keyboard shortcuts
    public static void AddShortcut(Form form, Keys keys, Action action)
    {
        form.KeyPreview = true;
        form.KeyDown += (s, e) =>
        {
            if (e.KeyData == keys)
            {
                action();
                e.Handled = true;
            }
        };
    }
}

// Usage
AccessibilityHelper.SetTabOrder(txtName, txtEmail, txtPhone, cboCountry, btnSave, btnCancel);
AccessibilityHelper.AddShortcut(this, Keys.Control | Keys.S, () => Save());
AccessibilityHelper.AddShortcut(this, Keys.Escape, () => Close());
```

### 8.2 Screen Reader Support

```csharp
public static void SetAccessibleProperties(Control control, string name, string description = null)
{
    control.AccessibleName = name;
    if (description != null)
        control.AccessibleDescription = description;
}

// Usage
SetAccessibleProperties(txtEmail, "Email address", "Enter your email address for account recovery");
SetAccessibleProperties(btnSave, "Save", "Save the current form data");
SetAccessibleProperties(dgvCustomers, "Customer list", "List of all customers with sorting and filtering");
```

### 8.3 High Contrast Support

```csharp
public static bool IsHighContrastMode => SystemInformation.HighContrast;

public void ApplyHighContrastIfNeeded()
{
    if (IsHighContrastMode)
    {
        // Use system colors instead of custom colors
        foreach (Control control in Controls)
        {
            control.BackColor = SystemColors.Window;
            control.ForeColor = SystemColors.WindowText;
        }
    }
}
```

---

## 9. Performance & Optimization

### 9.1 Async Loading Pattern

```csharp
public async Task LoadDataAsync()
{
    // Show loading state
    ShowLoadingOverlay();
    statusBar.SetBusy("Loading data...");
    dgvData.Enabled = false;

    try
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var data = await _service.GetAllAsync(cts.Token);

        // Update UI on UI thread
        dgvData.DataSource = data;
        statusBar.SetRecordCount(data.Count);
    }
    catch (OperationCanceledException)
    {
        ToastNotification.Show("Operation timed out", ToastType.Warning);
    }
    catch (Exception ex)
    {
        ToastNotification.Show("Failed to load data", ToastType.Error);
        Log.Error(ex, "Failed to load data");
    }
    finally
    {
        HideLoadingOverlay();
        statusBar.SetReady();
        dgvData.Enabled = true;
    }
}
```

### 9.2 Virtual Mode for Large Data

```csharp
public class VirtualDataGridView
{
    private DataGridView dgv;
    private List<Customer> cachedData = new();
    private int pageSize = 100;

    public void EnableVirtualMode(DataGridView grid)
    {
        dgv = grid;
        dgv.VirtualMode = true;
        dgv.CellValueNeeded += Dgv_CellValueNeeded;
    }

    public async Task LoadDataAsync(int totalRows)
    {
        dgv.RowCount = totalRows;
    }

    private void Dgv_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
    {
        // Load page if not cached
        int pageIndex = e.RowIndex / pageSize;
        if (!IsPageCached(pageIndex))
        {
            LoadPage(pageIndex);
        }

        // Return cached value
        var item = cachedData[e.RowIndex % pageSize];
        e.Value = dgv.Columns[e.ColumnIndex].DataPropertyName switch
        {
            "Name" => item.Name,
            "Email" => item.Email,
            _ => null
        };
    }
}
```

### 9.3 Double Buffering

```csharp
public static class ControlExtensions
{
    public static void EnableDoubleBuffering(this Control control)
    {
        typeof(Control).GetProperty("DoubleBuffered",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance)
            ?.SetValue(control, true);
    }
}

// Usage
dgvCustomers.EnableDoubleBuffering();
panelContent.EnableDoubleBuffering();
```

---

## 10. Internationalization

### 10.1 Resource-Based Strings

```csharp
// All strings in Resources.resx
// Resources.resx (default - English)
// Resources.vi.resx (Vietnamese)
// Resources.ja.resx (Japanese)

// Usage
lblTitle.Text = Resources.FormTitle;
btnSave.Text = Resources.ButtonSave;
MessageBox.Show(Resources.SaveSuccessMessage);

// With parameters
var message = string.Format(Resources.RecordCountFormat, count);
// RecordCountFormat = "Found {0} records"
```

### 10.2 Culture-Aware Formatting

```csharp
// Dates
txtDate.Text = date.ToString("d", CultureInfo.CurrentCulture);

// Numbers
txtAmount.Text = amount.ToString("N2", CultureInfo.CurrentCulture);

// Currency
txtPrice.Text = price.ToString("C", CultureInfo.CurrentCulture);

// Parsing
if (decimal.TryParse(txtAmount.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out var value))
{
    // Success
}
```

### 10.3 RTL Support

```csharp
public void ApplyRtlSupport()
{
    if (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
    {
        RightToLeft = RightToLeft.Yes;
        RightToLeftLayout = true;
    }
}
```

---

## 11. Professional Polish

### 11.1 Icon Standards

```csharp
// Use consistent icon set (e.g., Material Design Icons, Font Awesome)
public static class AppIcons
{
    // 16x16 for buttons and menus
    public static Image Add => Resources.icon_add_16;
    public static Image Edit => Resources.icon_edit_16;
    public static Image Delete => Resources.icon_delete_16;
    public static Image Save => Resources.icon_save_16;
    public static Image Refresh => Resources.icon_refresh_16;
    public static Image Search => Resources.icon_search_16;
    public static Image Export => Resources.icon_export_16;
    public static Image Print => Resources.icon_print_16;

    // 24x24 for toolbar
    public static Image AddLarge => Resources.icon_add_24;
    // ...
}

// Usage
btnAdd.Image = AppIcons.Add;
btnAdd.ImageAlign = ContentAlignment.MiddleLeft;
btnAdd.TextImageRelation = TextImageRelation.ImageBeforeText;
```

### 11.2 Consistent Typography

```csharp
public static class AppFonts
{
    public static readonly Font Title = new Font("Segoe UI", 16, FontStyle.Bold);
    public static readonly Font Subtitle = new Font("Segoe UI", 12, FontStyle.Regular);
    public static readonly Font Body = new Font("Segoe UI", 9.5f, FontStyle.Regular);
    public static readonly Font Caption = new Font("Segoe UI", 8, FontStyle.Regular);
    public static readonly Font Button = new Font("Segoe UI", 9, FontStyle.Regular);
    public static readonly Font Monospace = new Font("Consolas", 9.5f, FontStyle.Regular);
}
```

### 11.3 Smooth Transitions

```csharp
public static class AnimationHelper
{
    public static async Task FadeIn(Form form, int durationMs = 200)
    {
        form.Opacity = 0;
        form.Show();

        int steps = 20;
        int delay = durationMs / steps;

        for (int i = 1; i <= steps; i++)
        {
            form.Opacity = (double)i / steps;
            await Task.Delay(delay);
        }
    }

    public static async Task FadeOut(Form form, int durationMs = 200)
    {
        int steps = 20;
        int delay = durationMs / steps;

        for (int i = steps; i >= 0; i--)
        {
            form.Opacity = (double)i / steps;
            await Task.Delay(delay);
        }

        form.Hide();
    }
}
```

---

## 12. Navigation & Workflow

### 12.1 Dirty State Tracking

```csharp
public class DirtyStateTracker
{
    private readonly Form form;
    private readonly HashSet<Control> trackedControls = new();
    private bool isDirty;

    public bool IsDirty
    {
        get => isDirty;
        private set
        {
            isDirty = value;
            UpdateTitle();
        }
    }

    public DirtyStateTracker(Form form)
    {
        this.form = form;
    }

    public void Track(Control control)
    {
        trackedControls.Add(control);

        switch (control)
        {
            case TextBox tb:
                tb.TextChanged += (s, e) => IsDirty = true;
                break;
            case ComboBox cb:
                cb.SelectedIndexChanged += (s, e) => IsDirty = true;
                break;
            case CheckBox chk:
                chk.CheckedChanged += (s, e) => IsDirty = true;
                break;
            case DateTimePicker dtp:
                dtp.ValueChanged += (s, e) => IsDirty = true;
                break;
            case NumericUpDown nud:
                nud.ValueChanged += (s, e) => IsDirty = true;
                break;
        }
    }

    public void Reset()
    {
        IsDirty = false;
    }

    private void UpdateTitle()
    {
        var title = form.Text.TrimEnd('*', ' ');
        form.Text = IsDirty ? $"{title} *" : title;
    }

    public bool ConfirmDiscard()
    {
        if (!IsDirty) return true;

        var result = MessageBox.Show(
            "You have unsaved changes. Do you want to discard them?",
            "Unsaved Changes",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        return result == DialogResult.Yes;
    }
}

// Usage
private DirtyStateTracker dirtyTracker;

private void InitializeTracking()
{
    dirtyTracker = new DirtyStateTracker(this);
    dirtyTracker.Track(txtName);
    dirtyTracker.Track(txtEmail);
    dirtyTracker.Track(cboStatus);
}

private void Form_FormClosing(object sender, FormClosingEventArgs e)
{
    if (!dirtyTracker.ConfirmDiscard())
    {
        e.Cancel = true;
    }
}

private async void btnSave_Click(object sender, EventArgs e)
{
    await SaveAsync();
    dirtyTracker.Reset();
}
```

### 12.2 Recent Items

```csharp
public class RecentItemsManager
{
    private const int MaxItems = 10;
    private readonly string settingsKey;

    public List<RecentItem> Items { get; private set; } = new();

    public RecentItemsManager(string key)
    {
        settingsKey = key;
        Load();
    }

    public void Add(string name, string path)
    {
        // Remove if exists
        Items.RemoveAll(i => i.Path == path);

        // Add at beginning
        Items.Insert(0, new RecentItem { Name = name, Path = path, AccessedAt = DateTime.Now });

        // Trim to max
        if (Items.Count > MaxItems)
            Items = Items.Take(MaxItems).ToList();

        Save();
    }

    private void Load()
    {
        var json = Properties.Settings.Default[settingsKey] as string;
        if (!string.IsNullOrEmpty(json))
        {
            Items = JsonSerializer.Deserialize<List<RecentItem>>(json);
        }
    }

    private void Save()
    {
        Properties.Settings.Default[settingsKey] = JsonSerializer.Serialize(Items);
        Properties.Settings.Default.Save();
    }
}

public class RecentItem
{
    public string Name { get; set; }
    public string Path { get; set; }
    public DateTime AccessedAt { get; set; }
}
```

---

## 13. Security & Data Protection

### 13.1 Sensitive Data Masking

```csharp
public static class DataMasking
{
    public static string MaskEmail(string email)
    {
        if (string.IsNullOrEmpty(email)) return email;

        var parts = email.Split('@');
        if (parts.Length != 2) return email;

        var name = parts[0];
        var masked = name.Length > 2
            ? name[0] + new string('*', name.Length - 2) + name[^1]
            : new string('*', name.Length);

        return $"{masked}@{parts[1]}";
    }

    public static string MaskPhone(string phone)
    {
        if (string.IsNullOrEmpty(phone)) return phone;

        var digits = new string(phone.Where(char.IsDigit).ToArray());
        if (digits.Length < 4) return new string('*', phone.Length);

        return new string('*', digits.Length - 4) + digits[^4..];
    }

    public static string MaskCreditCard(string card)
    {
        if (string.IsNullOrEmpty(card)) return card;

        var digits = new string(card.Where(char.IsDigit).ToArray());
        if (digits.Length < 4) return new string('*', card.Length);

        return $"**** **** **** {digits[^4..]}";
    }
}
```

### 13.2 Session Timeout

```csharp
public class SessionManager
{
    private System.Windows.Forms.Timer idleTimer;
    private DateTime lastActivity;
    private readonly int timeoutMinutes;

    public event EventHandler SessionExpired;

    public SessionManager(int timeoutMinutes = 15)
    {
        this.timeoutMinutes = timeoutMinutes;

        idleTimer = new System.Windows.Forms.Timer { Interval = 60000 }; // Check every minute
        idleTimer.Tick += CheckIdle;

        // Track user activity
        Application.AddMessageFilter(new ActivityMessageFilter(this));
    }

    public void Start()
    {
        lastActivity = DateTime.Now;
        idleTimer.Start();
    }

    public void RecordActivity()
    {
        lastActivity = DateTime.Now;
    }

    private void CheckIdle(object sender, EventArgs e)
    {
        if ((DateTime.Now - lastActivity).TotalMinutes >= timeoutMinutes)
        {
            idleTimer.Stop();
            SessionExpired?.Invoke(this, EventArgs.Empty);
        }
    }

    private class ActivityMessageFilter : IMessageFilter
    {
        private readonly SessionManager manager;

        public ActivityMessageFilter(SessionManager manager)
        {
            this.manager = manager;
        }

        public bool PreFilterMessage(ref Message m)
        {
            // Mouse and keyboard messages
            if (m.Msg >= 0x200 && m.Msg <= 0x20E || // Mouse
                m.Msg >= 0x100 && m.Msg <= 0x108)   // Keyboard
            {
                manager.RecordActivity();
            }
            return false;
        }
    }
}
```

---

## 14. Production Checklist

### Before Completing ANY UI Task, Verify:

#### Data Display ✓
- [ ] DataGridView has sortable columns with visual indicators
- [ ] DataGridView has filtering capability
- [ ] DataGridView has paging for large datasets
- [ ] DataGridView has alternating row colors
- [ ] DataGridView shows empty state message when no data
- [ ] DataGridView shows loading indicator during fetch
- [ ] DataGridView has context menu with common actions
- [ ] DataGridView supports keyboard navigation
- [ ] Export to Excel/CSV is available

#### Input Controls ✓
- [ ] All text fields have placeholder text
- [ ] Required fields are marked with asterisk (*)
- [ ] Character count shown for limited fields
- [ ] ComboBox has "-- Select --" placeholder
- [ ] DateTimePicker supports null/clear
- [ ] Validation shows ErrorProvider icons
- [ ] Invalid fields have red border/background

#### Buttons ✓
- [ ] Primary button has high contrast (not same as background)
- [ ] Buttons have hover effects
- [ ] Buttons have disabled styling
- [ ] Loading state shown during async operations
- [ ] Destructive actions require confirmation
- [ ] Double-click is prevented
- [ ] Keyboard shortcuts work (Alt+key)

#### Layout ✓
- [ ] Form has MinimumSize set
- [ ] Form resizes properly (Anchor/Dock)
- [ ] Window position/size is remembered
- [ ] Consistent spacing between controls
- [ ] Status bar shows record count and status

#### Feedback ✓
- [ ] Loading indicators for operations >200ms
- [ ] Success/error toast notifications
- [ ] Status bar updates during operations
- [ ] Progress bar for long operations

#### Validation ✓
- [ ] Required fields validated
- [ ] Format validation (email, phone, etc.)
- [ ] Range validation where applicable
- [ ] Clear error messages (not technical)
- [ ] Validation on leave, not just submit

#### Accessibility ✓
- [ ] Logical tab order (TabIndex)
- [ ] All controls have AccessibleName
- [ ] Keyboard shortcuts for common actions
- [ ] Focus indicators visible
- [ ] Color contrast meets WCAG 4.5:1

#### Performance ✓
- [ ] Async/await for all I/O operations
- [ ] Virtual mode for 1000+ rows
- [ ] Double buffering enabled
- [ ] No blocking UI thread

#### Colors ✓
- [ ] Using defined color palette (not random)
- [ ] Buttons contrast with background
- [ ] Semantic colors consistent (green=success, red=danger)
- [ ] Text readable on all backgrounds
- [ ] Works in grayscale (not color-only)

#### Polish ✓
- [ ] Consistent icons throughout
- [ ] Professional fonts
- [ ] Proper alignment
- [ ] No orphaned labels
- [ ] Dirty state indicator (*)

---

## Quick Reference: Common Mistakes

| ❌ Mistake | ✅ Production Standard |
|-----------|------------------------|
| Blank grid when no data | "No records found" with icon |
| Button same color as background | High contrast, min 4.5:1 ratio |
| No loading indicator | Spinner for any operation >200ms |
| "An error occurred" | Specific, actionable error message |
| Hardcoded strings | All text in .resx resource files |
| Fixed-size form | Anchor/Dock for responsive resize |
| No validation feedback | ErrorProvider + inline messages |
| Missing tab order | Logical TabIndex for all controls |
| No keyboard shortcuts | Alt+key accelerators |
| Synchronous data loading | async/await with cancellation |
| Random colors | Defined color palette |
| No paging for 1000+ records | Paging with page size selector |
| No sort/filter on grid | Click-to-sort + filter panel |
| Generic save button | "Save Customer" with Ctrl+S shortcut |

---

## Summary

This document defines the minimum standards for production-quality WinForms UI. Every form, control, and interaction must meet these standards.

**Key Principles:**
1. **Never ship demo-quality UI** - Every feature must be complete
2. **Users expect professional software** - Match their expectations
3. **Accessibility is mandatory** - Not optional
4. **Performance matters** - Users notice lag
5. **Consistency builds trust** - Same patterns everywhere

**Remember:** If it's not in this checklist, it's probably not production-ready.
