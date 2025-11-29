---
description: Create a dialog form with proper result handling and validation
---

You are tasked with creating a dialog form (modal window) for user input or confirmation.

---

## 🔥 STEP 0: MANDATORY Context Loading (DO THIS FIRST!)

**Before ANY code generation, you MUST:**

### 1. Read Project Configuration
```
READ: .claude/project-context.md
```
Extract: `UI_FRAMEWORK`, `DATABASE`, `PATTERN`, `FRAMEWORK`

### 2. Load Correct Form Template

| UI Framework | Template to Use |
|--------------|-----------------|
| **Standard** | `templates/form-template.cs` |
| **DevExpress** | `templates/dx-form-template.cs` |
| **ReaLTaiizor** | `templates/rt-material-form-template.cs` |

### 3. Critical Rules

| 🚫 NEVER | ✅ ALWAYS |
|----------|----------|
| Separate Label + TextBox | Floating Label/Hint |
| Generate without template | Start from template |
| Skip validation | Validate required fields |

**⚠️ If project-context.md doesn't exist**: Ask user for UI framework preference.

---

## Workflow

1. **Ask for Dialog Information**
   - Dialog purpose (Input, Confirmation, Selection, Progress, About)
   - What data needs to be collected/displayed?
   - What buttons are needed? (OK/Cancel, Yes/No/Cancel, Custom)
   - Does it need validation?

2. **Read Documentation**
   - Reference `docs/ui-ux/form-communication.md` for dialog patterns
   - Reference `docs/ui-ux/input-validation.md` for validation
   - Reference `templates/form-template.cs` for base structure

3. **Determine Dialog Type**

   ### Type 1: Input Dialog (Collect Data)
   - User enters data (text, numbers, dates, etc.)
   - Returns data via properties or custom result class
   - Has OK/Cancel buttons
   - Requires validation

   ### Type 2: Confirmation Dialog (Yes/No/Cancel)
   - Asks user to confirm an action
   - Simple message with buttons
   - Returns DialogResult
   - No validation needed

   ### Type 3: Selection Dialog (Choose from list)
   - User selects from options (ListBox, ComboBox, CheckedListBox)
   - Returns selected items
   - Has OK/Cancel buttons
   - May have search/filter

   ### Type 4: Progress Dialog (Show Progress)
   - Displays progress bar
   - Shows status messages
   - May have Cancel button
   - Updates via IProgress<T>

   ### Type 5: About Dialog (Show Info)
   - Displays application info
   - Version, copyright, credits
   - Only OK button
   - No input needed

4. **Generate Dialog Code**

### Example: Input Dialog (Most Common)

