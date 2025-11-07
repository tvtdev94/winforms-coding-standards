# Keyboard Navigation in WinForms

> **Quick Reference**: Comprehensive guide to keyboard navigation, accessibility, and keyboard shortcuts for professional WinForms applications.

---

## 📋 Overview

Keyboard navigation enables users to interact with your application without a mouse, improving accessibility and productivity. Proper keyboard support is essential for power users, accessibility compliance, and professional applications.

**This guide covers**:
- Tab order management and focus flow
- Keyboard shortcuts and accelerators
- Access keys (mnemonics) for quick navigation
- Keyboard event handling patterns
- Focus management and visual feedback
- Accessibility features and WCAG compliance
- Custom navigation implementations

---

## 🎯 Why This Matters

### Accessibility
✅ **WCAG 2.1 Compliance** - keyboard access is a legal requirement for many applications
✅ **Screen Reader Support** - enables users with visual impairments
✅ **Motor Disability Support** - essential for users who cannot use a mouse
✅ **Inclusive Design** - makes your app usable by everyone

### Productivity
✅ **Power User Efficiency** - keyboard shortcuts are faster than mouse
✅ **Data Entry Speed** - tab through forms without lifting hands from keyboard
✅ **Workflow Optimization** - reduces repetitive mouse movements
✅ **Reduced Context Switching** - keep hands on keyboard

### Professional Quality
✅ **Industry Standards** - professional apps have full keyboard support
✅ **User Expectations** - users expect standard shortcuts (Ctrl+S, Ctrl+C, etc.)
✅ **Competitive Advantage** - better UX than mouse-only apps

---

## 🎛️ Tab Order Management

### TabIndex Property

The `TabIndex` property determines the order in which controls receive focus when pressing Tab.

#### How Tab Order Works

```csharp
public partial class CustomerForm : Form
{
    public CustomerForm()
    {
        InitializeComponent();

        // Tab order: txtName (0) -> txtEmail (1) -> txtPhone (2) -> btnSave (3)
        txtName.TabIndex = 0;
        txtEmail.TabIndex = 1;
        txtPhone.TabIndex = 2;
        btnSave.TabIndex = 3;
        btnCancel.TabIndex = 4;
    }
}
```

#### Setting Logical Tab Order

Tab order should follow the **visual flow** of the form (usually left-to-right, top-to-bottom).

```csharp
// ✅ GOOD: Logical flow
txtFirstName.TabIndex = 0;    // Top-left
txtLastName.TabIndex = 1;     // Top-right
txtAddress.TabIndex = 2;      // Middle
txtCity.TabIndex = 3;         // Bottom-left
txtZip.TabIndex = 4;          // Bottom-right
btnSave.TabIndex = 5;         // Action buttons last
btnCancel.TabIndex = 6;

// ❌ BAD: Jumping around the form
txtFirstName.TabIndex = 0;
btnSave.TabIndex = 1;         // Skips to bottom
txtLastName.TabIndex = 2;     // Jumps back to top
```

#### Container Controls and Tab Order

Container controls (Panel, GroupBox, TabControl) have their own tab order scope.

```csharp
public partial class OrderForm : Form
{
    public OrderForm()
    {
        InitializeComponent();

        // Panel container has TabIndex in parent form
        pnlCustomerInfo.TabIndex = 0;
        pnlOrderDetails.TabIndex = 1;

        // Controls inside panel have independent tab order
        // First, all controls in pnlCustomerInfo (0-2), then pnlOrderDetails (0-3)

        // pnlCustomerInfo controls
        txtCustomerName.TabIndex = 0;  // Focus order: 1st
        txtCustomerEmail.TabIndex = 1; // Focus order: 2nd
        txtCustomerPhone.TabIndex = 2; // Focus order: 3rd

        // pnlOrderDetails controls (tabbed to after pnlCustomerInfo)
        txtProductName.TabIndex = 0;   // Focus order: 4th
        nudQuantity.TabIndex = 1;      // Focus order: 5th
        dtpOrderDate.TabIndex = 2;     // Focus order: 6th
    }
}
```

---

### TabStop Property

The `TabStop` property controls whether a control can receive focus via Tab key.

#### Skipping Controls in Tab Order

