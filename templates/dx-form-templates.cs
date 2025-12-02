// Template: DevExpress Form Templates (Edit + List + Grid)
// Usage: Templates for DevExpress UI with MVP pattern
// Sections: Edit Form, List Form with Grid, Grid Configuration

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Microsoft.Extensions.Logging;

namespace YourNamespace.Forms
{
    // ============================================================================
    // SECTION 1: EDIT FORM TEMPLATE (MVP Pattern)
    // Replace: YourEntity, YourPresenter, YourView
    // ============================================================================

    /// <summary>
    /// DevExpress edit form with MVP pattern
    /// </summary>
    public partial class YourEntityForm : XtraForm, IYourEntityView
    {
        private readonly YourEntityPresenter _presenter;

        public YourEntityForm(YourEntityPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            _presenter.SetView(this);
            InitializeDevExpressForm();
            WireUpEvents();
        }

        #region Initialization

        private void InitializeDevExpressForm()
        {
            this.Text = "[Your Entity] Management";
            this.Size = new Size(900, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            ConfigureLayoutControl();
        }

        private void ConfigureLayoutControl()
        {
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.AllowCustomization = false;

            var root = layoutControl1.Root;
            root.GroupBordersVisible = false;
            root.Padding = new DevExpress.XtraLayout.Utils.Padding(10);

            // Set consistent label width
            foreach (var item in root.Items)
            {
                if (item is LayoutControlItem lci)
                {
                    lci.TextLocation = DevExpress.Utils.Locations.Left;
                    lci.TextAlignMode = TextAlignModeItem.CustomSize;
                    lci.TextSize = new Size(120, 0);
                }
            }
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
            get => memoDescription.Text;
            set => memoDescription.Text = value ?? string.Empty;
        }

        public int? EntityTypeId
        {
            get => lkeEntityType.EditValue as int?;
            set => lkeEntityType.EditValue = value;
        }

        public bool IsActive
        {
            get => chkIsActive.Checked;
            set => chkIsActive.Checked = value;
        }

        public DateTime? CreatedDate
        {
            get => dteCreatedDate.EditValue as DateTime?;
            set => dteCreatedDate.EditValue = value;
        }

        public bool IsSaveButtonEnabled
        {
            get => btnSave.Enabled;
            set => btnSave.Enabled = value;
        }

        public bool IsLoadingVisible
        {
            get => layoutControl1.UseWaitCursor;
            set => layoutControl1.UseWaitCursor = value;
        }

        public event EventHandler LoadRequested;
        public event EventHandler SaveRequested;
        public event EventHandler CancelRequested;
        public event EventHandler DeleteRequested;

        public void ShowError(string message) =>
            XtraMessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        public void ShowSuccess(string message) =>
            XtraMessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

        public DialogResult ShowConfirmation(string message) =>
            XtraMessageBox.Show(message, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        public void SetEntityTypes(List<EntityType> entityTypes)
        {
            lkeEntityType.Properties.DataSource = entityTypes;
            lkeEntityType.Properties.DisplayMember = "Name";
            lkeEntityType.Properties.ValueMember = "Id";
            lkeEntityType.Properties.NullText = "-- Select Type --";
            lkeEntityType.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoFilter;
        }

        public void CloseForm() => this.Close();

        #endregion

        #region Validation

        private bool ValidateForm()
        {
            dxErrorProvider1.ClearErrors();
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(txtEntityName.Text))
            {
                dxErrorProvider1.SetError(txtEntityName, "Entity name is required");
                isValid = false;
            }

            if (lkeEntityType.EditValue == null)
            {
                dxErrorProvider1.SetError(lkeEntityType, "Entity type is required");
                isValid = false;
            }

            return isValid;
        }

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
    /// View interface for DevExpress Edit Form
    /// </summary>
    public interface IYourEntityView
    {
        string EntityName { get; set; }
        string EntityDescription { get; set; }
        int? EntityTypeId { get; set; }
        bool IsActive { get; set; }
        DateTime? CreatedDate { get; set; }
        bool IsSaveButtonEnabled { get; set; }
        bool IsLoadingVisible { get; set; }

        event EventHandler LoadRequested;
        event EventHandler SaveRequested;
        event EventHandler CancelRequested;
        event EventHandler DeleteRequested;

        void ShowError(string message);
        void ShowSuccess(string message);
        DialogResult ShowConfirmation(string message);
        void SetEntityTypes(List<EntityType> entityTypes);
        void CloseForm();
    }

    /// <summary>
    /// Presenter for DevExpress Edit Form
    /// </summary>
    public class YourEntityPresenter : IDisposable
    {
        private readonly IYourEntityService _entityService;
        private readonly ILogger<YourEntityPresenter> _logger;
        private IYourEntityView _view;
        private int? _entityId;

        public YourEntityPresenter(IYourEntityService entityService, ILogger<YourEntityPresenter> logger)
        {
            _entityService = entityService ?? throw new ArgumentNullException(nameof(entityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void SetView(IYourEntityView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _view.LoadRequested += OnLoadRequested;
            _view.SaveRequested += OnSaveRequested;
            _view.CancelRequested += OnCancelRequested;
            _view.DeleteRequested += OnDeleteRequested;
        }

        public void LoadEntity(int entityId) => _entityId = entityId;

        private async void OnLoadRequested(object sender, EventArgs e)
        {
            try
            {
                _view.IsLoadingVisible = true;
                var entityTypes = await _entityService.GetEntityTypesAsync();
                _view.SetEntityTypes(entityTypes);

                if (_entityId.HasValue)
                {
                    var entity = await _entityService.GetByIdAsync(_entityId.Value);
                    if (entity != null)
                    {
                        _view.EntityName = entity.Name;
                        _view.EntityDescription = entity.Description;
                        _view.EntityTypeId = entity.EntityTypeId;
                        _view.IsActive = entity.IsActive;
                        _view.CreatedDate = entity.CreatedDate;
                    }
                }
                else
                {
                    _view.IsActive = true;
                    _view.CreatedDate = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load entity");
                _view.ShowError($"Failed to load: {ex.Message}");
            }
            finally
            {
                _view.IsLoadingVisible = false;
            }
        }

        private async void OnSaveRequested(object sender, EventArgs e)
        {
            try
            {
                _view.IsSaveButtonEnabled = false;
                _view.IsLoadingVisible = true;

                var entity = new YourEntity
                {
                    Id = _entityId ?? 0,
                    Name = _view.EntityName,
                    Description = _view.EntityDescription,
                    EntityTypeId = _view.EntityTypeId,
                    IsActive = _view.IsActive,
                    CreatedDate = _view.CreatedDate ?? DateTime.Now
                };

                if (_entityId.HasValue)
                    await _entityService.UpdateAsync(entity);
                else
                    await _entityService.AddAsync(entity);

                _view.ShowSuccess(_entityId.HasValue ? "Updated successfully" : "Created successfully");
                _view.CloseForm();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save entity");
                _view.ShowError($"Failed to save: {ex.Message}");
            }
            finally
            {
                _view.IsSaveButtonEnabled = true;
                _view.IsLoadingVisible = false;
            }
        }

        private void OnCancelRequested(object sender, EventArgs e) => _view.CloseForm();

        private async void OnDeleteRequested(object sender, EventArgs e)
        {
            if (!_entityId.HasValue) return;

            if (_view.ShowConfirmation("Delete this entity?") != DialogResult.Yes) return;

            try
            {
                _view.IsLoadingVisible = true;
                await _entityService.DeleteAsync(_entityId.Value);
                _view.ShowSuccess("Deleted successfully");
                _view.CloseForm();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete entity");
                _view.ShowError($"Failed to delete: {ex.Message}");
            }
            finally
            {
                _view.IsLoadingVisible = false;
            }
        }

        public void Dispose()
        {
            if (_view != null)
            {
                _view.LoadRequested -= OnLoadRequested;
                _view.SaveRequested -= OnSaveRequested;
                _view.CancelRequested -= OnCancelRequested;
                _view.DeleteRequested -= OnDeleteRequested;
            }
        }
    }

    // ============================================================================
    // SECTION 2: LIST FORM WITH GRID TEMPLATE
    // Replace: YourEntity, YourEntityService
    // ============================================================================

    /// <summary>
    /// DevExpress list form with GridControl
    /// </summary>
    public partial class YourEntityListForm : XtraForm
    {
        private readonly IYourEntityService _entityService;
        private readonly IFormFactory _formFactory;
        private readonly ILogger<YourEntityListForm> _logger;

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
            this.Text = "[Your Entity] List";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            ConfigureGrid();
            WireUpEvents();
        }

        private void ConfigureGrid()
        {
            var gv = GridView;
            gv.OptionsBehavior.Editable = false;
            gv.OptionsBehavior.ReadOnly = true;
            gv.OptionsView.ShowGroupPanel = false;
            gv.OptionsSelection.MultiSelect = false;

            // Row appearance
            gv.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(0, 122, 204);
            gv.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White;
            gv.OptionsView.EnableAppearanceEvenRow = true;
            gv.Appearance.EvenRow.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);

            // Search panel
            gv.OptionsFind.AlwaysVisible = true;
            gv.OptionsFind.FindNullPrompt = "Search...";
            gv.OptionsView.ShowAutoFilterRow = true;

            // Events
            gv.DoubleClick += GridView_DoubleClick;
            gv.FocusedRowChanged += GridView_FocusedRowChanged;
        }

        private void WireUpEvents()
        {
            this.Load += async (s, e) => await LoadDataAsync();
            btnNew.Click += (s, e) => CreateNew();
            btnEdit.Click += (s, e) => EditSelected();
            btnDelete.Click += async (s, e) => await DeleteSelected();
            btnRefresh.Click += async (s, e) => await RefreshGrid();
            btnExport.Click += (s, e) => ExportToExcel();
        }

        #endregion

        #region Data Operations

        private async Task LoadDataAsync()
        {
            try
            {
                gridControlEntities.UseWaitCursor = true;
                var entities = await _entityService.GetAllAsync();
                gridControlEntities.DataSource = entities;
                ConfigureColumnsAfterBinding();
                UpdateStatusBar();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load entities");
                XtraMessageBox.Show($"Failed to load: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                gridControlEntities.UseWaitCursor = false;
            }
        }

        private void ConfigureColumnsAfterBinding()
        {
            var gv = GridView;

            // Hide technical columns
            if (gv.Columns["Id"] != null) gv.Columns["Id"].Visible = false;
            if (gv.Columns["CreatedBy"] != null) gv.Columns["CreatedBy"].Visible = false;

            // Set captions
            if (gv.Columns["Name"] != null) gv.Columns["Name"].Caption = "Entity Name";
            if (gv.Columns["CreatedDate"] != null)
            {
                gv.Columns["CreatedDate"].Caption = "Created";
                gv.Columns["CreatedDate"].DisplayFormat.FormatString = "yyyy-MM-dd HH:mm";
            }

            gv.BestFitColumns();
        }

        private void CreateNew()
        {
            var form = _formFactory.Create<YourEntityEditForm>();
            if (form.ShowDialog() == DialogResult.OK)
                RefreshGrid();
        }

        private void EditSelected()
        {
            var entity = GetSelectedEntity();
            if (entity == null)
            {
                XtraMessageBox.Show("Select an entity to edit", "No Selection");
                return;
            }

            var form = _formFactory.Create<YourEntityEditForm>();
            form.LoadEntity(entity.Id);
            if (form.ShowDialog() == DialogResult.OK)
                RefreshGrid();
        }

        private async Task DeleteSelected()
        {
            var entity = GetSelectedEntity();
            if (entity == null)
            {
                XtraMessageBox.Show("Select an entity to delete", "No Selection");
                return;
            }

            if (XtraMessageBox.Show($"Delete '{entity.Name}'?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            try
            {
                await _entityService.DeleteAsync(entity.Id);
                await RefreshGrid();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Delete failed: {ex.Message}", "Error");
            }
        }

        #endregion

        #region Grid Events

        private void GridView_DoubleClick(object sender, EventArgs e) => EditSelected();

        private void GridView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            bool hasSelection = GetSelectedEntity() != null;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
        }

        #endregion

        #region Export

        private void ExportToExcel()
        {
            var sfd = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                FileName = $"Entities_{DateTime.Now:yyyyMMdd}.xlsx"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    GridView.ExportToXlsx(sfd.FileName);
                    XtraMessageBox.Show("Export completed", "Success");
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Export failed: {ex.Message}", "Error");
                }
            }
        }

        #endregion

        #region Helper Methods

        private async Task RefreshGrid()
        {
            var selectedId = GetSelectedEntity()?.Id;
            await LoadDataAsync();

            if (selectedId.HasValue)
            {
                int rowHandle = GridView.LocateByValue("Id", selectedId.Value);
                if (rowHandle != GridControl.InvalidRowHandle)
                    GridView.FocusedRowHandle = rowHandle;
            }
        }

        private YourEntity GetSelectedEntity() => GridView.GetFocusedRow() as YourEntity;

        private void UpdateStatusBar()
        {
            lblStatus.Text = $"Total: {GridView.DataRowCount} | Visible: {GridView.VisibleRowCount}";
        }

        #endregion
    }

    #region Sample Models

    public class YourEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? EntityTypeId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class EntityType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    #endregion
}
