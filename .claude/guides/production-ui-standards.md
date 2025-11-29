# Production-Level UI Standards for WinForms

> **Purpose**: Ensure AI generates production-quality UI, not student-level demos

---

## ⚠️ CRITICAL: This is NOT Optional

Every UI element you create MUST follow these standards. A DataGridView without sorting/filtering/paging is **UNACCEPTABLE**. A button that blends into the background is **UNACCEPTABLE**.

**Before completing ANY UI task, verify against the [Production Checklist](production-ui/CHECKLIST.md).**

---

## 🚨 CRITICAL RULE: UI Control Consistency

**NEVER mix different UI control libraries in the same application!**

| Project Type | Use ONLY |
|-------------|----------|
| **Standard WinForms** | TextBox, Button, DataGridView |
| **DevExpress** | TextEdit, SimpleButton, GridControl |
| **ReaLTaiizor** | MaterialTextBox, MaterialButton |

---

## 📚 Detailed Guides

| Guide | Topics |
|-------|--------|
| [01. Color & Theming](production-ui/01-color-theming.md) | Color palettes, contrast, theming |
| [02. Data Display](production-ui/02-data-display.md) | DataGridView, sorting, filtering, paging |
| [03. Input Controls](production-ui/03-input-controls.md) | Floating labels, TextBox, ComboBox, DateTimePicker |
| [04. Buttons & Actions](production-ui/04-buttons-actions.md) | Button styling, loading states, confirmations |
| [05. Layout & Responsive](production-ui/05-layout-responsive.md) | Form layout, Anchor/Dock, responsive design |
| [06. Feedback & Status](production-ui/06-feedback-status.md) | Toast notifications, status bar, loading |
| [07. Validation & Errors](production-ui/07-validation-errors.md) | Form validation, error handling |
| [08. Accessibility](production-ui/08-accessibility.md) | Keyboard nav, screen readers, WCAG |
| [09. Performance](production-ui/09-performance.md) | Async loading, virtual mode, double buffering |
| [**CHECKLIST**](production-ui/CHECKLIST.md) | Quick reference checklist |

---

## 🎯 Key Rules Summary

### Data Display
- Grid MUST use `Dock = DockStyle.Fill`
- Grid MUST have sorting, filtering, paging
- Grid MUST show empty state & loading indicator

### Input Controls
- ALWAYS use Floating Label (Material Design)
- NEVER create separate Label + TextBox
- ComboBox MUST have "-- Select --" placeholder

### Buttons
- Primary button MUST have high contrast
- Loading state MUST show during async operations
- Destructive actions MUST require confirmation

### Layout
- Form MUST have `MinimumSize` set
- Form MUST start maximized
- Form MUST remember window state

### Validation
- Validate on leave, not just submit
- Show ErrorProvider icons
- Highlight invalid fields with red background

### Accessibility
- Set logical TabIndex on all controls
- Set AccessibleName on all controls
- Provide keyboard shortcuts (Ctrl+S, Escape, etc.)

---

## Quick Reference: Common Mistakes

| ❌ Mistake | ✅ Production Standard |
|-----------|------------------------|
| Blank grid when no data | "No records found" with icon |
| Button same color as background | High contrast, min 4.5:1 ratio |
| No loading indicator | Spinner for any operation >200ms |
| Separate Label + TextBox | Floating Label (Material Design) |
| Fixed-size form | Anchor/Dock for responsive resize |
| No tab order | Logical TabIndex for all controls |
| Synchronous data loading | async/await with cancellation |
| No paging for 1000+ records | Paging with page size selector |

---

## 📋 Before You Submit

**Always run through the [Production Checklist](production-ui/CHECKLIST.md) before completing any UI task.**

If any item is missing, the UI is NOT production-ready.
