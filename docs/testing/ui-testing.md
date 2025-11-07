# UI Testing in WinForms

## 📋 Overview

UI (User Interface) testing, also known as **UI automation testing** or **end-to-end testing**, validates your WinForms application from the user's perspective. Unlike unit tests that test individual components, UI tests interact with your application as a real user would—clicking buttons, entering text, and verifying the results displayed on screen.

**Key Concepts**:
- **Automated UI testing**: Tests that programmatically control your application's UI
- **End-to-end testing**: Testing complete user workflows from start to finish
- **UI Automation**: Windows technology for programmatic access to UI elements
- **Regression testing**: Ensuring new changes don't break existing functionality

---

## 🎯 Why This Matters

### Benefits of UI Testing

1. **End-to-End Validation**: Tests complete user workflows, catching integration issues
2. **User Perspective**: Validates what users actually see and interact with
3. **Regression Detection**: Catches UI bugs introduced by code changes
4. **Documentation**: Tests serve as executable documentation of user workflows
5. **Confidence**: Provides confidence that critical paths work correctly

### Business Value

- **Reduced Manual Testing**: Automate repetitive test scenarios
- **Faster Feedback**: Catch UI bugs before production
- **Quality Assurance**: Ensure consistent user experience
- **Cost Savings**: Find bugs early when they're cheaper to fix

---

## UI Testing Challenges in WinForms

### Challenges

**1. Native Windows Controls**
- WinForms uses native Windows controls, not web-based UI
- Requires Windows-specific automation tools
- Limited cross-platform testing

**2. Limited Tooling**
- Fewer mature tools compared to web testing (Selenium, Playwright)
- Smaller community and fewer resources
- Less frequent updates

**3. Slower Execution**
- UI tests are inherently slower than unit tests
- Must launch entire application
- Real UI rendering takes time

**4. More Brittle Than Unit Tests**
- UI changes break tests easily
- Element locators can become stale
- Timing issues (race conditions)

**5. Higher Maintenance Cost**
- UI changes require test updates
- More complex setup and teardown
- Debugging is harder

### When to Use UI Tests

**✅ GOOD Use Cases**:
- Critical user workflows (login, checkout, data entry)
- Smoke tests to verify application launches
- End-to-end integration testing
- Testing third-party controls or components
- Compliance/regulatory requirements

**❌ AVOID UI Tests When**:
- You can test at a lower level (unit/integration)
- Testing business logic (use unit tests for Services)
- Testing data access (use integration tests for Repositories)
- The UI changes frequently
- Fast feedback is critical (use unit tests)

**Best Practice**: Follow the **Test Pyramid**—many unit tests, some integration tests, few UI tests.

---

## UI Testing Tools

### FlaUI (RECOMMENDED) ⭐

**FlaUI** is the modern, actively maintained library for UI automation in .NET.

**Pros**:
- ✅ Modern API, actively maintained
- ✅ Supports UIA3 (Windows 10+) and UIA2 (legacy)
- ✅ Best for .NET applications (WinForms, WPF, UWP)
- ✅ Fluent, easy-to-use API
- ✅ Great documentation and examples

**Cons**:
- ❌ Windows-only
- ❌ Smaller community than Selenium

**Best For**: All new WinForms UI testing projects.

---

### WinAppDriver

**WinAppDriver** is Microsoft's official UI automation tool using WebDriver protocol.

