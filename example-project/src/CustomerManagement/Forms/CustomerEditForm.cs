using CustomerManagement.Presenters;
using CustomerManagement.Services;
using CustomerManagement.Views;
using Microsoft.Extensions.Logging;

namespace CustomerManagement.Forms;

/// <summary>
/// Form for creating and editing customers.
/// Implements ICustomerEditView for MVP pattern.
/// </summary>
public partial class CustomerEditForm : Form, ICustomerEditView
{
    private readonly CustomerEditPresenter _presenter;
    private TextBox _txtName = null!;
    private TextBox _txtEmail = null!;
    private TextBox _txtPhone = null!;
    private TextBox _txtAddress = null!;
    private TextBox _txtCity = null!;
    private TextBox _txtCountry = null!;
    private CheckBox _chkIsActive = null!;
    private Button _btnSave = null!;
    private Button _btnCancel = null!;
    private ErrorProvider _errorProvider = null!;

    #region ICustomerEditView Events

    public event EventHandler? LoadRequested;
    public event EventHandler? SaveRequested;
    public event EventHandler? CancelRequested;

    #endregion

    #region ICustomerEditView Properties

    public int CustomerId { get; set; }

    public string CustomerName
    {
        get => _txtName.Text;
        set => _txtName.Text = value;
    }

    public string CustomerEmail
    {
        get => _txtEmail.Text;
        set => _txtEmail.Text = value;
    }

    public string CustomerPhone
    {
        get => _txtPhone.Text;
        set => _txtPhone.Text = value;
    }

    public string CustomerAddress
    {
        get => _txtAddress.Text;
        set => _txtAddress.Text = value;
    }

    public string CustomerCity
    {
        get => _txtCity.Text;
        set => _txtCity.Text = value;
    }

    public string CustomerCountry
    {
        get => _txtCountry.Text;
        set => _txtCountry.Text = value;
    }

    public bool IsActive
    {
        get => _chkIsActive.Checked;
        set => _chkIsActive.Checked = value;
    }

    public bool IsLoading
    {
        get => !_btnSave.Enabled;
        set
        {
            if (InvokeRequired)
            {
                Invoke(() => IsLoading = value);
                return;
            }

            _btnSave.Enabled = !value;
            _btnCancel.Enabled = !value;
            _txtName.Enabled = !value;
            _txtEmail.Enabled = !value;
            _txtPhone.Enabled = !value;
            _txtAddress.Enabled = !value;
            _txtCity.Enabled = !value;
            _txtCountry.Enabled = !value;
            _chkIsActive.Enabled = !value;
            Cursor = value ? Cursors.WaitCursor : Cursors.Default;
        }
    }

    public bool IsEditMode
    {
        get => CustomerId > 0;
        set
        {
            Text = value ? "Edit Customer" : "Add Customer";
            _btnSave.Text = value ? "&Update" : "&Save";
        }
    }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerEditForm"/> class.
    /// </summary>
    public CustomerEditForm(
        ICustomerService customerService,
        ILogger<CustomerEditPresenter> logger)
    {
        InitializeComponent();

        // Create presenter (MVP pattern)
        _presenter = new CustomerEditPresenter(this, customerService, logger);
    }

    /// <summary>
    /// Initializes the form components.
    /// </summary>
    private void InitializeComponent()
    {
        Text = "Add Customer";
        Size = new Size(500, 450);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        _errorProvider = new ErrorProvider
        {
            BlinkStyle = ErrorBlinkStyle.NeverBlink
        };

        // Create form layout
        var mainPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 8,
            Padding = new Padding(20),
            AutoSize = true
        };

        mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        // Name
        mainPanel.Controls.Add(CreateLabel("Name:*", 0), 0, 0);
        _txtName = CreateTextBox(1, 0);
        _txtName.MaxLength = 100;
        mainPanel.Controls.Add(_txtName, 1, 0);

        // Email
        mainPanel.Controls.Add(CreateLabel("Email:*", 0), 0, 1);
        _txtEmail = CreateTextBox(1, 1);
        _txtEmail.MaxLength = 100;
        mainPanel.Controls.Add(_txtEmail, 1, 1);

        // Phone
        mainPanel.Controls.Add(CreateLabel("Phone:", 0), 0, 2);
        _txtPhone = CreateTextBox(1, 2);
        _txtPhone.MaxLength = 20;
        mainPanel.Controls.Add(_txtPhone, 1, 2);

