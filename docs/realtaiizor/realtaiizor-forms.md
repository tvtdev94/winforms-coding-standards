# ReaLTaiizor Forms Guide

> **Purpose**: Creating forms with ReaLTaiizor using MVP pattern
> **Audience**: WinForms developers building ReaLTaiizor applications

---

## Form Types

Each theme provides its own Form base class:

| Theme | Form Class | Example |
|-------|-----------|---------|
| Material | `MaterialForm` | Modern Google-style |
| Metro | `MetroForm` | Windows Metro style |
| Poison | `PoisonForm` | Minimalist flat |
| Crown | `CrownForm` | Professional |
| Cyber | `CyberForm` | Futuristic |

---

## Material Form with MVP

```csharp
using ReaLTaiizor.Forms;
using ReaLTaiizor.Controls;

namespace YourApp.Forms
{
    public partial class CustomerEditForm : MaterialForm, ICustomerEditView
    {
        private readonly CustomerPresenter _presenter;

        // Controls
        private MaterialTextBox txtName;
        private MaterialTextBox txtEmail;
        private MaterialComboBox cboType;
        private MaterialButton btnSave;
        private MaterialButton btnCancel;

        public CustomerEditForm(CustomerPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            _presenter.SetView(this);

            InitializeControls();
            WireUpEvents();
        }

        #region ICustomerEditView Properties

        public string CustomerName
        {
            get => txtName.Text;
            set => txtName.Text = value ?? string.Empty;
        }

        public string Email
        {
            get => txtEmail.Text;
            set => txtEmail.Text = value ?? string.Empty;
        }

        public int? CustomerTypeId
        {
            get => cboType.SelectedValue as int?;
            set => cboType.SelectedValue = value;
        }

        #endregion

        #region ICustomerEditView Events

        public event EventHandler LoadRequested;
        public event EventHandler SaveRequested;
        public event EventHandler CancelRequested;

        #endregion

        #region ICustomerEditView Methods

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowSuccess(string message)
        {
            MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        private void InitializeControls()
        {
            // Initialize Material controls
            this.Text = "Customer Edit";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void WireUpEvents()
        {
            this.Load += (s, e) => LoadRequested?.Invoke(this, EventArgs.Empty);
            btnSave.Click += (s, e) => SaveRequested?.Invoke(this, EventArgs.Empty);
            btnCancel.Click += (s, e) => CancelRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    // View Interface
    public interface ICustomerEditView
    {
        string CustomerName { get; set; }
        string Email { get; set; }
        int? CustomerTypeId { get; set; }

        event EventHandler LoadRequested;
        event EventHandler SaveRequested;
        event EventHandler CancelRequested;

        void ShowError(string message);
        void ShowSuccess(string message);
        void Close();
    }
}
```

---

## Best Practices

### ✅ DO

1. **Inherit from theme-specific form** (MaterialForm, MetroForm, etc.)
2. **Follow MVP pattern**
3. **Use IFormFactory** for dependency injection
4. **Validate inputs** before saving
5. **Handle events** properly

### ❌ DON'T

1. ❌ Put business logic in forms
2. ❌ Mix form types from different themes
3. ❌ Skip validation

---

**Last Updated**: 2025-11-17
