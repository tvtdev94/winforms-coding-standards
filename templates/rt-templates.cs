// Template: ReaLTaiizor Templates (Material + Metro + Patterns)
// Usage: Templates for ReaLTaiizor UI framework with MVP pattern
// Sections: Material Form, Metro List Form, Common Patterns

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Controls;
using Microsoft.Extensions.Logging;

namespace YourNamespace.Forms
{
    // ============================================================================
    // SECTION 1: MATERIAL FORM TEMPLATE (Edit/Detail Form)
    // Replace: YourEntity, YourPresenter, YourView
    // ============================================================================

    /// <summary>
    /// Material Design form for editing entities
    /// </summary>
    public partial class YourEntityForm : MaterialForm, IYourEntityView
    {
        private readonly YourEntityPresenter _presenter;

        // Material Controls
        private MaterialTextBox txtEntityName;
        private MaterialTextBox txtDescription;
        private MaterialComboBox cboEntityType;
        private MaterialCheckBox chkIsActive;
        private MaterialButton btnSave;
        private MaterialButton btnCancel;
        private MaterialLabel lblTitle;

        public YourEntityForm(YourEntityPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            _presenter.SetView(this);
            InitializeMaterialForm();
        }

        #region Initialization

        private void InitializeMaterialForm()
        {
            this.Text = "[Your Entity] Management";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.Sizable = false;

            InitializeControls();
            WireUpEvents();
        }

        private void InitializeControls()
        {
            // Title
            lblTitle = new MaterialLabel
            {
                Location = new Point(20, 70),
                Size = new Size(560, 30),
                Text = "Entity Information",
                Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold)
            };
            this.Controls.Add(lblTitle);

            // Entity Name (Floating Label)
            txtEntityName = new MaterialTextBox
            {
                Location = new Point(20, 110),
                Size = new Size(560, 50),
                Hint = "Entity Name"  // Floating label
            };
            this.Controls.Add(txtEntityName);

            // Description
            txtDescription = new MaterialTextBox
            {
                Location = new Point(20, 170),
                Size = new Size(560, 50),
                Hint = "Description",
                Multiline = true
            };
            this.Controls.Add(txtDescription);

            // Type ComboBox
            cboEntityType = new MaterialComboBox
            {
                Location = new Point(20, 230),
                Size = new Size(560, 50),
                Hint = "Entity Type"
            };
            this.Controls.Add(cboEntityType);

            // Active CheckBox
            chkIsActive = new MaterialCheckBox
            {
                Location = new Point(20, 290),
                Size = new Size(200, 40),
                Text = "Active",
                Checked = true
            };
            this.Controls.Add(chkIsActive);

            // Buttons
            btnSave = new MaterialButton
            {
                Location = new Point(480, 350),
                Size = new Size(100, 40),
                Text = "Save",
                Type = MaterialButton.MaterialButtonType.Contained
            };
            this.Controls.Add(btnSave);

            btnCancel = new MaterialButton
            {
                Location = new Point(370, 350),
                Size = new Size(100, 40),
                Text = "Cancel",
                Type = MaterialButton.MaterialButtonType.Outlined
            };
            this.Controls.Add(btnCancel);
        }