```csharp
public partial class DataEntryForm : Form
{
    public CustomerForm()
    {
        InitializeComponent();

        // Input controls - can receive focus
        txtName.TabStop = true;
        txtEmail.TabStop = true;

        // Labels - skip in tab order
        lblName.TabStop = false;
        lblEmail.TabStop = false;

        // Read-only displays - skip in tab order
        lblTotalAmount.TabStop = false;
        txtReadOnlyStatus.TabStop = false;

        // Buttons - can receive focus
        btnSave.TabStop = true;
        btnCancel.TabStop = true;
    }
}
```

#### Best Practices for TabStop

```csharp
// ✅ DO: Skip decorative and read-only controls
lblTitle.TabStop = false;            // Static text
picLogo.TabStop = false;             // Image
pnlSeparator.TabStop = false;        // Visual separator
txtCalculatedTotal.TabStop = false;  // Calculated read-only field

// ✅ DO: Include interactive controls
txtUserInput.TabStop = true;         // Editable field
btnSubmit.TabStop = true;            // Clickable button
cbxOptions.TabStop = true;           // Selectable dropdown
chkAgree.TabStop = true;             // Checkable control

// ❌ DON'T: Make required input controls non-tabbable
txtRequiredField.TabStop = false;    // User can't reach it!
```

---

### Setting Tab Order in Designer

**Visual Studio Designer Method**:
1. Open form in Designer
2. Go to **View → Tab Order** (or press Ctrl+D)
3. Click controls in desired order
4. Press Esc when done

**Benefits**:
- Visual feedback of tab order
- Easy to reorder without code
- See the complete flow at a glance

**When to Use Code vs Designer**:
- **Designer**: Static forms, simple layouts
- **Code**: Dynamic controls, runtime-generated UI

---

## ⌨️ Keyboard Shortcuts

### Access Keys (Mnemonics)

Access keys allow users to activate controls using **Alt + Letter**.

#### Using & in Control Text

```csharp
public partial class MenuForm : Form
{
    public MenuForm()
    {
        InitializeComponent();

        // & before letter creates access key
        btnSave.Text = "&Save";        // Alt+S
        btnCancel.Text = "&Cancel";    // Alt+C
        btnExport.Text = "&Export";    // Alt+E
        btnPrint.Text = "&Print";      // Alt+P

        // Avoid duplicates on same form
        btnNew.Text = "&New";          // Alt+N
        btnOpen.Text = "&Open";        // Alt+O (not Alt+N)

        // Use && to display actual ampersand
        lblCompany.Text = "Smith && Sons"; // Displays "Smith & Sons"
    }
}
```

#### Label Shortcuts for TextBox

Labels can provide access keys for associated controls.

```csharp
public partial class CustomerForm : Form
{
    public CustomerForm()
    {
        InitializeComponent();

        // Label access key focuses associated control
        lblName.Text = "&Name:";
        lblName.UseMnemonic = true;    // Enable mnemonic processing (default: true)

        // Set TabIndex to link label to next control
        lblName.TabIndex = 0;
        txtName.TabIndex = 1;          // Pressing Alt+N focuses txtName

        lblEmail.Text = "&Email:";
        lblEmail.TabIndex = 2;
        txtEmail.TabIndex = 3;         // Alt+E focuses txtEmail

        lblPhone.Text = "&Phone:";
        lblPhone.TabIndex = 4;
        txtPhone.TabIndex = 5;         // Alt+P focuses txtPhone
    }
}
```

#### Best Practices for Access Keys

```csharp
// ✅ GOOD: Logical mnemonics
btnSave.Text = "&Save";        // S
btnDelete.Text = "&Delete";    // D
btnRefresh.Text = "&Refresh";  // R

// ✅ GOOD: Use first letter when possible
btnNew.Text = "&New";
btnOpen.Text = "&Open";
btnClose.Text = "&Close";

// ✅ GOOD: Use consonants for clarity
btnApply.Text = "A&pply";      // P (not A, already used)
btnUpdate.Text = "&Update";    // U

// ❌ BAD: Duplicate access keys on same form
btnSave.Text = "&Save";        // S
btnSearch.Text = "&Search";    // S - conflict!

// ❌ BAD: Unclear mnemonics
btnDelete.Text = "De&lete";    // L - not intuitive
```

---

### Shortcut Keys

Shortcut keys execute commands directly without Alt modifier.

#### KeyDown Event

