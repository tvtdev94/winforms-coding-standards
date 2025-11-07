# MVVM (Model-View-ViewModel) Pattern for WinForms

> **Quick Reference**: MVVM pattern for WinForms applications using .NET 8+ with improved data binding support.

⚡ **NEW**: Available for WinForms in .NET 7+, improved in .NET 8

---

## 📖 What is MVVM?

**MVVM (Model-View-ViewModel)** separates:
- **Model**: Data entities and business logic
- **View**: UI (Forms) - binds to ViewModel
- **ViewModel**: Presentation logic + data for View

### Why MVVM?
✅ **Two-way data binding** - View updates ViewModel automatically
✅ **No code-behind** - Logic in testable ViewModel
✅ **INotifyPropertyChanged** - Automatic UI updates
✅ **.NET 8 support** - Improved data binding for WinForms

---

## 🏗️ MVVM Architecture

```
┌────────┐         ┌───────────┐         ┌────────┐
│  View  │ ◄─────► │ ViewModel │ ◄─────► │  Model │
│ (Form) │ Binding │           │         │Service │
└────────┘         └───────────┘         └────────┘
    │                    │                     │
 Data Binding      INotifyProperty       Business Logic
                   Changed
```

---

## 💻 Complete MVVM Example

### Step 1: Create ViewModel Base Class

```csharp
// ViewModelBase.cs
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
```

### Step 2: Create ViewModel

```csharp
// CustomerViewModel.cs
public class CustomerViewModel : ViewModelBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomerViewModel> _logger;
    private int _customerId;
    private string _name = string.Empty;
    private string _email = string.Empty;
    private string _phone = string.Empty;
    private bool _isActive = true;
    private bool _isLoading;
    private string _errorMessage = string.Empty;

    public CustomerViewModel(ICustomerService customerService, ILogger<CustomerViewModel> logger)
    {
        _customerService = customerService;
        _logger = logger;

        // Commands
        SaveCommand = new RelayCommand(async () => await SaveAsync(), CanSave);
        CancelCommand = new RelayCommand(Cancel);
    }

    // Properties with INotifyPropertyChanged
    public int CustomerId
    {
        get => _customerId;
        set => SetProperty(ref _customerId, value);
    }

    public string Name
    {
        get => _name;
        set
        {
            if (SetProperty(ref _name, value))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            if (SetProperty(ref _email, value))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string Phone
    {
        get => _phone;
        set => SetProperty(ref _phone, value);
    }

    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    // Commands
    public RelayCommand SaveCommand { get; }
    public RelayCommand CancelCommand { get; }

    // Methods
    public async Task LoadAsync(int? customerId = null)
    {
        if (!customerId.HasValue) return;

        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var customer = await _customerService.GetByIdAsync(customerId.Value);

            if (customer != null)
            {
                CustomerId = customer.Id;
                Name = customer.Name;
                Email = customer.Email;
                Phone = customer.Phone;
                IsActive = customer.IsActive;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading customer");
            ErrorMessage = "Failed to load customer";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanSave()
    {
        return !string.IsNullOrWhiteSpace(Name) &&
               !string.IsNullOrWhiteSpace(Email) &&
               !IsLoading;
    }

    private async Task SaveAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var customer = new Customer
            {
                Id = CustomerId,
                Name = Name,
                Email = Email,
                Phone = Phone,
                IsActive = IsActive
            };

            var success = await _customerService.SaveAsync(customer);

            if (success)
            {
                _logger.LogInformation("Customer saved: {Name}", Name);
                // Raise event or close view
            }
            else
            {
                ErrorMessage = "Failed to save customer";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving customer");
            ErrorMessage = "An error occurred while saving";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void Cancel()
    {
        // Close view logic
    }
}

// RelayCommand implementation
public class RelayCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool>? _canExecute;

    public RelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

    public async void Execute(object? parameter) => await _execute();

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
```

### Step 3: Create View with Data Binding

```csharp
// CustomerForm.cs
public partial class CustomerForm : Form
{
    private readonly CustomerViewModel _viewModel;

    public CustomerForm(CustomerViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;

        // Setup data bindings
        SetupBindings();
    }

    private void SetupBindings()
    {
        // Bind TextBoxes to ViewModel properties
        txtName.DataBindings.Add(nameof(TextBox.Text), _viewModel, nameof(CustomerViewModel.Name),
            false, DataSourceUpdateMode.OnPropertyChanged);

        txtEmail.DataBindings.Add(nameof(TextBox.Text), _viewModel, nameof(CustomerViewModel.Email),
            false, DataSourceUpdateMode.OnPropertyChanged);

        txtPhone.DataBindings.Add(nameof(TextBox.Text), _viewModel, nameof(CustomerViewModel.Phone),
            false, DataSourceUpdateMode.OnPropertyChanged);

        chkActive.DataBindings.Add(nameof(CheckBox.Checked), _viewModel, nameof(CustomerViewModel.IsActive),
            false, DataSourceUpdateMode.OnPropertyChanged);

        // Bind button enabled state
        btnSave.DataBindings.Add(nameof(Button.Enabled), _viewModel.SaveCommand, nameof(RelayCommand.CanExecute),
            false, DataSourceUpdateMode.Never);

        // Bind loading indicator
        lblLoading.DataBindings.Add(nameof(Label.Visible), _viewModel, nameof(CustomerViewModel.IsLoading),
            false, DataSourceUpdateMode.Never);

        // Bind error message
        lblError.DataBindings.Add(nameof(Label.Text), _viewModel, nameof(CustomerViewModel.ErrorMessage),
            false, DataSourceUpdateMode.Never);

        // Wire command to button click
        btnSave.Click += (s, e) => _viewModel.SaveCommand.Execute(null);
        btnCancel.Click += (s, e) => _viewModel.CancelCommand.Execute(null);
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        // Load data if edit mode
        // await _viewModel.LoadAsync(customerId);
    }
}
```

---

## ✅ MVVM Best Practices

### DO:
✅ Implement INotifyPropertyChanged in ViewModels
✅ Use Commands for user actions
✅ Keep View code-behind minimal
✅ Test ViewModels (no UI dependencies)
✅ Use DataSourceUpdateMode.OnPropertyChanged for real-time updates

### DON'T:
❌ Don't reference View in ViewModel
❌ Don't put UI logic in ViewModel
❌ Don't use MessageBox in ViewModel (use events/dialogs)
❌ Don't forget to dispose bindings

---

## 🆚 MVVM vs MVP

| Feature | MVVM | MVP |
|---------|------|-----|
| **Binding** | Two-way automatic | Manual |
| **View Knowledge** | View knows ViewModel | View knows Presenter |
| **Testability** | Excellent | Excellent |
| **WinForms Support** | .NET 8+ | All versions |
| **Complexity** | Medium-High | Medium |
| **Best For** | Data-heavy forms | All WinForms apps |

**Recommendation**: Use MVP for most WinForms apps. Use MVVM if you need strong data binding and are on .NET 8+.

---

## 🔗 Related Topics

- [MVP Pattern](mvp-pattern.md) - Alternative pattern
- [Data Binding](../ui-ux/data-binding.md) - WinForms data binding details
- [Dependency Injection](dependency-injection.md) - DI setup

---

**Last Updated**: 2025-11-07
