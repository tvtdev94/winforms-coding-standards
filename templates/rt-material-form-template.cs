// Template: ReaLTaiizor Material Form with MVP Pattern
// Replace: YourEntity, YourPresenter, YourView
// Usage: Use this template for Material Design forms with MVP pattern

using System;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Controls;
using Microsoft.Extensions.Logging;

namespace YourNamespace.Forms
{
    /// <summary>
    /// Material Design form for [describe purpose]
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

        /// <summary>
        /// Initializes a new instance of the YourEntityForm class.
        /// </summary>
        public YourEntityForm(YourEntityPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            _presenter.SetView(this);

            InitializeMaterialForm();
            WireUpEvents();
        }

        #region Initialization

        private void InitializeMaterialForm()
        {
            // Form properties
            this.Text = "[Your Entity] Management";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.Sizable = false;

            // Initialize controls
            InitializeControls();
        }

        private void InitializeControls()
        {
            // Title Label
            lblTitle = new MaterialLabel
            {
                Location = new Point(20, 70),
                Size = new Size(560, 30),
                Text = "Entity Information",
                Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold)
            };
            this.Controls.Add(lblTitle);

            // Entity Name TextBox
            txtEntityName = new MaterialTextBox
            {
                Location = new Point(20, 110),
                Size = new Size(560, 50),
                Hint = "Entity Name"
            };
            this.Controls.Add(txtEntityName);

            // Description TextBox
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

            // Save Button
            btnSave = new MaterialButton
            {
                Location = new Point(480, 350),
                Size = new Size(100, 40),
                Text = "Save",
                Type = MaterialButton.MaterialButtonType.Contained
            };
            this.Controls.Add(btnSave);

            // Cancel Button
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

        #region IYourEntityView Properties

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

        #endregion

        #region IYourEntityView Events

        public event EventHandler LoadRequested;
        public event EventHandler SaveRequested;
        public event EventHandler CancelRequested;

        #endregion

        #region IYourEntityView Methods

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowSuccess(string message)
        {
            MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void SetEntityTypes(System.Collections.Generic.List<EntityType> entityTypes)
        {
            cboEntityType.DataSource = entityTypes;
            cboEntityType.DisplayMember = "Name";
            cboEntityType.ValueMember = "Id";
        }

        public void CloseForm()
        {
            this.Close();
        }

        #endregion

        #region Validation

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtEntityName.Text))
            {
                ShowError("Entity name is required");
                txtEntityName.Focus();
                return false;
            }

            if (cboEntityType.SelectedValue == null)
            {
                ShowError("Entity type is required");
                cboEntityType.Focus();
                return false;
            }

            return true;
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
        // Properties
        string EntityName { get; set; }
        string EntityDescription { get; set; }
        int? EntityTypeId { get; set; }
        bool IsActive { get; set; }
        bool IsSaveButtonEnabled { get; set; }

        // Events
        event EventHandler LoadRequested;
        event EventHandler SaveRequested;
        event EventHandler CancelRequested;

        // Methods
        void ShowError(string message);
        void ShowSuccess(string message);
        void SetEntityTypes(System.Collections.Generic.List<EntityType> entityTypes);
        void CloseForm();
    }
}
