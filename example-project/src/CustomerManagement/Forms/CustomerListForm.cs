using CustomerManagement.Factories;
using CustomerManagement.Models;
using CustomerManagement.Presenters;
using CustomerManagement.Services;
using CustomerManagement.Views;
using Microsoft.Extensions.Logging;

namespace CustomerManagement.Forms;

/// <summary>
/// Form for displaying and managing the list of customers.
/// Implements ICustomerListView for MVP pattern.
/// </summary>
public partial class CustomerListForm : Form, ICustomerListView
{
    private readonly IFormFactory _formFactory;
    private readonly CustomerListPresenter _presenter;
    private DataGridView _dgvCustomers = null!;
    private TextBox _txtSearch = null!;
    private Button _btnAdd = null!;
    private Button _btnEdit = null!;
    private Button _btnDelete = null!;
    private Button _btnRefresh = null!;
    private Button _btnViewDetails = null!;
    private StatusStrip _statusStrip = null!;
    private ToolStripStatusLabel _statusLabel = null!;
    private ToolStripProgressBar _progressBar = null!;

    #region ICustomerListView Events

    public event EventHandler? LoadRequested;
    public event EventHandler? AddRequested;
    public event EventHandler<int>? EditRequested;
    public event EventHandler<int>? DeleteRequested;
    public event EventHandler? RefreshRequested;
    public event EventHandler<string>? SearchRequested;
    public event EventHandler<int>? ViewDetailsRequested;

    #endregion

    #region ICustomerListView Properties

    public List<Customer> Customers
    {
        get => _dgvCustomers.DataSource as List<Customer> ?? new List<Customer>();
        set
        {
            if (InvokeRequired)
            {
                Invoke(() => Customers = value);
                return;
            }

            _dgvCustomers.DataSource = value;
        }
    }

    public Customer? SelectedCustomer =>
        _dgvCustomers.CurrentRow?.DataBoundItem as Customer;

    public bool IsLoading
    {
        get => _progressBar.Visible;
        set
        {
            if (InvokeRequired)
            {
                Invoke(() => IsLoading = value);
                return;
            }

            _progressBar.Visible = value;
            _statusLabel.Text = value ? "Loading..." : "Ready";
            Cursor = value ? Cursors.WaitCursor : Cursors.Default;

            // Disable buttons while loading
            _btnAdd.Enabled = !value;
            _btnEdit.Enabled = !value;
            _btnDelete.Enabled = !value;
            _btnRefresh.Enabled = !value;
            _btnViewDetails.Enabled = !value;
        }
    }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerListForm"/> class.
    /// </summary>
    public CustomerListForm(
        IFormFactory formFactory,
        ICustomerService customerService,
        ILogger<CustomerListPresenter> logger)
    {
        _formFactory = formFactory ?? throw new ArgumentNullException(nameof(formFactory));

        InitializeComponent();

        // Create presenter (MVP pattern)
        _presenter = new CustomerListPresenter(this, customerService, logger);
    }

    /// <summary>
    /// Initializes the form components.
    /// </summary>
    private void InitializeComponent()
    {
        Text = "Customer Management";
        Size = new Size(1000, 600);
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(800, 400);

        // Search panel at top
        var searchPanel = CreateSearchPanel();

        // Button panel on the right
        var buttonPanel = CreateButtonPanel();

        // DataGridView in the center
        _dgvCustomers = CreateDataGridView();

        // Status strip at bottom
        _statusStrip = CreateStatusStrip();

        // Layout
        var mainPanel = new Panel { Dock = DockStyle.Fill };
        mainPanel.Controls.Add(_dgvCustomers);
        mainPanel.Controls.Add(buttonPanel);

        Controls.Add(mainPanel);
        Controls.Add(searchPanel);
        Controls.Add(_statusStrip);

        // Wire up events
        Load += (s, e) => LoadRequested?.Invoke(this, EventArgs.Empty);
        _dgvCustomers.CellDoubleClick += OnCellDoubleClick;
    }

