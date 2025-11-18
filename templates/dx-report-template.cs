// Template: DevExpress XtraReport Basic Template
// Replace: YourEntity, YourReport
// Usage: Basic report template with common patterns

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;

namespace YourNamespace.Reports
{
    /// <summary>
    /// Report for [YourEntity]
    /// </summary>
    public partial class YourEntityReport : XtraReport
    {
        public YourEntityReport()
        {
            InitializeComponent();
            ConfigureReport();
        }

        /// <summary>
        /// Sets the data source for the report
        /// </summary>
        public void SetDataSource(List<YourEntity> entities)
        {
            this.DataSource = entities;
        }

        /// <summary>
        /// Sets filter parameters for the report
        /// </summary>
        public void SetParameters(DateTime? fromDate, DateTime? toDate, string filterText)
        {
            // Set parameter values (if using parameters)
            if (fromDate.HasValue)
            {
                // Example: Set parameter
                // this.Parameters["FromDate"].Value = fromDate.Value;
            }

            if (toDate.HasValue)
            {
                // this.Parameters["ToDate"].Value = toDate.Value;
            }

            // Update header labels
            if (fromDate.HasValue && toDate.HasValue)
            {
                lblPeriod.Text = $"Period: {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}";
            }
        }

        private void ConfigureReport()
        {
            // Report settings
            this.Landscape = false;
            this.Margins = new System.Drawing.Printing.Margins(50, 50, 50, 50);
            this.PageHeight = 1169;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;

            // Font settings
            this.Font = new Font("Segoe UI", 9);
        }

        /// <summary>
        /// Shows report preview
        /// </summary>
        public void ShowPreview()
        {
            using (var printTool = new ReportPrintTool(this))
            {
                printTool.ShowPreview();
            }
        }

        /// <summary>
        /// Prints the report
        /// </summary>
        public void Print()
        {
            using (var printTool = new ReportPrintTool(this))
            {
                printTool.Print();
            }
        }

        /// <summary>
        /// Exports report to PDF
        /// </summary>
        public void ExportToPdf(string filePath)
        {
            this.ExportToPdf(filePath);
        }

        /// <summary>
        /// Exports report to Excel
        /// </summary>
        public void ExportToExcel(string filePath)
        {
            this.ExportToXlsx(filePath);
        }
    }

    /// <summary>
    /// Helper class for showing reports
    /// </summary>
    public static class ReportHelper
    {
        /// <summary>
        /// Shows report preview with error handling
        /// </summary>
        public static void ShowReportPreview<T>(
            List<T> data,
            Func<List<T>, XtraReport> createReportFunc,
            string errorTitle = "Report Error")
        {
            try
            {
                var report = createReportFunc(data);
                using (var printTool = new ReportPrintTool(report))
                {
                    printTool.ShowPreview();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to generate report: {ex.Message}",
                    errorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Exports report to PDF with file dialog
        /// </summary>
        public static void ExportReportToPdf<T>(
            List<T> data,
            Func<List<T>, XtraReport> createReportFunc,
            string defaultFileName = "Report.pdf")
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF Files|*.pdf",
                FileName = defaultFileName
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var report = createReportFunc(data);
                    report.ExportToPdf(saveFileDialog.FileName);

                    MessageBox.Show(
                        "Report exported successfully",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    // Open file
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = saveFileDialog.FileName,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Failed to export report: {ex.Message}",
                        "Export Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Exports report to Excel with file dialog
        /// </summary>
        public static void ExportReportToExcel<T>(
            List<T> data,
            Func<List<T>, XtraReport> createReportFunc,
            string defaultFileName = "Report.xlsx")
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                FileName = defaultFileName
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var report = createReportFunc(data);
                    report.ExportToXlsx(saveFileDialog.FileName);

                    MessageBox.Show(
                        "Report exported successfully",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    // Open file
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = saveFileDialog.FileName,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Failed to export report: {ex.Message}",
                        "Export Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Prints report directly
        /// </summary>
        public static void PrintReport<T>(
            List<T> data,
            Func<List<T>, XtraReport> createReportFunc)
        {
            try
            {
                var report = createReportFunc(data);
                using (var printTool = new ReportPrintTool(report))
                {
                    printTool.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to print report: {ex.Message}",
                    "Print Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }

    /// <summary>
    /// Example usage of XtraReport
    /// </summary>
    public class ReportExamples
    {
        #region Example 1: Simple Report Preview

        public void ShowCustomerReport(List<Customer> customers)
        {
            var report = new CustomerReport();
            report.SetDataSource(customers);
            report.ShowPreview();
        }

        #endregion

        #region Example 2: Report with Parameters

        public void ShowSalesReport(DateTime fromDate, DateTime toDate, List<Sale> sales)
        {
            var report = new SalesReport();
            report.SetDataSource(sales);
            report.SetParameters(fromDate, toDate, null);
            report.ShowPreview();
        }

        #endregion

        #region Example 3: Export Report

        public void ExportCustomerReport(List<Customer> customers)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF Files|*.pdf|Excel Files|*.xlsx",
                FileName = $"Customers_{DateTime.Now:yyyyMMdd}.pdf"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var report = new CustomerReport();
                report.SetDataSource(customers);

                if (saveFileDialog.FileName.EndsWith(".pdf"))
                {
                    report.ExportToPdf(saveFileDialog.FileName);
                }
                else if (saveFileDialog.FileName.EndsWith(".xlsx"))
                {
                    report.ExportToXlsx(saveFileDialog.FileName);
                }

                MessageBox.Show("Report exported successfully", "Success");
            }
        }

        #endregion

        #region Example 4: Using ReportHelper

        public void ShowReportWithHelper(List<Customer> customers)
        {
            ReportHelper.ShowReportPreview(
                customers,
                data =>
                {
                    var report = new CustomerReport();
                    report.SetDataSource(data);
                    return report;
                });
        }

        public void ExportReportWithHelper(List<Customer> customers)
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

        #endregion

        #region Example 5: Report from Form

        private void btnPrintReport_Click(object sender, EventArgs e)
        {
            try
            {
                // Get data
                var customers = _customerService.GetAll();

                // Create and show report
                var report = new CustomerReport();
                report.SetDataSource(customers);
                report.ShowPreview();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"Failed to generate report: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Example 6: Async Report Generation

        public async System.Threading.Tasks.Task ShowReportAsync(
            ICustomerService customerService)
        {
            try
            {
                // Show loading
                // ...

                // Load data asynchronously
                var customers = await customerService.GetAllAsync();

                // Create and show report
                var report = new CustomerReport();
                report.SetDataSource(customers);
                report.ShowPreview();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to generate report: {ex.Message}", "Error");
            }
        }

        #endregion
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
        public string Address { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class Sale
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }

    #endregion
}
