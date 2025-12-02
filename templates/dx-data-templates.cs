// Template: DevExpress Data Templates (LookUp + Reports)
// Usage: Templates for DevExpress LookUpEdit and XtraReport
// Sections: LookUpEdit Patterns, Report Templates

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;

namespace YourNamespace.Helpers
{
    // ============================================================================
    // SECTION 1: LOOKUPEDIT PATTERNS
    // Common patterns for LookUpEdit configuration
    // ============================================================================

    /// <summary>
    /// Helper class for DevExpress LookUpEdit controls
    /// </summary>
    public static class LookUpEditHelper
    {
        #region Basic Setup

        /// <summary>
        /// Configures a basic LookUpEdit
        /// </summary>
        public static void ConfigureBasicLookUp<T>(
            LookUpEdit lookUpEdit,
            List<T> dataSource,
            string displayMember,
            string valueMember,
            string nullText = "-- Select --")
        {
            lookUpEdit.Properties.DataSource = dataSource;
            lookUpEdit.Properties.DisplayMember = displayMember;
            lookUpEdit.Properties.ValueMember = valueMember;
            lookUpEdit.Properties.NullText = nullText;
            lookUpEdit.Properties.SearchMode = SearchMode.AutoFilter;

            lookUpEdit.Properties.Columns.Clear();
            lookUpEdit.Properties.Columns.Add(new LookUpColumnInfo(displayMember));
        }

        /// <summary>
        /// Configures LookUpEdit with multiple columns
        /// </summary>
        public static void ConfigureMultiColumnLookUp<T>(
            LookUpEdit lookUpEdit,
            List<T> dataSource,
            string displayMember,
            string valueMember,
            Dictionary<string, string> columns,
            string nullText = "-- Select --")
        {
            lookUpEdit.Properties.DataSource = dataSource;
            lookUpEdit.Properties.DisplayMember = displayMember;
            lookUpEdit.Properties.ValueMember = valueMember;
            lookUpEdit.Properties.NullText = nullText;
            lookUpEdit.Properties.SearchMode = SearchMode.AutoFilter;

            lookUpEdit.Properties.Columns.Clear();
            foreach (var col in columns)
                lookUpEdit.Properties.Columns.Add(new LookUpColumnInfo(col.Key, col.Value));

            lookUpEdit.Properties.PopupWidth = 400;
        }

        #endregion

        #region Cascading LookUp

        /// <summary>
        /// Sets up cascading relationship between LookUpEdits
        /// </summary>
        public static void SetupCascadingLookUp(
            LookUpEdit parentLookUp,
            LookUpEdit childLookUp,
            Func<int, List<object>> getChildDataFunc)
        {
            parentLookUp.EditValueChanged += (s, e) =>
            {
                int? parentId = parentLookUp.EditValue as int?;

                if (parentId.HasValue)
                {
                    childLookUp.Properties.DataSource = getChildDataFunc(parentId.Value);
                    childLookUp.Enabled = true;
                }
                else
                {
                    childLookUp.Properties.DataSource = null;
                    childLookUp.EditValue = null;
                    childLookUp.Enabled = false;
                }
            };
        }

        #endregion

        #region Get/Set Values

        public static T GetSelectedValue<T>(LookUpEdit lookUpEdit) where T : struct
            => lookUpEdit.EditValue is T value ? value : default;

        public static T? GetSelectedValueNullable<T>(LookUpEdit lookUpEdit) where T : struct
            => lookUpEdit.EditValue as T?;

        public static T GetSelectedObject<T>(LookUpEdit lookUpEdit) where T : class
            => lookUpEdit.GetSelectedDataRow() as T;

        #endregion
    }

    /// <summary>
    /// LookUpEdit usage examples
    /// </summary>
    public class LookUpEditExamples
    {
        #region Simple LookUp

        public void SetupCustomerTypeLookUp(LookUpEdit lke, List<CustomerType> types)
        {
            lke.Properties.DataSource = types;
            lke.Properties.DisplayMember = "Name";
            lke.Properties.ValueMember = "Id";
            lke.Properties.NullText = "-- Select Type --";
            lke.Properties.SearchMode = SearchMode.AutoFilter;

            lke.Properties.Columns.Clear();
            lke.Properties.Columns.Add(new LookUpColumnInfo("Name", "Type"));
        }

