# Color Palette & Theming

> Part of Production UI Standards

---

## Professional Color Schemes

**NEVER use random colors. ALWAYS use a defined palette.**

### Modern Professional (Default)

```csharp
public static class AppColors
{
    // Primary colors
    public static readonly Color Primary = Color.FromArgb(41, 128, 185);        // #2980B9
    public static readonly Color PrimaryDark = Color.FromArgb(31, 97, 141);     // #1F618D
    public static readonly Color PrimaryLight = Color.FromArgb(174, 214, 241);  // #AED6F1

    // Secondary colors
    public static readonly Color Secondary = Color.FromArgb(155, 89, 182);      // #9B59B6
    public static readonly Color SecondaryLight = Color.FromArgb(215, 189, 226);

    // Semantic colors
    public static readonly Color Success = Color.FromArgb(39, 174, 96);         // #27AE60
    public static readonly Color Warning = Color.FromArgb(241, 196, 15);        // #F1C40F
    public static readonly Color Danger = Color.FromArgb(231, 76, 60);          // #E74C3C
    public static readonly Color Info = Color.FromArgb(52, 152, 219);           // #3498DB

    // Neutral colors
    public static readonly Color Background = Color.FromArgb(248, 249, 250);    // #F8F9FA
    public static readonly Color Surface = Color.FromArgb(255, 255, 255);       // #FFFFFF
    public static readonly Color Border = Color.FromArgb(222, 226, 230);        // #DEE2E6
    public static readonly Color TextPrimary = Color.FromArgb(33, 37, 41);      // #212529
    public static readonly Color TextSecondary = Color.FromArgb(108, 117, 125); // #6C757D
    public static readonly Color TextMuted = Color.FromArgb(173, 181, 189);     // #ADB5BD

    // States
    public static readonly Color Hover = Color.FromArgb(233, 236, 239);         // #E9ECEF
    public static readonly Color Selected = Color.FromArgb(209, 236, 241);      // #D1ECF1
    public static readonly Color Disabled = Color.FromArgb(206, 212, 218);      // #CED4DA
}
```

### Dark Theme

```csharp
public static class DarkColors
{
    public static readonly Color Primary = Color.FromArgb(100, 181, 246);       // #64B5F6
    public static readonly Color Background = Color.FromArgb(18, 18, 18);       // #121212
    public static readonly Color Surface = Color.FromArgb(30, 30, 30);          // #1E1E1E
    public static readonly Color Border = Color.FromArgb(66, 66, 66);           // #424242
    public static readonly Color TextPrimary = Color.FromArgb(255, 255, 255);   // #FFFFFF
    public static readonly Color TextSecondary = Color.FromArgb(179, 179, 179); // #B3B3B3
}
```

---

## Color Contrast Requirements (WCAG 2.1)

| Element Type | Minimum Ratio |
|-------------|---------------|
| Normal text (< 18pt) | 4.5:1 |
| Large text (≥ 18pt) | 3:1 |
| UI components | 3:1 |
| Focus indicators | 3:1 |

```csharp
public static double GetContrastRatio(Color foreground, Color background)
{
    double L1 = GetRelativeLuminance(foreground);
    double L2 = GetRelativeLuminance(background);
    return (Math.Max(L1, L2) + 0.05) / (Math.Min(L1, L2) + 0.05);
}
```

---

## Color Usage Rules

### ✅ DO:
- Use semantic colors consistently (Success = green, Danger = red)
- Provide sufficient contrast
- Use color + icon (for colorblind users)
- Use 60-30-10 rule (60% neutral, 30% secondary, 10% accent)
- Define color constants - never hardcode hex inline

### ❌ DON'T:
- Button same color as background
- Light gray text on white background
- Random colors for each screen
- Too many colors (max 5-6)
- Neon/fluorescent colors

---

## Control-Specific Guidelines

```csharp
// Button styling
public void ApplyButtonStyles(Button btn, ButtonType type)
{
    btn.FlatStyle = FlatStyle.Flat;
    btn.Cursor = Cursors.Hand;

    switch (type)
    {
        case ButtonType.Primary:
            btn.BackColor = AppColors.Primary;
            btn.ForeColor = Color.White;
            break;
        case ButtonType.Danger:
            btn.BackColor = AppColors.Danger;
            btn.ForeColor = Color.White;
            break;
    }
}

// DataGridView styling
public void ApplyGridStyles(DataGridView dgv)
{
    dgv.BackgroundColor = AppColors.Surface;
    dgv.GridColor = AppColors.Border;
    dgv.ColumnHeadersDefaultCellStyle.BackColor = AppColors.Primary;
    dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
    dgv.AlternatingRowsDefaultCellStyle.BackColor = AppColors.Background;
}
```
