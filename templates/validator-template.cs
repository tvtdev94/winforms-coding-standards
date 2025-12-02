// Template: Entity Validator using FluentValidation
// Replace: YourEntity, YourValidator
// Options: FluentValidation (recommended), Simple Validator, Data Annotations

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
    #region Option 1: FluentValidation (Recommended)

    /// <summary>
    /// FluentValidation validator. Install: dotnet add package FluentValidation
    /// </summary>
    public class YourEntityValidator : AbstractValidator<YourEntity>
    {
        public YourEntityValidator()
        {
            // Required fields
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            // Optional fields (validate only when provided)
            RuleFor(x => x.Phone)
                .MaximumLength(20)
                .Matches(@"^[\d\s\+\-\(\)]+$").WithMessage("Invalid phone format.")
                .When(x => !string.IsNullOrWhiteSpace(x.Phone));

            // Numeric fields
            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");

            // Date validation
            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.")
                .When(x => x.EndDate.HasValue);
        }
    }

    /// <summary>
    /// Async validator for database checks.
    /// </summary>
    public class YourEntityAsyncValidator : AbstractValidator<YourEntity>
    {
        private readonly IYourRepository _repository;

        public YourEntityAsyncValidator(IYourRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MustAsync(BeUniqueEmailAsync).WithMessage("Email already in use.");

            RuleFor(x => x.Code)
                .NotEmpty()
                .MustAsync(BeUniqueCodeAsync).WithMessage("Code already in use.");
        }

        private async Task<bool> BeUniqueEmailAsync(YourEntity entity, string email, CancellationToken ct)
        {
            var existing = await _repository.GetByEmailAsync(email, ct);
            return existing == null || existing.Id == entity.Id;
        }

        private async Task<bool> BeUniqueCodeAsync(YourEntity entity, string code, CancellationToken ct)
        {
            var existing = await _repository.GetByCodeAsync(code, ct);
            return existing == null || existing.Id == entity.Id;
        }
    }

    #endregion

    #region Option 2: Simple Validator (No dependencies)

    public interface ISimpleValidator<T>
    {
        Dictionary<string, string> Validate(T entity);
        bool IsValid(T entity);
    }

    public class YourEntitySimpleValidator : ISimpleValidator<YourEntity>
    {
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public Dictionary<string, string> Validate(YourEntity entity)
        {
            var errors = new Dictionary<string, string>();
            if (entity == null) { errors["Entity"] = "Entity cannot be null."; return errors; }

            // Name
            if (string.IsNullOrWhiteSpace(entity.Name))
                errors[nameof(entity.Name)] = "Name is required.";
            else if (entity.Name.Length > 100)
                errors[nameof(entity.Name)] = "Name cannot exceed 100 characters.";

            // Email
            if (string.IsNullOrWhiteSpace(entity.Email))
                errors[nameof(entity.Email)] = "Email is required.";
            else if (!EmailRegex.IsMatch(entity.Email))
                errors[nameof(entity.Email)] = "Invalid email format.";

            // Price
            if (entity.Price < 0)
                errors[nameof(entity.Price)] = "Price cannot be negative.";

            // Date range
            if (entity.EndDate.HasValue && entity.EndDate.Value <= entity.StartDate)
                errors[nameof(entity.EndDate)] = "End date must be after start date.";

            return errors;
        }

        public bool IsValid(YourEntity entity) => !Validate(entity).Any();
    }

    #endregion

    #region Option 3: Data Annotations Validator

    /// <summary>
    /// Validates using [Required], [MaxLength], [EmailAddress] attributes.
    /// </summary>
    public class DataAnnotationsValidator
    {
        public Dictionary<string, string> Validate<T>(T entity) where T : class
        {
            var errors = new Dictionary<string, string>();
            if (entity == null) { errors["Entity"] = "Entity cannot be null."; return errors; }

            var context = new System.ComponentModel.DataAnnotations.ValidationContext(entity);
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

            if (!System.ComponentModel.DataAnnotations.Validator.TryValidateObject(entity, context, results, true))
            {
                foreach (var result in results)
                {
                    var fieldName = result.MemberNames.FirstOrDefault() ?? "General";
                    if (!errors.ContainsKey(fieldName))
                        errors[fieldName] = result.ErrorMessage ?? "Validation failed.";
                }
            }
            return errors;
        }
    }

    #endregion

    #region Extension Methods

    public static class ValidationExtensions
    {
        public static Dictionary<string, string> ToDictionary(this ValidationResult result)
            => result.Errors.GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => string.Join("; ", g.Select(e => e.ErrorMessage)));

        public static string? ToErrorMessage(this ValidationResult result)
            => result.IsValid ? null : string.Join(Environment.NewLine,
                result.Errors.Select(e => $"• {e.PropertyName}: {e.ErrorMessage}"));

        public static bool IsValidEmail(this string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try { return new System.Net.Mail.MailAddress(email).Address == email; }
            catch { return false; }
        }
    }

    #endregion

    #region WinForms Integration

    public static class WinFormsValidationHelper
    {
        /// <summary>
        /// Maps validation errors to ErrorProvider.
        /// </summary>
        public static void SetErrors(
            System.Windows.Forms.ErrorProvider errorProvider,
            System.Windows.Forms.Control container,
            Dictionary<string, string> errors,
            string controlPrefix = "txt")
        {
            errorProvider.Clear();
            foreach (var error in errors)
            {
                var control = container.Controls.Find($"{controlPrefix}{error.Key}", true).FirstOrDefault();
                if (control != null)
                    errorProvider.SetError(control, error.Value);
            }
        }
    }

    #endregion

    #region Placeholder Types (Replace with actual types)

    public class YourEntity
    {
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime? EndDate { get; set; }
    }

    public interface IYourRepository
    {
        Task<YourEntity?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<YourEntity?> GetByCodeAsync(string code, CancellationToken ct = default);
    }

    #endregion
}