        #endregion

        #region Multi-Column LookUp

        public void SetupCustomerLookUp(LookUpEdit lke, List<Customer> customers)
        {
            lke.Properties.DataSource = customers;
            lke.Properties.DisplayMember = "Name";
            lke.Properties.ValueMember = "Id";
            lke.Properties.NullText = "-- Select Customer --";
            lke.Properties.SearchMode = SearchMode.AutoFilter;

            lke.Properties.Columns.Clear();
            lke.Properties.Columns.Add(new LookUpColumnInfo("Name", "Customer", 200));
            lke.Properties.Columns.Add(new LookUpColumnInfo("Email", "Email", 250));
            lke.Properties.Columns.Add(new LookUpColumnInfo("Phone", "Phone", 150));

            lke.Properties.PopupWidth = 600;
            lke.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
        }

        #endregion

        #region Cascading LookUps (Country -> State -> City)

        public void SetupCascadingLookUps(
            LookUpEdit lkeCountry,
            LookUpEdit lkeState,
            LookUpEdit lkeCity,
            List<Country> countries,
            List<State> states,
            List<City> cities)
        {
            // Country
            lkeCountry.Properties.DataSource = countries;
            lkeCountry.Properties.DisplayMember = "Name";
            lkeCountry.Properties.ValueMember = "Id";
            lkeCountry.Properties.NullText = "-- Select Country --";

            // State (initially disabled)
            lkeState.Properties.DisplayMember = "Name";
            lkeState.Properties.ValueMember = "Id";
            lkeState.Properties.NullText = "-- Select State --";
            lkeState.Enabled = false;

            // City (initially disabled)
            lkeCity.Properties.DisplayMember = "Name";
            lkeCity.Properties.ValueMember = "Id";
            lkeCity.Properties.NullText = "-- Select City --";
            lkeCity.Enabled = false;

            // Country -> State
            lkeCountry.EditValueChanged += (s, e) =>
            {
                int? countryId = lkeCountry.EditValue as int?;
                if (countryId.HasValue)
                {
                    lkeState.Properties.DataSource = states.Where(st => st.CountryId == countryId.Value).ToList();
                    lkeState.Enabled = true;
                }
                else
                {
                    lkeState.Properties.DataSource = null;
                    lkeState.EditValue = null;
                    lkeState.Enabled = false;
                    lkeCity.Properties.DataSource = null;
                    lkeCity.EditValue = null;
                    lkeCity.Enabled = false;
                }
            };

            // State -> City
            lkeState.EditValueChanged += (s, e) =>
            {
                int? stateId = lkeState.EditValue as int?;
                if (stateId.HasValue)
                {
                    lkeCity.Properties.DataSource = cities.Where(c => c.StateId == stateId.Value).ToList();
                    lkeCity.Enabled = true;
                }
                else
                {
                    lkeCity.Properties.DataSource = null;
                    lkeCity.EditValue = null;
                    lkeCity.Enabled = false;
                }
            };
        }

        #endregion

        #region Async Data Loading

        public async System.Threading.Tasks.Task SetupCustomerLookUpAsync(
            LookUpEdit lke,
            ICustomerService service)
        {
            try
            {
                lke.Enabled = false;
                var customers = await service.GetAllActiveAsync();

                lke.Properties.DataSource = customers;
                lke.Properties.DisplayMember = "Name";
                lke.Properties.ValueMember = "Id";
                lke.Properties.Columns.Clear();
                lke.Properties.Columns.Add(new LookUpColumnInfo("Name", "Customer"));
                lke.Properties.Columns.Add(new LookUpColumnInfo("Email", "Email"));

                lke.Enabled = true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Failed to load: {ex.Message}", "Error");
            }
        }

        #endregion

        #region Validation

