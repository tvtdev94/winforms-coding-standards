# Data Binding in WinForms

## 📋 Overview

Data binding is a mechanism that establishes a connection between the UI and data sources, automatically synchronizing values between them. In WinForms, data binding eliminates the need for manual UI updates when data changes and vice versa.

**What is Data Binding?**
- Automatic synchronization between UI controls and data sources
- Two-way communication: UI ↔ Data
- Built-in support for simple and complex binding scenarios
- Works with objects, collections, and datasets

**Types of Binding:**
- **Simple Binding**: One control property bound to one data property (TextBox.Text → Customer.Name)
- **Complex Binding**: Control bound to entire collection (DataGridView → List&lt;Customer&gt;)

---

## 🎯 Why This Matters

### Benefits of Data Binding

**1. Reduced Code**
```csharp
// ❌ Manual approach (10+ lines per property)
private void UpdateUI()
{
    txtName.Text = customer.Name;
    txtEmail.Text = customer.Email;
    dtpBirthDate.Value = customer.BirthDate;
    chkIsActive.Checked = customer.IsActive;
}

// ✅ Data binding (one-time setup)
txtName.DataBindings.Add("Text", customer, nameof(Customer.Name));
txtEmail.DataBindings.Add("Text", customer, nameof(Customer.Email));
dtpBirthDate.DataBindings.Add("Value", customer, nameof(Customer.BirthDate));
chkIsActive.DataBindings.Add("Checked", customer, nameof(Customer.IsActive));
```

**2. Automatic Synchronization**
- Changes in UI automatically update data
- Changes in data automatically update UI (with INotifyPropertyChanged)
- No manual event handlers needed

**3. Cleaner Architecture**
- Separation of UI and business logic
- Less coupling between layers
- Easier to test (data logic separate from UI)

**4. Less Error-Prone**
- No forgotten UI updates
- Type-safe property binding (with nameof)
- Consistent state between UI and data

---

## 🔧 Core Components

### BindingSource

**What is BindingSource?**
- Intermediary between data source and controls
- Provides currency management (current position in list)
- Enables filtering, sorting, and navigation
- Single point of control for all bindings

**Why Use BindingSource?**
- ✅ Simplifies complex binding scenarios
- ✅ Provides consistent interface for different data sources
- ✅ Enables master-detail relationships
- ✅ Supports filtering and sorting
- ✅ Automatic change notification

**Creating and Configuring:**

```csharp
public partial class CustomerForm : Form
{
    private BindingSource customerBindingSource;
    private List<Customer> customers;

    public CustomerForm()
    {
        InitializeComponent();
        InitializeBindingSource();
    }

    private void InitializeBindingSource()
    {
        // Create BindingSource
        customerBindingSource = new BindingSource();

        // Set data source
        customers = GetCustomers(); // Your data retrieval method
        customerBindingSource.DataSource = customers;

        // Bind controls to BindingSource
        txtName.DataBindings.Add("Text", customerBindingSource,
            nameof(Customer.Name), false, DataSourceUpdateMode.OnPropertyChanged);
        txtEmail.DataBindings.Add("Text", customerBindingSource,
            nameof(Customer.Email), false, DataSourceUpdateMode.OnPropertyChanged);

        // Bind DataGridView (complex binding)
        dgvCustomers.DataSource = customerBindingSource;

        // Navigation buttons
        btnPrevious.Click += (s, e) => customerBindingSource.MovePrevious();
        btnNext.Click += (s, e) => customerBindingSource.MoveNext();
    }
}
```

**BindingSource Methods:**
```csharp
// Navigation
customerBindingSource.MoveFirst();
customerBindingSource.MoveLast();
customerBindingSource.MoveNext();
customerBindingSource.MovePrevious();
customerBindingSource.Position = 5; // Move to specific index

// Data manipulation
customerBindingSource.Add(newCustomer);       // Add new item
customerBindingSource.RemoveCurrent();        // Remove current item
customerBindingSource.ResetBindings(false);   // Refresh all bindings

// Filtering and sorting
customerBindingSource.Filter = "IsActive = true";
customerBindingSource.Sort = "Name ASC";

// Current item access
var current = (Customer)customerBindingSource.Current;
```

---

### DataSourceUpdateMode

Controls **when** data source is updated from UI changes.

