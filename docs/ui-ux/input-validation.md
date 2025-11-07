# Input Validation in WinForms

> **Quick Reference**: Comprehensive guide to input validation patterns, techniques, and best practices for robust WinForms applications.

---

## 📋 Overview

Input validation is the process of verifying that user-entered data meets application requirements before processing. Proper validation ensures data integrity, prevents errors, and provides a better user experience.

**This guide covers**:
- Multiple validation approaches (ErrorProvider, Validating event, IDataErrorInfo)
- Real-time vs submit-time validation
- Validation patterns for common scenarios
- MVP integration
- Best practices and UX guidelines

---

## 🎯 Why This Matters

### Security
✅ Prevents SQL injection and XSS attacks
✅ Ensures data meets security constraints
✅ Validates input before processing

### Data Integrity
✅ Maintains database consistency
✅ Prevents invalid data states
✅ Enforces business rules

### User Experience
✅ Immediate feedback on errors
✅ Clear, helpful error messages
✅ Prevents frustration from submit failures

---

## 🛠️ Validation Approaches

### 1. ErrorProvider Component

The `ErrorProvider` component displays error icons next to invalid controls.

#### Setup and Configuration

```csharp
public partial class CustomerForm : Form
{
    private ErrorProvider _errorProvider;

    public CustomerForm()
    {
        InitializeComponent();

        // Initialize ErrorProvider
        _errorProvider = new ErrorProvider();
        _errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        _errorProvider.BlinkRate = 1000;
        _errorProvider.Icon = SystemIcons.Error;
    }
}
```

#### BlinkStyle Options

```csharp
// Never blink (recommended)
_errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;

// Blink if error is set
_errorProvider.BlinkStyle = ErrorBlinkStyle.BlinkIfDifferentError;

// Always blink
_errorProvider.BlinkStyle = ErrorBlinkStyle.AlwaysBlink;
```

#### Setting and Clearing Errors

```csharp
private void ValidateCustomerName()
{
    if (string.IsNullOrWhiteSpace(txtName.Text))
    {
        _errorProvider.SetError(txtName, "Customer name is required");
    }
    else if (txtName.Text.Length < 3)
    {
        _errorProvider.SetError(txtName, "Name must be at least 3 characters");
    }
    else
    {
        _errorProvider.SetError(txtName, string.Empty); // Clear error
    }
}
```

#### Complete ErrorProvider Example

```csharp
public partial class CustomerForm : Form
{
    private readonly ErrorProvider _errorProvider = new ErrorProvider();

    public CustomerForm()
    {
        InitializeComponent();
        _errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;

        // Validate on text change
        txtName.TextChanged += (s, e) => ValidateCustomerName();
        txtEmail.TextChanged += (s, e) => ValidateEmail();
    }

    private void ValidateCustomerName()
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            _errorProvider.SetError(txtName, "Required field");
        }
        else if (txtName.Text.Length < 3)
        {
            _errorProvider.SetError(txtName, "Minimum 3 characters");
        }
        else
        {
            _errorProvider.SetError(txtName, string.Empty);
        }
    }

    private void ValidateEmail()
    {
        if (string.IsNullOrWhiteSpace(txtEmail.Text))
        {
            _errorProvider.SetError(txtEmail, "Email is required");
        }
        else if (!IsValidEmail(txtEmail.Text))
        {
            _errorProvider.SetError(txtEmail, "Invalid email format");
        }
        else
        {
            _errorProvider.SetError(txtEmail, string.Empty);
        }
    }

    private bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    private bool ValidateForm()
    {
        ValidateCustomerName();
        ValidateEmail();

        // Check if any errors exist
        return string.IsNullOrEmpty(_errorProvider.GetError(txtName)) &&
               string.IsNullOrEmpty(_errorProvider.GetError(txtEmail));
    }

    private void BtnSave_Click(object sender, EventArgs e)
    {
        if (!ValidateForm())
        {
            MessageBox.Show("Please correct the errors before saving.",
                "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Proceed with save
        SaveCustomer();
    }
}
```

---

### 2. Validating Event

The `Validating` event fires when a control loses focus, allowing you to prevent focus change if validation fails.

#### Basic Validating Event

```csharp
private void TxtName_Validating(object sender, CancelEventArgs e)
{
    if (string.IsNullOrWhiteSpace(txtName.Text))
    {
        e.Cancel = true; // Prevent focus change
        _errorProvider.SetError(txtName, "Name is required");
    }
    else
    {
        e.Cancel = false; // Allow focus change
        _errorProvider.SetError(txtName, string.Empty);
    }
}

private void TxtName_Validated(object sender, EventArgs e)
{
    // Called after successful validation
    _errorProvider.SetError(txtName, string.Empty);
}
```

