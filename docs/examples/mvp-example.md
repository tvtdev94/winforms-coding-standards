# Complete MVP Pattern Example

> **Working Code**: Full implementation of MVP pattern for a Customer form.

See [MVP Pattern Guide](../architecture/mvp-pattern.md) for detailed explanation.

---

## 📁 Project Structure

```
/CustomerManagement
    ├── /Models
    │   └── Customer.cs
    ├── /Services
    │   ├── ICustomerService.cs
    │   └── CustomerService.cs
    ├── /Views
    │   ├── ICustomerView.cs
    │   └── CustomerForm.cs
    ├── /Presenters
    │   └── CustomerPresenter.cs
    └── Program.cs
```

---

## 1. Model

```csharp
// Customer.cs
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}
```

---

## 2. View Interface

```csharp
// ICustomerView.cs
public interface ICustomerView
{
    // Data properties
    int CustomerId { get; set; }
    string CustomerName { get; set; }
    string Email { get; set; }
    string Phone { get; set; }
    bool IsActive { get; set; }

    // UI state
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
```

---

## 3. View Implementation (Form)

```csharp
// CustomerForm.cs
public partial class CustomerForm : Form, ICustomerView
{
    private readonly CustomerPresenter _presenter;

    public CustomerForm(CustomerPresenter presenter)
    {
        InitializeComponent();
        _presenter = presenter;
        _presenter.AttachView(this);

        // Wire events
        Load += (s, e) => LoadRequested?.Invoke(this, EventArgs.Empty);
        btnSave.Click += (s, e) => SaveRequested?.Invoke(this, EventArgs.Empty);
        btnCancel.Click += (s, e) => CancelRequested?.Invoke(this, EventArgs.Empty);
    }

    // ICustomerView implementation
    public int CustomerId { get; set; }

    public string CustomerName
    {
        get => txtName.Text;
        set => txtName.Text = value;
    }

    public string Email
    {
        get => txtEmail.Text;
        set => txtEmail.Text = value;
    }

    public string Phone
    {
        get => txtPhone.Text;
        set => txtPhone.Text = value;
    }

    public bool IsActive
    {
        get => chkActive.Checked;
        set => chkActive.Checked = value;
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

    public event EventHandler LoadRequested;
    public event EventHandler SaveRequested;
    public event EventHandler CancelRequested;

    public void ShowError(string message)
    {
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public void ShowSuccess(string message)
    {
        MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

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
```

---

## 4. Presenter

```csharp
// CustomerPresenter.cs
public class CustomerPresenter
{
    private ICustomerView? _view;
    private readonly ICustomerService _service;
    private readonly int? _customerId;

    public CustomerPresenter(ICustomerService service, int? customerId = null)
    {
        _service = service;
        _customerId = customerId;
    }

    public void AttachView(ICustomerView view)
    {
        _view = view;
        _view.LoadRequested += OnLoadRequested;
        _view.SaveRequested += OnSaveRequested;
        _view.CancelRequested += OnCancelRequested;
    }

    public void DetachView()
    {
        if (_view != null)
        {
            _view.LoadRequested -= OnLoadRequested;
            _view.SaveRequested -= OnSaveRequested;
            _view.CancelRequested -= OnCancelRequested;
            _view = null;
        }
    }

    private async void OnLoadRequested(object? sender, EventArgs e)
    {
        if (_view == null) return;

        _view.IsLoadingVisible = true;
        _view.IsSaveButtonEnabled = false;

        try
        {
            if (_customerId.HasValue)
            {
                var customer = await _service.GetByIdAsync(_customerId.Value);
                if (customer != null)
                {
                    _view.CustomerId = customer.Id;
                    _view.CustomerName = customer.Name;
                    _view.Email = customer.Email;
                    _view.Phone = customer.Phone;
                    _view.IsActive = customer.IsActive;
                }
            }
        }
        catch (Exception ex)
        {
            _view.ShowError($"Error loading: {ex.Message}");
        }
        finally
        {
            _view.IsLoadingVisible = false;
            _view.IsSaveButtonEnabled = true;
        }
    }

    private async void OnSaveRequested(object? sender, EventArgs e)
    {
        if (_view == null) return;

        // Validation
        if (string.IsNullOrWhiteSpace(_view.CustomerName))
        {
            _view.ShowError("Name is required");
            return;
        }

        _view.IsLoadingVisible = true;
        _view.IsSaveButtonEnabled = false;

        try
        {
            var customer = new Customer
            {
                Id = _view.CustomerId,
                Name = _view.CustomerName,
                Email = _view.Email,
                Phone = _view.Phone,
                IsActive = _view.IsActive
            };

            await _service.SaveAsync(customer);
            _view.ShowSuccess("Customer saved successfully");
            _view.Close();
        }
        catch (Exception ex)
        {
            _view.ShowError($"Error saving: {ex.Message}");
        }
        finally
        {
            _view.IsLoadingVisible = false;
            _view.IsSaveButtonEnabled = true;
        }
    }

    private void OnCancelRequested(object? sender, EventArgs e)
    {
        _view?.Close();
    }
}
```

---

## 5. Service

```csharp
// ICustomerService.cs
public interface ICustomerService
{
    Task<Customer?> GetByIdAsync(int id);
    Task<List<Customer>> GetAllAsync();
    Task SaveAsync(Customer customer);
    Task DeleteAsync(int id);
}

// CustomerService.cs
public class CustomerService : ICustomerService
{
    private readonly List<Customer> _customers = new();

    public Task<Customer?> GetByIdAsync(int id)
    {
        return Task.FromResult(_customers.FirstOrDefault(c => c.Id == id));
    }

    public Task<List<Customer>> GetAllAsync()
    {
        return Task.FromResult(_customers.ToList());
    }

    public Task SaveAsync(Customer customer)
    {
        if (customer.Id == 0)
        {
            customer.Id = _customers.Any() ? _customers.Max(c => c.Id) + 1 : 1;
            _customers.Add(customer);
        }
        else
        {
            var existing = _customers.FirstOrDefault(c => c.Id == customer.Id);
            if (existing != null)
            {
                existing.Name = customer.Name;
                existing.Email = customer.Email;
                existing.Phone = customer.Phone;
                existing.IsActive = customer.IsActive;
            }
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _customers.RemoveAll(c => c.Id == id);
        return Task.CompletedTask;
    }
}
```

---

## 6. DI Setup

```csharp
// Program.cs
services.AddSingleton<ICustomerService, CustomerService>();
services.AddTransient<CustomerPresenter>();
services.AddTransient<CustomerForm>();
```

---

## ✅ Benefits of This Pattern

✅ **Testable** - Presenter can be unit tested
✅ **Decoupled** - View and business logic separated
✅ **Maintainable** - Easy to modify without breaking tests
✅ **SOLID** - Follows Single Responsibility Principle

---

**Last Updated**: 2025-11-07
