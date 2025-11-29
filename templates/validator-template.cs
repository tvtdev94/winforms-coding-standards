// Template: Entity Validator using FluentValidation
// Replace: YourEntity, YourValidator
// IMPORTANT: Validators separate validation logic from business logic for clean, testable code

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace YourNamespace.Validators
{
    #region Option 1: FluentValidation (Recommended for complex validation)

    /// <summary>
    /// Validator for YourEntity using FluentValidation library.
    /// Install: dotnet add package FluentValidation
    /// </summary>
    /// <remarks>
    /// Benefits of FluentValidation:
    /// - Fluent, readable syntax
    /// - Built-in validators (NotEmpty, MaxLength, EmailAddress, etc.)
    /// - Easy to test
    /// - Supports async validation
    /// - Integrates with ASP.NET Core, WinForms, etc.
    /// </remarks>
    public class YourEntityValidator : AbstractValidator<YourEntity>
    {
        /// <summary>
        /// Initializes validation rules for YourEntity.
        /// </summary>
        public YourEntityValidator()
        {
            // ═══════════════════════════════════════════════════════════════
            // Required Fields
            // ═══════════════════════════════════════════════════════════════

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Name can only contain letters and spaces.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters.");

            // ═══════════════════════════════════════════════════════════════
            // Optional Fields (validate only when provided)
            // ═══════════════════════════════════════════════════════════════

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("Phone cannot exceed 20 characters.")
                .Matches(@"^[\d\s\+\-\(\)]+$")
                    .WithMessage("Phone can only contain digits, spaces, and +-()")
                    .When(x => !string.IsNullOrWhiteSpace(x.Phone));

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            // ═══════════════════════════════════════════════════════════════
            // Numeric Fields
            // ═══════════════════════════════════════════════════════════════

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.")
                .LessThanOrEqualTo(999999.99m).WithMessage("Price cannot exceed 999,999.99.");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative.")
                .LessThanOrEqualTo(10000).WithMessage("Quantity cannot exceed 10,000.");

            // ═══════════════════════════════════════════════════════════════
            // Date Fields
            // ═══════════════════════════════════════════════════════════════

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Start date cannot be in the past.")
                .When(x => x.Id == 0); // Only for new entities

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.")
                .When(x => x.EndDate.HasValue);

            // ═══════════════════════════════════════════════════════════════
            // Complex Validation Rules
            // ═══════════════════════════════════════════════════════════════

            // Cross-field validation
            RuleFor(x => x)
                .Must(HaveValidDateRange)
                .WithMessage("End date must be within 1 year of start date.")
                .When(x => x.EndDate.HasValue);

            // Custom validation with external dependency
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MaximumLength(10).WithMessage("Code cannot exceed 10 characters.")
                .Matches(@"^[A-Z]{2}\d{4}$").WithMessage("Code must be in format XX0000 (2 letters + 4 digits).");
        }

        /// <summary>
        /// Cross-field validation: date range must be valid.
        /// </summary>
        private bool HaveValidDateRange(YourEntity entity)
        {
            if (!entity.EndDate.HasValue)
                return true;

            var maxEndDate = entity.StartDate.AddYears(1);
            return entity.EndDate.Value <= maxEndDate;
        }
    }

    /// <summary>
    /// Validator with async validation support.
    /// Use when validation requires database/API calls.
    /// </summary>
    public class YourEntityAsyncValidator : AbstractValidator<YourEntity>
    {
        private readonly IYourRepository _repository;

        public YourEntityAsyncValidator(IYourRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MustAsync(BeUniqueEmailAsync).WithMessage("Email is already in use.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MustAsync(BeUniqueCodeAsync).WithMessage("Code is already in use.");
        }

        /// <summary>
        /// Checks if email is unique (async validation).
        /// </summary>
        private async Task<bool> BeUniqueEmailAsync(YourEntity entity, string email, CancellationToken cancellationToken)
        {
            var existing = await _repository.GetByEmailAsync(email, cancellationToken);
            return existing == null || existing.Id == entity.Id;
        }

        /// <summary>
        /// Checks if code is unique (async validation).
        /// </summary>
        private async Task<bool> BeUniqueCodeAsync(YourEntity entity, string code, CancellationToken cancellationToken)
        {
            var existing = await _repository.GetByCodeAsync(code, cancellationToken);
            return existing == null || existing.Id == entity.Id;
        }
    }

    #endregion

    #region Option 2: Simple Validator (No external dependencies)

    /// <summary>
    /// Simple validator interface for entities.
    /// Use when you don't want FluentValidation dependency.
    /// </summary>
    /// <typeparam name="T">The entity type to validate.</typeparam>
    public interface ISimpleValidator<T>
    {
        /// <summary>
        /// Validates the entity synchronously.
        /// </summary>
        /// <param name="entity">The entity to validate.</param>
        /// <returns>Dictionary of field names and error messages. Empty if valid.</returns>
        Dictionary<string, string> Validate(T entity);

        /// <summary>
        /// Validates the entity asynchronously (for database checks).
        /// </summary>
        /// <param name="entity">The entity to validate.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Dictionary of field names and error messages. Empty if valid.</returns>
        Task<Dictionary<string, string>> ValidateAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if the entity is valid.
        /// </summary>
        /// <param name="entity">The entity to validate.</param>
        /// <returns>True if valid; false otherwise.</returns>
        bool IsValid(T entity);
    }

    /// <summary>
    /// Simple validator implementation for YourEntity.
    /// No external dependencies - pure C#.
    /// </summary>
    public class YourEntitySimpleValidator : ISimpleValidator<YourEntity>
    {
        // Common regex patterns
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        private static readonly Regex PhoneRegex = new(@"^[\d\s\+\-\(\)]+$", RegexOptions.Compiled);
        private static readonly Regex CodeRegex = new(@"^[A-Z]{2}\d{4}$", RegexOptions.Compiled);
        private static readonly Regex NameRegex = new(@"^[a-zA-Z\s]+$", RegexOptions.Compiled);

        /// <inheritdoc />
        public Dictionary<string, string> Validate(YourEntity entity)
        {
            var errors = new Dictionary<string, string>();

            if (entity == null)
            {
                errors["Entity"] = "Entity cannot be null.";
                return errors;
            }

            // ═══════════════════════════════════════════════════════════════
            // Name validation
            // ═══════════════════════════════════════════════════════════════
            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                errors[nameof(entity.Name)] = "Name is required.";
            }
            else if (entity.Name.Length > 100)
            {
                errors[nameof(entity.Name)] = "Name cannot exceed 100 characters.";
            }
            else if (!NameRegex.IsMatch(entity.Name))
            {
                errors[nameof(entity.Name)] = "Name can only contain letters and spaces.";
            }

            // ═══════════════════════════════════════════════════════════════
            // Email validation
            // ═══════════════════════════════════════════════════════════════
            if (string.IsNullOrWhiteSpace(entity.Email))
            {
                errors[nameof(entity.Email)] = "Email is required.";
            }
            else if (entity.Email.Length > 255)
            {
                errors[nameof(entity.Email)] = "Email cannot exceed 255 characters.";
            }
            else if (!EmailRegex.IsMatch(entity.Email))
            {
                errors[nameof(entity.Email)] = "Invalid email format.";
            }

            // ═══════════════════════════════════════════════════════════════
            // Phone validation (optional field)
            // ═══════════════════════════════════════════════════════════════
            if (!string.IsNullOrWhiteSpace(entity.Phone))
            {
                if (entity.Phone.Length > 20)
                {
                    errors[nameof(entity.Phone)] = "Phone cannot exceed 20 characters.";
                }
                else if (!PhoneRegex.IsMatch(entity.Phone))
                {
                    errors[nameof(entity.Phone)] = "Phone can only contain digits, spaces, and +-()";
                }
            }

            // ═══════════════════════════════════════════════════════════════
            // Code validation
            // ═══════════════════════════════════════════════════════════════
            if (string.IsNullOrWhiteSpace(entity.Code))
            {
                errors[nameof(entity.Code)] = "Code is required.";
            }
            else if (!CodeRegex.IsMatch(entity.Code))
            {
                errors[nameof(entity.Code)] = "Code must be in format XX0000 (2 letters + 4 digits).";
            }

            // ═══════════════════════════════════════════════════════════════
            // Price validation
            // ═══════════════════════════════════════════════════════════════
            if (entity.Price < 0)
            {
                errors[nameof(entity.Price)] = "Price cannot be negative.";
            }
            else if (entity.Price > 999999.99m)
            {
                errors[nameof(entity.Price)] = "Price cannot exceed 999,999.99.";
            }

            // ═══════════════════════════════════════════════════════════════
            // Date validation
            // ═══════════════════════════════════════════════════════════════
            if (entity.Id == 0 && entity.StartDate < DateTime.Today)
            {
                errors[nameof(entity.StartDate)] = "Start date cannot be in the past for new entities.";
            }

            if (entity.EndDate.HasValue)
            {
                if (entity.EndDate.Value <= entity.StartDate)
                {
                    errors[nameof(entity.EndDate)] = "End date must be after start date.";
                }
                else if (entity.EndDate.Value > entity.StartDate.AddYears(1))
                {
                    errors[nameof(entity.EndDate)] = "End date must be within 1 year of start date.";
                }
            }

            return errors;
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, string>> ValidateAsync(YourEntity entity, CancellationToken cancellationToken = default)
        {
            // First, run synchronous validation
            var errors = Validate(entity);

            // If basic validation fails, return early
            if (errors.Any())
            {
                return errors;
            }

            // TODO: Add async validation here (database checks)
            // Example:
            // var existingEmail = await _repository.GetByEmailAsync(entity.Email, cancellationToken);
            // if (existingEmail != null && existingEmail.Id != entity.Id)
            // {
            //     errors[nameof(entity.Email)] = "Email is already in use.";
            // }

            await Task.CompletedTask;
            return errors;
        }

        /// <inheritdoc />
        public bool IsValid(YourEntity entity)
        {
            return !Validate(entity).Any();
        }
    }

    #endregion

    #region Option 3: Data Annotations Validator (Use with [Required], [MaxLength], etc.)

    /// <summary>
    /// Validator that works with Data Annotations attributes.
    /// Reads [Required], [MaxLength], [EmailAddress], etc. from entity properties.
    /// </summary>
    /// <remarks>
    /// Usage:
    /// 1. Add attributes to your entity properties:
    ///    [Required(ErrorMessage = "Name is required")]
    ///    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    ///    public string Name { get; set; }
    ///
    /// 2. Use this validator to validate:
    ///    var validator = new DataAnnotationsValidator();
    ///    var errors = validator.Validate(entity);
    /// </remarks>
    public class DataAnnotationsValidator
    {
        /// <summary>
        /// Validates an entity using Data Annotations attributes.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity to validate.</param>
        /// <returns>Dictionary of field names and error messages.</returns>
        public Dictionary<string, string> Validate<T>(T entity) where T : class
        {
            var errors = new Dictionary<string, string>();

            if (entity == null)
            {
                errors["Entity"] = "Entity cannot be null.";
                return errors;
            }

            var context = new System.ComponentModel.DataAnnotations.ValidationContext(entity);
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

            var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
                entity, context, results, validateAllProperties: true);

            if (!isValid)
            {
                foreach (var result in results)
                {
                    var fieldName = result.MemberNames.FirstOrDefault() ?? "General";
                    var errorMessage = result.ErrorMessage ?? "Validation failed.";

                    if (!errors.ContainsKey(fieldName))
                    {
                        errors[fieldName] = errorMessage;
                    }
                }
            }

            return errors;
        }
    }

    #endregion

    #region Validation Helper Extension Methods

    /// <summary>
    /// Extension methods for validation helpers.
    /// </summary>
    public static class ValidationExtensions
    {
        /// <summary>
        /// Converts FluentValidation ValidationResult to dictionary format.
        /// </summary>
        /// <param name="result">The validation result.</param>
        /// <returns>Dictionary of property names and error messages.</returns>
        public static Dictionary<string, string> ToDictionary(this ValidationResult result)
        {
            return result.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => string.Join("; ", g.Select(e => e.ErrorMessage)));
        }

        /// <summary>
        /// Converts FluentValidation ValidationResult to a single error message.
        /// </summary>
        /// <param name="result">The validation result.</param>
        /// <returns>Combined error message or null if valid.</returns>
        public static string? ToErrorMessage(this ValidationResult result)
        {
            if (result.IsValid)
                return null;

            return string.Join(Environment.NewLine,
                result.Errors.Select(e => $"• {e.PropertyName}: {e.ErrorMessage}"));
        }

        /// <summary>
        /// Checks if a string is a valid email format.
        /// </summary>
        public static bool IsValidEmail(this string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a string is a valid phone number format.
        /// </summary>
        public static bool IsValidPhone(this string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return true; // Optional field

            return Regex.IsMatch(phone, @"^[\d\s\+\-\(\)]+$");
        }
    }

    #endregion

    #region Placeholder Types (Remove when using actual types)

    // TODO: Remove these placeholders and use your actual types

    /// <summary>
    /// Placeholder entity class with Data Annotations.
    /// Replace with your actual entity.
    /// </summary>
    public class YourEntity
    {
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Name is required.")]
        [System.ComponentModel.DataAnnotations.MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Email is required.")]
        [System.ComponentModel.DataAnnotations.EmailAddress(ErrorMessage = "Invalid email format.")]
        [System.ComponentModel.DataAnnotations.MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.MaxLength(20)]
        public string? Phone { get; set; }

        [System.ComponentModel.DataAnnotations.MaxLength(500)]
        public string? Description { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Code is required.")]
        [System.ComponentModel.DataAnnotations.MaxLength(10)]
        public string Code { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Range(0, 999999.99, ErrorMessage = "Price must be between 0 and 999,999.99.")]
        public decimal Price { get; set; }

        [System.ComponentModel.DataAnnotations.Range(0, 10000, ErrorMessage = "Quantity must be between 0 and 10,000.")]
        public int Quantity { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime? EndDate { get; set; }
    }

    /// <summary>
    /// Placeholder repository interface for async validation.
    /// Replace with your actual repository.
    /// </summary>
    public interface IYourRepository
    {
        Task<YourEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<YourEntity?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    }

    #endregion
}

#region WinForms Integration Example

namespace YourNamespace.Validators
{
    /// <summary>
    /// Example showing how to integrate FluentValidation with WinForms ErrorProvider.
    /// </summary>
    /// <remarks>
    /// Usage in Form:
    /// <code>
    /// private readonly ErrorProvider _errorProvider = new();
    /// private readonly YourEntityValidator _validator = new();
    ///
    /// public void SetFieldError(string fieldName, string errorMessage)
    /// {
    ///     var control = Controls.Find($"txt{fieldName}", true).FirstOrDefault();
    ///     if (control != null)
    ///     {
    ///         _errorProvider.SetError(control, errorMessage);
    ///     }
    /// }
    ///
    /// public void ClearAllErrors()
    /// {
    ///     _errorProvider.Clear();
    /// }
    ///
    /// // In presenter:
    /// var result = _validator.Validate(entity);
    /// if (!result.IsValid)
    /// {
    ///     foreach (var error in result.Errors)
    ///     {
    ///         _view.SetFieldError(error.PropertyName, error.ErrorMessage);
    ///     }
    ///     _view.ShowError("Please fix the validation errors.");
    ///     return;
    /// }
    /// </code>
    /// </remarks>
    public static class WinFormsValidationHelper
    {
        /// <summary>
        /// Maps validation errors to ErrorProvider on a form.
        /// </summary>
        /// <param name="errorProvider">The ErrorProvider control.</param>
        /// <param name="container">The control container (Form or Panel).</param>
        /// <param name="errors">Dictionary of property names to error messages.</param>
        /// <param name="controlPrefix">Prefix for control names (default: "txt").</param>
        public static void SetErrors(
            System.Windows.Forms.ErrorProvider errorProvider,
            System.Windows.Forms.Control container,
            Dictionary<string, string> errors,
            string controlPrefix = "txt")
        {
            // Clear existing errors
            errorProvider.Clear();

            // Set new errors
            foreach (var error in errors)
            {
                var controlName = $"{controlPrefix}{error.Key}";
                var control = container.Controls.Find(controlName, searchAllChildren: true).FirstOrDefault();

                if (control != null)
                {
                    errorProvider.SetError(control, error.Value);
                }
            }
        }

        /// <summary>
        /// Maps FluentValidation result to ErrorProvider.
        /// </summary>
        public static void SetErrors(
            System.Windows.Forms.ErrorProvider errorProvider,
            System.Windows.Forms.Control container,
            ValidationResult result,
            string controlPrefix = "txt")
        {
            SetErrors(errorProvider, container, result.ToDictionary(), controlPrefix);
        }
    }
}

#endregion
