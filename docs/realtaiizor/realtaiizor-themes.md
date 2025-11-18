# ReaLTaiizor Themes Guide

> **Purpose**: Complete guide to ReaLTaiizor themes and customization
> **Audience**: WinForms developers choosing and applying themes

---

## 📋 Available Themes

ReaLTaiizor provides **20+ professionally designed themes**. Choose ONE for your entire application.

### Theme Overview

| Theme | Style | Color Scheme | Best For |
|-------|-------|--------------|----------|
| **Material** | Google Material Design | Modern, colorful | Business apps, dashboards |
| **Metro** | Microsoft Fluent/Metro | Clean, minimalist | Windows-style apps |
| **Poison** | Flat minimalist | Soft, muted | Simple, elegant apps |
| **Crown** | Modern professional | Rich, premium | Corporate applications |
| **Cyber** | Futuristic neon | Dark, electric | Gaming, tech apps |
| **Parrot** | Vibrant colorful | Bright, lively | Creative apps |
| **Air** | Light airy | Soft pastels | Lightweight apps |
| **Dungeon** | Dark gaming | Deep, mysterious | Dark mode apps |
| **Dream** | Soft gradients | Dreamy, smooth | Modern artistic apps |
| **Night** | Dark professional | Dark, calm | Night mode apps |

---

## Material Theme

### Example

```csharp
using ReaLTaiizor.Forms;
using ReaLTaiizor.Controls;

public partial class MaterialDemoForm : MaterialForm
{
    public MaterialDemoForm()
    {
        InitializeComponent();
        SetupMaterialTheme();
    }

    private void SetupMaterialTheme()
    {
        // Material properties
        this.Text = "Material Design App";
        this.MaximizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;
    }
}
```

---

## Metro Theme

### Example

```csharp
using ReaLTaiizor.Forms;
using ReaLTaiizor.Controls;

public partial class MetroDemoForm : MetroForm
{
    public MetroDemoForm()
    {
        InitializeComponent();
        SetupMetroTheme();
    }

    private void SetupMetroTheme()
    {
        // Metro style
        this.Style = ReaLTaiizor.Enum.Metro.Style.Light;
        this.Text = "Metro Style App";
    }
}
```

---

## Poison Theme

### Example

```csharp
using ReaLTaiizor.Forms;
using ReaLTaiizor.Controls;

public partial class PoisonDemoForm : PoisonForm
{
    public PoisonDemoForm()
    {
        InitializeComponent();
        SetupPoisonTheme();
    }

    private void SetupPoisonTheme()
    {
        // Poison minimalist theme
        this.Text = "Poison Minimalist App";
    }
}
```

---

## Best Practices

### ✅ DO

1. **Choose ONE theme** for entire application
2. **Use theme consistently** across all forms
3. **Test theme** on different resolutions
4. **Consider target audience** when choosing theme

### ❌ DON'T

1. ❌ Mix multiple themes in same app
2. ❌ Change theme mid-application
3. ❌ Mix ReaLTaiizor with standard controls

---

## Theme Selection Guide

Choose based on your application type:

- **Business/Corporate** → Material or Crown
- **Windows-like** → Metro
- **Minimalist** → Poison
- **Gaming** → Cyber or Dungeon
- **Creative** → Parrot or Dream
- **Dark Mode** → Night or Lost

---

**Last Updated**: 2025-11-17
