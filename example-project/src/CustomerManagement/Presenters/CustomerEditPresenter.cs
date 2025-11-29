using CustomerManagement.Models;
using CustomerManagement.Services;
using CustomerManagement.Views;
using Microsoft.Extensions.Logging;

namespace CustomerManagement.Presenters;

/// <summary>
/// Presenter for the customer edit/create view.
/// Handles business logic and coordinates between view and service.
/// </summary>
public class CustomerEditPresenter
{
    private readonly ICustomerEditView _view;
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomerEditPresenter> _logger;
    private Customer? _currentCustomer;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerEditPresenter"/> class.
    /// </summary>
    /// <param name="view">The view.</param>
    /// <param name="customerService">The customer service.</param>
    /// <param name="logger">The logger.</param>
    public CustomerEditPresenter(
        ICustomerEditView view,
        ICustomerService customerService,
        ILogger<CustomerEditPresenter> logger)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Subscribe to view events
        _view.LoadRequested += OnLoadRequested;
        _view.SaveRequested += OnSaveRequested;
        _view.CancelRequested += OnCancelRequested;
    }

    /// <summary>
    /// Handles the load requested event.
    /// </summary>
    private async void OnLoadRequested(object? sender, EventArgs e)
    {
        await LoadCustomerAsync();
    }

    /// <summary>
    /// Loads the customer data if in edit mode.
    /// </summary>
    private async Task LoadCustomerAsync()
    {
        try
        {
            if (_view.CustomerId == 0)
            {
                // Create mode
                _view.IsEditMode = false;
                _logger.LogInformation("Creating new customer");
                return;
            }

            // Edit mode
            _view.IsEditMode = true;
            _view.IsLoading = true;

            _logger.LogInformation("Loading customer with ID: {CustomerId}", _view.CustomerId);

            _currentCustomer = await _customerService.GetCustomerByIdAsync(_view.CustomerId);

            if (_currentCustomer == null)
            {
                _view.ShowError($"Customer with ID {_view.CustomerId} not found.");
                _view.CloseWithResult(false);
                return;
            }

            // Populate view with customer data
            _view.CustomerName = _currentCustomer.Name;
            _view.CustomerEmail = _currentCustomer.Email;
            _view.CustomerPhone = _currentCustomer.Phone ?? string.Empty;
            _view.CustomerAddress = _currentCustomer.Address ?? string.Empty;
            _view.CustomerCity = _currentCustomer.City ?? string.Empty;
            _view.CustomerCountry = _currentCustomer.Country ?? string.Empty;
            _view.IsActive = _currentCustomer.IsActive;

            _logger.LogInformation("Customer loaded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading customer with ID: {CustomerId}", _view.CustomerId);
            _view.ShowError($"Failed to load customer: {ex.Message}");
            _view.CloseWithResult(false);
        }
        finally
        {
            _view.IsLoading = false;
        }
    }

    /// <summary>
    /// Handles the save requested event.
    /// </summary>
    private async void OnSaveRequested(object? sender, EventArgs e)
    {
        await SaveCustomerAsync();
    }

    /// <summary>
    /// Saves the customer data (create or update).
    /// </summary>
    private async Task SaveCustomerAsync()
    {
        try
        {
            _view.ClearAllErrors();
            _view.IsLoading = true;

            // Create or update customer object
            var customer = _currentCustomer ?? new Customer();
            customer.Name = _view.CustomerName?.Trim() ?? string.Empty;
            customer.Email = _view.CustomerEmail?.Trim() ?? string.Empty;
            customer.Phone = string.IsNullOrWhiteSpace(_view.CustomerPhone) ? null : _view.CustomerPhone.Trim();
            customer.Address = string.IsNullOrWhiteSpace(_view.CustomerAddress) ? null : _view.CustomerAddress.Trim();
            customer.City = string.IsNullOrWhiteSpace(_view.CustomerCity) ? null : _view.CustomerCity.Trim();
            customer.Country = string.IsNullOrWhiteSpace(_view.CustomerCountry) ? null : _view.CustomerCountry.Trim();
            customer.IsActive = _view.IsActive;

            // Validate customer
            var validationErrors = _customerService.ValidateCustomer(customer);
            if (validationErrors.Any())
            {
                _logger.LogWarning("Validation failed for customer");

                // Map model property names to view property names
                // Model uses: Name, Email, Phone, Address, City, Country
                // View uses: CustomerName, CustomerEmail, CustomerPhone, etc.
                var fieldNameMap = new Dictionary<string, string>
                {
                    { "Name", nameof(_view.CustomerName) },
                    { "Email", nameof(_view.CustomerEmail) },
                    { "Phone", nameof(_view.CustomerPhone) },
                    { "Address", nameof(_view.CustomerAddress) },
                    { "City", nameof(_view.CustomerCity) },
                    { "Country", nameof(_view.CustomerCountry) }
                };

                foreach (var error in validationErrors)
                {
                    var viewFieldName = fieldNameMap.TryGetValue(error.Key, out var mapped)
                        ? mapped
                        : error.Key;
                    _view.SetFieldError(viewFieldName, error.Value);
                }

                _view.ShowError("Please fix the validation errors.");
                return;
            }

            if (_view.IsEditMode)
            {
                // Update existing customer
                _logger.LogInformation("Updating customer with ID: {CustomerId}", customer.Id);
                await _customerService.UpdateCustomerAsync(customer);
                _view.ShowSuccess("Customer updated successfully.");
            }
            else
            {
                // Create new customer
                _logger.LogInformation("Creating new customer");
                await _customerService.CreateCustomerAsync(customer);
                _view.ShowSuccess("Customer created successfully.");
            }

            _view.CloseWithResult(true);
        }
        catch (InvalidOperationException ex)
        {
            // Business logic validation errors
            _logger.LogWarning(ex, "Business validation failed");
            _view.ShowError(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving customer");
            _view.ShowError($"Failed to save customer: {ex.Message}");
        }
        finally
        {
            _view.IsLoading = false;
        }
    }

    /// <summary>
    /// Handles the cancel requested event.
    /// </summary>
    private void OnCancelRequested(object? sender, EventArgs e)
    {
        _logger.LogInformation("Customer edit cancelled");
        _view.CloseWithResult(false);
    }

    /// <summary>
    /// Disposes resources and unsubscribes from events.
    /// </summary>
    public void Dispose()
    {
        // Unsubscribe from view events
        _view.LoadRequested -= OnLoadRequested;
        _view.SaveRequested -= OnSaveRequested;
        _view.CancelRequested -= OnCancelRequested;
    }
}
