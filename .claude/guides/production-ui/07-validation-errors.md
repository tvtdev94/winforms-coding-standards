# Validation & Error Handling

> Part of [Production UI Standards](../production-ui-standards.md)

---

## Validation Framework

```csharp
public class FormValidator
{
    private readonly ErrorProvider errorProvider;
    private readonly Dictionary<Control, List<Func<string>>> validationRules = new();

    public FormValidator(Form form)
    {
        errorProvider = new ErrorProvider(form) { BlinkStyle = ErrorBlinkStyle.NeverBlink };
    }

    public void AddRule(Control control, Func<string> rule)
    {
        if (!validationRules.ContainsKey(control))
        {
            validationRules[control] = new List<Func<string>>();
            control.Leave += (s, e) => ValidateControl(control);
        }
        validationRules[control].Add(rule);
    }

    public void AddRequired(Control control, string fieldName)
    {
        AddRule(control, () =>
        {
            var value = control switch
            {
                TextBox tb => tb.Text,
                ComboBox cb => cb.SelectedIndex > 0 ? "selected" : "",
                _ => control.Text
            };
            return string.IsNullOrWhiteSpace(value) ? $"{fieldName} is required" : null;
        });
    }

    public void AddEmail(TextBox textBox)
    {
        AddRule(textBox, () =>
        {
            if (string.IsNullOrEmpty(textBox.Text)) return null;
            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return !Regex.IsMatch(textBox.Text, emailRegex) ? "Invalid email format" : null;
        });
    }

    public void AddRange(NumericUpDown nud, decimal min, decimal max)
    {
        AddRule(nud, () =>
            nud.Value < min || nud.Value > max
                ? $"Value must be between {min} and {max}"
                : null);
    }

    private void ValidateControl(Control control)
    {
        var errors = validationRules[control]
            .Select(rule => rule())
            .Where(error => error != null)
            .ToList();

        var errorMessage = string.Join("\n", errors);
        errorProvider.SetError(control, errorMessage);

        control.BackColor = string.IsNullOrEmpty(errorMessage)
            ? SystemColors.Window
            : Color.FromArgb(255, 240, 240); // Light red
    }

    public bool ValidateAll()
    {
        var isValid = true;
        foreach (var control in validationRules.Keys)
        {
            ValidateControl(control);
            if (!string.IsNullOrEmpty(errorProvider.GetError(control)))
                isValid = false;
        }
        return isValid;
    }
}

// Usage
var validator = new FormValidator(this);
validator.AddRequired(txtName, "Name");
validator.AddEmail(txtEmail);
validator.AddRange(nudAge, 0, 120);

if (!validator.ValidateAll())
{
    ToastNotification.Show("Please fix validation errors", ToastType.Warning);
    return;
}
```

---

## Global Exception Handler

```csharp
public static class GlobalExceptionHandler
{
    public static void Initialize()
    {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += Application_ThreadException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
        HandleException(e.Exception);
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        HandleException(e.ExceptionObject as Exception);
    }

    private static void HandleException(Exception ex)
    {
        Log.Error(ex, "Unhandled exception");
        var dialog = new ErrorDialog(ex);
        dialog.ShowDialog();
    }
}
```

---

## User-Friendly Error Dialog

```csharp
public class ErrorDialog : Form
{
    public ErrorDialog(Exception ex)
    {
        Text = "An Error Occurred";
        Size = new Size(500, 300);
        StartPosition = FormStartPosition.CenterScreen;

        // User-friendly message
        var lblMessage = new Label
        {
            Text = "We apologize for the inconvenience. The error has been logged.",
            AutoSize = true
        };

        // Technical details (expandable)
        var txtDetails = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            Text = ex.ToString()
        };

        var btnCopy = new Button { Text = "Copy Details" };
        btnCopy.Click += (s, e) => Clipboard.SetText(ex.ToString());

        var btnClose = new Button { Text = "Close", DialogResult = DialogResult.OK };
    }
}
```

---

## Checklist

- [ ] Required fields validated
- [ ] Format validation (email, phone)
- [ ] Range validation where applicable
- [ ] Clear error messages (not technical)
- [ ] Validation on leave, not just submit
- [ ] ErrorProvider icons shown
- [ ] Invalid fields highlighted
- [ ] Global exception handler configured