#### Examples for Different Control Types

**TextBox Validation**
```csharp
private void TxtEmail_Validating(object sender, CancelEventArgs e)
{
    string email = txtEmail.Text.Trim();

    if (string.IsNullOrEmpty(email))
    {
        e.Cancel = true;
        _errorProvider.SetError(txtEmail, "Email is required");
    }
    else if (!IsValidEmail(email))
    {
        e.Cancel = true;
        _errorProvider.SetError(txtEmail, "Invalid email format");
    }
    else
    {
        _errorProvider.SetError(txtEmail, string.Empty);
    }
}
```

**NumericUpDown Validation**
```csharp
private void NudAge_Validating(object sender, CancelEventArgs e)
{
    if (nudAge.Value < 18)
    {
        e.Cancel = true;
        _errorProvider.SetError(nudAge, "Must be 18 or older");
    }
    else if (nudAge.Value > 120)
    {
        e.Cancel = true;
        _errorProvider.SetError(nudAge, "Invalid age");
    }
    else
    {
        _errorProvider.SetError(nudAge, string.Empty);
    }
}
```

**ComboBox Validation**
```csharp
private void CbxCountry_Validating(object sender, CancelEventArgs e)
{
    if (cbxCountry.SelectedIndex == -1)
    {
        e.Cancel = true;
        _errorProvider.SetError(cbxCountry, "Please select a country");
    }
    else
    {
        _errorProvider.SetError(cbxCountry, string.Empty);
    }
}
```

**DateTimePicker Validation**
```csharp
private void DtpBirthDate_Validating(object sender, CancelEventArgs e)
{
    var age = DateTime.Today.Year - dtpBirthDate.Value.Year;

    if (age < 18)
    {
        e.Cancel = true;
        _errorProvider.SetError(dtpBirthDate, "Must be 18 or older");
    }
    else if (dtpBirthDate.Value > DateTime.Today)
    {
        e.Cancel = true;
        _errorProvider.SetError(dtpBirthDate, "Birth date cannot be in future");
    }
    else
    {
        _errorProvider.SetError(dtpBirthDate, string.Empty);
    }
}
```

---

### 3. IDataErrorInfo Interface

The `IDataErrorInfo` interface enables automatic validation with data binding.

#### Implementation in Model Class

```csharp
public class Customer : IDataErrorInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int Age { get; set; }

    // IDataErrorInfo implementation
    public string Error
    {
        get
        {
            // Return overall validation error summary
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Name))
                errors.Add("Name is required");
            if (string.IsNullOrWhiteSpace(Email))
                errors.Add("Email is required");
            if (Age < 18)
                errors.Add("Must be 18 or older");

            return errors.Count > 0 ? string.Join("; ", errors) : string.Empty;
        }
    }

    // Property-specific validation
    public string this[string columnName]
    {
        get
        {
            switch (columnName)
            {
                case nameof(Name):
                    if (string.IsNullOrWhiteSpace(Name))
                        return "Name is required";
                    if (Name.Length < 3)
                        return "Name must be at least 3 characters";
                    break;

                case nameof(Email):
                    if (string.IsNullOrWhiteSpace(Email))
                        return "Email is required";
                    if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                        return "Invalid email format";
                    break;

                case nameof(Phone):
                    if (!string.IsNullOrWhiteSpace(Phone) &&
                        !Regex.IsMatch(Phone, @"^\d{3}-\d{3}-\d{4}$"))
                        return "Phone format: 123-456-7890";
                    break;

                case nameof(Age):
                    if (Age < 18)
                        return "Must be 18 or older";
                    if (Age > 120)
                        return "Invalid age";
                    break;
            }

            return string.Empty;
        }
    }

    public bool IsValid => string.IsNullOrEmpty(Error);
}
```

#### Using IDataErrorInfo with Binding

```csharp
public partial class CustomerForm : Form
{
    private Customer _customer;
    private BindingSource _bindingSource;
    private ErrorProvider _errorProvider;

    public CustomerForm()
    {
        InitializeComponent();

        _customer = new Customer();
        _bindingSource = new BindingSource { DataSource = _customer };
        _errorProvider = new ErrorProvider();

        // Bind controls to customer properties
        txtName.DataBindings.Add("Text", _bindingSource, nameof(Customer.Name),
            false, DataSourceUpdateMode.OnPropertyChanged);
        txtEmail.DataBindings.Add("Text", _bindingSource, nameof(Customer.Email),
            false, DataSourceUpdateMode.OnPropertyChanged);
        txtPhone.DataBindings.Add("Text", _bindingSource, nameof(Customer.Phone),
            false, DataSourceUpdateMode.OnPropertyChanged);

        // Enable automatic validation with IDataErrorInfo
        _bindingSource.DataError += BindingSource_DataError;
    }

    private void BindingSource_DataError(object sender, BindingManagerDataErrorEventArgs e)
    {
        if (e.Exception != null)
        {
            _errorProvider.SetError((Control)e.Control, e.Exception.Message);
        }
    }
}
```

