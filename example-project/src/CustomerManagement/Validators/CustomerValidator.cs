using CustomerManagement.Models;
using System.Text.RegularExpressions;

namespace CustomerManagement.Validators;

/// <summary>
/// Validator for Customer entity.
/// Provides validation logic separate from service layer.
/// </summary>
/// <remarks>
/// This validator can be used standalone or with FluentValidation.
/// For FluentValidation, install: dotnet add package FluentValidation
/// </remarks>
public partial class CustomerValidator : IEntityValidator<Customer>
{
    /// <inheritdoc />
    public Dictionary<string, string> Validate(Customer entity)
    {
        var errors = new Dictionary<string, string>();

        if (entity == null)
        {
            errors["Entity"] = "Customer cannot be null.";
            return errors;
        }

        // ═══════════════════════════════════════════════════════════════
        // Name validation (required)
        // ═══════════════════════════════════════════════════════════════
        if (string.IsNullOrWhiteSpace(entity.Name))
        {
            errors[nameof(Customer.Name)] = "Name is required.";
        }
        else if (entity.Name.Length > 100)
        {
            errors[nameof(Customer.Name)] = "Name cannot exceed 100 characters.";
        }

        // ═══════════════════════════════════════════════════════════════
        // Email validation (required)
        // ═══════════════════════════════════════════════════════════════
        if (string.IsNullOrWhiteSpace(entity.Email))
        {
            errors[nameof(Customer.Email)] = "Email is required.";
        }
        else if (entity.Email.Length > 100)
        {
            errors[nameof(Customer.Email)] = "Email cannot exceed 100 characters.";
        }
        else if (!IsValidEmail(entity.Email))
        {
            errors[nameof(Customer.Email)] = "Email format is invalid.";
        }

        // ═══════════════════════════════════════════════════════════════
        // Phone validation (optional)
        // ═══════════════════════════════════════════════════════════════
        if (!string.IsNullOrWhiteSpace(entity.Phone))
        {
            if (entity.Phone.Length > 20)
            {
                errors[nameof(Customer.Phone)] = "Phone cannot exceed 20 characters.";
            }
            else if (!IsValidPhone(entity.Phone))
            {
                errors[nameof(Customer.Phone)] = "Phone format is invalid. Use digits, spaces, and +-() only.";
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // Address validation (optional)
        // ═══════════════════════════════════════════════════════════════
        if (!string.IsNullOrWhiteSpace(entity.Address) && entity.Address.Length > 200)
        {
            errors[nameof(Customer.Address)] = "Address cannot exceed 200 characters.";
        }

        // ═══════════════════════════════════════════════════════════════
        // City validation (optional)
        // ═══════════════════════════════════════════════════════════════
        if (!string.IsNullOrWhiteSpace(entity.City) && entity.City.Length > 50)
        {
            errors[nameof(Customer.City)] = "City cannot exceed 50 characters.";
        }

        // ═══════════════════════════════════════════════════════════════
        // Country validation (optional)
        // ═══════════════════════════════════════════════════════════════
        if (!string.IsNullOrWhiteSpace(entity.Country) && entity.Country.Length > 50)
        {
            errors[nameof(Customer.Country)] = "Country cannot exceed 50 characters.";
        }

        return errors;
    }

    /// <inheritdoc />
    public bool IsValid(Customer entity)
    {
        return !Validate(entity).Any();
    }

    /// <summary>
    /// Validates email format using regex.
    /// </summary>
    /// <param name="email">The email to validate.</param>
    /// <returns>True if valid; otherwise, false.</returns>
    private static bool IsValidEmail(string email)
    {
        return EmailRegex().IsMatch(email);
    }

    /// <summary>
    /// Validates phone format.
    /// Allows digits, spaces, and common phone characters.
    /// </summary>
    /// <param name="phone">The phone number to validate.</param>
    /// <returns>True if valid; otherwise, false.</returns>
    private static bool IsValidPhone(string phone)
    {
        return PhoneRegex().IsMatch(phone);
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"^[\d\s\-\+\(\)]+$")]
    private static partial Regex PhoneRegex();
}

/// <summary>
/// Generic validator interface for entities.
/// </summary>
/// <typeparam name="T">The entity type to validate.</typeparam>
public interface IEntityValidator<T> where T : class
{
    /// <summary>
    /// Validates the entity and returns any validation errors.
    /// </summary>
    /// <param name="entity">The entity to validate.</param>
    /// <returns>Dictionary of property names and error messages. Empty if valid.</returns>
    Dictionary<string, string> Validate(T entity);

    /// <summary>
    /// Checks if the entity is valid.
    /// </summary>
    /// <param name="entity">The entity to validate.</param>
    /// <returns>True if valid; otherwise, false.</returns>
    bool IsValid(T entity);
}

/// <summary>
/// Extension methods for validation.
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Maps model property names to view property names.
    /// Used when view properties have different names than model properties.
    /// </summary>
    /// <param name="errors">The validation errors with model property names.</param>
    /// <param name="prefix">The prefix to add (e.g., "Customer" → CustomerName).</param>
    /// <returns>Dictionary with view property names.</returns>
    public static Dictionary<string, string> MapToViewProperties(
        this Dictionary<string, string> errors,
        string prefix = "Customer")
    {
        return errors.ToDictionary(
            kvp => $"{prefix}{kvp.Key}",
            kvp => kvp.Value);
    }

    /// <summary>
    /// Converts validation errors to a single error message.
    /// </summary>
    /// <param name="errors">The validation errors.</param>
    /// <returns>Combined error message or null if no errors.</returns>
    public static string? ToErrorMessage(this Dictionary<string, string> errors)
    {
        if (!errors.Any())
            return null;

        return string.Join(Environment.NewLine,
            errors.Select(e => $"• {e.Key}: {e.Value}"));
    }
}