```csharp
using System;
using System.Windows.Forms;

namespace YourApp.Forms.Dialogs;

/// <summary>
/// Dialog for {purpose}
/// </summary>
public partial class {DialogName}Dialog : Form
{
    private ErrorProvider _errorProvider;

    #region Controls

    private Label _lblTitle;
    private TextBox _txtInput;
    private Label _lblDescription;
    private Button _btnOk;
    private Button _btnCancel;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets or sets the input value
    /// </summary>
    public string InputValue
    {
        get => _txtInput.Text.Trim();
        set => _txtInput.Text = value;
    }

    /// <summary>
    /// Gets or sets the dialog title
    /// </summary>
    public string Title
    {
        get => Text;
        set => Text = value;
    }

    /// <summary>
    /// Gets or sets the description text
    /// </summary>
    public string Description
    {
        get => _lblDescription.Text;
        set => _lblDescription.Text = value;
    }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="{DialogName}Dialog"/> class
    /// </summary>
    public {DialogName}Dialog()
    {
        InitializeComponent();
        InitializeCustom();
    }

    /// <summary>
    /// Initializes a new instance with initial value
    /// </summary>
    /// <param name="initialValue">The initial input value</param>
    public {DialogName}Dialog(string initialValue) : this()
    {
        InputValue = initialValue;
    }

    private void InitializeComponent()
    {
        SuspendLayout();

        // Form properties
        Text = "{Dialog Title}";
        Size = new Size(450, 250);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;

        // Title Label
        _lblTitle = new Label
        {
            Text = "{Title Text}",
            Location = new Point(20, 20),
            Size = new Size(400, 25),
            Font = new Font(Font.FontFamily, 12, FontStyle.Bold)
        };

        // Description Label
        _lblDescription = new Label
        {
            Text = "{Description text}",
            Location = new Point(20, 55),
            Size = new Size(400, 40),
            AutoSize = false
        };

        // Input TextBox
        _txtInput = new TextBox
        {
            Location = new Point(20, 105),
            Size = new Size(400, 25),
            TabIndex = 0
        };

        // OK Button
        _btnOk = new Button
        {
            Text = "&OK",
            DialogResult = DialogResult.OK,
            Location = new Point(245, 165),
            Size = new Size(85, 30),
            TabIndex = 1
        };
        _btnOk.Click += BtnOk_Click;

        // Cancel Button
        _btnCancel = new Button
        {
            Text = "&Cancel",
            DialogResult = DialogResult.Cancel,
            Location = new Point(335, 165),
            Size = new Size(85, 30),
            TabIndex = 2
        };

        // Add controls to form
        Controls.AddRange(new Control[]
        {
            _lblTitle,
            _lblDescription,
            _txtInput,
            _btnOk,
            _btnCancel
        });

        // Set form buttons
        AcceptButton = _btnOk;
        CancelButton = _btnCancel;

        ResumeLayout(false);
        PerformLayout();
    }

    private void InitializeCustom()
    {
        // Initialize ErrorProvider
        _errorProvider = new ErrorProvider
        {
            BlinkStyle = ErrorBlinkStyle.NeverBlink
        };

        // Focus on input when shown
        Shown += (s, e) =>
        {
            _txtInput.Focus();
            _txtInput.SelectAll();
        };
    }

    private void BtnOk_Click(object? sender, EventArgs e)
    {
        // Clear previous errors
        _errorProvider.Clear();

        // Validate input
        if (!ValidateInput())
        {
            DialogResult = DialogResult.None; // Prevent dialog from closing
            return;
        }

        // Dialog will close with OK result
    }

    private bool ValidateInput()
    {
        bool isValid = true;

        // Check if empty
        if (string.IsNullOrWhiteSpace(InputValue))
        {
            _errorProvider.SetError(_txtInput, "This field is required.");
            isValid = false;
        }
        // Check length
        else if (InputValue.Length < 3)
        {
            _errorProvider.SetError(_txtInput, "Must be at least 3 characters.");
            isValid = false;
        }
        else if (InputValue.Length > 100)
        {
            _errorProvider.SetError(_txtInput, "Cannot exceed 100 characters.");
            isValid = false;
        }

        // Add more validation as needed

        return isValid;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _errorProvider?.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Static Helper Methods

    /// <summary>
    /// Shows the dialog and returns the input value
    /// </summary>
    /// <param name="owner">The owner form</param>
    /// <param name="title">Dialog title</param>
    /// <param name="description">Description text</param>
    /// <param name="initialValue">Initial input value</param>
    /// <returns>The input value if OK was clicked, null if cancelled</returns>
    public static string? ShowDialog(
        IWin32Window? owner,
        string title,
        string description,
        string initialValue = "")
    {
        using var dialog = new {DialogName}Dialog(initialValue)
        {
            Title = title,
            Description = description
        };

        return dialog.ShowDialog(owner) == DialogResult.OK
            ? dialog.InputValue
            : null;
    }

    #endregion
}
```

### Example: Confirmation Dialog (Simple)

```csharp
public static class ConfirmationDialog
{
    /// <summary>
    /// Shows a confirmation dialog
    /// </summary>
    /// <param name="owner">The owner window</param>
    /// <param name="message">The confirmation message</param>
    /// <param name="title">Dialog title</param>
    /// <returns>True if Yes was clicked, false otherwise</returns>
    public static bool Show(
        IWin32Window? owner,
        string message,
        string title = "Confirm")
    {
        return MessageBox.Show(
            owner,
            message,
            title,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2) == DialogResult.Yes;
    }

    /// <summary>
    /// Shows a delete confirmation dialog
    /// </summary>
    public static bool ShowDelete(IWin32Window? owner, string itemName)
    {
        return Show(
            owner,
            $"Are you sure you want to delete '{itemName}'?\n\nThis action cannot be undone.",
            "Confirm Delete");
    }

    /// <summary>
    /// Shows a save changes confirmation dialog
    /// </summary>
    /// <returns>Yes, No, or Cancel</returns>
    public static DialogResult ShowSaveChanges(IWin32Window? owner)
    {
        return MessageBox.Show(
            owner,
            "Do you want to save changes?",
            "Save Changes",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button1);
    }
}
```

### Example: Selection Dialog (Choose Items)

```csharp
public partial class SelectItemDialog : Form
{
    private ListBox _lstItems;
    private TextBox _txtSearch;
    private Button _btnOk;
    private Button _btnCancel;

    private readonly List<string> _allItems;

    public string? SelectedItem => _lstItems.SelectedItem as string;

    public SelectItemDialog(List<string> items, string? selectedItem = null)
    {
        _allItems = items;

        InitializeComponent();
        LoadItems();

        if (!string.IsNullOrEmpty(selectedItem))
        {
            _lstItems.SelectedItem = selectedItem;
        }

        _txtSearch.TextChanged += (s, e) => FilterItems();
    }

    private void LoadItems()
    {
        _lstItems.Items.Clear();
        _lstItems.Items.AddRange(_allItems.ToArray());
    }

    private void FilterItems()
    {
        var filter = _txtSearch.Text;
        _lstItems.Items.Clear();

        var filtered = string.IsNullOrWhiteSpace(filter)
            ? _allItems
            : _allItems.Where(i => i.Contains(filter, StringComparison.OrdinalIgnoreCase));

        _lstItems.Items.AddRange(filtered.ToArray());
    }

    public static string? ShowDialog(IWin32Window? owner, List<string> items, string? selectedItem = null)
    {
        using var dialog = new SelectItemDialog(items, selectedItem);
        return dialog.ShowDialog(owner) == DialogResult.OK
            ? dialog.SelectedItem
            : null;
    }
}
```