---

### 4. Form-Level Validation

The `ValidateChildren` method validates all child controls.

```csharp
public partial class CustomerForm : Form
{
    private void BtnSave_Click(object sender, EventArgs e)
    {
        // Validate all child controls
        if (!ValidateChildren(ValidationConstraints.Enabled))
        {
            MessageBox.Show("Please correct the errors before saving.",
                "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // All validations passed
        SaveCustomer();
    }

    private bool ValidateForm()
    {
        // Validate all controls and collect errors
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(txtName.Text))
            errors.Add("Customer name is required");

        if (string.IsNullOrWhiteSpace(txtEmail.Text))
            errors.Add("Email is required");
        else if (!IsValidEmail(txtEmail.Text))
            errors.Add("Invalid email format");

        if (cbxCountry.SelectedIndex == -1)
            errors.Add("Country is required");

        if (errors.Count > 0)
        {
            string message = "Please correct the following errors:\n\n" +
                           string.Join("\n", errors.Select((e, i) => $"{i + 1}. {e}"));
            MessageBox.Show(message, "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        return true;
    }
}
```

---

## 🎨 Validation Patterns

### Required Field Validation

**TextBox**
```csharp
private bool ValidateRequired(TextBox textBox, string fieldName)
{
    if (string.IsNullOrWhiteSpace(textBox.Text))
    {
        _errorProvider.SetError(textBox, $"{fieldName} is required");
        return false;
    }

    _errorProvider.SetError(textBox, string.Empty);
    return true;
}

// Usage
if (!ValidateRequired(txtName, "Customer Name")) return;
```

**ComboBox**
```csharp
private bool ValidateComboBoxRequired(ComboBox comboBox, string fieldName)
{
    if (comboBox.SelectedIndex == -1)
    {
        _errorProvider.SetError(comboBox, $"{fieldName} is required");
        return false;
    }

    _errorProvider.SetError(comboBox, string.Empty);
    return true;
}
```

---

### Format Validation

**Email Validation**
```csharp
private bool ValidateEmail(string email)
{
    const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    return Regex.IsMatch(email, pattern);
}

private void TxtEmail_Validating(object sender, CancelEventArgs e)
{
    if (string.IsNullOrWhiteSpace(txtEmail.Text))
    {
        _errorProvider.SetError(txtEmail, "Email is required");
        e.Cancel = true;
    }
    else if (!ValidateEmail(txtEmail.Text))
    {
        _errorProvider.SetError(txtEmail, "Invalid email format (e.g., user@example.com)");
        e.Cancel = true;
    }
    else
    {
        _errorProvider.SetError(txtEmail, string.Empty);
    }
}
```

**Phone Number Validation**
```csharp
private bool ValidatePhoneNumber(string phone)
{
    // Format: 123-456-7890 or (123) 456-7890
    const string pattern = @"^(\d{3}-\d{3}-\d{4}|\(\d{3}\)\s\d{3}-\d{4})$";
    return Regex.IsMatch(phone, pattern);
}

private void TxtPhone_Validating(object sender, CancelEventArgs e)
{
    if (!string.IsNullOrWhiteSpace(txtPhone.Text) && !ValidatePhoneNumber(txtPhone.Text))
    {
        _errorProvider.SetError(txtPhone, "Phone format: 123-456-7890 or (123) 456-7890");
        e.Cancel = true;
    }
    else
    {
        _errorProvider.SetError(txtPhone, string.Empty);
    }
}
```

**URL Validation**
```csharp
private bool ValidateUrl(string url)
{
    return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult) &&
           (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
}
```

**Postal Code Validation**
```csharp
private bool ValidatePostalCode(string postalCode, string country)
{
    return country switch
    {
        "USA" => Regex.IsMatch(postalCode, @"^\d{5}(-\d{4})?$"),
        "Canada" => Regex.IsMatch(postalCode, @"^[A-Z]\d[A-Z]\s?\d[A-Z]\d$"),
        "UK" => Regex.IsMatch(postalCode, @"^[A-Z]{1,2}\d{1,2}\s?\d[A-Z]{2}$"),
        _ => true // Unknown country, skip validation
    };
}
```

