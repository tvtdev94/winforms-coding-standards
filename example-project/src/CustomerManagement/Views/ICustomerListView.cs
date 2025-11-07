using CustomerManagement.Models;

namespace CustomerManagement.Views;

/// <summary>
/// View interface for the customer list form.
/// Defines the contract between the view and presenter.
/// </summary>
public interface ICustomerListView
{
    /// <summary>
    /// Event raised when the form is loaded.
    /// </summary>
    event EventHandler? LoadRequested;

    /// <summary>
    /// Event raised when the user requests to add a new customer.
    /// </summary>
    event EventHandler? AddRequested;

    /// <summary>
    /// Event raised when the user requests to edit a customer.
    /// </summary>
    event EventHandler<int>? EditRequested;

    /// <summary>
    /// Event raised when the user requests to delete a customer.
    /// </summary>
    event EventHandler<int>? DeleteRequested;

    /// <summary>
    /// Event raised when the user requests to refresh the data.
    /// </summary>
    event EventHandler? RefreshRequested;

    /// <summary>
    /// Event raised when the user types in the search box.
    /// </summary>
    event EventHandler<string>? SearchRequested;

    /// <summary>
    /// Event raised when the user requests to view customer details.
    /// </summary>
    event EventHandler<int>? ViewDetailsRequested;

    /// <summary>
    /// Gets or sets the list of customers to display.
    /// </summary>
    List<Customer> Customers { get; set; }

    /// <summary>
    /// Gets the currently selected customer (if any).
    /// </summary>
    Customer? SelectedCustomer { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the view is in loading state.
    /// </summary>
    bool IsLoading { get; set; }

    /// <summary>
    /// Shows a success message to the user.
    /// </summary>
    /// <param name="message">The message to display.</param>
    void ShowSuccess(string message);

    /// <summary>
    /// Shows an error message to the user.
    /// </summary>
    /// <param name="message">The error message to display.</param>
    void ShowError(string message);

    /// <summary>
    /// Shows a confirmation dialog and returns the user's choice.
    /// </summary>
    /// <param name="message">The confirmation message.</param>
    /// <param name="title">The dialog title.</param>
    /// <returns>True if user confirmed; otherwise, false.</returns>
    bool ShowConfirmation(string message, string title);

    /// <summary>
    /// Closes the view.
    /// </summary>
    void Close();
}
