// Template: DevExpress GridControl Setup Pattern
// Replace: YourEntity, YourService
// Usage: Complete pattern for XtraGrid with CRUD operations

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Microsoft.Extensions.Logging;

namespace YourNamespace.Forms
{
    /// <summary>
    /// List form for managing [YourEntity] with DevExpress GridControl
    /// </summary>
    public partial class YourEntityListForm : XtraForm
    {
        private readonly IYourEntityService _entityService;
        private readonly IFormFactory _formFactory;
        private readonly ILogger<YourEntityListForm> _logger;

        // Grid references
        private GridView GridView => gridControlEntities.MainView as GridView;

        public YourEntityListForm(
            IYourEntityService entityService,
            IFormFactory formFactory,
            ILogger<YourEntityListForm> logger)
        {
            InitializeComponent();

            _entityService = entityService ?? throw new ArgumentNullException(nameof(entityService));
            _formFactory = formFactory ?? throw new ArgumentNullException(nameof(formFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            InitializeForm();
        }

        #region Initialization

        private void InitializeForm()
        {
            // Form properties
            this.Text = "[Your Entity] List";
            this.MinimumSize = new Size(1000, 600);
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Configure grid
            ConfigureGrid();

            // Setup context menu
            SetupContextMenu();

            // Wire up events
            WireUpEvents();
        }

        private void ConfigureGrid()
        {
            var gridView = GridView;

            // Basic configuration
            gridView.OptionsBehavior.Editable = false;
            gridView.OptionsBehavior.ReadOnly = true;
            gridView.OptionsView.ShowGroupPanel = false;
            gridView.OptionsSelection.MultiSelect = false;
            gridView.OptionsSelection.EnableAppearanceFocusedCell = false;

            // Row appearance
            gridView.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(0, 122, 204);
            gridView.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White;
            gridView.OptionsView.EnableAppearanceEvenRow = true;
            gridView.Appearance.EvenRow.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);

            // Built-in search panel
            gridView.OptionsFind.AlwaysVisible = true;
            gridView.OptionsFind.FindNullPrompt = "Search...";
            gridView.OptionsFind.ShowCloseButton = false;
            gridView.OptionsFind.ShowClearButton = true;

            // Auto filter row (optional)
            gridView.OptionsView.ShowAutoFilterRow = true;

            // Double-click to edit
            gridView.DoubleClick += GridView_DoubleClick;

            // Selection changed
            gridView.FocusedRowChanged += GridView_FocusedRowChanged;
        }

        private void WireUpEvents()
        {
            this.Load += YourEntityListForm_Load;

            // Button events
            btnNew.Click += BtnNew_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnRefresh.Click += BtnRefresh_Click;
            btnExport.Click += BtnExport_Click;
        }

        private void SetupContextMenu()
        {
            var contextMenu = new ContextMenuStrip();

            contextMenu.Items.Add("New", null, (s, e) => CreateNew());
            contextMenu.Items.Add("Edit", null, (s, e) => EditSelected());
            contextMenu.Items.Add("Delete", null, (s, e) => DeleteSelected());
            contextMenu.Items.Add("-"); // Separator
            contextMenu.Items.Add("Refresh", null, (s, e) => RefreshGrid());
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Export to Excel", null, (s, e) => ExportToExcel());
            contextMenu.Items.Add("Export to PDF", null, (s, e) => ExportToPdf());

            gridControlEntities.ContextMenuStrip = contextMenu;
        }

        #endregion

        #region Data Loading

        private async void YourEntityListForm_Load(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                ShowLoadingIndicator();

                // Load data
                var entities = await _entityService.GetAllAsync();

                // Bind to grid
                gridControlEntities.DataSource = entities;

                // Configure columns after binding
                ConfigureColumnsAfterBinding();

                // Update status
                UpdateStatusBar();

                _logger.LogInformation("Loaded {Count} entities", entities.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load entities");
                XtraMessageBox.Show(
                    $"Failed to load data: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                HideLoadingIndicator();
            }
        }

        private void ConfigureColumnsAfterBinding()
        {
            var gridView = GridView;

            // Hide technical columns
            if (gridView.Columns["Id"] != null)
                gridView.Columns["Id"].Visible = false;

            if (gridView.Columns["CreatedBy"] != null)
                gridView.Columns["CreatedBy"].Visible = false;

            if (gridView.Columns["ModifiedBy"] != null)
                gridView.Columns["ModifiedBy"].Visible = false;

            // Set column captions
            if (gridView.Columns["Name"] != null)
                gridView.Columns["Name"].Caption = "Entity Name";

            if (gridView.Columns["Description"] != null)
                gridView.Columns["Description"].Caption = "Description";

            if (gridView.Columns["IsActive"] != null)
                gridView.Columns["IsActive"].Caption = "Active";

            if (gridView.Columns["CreatedDate"] != null)
            {
                gridView.Columns["CreatedDate"].Caption = "Created";
                gridView.Columns["CreatedDate"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                gridView.Columns["CreatedDate"].DisplayFormat.FormatString = "yyyy-MM-dd HH:mm";
            }

            // Set column widths
            if (gridView.Columns["Name"] != null)
                gridView.Columns["Name"].Width = 200;

            if (gridView.Columns["Description"] != null)
                gridView.Columns["Description"].Width = 300;

            // Auto-size remaining columns
            gridView.BestFitColumns();
        }

        #endregion

        #region CRUD Operations

        private void BtnNew_Click(object sender, EventArgs e)
        {
            CreateNew();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            EditSelected();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            DeleteSelected();
        }

        private void CreateNew()
        {
            try
            {
                var form = _formFactory.Create<YourEntityEditForm>();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    RefreshGrid();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create new entity");
                XtraMessageBox.Show(
                    $"Failed to open form: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void EditSelected()
        {
            var entity = GetSelectedEntity();
            if (entity == null)
            {
                XtraMessageBox.Show(
                    "Please select an entity to edit",
                    "No Selection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            try
            {
                var form = _formFactory.Create<YourEntityEditForm>();
                form.LoadEntity(entity.Id);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    RefreshGrid();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to edit entity {EntityId}", entity.Id);
                XtraMessageBox.Show(
                    $"Failed to open form: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async void DeleteSelected()
        {
            var entity = GetSelectedEntity();
            if (entity == null)
            {
                XtraMessageBox.Show(
                    "Please select an entity to delete",
                    "No Selection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var result = XtraMessageBox.Show(
                $"Are you sure you want to delete '{entity.Name}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                ShowLoadingIndicator();

                await _entityService.DeleteAsync(entity.Id);

                XtraMessageBox.Show(
                    "Entity deleted successfully",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                await RefreshGrid();

                _logger.LogInformation("Deleted entity {EntityId}", entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete entity {EntityId}", entity.Id);
                XtraMessageBox.Show(
                    $"Failed to delete: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                HideLoadingIndicator();
            }
        }

        #endregion

        #region Grid Events

        private void GridView_DoubleClick(object sender, EventArgs e)
        {
            var gridView = sender as GridView;
            var hitInfo = gridView.CalcHitInfo((sender as Control).PointToClient(Cursor.Position));

            if (hitInfo.InRow && hitInfo.RowHandle >= 0)
            {
                EditSelected();
            }
        }

        private void GridView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            UpdateButtonStates();
        }

        #endregion

        #region Export

        private void BtnExport_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }

        private void ExportToExcel()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                FileName = $"YourEntities_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    GridView.ExportToXlsx(saveFileDialog.FileName);
                    XtraMessageBox.Show(
                        "Export completed successfully",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to export to Excel");
                    XtraMessageBox.Show(
                        $"Export failed: {ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void ExportToPdf()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF Files|*.pdf",
                FileName = $"YourEntities_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    GridView.ExportToPdf(saveFileDialog.FileName);
                    XtraMessageBox.Show(
                        "Export completed successfully",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to export to PDF");
                    XtraMessageBox.Show(
                        $"Export failed: {ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Helper Methods

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private async Task RefreshGrid()
        {
            // Save current selection
            var selectedId = GetSelectedEntity()?.Id;

            // Reload data
            await LoadDataAsync();

            // Restore selection
            if (selectedId.HasValue)
            {
                int rowHandle = GridView.LocateByValue("Id", selectedId.Value);
                if (rowHandle != GridControl.InvalidRowHandle)
                {
                    GridView.FocusedRowHandle = rowHandle;
                }
            }
        }

        private YourEntity GetSelectedEntity()
        {
            return GridView.GetFocusedRow() as YourEntity;
        }

        private void UpdateButtonStates()
        {
            bool hasSelection = GetSelectedEntity() != null;

            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
        }

        private void UpdateStatusBar()
        {
            int totalRows = GridView.DataRowCount;
            int visibleRows = GridView.VisibleRowCount;

            lblStatus.Text = $"Total: {totalRows} | Visible: {visibleRows}";
        }

        private void ShowLoadingIndicator()
        {
            gridControlEntities.UseWaitCursor = true;
            // Optional: Show overlay progress panel
        }

        private void HideLoadingIndicator()
        {
            gridControlEntities.UseWaitCursor = false;
            // Optional: Hide overlay progress panel
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
