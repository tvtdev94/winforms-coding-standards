---
description: Add comprehensive input validation to a WinForms form
---

You are tasked with adding comprehensive input validation to a WinForms form.

## Workflow

1. **Ask the user**:
   - Which form file needs validation?
   - What fields need validation?
   - What validation rules apply? (required, email, phone, range, regex, etc.)

2. **Read the form file** to understand current structure

3. **Add validation implementation**:
   - Add ErrorProvider component if not exists
   - Create validation methods for each field
   - Add real-time validation on TextChanged/Validating events
   - Add form-level validation before save/submit
   - Display clear error messages

4. **Follow these patterns**:

### ErrorProvider Setup
```csharp
private ErrorProvider errorProvider;

public MyForm()
{
    InitializeComponent();
    errorProvider = new ErrorProvider
    {
        BlinkStyle = ErrorBlinkStyle.NeverBlink
    };
}
```

### Field Validation
```csharp
private void txtEmail_Validating(object sender, CancelEventArgs e)
{
    string email = txtEmail.Text.Trim();

    if (string.IsNullOrWhiteSpace(email))
    {
        errorProvider.SetError(txtEmail, "Email is required");
        e.Cancel = true;
    }
    else if (!IsValidEmail(email))
    {
        errorProvider.SetError(txtEmail, "Please enter a valid email address");
        e.Cancel = true;
    }
    else
    {
        errorProvider.SetError(txtEmail, string.Empty);
    }
}

private bool IsValidEmail(string email)
{
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
```

### Form-Level Validation
```csharp
private bool ValidateForm()
{
    bool isValid = true;
    errorProvider.Clear();

    // Validate each field
    if (string.IsNullOrWhiteSpace(txtName.Text))
    {
        errorProvider.SetError(txtName, "Name is required");
        isValid = false;
    }

    if (string.IsNullOrWhiteSpace(txtEmail.Text) || !IsValidEmail(txtEmail.Text))
    {
        errorProvider.SetError(txtEmail, "Valid email is required");
        isValid = false;
    }

    // Add more validations...

    return isValid;
}

private async void btnSave_Click(object sender, EventArgs e)
{
    if (!ValidateForm())
    {
        MessageBox.Show("Please correct the errors before saving.",
            "Validation Error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
        return;
    }

    // Proceed with save...
}
```

5. **Validation Rules to Implement**:
   - Required fields
   - Email format validation
   - Phone number format
   - Numeric range validation
   - String length limits
   - Custom regex patterns
   - Date range validation
   - Dependent field validation

6. **Best Practices**:
   - ✅ Clear error messages
   - ✅ Real-time validation on blur/change
   - ✅ Form-level validation before submit
   - ✅ Use ErrorProvider for visual feedback
   - ✅ Don't allow invalid data submission
   - ✅ Clear errors when corrected
   - ✅ Focus on first error field

7. **Show the user**:
   - Updated form code with validation
   - List of validation rules added
   - Offer to add more validations if needed