**Three Modes:**

| Mode | When Updates Occur | Use Case |
|------|-------------------|----------|
| **OnPropertyChanged** | Immediately on every keystroke | Real-time validation, live calculations |
| **OnValidation** | When control loses focus (Validating event) | Standard forms, delayed validation |
| **Never** | Manual updates only | Read-only displays, custom update logic |

**Examples:**

```csharp
// OnPropertyChanged - Immediate updates
txtName.DataBindings.Add("Text", customer, nameof(Customer.Name),
    false, DataSourceUpdateMode.OnPropertyChanged);
// Use case: Live character counter, instant search

// OnValidation - Update on focus loss (DEFAULT)
txtEmail.DataBindings.Add("Text", customer, nameof(Customer.Email),
    false, DataSourceUpdateMode.OnValidation);
// Use case: Standard data entry forms

// Never - Manual control
txtReadOnly.DataBindings.Add("Text", customer, nameof(Customer.CreatedDate),
    false, DataSourceUpdateMode.Never);
// Use case: Display-only fields
```

**Real-World Example - Live Calculation:**

```csharp
public partial class InvoiceForm : Form
{
    private Invoice invoice = new Invoice();

    private void SetupBindings()
    {
        // Immediate updates for live calculation
        txtQuantity.DataBindings.Add("Text", invoice, nameof(Invoice.Quantity),
            false, DataSourceUpdateMode.OnPropertyChanged);
        txtUnitPrice.DataBindings.Add("Text", invoice, nameof(Invoice.UnitPrice),
            false, DataSourceUpdateMode.OnPropertyChanged);

        // Total updates automatically when Quantity or UnitPrice changes
        txtTotal.DataBindings.Add("Text", invoice, nameof(Invoice.Total),
            false, DataSourceUpdateMode.Never); // Read-only
    }
}

public class Invoice : INotifyPropertyChanged
{
    private int _quantity;
    private decimal _unitPrice;

    public int Quantity
    {
        get => _quantity;
        set
        {
            _quantity = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Total)); // Trigger Total update
        }
    }

    public decimal UnitPrice
    {
        get => _unitPrice;
        set
        {
            _unitPrice = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Total));
        }
    }

    public decimal Total => Quantity * UnitPrice; // Calculated property

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

---

### INotifyPropertyChanged

**Why It's Important:**
- Enables **automatic UI updates** when data changes programmatically
- Required for two-way binding to work properly
- Standard pattern for observable objects in .NET

**Without INotifyPropertyChanged:**
```csharp
// ❌ UI doesn't update when data changes programmatically
public class Customer
{
    public string Name { get; set; }
}

// Somewhere in code:
customer.Name = "New Name"; // TextBox still shows old value!
```

**With INotifyPropertyChanged:**
```csharp
// ✅ UI automatically updates
public class Customer : INotifyPropertyChanged
{
    private string _name;

    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// Now this works:
customer.Name = "New Name"; // TextBox updates automatically!
```

**Complete Example Class:**

```csharp
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class Customer : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private string _email = string.Empty;
    private DateTime _birthDate;
    private bool _isActive;

