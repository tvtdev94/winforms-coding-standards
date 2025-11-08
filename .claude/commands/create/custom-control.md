---
description: Create a reusable custom UserControl
---

You are tasked with creating a custom UserControl that can be reused across multiple forms.

## Workflow

1. **Ask for Control Information**
   - Control name (e.g., "CustomerInfoPanel", "SearchBox", "StatusBar")
   - What is the purpose of this control?
   - What data/properties does it expose?
   - What events should it raise?
   - Does it need data binding support?

2. **Read Documentation**
   - Reference `docs/ui-ux/data-binding.md` for binding support
   - Reference `docs/best-practices/resource-management.md` for disposal

3. **Determine Control Type**

   ### Type 1: Composite Control (Common)
   - Combines multiple standard controls
   - Example: Address entry panel, Search box with button
   - Exposes properties for each field
   - Raises events for user actions

   ### Type 2: Enhanced Control
   - Extends existing control with new features
   - Example: Enhanced TextBox with validation, NumericUpDown with format
   - Inherits from base control
   - Adds new properties/events

   ### Type 3: Custom Drawn Control
   - Completely custom painting
   - Override OnPaint method
   - Example: Charts, gauges, custom buttons
   - Complex but full control

4. **Generate UserControl Code**

### Example 1: Composite Control (Most Common)