**Pros**:
- ✅ Official Microsoft tool
- ✅ Uses familiar WebDriver/Selenium API
- ✅ Works with multiple languages (C#, Java, Python)
- ✅ Can test WinForms, WPF, UWP

**Cons**:
- ❌ Requires separate WinAppDriver.exe process
- ❌ Less .NET-native than FlaUI
- ❌ More setup complexity

**Best For**: Teams already familiar with Selenium, cross-language testing.

---

### White Framework (Legacy)

**White** is an older UIAutomation wrapper library.

**Status**: ⚠️ No longer actively maintained (last update 2015).

**Pros**:
- ✅ Mature, stable API
- ✅ Many examples available online

**Cons**:
- ❌ No longer maintained
- ❌ Doesn't support modern Windows features
- ❌ Limited .NET Core support

**Best For**: Legacy projects already using White (not recommended for new projects).

---

### Comparison Table

| Feature | FlaUI | WinAppDriver | White |
|---------|-------|--------------|-------|
| **Status** | ✅ Active | ✅ Active | ❌ Unmaintained |
| **Technology** | UIA2/UIA3 | UIA | UIA |
| **.NET Core** | ✅ Yes | ✅ Yes | ❌ No |
| **Learning Curve** | Easy | Medium | Easy |
| **Setup** | NuGet only | NuGet + EXE | NuGet only |
| **API Style** | Fluent .NET | Selenium-like | Fluent .NET |
| **Recommendation** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐ |

**Recommendation**: Use **FlaUI** for all new WinForms projects.

---

## Setting Up FlaUI

### Installing FlaUI

**Step 1**: Install NuGet packages

```bash
# Core FlaUI library
dotnet add package FlaUI.Core

# UIA3 automation (Windows 10+)
dotnet add package FlaUI.UIA3

# Optional: UIA2 for legacy Windows
dotnet add package FlaUI.UIA2
```

**Step 2**: Install test framework (if not already installed)

```bash
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
```

**Step 3**: Create test project structure

```
/YourApp.UITests
    ├── YourApp.UITests.csproj
    ├── LoginFormTests.cs
    ├── CustomerFormTests.cs
    └── PageObjects/
        ├── LoginPage.cs
        └── CustomerPage.cs
```

---

### Basic FlaUI Test Structure

```csharp
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using Xunit;

namespace YourApp.UITests
{
    public class BasicTests : IDisposable
    {
        private Application _app;
        private UIA3Automation _automation;
        private Window _mainWindow;

        public BasicTests()
        {
            // Arrange: Launch application
            _automation = new UIA3Automation();
            _app = Application.Launch("YourApp.exe");
            _mainWindow = _app.GetMainWindow(_automation);
        }

        [Fact]
        public void CanOpenAndCloseApplication()
        {
            // Assert: Main window is open
            Assert.NotNull(_mainWindow);
            Assert.True(_mainWindow.IsAvailable);
            Assert.Equal("Main Form", _mainWindow.Title);
        }

        public void Dispose()
        {
            // Cleanup: Close application
            _app?.Close();
            _app?.Dispose();
            _automation?.Dispose();
        }
    }
}
```

---

## Finding UI Elements

### Finding by AutomationId (RECOMMENDED) ⭐

**Setting AutomationId in WinForms**:

```csharp
// In your Form designer or constructor
txtUsername.AccessibleName = "UsernameTextBox"; // Sets AutomationId
btnLogin.AccessibleName = "LoginButton";
```

**Finding elements in tests**:

```csharp
// Find by AutomationId (most reliable)
var usernameBox = _mainWindow.FindFirstDescendant(cf =>
    cf.ByAutomationId("UsernameTextBox"))
    .AsTextBox();

var loginButton = _mainWindow.FindFirstDescendant(cf =>
    cf.ByAutomationId("LoginButton"))
    .AsButton();
```

**Why AutomationId is best**:
- ✅ Most stable (doesn't change when UI text changes)
- ✅ Unique identifier
- ✅ Fast lookup
- ✅ Explicitly set for testing

---

### Finding by Name/Text

```csharp
// Find button by text
var loginButton = _mainWindow.FindFirstDescendant(cf =>
    cf.ByName("Login"))
    .AsButton();

// Find label by text
var welcomeLabel = _mainWindow.FindFirstDescendant(cf =>
    cf.ByName("Welcome!"))
    .AsLabel();
```

**Warning**: ⚠️ Breaks when button text changes or is localized.

---

### Finding by Type

```csharp
using FlaUI.Core.Definitions;

// Find first TextBox
var textBox = _mainWindow.FindFirstDescendant(cf =>
    cf.ByControlType(ControlType.Edit))
    .AsTextBox();

// Find first Button
var button = _mainWindow.FindFirstDescendant(cf =>
    cf.ByControlType(ControlType.Button))
    .AsButton();
```

**Warning**: ⚠️ Not unique, finds first match only.

---

### XPath-like Selectors

```csharp
// Find nested element
var saveButton = _mainWindow
    .FindFirstDescendant(cf => cf.ByAutomationId("ToolbarPanel"))
    .FindFirstDescendant(cf => cf.ByAutomationId("SaveButton"))
    .AsButton();

// Find all buttons in a panel
var panel = _mainWindow.FindFirstDescendant(cf =>
    cf.ByAutomationId("ButtonPanel"));
var allButtons = panel.FindAllDescendants(cf =>
    cf.ByControlType(ControlType.Button));
```

---

## Interacting with Controls

### Buttons

```csharp
// Click button
var loginButton = _mainWindow.FindFirstDescendant(cf =>
    cf.ByAutomationId("LoginButton")).AsButton();
loginButton.Click();

// Double-click
loginButton.DoubleClick();

// Check if enabled
Assert.True(loginButton.IsEnabled);
```

---

### TextBoxes

```csharp
// Enter text
var usernameBox = _mainWindow.FindFirstDescendant(cf =>
    cf.ByAutomationId("UsernameTextBox")).AsTextBox();
usernameBox.Text = "testuser";

// Clear text
usernameBox.Text = string.Empty;

// Get text
string value = usernameBox.Text;
Assert.Equal("testuser", value);
```

---

### ComboBoxes

```csharp
var combo = _mainWindow.FindFirstDescendant(cf =>
    cf.ByAutomationId("StatusComboBox")).AsComboBox();

// Select by index
combo.Select(0);

// Select by text
combo.Select("Active");

// Get selected value
string selected = combo.SelectedItem.Text;
Assert.Equal("Active", selected);
```

---

### DataGridView

```csharp
var grid = _mainWindow.FindFirstDescendant(cf =>
    cf.ByAutomationId("CustomersGrid")).AsGrid();

// Get rows
var rows = grid.Rows;
Assert.Equal(5, rows.Length);

// Get cell value
var firstRowFirstCell = rows[0].Cells[0];
string cellValue = firstRowFirstCell.Name;

// Select row
rows[0].Select();
```

---

### CheckBoxes and RadioButtons

```csharp
// CheckBox
var checkbox = _mainWindow.FindFirstDescendant(cf =>
    cf.ByAutomationId("IsActiveCheckBox")).AsCheckBox();
checkbox.IsChecked = true;
Assert.True(checkbox.IsChecked);

// RadioButton
var radioButton = _mainWindow.FindFirstDescendant(cf =>
    cf.ByAutomationId("MaleRadioButton")).AsRadioButton();
radioButton.IsChecked = true;
Assert.True(radioButton.IsChecked);
```

---

## Test Scenarios

### Scenario 1: Login Form

```csharp
[Fact]
public void CanLoginWithValidCredentials()
{
    // Arrange
    var usernameBox = _mainWindow.FindFirstDescendant(cf =>
        cf.ByAutomationId("UsernameTextBox")).AsTextBox();
    var passwordBox = _mainWindow.FindFirstDescendant(cf =>
        cf.ByAutomationId("PasswordTextBox")).AsTextBox();
    var loginButton = _mainWindow.FindFirstDescendant(cf =>
        cf.ByAutomationId("LoginButton")).AsButton();

    // Act
    usernameBox.Text = "admin";
    passwordBox.Text = "password123";
    loginButton.Click();

    // Assert: Wait for main form to appear
    var mainForm = Retry.WhileNull(() =>
        _app.GetAllTopLevelWindows(_automation)
            .FirstOrDefault(w => w.Title == "Main Form"),
        timeout: TimeSpan.FromSeconds(5)).Result;

    Assert.NotNull(mainForm);
}
```

---

### Scenario 2: CRUD Operations

```csharp
[Fact]
public void CanCreateNewCustomer()
{
    // Arrange
    var newButton = _mainWindow.FindFirstDescendant(cf =>
        cf.ByAutomationId("NewButton")).AsButton();
    newButton.Click();

    // Wait for edit form
    var editForm = Retry.WhileNull(() =>
        _app.GetAllTopLevelWindows(_automation)
            .FirstOrDefault(w => w.Title == "Edit Customer"),
        timeout: TimeSpan.FromSeconds(3)).Result;

    // Act: Fill form
    var nameBox = editForm.FindFirstDescendant(cf =>
        cf.ByAutomationId("NameTextBox")).AsTextBox();
    var emailBox = editForm.FindFirstDescendant(cf =>
        cf.ByAutomationId("EmailTextBox")).AsTextBox();
    var saveButton = editForm.FindFirstDescendant(cf =>
        cf.ByAutomationId("SaveButton")).AsButton();

    nameBox.Text = "John Doe";
    emailBox.Text = "john@example.com";
    saveButton.Click();

    // Assert: Form closes, customer appears in grid
    Wait.UntilInputIsProcessed();
    var grid = _mainWindow.FindFirstDescendant(cf =>
        cf.ByAutomationId("CustomersGrid")).AsGrid();

    var lastRow = grid.Rows.Last();
    Assert.Contains("John Doe", lastRow.Cells[0].Name);
}
```

---

### Scenario 3: Form Validation

```csharp
[Fact]
public void ShowsErrorWhenRequiredFieldEmpty()
{
    // Arrange
    var saveButton = _mainWindow.FindFirstDescendant(cf =>
        cf.ByAutomationId("SaveButton")).AsButton();

    // Act: Try to save with empty required field
    saveButton.Click();

    // Assert: Error message appears
    var errorLabel = _mainWindow.FindFirstDescendant(cf =>
        cf.ByAutomationId("ErrorLabel")).AsLabel();

    Assert.True(errorLabel.IsAvailable);
    Assert.Contains("required", errorLabel.Name.ToLower());
}
```

---

## Waiting Strategies

### Wait for Element

```csharp
using FlaUI.Core.Tools;

// Wait for element to appear
var button = Retry.WhileNull(() =>
    _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("SaveButton")),
    timeout: TimeSpan.FromSeconds(5)).Result;

// Wait for element to become enabled
Retry.WhileTrue(() => !button.IsEnabled,
    timeout: TimeSpan.FromSeconds(3));
```

---

### Wait for Window

```csharp
// Wait for dialog to appear
var dialog = Retry.WhileNull(() =>
    _app.GetAllTopLevelWindows(_automation)
        .FirstOrDefault(w => w.Title == "Confirmation"),
    timeout: TimeSpan.FromSeconds(5)).Result;

// Wait for window to close
Retry.WhileFalse(() => _mainWindow.IsAvailable == false,
    timeout: TimeSpan.FromSeconds(3));
```

---

### Implicit vs Explicit Waits

**Explicit Waits (Recommended)**:
```csharp
// Wait for specific condition
var element = Retry.WhileNull(() =>
    _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("Element")),
    timeout: TimeSpan.FromSeconds(5)).Result;
```

**Implicit Waits**:
```csharp
// Wait for input to be processed (generic)
Wait.UntilInputIsProcessed();
Thread.Sleep(1000); // ❌ Avoid fixed sleeps
```

**Best Practice**: Use explicit waits for specific conditions, avoid `Thread.Sleep`.

---

## Best Practices

### ✅ DO

1. **Use AutomationId for element finding**
   ```csharp
   // ✅ GOOD
   var btn = _mainWindow.FindFirstDescendant(cf =>
       cf.ByAutomationId("SaveButton")).AsButton();
   ```

2. **Use Page Object Pattern for reusability**
   ```csharp
   // ✅ GOOD
   var loginPage = new LoginPage(_mainWindow);
   loginPage.Login("user", "pass");
   ```

3. **Wait explicitly for elements**
   ```csharp
   // ✅ GOOD
   var element = Retry.WhileNull(() => FindElement(),
       timeout: TimeSpan.FromSeconds(5)).Result;
   ```

4. **Dispose resources properly**
   ```csharp
   // ✅ GOOD
   public void Dispose()
   {
       _app?.Close();
       _app?.Dispose();
       _automation?.Dispose();
   }
   ```

5. **Test critical paths only**
   - Login, checkout, data entry
   - Don't test every button

6. **Keep tests independent**
   - Each test starts fresh
   - No shared state between tests

7. **Use descriptive test names**
   ```csharp
   // ✅ GOOD
   [Fact]
   public void CanLoginWithValidCredentials() { }
   ```

8. **Clean up test data**
   - Delete created records
   - Reset database state

9. **Take screenshots on failure**
   ```csharp
   // ✅ GOOD
   catch (Exception)
   {
       CaptureScreenshot("test-failure.png");
       throw;
   }
   ```

10. **Prefer testing Presenters over UI**
    - Faster, more reliable
    - Test business logic in unit tests

---

### ❌ DON'T

1. **Don't use fixed Thread.Sleep**
   ```csharp
   // ❌ BAD
   Thread.Sleep(2000);

   // ✅ GOOD
   Retry.WhileNull(() => FindElement(), timeout: TimeSpan.FromSeconds(5));
   ```

2. **Don't find elements by text**
   ```csharp
   // ❌ BAD (breaks with localization)
   var btn = _mainWindow.FindFirstDescendant(cf => cf.ByName("Save"));

   // ✅ GOOD
   var btn = _mainWindow.FindFirstDescendant(cf =>
       cf.ByAutomationId("SaveButton"));
   ```

3. **Don't test business logic in UI tests**
   ```csharp
   // ❌ BAD - Test this in unit tests
   [Fact]
   public void ValidateEmailFormat() { }
   ```

4. **Don't hardcode paths**
   ```csharp
   // ❌ BAD
   Application.Launch("C:\\MyApp\\bin\\Debug\\App.exe");

   // ✅ GOOD
   Application.Launch(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App.exe"));
   ```

5. **Don't run UI tests in parallel**
   - UI tests interfere with each other
   - Run sequentially

6. **Don't create too many UI tests**
   - Slow, brittle, expensive
   - Prefer unit/integration tests

7. **Don't ignore flaky tests**
   - Fix root cause (timing, setup)
   - Don't just retry indefinitely

8. **Don't skip cleanup**
   - Always dispose Application and Automation
   - Prevents orphaned processes

---

## UI Test Patterns

### Page Object Pattern

**Purpose**: Encapsulate UI interaction logic in reusable page classes.

```csharp
// PageObjects/LoginPage.cs
public class LoginPage
{
    private readonly Window _window;

    public LoginPage(Window window)
    {
        _window = window;
    }

    private TextBox UsernameTextBox => _window
        .FindFirstDescendant(cf => cf.ByAutomationId("UsernameTextBox"))
        .AsTextBox();

    private TextBox PasswordTextBox => _window
        .FindFirstDescendant(cf => cf.ByAutomationId("PasswordTextBox"))
        .AsTextBox();

    private Button LoginButton => _window
        .FindFirstDescendant(cf => cf.ByAutomationId("LoginButton"))
        .AsButton();

    public void Login(string username, string password)
    {
        UsernameTextBox.Text = username;
        PasswordTextBox.Text = password;
        LoginButton.Click();
    }

    public bool IsLoginButtonEnabled => LoginButton.IsEnabled;
}

// Usage in tests
[Fact]
public void CanLoginWithValidCredentials()
{
    var loginPage = new LoginPage(_mainWindow);
    loginPage.Login("admin", "password");

    // Assert...
}
```

---

### Screenshot on Failure

```csharp
using FlaUI.Core.Capturing;

public class BaseUITest : IDisposable
{
    protected Application _app;
    protected UIA3Automation _automation;
    protected Window _mainWindow;

    public BaseUITest()
    {
        _automation = new UIA3Automation();
        _app = Application.Launch("YourApp.exe");
        _mainWindow = _app.GetMainWindow(_automation);
    }

    protected void CaptureScreenshot(string filename)
    {
        var capture = Capture.Screen();
        capture.ToFile(filename);
    }

    public void Dispose()
    {
        try
        {
            _app?.Close();
        }
        catch (Exception ex)
        {
            // Capture screenshot on failure
            CaptureScreenshot($"failure-{DateTime.Now:yyyyMMdd-HHmmss}.png");
            throw;
        }
        finally
        {
            _app?.Dispose();
            _automation?.Dispose();
        }
    }
}
```

---

## Complete Working Example

### CustomerForm UI Test

```csharp
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using Xunit;

namespace YourApp.UITests
{
    public class CustomerFormTests : IDisposable
    {
        private Application _app;
        private UIA3Automation _automation;
        private Window _mainWindow;

        public CustomerFormTests()
        {
            _automation = new UIA3Automation();
            _app = Application.Launch("CustomerApp.exe");
            _mainWindow = _app.GetMainWindow(_automation, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void CanCreateNewCustomer()
        {
            // Arrange
            var newButton = _mainWindow.FindFirstDescendant(cf =>
                cf.ByAutomationId("NewButton")).AsButton();

            // Act: Click New
            newButton.Click();

            // Wait for edit form
            var editWindow = Retry.WhileNull(() =>
                _app.GetAllTopLevelWindows(_automation)
                    .FirstOrDefault(w => w.Title == "Edit Customer"),
                timeout: TimeSpan.FromSeconds(5)).Result;

            // Fill form
            var nameBox = editWindow.FindFirstDescendant(cf =>
                cf.ByAutomationId("NameTextBox")).AsTextBox();
            var emailBox = editWindow.FindFirstDescendant(cf =>
                cf.ByAutomationId("EmailTextBox")).AsTextBox();
            var saveButton = editWindow.FindFirstDescendant(cf =>
                cf.ByAutomationId("SaveButton")).AsButton();

            nameBox.Text = "Test Customer";
            emailBox.Text = "test@example.com";
            saveButton.Click();

            // Assert: Verify customer appears in grid
            Wait.UntilInputIsProcessed();
            var grid = _mainWindow.FindFirstDescendant(cf =>
                cf.ByAutomationId("CustomersGrid")).AsGrid();

            var rows = grid.Rows;
            Assert.Contains(rows, r => r.Cells[0].Name == "Test Customer");
        }

        [Fact]
        public void CanEditExistingCustomer()
        {
            // Arrange: Select first customer
            var grid = _mainWindow.FindFirstDescendant(cf =>
                cf.ByAutomationId("CustomersGrid")).AsGrid();
            grid.Rows[0].Select();

            var editButton = _mainWindow.FindFirstDescendant(cf =>
                cf.ByAutomationId("EditButton")).AsButton();

            // Act: Edit customer
            editButton.Click();

            var editWindow = Retry.WhileNull(() =>
                _app.GetAllTopLevelWindows(_automation)
                    .FirstOrDefault(w => w.Title == "Edit Customer"),
                timeout: TimeSpan.FromSeconds(5)).Result;

            var nameBox = editWindow.FindFirstDescendant(cf =>
                cf.ByAutomationId("NameTextBox")).AsTextBox();
            nameBox.Text = "Updated Name";

            var saveButton = editWindow.FindFirstDescendant(cf =>
                cf.ByAutomationId("SaveButton")).AsButton();
            saveButton.Click();

            // Assert
            Wait.UntilInputIsProcessed();
            grid = _mainWindow.FindFirstDescendant(cf =>
                cf.ByAutomationId("CustomersGrid")).AsGrid();
            Assert.Equal("Updated Name", grid.Rows[0].Cells[0].Name);
        }

        [Fact]
        public void CanDeleteCustomer()
        {
            // Arrange
            var grid = _mainWindow.FindFirstDescendant(cf =>
                cf.ByAutomationId("CustomersGrid")).AsGrid();
            int initialCount = grid.Rows.Length;
            grid.Rows[0].Select();

            var deleteButton = _mainWindow.FindFirstDescendant(cf =>
                cf.ByAutomationId("DeleteButton")).AsButton();

            // Act: Delete
            deleteButton.Click();

            // Confirm dialog
            var confirmDialog = Retry.WhileNull(() =>
                _app.GetAllTopLevelWindows(_automation)
                    .FirstOrDefault(w => w.Title.Contains("Confirm")),
                timeout: TimeSpan.FromSeconds(3)).Result;

            var yesButton = confirmDialog.FindFirstDescendant(cf =>
                cf.ByName("Yes")).AsButton();
            yesButton.Click();

            // Assert
            Wait.UntilInputIsProcessed();
            grid = _mainWindow.FindFirstDescendant(cf =>
                cf.ByAutomationId("CustomersGrid")).AsGrid();
            Assert.Equal(initialCount - 1, grid.Rows.Length);
        }

        public void Dispose()
        {
            _app?.Close();
            _app?.Dispose();
            _automation?.Dispose();
        }
    }
}
```

---

## Running UI Tests

### Test Isolation

```csharp
// Reset database before each test
public CustomerFormTests()
{
    ResetTestDatabase();
    _automation = new UIA3Automation();
    _app = Application.Launch("CustomerApp.exe");
    _mainWindow = _app.GetMainWindow(_automation);
}

private void ResetTestDatabase()
{
    // Delete test database and recreate
    var dbPath = "test.db";
    if (File.Exists(dbPath))
        File.Delete(dbPath);
}
```

---

### Sequential Execution (NOT Parallel)

```csharp
// In xUnit, disable parallel execution
[assembly: CollectionBehavior(DisableTestParallelization = true)]
```

**Why**: UI tests interfere with each other when run in parallel (window focus, keyboard/mouse).

---

### Debugging UI Tests

**Tips**:
1. **Add breakpoints** and inspect elements
2. **Capture screenshots** at key points
3. **Use Debug.WriteLine** for logging
4. **Run in Debug mode** to see the UI
5. **Use `Thread.Sleep` temporarily** to observe UI state (remove after debugging)

---

## CI/CD Considerations

### Running in CI

**Challenge**: WinForms UI tests require a Windows desktop session with UI rendering.

**Solutions**:
- Use Windows agents (GitHub Actions, Azure DevOps)
- Ensure agent has UI session (not headless)
- Run tests on dedicated test agents

---

### GitHub Actions Example

```yaml
name: UI Tests

on: [push, pull_request]

jobs:
  ui-tests:
    runs-on: windows-latest

    steps:
    - uses: actions/checkout@v3

    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'

    - name: Restore dependencies
      run: dotnet restore

    - name: Build
      run: dotnet build --no-restore

    - name: Run UI Tests
      run: dotnet test --filter Category=UI --no-build --verbosity normal

    - name: Upload Screenshots on Failure
      if: failure()
      uses: actions/upload-artifact@v3
      with:
        name: test-screenshots
        path: '**/*.png'
```

---

### Azure DevOps Example

```yaml
trigger:
- main

pool:
  vmImage: 'windows-latest'

steps:
- task: UseDotNet@2
  inputs:
    version: '8.0.x'

- task: DotNetCoreCLI@2
  displayName: 'Build'
  inputs:
    command: 'build'

- task: DotNetCoreCLI@2
  displayName: 'Run UI Tests'
  inputs:
    command: 'test'
    arguments: '--filter Category=UI --no-build'
```

---

## Maintenance

### Keeping Tests Stable

1. **Use AutomationId exclusively**
   - Most stable identifier
   - Doesn't change with UI text

2. **Avoid brittle selectors**
   - Don't rely on text
   - Don't rely on element order

3. **Add retries for flaky tests**
   ```csharp
   var element = Retry.WhileNull(() => FindElement(),
       timeout: TimeSpan.FromSeconds(10)).Result;
   ```

4. **Use Page Object Pattern**
   - Encapsulate element finding
   - Single place to update selectors

---

### Refactoring

**Before (Brittle)**:
```csharp
[Fact]
public void Test()
{
    var btn = _mainWindow.FindFirstDescendant(cf => cf.ByName("Save")).AsButton();
    btn.Click();
}
```

**After (Maintainable)**:
```csharp
public class CustomerPage
{
    private Button SaveButton => _window
        .FindFirstDescendant(cf => cf.ByAutomationId("SaveButton"))
        .AsButton();

    public void Save() => SaveButton.Click();
}

[Fact]
public void Test()
{
    var page = new CustomerPage(_mainWindow);
    page.Save();
}
```

---

## Alternatives to UI Testing

### Test Presenters Instead (RECOMMENDED)

**Why it's better**:
- ✅ 100x faster than UI tests
- ✅ More reliable, no timing issues
- ✅ Easier to debug
- ✅ Tests business logic directly

**Example**:

```csharp
// UI Test (slow, brittle)
[Fact]
public void CanSaveCustomer_UITest()
{
    // Launch app, find elements, click buttons...
    // 5+ seconds
}

// Presenter Test (fast, reliable)
[Fact]
public async Task SaveCustomer_ValidData_CallsRepository()
{
    // Arrange
    var mockRepo = new Mock<ICustomerRepository>();
    var presenter = new CustomerPresenter(mockRepo.Object);
    var customer = new Customer { Name = "Test" };

    // Act
    await presenter.SaveCustomerAsync(customer);

    // Assert
    mockRepo.Verify(r => r.SaveAsync(customer), Times.Once);
    // < 100 milliseconds
}
```

**Best Practice**: Test Presenters/ViewModels with unit tests. Use UI tests only for end-to-end validation.

---

### Manual Testing

**When it's more efficient**:
- Exploratory testing
- Usability testing
- One-off verification
- UI changes frequently
- Test ROI is low

---

## Common Issues

### Issue 1: Element Not Found

**Problem**: `FindFirstDescendant` returns null.

**Solutions**:
- Add wait/retry logic
- Verify AutomationId is set correctly
- Check element is visible (not hidden)
- Use FlaUI Inspector tool to debug

### Issue 2: Timing Issues

**Problem**: Tests fail randomly due to timing.

**Solutions**:
- Use explicit waits (`Retry.WhileNull`)
- Avoid `Thread.Sleep`
- Increase timeout for slow operations

### Issue 3: Application Doesn't Close

**Problem**: `_app.Close()` hangs or fails.

**Solutions**:
```csharp
try
{
    _app?.Close();
    _app?.WaitWhileBusy(TimeSpan.FromSeconds(5));
}
catch
{
    _app?.Kill();
}
```

### Issue 4: Tests Interfere with Each Other

**Problem**: Tests fail when run together.

**Solutions**:
- Disable parallel execution
- Reset database between tests
- Launch fresh app instance per test

### Issue 5: Can't Find Third-Party Controls

**Problem**: Custom controls not recognized.

**Solutions**:
- Ensure controls support UIAutomation
- Use UIA Spy tool to inspect
- Consider using Win32 API as fallback

---

## Related Topics

- **[Unit Testing](unit-testing.md)** - Test business logic in Services
- **[Integration Testing](integration-testing.md)** - Test data access in Repositories
- **[MVP Pattern](../architecture/mvp-pattern.md)** - Separate UI from logic (testable)
- **[MVVM Pattern](../architecture/mvvm-pattern.md)** - Alternative testable pattern

---

**Last Updated**: 2025-11-07
**Version**: 1.0