        // Address
        mainPanel.Controls.Add(CreateLabel("Address:", 0), 0, 3);
        _txtAddress = CreateTextBox(1, 3);
        _txtAddress.MaxLength = 200;
        mainPanel.Controls.Add(_txtAddress, 1, 3);

        // City
        mainPanel.Controls.Add(CreateLabel("City:", 0), 0, 4);
        _txtCity = CreateTextBox(1, 4);
        _txtCity.MaxLength = 50;
        mainPanel.Controls.Add(_txtCity, 1, 4);

        // Country
        mainPanel.Controls.Add(CreateLabel("Country:", 0), 0, 5);
        _txtCountry = CreateTextBox(1, 5);
        _txtCountry.MaxLength = 50;
        mainPanel.Controls.Add(_txtCountry, 1, 5);

        // Is Active
        _chkIsActive = new CheckBox
        {
            Text = "Active",
            Checked = true,
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 5, 0, 5)
        };
        mainPanel.Controls.Add(new Label(), 0, 6); // Spacer
        mainPanel.Controls.Add(_chkIsActive, 1, 6);

        // Buttons panel
        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            FlowDirection = FlowDirection.RightToLeft,
            Height = 50,
            Padding = new Padding(20, 10, 20, 10)
        };

        _btnCancel = new Button
        {
            Text = "&Cancel",
            Width = 90,
            Height = 30,
            DialogResult = DialogResult.Cancel
        };
        _btnCancel.Click += (s, e) => CancelRequested?.Invoke(this, EventArgs.Empty);

        _btnSave = new Button
        {
            Text = "&Save",
            Width = 90,
            Height = 30,
            Margin = new Padding(10, 0, 0, 0)
        };
        _btnSave.Click += (s, e) => SaveRequested?.Invoke(this, EventArgs.Empty);

        buttonPanel.Controls.Add(_btnCancel);
        buttonPanel.Controls.Add(_btnSave);

        // Add panels to form
        Controls.Add(mainPanel);
        Controls.Add(buttonPanel);

        // Set default button and cancel button
        AcceptButton = _btnSave;
        CancelButton = _btnCancel;

        // Wire up load event
        Load += (s, e) => LoadRequested?.Invoke(this, EventArgs.Empty);

        // Set tab order
        _txtName.TabIndex = 0;
        _txtEmail.TabIndex = 1;
        _txtPhone.TabIndex = 2;
        _txtAddress.TabIndex = 3;
        _txtCity.TabIndex = 4;
        _txtCountry.TabIndex = 5;
        _chkIsActive.TabIndex = 6;
        _btnSave.TabIndex = 7;
        _btnCancel.TabIndex = 8;
    }

    /// <summary>
    /// Creates a label control.
    /// </summary>
    private static Label CreateLabel(string text, int row)
    {
        return new Label
        {
            Text = text,
            AutoSize = true,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleRight,
            Margin = new Padding(0, 5, 10, 5)
        };
    }

    /// <summary>
    /// Creates a textbox control.
    /// </summary>
    private static TextBox CreateTextBox(int column, int row)
    {
        return new TextBox
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 5, 0, 5)
        };
    }

    #region ICustomerEditView Methods

    public void SetFieldError(string fieldName, string errorMessage)
    {
        Control? control = fieldName switch
        {
            nameof(CustomerName) => _txtName,
            nameof(CustomerEmail) => _txtEmail,
            nameof(CustomerPhone) => _txtPhone,
            nameof(CustomerAddress) => _txtAddress,
            nameof(CustomerCity) => _txtCity,
            nameof(CustomerCountry) => _txtCountry,
            _ => null
        };

        if (control != null)
        {
            _errorProvider.SetError(control, errorMessage);
        }
    }

    public void ClearAllErrors()
    {
        _errorProvider.Clear();
    }

    public void ShowSuccess(string message)
    {
        MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public void ShowError(string message)
    {
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public void CloseWithResult(bool success)
    {
        DialogResult = success ? DialogResult.OK : DialogResult.Cancel;
        Close();
    }

    #endregion

    /// <summary>
    /// Clean up resources.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _presenter?.Dispose();
            _errorProvider?.Dispose();
        }
        base.Dispose(disposing);
    }
}