```csharp
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace YourApp.Controls;

/// <summary>
/// Custom control for {purpose}
/// </summary>
public partial class {ControlName} : UserControl
{
    #region Private Fields

    private bool _isDataChanged = false;

    #endregion

    #region Controls (Designer)

    private Label _lblField1;
    private TextBox _txtField1;
    private Label _lblField2;
    private TextBox _txtField2;
    private Button _btnAction;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets or sets the field 1 value
    /// </summary>
    [Category("Data")]
    [Description("The field 1 value")]
    [Bindable(true)]
    public string Field1
    {
        get => _txtField1.Text;
        set
        {
            if (_txtField1.Text != value)
            {
                _txtField1.Text = value;
                OnField1Changed(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Gets or sets the field 2 value
    /// </summary>
    [Category("Data")]
    [Description("The field 2 value")]
    [Bindable(true)]
    public string Field2
    {
        get => _txtField2.Text;
        set
        {
            if (_txtField2.Text != value)
            {
                _txtField2.Text = value;
                OnField2Changed(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Gets or sets whether the control is in read-only mode
    /// </summary>
    [Category("Behavior")]
    [Description("Indicates whether the control is read-only")]
    [DefaultValue(false)]
    public bool ReadOnly
    {
        get => _txtField1.ReadOnly;
        set
        {
            _txtField1.ReadOnly = value;
            _txtField2.ReadOnly = value;
            _btnAction.Enabled = !value;
        }
    }

    /// <summary>
    /// Gets whether the data has been modified
    /// </summary>
    [Browsable(false)]
    public bool IsDataChanged => _isDataChanged;

    #endregion

    #region Events

    /// <summary>
    /// Occurs when Field1 value changes
    /// </summary>
    [Category("Property Changed")]
    [Description("Occurs when Field1 value changes")]
    public event EventHandler? Field1Changed;

    /// <summary>
    /// Occurs when Field2 value changes
    /// </summary>
    [Category("Property Changed")]
    [Description("Occurs when Field2 value changes")]
    public event EventHandler? Field2Changed;

    /// <summary>
    /// Occurs when the action button is clicked
    /// </summary>
    [Category("Action")]
    [Description("Occurs when the action button is clicked")]
    public event EventHandler? ActionButtonClicked;

    /// <summary>
    /// Occurs when data changes
    /// </summary>
    [Category("Property Changed")]
    [Description("Occurs when any data in the control changes")]
    public event EventHandler? DataChanged;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="{ControlName}"/> class
    /// </summary>
    public {ControlName}()
    {
        InitializeComponent();
        InitializeCustom();
    }

    #endregion

    #region Initialization

    private void InitializeComponent()
    {
        SuspendLayout();

        // Set UserControl properties
        Name = "{ControlName}";
        Size = new Size(400, 120);
        BackColor = SystemColors.Control;

        // Label 1
        _lblField1 = new Label
        {
            Text = "Field 1:",
            Location = new Point(10, 15),
            Size = new Size(80, 20),
            TextAlign = ContentAlignment.MiddleRight
        };

        // TextBox 1
        _txtField1 = new TextBox
        {
            Location = new Point(100, 12),
            Size = new Size(280, 23),
            TabIndex = 0
        };

        // Label 2
        _lblField2 = new Label
        {
            Text = "Field 2:",
            Location = new Point(10, 50),
            Size = new Size(80, 20),
            TextAlign = ContentAlignment.MiddleRight
        };

        // TextBox 2
        _txtField2 = new TextBox
        {
            Location = new Point(100, 47),
            Size = new Size(280, 23),
            TabIndex = 1
        };

        // Action Button
        _btnAction = new Button
        {
            Text = "Action",
            Location = new Point(100, 82),
            Size = new Size(100, 28),
            TabIndex = 2
        };

        // Add controls
        Controls.AddRange(new Control[]
        {
            _lblField1,
            _txtField1,
            _lblField2,
            _txtField2,
            _btnAction
        });

        ResumeLayout(false);
        PerformLayout();
    }

    private void InitializeCustom()
    {
        // Wire up events
        _txtField1.TextChanged += TxtField1_TextChanged;
        _txtField2.TextChanged += TxtField2_TextChanged;
        _btnAction.Click += BtnAction_Click;

        // Set initial state
        _isDataChanged = false;
    }

    #endregion

    #region Event Handlers

    private void TxtField1_TextChanged(object? sender, EventArgs e)
    {
        _isDataChanged = true;
        OnField1Changed(EventArgs.Empty);
        OnDataChanged(EventArgs.Empty);
    }

    private void TxtField2_TextChanged(object? sender, EventArgs e)
    {
        _isDataChanged = true;
        OnField2Changed(EventArgs.Empty);
        OnDataChanged(EventArgs.Empty);
    }

    private void BtnAction_Click(object? sender, EventArgs e)
    {
        OnActionButtonClicked(EventArgs.Empty);
    }

    #endregion

    #region Event Raising Methods

    /// <summary>
    /// Raises the <see cref="Field1Changed"/> event
    /// </summary>
    protected virtual void OnField1Changed(EventArgs e)
    {
        Field1Changed?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="Field2Changed"/> event
    /// </summary>
    protected virtual void OnField2Changed(EventArgs e)
    {
        Field2Changed?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="ActionButtonClicked"/> event
    /// </summary>
    protected virtual void OnActionButtonClicked(EventArgs e)
    {
        ActionButtonClicked?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="DataChanged"/> event
    /// </summary>
    protected virtual void OnDataChanged(EventArgs e)
    {
        DataChanged?.Invoke(this, e);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Clears all fields in the control
    /// </summary>
    public void Clear()
    {
        _txtField1.Clear();
        _txtField2.Clear();
        _isDataChanged = false;
    }

    /// <summary>
    /// Validates the control's data
    /// </summary>
    /// <returns>True if valid, false otherwise</returns>
    public bool ValidateData()
    {
        // Add validation logic
        if (string.IsNullOrWhiteSpace(Field1))
        {
            MessageBox.Show("Field 1 is required.", "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtField1.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(Field2))
        {
            MessageBox.Show("Field 2 is required.", "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtField2.Focus();
            return false;
        }

        return true;
    }

    /// <summary>
    /// Resets the data changed flag
    /// </summary>
    public void ResetDataChangedFlag()
    {
        _isDataChanged = false;
    }

    #endregion

    #region Cleanup

    /// <summary>
    /// Clean up any resources being used
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Unsubscribe from events
            if (_txtField1 != null)
                _txtField1.TextChanged -= TxtField1_TextChanged;

            if (_txtField2 != null)
                _txtField2.TextChanged -= TxtField2_TextChanged;

            if (_btnAction != null)
                _btnAction.Click -= BtnAction_Click;
        }

        base.Dispose(disposing);
    }

    #endregion
}
```

