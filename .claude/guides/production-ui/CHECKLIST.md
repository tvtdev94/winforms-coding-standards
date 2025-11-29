# Production UI Checklist

> **Use this checklist before completing ANY UI task**

---

## Data Display ✓
- [ ] DataGridView has sortable columns
- [ ] DataGridView has filtering capability
- [ ] DataGridView has paging for large datasets
- [ ] DataGridView has alternating row colors
- [ ] DataGridView shows empty state when no data
- [ ] DataGridView shows loading indicator
- [ ] DataGridView has context menu
- [ ] DataGridView supports keyboard navigation
- [ ] Export to Excel/CSV available

## Input Controls ✓
- [ ] All text fields have placeholder/hint
- [ ] Required fields marked with asterisk (*)
- [ ] Character count for limited fields
- [ ] ComboBox has "-- Select --" placeholder
- [ ] DateTimePicker supports null/clear
- [ ] Validation shows ErrorProvider icons
- [ ] Invalid fields highlighted (red border)

## Buttons ✓
- [ ] Primary button has high contrast
- [ ] Buttons have hover effects
- [ ] Buttons have disabled styling
- [ ] Loading state during async operations
- [ ] Destructive actions require confirmation
- [ ] Double-click prevented
- [ ] Keyboard shortcuts work (Alt+key)

## Layout ✓
- [ ] Form has MinimumSize set
- [ ] Form resizes properly (Anchor/Dock)
- [ ] Window position/size remembered
- [ ] Consistent spacing between controls
- [ ] Status bar shows record count

## Feedback ✓
- [ ] Loading indicators for operations >200ms
- [ ] Success/error toast notifications
- [ ] Status bar updates during operations
- [ ] Progress bar for long operations

## Validation ✓
- [ ] Required fields validated
- [ ] Format validation (email, phone)
- [ ] Range validation where applicable
- [ ] Clear error messages (not technical)
- [ ] Validation on leave, not just submit

## Accessibility ✓
- [ ] Logical tab order (TabIndex)
- [ ] All controls have AccessibleName
- [ ] Keyboard shortcuts for common actions
- [ ] Focus indicators visible
- [ ] Color contrast meets WCAG 4.5:1

## Performance ✓
- [ ] Async/await for all I/O
- [ ] Virtual mode for 1000+ rows
- [ ] Double buffering enabled
- [ ] No blocking UI thread

## Colors ✓
- [ ] Using defined color palette
- [ ] Buttons contrast with background
- [ ] Semantic colors consistent
- [ ] Text readable on all backgrounds

## Polish ✓
- [ ] Consistent icons
- [ ] Professional fonts
- [ ] Proper alignment
- [ ] Dirty state indicator (*)

---

## Quick Reference: Common Mistakes

| ❌ Mistake | ✅ Production Standard |
|-----------|------------------------|
| Blank grid | "No records found" message |
| Button same as background | High contrast, 4.5:1 ratio |
| No loading indicator | Spinner for >200ms operations |
| "An error occurred" | Specific, actionable message |
| Fixed-size form | Anchor/Dock responsive |
| No tab order | Logical TabIndex |
| Random colors | Defined palette |
| No paging | Page for 100+ records |
| No sorting | Click-to-sort columns |