```csharp
public partial class EditorForm : Form
{
    public EditorForm()
    {
        InitializeComponent();

        // Enable key preview to receive keys before controls
        this.KeyPreview = true;

        // Handle KeyDown for shortcuts
        this.KeyDown += EditorForm_KeyDown;
    }

    private void EditorForm_KeyDown(object sender, KeyEventArgs e)
    {
        // Ctrl+S - Save
        if (e.Control && e.KeyCode == Keys.S)
        {
            SaveDocument();
            e.Handled = true;  // Prevent further processing
            e.SuppressKeyPress = true;  // Prevent beep
        }

        // Ctrl+N - New
        if (e.Control && e.KeyCode == Keys.N)
        {
            CreateNewDocument();
            e.Handled = true;
        }

        // Ctrl+O - Open
        if (e.Control && e.KeyCode == Keys.O)
        {
            OpenDocument();
            e.Handled = true;
        }

        // Ctrl+F - Find
        if (e.Control && e.KeyCode == Keys.F)
        {
            ShowFindDialog();
            e.Handled = true;
        }

        // F5 - Refresh
        if (e.KeyCode == Keys.F5)
        {
            RefreshData();
            e.Handled = true;
        }

        // Delete - Remove selected item
        if (e.KeyCode == Keys.Delete && dgvData.SelectedRows.Count > 0)
        {
            DeleteSelectedRows();
            e.Handled = true;
        }
    }
}
```

#### ProcessCmdKey Override

For more reliable shortcut handling, override `ProcessCmdKey`:

```csharp
public partial class MainForm : Form
{
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        // Ctrl+S - Save
        if (keyData == (Keys.Control | Keys.S))
        {
            SaveDocument();
            return true;  // Key handled
        }

        // Ctrl+Shift+S - Save As
        if (keyData == (Keys.Control | Keys.Shift | Keys.S))
        {
            SaveDocumentAs();
            return true;
        }

        // Ctrl+P - Print
        if (keyData == (Keys.Control | Keys.P))
        {
            PrintDocument();
            return true;
        }

        // Ctrl+Z - Undo
        if (keyData == (Keys.Control | Keys.Z))
        {
            Undo();
            return true;
        }

        // Ctrl+Y - Redo
        if (keyData == (Keys.Control | Keys.Y))
        {
            Redo();
            return true;
        }

        // F1 - Help
        if (keyData == Keys.F1)
        {
            ShowHelp();
            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }
}
```

#### Global vs Form-Level Shortcuts

```csharp
// Form-level shortcuts (KeyPreview = true)
public partial class DocumentForm : Form
{
    public DocumentForm()
    {
        InitializeComponent();
        this.KeyPreview = true;  // Form receives keys first
        this.KeyDown += DocumentForm_KeyDown;
    }

    private void DocumentForm_KeyDown(object sender, KeyEventArgs e)
    {
        // Only active when this form has focus
        if (e.Control && e.KeyCode == Keys.S)
        {
            SaveCurrentDocument();
            e.Handled = true;
        }
    }
}

// Application-level shortcuts
public class GlobalShortcutManager
{
    public static void RegisterGlobalShortcuts(Form mainForm)
    {
        // Register shortcuts that work across all forms
        Application.AddMessageFilter(new GlobalShortcutFilter());
    }
}

public class GlobalShortcutFilter : IMessageFilter
{
    public bool PreFilterMessage(ref Message m)
    {
        const int WM_KEYDOWN = 0x100;

        if (m.Msg == WM_KEYDOWN)
        {
            Keys key = (Keys)m.WParam | Control.ModifierKeys;

            // Ctrl+Q - Quit application (global)
            if (key == (Keys.Control | Keys.Q))
            {
                Application.Exit();
                return true;
            }
        }

        return false;
    }
}
```

---

### AcceptButton and CancelButton

Forms can designate default buttons for Enter and Escape keys.

#### Form.AcceptButton (Enter Key)

```csharp
public partial class LoginForm : Form
{
    public LoginForm()
    {
        InitializeComponent();

        // Enter key clicks Save button
        this.AcceptButton = btnLogin;

        // User can press Enter instead of clicking Login
    }
}
```

#### Form.CancelButton (Escape Key)

```csharp
public partial class DialogForm : Form
{
    public DialogForm()
    {
        InitializeComponent();

        // Escape key clicks Cancel button
        this.CancelButton = btnCancel;

        // Enter key clicks OK button
        this.AcceptButton = btnOK;
    }
}
```

#### Complete Dialog Example

