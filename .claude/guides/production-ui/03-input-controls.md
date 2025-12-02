# Input Controls

> Part of Production UI Standards

---

## Floating Label (Material Design)

### ReaLTaiizor

```csharp
var txtEmail = new MaterialTextBoxEdit
{
    Hint = "Email Address",           // Floating label
    UseAccent = true,
    ShowAssistiveText = true,
    HelperText = "Enter your email"   // Helper text below
};

var txtPassword = new MaterialTextBoxEdit
{
    Hint = "Password",
    Password = true,
    HelperText = "Minimum 8 characters"
};
```

### DevExpress

```csharp
var txtEmail = new TextEdit();
txtEmail.Properties.NullValuePrompt = "Email Address";
txtEmail.Properties.NullValuePromptShowForEmptyValue = true;
```

### Standard WinForms (Custom)

```csharp
public class FloatingLabelTextBox : UserControl
{
    public string Hint { get; set; } = "Label";

    private void AnimateLabelUp()
    {
        floatingLabel.Font = new Font("Segoe UI", 7.5f);
        floatingLabel.ForeColor = AppColors.Primary;
        floatingLabel.Top = 0;
    }

    private void AnimateLabelDown()
    {
        floatingLabel.Font = new Font("Segoe UI", 9.5f);
        floatingLabel.ForeColor = AppColors.TextMuted;
        floatingLabel.Top = textBox.Top;
    }
}
```

---

## TextBox Standards

```csharp
public class ProductionTextBox : TextBox
{
    public int? MaxLength { get; set; }
    public bool ShowCharacterCount { get; set; } = true;
    public bool Required { get; set; }

    public ProductionTextBox()
    {
        BorderStyle = BorderStyle.FixedSingle;
        Font = new Font("Segoe UI", 9.5f);

        TextChanged += OnTextChanged;
        Enter += (s, e) => BackColor = AppColors.Selected;
        Leave += (s, e) => BackColor = SystemColors.Window;
    }

    private void OnTextChanged(object sender, EventArgs e)
    {
        if (ShowCharacterCount && MaxLength.HasValue)
        {
            lblCharCount.Text = $"{Text.Length}/{MaxLength}";
        }
    }
}
```

---

## ComboBox Standards

```csharp
public class ProductionComboBox : ComboBox
{
    private const string PlaceholderText = "-- Select --";

    public ProductionComboBox()
    {
        DropDownStyle = ComboBoxStyle.DropDownList;
        Font = new Font("Segoe UI", 9.5f);
        Items.Add(PlaceholderText);
        SelectedIndex = 0;
    }

    public bool HasSelection => SelectedIndex > 0;

    public void LoadItems<T>(IEnumerable<T> items, string displayMember, string valueMember)
    {
        Items.Clear();
        Items.Add(PlaceholderText);
        foreach (var item in items) Items.Add(item);
        DisplayMember = displayMember;
        ValueMember = valueMember;
        SelectedIndex = 0;
    }
}

// Searchable ComboBox
public class SearchableComboBox : ComboBox
{
    public SearchableComboBox()
    {
        DropDownStyle = ComboBoxStyle.DropDown;
        AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        AutoCompleteSource = AutoCompleteSource.ListItems;
    }
}
```

---

## DateTimePicker - Nullable Support

```csharp
public class NullableDateTimePicker : DateTimePicker
{
    private bool isNull = true;

    public new DateTime? Value
    {
        get => isNull ? null : base.Value;
        set
        {
            if (value.HasValue)
            {
                base.Value = value.Value;
                isNull = false;
                Format = originalFormat;
            }
            else
            {
                isNull = true;
                Format = DateTimePickerFormat.Custom;
                CustomFormat = " "; // Blank display
            }
        }
    }

    public NullableDateTimePicker()
    {
        KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                Value = null;
                e.Handled = true;
            }
        };
    }
}
```

---

## Checklist

- [ ] All text fields have placeholder/hint
- [ ] Required fields marked with asterisk (*)
- [ ] Character count for limited fields
- [ ] ComboBox has "-- Select --" placeholder
- [ ] DateTimePicker supports null/clear
- [ ] Validation shows ErrorProvider icons
- [ ] Invalid fields have red border