        public bool ValidateLookUp(LookUpEdit lke, DXErrorProvider errorProvider, string fieldName)
        {
            if (lke.EditValue == null)
            {
                errorProvider.SetError(lke, $"{fieldName} is required");
                return false;
            }

            errorProvider.SetError(lke, string.Empty);
            return true;
        }

        #endregion
    }
}

// ============================================================================
// SECTION 2: XTRAREPORT TEMPLATES
// Basic report template with common patterns
// ============================================================================

namespace YourNamespace.Reports
{
    /// <summary>
    /// Basic XtraReport template
    /// </summary>
    public partial class YourEntityReport : XtraReport
    {
        public YourEntityReport()
        {
            InitializeComponent();
            ConfigureReport();
        }

        public void SetDataSource(List<YourEntity> entities) => this.DataSource = entities;

        public void SetParameters(DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate.HasValue && toDate.HasValue)
                lblPeriod.Text = $"Period: {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}";
        }

        private void ConfigureReport()
        {
            this.Landscape = false;
            this.Margins = new System.Drawing.Printing.Margins(50, 50, 50, 50);
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.Font = new Font("Segoe UI", 9);
        }

        public void ShowPreview()
        {
            using (var printTool = new ReportPrintTool(this))
                printTool.ShowPreview();
        }

        public void Print()
        {
            using (var printTool = new ReportPrintTool(this))
                printTool.Print();
        }
    }

    /// <summary>
    /// Report helper utilities
    /// </summary>
    public static class ReportHelper
    {
        public static void ShowReportPreview<T>(
            List<T> data,
            Func<List<T>, XtraReport> createReportFunc)
        {
            try
            {
                var report = createReportFunc(data);
                using (var printTool = new ReportPrintTool(report))
                    printTool.ShowPreview();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to generate report: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void ExportReportToPdf<T>(
            List<T> data,
            Func<List<T>, XtraReport> createReportFunc,
            string defaultFileName = "Report.pdf")
        {
            var sfd = new SaveFileDialog
            {
                Filter = "PDF Files|*.pdf",
                FileName = defaultFileName
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var report = createReportFunc(data);
                    report.ExportToPdf(sfd.FileName);
                    MessageBox.Show("Report exported successfully", "Success");

                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = sfd.FileName,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Export failed: {ex.Message}", "Error");
                }
            }
        }

        public static void ExportReportToExcel<T>(
            List<T> data,
            Func<List<T>, XtraReport> createReportFunc,
            string defaultFileName = "Report.xlsx")
        {
            var sfd = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                FileName = defaultFileName
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var report = createReportFunc(data);
                    report.ExportToXlsx(sfd.FileName);
                    MessageBox.Show("Report exported successfully", "Success");

                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = sfd.FileName,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Export failed: {ex.Message}", "Error");
                }
            }
        }
    }

    /// <summary>
    /// Report usage examples
    /// </summary>
    public class ReportExamples
    {
        public void ShowCustomerReport(List<Customer> customers)
        {
            var report = new CustomerReport();
            report.SetDataSource(customers);
            report.ShowPreview();
        }

        public void ShowSalesReport(DateTime fromDate, DateTime toDate, List<Sale> sales)
        {
            var report = new SalesReport();
            report.SetDataSource(sales);
            report.SetParameters(fromDate, toDate);
            report.ShowPreview();
        }

        public void ExportWithHelper(List<Customer> customers)
        {
            ReportHelper.ExportReportToPdf(
                customers,
                data =>
                {
                    var report = new CustomerReport();
                    report.SetDataSource(data);
                    return report;
                },
                $"Customers_{DateTime.Now:yyyyMMdd}.pdf");
        }

        public async System.Threading.Tasks.Task ShowReportAsync(ICustomerService service)
        {
            try
            {
                var customers = await service.GetAllAsync();
                var report = new CustomerReport();
                report.SetDataSource(customers);
                report.ShowPreview();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to generate report: {ex.Message}", "Error");
            }
        }
    }

    #region Sample Models

    public class YourEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class CustomerType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class State
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountryId { get; set; }
    }

    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StateId { get; set; }
    }

    public class Sale
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
    }

    #endregion
}
