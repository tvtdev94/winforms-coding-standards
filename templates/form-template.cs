// Template: WinForms Form with MVP Pattern
// Replace: YourForm, YourPresenter, YourView

using System;
using System.Windows.Forms;

namespace YourNamespace.Forms
{
    /// <summary>
    /// Form for [describe purpose]
    /// </summary>
    public partial class YourForm : Form, IYourView
    {
        private readonly YourPresenter _presenter;

        /// <summary>
        /// Initializes a new instance of the YourForm class.
        /// </summary>
        /// <param name="presenter">The presenter for this view.</param>
        public YourForm(YourPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            _presenter.AttachView(this);

            // Wire UI events to View events
            Load += (s, e) => LoadRequested?.Invoke(this, EventArgs.Empty);
            btnSave.Click += (s, e) => SaveRequested?.Invoke(this, EventArgs.Empty);
            btnCancel.Click += (s, e) => CancelRequested?.Invoke(this, EventArgs.Empty);
        }

        #region IYourView Properties

        public string PropertyName
        {
            get => txtPropertyName.Text;
            set => txtPropertyName.Text = value;
        }

        public bool IsSaveButtonEnabled
        {
            get => btnSave.Enabled;
            set => btnSave.Enabled = value;
        }

        public bool IsLoadingVisible
        {
            get => lblLoading.Visible;
            set => lblLoading.Visible = value;
        }

        #endregion

        #region IYourView Events

        public event EventHandler LoadRequested;
        public event EventHandler SaveRequested;
        public event EventHandler CancelRequested;

        #endregion

        #region IYourView Methods

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowSuccess(string message)
        {
            MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _presenter.DetachView();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// View interface for YourForm
    /// </summary>
    public interface IYourView
    {
        // Properties
        string PropertyName { get; set; }
        bool IsSaveButtonEnabled { get; set; }
        bool IsLoadingVisible { get; set; }

        // Events
        event EventHandler LoadRequested;
        event EventHandler SaveRequested;
        event EventHandler CancelRequested;

        // Methods
        void ShowError(string message);
        void ShowSuccess(string message);
        void Close();
    }
}
