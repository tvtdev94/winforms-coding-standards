# Code Snippets for WinForms Development

Code snippets to dramatically speed up WinForms development with MVP pattern, async/await, and best practices.

## 📦 Available Snippets

| Shortcut | Description | Generates |
|----------|-------------|-----------|
| `mvpform` | MVP Form | Complete form with View interface and implementation |
| `mvppresenter` | MVP Presenter | Presenter with event handling and DI |
| `mvpservice` | Service Class | Service with repository, logging, and error handling |
| `mvprepo` | Repository | EF Core repository with CRUD operations |
| `asyncevent` | Async Event Handler | Button click handler with try-catch-finally |
| `invokeui` | Thread-Safe UI | InvokeRequired pattern for cross-thread UI updates |
| `mvptest` | Unit Test | xUnit test method with Arrange-Act-Assert |

---

## 🚀 Installation

### Visual Studio 2022

1. **Locate Snippets Folder**:
   ```
   %USERPROFILE%\Documents\Visual Studio 2022\Code Snippets\Visual C#\My Code Snippets\
   ```

2. **Copy Snippet Files**:
   - Copy all `.snippet` files from `snippets/visual-studio/` to the folder above
   - Or use Visual Studio's Code Snippets Manager (see below)

3. **Using Code Snippets Manager**:
   - Open Visual Studio
   - Go to **Tools** → **Code Snippets Manager** (Ctrl+K, Ctrl+B)
   - Select language: **Visual C#**
   - Click **Import**
   - Navigate to `snippets/visual-studio/` folder
   - Select all `.snippet` files
   - Click **Finish**

4. **Verify Installation**:
   - Create a new C# file
   - Type `mvpform` and press **Tab**
   - Snippet should expand!

---

### Visual Studio Code

