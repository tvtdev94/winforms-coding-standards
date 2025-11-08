---
description: Smart bug fixer that analyzes error logs and suggests fixes
---

# Fix Bug - Smart Debugging Assistant

Analyzes error messages, stack traces, and logs to identify and fix bugs in WinForms applications.

---

## 🎯 Purpose

Fix bugs faster by automatically:
1. Parsing error logs and stack traces
2. Identifying bug type and root cause
3. Locating problematic code
4. Suggesting fixes with explanations
5. Optionally applying the fix

---

## 📝 Usage

### Format 1: Paste Error Log (Recommended)

```
Paste your error message, stack trace, or describe the bug:
```

Then paste the full error, e.g.:

```
System.NullReferenceException: Object reference not set to an instance of an object.
   at CustomerApp.Forms.CustomerForm.btnSave_Click(Object sender, EventArgs e) in C:\Projects\CustomerApp\Forms\CustomerForm.cs:line 45
   at System.Windows.Forms.Control.OnClick(EventArgs e)
```

### Format 2: File-Based Analysis

If you don't have an error log, provide:
- File path where bug occurs
- Description of the bug

```
File: CustomerForm.cs
Issue: Button click crashes with NullReferenceException
```

---

## 🔍 What I Will Do

### Step 1: Parse Error Input

**If you provide a stack trace**, I will:
- Extract exception type (NullReferenceException, InvalidOperationException, etc.)
- Extract file path and line number
- Extract method name
- Identify thread info (if cross-thread issue)

**If you provide a description**, I will:
- Search for the file
- Analyze code for common bugs
- Check against WinForms anti-patterns

### Step 2: Analyze Bug Type

I will detect common WinForms bugs:

| Bug Type | Detection Pattern | Auto-Fix |
|----------|------------------|----------|
| **NullReferenceException** | Accessing null object | Add null check ✅ |
| **Cross-thread UI access** | "Cross-thread operation not valid" | Add Invoke() ✅ |
| **ObjectDisposedException** | Using disposed object | Check disposal timing ✅ |
| **InvalidOperationException** | Invalid state | Add validation ✅ |
| **ArgumentNullException** | Null parameter | Add null guard ✅ |
| **Missing Dispose** | Resource leak | Implement IDisposable ✅ |
| **Async void** | async void method | Change to async Task ✅ |
| **Exception swallowing** | Empty catch block | Add proper logging ✅ |
| **Business logic in Form** | Form does DB/business logic | Refactor to Service ⚠️ |
| **Hardcoded strings** | Magic strings/numbers | Extract constants ⚠️ |

✅ = Can auto-fix
⚠️ = Requires manual refactoring

### Step 3: Locate Code

I will:
1. Read the file at the problematic line
2. Read surrounding context (5 lines before/after)
3. Understand the method/class structure
4. Check if it follows MVP/MVVM pattern

### Step 4: Root Cause Analysis

I will explain:
- **What** went wrong
- **Why** it happened
- **How** to prevent it in the future
- **Where** similar issues might exist

### Step 5: Suggest Fix

I will provide:
- **Current buggy code** (highlighted)
- **Fixed code** with explanation
- **Why the fix works**
- **Best practices** to follow
- **Documentation references**

### Step 6: Apply Fix (Optional)

I will ask if you want me to:
- Apply the fix automatically
- Run tests after fixing
- Check for similar issues in other files

---

## 📊 Example Session

### Example 1: NullReferenceException

**You paste**:
```
System.NullReferenceException: Object reference not set to an instance of an object.
   at CustomerForm.btnSave_Click(Object sender, EventArgs e) in CustomerForm.cs:line 45
```

