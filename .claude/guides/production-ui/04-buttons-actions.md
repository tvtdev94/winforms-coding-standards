# Buttons and Actions

> Part of Production UI Standards

---

## Button Styling Standards

```csharp
public enum ButtonType { Primary, Secondary, Success, Danger, Warning, Link }

public static class ButtonStyles
{
    public static void ApplyStyle(Button btn, ButtonType type)
    {
        // Base styling
        btn.FlatStyle = FlatStyle.Flat;
        btn.Cursor = Cursors.Hand;
        btn.Font = new Font("Segoe UI", 9f);
        btn.Padding = new Padding(15, 8, 15, 8);
        btn.MinimumSize = new Size(80, 32);

        switch (type)
        {
            case ButtonType.Primary:
                btn.BackColor = AppColors.Primary;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.MouseOverBackColor = AppColors.PrimaryDark;
                break;

            case ButtonType.Secondary:
                btn.BackColor = AppColors.Surface;
                btn.ForeColor = AppColors.TextPrimary;
                btn.FlatAppearance.BorderColor = AppColors.Border;
                break;

            case ButtonType.Success:
                btn.BackColor = AppColors.Success;
                btn.ForeColor = Color.White;
                break;

            case ButtonType.Danger:
                btn.BackColor = AppColors.Danger;
                btn.ForeColor = Color.White;
                break;

            case ButtonType.Link:
                btn.BackColor = Color.Transparent;
                btn.ForeColor = AppColors.Primary;
                btn.FlatAppearance.BorderSize = 0;
                break;
        }
    }

    public static void ApplyDisabledState(Button btn)
    {
        btn.BackColor = AppColors.Disabled;
        btn.ForeColor = AppColors.TextMuted;
        btn.Cursor = Cursors.No;
    }
}
```

---

## Loading State

```csharp
public static class ButtonExtensions
{
    private static readonly Dictionary<Button, (string Text, bool Enabled)> OriginalStates = new();

    public static void SetLoading(this Button btn, bool isLoading, string loadingText = "Loading...")
    {
        if (isLoading)
        {
            OriginalStates[btn] = (btn.Text, btn.Enabled);
            btn.Text = loadingText;
            btn.Enabled = false;
            btn.Cursor = Cursors.WaitCursor;
        }
        else
        {
            if (OriginalStates.TryGetValue(btn, out var state))
            {
                btn.Text = state.Text;
                btn.Enabled = state.Enabled;
                btn.Cursor = Cursors.Hand;
                OriginalStates.Remove(btn);
            }
        }
    }
}

// Usage
private async void btnSave_Click(object sender, EventArgs e)
{
    btnSave.SetLoading(true, "Saving...");
    try
    {
        await _presenter.SaveAsync();
    }
    finally
    {
        btnSave.SetLoading(false);
    }
}
```

---

## Double-Click Prevention

```csharp
public class SafeButton : Button
{
    private DateTime lastClick = DateTime.MinValue;
    private readonly int clickDelayMs = 500;

    protected override void OnClick(EventArgs e)
    {
        if ((DateTime.Now - lastClick).TotalMilliseconds < clickDelayMs)
            return;

        lastClick = DateTime.Now;
        base.OnClick(e);
    }
}
```

---

## Confirmation for Destructive Actions

```csharp
public static async Task<bool> ConfirmDangerousAction(string message, string title = "Confirm Action")
{
    var result = MessageBox.Show(
        message,
        title,
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Warning,
        MessageBoxDefaultButton.Button2); // Default to No

    return result == DialogResult.Yes;
}

// Usage
private async void btnDelete_Click(object sender, EventArgs e)
{
    if (!await ConfirmDangerousAction("Delete this item? This cannot be undone."))
        return;

    await _presenter.DeleteAsync();
}
```

---

## Checklist

- [ ] Primary button has high contrast (not same as background)
- [ ] Buttons have hover effects
- [ ] Buttons have disabled styling
- [ ] Loading state during async operations
- [ ] Destructive actions require confirmation
- [ ] Double-click prevented
- [ ] Keyboard shortcuts work (Alt+key)
