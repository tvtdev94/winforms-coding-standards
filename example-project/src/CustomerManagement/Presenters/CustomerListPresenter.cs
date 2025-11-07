using CustomerManagement.Services;
using CustomerManagement.Views;
using Microsoft.Extensions.Logging;

namespace CustomerManagement.Presenters;

/// <summary>
/// Presenter for the customer list view.
/// Handles business logic and coordinates between view and service.
/// </summary>
public class CustomerListPresenter
{
    private readonly ICustomerListView _view;
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomerListPresenter> _logger;
    private CancellationTokenSource? _searchCts;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerListPresenter"/> class.
    /// </summary>
    /// <param name="view">The view.</param>
    /// <param name="customerService">The customer service.</param>
    /// <param name="logger">The logger.</param>
    public CustomerListPresenter(
        ICustomerListView view,
        ICustomerService customerService,
        ILogger<CustomerListPresenter> logger)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Subscribe to view events
        _view.LoadRequested += OnLoadRequested;
        _view.RefreshRequested += OnRefreshRequested;
        _view.SearchRequested += OnSearchRequested;
        _view.AddRequested += OnAddRequested;
        _view.EditRequested += OnEditRequested;
        _view.DeleteRequested += OnDeleteRequested;
        _view.ViewDetailsRequested += OnViewDetailsRequested;
    }

    /// <summary>
    /// Handles the load requested event.
    /// </summary>
    private async void OnLoadRequested(object? sender, EventArgs e)
    {
        await LoadCustomersAsync();
    }

    /// <summary>
    /// Handles the refresh requested event.
    /// </summary>
    private async void OnRefreshRequested(object? sender, EventArgs e)
    {
        await LoadCustomersAsync();
    }

    /// <summary>
    /// Loads all customers asynchronously.
    /// </summary>
    private async Task LoadCustomersAsync()
    {
        try
        {
            _logger.LogInformation("Loading customers");
            _view.IsLoading = true;

            var customers = await _customerService.GetAllCustomersAsync();
            _view.Customers = customers;

            _logger.LogInformation("Loaded {Count} customers", customers.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading customers");
            _view.ShowError($"Failed to load customers: {ex.Message}");
        }
        finally
        {
            _view.IsLoading = false;
        }
    }

    /// <summary>
    /// Handles the search requested event with debouncing.
    /// </summary>
    private async void OnSearchRequested(object? sender, string searchTerm)
    {
        // Cancel previous search if still running
        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();
        var cancellationToken = _searchCts.Token;

        try
        {
            // Debounce: wait 300ms before searching
            await Task.Delay(300, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            _logger.LogInformation("Searching customers with term: {SearchTerm}", searchTerm);
            _view.IsLoading = true;

            var customers = await _customerService.SearchCustomersAsync(searchTerm, cancellationToken);
            _view.Customers = customers;

            _logger.LogInformation("Found {Count} customers", customers.Count);
        }
        catch (OperationCanceledException)
        {
            // Search was cancelled, this is expected
            _logger.LogDebug("Search cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching customers");
            _view.ShowError($"Failed to search customers: {ex.Message}");
        }
        finally
        {
            _view.IsLoading = false;
        }
    }

    /// <summary>
    /// Handles the add requested event.
    /// </summary>
    private void OnAddRequested(object? sender, EventArgs e)
    {
        _logger.LogInformation("Add customer requested");
        // Note: In the actual implementation, this would open the CustomerEditForm
        // For now, we'll just log it. The Form will handle creating and showing the edit form.
    }

    /// <summary>
    /// Handles the edit requested event.
    /// </summary>
    private void OnEditRequested(object? sender, int customerId)
    {
        _logger.LogInformation("Edit customer requested for ID: {CustomerId}", customerId);
        // Note: In the actual implementation, this would open the CustomerEditForm
        // For now, we'll just log it. The Form will handle creating and showing the edit form.
    }

    /// <summary>
    /// Handles the delete requested event.
    /// </summary>
    private async void OnDeleteRequested(object? sender, int customerId)
    {
        try
        {
            var customer = _view.SelectedCustomer;
            if (customer == null)
            {
                _view.ShowError("Please select a customer to delete.");
                return;
            }

            var confirmed = _view.ShowConfirmation(
                $"Are you sure you want to delete customer '{customer.Name}'?",
                "Confirm Delete");

            if (!confirmed)
            {
                return;
            }

            _logger.LogInformation("Deleting customer with ID: {CustomerId}", customerId);
            _view.IsLoading = true;

            await _customerService.DeleteCustomerAsync(customerId);

            _view.ShowSuccess($"Customer '{customer.Name}' deleted successfully.");
            await LoadCustomersAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer with ID: {CustomerId}", customerId);
            _view.ShowError($"Failed to delete customer: {ex.Message}");
        }
        finally
        {
            _view.IsLoading = false;
        }
    }

    /// <summary>
    /// Handles the view details requested event.
    /// </summary>
    private async void OnViewDetailsRequested(object? sender, int customerId)
    {
        try
        {
            _logger.LogInformation("Viewing details for customer ID: {CustomerId}", customerId);
            _view.IsLoading = true;

            var customer = await _customerService.GetCustomerWithOrdersAsync(customerId);

            if (customer == null)
            {
                _view.ShowError("Customer not found.");
                return;
            }

            // Note: In a real implementation, this would open a details dialog
            // For now, we'll show a simple message
            var message = $"Customer: {customer.Name}\n" +
                         $"Email: {customer.Email}\n" +
                         $"Phone: {customer.Phone}\n" +
                         $"Orders: {customer.Orders.Count}";

            _view.ShowSuccess(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error viewing customer details for ID: {CustomerId}", customerId);
            _view.ShowError($"Failed to load customer details: {ex.Message}");
        }
        finally
        {
            _view.IsLoading = false;
        }
    }

    /// <summary>
    /// Disposes resources and unsubscribes from events.
    /// </summary>
    public void Dispose()
    {
        // Unsubscribe from view events
        _view.LoadRequested -= OnLoadRequested;
        _view.RefreshRequested -= OnRefreshRequested;
        _view.SearchRequested -= OnSearchRequested;
        _view.AddRequested -= OnAddRequested;
        _view.EditRequested -= OnEditRequested;
        _view.DeleteRequested -= OnDeleteRequested;
        _view.ViewDetailsRequested -= OnViewDetailsRequested;

        // Dispose cancellation token source
        _searchCts?.Cancel();
        _searchCts?.Dispose();
    }
}
