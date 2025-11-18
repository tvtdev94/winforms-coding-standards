// Template: ReaLTaiizor Metro Form with MVP Pattern
// Replace: YourEntity, YourPresenter
// Usage: Use this template for Metro-style forms with data grid

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Controls;
using Microsoft.Extensions.Logging;

namespace YourNamespace.Forms
{
    /// <summary>
    /// Metro-style list form for managing [YourEntity]
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
            // Form properties
            this.Text = "[Your Entity] List";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Style = ReaLTaiizor.Enum.Metro.Style.Light;

            InitializeControls();
            WireUpEvents();
        }

        private void InitializeControls()
        {
            // Search TextBox
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
            btnNew = new MetroButton
            {
                Location = new Point(20, 470),
                Size = new Size(100, 35),
                Text = "New"
            };
            this.Controls.Add(btnNew);

            btnEdit = new MetroButton
            {
                Location = new Point(130, 470),
                Size = new Size(100, 35),
                Text = "Edit"
            };
            this.Controls.Add(btnEdit);

            btnDelete = new MetroButton
            {
                Location = new Point(240, 470),
                Size = new Size(100, 35),
                Text = "Delete"
            };
            this.Controls.Add(btnDelete);

            btnRefresh = new MetroButton
            {
                Location = new Point(870, 470),
                Size = new Size(100, 35),
                Text = "Refresh"
            };
            this.Controls.Add(btnRefresh);

            // Status Label
            lblStatus = new MetroLabel
            {
                Location = new Point(20, 520),
                Size = new Size(950, 20),
                Text = "Ready"
            };
            this.Controls.Add(lblStatus);
        }

        private void WireUpEvents()
        {
            this.Load += YourEntityListForm_Load;
            btnNew.Click += BtnNew_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnRefresh.Click += BtnRefresh_Click;
            txtSearch.TextChanged += TxtSearch_TextChanged;
            gridEntities.SelectionChanged += GridEntities_SelectionChanged;
        }

        #endregion

        #region Data Loading

        private async void YourEntityListForm_Load(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        private async System.Threading.Tasks.Task LoadDataAsync()
        {
            try
            {
                lblStatus.Text = "Loading...";
                btnRefresh.Enabled = false;

                var entities = await _entityService.GetAllAsync();

                // Bind to grid
                gridEntities.DataSource = entities;

                // Configure columns
                if (gridEntities.Columns["Id"] != null)
                    gridEntities.Columns["Id"].Visible = false;

                lblStatus.Text = $"Total: {entities.Count} records";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load entities");
                MessageBox.Show($"Failed to load data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Error loading data";
            }
            finally
            {
                btnRefresh.Enabled = true;
            }
        }

        #endregion

        #region CRUD Operations

        private void BtnNew_Click(object sender, EventArgs e)
        {
            var form = _formFactory.Create<YourEntityEditForm>();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadDataAsync();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            var entity = GetSelectedEntity();
            if (entity == null)
            {
                MessageBox.Show("Please select an entity to edit", "No Selection");
                return;
            }

            var form = _formFactory.Create<YourEntityEditForm>();
            form.LoadEntity(entity.Id);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadDataAsync();
            }
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            var entity = GetSelectedEntity();
            if (entity == null)
            {
                MessageBox.Show("Please select an entity to delete", "No Selection");
                return;
            }

            var result = MessageBox.Show(
                $"Delete '{entity.Name}'?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
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

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        #endregion

        #region Search

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            // Implement search/filter logic
            FilterGrid(txtSearch.Text);
        }

        private void FilterGrid(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                (gridEntities.DataSource as BindingSource)?.RemoveFilter();
                return;
            }

            // Simple filter example
            var source = gridEntities.DataSource as BindingSource;
            if (source != null)
            {
                source.Filter = $"Name LIKE '%{searchText}%' OR Email LIKE '%{searchText}%'";
            }
        }

        #endregion

        #region Helper Methods

        private void GridEntities_SelectionChanged(object sender, EventArgs e)
        {
            bool hasSelection = gridEntities.SelectedRows.Count > 0;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
        }

        private YourEntity GetSelectedEntity()
        {
            if (gridEntities.SelectedRows.Count == 0)
                return null;

            return gridEntities.SelectedRows[0].DataBoundItem as YourEntity;
        }

        #endregion
    }
}
