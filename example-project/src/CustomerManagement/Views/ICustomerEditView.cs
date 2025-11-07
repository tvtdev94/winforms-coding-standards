using CustomerManagement.Models;

namespace CustomerManagement.Views;

/// <summary>
/// View interface for the customer edit/create form.
/// Defines the contract between the view and presenter.
/// </summary>
public interface ICustomerEditView
{
    /// <summary>
    /// Event raised when the form is loaded.
    /// </summary>
    event EventHandler? LoadRequested;

    /// <summary>
    /// Event raised when the user requests to save the customer.
    /// </summary>
    event EventHandler? SaveRequested;

    /// <summary>
    /// Event raised when the user requests to cancel.
    /// </summary>
    event EventHandler? CancelRequested;

    /// <summary>
    /// Gets or sets the customer ID (0 for new customer).
    /// </summary>
    int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the customer name.
    /// </summary>
    string CustomerName { get; set; }

    /// <summary>
    /// Gets or sets the customer email.
    /// </summary>
    string CustomerEmail { get; set; }

    /// <summary>
    /// Gets or sets the customer phone.
    /// </summary>
    string CustomerPhone { get; set; }

    /// <summary>
    /// Gets or sets the customer address.
    /// </summary>
    string CustomerAddress { get; set; }

    /// <summary>
    /// Gets or sets the customer city.
    /// </summary>
    string CustomerCity { get; set; }

    /// <summary>
    /// Gets or sets the customer country.
    /// </summary>
    string CustomerCountry { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the customer is active.
    /// </summary>
    bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the view is in loading state.
    /// </summary>
    bool IsLoading { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the form is in edit mode (vs create mode).
    /// </summary>
    bool IsEditMode { get; set; }

    /// <summary>
    /// Sets a validation error for a specific field.
    /// </summary>
    /// <param name="fieldName">The field name.</param>
    /// <param name="errorMessage">The error message.</param>
    void SetFieldError(string fieldName, string errorMessage);

    /// <summary>
    /// Clears all validation errors.
    /// </summary>
    void ClearAllErrors();

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
    /// Closes the view with a dialog result.
    /// </summary>
    /// <param name="success">True if operation was successful; otherwise, false.</param>
    void CloseWithResult(bool success);
}
