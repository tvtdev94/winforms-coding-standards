// Template: DevExpress LookUpEdit Patterns
// Usage: Common patterns for LookUpEdit configuration

using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace YourNamespace.Helpers
{
    /// <summary>
    /// Helper class for configuring DevExpress LookUpEdit controls
    /// </summary>
    public static class LookUpEditHelper
    {
        #region Basic LookUpEdit Setup

        /// <summary>
        /// Configures a basic LookUpEdit with standard settings
        /// </summary>
        public static void ConfigureBasicLookUp<T>(
            LookUpEdit lookUpEdit,
            List<T> dataSource,
            string displayMember,
            string valueMember,
            string nullText = "-- Select --")
        {
            // Data source
            lookUpEdit.Properties.DataSource = dataSource;
            lookUpEdit.Properties.DisplayMember = displayMember;
            lookUpEdit.Properties.ValueMember = valueMember;

            // Basic settings
            lookUpEdit.Properties.NullText = nullText;
            lookUpEdit.Properties.SearchMode = SearchMode.AutoFilter;
            lookUpEdit.Properties.AutoSearchColumnIndex = 0;

            // Clear existing columns
            lookUpEdit.Properties.Columns.Clear();

            // Add display column
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
            // Data source
            lookUpEdit.Properties.DataSource = dataSource;
            lookUpEdit.Properties.DisplayMember = displayMember;
            lookUpEdit.Properties.ValueMember = valueMember;

            // Basic settings
            lookUpEdit.Properties.NullText = nullText;
            lookUpEdit.Properties.SearchMode = SearchMode.AutoFilter;
            lookUpEdit.Properties.AutoSearchColumnIndex = 0;

            // Clear existing columns
            lookUpEdit.Properties.Columns.Clear();

            // Add columns
            foreach (var column in columns)
            {
                lookUpEdit.Properties.Columns.Add(new LookUpColumnInfo(column.Key, column.Value));
            }

            // Set popup width
            lookUpEdit.Properties.PopupWidth = 400;
        }

        #endregion

        #region Cascading LookUpEdit

        /// <summary>
        /// Sets up cascading relationship between two LookUpEdits
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
                    var childData = getChildDataFunc(parentId.Value);
                    childLookUp.Properties.DataSource = childData;
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

        #region Filter & Search

        /// <summary>
        /// Enables instant search with custom filter
        /// </summary>
        public static void EnableInstantSearch(LookUpEdit lookUpEdit)
        {
            lookUpEdit.Properties.SearchMode = SearchMode.AutoFilter;
            lookUpEdit.Properties.AutoSearchColumnIndex = 0;
            lookUpEdit.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
        }

        #endregion

        #region Get/Set Values

        /// <summary>
        /// Gets selected value safely
        /// </summary>
        public static T GetSelectedValue<T>(LookUpEdit lookUpEdit) where T : struct
        {
            if (lookUpEdit.EditValue is T value)
                return value;

            return default(T);
        }

        /// <summary>
        /// Gets selected value as nullable
        /// </summary>
        public static T? GetSelectedValueNullable<T>(LookUpEdit lookUpEdit) where T : struct
        {
            return lookUpEdit.EditValue as T?;
        }

        /// <summary>
        /// Gets selected object
        /// </summary>
        public static T GetSelectedObject<T>(LookUpEdit lookUpEdit) where T : class
        {
            return lookUpEdit.GetSelectedDataRow() as T;
        }

        #endregion
    }

    /// <summary>
    /// Example usage of LookUpEdit patterns
    /// </summary>
    public class LookUpEditExamples
    {
        #region Example 1: Simple LookUp

        public void SetupCustomerTypeLookUp(LookUpEdit lkeCustomerType, List<CustomerType> customerTypes)
        {
            // Simple single-column lookup
            lkeCustomerType.Properties.DataSource = customerTypes;
            lkeCustomerType.Properties.DisplayMember = "Name";
            lkeCustomerType.Properties.ValueMember = "Id";
            lkeCustomerType.Properties.NullText = "-- Select Type --";

            // Enable search
            lkeCustomerType.Properties.SearchMode = SearchMode.AutoFilter;

            // Configure columns
            lkeCustomerType.Properties.Columns.Clear();
            lkeCustomerType.Properties.Columns.Add(new LookUpColumnInfo("Name", "Type"));
        }

        #endregion

        #region Example 2: Multi-Column LookUp

        public void SetupCustomerLookUp(LookUpEdit lkeCustomer, List<Customer> customers)
        {
            // Multi-column lookup
            lkeCustomer.Properties.DataSource = customers;
            lkeCustomer.Properties.DisplayMember = "Name";
            lkeCustomer.Properties.ValueMember = "Id";
            lkeCustomer.Properties.NullText = "-- Select Customer --";

            // Enable search
            lkeCustomer.Properties.SearchMode = SearchMode.AutoFilter;
            lkeCustomer.Properties.AutoSearchColumnIndex = 0;

            // Configure columns
            lkeCustomer.Properties.Columns.Clear();
            lkeCustomer.Properties.Columns.Add(new LookUpColumnInfo("Name", "Customer", 200));
            lkeCustomer.Properties.Columns.Add(new LookUpColumnInfo("Email", "Email", 250));
            lkeCustomer.Properties.Columns.Add(new LookUpColumnInfo("Phone", "Phone", 150));

            // Set popup width
            lkeCustomer.Properties.PopupWidth = 600;

            // Best fit columns
            lkeCustomer.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
        }

        #endregion

        #region Example 3: Cascading LookUps (Country -> State -> City)

        private List<State> _states;
        private List<City> _cities;

        public void SetupCascadingLookUps(
            LookUpEdit lkeCountry,
            LookUpEdit lkeState,
            LookUpEdit lkeCity,
            List<Country> countries,
            List<State> states,
            List<City> cities)
        {
            _states = states;
            _cities = cities;

            // Setup Country
            lkeCountry.Properties.DataSource = countries;
            lkeCountry.Properties.DisplayMember = "Name";
            lkeCountry.Properties.ValueMember = "Id";
            lkeCountry.Properties.NullText = "-- Select Country --";

            // Setup State (initially disabled)
            lkeState.Properties.DisplayMember = "Name";
            lkeState.Properties.ValueMember = "Id";
            lkeState.Properties.NullText = "-- Select State --";
            lkeState.Enabled = false;

            // Setup City (initially disabled)
            lkeCity.Properties.DisplayMember = "Name";
            lkeCity.Properties.ValueMember = "Id";
            lkeCity.Properties.NullText = "-- Select City --";
            lkeCity.Enabled = false;

            // Country changed -> Load States
            lkeCountry.EditValueChanged += (s, e) =>
            {
                int? countryId = lkeCountry.EditValue as int?;

                if (countryId.HasValue)
                {
                    var filteredStates = _states.Where(st => st.CountryId == countryId.Value).ToList();
                    lkeState.Properties.DataSource = filteredStates;
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

            // State changed -> Load Cities
            lkeState.EditValueChanged += (s, e) =>
            {
                int? stateId = lkeState.EditValue as int?;

                if (stateId.HasValue)
                {
                    var filteredCities = _cities.Where(c => c.StateId == stateId.Value).ToList();
                    lkeCity.Properties.DataSource = filteredCities;
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

        #region Example 4: Async Data Loading

        public async System.Threading.Tasks.Task SetupCustomerLookUpAsync(
            LookUpEdit lkeCustomer,
            ICustomerService customerService)
        {
            try
            {
                // Show loading
                lkeCustomer.Enabled = false;

                // Load data asynchronously
                var customers = await customerService.GetAllActiveAsync();

                // Configure lookup
                lkeCustomer.Properties.DataSource = customers;
                lkeCustomer.Properties.DisplayMember = "Name";
                lkeCustomer.Properties.ValueMember = "Id";
                lkeCustomer.Properties.NullText = "-- Select Customer --";

                // Columns
                lkeCustomer.Properties.Columns.Clear();
                lkeCustomer.Properties.Columns.Add(new LookUpColumnInfo("Name", "Customer"));
                lkeCustomer.Properties.Columns.Add(new LookUpColumnInfo("Email", "Email"));

                lkeCustomer.Enabled = true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Failed to load customers: {ex.Message}", "Error");
            }
        }

        #endregion

        #region Example 5: Get Selected Values

        public void GetSelectedValues(
            LookUpEdit lkeCustomer,
            LookUpEdit lkeCustomerType,
            LookUpEdit lkeCountry)
        {
            // Get as int
            int customerId = lkeCustomer.EditValue as int? ?? 0;

            // Get as nullable int
            int? customerTypeId = lkeCustomerType.EditValue as int?;

            // Get selected object
            var selectedCustomer = lkeCustomer.GetSelectedDataRow() as Customer;

            // Get display text
            string displayText = lkeCustomer.Text;
        }

        #endregion

        #region Example 6: Validation

        public bool ValidateLookUp(LookUpEdit lookUpEdit, DXErrorProvider errorProvider, string fieldName)
        {
            if (lookUpEdit.EditValue == null)
            {
                errorProvider.SetError(lookUpEdit, $"{fieldName} is required");
                return false;
            }

            errorProvider.SetError(lookUpEdit, string.Empty);
            return true;
        }

        #endregion
    }

    #region Sample Models

    public class CustomerType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int CustomerTypeId { get; set; }
    }

    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
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

    #endregion
}
