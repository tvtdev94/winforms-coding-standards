using CustomerManagement.Factories;

namespace CustomerManagement.Forms;

/// <summary>
/// Main application form with navigation menu.
/// </summary>
public partial class MainForm : Form
{
    private readonly IFormFactory _formFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainForm"/> class.
    /// </summary>
    /// <param name="formFactory">The form factory for creating child forms.</param>
    public MainForm(IFormFactory formFactory)
    {
        _formFactory = formFactory ?? throw new ArgumentNullException(nameof(formFactory));
        InitializeComponent();
    }

    /// <summary>
    /// Initializes the form components.
    /// </summary>
    private void InitializeComponent()
    {
        // Form properties
        Text = "Customer Management System";
        Size = new Size(800, 600);
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(640, 480);

        // Create menu strip
        var menuStrip = new MenuStrip();

        // File menu
        var fileMenu = new ToolStripMenuItem("&File");
        var exitMenuItem = new ToolStripMenuItem("E&xit");
        exitMenuItem.Click += (s, e) => Close();
        fileMenu.DropDownItems.Add(exitMenuItem);

        // Customers menu
        var customersMenu = new ToolStripMenuItem("&Customers");
        var manageCustomersMenuItem = new ToolStripMenuItem("&Manage Customers");
        manageCustomersMenuItem.Click += OnManageCustomersClick;
        customersMenu.DropDownItems.Add(manageCustomersMenuItem);

        // Help menu
        var helpMenu = new ToolStripMenuItem("&Help");
        var aboutMenuItem = new ToolStripMenuItem("&About");
        aboutMenuItem.Click += OnAboutClick;
        helpMenu.DropDownItems.Add(aboutMenuItem);

        // Add menus to menu strip
        menuStrip.Items.AddRange(new ToolStripItem[]
        {
            fileMenu,
            customersMenu,
            helpMenu
        });

        // Create welcome label
        var welcomeLabel = new Label
        {
            Text = "Welcome to Customer Management System\n\n" +
                   "Select 'Customers > Manage Customers' to get started.",
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font(Font.FontFamily, 14),
            ForeColor = Color.FromArgb(64, 64, 64)
        };

        // Add controls to form
        Controls.Add(welcomeLabel);
        Controls.Add(menuStrip);
        MainMenuStrip = menuStrip;
    }

    /// <summary>
    /// Handles the manage customers menu click.
    /// </summary>
    private void OnManageCustomersClick(object? sender, EventArgs e)
    {
        // Create customer list form using Factory Pattern
        var customerListForm = _formFactory.Create<CustomerListForm>();
        customerListForm.ShowDialog(this);
    }

    /// <summary>
    /// Handles the about menu click.
    /// </summary>
    private void OnAboutClick(object? sender, EventArgs e)
    {
        MessageBox.Show(
            "Customer Management System\n" +
            "Version 1.0\n\n" +
            "A WinForms example demonstrating:\n" +
            "- MVP Pattern\n" +
            "- Dependency Injection\n" +
            "- Unit of Work Pattern\n" +
            "- Factory Pattern\n" +
            "- Entity Framework Core\n" +
            "- Async/Await\n" +
            "- Best Practices",
            "About",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}
