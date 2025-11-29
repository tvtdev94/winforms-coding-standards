# Form Layout & Responsive Design

> Part of [Production UI Standards](../production-ui-standards.md)

---

## Responsive Layout Requirements

**Every form MUST:**

```csharp
public partial class ProductionForm : Form
{
    public ProductionForm()
    {
        InitializeComponent();

        // REQUIRED: Set minimum size
        MinimumSize = new Size(800, 600);

        // REQUIRED: Start maximized
        StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Maximized;

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

---

## Layout Patterns

### Master-Detail Layout

```csharp
public void SetupMasterDetailLayout()
{
    var splitContainer = new SplitContainer
    {
        Dock = DockStyle.Fill,
        Orientation = Orientation.Vertical,
        SplitterDistance = 300,
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
```

### Standard Form Layout

```csharp
public void SetupStandardLayout()
{
    // Toolbar at top
    var toolStrip = new ToolStrip { Dock = DockStyle.Top };

    // Status bar at bottom
    var statusStrip = new StatusStrip { Dock = DockStyle.Bottom };

    // Main content fills remaining space
    var contentPanel = new Panel { Dock = DockStyle.Fill };

    // Add in correct order (reverse of visual order)
    Controls.Add(contentPanel);
    Controls.Add(statusStrip);
    Controls.Add(toolStrip);
}
```

---

## Spacing Standards

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

## Anchor vs Dock

| Use | When |
|-----|------|
| **Dock.Fill** | Main content area, grids |
| **Dock.Top/Bottom** | Toolbars, status bars, filter panels |
| **Anchor** | Buttons that stay in corner, controls that stretch |

```csharp
// Grid fills all remaining space
dgvData.Dock = DockStyle.Fill;

// Buttons stay bottom-right
btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

// TextBox stretches horizontally
txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
```

---

## Checklist

- [ ] Form has MinimumSize set
- [ ] Form resizes properly (Anchor/Dock)
- [ ] Window position/size remembered
- [ ] Consistent spacing between controls
- [ ] Status bar shows record count and status
- [ ] Grid fills available space (Dock.Fill)