```csharp
public partial class ConfirmationDialog : Form
{
    public ConfirmationDialog()
    {
        InitializeComponent();

        // Configure dialog buttons
        btnYes.DialogResult = DialogResult.Yes;
        btnNo.DialogResult = DialogResult.No;
        btnCancel.DialogResult = DialogResult.Cancel;

        // Keyboard shortcuts
        this.AcceptButton = btnYes;      // Enter = Yes
        this.CancelButton = btnCancel;   // Escape = Cancel

        // Access keys
        btnYes.Text = "&Yes";            // Alt+Y
        btnNo.Text = "&No";              // Alt+N
        btnCancel.Text = "&Cancel";      // Alt+C

        // Tab order
        btnYes.TabIndex = 0;
        btnNo.TabIndex = 1;
        btnCancel.TabIndex = 2;
    }
}

// Usage
var result = new ConfirmationDialog().ShowDialog();
if (result == DialogResult.Yes)
{
    // User pressed Enter or clicked Yes or pressed Alt+Y
}
```

---

## 🎛️ Keyboard Events

### KeyDown vs KeyPress vs KeyUp

Understanding when to use each event:

```csharp
public partial class EventDemoForm : Form
{
    public EventDemoForm()
    {
        InitializeComponent();

        txtInput.KeyDown += TxtInput_KeyDown;
        txtInput.KeyPress += TxtInput_KeyPress;
        txtInput.KeyUp += TxtInput_KeyUp;
    }

    // KeyDown - First event, access to Keys enum, modifier keys
    private void TxtInput_KeyDown(object sender, KeyEventArgs e)
    {
        lblKeyDown.Text = $"KeyDown: {e.KeyCode} | " +
                         $"Ctrl: {e.Control} | Shift: {e.Shift} | Alt: {e.Alt}";

        // Good for: Shortcuts, special keys (F1-F12, Ctrl+X, etc.)
        if (e.Control && e.KeyCode == Keys.V)
        {
            // Custom paste logic
            e.Handled = true;
        }
    }

    // KeyPress - Character event, access to actual character
    private void TxtInput_KeyPress(object sender, KeyPressEventArgs e)
    {
        lblKeyPress.Text = $"KeyPress: '{e.KeyChar}' (ASCII: {(int)e.KeyChar})";

        // Good for: Character validation, filtering input
        // Only allow digits
        if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
        {
            e.Handled = true;  // Block character
        }
    }

    // KeyUp - Last event, when key is released
    private void TxtInput_KeyUp(object sender, KeyEventArgs e)
    {
        lblKeyUp.Text = $"KeyUp: {e.KeyCode}";

        // Good for: Detecting key release, auto-search after typing stops
    }
}
```

#### When to Use Each Event

| Event | Use For | Access To |
|-------|---------|-----------|
| **KeyDown** | Shortcuts, special keys, modifiers | Keys enum, Ctrl/Shift/Alt |
| **KeyPress** | Character validation, text filtering | Actual character (char) |
| **KeyUp** | Key release detection, combo detection | Keys enum |

---

### Key Event Handling Examples

#### Preventing Default Behavior

```csharp
private void TxtNumeric_KeyPress(object sender, KeyPressEventArgs e)
{
    // Allow only digits and control characters (Backspace, Delete)
    if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
    {
        e.Handled = true;  // Prevent character from being entered
    }
}
```

#### Handling Special Keys

```csharp
private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
{
    switch (e.KeyCode)
    {
        case Keys.Enter:
            // Perform search
            PerformSearch();
            e.Handled = true;
            e.SuppressKeyPress = true;  // Prevent beep
            break;

        case Keys.Escape:
            // Clear search
            txtSearch.Clear();
            e.Handled = true;
            break;

        case Keys.Down:
            // Navigate to results
            if (lstResults.Items.Count > 0)
            {
                lstResults.Focus();
                lstResults.SelectedIndex = 0;
                e.Handled = true;
            }
            break;
    }
}
```

#### Custom Tab Handling

```csharp
private void TxtMultiline_KeyDown(object sender, KeyEventArgs e)
{
    // Allow Tab in multiline textbox (normally switches focus)
    if (e.KeyCode == Keys.Tab)
    {
        // Insert tab character instead of changing focus
        int selStart = txtMultiline.SelectionStart;
        txtMultiline.Text = txtMultiline.Text.Insert(selStart, "\t");
        txtMultiline.SelectionStart = selStart + 1;

        e.Handled = true;
        e.SuppressKeyPress = true;
    }
}
```

