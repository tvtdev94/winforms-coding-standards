---
description: Setup data binding for WinForms controls
---

You are tasked with setting up data binding for WinForms controls.

## Workflow

1. **Ask the user**:
   - What data source to bind? (object, list, database)
   - Which controls need binding?
   - One-way or two-way binding?

2. **Read the form file** to understand current structure

3. **Implement data binding** based on scenario:

### Scenario 1: Binding Object to Controls

```csharp
public class Customer
{
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }
    public bool IsActive { get; set; }
}

// In form
private Customer _customer;
private BindingSource _bindingSource;

public MyForm()
{
    InitializeComponent();
    _bindingSource = new BindingSource();
}

private void LoadCustomer(Customer customer)
{
    _customer = customer;
    _bindingSource.DataSource = _customer;

    // Bind controls
    txtName.DataBindings.Clear();
    txtName.DataBindings.Add("Text", _bindingSource, nameof(Customer.Name),
        false, DataSourceUpdateMode.OnPropertyChanged);

    txtEmail.DataBindings.Clear();
    txtEmail.DataBindings.Add("Text", _bindingSource, nameof(Customer.Email),
        false, DataSourceUpdateMode.OnPropertyChanged);

    dtpBirthDate.DataBindings.Clear();
    dtpBirthDate.DataBindings.Add("Value", _bindingSource, nameof(Customer.BirthDate),
        false, DataSourceUpdateMode.OnPropertyChanged);

    chkActive.DataBindings.Clear();
    chkActive.DataBindings.Add("Checked", _bindingSource, nameof(Customer.IsActive),
        false, DataSourceUpdateMode.OnPropertyChanged);
}
```

### Scenario 2: Binding List to DataGridView

```csharp
private BindingSource _customersBindingSource;
private List<Customer> _customers;

public MyForm()
{
    InitializeComponent();
    _customersBindingSource = new BindingSource();
    dgvCustomers.DataSource = _customersBindingSource;
}

private void LoadCustomers(List<Customer> customers)
{
    _customers = customers;
    _customersBindingSource.DataSource = new BindingList<Customer>(_customers);

    // Configure columns
    dgvCustomers.AutoGenerateColumns = false;
    dgvCustomers.Columns.Clear();

    dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
    {
        DataPropertyName = nameof(Customer.Name),
        HeaderText = "Name",
        Name = "colName",
        Width = 150
    });

    dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
    {
        DataPropertyName = nameof(Customer.Email),
        HeaderText = "Email",
        Name = "colEmail",
        Width = 200
    });

    dgvCustomers.Columns.Add(new DataGridViewCheckBoxColumn
    {
        DataPropertyName = nameof(Customer.IsActive),
        HeaderText = "Active",
        Name = "colActive",
        Width = 60
    });
}
```

### Scenario 3: Binding List to ComboBox/ListBox

```csharp
private BindingSource _categoriesBindingSource;

public MyForm()
{
    InitializeComponent();
    _categoriesBindingSource = new BindingSource();
    cbxCategory.DataSource = _categoriesBindingSource;
}

private void LoadCategories(List<Category> categories)
{
    _categoriesBindingSource.DataSource = categories;
    cbxCategory.DisplayMember = nameof(Category.Name);
    cbxCategory.ValueMember = nameof(Category.Id);
}

// Get selected value
private int GetSelectedCategoryId()
{
    return cbxCategory.SelectedValue != null
        ? (int)cbxCategory.SelectedValue
        : 0;
}
```

### Scenario 4: Master-Detail Binding

```csharp
private BindingSource _ordersBindingSource;
private BindingSource _orderDetailsBindingSource;

public MyForm()
{
    InitializeComponent();

    _ordersBindingSource = new BindingSource();
    _orderDetailsBindingSource = new BindingSource();

    dgvOrders.DataSource = _ordersBindingSource;
    dgvOrderDetails.DataSource = _orderDetailsBindingSource;
}

private void LoadOrders(List<Order> orders)
{
    _ordersBindingSource.DataSource = new BindingList<Order>(orders);

    // Setup master-detail relationship
    _orderDetailsBindingSource.DataSource = _ordersBindingSource;
    _orderDetailsBindingSource.DataMember = nameof(Order.OrderDetails);
}
```

### Scenario 5: Two-Way Binding with INotifyPropertyChanged

```csharp
public class Customer : INotifyPropertyChanged
{
    private string _name;
    private string _email;

    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            if (_email != value)
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

4. **Best Practices**:
   - ✅ Use BindingSource for better control
   - ✅ Use DataSourceUpdateMode.OnPropertyChanged for immediate updates
   - ✅ Clear existing bindings before adding new ones
   - ✅ Use BindingList<T> for list binding with notifications
   - ✅ Implement INotifyPropertyChanged for two-way binding
   - ✅ Set AutoGenerateColumns = false for DataGridView
   - ✅ Use DisplayMember/ValueMember for ComboBox
   - ✅ Dispose BindingSource in form disposal

5. **Common Pitfalls to Avoid**:
   - ❌ Not clearing existing bindings
   - ❌ Binding directly to List<T> instead of BindingList<T>
   - ❌ Not disposing BindingSource
   - ❌ Using wrong DataSourceUpdateMode
   - ❌ Forgetting to implement INotifyPropertyChanged

6. **Show the user**:
   - Updated form code with data binding
   - Explanation of binding strategy used
   - Offer to add more bindings or modify existing ones
