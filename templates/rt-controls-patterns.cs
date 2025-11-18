// Template: ReaLTaiizor Common Control Patterns
// Usage: Copy-paste patterns for common scenarios

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ReaLTaiizor.Controls;

namespace YourNamespace.Patterns
{
    /// <summary>
    /// Common ReaLTaiizor control patterns
    /// </summary>
    public class ReaLTaiizorPatterns
    {
        #region Material ListView Pattern

        public void MaterialListViewPattern(List<Customer> customers)
        {
            var lvCustomers = new MaterialListView();
            lvCustomers.View = View.Details;
            lvCustomers.FullRowSelect = true;
            lvCustomers.GridLines = true;

            // Columns
            lvCustomers.Columns.Add("ID", 60);
            lvCustomers.Columns.Add("Name", 200);
            lvCustomers.Columns.Add("Email", 250);
            lvCustomers.Columns.Add("Active", 80);

            // Data
            lvCustomers.Items.Clear();
            foreach (var customer in customers)
            {
                var item = new ListViewItem(new[]
                {
                    customer.Id.ToString(),
                    customer.Name,
                    customer.Email,
                    customer.IsActive ? "Yes" : "No"
                });
                lvCustomers.Items.Add(item);
            }

            // Double-click to edit
            lvCustomers.DoubleClick += (s, e) =>
            {
                if (lvCustomers.SelectedItems.Count > 0)
                {
                    var id = int.Parse(lvCustomers.SelectedItems[0].Text);
                    // Open edit form
                }
            };
        }

        #endregion

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
                new CustomerType { Id = 2, Name = "Premium" },
                new CustomerType { Id = 3, Name = "VIP" }
            };
            cboType.DataSource = types;
            cboType.DisplayMember = "Name";
            cboType.ValueMember = "Id";
            cboType.Hint = "Select type...";

            // Get selected value
            var selectedType = (CustomerType)cboType.SelectedItem;
            int selectedId = (int)cboType.SelectedValue;
        }

        #endregion

        #region Metro Grid Pattern

        public void MetroGridPattern(List<Customer> customers)
        {
            var grid = new MetroGrid();

            // Configuration
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;

            // Bind data
            grid.DataSource = customers;

            // Hide/Configure columns
            grid.Columns["Id"].Visible = false;
            grid.Columns["Name"].HeaderText = "Customer Name";
            grid.Columns["Name"].Width = 200;
            grid.Columns["Email"].HeaderText = "Email Address";
            grid.Columns["Email"].Width = 250;

            // Cell formatting
            grid.CellFormatting += (s, e) =>
            {
                if (grid.Columns[e.ColumnIndex].Name == "IsActive")
                {
                    bool isActive = (bool)e.Value;
                    e.CellStyle.BackColor = isActive ?
                        System.Drawing.Color.LightGreen :
                        System.Drawing.Color.LightCoral;
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
            // Name validation
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Name is required", "Validation Error");
                txtName.Focus();
                return false;
            }

            // Email validation
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Email is required", "Validation Error");
                txtEmail.Focus();
                return false;
            }

            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Invalid email format", "Validation Error");
                txtEmail.Focus();
                return false;
            }

            // ComboBox validation
            if (cboType.SelectedValue == null)
            {
                MessageBox.Show("Type is required", "Validation Error");
                cboType.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Button Click Patterns

        public void MaterialButtonPatterns()
        {
            var btnSave = new MaterialButton();
            btnSave.Text = "Save";
            btnSave.Type = MaterialButton.MaterialButtonType.Contained;

            // Async click handler
            btnSave.Click += async (s, e) =>
            {
                try
                {
                    btnSave.Enabled = false;
                    btnSave.Text = "Saving...";

                    // Perform async operation
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

        #region Models

        public class Customer
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public bool IsActive { get; set; }
            public int CustomerTypeId { get; set; }
        }

        public class CustomerType
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public override string ToString() => Name;
        }

        #endregion
    }
}