---

## 🎯 Focus Management

### Setting Focus

#### Control.Focus() Method

```csharp
public partial class LoginForm : Form
{
    public LoginForm()
    {
        InitializeComponent();
    }

    private void LoginForm_Load(object sender, EventArgs e)
    {
        // Set initial focus to username field
        txtUsername.Focus();
    }

    private void BtnLogin_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtUsername.Text))
        {
            MessageBox.Show("Username is required");
            txtUsername.Focus();  // Return focus to username
            return;
        }

        if (string.IsNullOrWhiteSpace(txtPassword.Text))
        {
            MessageBox.Show("Password is required");
            txtPassword.Focus();  // Focus password field
            return;
        }
    }
}
```

#### ActiveControl Property

```csharp
public partial class FormManager : Form
{
    private void SaveCurrentFocus()
    {
        // Get currently focused control
        Control? focused = this.ActiveControl;

        // Can check which control has focus
        if (focused == txtName)
        {
            // Name field has focus
        }
    }

    private void SetFocusToFirstError()
    {
        // Focus first control with validation error
        foreach (Control ctrl in this.Controls)
        {
            if (!string.IsNullOrEmpty(_errorProvider.GetError(ctrl)))
            {
                this.ActiveControl = ctrl;
                break;
            }
        }
    }
}
```

#### Focus After Validation Errors

```csharp
private void BtnSave_Click(object sender, EventArgs e)
{
    // Validate and focus first error
    if (!ValidateChildren(ValidationConstraints.Enabled))
    {
        // Find first control with error and focus it
        Control? firstError = FindFirstControlWithError();
        if (firstError != null)
        {
            firstError.Focus();
            MessageBox.Show("Please correct the highlighted errors",
                "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        return;
    }

    SaveData();
}

private Control? FindFirstControlWithError()
{
    foreach (Control ctrl in GetAllControls(this))
    {
        if (!string.IsNullOrEmpty(_errorProvider.GetError(ctrl)))
        {
            return ctrl;
        }
    }
    return null;
}

private IEnumerable<Control> GetAllControls(Control container)
{
    foreach (Control ctrl in container.Controls)
    {
        yield return ctrl;

        foreach (Control child in GetAllControls(ctrl))
        {
            yield return child;
        }
    }
}
```

---

### Focus Events

WinForms provides multiple focus events with subtle differences.

#### Enter vs GotFocus

```csharp
public partial class FocusEventForm : Form
{
    public FocusEventForm()
    {
        InitializeComponent();

        // Enter - Occurs when focus enters control
        txtName.Enter += TxtName_Enter;

        // GotFocus - Occurs after Enter
        txtName.GotFocus += TxtName_GotFocus;

        // Leave - Occurs when focus leaves control
        txtName.Leave += TxtName_Leave;

        // LostFocus - Occurs after Leave
        txtName.LostFocus += TxtName_LostFocus;
    }

    private void TxtName_Enter(object sender, EventArgs e)
    {
        // Highlight field when focused
        txtName.BackColor = Color.LightYellow;
        statusBar.Text = "Enter your full name";
    }

    private void TxtName_GotFocus(object sender, EventArgs e)
    {
        // Select all text for easy replacement
        txtName.SelectAll();
    }

    private void TxtName_Leave(object sender, EventArgs e)
    {
        // Remove highlight
        txtName.BackColor = SystemColors.Window;
        statusBar.Text = "";
    }

    private void TxtName_LostFocus(object sender, EventArgs e)
    {
        // Clear selection
        txtName.SelectionLength = 0;
    }
}
```

#### Validation on Focus Change

```csharp
private void TxtEmail_Leave(object sender, EventArgs e)
{
    // Validate when user leaves field
    if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !IsValidEmail(txtEmail.Text))
    {
        _errorProvider.SetError(txtEmail, "Invalid email format");
    }
    else
    {
        _errorProvider.SetError(txtEmail, string.Empty);
    }
}

private void TxtEmail_Enter(object sender, EventArgs e)
{
    // Show hint when field is focused
    statusBar.Text = "Enter email in format: user@example.com";
}
```

#### Visual Feedback on Focus