**I respond**:
```
🐛 Bug Analysis
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Type: NullReferenceException
File: CustomerForm.cs:45
Method: btnSave_Click

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

📍 Buggy Code (line 45):

   43: private void btnSave_Click(object sender, EventArgs e)
   44: {
   45:     customer.Name = txtName.Text;  // ❌ 'customer' is null
   46:     _service.SaveCustomer(customer);
   47: }

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

🔍 Root Cause:
   Variable 'customer' was never initialized before accessing .Name property

✅ Suggested Fix:

   43: private void btnSave_Click(object sender, EventArgs e)
   44: {
   45:     // Validate input first
   46:     if (string.IsNullOrWhiteSpace(txtName.Text))
   47:     {
   48:         MessageBox.Show("Name is required", "Validation Error");
   49:         return;
   50:     }
   51:
   52:     // Create new customer
   53:     var customer = new Customer
   54:     {
   55:         Name = txtName.Text
   56:     };
   57:
   58:     try
   59:     {
   60:         _service.SaveCustomer(customer);
   61:         MessageBox.Show("Customer saved successfully");
   62:     }
   63:     catch (Exception ex)
   64:     {
   65:         _logger.LogError(ex, "Failed to save customer");
   66:         MessageBox.Show($"Error: {ex.Message}");
   67:     }
   68: }

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

💡 Why This Works:
   1. Input validation prevents empty names
   2. Customer object is properly initialized
   3. Error handling catches save failures
   4. User gets feedback on success/failure

📚 Reference:
   - docs/best-practices/error-handling.md
   - docs/ui-ux/input-validation.md

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Apply fix to CustomerForm.cs? (Y/n):
```

### Example 2: Cross-Thread Error

**You paste**:
```
Cross-thread operation not valid: Control 'lblStatus' accessed from a thread other than the thread it was created on.
```

**I respond**:
```
🐛 Bug Analysis
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Type: Cross-Thread UI Access (InvalidOperationException)
Issue: UI control accessed from background thread

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

🔍 Searching for 'lblStatus' in project...

Found in: CustomerForm.cs

📍 Problematic Code:

   private async void LoadData()
   {
       var data = await _service.GetDataAsync();

       lblStatus.Text = "Data loaded";  // ❌ Cross-thread access!
   }

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

✅ Suggested Fix (Option 1 - Recommended):

   private async void LoadData()
   {
       var data = await _service.GetDataAsync();

       // ✅ Use Invoke for thread-safe UI update
       if (lblStatus.InvokeRequired)
       {
           lblStatus.Invoke(() => lblStatus.Text = "Data loaded");
       }
       else
       {
           lblStatus.Text = "Data loaded";
       }
   }

✅ Better Fix (Option 2 - Extension Method):

   private async void LoadData()
   {
       var data = await _service.GetDataAsync();

       // ✅ Use helper extension
       lblStatus.SafeInvoke(() => lblStatus.Text = "Data loaded");
   }

   // Add to Utils/ControlExtensions.cs:
   public static void SafeInvoke(this Control control, Action action)
   {
       if (control.InvokeRequired)
           control.Invoke(action);
       else
           action();
   }

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

💡 Why This Happens:
   WinForms controls can only be accessed from the UI thread.
   Async methods may resume on different threads.

🔎 Checking for similar issues...
   Found 3 other potential cross-thread accesses:
   - txtResult.Text in LoadData() (line 67)
   - dgvCustomers.DataSource in RefreshGrid() (line 89)
   - btnSave.Enabled in ProcessAsync() (line 112)

📚 Reference:
   - docs/best-practices/thread-safety.md
   - Use /fix:threading for comprehensive thread safety review

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Apply fixes? (Y/n):
```

### Example 3: ObjectDisposedException

**You paste**:
```
System.ObjectDisposedException: Cannot access a disposed object.
Object name: 'SqlConnection'.
   at CustomerRepository.GetCustomers() in CustomerRepository.cs:line 28
```

**I detect and fix**:
- Connection disposed too early
- Missing `using` statement
- Suggest proper disposal pattern

---

## 🎓 Learning Mode

After fixing each bug, I provide:

1. **Prevention Tips**: How to avoid this in the future
2. **Related Patterns**: Best practices that prevent this class of bugs
3. **Code Review Checklist**: What to check before committing
4. **Similar Issues**: Where else this might occur

---

## 🔧 Integration with Other Commands

This command works together with:

- **`/fix:threading`** - For comprehensive thread safety review
- **`/fix:performance`** - If bug is performance-related
- **`/review-code`** - For full code quality review after fixing

---

## 🚨 When to Use This vs Other Commands

| Scenario | Use This | Use Instead |
|----------|----------|-------------|
| Have error log/stack trace | ✅ `/fix:bug` | - |
| Cross-thread issues only | Use this OR | `/fix:threading` |
| Performance problems only | Use this OR | `/fix:performance` |
| General code review | No | `/review-code` |
| Architecture issues | No | Talk to me directly |

---

## ⚡ Quick Start

1. Copy your error message/stack trace
2. Run this command
3. Paste the error
4. Review suggested fix
5. Apply fix (Y/n)
6. Verify fix works

---

**Last Updated**: 2025-11-08
**Version**: 1.0