    public int Id { get; set; }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public DateTime BirthDate
    {
        get => _birthDate;
        set => SetProperty(ref _birthDate, value);
    }

    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    // Calculated property
    public int Age
    {
        get
        {
            var today = DateTime.Today;
            var age = today.Year - BirthDate.Year;
            if (BirthDate.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

    // INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Helper method to reduce boilerplate
    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
            return false;

        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
```

**Base Class Pattern (Reusable):**

```csharp
public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
            return false;

        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

// Usage
public class Customer : ObservableObject
{
    private string _name = string.Empty;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
}
```

---

## 📊 Binding Scenarios

### Binding Object to Controls

**Simple Property Binding:**

```csharp
public partial class CustomerEditForm : Form
{
    private Customer customer;
    private BindingSource bindingSource;

    public CustomerEditForm(Customer customer)
    {
        InitializeComponent();
        this.customer = customer;
        SetupBindings();
    }

    private void SetupBindings()
    {
        bindingSource = new BindingSource { DataSource = customer };

        // TextBox - Text property
        txtName.DataBindings.Add("Text", bindingSource,
            nameof(Customer.Name), false, DataSourceUpdateMode.OnPropertyChanged);

        txtEmail.DataBindings.Add("Text", bindingSource,
            nameof(Customer.Email), false, DataSourceUpdateMode.OnValidation);

        // DateTimePicker - Value property
        dtpBirthDate.DataBindings.Add("Value", bindingSource,
            nameof(Customer.BirthDate), false, DataSourceUpdateMode.OnValidation);

        // CheckBox - Checked property
        chkIsActive.DataBindings.Add("Checked", bindingSource,
            nameof(Customer.IsActive), false, DataSourceUpdateMode.OnPropertyChanged);

        // NumericUpDown
        numAge.DataBindings.Add("Value", bindingSource,
            nameof(Customer.Age), false, DataSourceUpdateMode.Never); // Read-only

        // Label (read-only)
        lblCustomerId.DataBindings.Add("Text", bindingSource,
            nameof(Customer.Id), false, DataSourceUpdateMode.Never);
    }
}
```

**Two-Way Binding Example:**

```csharp
public partial class ProductForm : Form
{
    private Product product = new Product();

    private void SetupTwoWayBinding()
    {
        // UI → Data (user types in TextBox)
        txtProductName.DataBindings.Add("Text", product, nameof(Product.Name),
            false, DataSourceUpdateMode.OnPropertyChanged);

        // Data → UI (programmatic change)
        btnReset.Click += (s, e) =>
        {
            product.Name = "Default Product"; // TextBox updates automatically!
        };
    }
}
```

**Complete Working Example:**

```csharp
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

public partial class PersonEditorForm : Form
{
    private Person person = new Person();
    private BindingSource personBindingSource;

    public PersonEditorForm()
    {
        InitializeComponent();
        InitializeBindings();
    }

    private void InitializeBindings()
    {
        personBindingSource = new BindingSource { DataSource = person };

        // Bind all controls
        txtFirstName.DataBindings.Add("Text", personBindingSource,
            nameof(Person.FirstName), false, DataSourceUpdateMode.OnPropertyChanged);
        txtLastName.DataBindings.Add("Text", personBindingSource,
            nameof(Person.LastName), false, DataSourceUpdateMode.OnPropertyChanged);
        txtFullName.DataBindings.Add("Text", personBindingSource,
            nameof(Person.FullName), false, DataSourceUpdateMode.Never); // Calculated
        dtpBirthDate.DataBindings.Add("Value", personBindingSource,
            nameof(Person.BirthDate), false, DataSourceUpdateMode.OnValidation);
        chkIsSubscribed.DataBindings.Add("Checked", personBindingSource,
            nameof(Person.IsSubscribed), false, DataSourceUpdateMode.OnPropertyChanged);

        // Demo button
        btnRandomize.Click += (s, e) => RandomizePerson();
    }

    private void RandomizePerson()
    {
        var rand = new Random();
        person.FirstName = $"First{rand.Next(100)}";
        person.LastName = $"Last{rand.Next(100)}";
        person.BirthDate = DateTime.Today.AddYears(-rand.Next(20, 80));
        person.IsSubscribed = rand.Next(2) == 1;
        // UI updates automatically for all properties!
    }
}

public class Person : INotifyPropertyChanged
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private DateTime _birthDate = DateTime.Today;
    private bool _isSubscribed;

    public string FirstName
    {
        get => _firstName;
        set
        {
            if (_firstName != value)
            {
                _firstName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FullName)); // Update calculated property
            }
        }
    }

    public string LastName
    {
        get => _lastName;
        set
        {
            if (_lastName != value)
            {
                _lastName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FullName));
            }
        }
    }

    public string FullName => $"{FirstName} {LastName}";

    public DateTime BirthDate
    {
        get => _birthDate;
        set
        {
            if (_birthDate != value)
            {
                _birthDate = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsSubscribed
    {
        get => _isSubscribed;
        set
        {
            if (_isSubscribed != value)
            {
                _isSubscribed = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

---

### Binding Collections to DataGridView

**BindingList&lt;T&gt; vs List&lt;T&gt;:**

```csharp
// ❌ List<T> - NO automatic UI updates when adding/removing items
var customers = new List<Customer>();
dgvCustomers.DataSource = customers;
customers.Add(new Customer()); // DataGridView doesn't update!

// ✅ BindingList<T> - Automatic UI updates
var customers = new BindingList<Customer>();
dgvCustomers.DataSource = customers;
customers.Add(new Customer()); // DataGridView updates automatically!
```

**Auto-Generating vs Manual Columns:**

```csharp
public partial class CustomerGridForm : Form
{
    private BindingList<Customer> customers;
    private BindingSource bindingSource;

    private void SetupDataGridView_AutoColumns()
    {
        customers = new BindingList<Customer>(GetCustomers());
        bindingSource = new BindingSource { DataSource = customers };

        // Auto-generate columns
        dgvCustomers.DataSource = bindingSource;
        dgvCustomers.AutoGenerateColumns = true;

        // Customize after auto-generation
        dgvCustomers.Columns[nameof(Customer.Id)].Visible = false;
        dgvCustomers.Columns[nameof(Customer.Name)].Width = 200;
        dgvCustomers.Columns[nameof(Customer.Email)].Width = 250;
    }

    private void SetupDataGridView_ManualColumns()
    {
        customers = new BindingList<Customer>(GetCustomers());
        bindingSource = new BindingSource { DataSource = customers };

        dgvCustomers.AutoGenerateColumns = false;
        dgvCustomers.DataSource = bindingSource;

        // Define columns manually
        dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Customer.Name),
            HeaderText = "Customer Name",
            Width = 200
        });

        dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Customer.Email),
            HeaderText = "Email Address",
            Width = 250
        });

        dgvCustomers.Columns.Add(new DataGridViewCheckBoxColumn
        {
            DataPropertyName = nameof(Customer.IsActive),
            HeaderText = "Active",
            Width = 60
        });

        // Calculated column
        dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Customer.Age),
            HeaderText = "Age",
            Width = 60,
            ReadOnly = true
        });
    }
}
```

**Formatting Cells:**

```csharp
private void SetupFormattedColumns()
{
    // Currency formatting
    var priceColumn = new DataGridViewTextBoxColumn
    {
        DataPropertyName = nameof(Product.Price),
        HeaderText = "Price",
        DefaultCellStyle = new DataGridViewCellStyle
        {
            Format = "C2", // Currency with 2 decimals
            Alignment = DataGridViewContentAlignment.MiddleRight
        }
    };
    dgvProducts.Columns.Add(priceColumn);

    // Date formatting
    var dateColumn = new DataGridViewTextBoxColumn
    {
        DataPropertyName = nameof(Product.CreatedDate),
        HeaderText = "Created",
        DefaultCellStyle = new DataGridViewCellStyle
        {
            Format = "yyyy-MM-dd",
            Alignment = DataGridViewContentAlignment.MiddleCenter
        }
    };
    dgvProducts.Columns.Add(dateColumn);

    // Percentage formatting
    var discountColumn = new DataGridViewTextBoxColumn
    {
        DataPropertyName = nameof(Product.Discount),
        HeaderText = "Discount",
        DefaultCellStyle = new DataGridViewCellStyle
        {
            Format = "P1", // Percentage with 1 decimal
            Alignment = DataGridViewContentAlignment.MiddleRight
        }
    };
    dgvProducts.Columns.Add(discountColumn);
}
```

**Complete DataGridView Example:**

```csharp
public partial class ProductGridForm : Form
{
    private BindingList<Product> products;
    private BindingSource productBindingSource;

    public ProductGridForm()
    {
        InitializeComponent();
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        products = new BindingList<Product>(LoadProducts());
        productBindingSource = new BindingSource { DataSource = products };

        dgvProducts.AutoGenerateColumns = false;
        dgvProducts.DataSource = productBindingSource;

        // Configure columns
        dgvProducts.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Product.Name),
                HeaderText = "Product Name",
                Width = 200
            },
            new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Product.Price),
                HeaderText = "Price",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            },
            new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Product.Stock),
                HeaderText = "Stock",
                Width = 80
            },
            new DataGridViewCheckBoxColumn
            {
                DataPropertyName = nameof(Product.IsAvailable),
                HeaderText = "Available",
                Width = 80
            }
        });

        // Add/Remove buttons
        btnAdd.Click += (s, e) => AddProduct();
        btnRemove.Click += (s, e) => RemoveProduct();

        // Cell formatting event
        dgvProducts.CellFormatting += DgvProducts_CellFormatting;
    }

    private void DgvProducts_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (dgvProducts.Columns[e.ColumnIndex].DataPropertyName == nameof(Product.Stock))
        {
            if (e.Value is int stock && stock < 10)
            {
                e.CellStyle.BackColor = Color.LightCoral;
                e.CellStyle.ForeColor = Color.White;
            }
        }
    }

    private void AddProduct()
    {
        var newProduct = new Product
        {
            Id = products.Count + 1,
            Name = $"Product {products.Count + 1}",
            Price = 99.99m,
            Stock = 100,
            IsAvailable = true
        };
        products.Add(newProduct); // Grid updates automatically!
    }

    private void RemoveProduct()
    {
        if (productBindingSource.Current is Product product)
        {
            products.Remove(product); // Grid updates automatically!
        }
    }

    private List<Product> LoadProducts()
    {
        return new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Price = 999.99m, Stock = 15, IsAvailable = true },
            new Product { Id = 2, Name = "Mouse", Price = 29.99m, Stock = 5, IsAvailable = true },
            new Product { Id = 3, Name = "Keyboard", Price = 79.99m, Stock = 8, IsAvailable = false }
        };
    }
}

public class Product : INotifyPropertyChanged
{
    private int _id;
    private string _name = string.Empty;
    private decimal _price;
    private int _stock;
    private bool _isAvailable;

    public int Id
    {
        get => _id;
        set { _id = value; OnPropertyChanged(); }
    }

    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }

    public decimal Price
    {
        get => _price;
        set { _price = value; OnPropertyChanged(); }
    }

    public int Stock
    {
        get => _stock;
        set { _stock = value; OnPropertyChanged(); }
    }

    public bool IsAvailable
    {
        get => _isAvailable;
        set { _isAvailable = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

---

### Binding to ComboBox/ListBox

**DisplayMember and ValueMember:**

```csharp
public partial class OrderForm : Form
{
    private void SetupComboBox()
    {
        var customers = GetCustomers();

        cbxCustomer.DataSource = customers;
        cbxCustomer.DisplayMember = nameof(Customer.Name); // What user sees
        cbxCustomer.ValueMember = nameof(Customer.Id);     // Underlying value

        // Get selected values
        cbxCustomer.SelectedIndexChanged += (s, e) =>
        {
            if (cbxCustomer.SelectedItem is Customer customer)
            {
                var id = (int)cbxCustomer.SelectedValue;  // Customer.Id
                var name = customer.Name;                  // Customer.Name
                MessageBox.Show($"Selected: {name} (ID: {id})");
            }
        };
    }
}
```

**Binding ComboBox to Object Property:**

```csharp
public partial class OrderEditForm : Form
{
    private Order order = new Order();
    private List<Customer> customers;

    private void SetupOrderBinding()
    {
        customers = GetCustomers();

        // Bind ComboBox to list
        cbxCustomer.DataSource = customers;
        cbxCustomer.DisplayMember = nameof(Customer.Name);
        cbxCustomer.ValueMember = nameof(Customer.Id);

        // Bind ComboBox selection to Order.CustomerId
        cbxCustomer.DataBindings.Add("SelectedValue", order,
            nameof(Order.CustomerId), false, DataSourceUpdateMode.OnPropertyChanged);

        // When order.CustomerId changes, ComboBox selection updates!
        btnSetCustomer.Click += (s, e) =>
        {
            order.CustomerId = 2; // ComboBox automatically selects customer with ID 2
        };
    }
}
```

**Multi-Select ListBox:**

```csharp
public partial class RoleAssignmentForm : Form
{
    private User user = new User();
    private List<Role> allRoles;

    private void SetupMultiSelectListBox()
    {
        allRoles = GetAllRoles();

        lstRoles.DataSource = allRoles;
        lstRoles.DisplayMember = nameof(Role.Name);
        lstRoles.SelectionMode = SelectionMode.MultiExtended;

        // Pre-select user's roles
        LoadUserRoles();

        // Save selected roles
        btnSave.Click += (s, e) => SaveUserRoles();
    }

    private void LoadUserRoles()
    {
        var userRoleIds = user.RoleIds; // List<int>

        for (int i = 0; i < lstRoles.Items.Count; i++)
        {
            if (lstRoles.Items[i] is Role role && userRoleIds.Contains(role.Id))
            {
                lstRoles.SetSelected(i, true);
            }
        }
    }

    private void SaveUserRoles()
    {
        user.RoleIds = lstRoles.SelectedItems
            .Cast<Role>()
            .Select(r => r.Id)
            .ToList();
    }
}
```

---

### Master-Detail Binding

**Related Data Binding:**

```csharp
public partial class MasterDetailForm : Form
{
    private BindingSource customerBindingSource;
    private BindingSource ordersBindingSource;

    private void SetupMasterDetail()
    {
        var customers = GetCustomersWithOrders();

        // Master binding (Customers)
        customerBindingSource = new BindingSource { DataSource = customers };
        dgvCustomers.DataSource = customerBindingSource;

        // Detail binding (Orders for selected customer)
        ordersBindingSource = new BindingSource
        {
            DataSource = customerBindingSource,
            DataMember = nameof(Customer.Orders) // Navigation property
        };
        dgvOrders.DataSource = ordersBindingSource;

        // When customer selection changes, orders grid updates automatically!
    }
}

public class Customer : INotifyPropertyChanged
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Order> Orders { get; set; } = new List<Order>();

    // INotifyPropertyChanged implementation...
}

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Amount { get; set; }
}
```

**Using DataRelation (for DataSet/DataTable):**

```csharp
private void SetupDataRelation()
{
    var dataSet = new DataSet();

    // Create tables
    var customersTable = CreateCustomersTable();
    var ordersTable = CreateOrdersTable();

    dataSet.Tables.Add(customersTable);
    dataSet.Tables.Add(ordersTable);

    // Create relation
    var relation = new DataRelation("CustomerOrders",
        customersTable.Columns["Id"],
        ordersTable.Columns["CustomerId"]);
    dataSet.Relations.Add(relation);

    // Master binding
    customerBindingSource = new BindingSource { DataSource = dataSet, DataMember = "Customers" };
    dgvCustomers.DataSource = customerBindingSource;

    // Detail binding
    ordersBindingSource = new BindingSource
    {
        DataSource = customerBindingSource,
        DataMember = "CustomerOrders"
    };
    dgvOrders.DataSource = ordersBindingSource;
}
```

---

### Complex Property Binding

**Binding to Nested Properties:**

```csharp
public class Order
{
    public int Id { get; set; }
    public Customer Customer { get; set; } = new Customer();
    public decimal Amount { get; set; }
}

public partial class OrderForm : Form
{
    private Order order = new Order();

    private void SetupNestedBinding()
    {
        // Bind to nested property using dot notation
        txtCustomerName.DataBindings.Add("Text", order,
            "Customer.Name", false, DataSourceUpdateMode.OnValidation);

        txtCustomerEmail.DataBindings.Add("Text", order,
            "Customer.Email", false, DataSourceUpdateMode.OnValidation);
    }
}
```

**Custom Formatting with Format Event:**

```csharp
private void SetupCustomFormatting()
{
    var binding = new Binding("Text", product, nameof(Product.Price));

    // Format data → UI (custom display)
    binding.Format += (s, e) =>
    {
        if (e.Value is decimal price)
        {
            e.Value = $"${price:F2} USD";
        }
    };

    // Parse UI → data (custom parsing)
    binding.Parse += (s, e) =>
    {
        if (e.Value is string str)
        {
            // Remove $ and USD, parse decimal
            var cleaned = str.Replace("$", "").Replace("USD", "").Trim();
            if (decimal.TryParse(cleaned, out var price))
            {
                e.Value = price;
            }
        }
    };

    txtPrice.DataBindings.Add(binding);
}
```

**Custom Type Converter:**

```csharp
[TypeConverter(typeof(PhoneNumberConverter))]
public class PhoneNumber
{
    public string AreaCode { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;

    public override string ToString() => $"({AreaCode}) {Number}";
}

public class PhoneNumberConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context,
        CultureInfo? culture, object value)
    {
        if (value is string str)
        {
            // Parse "(123) 456-7890" to PhoneNumber
            var cleaned = new string(str.Where(char.IsDigit).ToArray());
            if (cleaned.Length == 10)
            {
                return new PhoneNumber
                {
                    AreaCode = cleaned.Substring(0, 3),
                    Number = cleaned.Substring(3)
                };
            }
        }
        return base.ConvertFrom(context, culture, value);
    }
}

// Usage
txtPhone.DataBindings.Add("Text", contact, nameof(Contact.PhoneNumber));
```

---

## 🚀 Advanced Binding

### CurrencyManager

**Managing Current Position:**

```csharp
public partial class NavigationForm : Form
{
    private BindingSource bindingSource;
    private CurrencyManager? currencyManager;

    private void SetupCurrencyManager()
    {
        bindingSource = new BindingSource { DataSource = GetCustomers() };

        // Get CurrencyManager
        currencyManager = (CurrencyManager)this.BindingContext[bindingSource];

        // Position changed event
        currencyManager.PositionChanged += CurrencyManager_PositionChanged;

        // Navigation
        btnFirst.Click += (s, e) => currencyManager.Position = 0;
        btnPrevious.Click += (s, e) => currencyManager.Position--;
        btnNext.Click += (s, e) => currencyManager.Position++;
        btnLast.Click += (s, e) => currencyManager.Position = currencyManager.Count - 1;

        // Update navigation button states
        UpdateNavigationButtons();
    }

    private void CurrencyManager_PositionChanged(object? sender, EventArgs e)
    {
        UpdateNavigationButtons();
        lblPosition.Text = $"Record {currencyManager.Position + 1} of {currencyManager.Count}";
    }

    private void UpdateNavigationButtons()
    {
        if (currencyManager != null)
        {
            btnFirst.Enabled = btnPrevious.Enabled = currencyManager.Position > 0;
            btnNext.Enabled = btnLast.Enabled =
                currencyManager.Position < currencyManager.Count - 1;
        }
    }
}
```

---

### Binding with Validation

**Using ErrorProvider with Binding:**

```csharp
public partial class ValidatedForm : Form
{
    private Customer customer = new Customer();
    private ErrorProvider errorProvider = new ErrorProvider();

    private void SetupValidation()
    {
        txtEmail.DataBindings.Add("Text", customer, nameof(Customer.Email),
            false, DataSourceUpdateMode.OnValidation);

        txtEmail.Validating += TxtEmail_Validating;
    }

    private void TxtEmail_Validating(object? sender, CancelEventArgs e)
    {
        var email = txtEmail.Text;

        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
        {
            errorProvider.SetError(txtEmail, "Please enter a valid email address");
            e.Cancel = true; // Prevent focus change
        }
        else
        {
            errorProvider.SetError(txtEmail, string.Empty);
        }
    }
}
```

**IDataErrorInfo Interface:**

```csharp
public class ValidatedCustomer : INotifyPropertyChanged, IDataErrorInfo
{
    private string _name = string.Empty;
    private string _email = string.Empty;
    private int _age;

    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }

    public string Email
    {
        get => _email;
        set { _email = value; OnPropertyChanged(); }
    }

    public int Age
    {
        get => _age;
        set { _age = value; OnPropertyChanged(); }
    }

    // IDataErrorInfo implementation
    public string Error => string.Empty; // Not used in WinForms

    public string this[string columnName]
    {
        get
        {
            switch (columnName)
            {
                case nameof(Name):
                    if (string.IsNullOrWhiteSpace(Name))
                        return "Name is required";
                    if (Name.Length < 2)
                        return "Name must be at least 2 characters";
                    break;

                case nameof(Email):
                    if (string.IsNullOrWhiteSpace(Email))
                        return "Email is required";
                    if (!Email.Contains("@"))
                        return "Invalid email format";
                    break;

                case nameof(Age):
                    if (Age < 0 || Age > 150)
                        return "Age must be between 0 and 150";
                    break;
            }
            return string.Empty;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// Usage in Form
private void SetupIDataErrorInfo()
{
    txtName.Validating += (s, e) =>
    {
        var error = customer[nameof(ValidatedCustomer.Name)];
        if (!string.IsNullOrEmpty(error))
        {
            errorProvider.SetError(txtName, error);
            e.Cancel = true;
        }
        else
        {
            errorProvider.SetError(txtName, string.Empty);
        }
    };
}
```

---

## ✅ Best Practices

### DOs

**✅ Use BindingSource as Intermediary**
```csharp
// ✅ GOOD - BindingSource provides flexibility
bindingSource = new BindingSource { DataSource = customers };
dgvCustomers.DataSource = bindingSource;
txtName.DataBindings.Add("Text", bindingSource, "Name");

// ❌ BAD - Direct binding limits functionality
dgvCustomers.DataSource = customers;
```

**✅ Use BindingList&lt;T&gt; for Collections**
```csharp
// ✅ GOOD - Automatic UI updates
var customers = new BindingList<Customer>(GetCustomers());

// ❌ BAD - Manual refresh needed
var customers = new List<Customer>(GetCustomers());
```

**✅ Implement INotifyPropertyChanged**
```csharp
// ✅ GOOD - Bidirectional binding
public class Customer : INotifyPropertyChanged
{
    private string _name;
    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }
}

// ❌ BAD - Only UI→Data works
public class Customer
{
    public string Name { get; set; }
}
```

**✅ Use nameof for Property Names**
```csharp
// ✅ GOOD - Refactor-safe
txtName.DataBindings.Add("Text", customer, nameof(Customer.Name));

// ❌ BAD - Breaks on rename
txtName.DataBindings.Add("Text", customer, "Name");
```

**✅ Choose Appropriate UpdateMode**
```csharp
// ✅ Real-time validation
txtSearch.DataBindings.Add("Text", filter, "SearchText",
    false, DataSourceUpdateMode.OnPropertyChanged);

// ✅ Standard form fields
txtEmail.DataBindings.Add("Text", customer, "Email",
    false, DataSourceUpdateMode.OnValidation);
```

### DON'Ts

**❌ Don't Forget to Dispose BindingSource**
```csharp
// ❌ BAD - Memory leak
public partial class MyForm : Form
{
    private BindingSource bindingSource = new BindingSource();
}

// ✅ GOOD - Proper disposal
public partial class MyForm : Form
{
    private BindingSource bindingSource = new BindingSource();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            bindingSource?.Dispose();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
```

**❌ Don't Bind to Properties Without Getters**
```csharp
// ❌ BAD - Write-only property
public string Name { set => _name = value; }

// ✅ GOOD - Full property
public string Name
{
    get => _name;
    set => SetProperty(ref _name, value);
}
```

**❌ Don't Modify Bound Collections Directly (Use BindingList)**
```csharp
// ❌ BAD
List<Customer> customers = (List<Customer>)bindingSource.DataSource;
customers.Add(newCustomer); // UI doesn't update!

// ✅ GOOD
bindingSource.Add(newCustomer);
// OR
((BindingList<Customer>)bindingSource.DataSource).Add(newCustomer);
```

---

## ⚡ Performance Considerations

### Suspending Binding During Bulk Updates

```csharp
private void BulkUpdateCustomers()
{
    // Suspend binding
    bindingSource.RaiseListChangedEvents = false;

    try
    {
        // Perform bulk updates
        for (int i = 0; i < 1000; i++)
        {
            customers.Add(new Customer { Name = $"Customer {i}" });
        }
    }
    finally
    {
        // Resume binding
        bindingSource.RaiseListChangedEvents = true;
        bindingSource.ResetBindings(false); // Refresh UI once
    }
}
```

### Virtual Mode for Large Data Sets

```csharp
private void SetupVirtualMode()
{
    dgvCustomers.VirtualMode = true;
    dgvCustomers.RowCount = 1000000; // Large dataset

    dgvCustomers.CellValueNeeded += (s, e) =>
    {
        // Load data on-demand
        e.Value = GetCellValue(e.RowIndex, e.ColumnIndex);
    };
}
```

---

## 🔗 Related Topics

- **[Form Communication](form-communication.md)** - Passing data between forms
- **[Input Validation](input-validation.md)** - Validating bound data
- **[DataGridView Best Practices](datagridview-practices.md)** - Advanced grid scenarios
- **[MVP Pattern](../architecture/mvp-pattern.md)** - Using binding with MVP
- **[MVVM Pattern](../architecture/mvvm-pattern.md)** - MVVM with WinForms

---

**Last Updated**: 2025-11-07
**Related Standards**: [Code Style](../conventions/code-style.md) | [Best Practices](../best-practices/)