### Example 2: Enhanced TextBox (Validation)

```csharp
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace YourApp.Controls;

/// <summary>
/// Enhanced TextBox with built-in validation
/// </summary>
public class ValidatedTextBox : TextBox
{
    private ErrorProvider _errorProvider;
    private string _validationErrorMessage = string.Empty;

    [Category("Validation")]
    [Description("The validation error message")]
    public string ValidationErrorMessage
    {
        get => _validationErrorMessage;
        set => _validationErrorMessage = value;
    }

    [Category("Validation")]
    [Description("Indicates if the field is required")]
    [DefaultValue(false)]
    public bool Required { get; set; }

    [Category("Validation")]
    [Description("Minimum length for the text")]
    [DefaultValue(0)]
    public int MinLength { get; set; }

    [Category("Validation")]
    [Description("Maximum length for the text")]
    [DefaultValue(int.MaxValue)]
    public int MaxLength { get; set; }

    [Category("Validation")]
    [Description("Regex pattern for validation")]
    public string? ValidationPattern { get; set; }

    [Browsable(false)]
    public bool IsValid { get; private set; } = true;

    public ValidatedTextBox()
    {
        _errorProvider = new ErrorProvider
        {
            BlinkStyle = ErrorBlinkStyle.NeverBlink
        };

        Validating += ValidatedTextBox_Validating;
    }

    private void ValidatedTextBox_Validating(object? sender, CancelEventArgs e)
    {
        Validate();
    }

    public new bool Validate()
    {
        _errorProvider.SetError(this, string.Empty);
        IsValid = true;

        // Required validation
        if (Required && string.IsNullOrWhiteSpace(Text))
        {
            _errorProvider.SetError(this, ValidationErrorMessage ?? "This field is required.");
            IsValid = false;
            return false;
        }

        // Min length validation
        if (MinLength > 0 && Text.Length < MinLength)
        {
            _errorProvider.SetError(this, $"Minimum length is {MinLength} characters.");
            IsValid = false;
            return false;
        }

        // Max length validation
        if (Text.Length > MaxLength)
        {
            _errorProvider.SetError(this, $"Maximum length is {MaxLength} characters.");
            IsValid = false;
            return false;
        }

        // Pattern validation
        if (!string.IsNullOrEmpty(ValidationPattern))
        {
            var regex = new System.Text.RegularExpressions.Regex(ValidationPattern);
            if (!regex.IsMatch(Text))
            {
                _errorProvider.SetError(this, ValidationErrorMessage ?? "Invalid format.");
                IsValid = false;
                return false;
            }
        }

        return true;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _errorProvider?.Dispose();
            Validating -= ValidatedTextBox_Validating;
        }
        base.Dispose(disposing);
    }
}
```

### Example 3: Custom Drawn Control (StatusIndicator)

```csharp
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace YourApp.Controls;

public enum IndicatorStatus
{
    None,
    Success,
    Warning,
    Error,
    Info
}

/// <summary>
/// Custom status indicator control
/// </summary>
public class StatusIndicator : Control
{
    private IndicatorStatus _status = IndicatorStatus.None;
    private string _statusText = string.Empty;

    [Category("Appearance")]
    [Description("The current status")]
    [DefaultValue(IndicatorStatus.None)]
    public IndicatorStatus Status
    {
        get => _status;
        set
        {
            if (_status != value)
            {
                _status = value;
                Invalidate(); // Trigger repaint
            }
        }
    }

    [Category("Appearance")]
    [Description("The status text")]
    public string StatusText
    {
        get => _statusText;
        set
        {
            if (_statusText != value)
            {
                _statusText = value;
                Invalidate();
            }
        }
    }

    public StatusIndicator()
    {
        SetStyle(ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.DoubleBuffer |
                 ControlStyles.ResizeRedraw, true);

        Size = new Size(200, 40);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Draw background
        using (var brush = new SolidBrush(BackColor))
        {
            g.FillRectangle(brush, ClientRectangle);
        }

        // Draw indicator circle
        var circleRect = new Rectangle(5, (Height - 30) / 2, 30, 30);
        var color = GetStatusColor();

        using (var brush = new SolidBrush(color))
        {
            g.FillEllipse(brush, circleRect);
        }

        // Draw text
        if (!string.IsNullOrEmpty(StatusText))
        {
            using (var brush = new SolidBrush(ForeColor))
            {
                var textRect = new RectangleF(45, 0, Width - 50, Height);
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center
                };

                g.DrawString(StatusText, Font, brush, textRect, format);
            }
        }
    }

    private Color GetStatusColor()
    {
        return Status switch
        {
            IndicatorStatus.Success => Color.Green,
            IndicatorStatus.Warning => Color.Orange,
            IndicatorStatus.Error => Color.Red,
            IndicatorStatus.Info => Color.Blue,
            _ => Color.Gray
        };
    }
}
```