---

### Range Validation

**Numeric Range**
```csharp
private bool ValidateRange(NumericUpDown control, decimal min, decimal max, string fieldName)
{
    if (control.Value < min || control.Value > max)
    {
        _errorProvider.SetError(control, $"{fieldName} must be between {min} and {max}");
        return false;
    }

    _errorProvider.SetError(control, string.Empty);
    return true;
}

// Usage
if (!ValidateRange(nudQuantity, 1, 1000, "Quantity")) return;
```

**Date Range**
```csharp
private void DtpStartDate_Validating(object sender, CancelEventArgs e)
{
    if (dtpStartDate.Value > dtpEndDate.Value)
    {
        _errorProvider.SetError(dtpStartDate, "Start date must be before end date");
        e.Cancel = true;
    }
    else
    {
        _errorProvider.SetError(dtpStartDate, string.Empty);
    }
}

private bool ValidateDateInPast(DateTimePicker dtp, string fieldName)
{
    if (dtp.Value > DateTime.Today)
    {
        _errorProvider.SetError(dtp, $"{fieldName} cannot be in the future");
        return false;
    }

    _errorProvider.SetError(dtp, string.Empty);
    return true;
}
```

---

### Custom Business Rules

**Cross-Field Validation**
```csharp
private bool ValidateDiscountRules()
{
    decimal orderTotal = nudOrderTotal.Value;
    decimal discount = nudDiscount.Value;

    // Business rule: Discount cannot exceed 20% of order total
    decimal maxDiscount = orderTotal * 0.20m;

    if (discount > maxDiscount)
    {
        _errorProvider.SetError(nudDiscount,
            $"Discount cannot exceed 20% of order total (${maxDiscount:F2})");
        return false;
    }

    _errorProvider.SetError(nudDiscount, string.Empty);
    return true;
}
```

**Async Validation (e.g., Check if Username Exists)**
```csharp
private async Task<bool> ValidateUsernameAvailableAsync()
{
    if (string.IsNullOrWhiteSpace(txtUsername.Text))
        return false;

    try
    {
        bool isAvailable = await _userService.IsUsernameAvailableAsync(txtUsername.Text);

        if (!isAvailable)
        {
            _errorProvider.SetError(txtUsername, "Username is already taken");
            return false;
        }

        _errorProvider.SetError(txtUsername, string.Empty);
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error validating username");
        return true; // Allow on error to not block user
    }
}

private async void TxtUsername_Leave(object sender, EventArgs e)
{
    await ValidateUsernameAvailableAsync();
}
```

---

### Real-Time vs Submit-Time Validation

**Real-Time Validation (TextChanged)**
```csharp
// Immediate feedback as user types
private void TxtEmail_TextChanged(object sender, EventArgs e)
{
    if (string.IsNullOrWhiteSpace(txtEmail.Text))
    {
        _errorProvider.SetError(txtEmail, string.Empty); // Don't show error while typing
    }
    else if (!ValidateEmail(txtEmail.Text))
    {
        _errorProvider.SetError(txtEmail, "Invalid email format");
    }
    else
    {
        _errorProvider.SetError(txtEmail, string.Empty);
    }
}
```

**On-Blur Validation (Validating Event)**
```csharp
// Validate when user leaves the field
private void TxtEmail_Validating(object sender, CancelEventArgs e)
{
    if (string.IsNullOrWhiteSpace(txtEmail.Text))
    {
        e.Cancel = true;
        _errorProvider.SetError(txtEmail, "Email is required");
    }
}
```

**When to Use Each**
- **Real-time**: Format validation (email, phone) - show errors as user types
- **On-blur**: Required fields - show error only when user leaves empty field
- **Submit-time**: Complex business rules - validate before saving

---

## 🏛️ Validation with MVP Pattern

### Where Validation Lives

**Presentation Validation (Presenter)**
- Required fields
- Format validation (email, phone)
- Range validation
- Cross-field validation

**Business Validation (Service)**
- Complex business rules
- Database constraints
- External system validation

### View Interface for Validation

```csharp
public interface ICustomerView
{
    string CustomerName { get; set; }
    string Email { get; set; }
    string Phone { get; set; }

    void ShowValidationError(string controlName, string message);
    void ClearValidationError(string controlName);
    void ShowValidationSummary(List<string> errors);

    event EventHandler SaveRequested;
}
```

### Complete MVP Validation Example