1. **Open Snippets Configuration**:
   - Press **Ctrl+Shift+P** (Windows/Linux) or **Cmd+Shift+P** (Mac)
   - Type: `Preferences: Configure User Snippets`
   - Select: **csharp.json** (create if doesn't exist)

2. **Copy Snippet Content**:
   - Open `snippets/vscode/csharp.json`
   - Copy entire content
   - Paste into your `csharp.json` file in VS Code

3. **Verify Installation**:
   - Create a new `.cs` file
   - Type `mvpform` and press **Tab** or **Ctrl+Space**
   - Select snippet from IntelliSense
   - Snippet should expand!

---

## 📝 Usage Examples

### Example 1: Creating MVP Form

**Steps**:
1. Create new file: `CustomerForm.cs`
2. Type: `mvpform`
3. Press **Tab**
4. Enter form name: `Customer`
5. Press **Tab** to navigate to next field
6. Enter namespace: `MyApp.Forms`
7. Press **Tab** → Full MVP form generated!

**What You Get**:
```csharp
public interface ICustomerView
{
    event EventHandler? LoadRequested;
    event EventHandler? SaveRequested;
    // ... complete view interface
}

public partial class CustomerForm : Form, ICustomerView
{
    private readonly CustomerPresenter _presenter;
    // ... complete form implementation with events, properties, methods
}
```

---

### Example 2: Creating Service

**Steps**:
1. Create new file: `CustomerService.cs`
2. Type: `mvpservice`
3. Press **Tab**
4. Enter: `Customer` (generates CustomerService + ICustomerService)
5. Code ready with:
   - ✅ Constructor injection
   - ✅ Async CRUD methods
   - ✅ Logging
   - ✅ Error handling
   - ✅ XML comments

---

### Example 3: Adding Async Event Handler

**In Form Code**:
```csharp
public partial class CustomerForm : Form
{
    // Type: asyncevent [Tab]
    // Enter control: btnLoad
    // Enter action: Load

    // Generated:
    private async void btnLoad_Click(object sender, EventArgs e)
    {
        try
        {
            btnLoad.Enabled = false;
            Cursor = Cursors.WaitCursor;

            // Your code here
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Load operation failed");
            MessageBox.Show(...);
        }
        finally
        {
            btnLoad.Enabled = true;
            Cursor = Cursors.Default;
        }
    }
}
```

---

### Example 4: Thread-Safe UI Update

**Problem**: Updating UI from background thread causes `InvalidOperationException`

**Solution**:
```csharp
// Type: invokeui [Tab]
// Enter method name: UpdateStatus
// Enter parameter type: string
// Enter parameter name: message

// Generated:
private void UpdateStatus(string message)
{
    if (InvokeRequired)
    {
        Invoke(new Action<string>(UpdateStatus), message);
        return;
    }

    // Safe to update UI here
    lblStatus.Text = message; // Add your code
}

// Use from any thread:
Task.Run(() => {
    // Do work...
    UpdateStatus("Processing complete");
});
```

---

## 🎯 Productivity Impact

### Time Savings Comparison

| Task | Manual | With Snippet | Time Saved |
|------|--------|--------------|------------|
| **Create MVP Form** | 15-20 min | 30 sec | **~95%** |
| **Create Service** | 10-15 min | 20 sec | **~95%** |
| **Create Presenter** | 8-10 min | 20 sec | **~95%** |
| **Async Event Handler** | 3-5 min | 10 sec | **~95%** |
| **Thread-Safe UI** | 5 min | 10 sec | **~95%** |

**For a typical WinForms project** (10 forms, 5 services, 3 repositories):
- **Manual**: ~8-10 hours of boilerplate coding
- **With Snippets**: ~30 minutes
- **Time Saved**: ~9 hours ⚡

---

## 🔧 Customizing Snippets

### Visual Studio

Edit snippet files (`.snippet` XML):
```xml
<Literal>
  <ID>FormName</ID>
  <ToolTip>Name of the form</ToolTip>
  <Default>Customer</Default>  <!-- Change default value -->
</Literal>
```

### VS Code

Edit `csharp.json`:
```json
{
  "MVP Form": {
    "prefix": "mvpform",
    "body": [
      "public class ${1:Customer}Form : Form",
      // ... modify body as needed
    ]
  }
}
```

---

## 💡 Tips & Best Practices

### 1. Use Tab to Navigate Fields
- After expanding snippet, press **Tab** to jump between editable fields
- All instances of the same field update automatically

### 2. Combine Snippets
```csharp
// Step 1: mvpform [Tab] → Create form
// Step 2: mvppresenter [Tab] → Create presenter
// Step 3: mvpservice [Tab] → Create service
// Result: Complete MVP stack in < 2 minutes!
```

### 3. Leverage IntelliSense (VS Code)
- Press **Ctrl+Space** to see all available snippets
- Type partial name to filter

### 4. Create Project-Specific Snippets
- Add your team's specific patterns
- Share across team via source control

---

## 🆘 Troubleshooting

### Snippets Not Appearing

**Visual Studio**:
- Check: Tools → Code Snippets Manager → Visual C# → My Code Snippets
- Ensure files are in correct location
- Restart Visual Studio

**VS Code**:
- Check: File → Preferences → User Snippets → csharp.json
- Ensure valid JSON syntax (use JSON validator)
- Restart VS Code

### Snippet Doesn't Expand

**Issue**: Typed `mvpform` but nothing happens

**Solutions**:
- **Visual Studio**: Make sure you press **Tab** (not Enter)
- **VS Code**: Press **Tab** or **Ctrl+Space** to trigger
- Check if snippet is in correct language scope (C#)

### Wrong Template Variables

**Issue**: Variables like `$FormName$` appear in code

**Solution**:
- You're in wrong editor or snippet wasn't installed correctly
- Re-check installation steps

---

## 📚 Related Documentation

- [MVP Pattern Guide](../docs/architecture/mvp-pattern.md)
- [Async/Await Best Practices](../docs/best-practices/async-await.md)
- [Thread Safety](../docs/best-practices/thread-safety.md)
- [Code Templates](../templates/)

---

## 🤝 Contributing

Have a useful snippet? Please contribute!

1. Add snippet to both VS and VS Code formats
2. Update this README with usage example
3. Submit Pull Request

---

**Last Updated**: 2025-11-07
**Version**: 1.0