5. **Usage Examples**

```csharp
// Using the custom control in a form
var customControl = new {ControlName}
{
    Location = new Point(20, 20),
    Size = new Size(400, 120)
};

// Subscribe to events
customControl.DataChanged += (s, e) =>
{
    Console.WriteLine("Data changed!");
};

customControl.ActionButtonClicked += (s, e) =>
{
    if (customControl.ValidateData())
    {
        // Process data
        var field1 = customControl.Field1;
        var field2 = customControl.Field2;
    }
};

// Add to form
Controls.Add(customControl);
```

6. **Suggest Next Steps**

Ask: "Would you like me to:
- Add data binding support?
- Create a designer for visual editing?
- Add more properties/events?
- Generate usage examples?"

## Best Practices Checklist

Before finishing, verify:
- ✅ Inherits from UserControl (composite) or base control (enhanced)
- ✅ Properties use [Category], [Description], [DefaultValue] attributes
- ✅ Bindable properties marked with [Bindable(true)]
- ✅ Events follow .NET naming (EventName + EventArgs)
- ✅ Protected virtual On{EventName} methods
- ✅ Proper disposal of resources
- ✅ Events unsubscribed in Dispose
- ✅ SuspendLayout/ResumeLayout in InitializeComponent
- ✅ XML documentation on public members
- ✅ Clear() and Reset() methods for data controls
- ✅ Validation methods return bool
- ✅ DoubleBuffer for custom paint controls

## Control Design Attributes

```csharp
// Property attributes
[Category("Appearance")]       // Properties window category
[Description("...")]           // Property description
[DefaultValue(false)]          // Default value
[Bindable(true)]              // Supports data binding
[Browsable(false)]            // Hide from designer

// Event attributes
[Category("Action")]
[Description("Occurs when...")]

// Control attributes
[ToolboxItem(true)]           // Show in toolbox
[DesignTimeVisible(true)]     // Visible at design time
[DefaultEvent("Click")]       // Default event in designer
[DefaultProperty("Text")]     // Default property in designer
```

## Common Custom Control Types

### 1. Data Entry Controls
- Combined input fields (Address, Name, etc.)
- Validated inputs
- Formatted inputs (Phone, SSN, etc.)

### 2. Display Controls
- Status indicators
- Progress displays
- Custom charts/graphs

### 3. Navigation Controls
- Breadcrumbs
- Tab strips
- Menu systems

### 4. Container Controls
- Panels with headers
- Collapsible sections
- Wizard steps

## Performance Tips

- Use `SuspendLayout()` and `ResumeLayout()` when adding multiple controls
- Set `DoubleBuffer = true` for custom paint controls
- Cache brushes and pens in custom paint
- Use `Invalidate(rectangle)` instead of `Invalidate()` for partial updates
- Minimize work in `OnPaint` - prepare data beforehand

## Notes

- **Designer Support**: Controls can be dragged from toolbox if added to project
- **Property Grid**: Attributes control how properties appear in designer
- **Events**: Raise events using protected virtual methods
- **Validation**: Provide `Validate()` method returning bool
- **Disposal**: Always unsubscribe events in Dispose
- **Thread Safety**: Use Invoke if updating from background thread
- **Custom Paint**: Override OnPaint for full drawing control
