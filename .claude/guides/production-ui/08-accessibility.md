# Accessibility (WCAG)

> Part of [Production UI Standards](../production-ui-standards.md)

---

## Keyboard Navigation

```csharp
public static class AccessibilityHelper
{
    // Set logical tab order
    public static void SetTabOrder(params Control[] controls)
    {
        for (int i = 0; i < controls.Length; i++)
        {
            controls[i].TabIndex = i;
        }
    }

    // Add keyboard shortcuts
    public static void AddShortcut(Form form, Keys keys, Action action)
    {
        form.KeyPreview = true;
        form.KeyDown += (s, e) =>
        {
            if (e.KeyData == keys)
            {
                action();
                e.Handled = true;
            }
        };
    }
}

// Usage
AccessibilityHelper.SetTabOrder(txtName, txtEmail, txtPhone, cboCountry, btnSave, btnCancel);
AccessibilityHelper.AddShortcut(this, Keys.Control | Keys.S, () => Save());
AccessibilityHelper.AddShortcut(this, Keys.Escape, () => Close());
```

---

## Screen Reader Support

```csharp
public static void SetAccessibleProperties(Control control, string name, string description = null)
{
    control.AccessibleName = name;
    if (description != null)
        control.AccessibleDescription = description;
}

// Usage
SetAccessibleProperties(txtEmail, "Email address", "Enter your email for account recovery");
SetAccessibleProperties(btnSave, "Save", "Save the current form data");
SetAccessibleProperties(dgvCustomers, "Customer list", "List of all customers");
```

---

## High Contrast Support

```csharp
public static bool IsHighContrastMode => SystemInformation.HighContrast;

public void ApplyHighContrastIfNeeded()
{
    if (IsHighContrastMode)
    {
        // Use system colors instead of custom colors
        foreach (Control control in Controls)
        {
            control.BackColor = SystemColors.Window;
            control.ForeColor = SystemColors.WindowText;
        }
    }
}
```

---

## Focus Indicators

```csharp
// Ensure focus is visible
public void EnhanceFocusIndicators()
{
    foreach (Control control in GetAllControls(this))
    {
        control.GotFocus += (s, e) =>
        {
            if (control is TextBox tb)
                tb.BackColor = AppColors.Selected;
        };

        control.LostFocus += (s, e) =>
        {
            if (control is TextBox tb)
                tb.BackColor = SystemColors.Window;
        };
    }
}
```

---

## Required Shortcuts

| Action | Shortcut |
|--------|----------|
| Save | Ctrl+S |
| Close/Cancel | Escape |
| New | Ctrl+N |
| Delete | Delete |
| Refresh | F5 |
| Help | F1 |
| Edit | F2 |
| Search/Find | Ctrl+F |

---

## Checklist

- [ ] Logical tab order (TabIndex)
- [ ] All controls have AccessibleName
- [ ] Keyboard shortcuts for common actions
- [ ] Focus indicators visible
- [ ] Color contrast meets WCAG 4.5:1
- [ ] High contrast mode supported
- [ ] Screen reader compatible