        private void WireUpEvents()
        {
            this.Load += (s, e) => LoadRequested?.Invoke(this, EventArgs.Empty);
            btnSave.Click += (s, e) => SaveRequested?.Invoke(this, EventArgs.Empty);
            btnCancel.Click += (s, e) => CancelRequested?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region IYourEntityView Implementation

        public string EntityName
        {
            get => txtEntityName.Text;
            set => txtEntityName.Text = value ?? string.Empty;
        }

        public string EntityDescription
        {
            get => txtDescription.Text;
            set => txtDescription.Text = value ?? string.Empty;
        }

        public int? EntityTypeId
        {
            get => cboEntityType.SelectedValue as int?;
            set => cboEntityType.SelectedValue = value;
        }

        public bool IsActive
        {
            get => chkIsActive.Checked;
            set => chkIsActive.Checked = value;
        }

        public bool IsSaveButtonEnabled
        {
            get => btnSave.Enabled;
            set => btnSave.Enabled = value;
        }

        public event EventHandler LoadRequested;
        public event EventHandler SaveRequested;
        public event EventHandler CancelRequested;

        public void ShowError(string message) =>
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        public void ShowSuccess(string message) =>
            MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

        public void SetEntityTypes(List<EntityType> entityTypes)
        {
            cboEntityType.DataSource = entityTypes;
            cboEntityType.DisplayMember = "Name";
            cboEntityType.ValueMember = "Id";
        }

        public void CloseForm() => this.Close();

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _presenter?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// View interface for Material Form
    /// </summary>
    public interface IYourEntityView
    {
        string EntityName { get; set; }
        string EntityDescription { get; set; }
        int? EntityTypeId { get; set; }
        bool IsActive { get; set; }
        bool IsSaveButtonEnabled { get; set; }

        event EventHandler LoadRequested;
        event EventHandler SaveRequested;
        event EventHandler CancelRequested;

        void ShowError(string message);
        void ShowSuccess(string message);
        void SetEntityTypes(List<EntityType> entityTypes);
        void CloseForm();
    }

    // ============================================================================
    // SECTION 2: METRO LIST FORM TEMPLATE (List/Grid Form)
    // Replace: YourEntity, YourEntityService
    // ============================================================================

    /// <summary>
    /// Metro-style list form with grid
    /// </summary>
    public partial class YourEntityListForm : MetroForm
    {
        private readonly IYourEntityService _entityService;
        private readonly IFormFactory _formFactory;
        private readonly ILogger<YourEntityListForm> _logger;

        // Metro Controls
        private MetroGrid gridEntities;
        private MetroTextBox txtSearch;
        private MetroButton btnNew;
        private MetroButton btnEdit;
        private MetroButton btnDelete;
        private MetroButton btnRefresh;
        private MetroLabel lblStatus;

        public YourEntityListForm(
            IYourEntityService entityService,
            IFormFactory formFactory,
            ILogger<YourEntityListForm> logger)
        {
            InitializeComponent();
            _entityService = entityService ?? throw new ArgumentNullException(nameof(entityService));
            _formFactory = formFactory ?? throw new ArgumentNullException(nameof(formFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            InitializeMetroForm();
        }

        #region Initialization

        private void InitializeMetroForm()
        {
            this.Text = "[Your Entity] List";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Style = ReaLTaiizor.Enum.Metro.Style.Light;

            InitializeControls();
            WireUpEvents();
        }

        private void InitializeControls()
        {
            // Search
            txtSearch = new MetroTextBox
            {
                Location = new Point(20, 70),
                Size = new Size(300, 30),
                WatermarkText = "Search..."
            };
            this.Controls.Add(txtSearch);

            // Grid
            gridEntities = new MetroGrid
            {
                Location = new Point(20, 110),
                Size = new Size(950, 350),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            this.Controls.Add(gridEntities);

            // Buttons
            btnNew = new MetroButton { Location = new Point(20, 470), Size = new Size(100, 35), Text = "New" };
            btnEdit = new MetroButton { Location = new Point(130, 470), Size = new Size(100, 35), Text = "Edit" };
            btnDelete = new MetroButton { Location = new Point(240, 470), Size = new Size(100, 35), Text = "Delete" };
            btnRefresh = new MetroButton { Location = new Point(870, 470), Size = new Size(100, 35), Text = "Refresh" };

            this.Controls.Add(btnNew);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
            this.Controls.Add(btnRefresh);

            // Status
            lblStatus = new MetroLabel { Location = new Point(20, 520), Size = new Size(950, 20), Text = "Ready" };
            this.Controls.Add(lblStatus);
        }

        private void WireUpEvents()
        {
            this.Load += async (s, e) => await LoadDataAsync();
            btnNew.Click += BtnNew_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnRefresh.Click += async (s, e) => await LoadDataAsync();
            txtSearch.TextChanged += (s, e) => FilterGrid(txtSearch.Text);
            gridEntities.SelectionChanged += (s, e) => UpdateButtonStates();
        }

        #endregion

        #region Data Operations

        private async System.Threading.Tasks.Task LoadDataAsync()
        {
            try
            {
                lblStatus.Text = "Loading...";
                btnRefresh.Enabled = false;

                var entities = await _entityService.GetAllAsync();
                gridEntities.DataSource = entities;

                if (gridEntities.Columns["Id"] != null)
                    gridEntities.Columns["Id"].Visible = false;

                lblStatus.Text = $"Total: {entities.Count} records";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load entities");
                MessageBox.Show($"Failed to load: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnRefresh.Enabled = true;
            }
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            var form = _formFactory.Create<YourEntityEditForm>();
            if (form.ShowDialog() == DialogResult.OK)
                LoadDataAsync();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            var entity = GetSelectedEntity();
            if (entity == null) { MessageBox.Show("Select an entity to edit"); return; }

            var form = _formFactory.Create<YourEntityEditForm>();
            form.LoadEntity(entity.Id);
            if (form.ShowDialog() == DialogResult.OK)
                LoadDataAsync();
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            var entity = GetSelectedEntity();
            if (entity == null) { MessageBox.Show("Select an entity to delete"); return; }

            if (MessageBox.Show($"Delete '{entity.Name}'?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    await _entityService.DeleteAsync(entity.Id);
                    await LoadDataAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Delete failed: {ex.Message}", "Error");
                }
            }
        }

        #endregion

        #region Helper Methods

        private void FilterGrid(string searchText)
        {
            var source = gridEntities.DataSource as BindingSource;
            if (string.IsNullOrWhiteSpace(searchText))
                source?.RemoveFilter();
            else
                source.Filter = $"Name LIKE '%{searchText}%'";
        }

        private void UpdateButtonStates()
        {
            bool hasSelection = gridEntities.SelectedRows.Count > 0;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
        }

        private YourEntity GetSelectedEntity() =>
            gridEntities.SelectedRows.Count > 0
                ? gridEntities.SelectedRows[0].DataBoundItem as YourEntity
                : null;

        #endregion
    }
}

// ============================================================================
// SECTION 3: COMMON REALTAIIZOR PATTERNS
// Copy-paste patterns for common scenarios
// ============================================================================

namespace YourNamespace.Patterns
{
    /// <summary>
    /// Common ReaLTaiizor control patterns
    /// </summary>
    public class ReaLTaiizorPatterns
    {
        #region Material ComboBox Pattern

        public void MaterialComboBoxPattern()
        {
            // Simple string list
            var cboStatus = new MaterialComboBox();
            cboStatus.Items.AddRange(new[] { "Active", "Inactive", "Pending" });
            cboStatus.Hint = "Select status...";

            // Object list with data binding
            var cboType = new MaterialComboBox();
            var types = new List<CustomerType>
            {
                new CustomerType { Id = 1, Name = "Regular" },
                new CustomerType { Id = 2, Name = "Premium" }
            };
            cboType.DataSource = types;
            cboType.DisplayMember = "Name";
            cboType.ValueMember = "Id";
            cboType.Hint = "Select type...";

            // Get selected value
            int selectedId = (int)cboType.SelectedValue;
        }

        #endregion

        #region Metro Grid Pattern

        public void MetroGridPattern(List<Customer> customers)
        {
            var grid = new MetroGrid();
            grid.AllowUserToAddRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Bind data
            grid.DataSource = customers;

            // Configure columns
            grid.Columns["Id"].Visible = false;
            grid.Columns["Name"].HeaderText = "Customer Name";
            grid.Columns["Name"].Width = 200;

            // Cell formatting
            grid.CellFormatting += (s, e) =>
            {
                if (grid.Columns[e.ColumnIndex].Name == "IsActive")
                {
                    bool isActive = (bool)e.Value;
                    e.CellStyle.BackColor = isActive
                        ? System.Drawing.Color.LightGreen
                        : System.Drawing.Color.LightCoral;
                }
            };
        }

        #endregion

        #region Validation Pattern

        public bool ValidateMaterialForm(
            MaterialTextBox txtName,
            MaterialTextBox txtEmail,
            MaterialComboBox cboType)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Name is required", "Validation Error");
                txtName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Email is required", "Validation Error");
                txtEmail.Focus();
                return false;
            }

            if (cboType.SelectedValue == null)
            {
                MessageBox.Show("Type is required", "Validation Error");
                cboType.Focus();
                return false;
            }

            return true;
        }

        #endregion

        #region Async Button Pattern

        public void AsyncButtonPattern()
        {
            var btnSave = new MaterialButton();
            btnSave.Text = "Save";
            btnSave.Type = MaterialButton.MaterialButtonType.Contained;

            btnSave.Click += async (s, e) =>
            {
                try
                {
                    btnSave.Enabled = false;
                    btnSave.Text = "Saving...";

                    await SaveDataAsync();

                    MessageBox.Show("Saved successfully", "Success");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Save failed: {ex.Message}", "Error");
                }
                finally
                {
                    btnSave.Enabled = true;
                    btnSave.Text = "Save";
                }
            };
        }

        private async System.Threading.Tasks.Task SaveDataAsync()
        {
            await System.Threading.Tasks.Task.Delay(1000); // Simulate
        }

        #endregion
    }

    #region Sample Models

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
    }

    public class CustomerType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class EntityType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    #endregion
}
