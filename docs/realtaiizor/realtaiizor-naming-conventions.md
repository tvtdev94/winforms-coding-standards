# ReaLTaiizor Naming Conventions

> **Purpose**: Standard naming conventions for ReaLTaiizor controls
> **Audience**: WinForms developers using ReaLTaiizor

---

## Naming Principles

### General Rules

1. **Use prefixes** to identify control types
2. **Use PascalCase** for control names (after prefix)
3. **Be descriptive** but concise
4. **Stay consistent** with chosen theme

### Format

```
[prefix][EntityName][Property/Purpose]
```

---

## Control Prefixes

### Material Theme

| Control | Prefix | Example |
|---------|--------|---------|
| MaterialButton | `btn` | `btnSave`, `btnCancel` |
| MaterialTextBox | `txt` | `txtName`, `txtEmail` |
| MaterialComboBox | `cbo` | `cboType`, `cboStatus` |
| MaterialListView | `lv` | `lvCustomers`, `lvOrders` |
| MaterialCheckBox | `chk` | `chkActive`, `chkAgree` |
| MaterialRadioButton | `radio` | `radioMale`, `radioFemale` |
| MaterialLabel | `lbl` | `lblTitle`, `lblStatus` |
| MaterialProgressBar | `progress` | `progressLoad` |

### Metro Theme

| Control | Prefix | Example |
|---------|--------|---------|
| MetroButton | `btn` | `btnAction`, `btnSubmit` |
| MetroTextBox | `txt` | `txtInput`, `txtData` |
| MetroComboBox | `cbo` | `cboOptions`, `cboList` |
| MetroGrid | `grid` | `gridData`, `gridCustomers` |
| MetroCheckBox | `chk` | `chkEnabled`, `chkOption` |
| MetroLabel | `lbl` | `lblInfo`, `lblMessage` |
| MetroProgressBar | `progress` | `progressBar` |

### Poison Theme

| Control | Prefix | Example |
|---------|--------|---------|
| PoisonButton | `btn` | `btnSubmit`, `btnClose` |
| PoisonTextBox | `txt` | `txtSearch`, `txtFilter` |
| PoisonComboBox | `cbo` | `cboCategory` |
| PoisonCheckBox | `chk` | `chkShow` |
| PoisonLabel | `lbl` | `lblHeader` |

---

## Examples

### Material Form

```csharp
public partial class CustomerEditForm : MaterialForm
{
    // Material controls
    private MaterialTextBox txtCustomerName;
    private MaterialTextBox txtEmail;
    private MaterialTextBox txtPhone;
    private MaterialComboBox cboCustomerType;
    private MaterialCheckBox chkIsActive;
    private MaterialButton btnSave;
    private MaterialButton btnCancel;
    private MaterialLabel lblTitle;
    private MaterialProgressBar progressSave;
}
```

### Metro Form

```csharp
public partial class CustomerListForm : MetroForm
{
    // Metro controls
    private MetroGrid gridCustomers;
    private MetroTextBox txtSearch;
    private MetroComboBox cboFilter;
    private MetroButton btnNew;
    private MetroButton btnEdit;
    private MetroButton btnDelete;
    private MetroLabel lblStatus;
}
```

---

## Best Practices

### ✅ DO

1. **Use consistent prefixes** across project
2. **Be descriptive** - `txtCustomerEmail` not `txt1`
3. **Match theme** - MaterialButton → `btn`
4. **Follow PascalCase** after prefix

### ❌ DON'T

1. ❌ Use default names (`materialButton1`)
2. ❌ Mix naming conventions
3. ❌ Use unclear abbreviations

---

## Quick Reference

**Common Prefixes**:
- `btn` - Buttons (all themes)
- `txt` - TextBox (all themes)
- `cbo` - ComboBox (all themes)
- `lv` - ListView (Material)
- `grid` - Grid (Metro)
- `chk` - CheckBox (all themes)
- `lbl` - Label (all themes)
- `progress` - ProgressBar (all themes)

---

**Last Updated**: 2025-11-17