```csharp
// Presenter
public class CustomerPresenter
{
    private ICustomerView? _view;
    private readonly ICustomerService _service;

    public void AttachView(ICustomerView view)
    {
        _view = view;
        _view.SaveRequested += OnSaveRequested;
    }

    private async void OnSaveRequested(object? sender, EventArgs e)
    {
        if (_view == null) return;

        // Clear previous errors
        ClearAllValidationErrors();

        // Validate in Presenter
        var errors = ValidateForm();

        if (errors.Count > 0)
        {
            _view.ShowValidationSummary(errors);
            return;
        }

        try
        {
            var customer = new Customer
            {
                Name = _view.CustomerName,
                Email = _view.Email,
                Phone = _view.Phone
            };

            // Business validation in Service
            await _service.SaveAsync(customer);
            _view.ShowSuccess("Customer saved successfully");
        }
        catch (ValidationException vex)
        {
            _view.ShowValidationError("", vex.Message);
        }
    }

    private List<string> ValidateForm()
    {
        var errors = new List<string>();

        // Required field validation
        if (string.IsNullOrWhiteSpace(_view.CustomerName))
        {
            errors.Add("Customer name is required");
            _view.ShowValidationError(nameof(_view.CustomerName), "Required");
        }

        // Email validation
        if (string.IsNullOrWhiteSpace(_view.Email))
        {
            errors.Add("Email is required");
            _view.ShowValidationError(nameof(_view.Email), "Required");
        }
        else if (!IsValidEmail(_view.Email))
        {
            errors.Add("Invalid email format");
            _view.ShowValidationError(nameof(_view.Email), "Invalid format");
        }

        return errors;
    }

    private void ClearAllValidationErrors()
    {
        _view?.ClearValidationError(nameof(_view.CustomerName));
        _view?.ClearValidationError(nameof(_view.Email));
        _view?.ClearValidationError(nameof(_view.Phone));
    }

    private bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
}
```

---

## ✅ Best Practices

### DO:
1. ✅ **Validate early and often** - catch errors before they propagate
2. ✅ **Provide clear error messages** - tell users exactly what's wrong
3. ✅ **Use ErrorProvider** - visual feedback next to controls
4. ✅ **Validate on blur** - use Validating event for required fields
5. ✅ **Validate before save** - always validate before submitting data
6. ✅ **Use regex for formats** - email, phone, postal code validation
7. ✅ **Implement IDataErrorInfo** - automatic validation with data binding
8. ✅ **Show validation summary** - list all errors before submit
9. ✅ **Keep validation in Presenter** - separate from UI (MVP pattern)
10. ✅ **Test validation logic** - unit test all validation rules

### DON'T:
1. ❌ **Don't trust user input** - always validate
2. ❌ **Don't show errors while typing required fields** - wait for blur
3. ❌ **Don't use generic error messages** - be specific
4. ❌ **Don't validate in multiple places** - centralize validation logic
5. ❌ **Don't block UI for async validation** - use background tasks
6. ❌ **Don't ignore validation in Services** - validate business rules there
7. ❌ **Don't forget to clear errors** - clear when validation passes
8. ❌ **Don't use e.Cancel excessively** - can frustrate users
9. ❌ **Don't mix validation and business logic** - separate concerns
10. ❌ **Don't skip server-side validation** - client validation is not enough

---

## 🎨 User Experience Guidelines

### Clear Error Messages
```csharp
// ❌ BAD: Vague
"Invalid input"

// ✅ GOOD: Specific and helpful
"Email must be in format: user@example.com"
```

### Visual Indicators
- Use `ErrorProvider` for field-level errors
- Highlight invalid controls with red border (optional)
- Show error icon next to control
- Display validation summary for multiple errors

### Error Positioning
```csharp
// Position ErrorProvider icon on the right
_errorProvider.SetIconAlignment(txtEmail, ErrorIconAlignment.MiddleRight);
_errorProvider.SetIconPadding(txtEmail, 5);
```

### Helpful Hints
```csharp
// Show format hint in ToolTip
ToolTip toolTip = new ToolTip();
toolTip.SetToolTip(txtPhone, "Format: 123-456-7890");

// Show character counter for limited fields
private void TxtDescription_TextChanged(object sender, EventArgs e)
{
    int remaining = 500 - txtDescription.Text.Length;
    lblCharCount.Text = $"{remaining} characters remaining";
    lblCharCount.ForeColor = remaining < 50 ? Color.Red : Color.Gray;
}
```

---

## 📚 Related Topics

- [Error Handling & Logging](../best-practices/error-handling.md) - Exception handling patterns
- [Data Binding](data-binding.md) - Binding validation to models
- [MVP Pattern](../architecture/mvp-pattern.md) - Validation in MVP architecture
- [Form Communication](form-communication.md) - Passing validation state between forms

---

**Last Updated**: 2025-11-07