```csharp
private void SetupFocusVisualFeedback()
{
    foreach (Control ctrl in this.Controls)
    {
        if (ctrl is TextBox || ctrl is ComboBox)
        {
            ctrl.Enter += (s, e) =>
            {
                var control = (Control)s;
                control.BackColor = Color.FromArgb(255, 255, 200);  // Light yellow
            };

            ctrl.Leave += (s, e) =>
            {
                var control = (Control)s;
                control.BackColor = SystemColors.Window;  // White
            };
        }
    }
}
```

---

## ♿ Accessibility Features

### AutoScaleMode

DPI-aware scaling ensures proper rendering on high-DPI displays.

```csharp
public partial class AccessibleForm : Form
{
    public AccessibleForm()
    {
        InitializeComponent();

        // Enable DPI-aware scaling
        this.AutoScaleMode = AutoScaleMode.Dpi;

        // Alternative: Scale by font
        // this.AutoScaleMode = AutoScaleMode.Font;
    }
}
```

### ToolTips for Keyboard Hints

```csharp
public partial class MainForm : Form
{
    private ToolTip _toolTip;

    public MainForm()
    {
        InitializeComponent();

        _toolTip = new ToolTip
        {
            AutoPopDelay = 5000,
            InitialDelay = 500,
            ReshowDelay = 100,
            ShowAlways = true
        };

        // Show keyboard shortcuts in tooltips
        _toolTip.SetToolTip(btnSave, "Save (Ctrl+S)");
        _toolTip.SetToolTip(btnOpen, "Open (Ctrl+O)");
        _toolTip.SetToolTip(btnPrint, "Print (Ctrl+P)");
        _toolTip.SetToolTip(btnRefresh, "Refresh (F5)");

        // Show format hints
        _toolTip.SetToolTip(txtEmail, "Format: user@example.com");
        _toolTip.SetToolTip(txtPhone, "Format: 123-456-7890");
    }
}
```

### Status Bar Context Help

```csharp
public partial class FormWithHelp : Form
{
    public FormWithHelp()
    {
        InitializeComponent();

        // Show context-sensitive help in status bar
        txtUsername.Enter += (s, e) => statusLabel.Text = "Enter your username (Alt+U)";
        txtPassword.Enter += (s, e) => statusLabel.Text = "Enter your password (Alt+P)";
        btnLogin.Enter += (s, e) => statusLabel.Text = "Click to login (Enter) or press Alt+L";

        // Clear on leave
        txtUsername.Leave += (s, e) => statusLabel.Text = "";
        txtPassword.Leave += (s, e) => statusLabel.Text = "";
        btnLogin.Leave += (s, e) => statusLabel.Text = "";
    }
}
```

---

## 🛠️ Custom Keyboard Handling

### Arrow Key Navigation in Custom Control

```csharp
public class TilePanel : Panel
{
    private int _selectedIndex = 0;
    private List<Control> _tiles = new List<Control>();

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        switch (keyData)
        {
            case Keys.Right:
                SelectNext();
                return true;

            case Keys.Left:
                SelectPrevious();
                return true;

            case Keys.Enter:
                ActivateSelected();
                return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void SelectNext()
    {
        if (_tiles.Count == 0) return;
        _selectedIndex = (_selectedIndex + 1) % _tiles.Count;
        HighlightSelected();
    }

    private void SelectPrevious()
    {
        if (_tiles.Count == 0) return;
        _selectedIndex = (_selectedIndex - 1 + _tiles.Count) % _tiles.Count;
        HighlightSelected();
    }

    private void HighlightSelected()
    {
        for (int i = 0; i < _tiles.Count; i++)
        {
            _tiles[i].BackColor = i == _selectedIndex
                ? Color.LightBlue
                : SystemColors.Control;
        }
    }
}
```

### DataGridView Custom Navigation

```csharp
public partial class GridForm : Form
{
    public GridForm()
    {
        InitializeComponent();

        dgvData.KeyDown += DgvData_KeyDown;
    }

    private void DgvData_KeyDown(object sender, KeyEventArgs e)
    {
        // Ctrl+C - Copy selected cells
        if (e.Control && e.KeyCode == Keys.C)
        {
            CopySelectedCellsToClipboard();
            e.Handled = true;
        }

        // Delete - Remove selected rows
        if (e.KeyCode == Keys.Delete)
        {
            DeleteSelectedRows();
            e.Handled = true;
        }

        // F2 - Edit mode
        if (e.KeyCode == Keys.F2)
        {
            dgvData.BeginEdit(true);
            e.Handled = true;
        }

        // Enter - Save and move down
        if (e.KeyCode == Keys.Enter)
        {
            if (dgvData.CurrentCell.RowIndex < dgvData.Rows.Count - 1)
            {
                dgvData.CurrentCell = dgvData[
                    dgvData.CurrentCell.ColumnIndex,
                    dgvData.CurrentCell.RowIndex + 1];
            }
            e.Handled = true;
        }
    }
}
```

