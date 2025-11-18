// Template: DevExpress Form with MVP Pattern
// Replace: YourEntity, YourPresenter, YourView
// Usage: Use this template for all DevExpress forms with MVP pattern

using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using Microsoft.Extensions.Logging;

namespace YourNamespace.Forms
{
    /// <summary>
    /// DevExpress form for [describe purpose]
    /// </summary>
    public partial class YourEntityForm : XtraForm, IYourEntityView
    {
        private readonly YourEntityPresenter _presenter;

        /// <summary>
        /// Initializes a new instance of the YourEntityForm class.
        /// </summary>
        /// <param name="presenter">The presenter for this view.</param>
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
            // Form properties
            this.Text = "[Your Entity] Management";
            this.MinimumSize = new Size(800, 600);
            this.Size = new Size(900, 650);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Apply DevExpress skin (optional)
            // this.LookAndFeel.SetSkinStyle("Office 2019 Colorful");

            // Configure layout control
            ConfigureLayoutControl();
        }

        private void ConfigureLayoutControl()
        {
            // layoutControl1 should exist in Designer
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.AllowCustomization = false;

            var root = layoutControl1.Root;
            root.GroupBordersVisible = false;
            root.Padding = new DevExpress.XtraLayout.Utils.Padding(10);

            // Set consistent label width for all items
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
            // Wire UI events to View events
            this.Load += (s, e) => LoadRequested?.Invoke(this, EventArgs.Empty);
            btnSave.Click += (s, e) => SaveRequested?.Invoke(this, EventArgs.Empty);
            btnCancel.Click += (s, e) => CancelRequested?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region IYourEntityView Properties

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
            set
            {
                layoutControl1.UseWaitCursor = value;
                if (value)
                {
                    // Optional: Show overlay progress panel
                    // overlayWindowOptions1.ShowProgressPanel();
                }
                else
                {
                    // overlayWindowOptions1.HideProgressPanel();
                }
            }
        }

        #endregion

        #region IYourEntityView Events

        public event EventHandler LoadRequested;
        public event EventHandler SaveRequested;
        public event EventHandler CancelRequested;
        public event EventHandler DeleteRequested;

        #endregion

        #region IYourEntityView Methods

        public void ShowError(string message)
        {
            XtraMessageBox.Show(
                message,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        public void ShowSuccess(string message)
        {
            XtraMessageBox.Show(
                message,
                "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        public void ShowWarning(string message)
        {
            XtraMessageBox.Show(
                message,
                "Warning",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        public DialogResult ShowConfirmation(string message)
        {
            return XtraMessageBox.Show(
                message,
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
        }

        public void SetEntityTypes(System.Collections.Generic.List<EntityType> entityTypes)
        {
            lkeEntityType.Properties.DataSource = entityTypes;
            lkeEntityType.Properties.DisplayMember = "Name";
            lkeEntityType.Properties.ValueMember = "Id";
            lkeEntityType.Properties.NullText = "-- Select Type --";

            // Configure columns
            lkeEntityType.Properties.Columns.Clear();
            lkeEntityType.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Type"));
            lkeEntityType.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Description"));

            // Enable search
            lkeEntityType.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoFilter;
        }

        public void CloseForm()
        {
            this.Close();
        }

        #endregion

        #region Validation

        private bool ValidateForm()
        {
            // Clear previous errors
            dxErrorProvider1.ClearErrors();

            bool isValid = true;

            // Validate required fields
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

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _presenter?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }

    /// <summary>
    /// View interface for YourEntityForm
    /// </summary>
    public interface IYourEntityView
    {
        // Properties for data binding
        string EntityName { get; set; }
        string EntityDescription { get; set; }
        int? EntityTypeId { get; set; }
        bool IsActive { get; set; }
        DateTime? CreatedDate { get; set; }

        // UI state properties
        bool IsSaveButtonEnabled { get; set; }
        bool IsLoadingVisible { get; set; }

        // Events the presenter will handle
        event EventHandler LoadRequested;
        event EventHandler SaveRequested;
        event EventHandler CancelRequested;
        event EventHandler DeleteRequested;

        // Methods for presenter to call
        void ShowError(string message);
        void ShowSuccess(string message);
        void ShowWarning(string message);
        DialogResult ShowConfirmation(string message);
        void SetEntityTypes(System.Collections.Generic.List<EntityType> entityTypes);
        void CloseForm();
    }

    /// <summary>
    /// Presenter for YourEntityForm
    /// </summary>
    public class YourEntityPresenter : IDisposable
    {
        private readonly IYourEntityService _entityService;
        private readonly ILogger<YourEntityPresenter> _logger;
        private IYourEntityView _view;
        private int? _entityId;

        public YourEntityPresenter(
            IYourEntityService entityService,
            ILogger<YourEntityPresenter> logger)
        {
            _entityService = entityService ?? throw new ArgumentNullException(nameof(entityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void SetView(IYourEntityView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));

            // Subscribe to view events
            _view.LoadRequested += OnLoadRequested;
            _view.SaveRequested += OnSaveRequested;
            _view.CancelRequested += OnCancelRequested;
            _view.DeleteRequested += OnDeleteRequested;
        }

        public void LoadEntity(int entityId)
        {
            _entityId = entityId;
        }

        private async void OnLoadRequested(object sender, EventArgs e)
        {
            try
            {
                _view.IsLoadingVisible = true;

                // Load reference data
                var entityTypes = await _entityService.GetEntityTypesAsync();
                _view.SetEntityTypes(entityTypes);

                // Load entity if editing
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
                    // New entity defaults
                    _view.IsActive = true;
                    _view.CreatedDate = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load entity");
                _view.ShowError($"Failed to load entity: {ex.Message}");
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

                // Create or update entity
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
                {
                    await _entityService.UpdateAsync(entity);
                    _view.ShowSuccess("Entity updated successfully");
                }
                else
                {
                    await _entityService.AddAsync(entity);
                    _view.ShowSuccess("Entity created successfully");
                }

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

        private void OnCancelRequested(object sender, EventArgs e)
        {
            _view.CloseForm();
        }

        private async void OnDeleteRequested(object sender, EventArgs e)
        {
            if (!_entityId.HasValue)
                return;

            var result = _view.ShowConfirmation("Are you sure you want to delete this entity?");
            if (result != DialogResult.Yes)
                return;

            try
            {
                _view.IsLoadingVisible = true;

                await _entityService.DeleteAsync(_entityId.Value);
                _view.ShowSuccess("Entity deleted successfully");
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
}