    /// <summary>
    /// Creates the search panel.
    /// </summary>
    private Panel CreateSearchPanel()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 60,
            Padding = new Padding(10)
        };

        var label = new Label
        {
            Text = "Search:",
            AutoSize = true,
            Location = new Point(10, 20)
        };

        _txtSearch = new TextBox
        {
            Location = new Point(70, 17),
            Width = 300
        };
        _txtSearch.TextChanged += (s, e) => SearchRequested?.Invoke(this, _txtSearch.Text);

        panel.Controls.AddRange(new Control[] { label, _txtSearch });
        return panel;
    }

    /// <summary>
    /// Creates the button panel.
    /// </summary>
    private Panel CreateButtonPanel()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Right,
            Width = 120,
            Padding = new Padding(5)
        };

        _btnAdd = new Button
        {
            Text = "&Add",
            Dock = DockStyle.Top,
            Height = 35,
            Margin = new Padding(0, 0, 0, 5)
        };
        _btnAdd.Click += (s, e) =>
        {
            var editForm = _formFactory.Create<CustomerEditForm>();
            editForm.CustomerId = 0; // New customer
            if (editForm.ShowDialog(this) == DialogResult.OK)
            {
                RefreshRequested?.Invoke(this, EventArgs.Empty);
            }
        };

        _btnEdit = new Button
        {
            Text = "&Edit",
            Dock = DockStyle.Top,
            Height = 35,
            Margin = new Padding(0, 5, 0, 5)
        };
        _btnEdit.Click += (s, e) =>
        {
            var customer = SelectedCustomer;
            if (customer == null)
            {
                ShowError("Please select a customer to edit.");
                return;
            }

            var editForm = _formFactory.Create<CustomerEditForm>();
            editForm.CustomerId = customer.Id;
            if (editForm.ShowDialog(this) == DialogResult.OK)
            {
                RefreshRequested?.Invoke(this, EventArgs.Empty);
            }
        };

        _btnDelete = new Button
        {
            Text = "&Delete",
            Dock = DockStyle.Top,
            Height = 35,
            Margin = new Padding(0, 5, 0, 5)
        };
        _btnDelete.Click += (s, e) =>
        {
            var customer = SelectedCustomer;
            if (customer != null)
            {
                DeleteRequested?.Invoke(this, customer.Id);
            }
        };

        _btnViewDetails = new Button
        {
            Text = "&View Details",
            Dock = DockStyle.Top,
            Height = 35,
            Margin = new Padding(0, 5, 0, 5)
        };
        _btnViewDetails.Click += (s, e) =>
        {
            var customer = SelectedCustomer;
            if (customer != null)
            {
                ViewDetailsRequested?.Invoke(this, customer.Id);
            }
        };

        _btnRefresh = new Button
        {
            Text = "&Refresh",
            Dock = DockStyle.Top,
            Height = 35,
            Margin = new Padding(0, 5, 0, 0)
        };
        _btnRefresh.Click += (s, e) => RefreshRequested?.Invoke(this, EventArgs.Empty);

        // Add spacers to push buttons to top
        var spacer1 = new Panel { Dock = DockStyle.Top, Height = 5 };
        var spacer2 = new Panel { Dock = DockStyle.Top, Height = 5 };
        var spacer3 = new Panel { Dock = DockStyle.Top, Height = 5 };

        panel.Controls.Add(_btnRefresh);
        panel.Controls.Add(spacer3);
        panel.Controls.Add(_btnViewDetails);
        panel.Controls.Add(spacer2);
        panel.Controls.Add(_btnDelete);
        panel.Controls.Add(spacer1);
        panel.Controls.Add(_btnEdit);
        panel.Controls.Add(_btnAdd);

        return panel;
    }

    /// <summary>
    /// Creates and configures the DataGridView.
    /// </summary>
    private DataGridView CreateDataGridView()
    {
        var dgv = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = SystemColors.Window,
            BorderStyle = BorderStyle.None
        };

        // Define columns
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Customer.Id),
            HeaderText = "ID",
            Width = 50,
            FillWeight = 10
        });

        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Customer.Name),
            HeaderText = "Name",
            FillWeight = 30
        });

        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Customer.Email),
            HeaderText = "Email",
            FillWeight = 30
        });

        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Customer.Phone),
            HeaderText = "Phone",
            FillWeight = 20
        });

        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Customer.City),
            HeaderText = "City",
            FillWeight = 20
        });

        dgv.Columns.Add(new DataGridViewCheckBoxColumn
        {
            DataPropertyName = nameof(Customer.IsActive),
            HeaderText = "Active",
            Width = 60,
            FillWeight = 10
        });

        return dgv;
    }

    /// <summary>
    /// Creates the status strip.
    /// </summary>
    private StatusStrip CreateStatusStrip()
    {
        var statusStrip = new StatusStrip();

        _statusLabel = new ToolStripStatusLabel
        {
            Text = "Ready",
            Spring = true,
            TextAlign = ContentAlignment.MiddleLeft
        };

        _progressBar = new ToolStripProgressBar
        {
            Style = ProgressBarStyle.Marquee,
            Visible = false
        };

        statusStrip.Items.AddRange(new ToolStripItem[]
        {
            _statusLabel,
            _progressBar
        });

        return statusStrip;
    }

    /// <summary>
    /// Handles double-click on a cell (opens edit).
    /// </summary>
    private void OnCellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
        {
            _btnEdit.PerformClick();
        }
    }

    #region ICustomerListView Methods

    public void ShowSuccess(string message)
    {
        MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public void ShowError(string message)
    {
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public bool ShowConfirmation(string message, string title)
    {
        var result = MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        return result == DialogResult.Yes;
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
        }
        base.Dispose(disposing);
    }
}