---

## ✅ Best Practices

### DO:
1. ✅ **Set logical tab order** - follow visual flow (left-to-right, top-to-bottom)
2. ✅ **Use access keys consistently** - Alt+S for Save, Alt+C for Cancel
3. ✅ **Set AcceptButton and CancelButton** - Enter and Escape should work in dialogs
4. ✅ **Enable KeyPreview** - handle form-level shortcuts
5. ✅ **Provide visual focus indicators** - highlight focused controls
6. ✅ **Set initial focus** - focus appropriate field on form load
7. ✅ **Handle standard shortcuts** - Ctrl+S, Ctrl+C, Ctrl+V, etc.
8. ✅ **Show shortcuts in tooltips** - help users discover shortcuts
9. ✅ **Skip decorative controls** - set TabStop = false for labels, panels
10. ✅ **Test keyboard-only navigation** - unplug mouse and test

### DON'T:
1. ❌ **Don't create duplicate access keys** - causes ambiguity
2. ❌ **Don't skip interactive controls** - all inputs should be tabbable
3. ❌ **Don't forget containers** - panels and groups have separate tab order
4. ❌ **Don't use illogical tab order** - jumping around confuses users
5. ❌ **Don't override standard shortcuts** - Ctrl+C should always copy
6. ❌ **Don't trap keyboard focus** - always allow Escape to exit
7. ❌ **Don't ignore accessibility** - keyboard navigation is legally required
8. ❌ **Don't forget to set e.Handled** - prevents unwanted beeps and behavior

---

## 🧪 Testing Keyboard Navigation

### Manual Testing Checklist

- [ ] **Tab through entire form** - reaches all interactive controls in logical order
- [ ] **Shift+Tab works** - navigates backwards through controls
- [ ] **Access keys work** - Alt+Letter activates correct controls
- [ ] **Enter submits** - AcceptButton works in dialogs
- [ ] **Escape cancels** - CancelButton works in dialogs
- [ ] **Shortcuts work** - Ctrl+S, Ctrl+C, F5, etc. function correctly
- [ ] **Visual focus indicator** - can see which control has focus
- [ ] **No focus traps** - can always navigate away from any control
- [ ] **Disabled controls skipped** - Tab skips disabled controls
- [ ] **Read-only controls skipped** - Labels don't receive focus

### Accessibility Testing Tools

```csharp
// Test helper: Log tab order
private void LogTabOrder()
{
    var controls = GetAllControls(this)
        .Where(c => c.TabStop)
        .OrderBy(c => c.TabIndex)
        .ToList();

    foreach (var ctrl in controls)
    {
        Debug.WriteLine($"TabIndex {ctrl.TabIndex}: {ctrl.Name} ({ctrl.GetType().Name})");
    }
}

// Test helper: Verify no duplicate access keys
private void VerifyNoDuplicateAccessKeys()
{
    var accessKeys = new Dictionary<char, string>();

    foreach (Control ctrl in GetAllControls(this))
    {
        string text = ctrl.Text;
        int ampersandIndex = text.IndexOf('&');

        if (ampersandIndex >= 0 && ampersandIndex < text.Length - 1)
        {
            char accessKey = char.ToUpper(text[ampersandIndex + 1]);

            if (accessKeys.ContainsKey(accessKey))
            {
                Debug.WriteLine($"WARNING: Duplicate access key '{accessKey}' " +
                    $"in {accessKeys[accessKey]} and {ctrl.Name}");
            }
            else
            {
                accessKeys[accessKey] = ctrl.Name;
            }
        }
    }
}
```

---

## 📚 Related Topics

- [Input Validation](input-validation.md) - Validate input during keyboard entry
- [Form Communication](form-communication.md) - Pass focus between forms
- [DataGridView Best Practices](datagridview-practices.md) - Grid keyboard navigation
- [Responsive Design](responsive-design.md) - DPI-aware layouts
- [Accessibility Standards](https://www.w3.org/WAI/WCAG21/quickref/) - WCAG 2.1 Guidelines

---

**Last Updated**: 2025-11-07
