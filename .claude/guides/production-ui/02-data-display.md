# Data Display Controls

> Part of Production UI Standards

---

## DataGridView - MANDATORY Features

### Grid Layout Rule

**⚠️ CRITICAL: Grid MUST fill available space - NO empty gap below grid!**

```csharp
// ✅ CORRECT - Grid fills remaining space
dgvData.Dock = DockStyle.Fill;

// Layout structure:
// ┌─────────────────────────┐
// │ Filter Panel (Top)      │  ← Dock.Top
// ├─────────────────────────┤
// │                         │
// │   DataGridView          │  ← Dock.Fill
// │                         │
// ├─────────────────────────┤
// │ Paging Panel (Bottom)   │  ← Dock.Bottom
// └─────────────────────────┘

// ❌ WRONG - Fixed height leaves gap
dgvData.Height = 400;  // NEVER use fixed height
```

### Production Grid Configuration

```csharp
public void ConfigureProductionGrid(DataGridView dgv)
{
    // Visual styling
    dgv.BorderStyle = BorderStyle.None;
    dgv.EnableHeadersVisualStyles = false;
    dgv.ColumnHeadersDefaultCellStyle.BackColor = AppColors.Primary;
    dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
    dgv.ColumnHeadersDefaultCellStyle.Font = new Font(dgv.Font.FontFamily, 9f, FontStyle.Bold);
    dgv.ColumnHeadersHeight = 40;

    // Alternating rows
    dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);

    // Sorting - MANDATORY
    foreach (DataGridViewColumn col in dgv.Columns)
    {
        col.SortMode = DataGridViewColumnSortMode.Automatic;
    }

    // Column behavior
    dgv.AllowUserToResizeColumns = true;
    dgv.AllowUserToOrderColumns = true;
    dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

    // Row behavior
    dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    dgv.MultiSelect = false;
    dgv.AllowUserToAddRows = false;
    dgv.ReadOnly = true;
    dgv.RowTemplate.Height = 35;
}
```

---

## Grid Filtering

```csharp
public class GridFilterPanel : Panel
{
    private TextBox txtSearch;
    private ComboBox cboStatus;
    private DateTimePicker dtpFrom, dtpTo;
    private Button btnFilter, btnClear;

    public event EventHandler<FilterEventArgs> FilterApplied;

    public GridFilterPanel()
    {
        Height = 50;
        BackColor = AppColors.Background;
        Padding = new Padding(10);

        txtSearch = new TextBox { PlaceholderText = "Search...", Width = 200 };
        cboStatus = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
        cboStatus.Items.AddRange(new[] { "All", "Active", "Inactive" });

        // Layout with FlowLayoutPanel...
    }
}
```

---

## Grid Paging

```csharp
public class GridPagingPanel : Panel
{
    public int CurrentPage { get; private set; } = 1;
    public int PageSize { get; private set; } = 25;
    public int TotalRecords { get; private set; }
    public int TotalPages => (int)Math.Ceiling(TotalRecords / (double)PageSize);

    public event EventHandler PageChanged;

    public void UpdatePageInfo(int totalRecords)
    {
        TotalRecords = totalRecords;
        lblPageInfo.Text = $"Page {CurrentPage} of {TotalPages} ({TotalRecords:N0} records)";
        btnPrev.Enabled = CurrentPage > 1;
        btnNext.Enabled = CurrentPage < TotalPages;
    }
}
```

---

## Empty State & Loading

```csharp
// Empty state overlay
public class EmptyStateOverlay : Panel
{
    public EmptyStateOverlay(string message, string icon = "📭")
    {
        BackColor = AppColors.Surface;
        Dock = DockStyle.Fill;

        // Icon + message + action button
        Controls.Add(new Label { Text = icon, Font = new Font("Segoe UI", 48) });
        Controls.Add(new Label { Text = message, ForeColor = AppColors.TextSecondary });
        Controls.Add(new Button { Text = "Add New Item" });
    }
}

// Loading overlay
public class LoadingOverlay : Panel
{
    public LoadingOverlay(string message = "Loading")
    {
        BackColor = Color.FromArgb(200, 255, 255, 255);
        Dock = DockStyle.Fill;
        // Animated dots...
    }
}
```

---

## Context Menu

```csharp
public ContextMenuStrip CreateGridContextMenu(DataGridView dgv)
{
    var menu = new ContextMenuStrip();

    menu.Items.Add(new ToolStripMenuItem("View Details", null, (s, e) => ViewSelected()));
    menu.Items.Add(new ToolStripMenuItem("Edit", null, (s, e) => EditSelected()) { ShortcutKeys = Keys.F2 });
    menu.Items.Add(new ToolStripSeparator());
    menu.Items.Add(new ToolStripMenuItem("Delete", null, (s, e) => DeleteSelected()) { ShortcutKeys = Keys.Delete });
    menu.Items.Add(new ToolStripSeparator());

    var exportMenu = new ToolStripMenuItem("Export");
    exportMenu.DropDownItems.Add("Export to Excel", null, (s, e) => ExportToExcel());
    exportMenu.DropDownItems.Add("Export to CSV", null, (s, e) => ExportToCsv());
    menu.Items.Add(exportMenu);

    menu.Items.Add(new ToolStripMenuItem("Refresh", null, (s, e) => RefreshData()) { ShortcutKeys = Keys.F5 });

    dgv.ContextMenuStrip = menu;
    return menu;
}
```

---

## Checklist

- [ ] Grid has sortable columns
- [ ] Grid has filtering
- [ ] Grid has paging for large datasets
- [ ] Grid has alternating row colors
- [ ] Grid shows empty state when no data
- [ ] Grid shows loading indicator
- [ ] Grid has context menu
- [ ] Grid supports keyboard navigation
- [ ] Export to Excel/CSV available