### Example: Progress Dialog

```csharp
public partial class ProgressDialog : Form
{
    private ProgressBar _progressBar;
    private Label _lblStatus;
    private Button _btnCancel;

    public CancellationTokenSource CancellationTokenSource { get; private set; }
    public IProgress<ProgressReport> Progress { get; private set; }

    public ProgressDialog()
    {
        InitializeComponent();
        CancellationTokenSource = new CancellationTokenSource();

        Progress = new Progress<ProgressReport>(report =>
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateProgress(report)));
            }
            else
            {
                UpdateProgress(report);
            }
        });
    }

    private void UpdateProgress(ProgressReport report)
    {
        _progressBar.Value = report.Percentage;
        _lblStatus.Text = report.Message;

        if (report.IsComplete)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    private void BtnCancel_Click(object? sender, EventArgs e)
    {
        CancellationTokenSource.Cancel();
        _btnCancel.Enabled = false;
        _lblStatus.Text = "Cancelling...";
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            CancellationTokenSource?.Dispose();
        }
        base.Dispose(disposing);
    }
}

public class ProgressReport
{
    public int Percentage { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
}
```

5. **Usage Examples**

Show the user how to use the dialog:

```csharp
// Input Dialog
var result = InputDialog.ShowDialog(this, "Enter Name", "Please enter your name:");
if (result != null)
{
    MessageBox.Show($"You entered: {result}");
}

// Confirmation Dialog
if (ConfirmationDialog.ShowDelete(this, "Customer XYZ"))
{
    // Delete the customer
}

// Selection Dialog
var items = new List<string> { "Option 1", "Option 2", "Option 3" };
var selected = SelectItemDialog.ShowDialog(this, items);
if (selected != null)
{
    MessageBox.Show($"Selected: {selected}");
}

// Progress Dialog
var progressDialog = new ProgressDialog();
progressDialog.Show(this);

await ProcessDataAsync(progressDialog.Progress, progressDialog.CancellationTokenSource.Token);
```

6. **Suggest Next Steps**

Ask: "Would you like me to:
- Add more validation rules?
- Create a custom result class for complex data?
- Add data binding for the dialog?
- Generate unit tests for validation logic?"

## Best Practices Checklist

Before finishing, verify:
- ✅ FormBorderStyle = FixedDialog
- ✅ StartPosition = CenterParent
- ✅ MaximizeBox = false, MinimizeBox = false
- ✅ ShowInTaskbar = false
- ✅ AcceptButton and CancelButton set
- ✅ DialogResult properly set on buttons
- ✅ Validation prevents closing if invalid
- ✅ ErrorProvider for validation messages
- ✅ Focus set to first input control on Shown
- ✅ Proper disposal of resources
- ✅ Static helper method for easy usage
- ✅ XML documentation on public members

## Common Dialog Types

### 1. Input Dialog
- Single or multiple inputs
- Text, numbers, dates
- Validation required
- Returns data via properties

### 2. Confirmation Dialog
- Yes/No/Cancel
- Warning/Question icons
- Simple MessageBox wrapper
- Returns DialogResult

### 3. Selection Dialog
- ListBox/ComboBox
- Search/filter capability
- Single or multi-select
- Returns selected items

### 4. Progress Dialog
- ProgressBar
- Status messages
- Cancellation support
- IProgress<T> pattern

### 5. About Dialog
- App name, version
- Copyright, credits
- Logo/icon
- Links to website/support

## Dialog Design Guidelines

### Size
- Small: 300x150 (confirmation)
- Medium: 450x250 (input)
- Large: 600x400 (selection with list)

### Buttons
- Always right-aligned
- OK/Yes first, Cancel/No last
- Spacing: 10px between buttons
- Size: 85x30 or 100x30

### Layout
- Margin: 20px all sides
- Spacing between controls: 10-15px
- Label above input (5px gap)
- Buttons 20px from bottom

### Behavior
- Enter key = OK button
- Escape key = Cancel button
- Tab order logical
- Focus on first input
- Validate on OK click

## Notes

- **Always validate** before allowing OK to close
- **Use DialogResult** property on buttons
- **Prevent closing** by setting DialogResult = None
- **ShowDialog()** is blocking - UI freezes until closed
- **Use owner** parameter to center on parent
- **Dispose** properly if creating manually
- **Static helper methods** make usage easier
- **Thread-safe** progress updates with Invoke
