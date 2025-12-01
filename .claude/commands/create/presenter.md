---
description: Create a new MVP Presenter class with view interface
---

# Create MVP Presenter

You are tasked with creating a new MVP Presenter class following WinForms best practices.

---

## 🔥 STEP 0: Load Rules (MANDATORY - ALWAYS FIRST!)

**Use `rules-loader` subagent to load ALL coding rules:**

```
Task(subagent_type="rules-loader", prompt="Load rules for creating a new MVP Presenter class")
```

**Wait for rules summary before proceeding.** The rules-loader will:
- Read `.claude/project-context.md` for project settings
- Load MVP pattern rules
- Load event subscription/unsubscription rules
- Identify `templates/presenter-template.cs` to use

**⚠️ If project-context.md doesn't exist**: Ask user for preferences.

---

## Workflow

1. **Ask for Presenter Information**
   - Presenter name (e.g., "CustomerEditPresenter", "OrderListPresenter")
   - What entity/domain does it manage?
   - Is it for editing (single entity) or listing (multiple entities)?
   - What operations are needed? (CRUD, search, etc.)

2. **Read Templates and Documentation**
   - Read `templates/presenter-template.cs` as the base
   - Reference `docs/patterns/mvp-pattern.md` for MVP rules
   - Reference existing presenters in the project for consistency

3. **Create Presenter Files**

   Generate **TWO files**:

   ### File 1: View Interface (`I{EntityName}View.cs`)

   **Location**: `Views/` folder

   ```csharp
   namespace YourApp.Views;

   /// <summary>
   /// View interface for {Entity} form.
   /// </summary>
   public interface I{EntityName}View
   {
       // ═══════════════════════════════════════════════════════════════
       // Events - What can happen in the UI
       // ═══════════════════════════════════════════════════════════════
       event EventHandler? LoadRequested;
       event EventHandler? SaveRequested;
       event EventHandler? CancelRequested;
       event EventHandler? DeleteRequested;

       // For list views, add:
       // event EventHandler<string>? SearchRequested;
       // event EventHandler<int>? EditRequested;

       // ═══════════════════════════════════════════════════════════════
       // Data Properties - Form fields (two-way binding)
       // ═══════════════════════════════════════════════════════════════
       int EntityId { get; set; }
       string EntityName { get; set; }
       // Add properties for each form field

       // ═══════════════════════════════════════════════════════════════
       // UI State Properties
       // ═══════════════════════════════════════════════════════════════
       bool IsLoading { get; set; }
       bool IsEditMode { get; set; }
       bool IsSaveEnabled { get; set; }

       // ═══════════════════════════════════════════════════════════════
       // Methods - Actions the presenter can trigger
       // ═══════════════════════════════════════════════════════════════
       void SetFieldError(string fieldName, string errorMessage);
       void ClearAllErrors();
       void ShowSuccess(string message);
       void ShowError(string message);
       bool ShowConfirmation(string message, string title);
       void CloseWithResult(bool success);
   }
   ```

   ### File 2: Presenter (`{EntityName}Presenter.cs`)

   **Location**: `Presenters/` folder

   ```csharp
   using Microsoft.Extensions.Logging;

   namespace YourApp.Presenters;

   /// <summary>
   /// Presenter for {Entity} view.
   /// Contains ALL business logic - Form is just a thin UI shell.
   /// </summary>
   public class {EntityName}Presenter : IDisposable
   {
       private readonly I{EntityName}View _view;
       private readonly I{Entity}Service _service;
       private readonly ILogger<{EntityName}Presenter> _logger;
       private {Entity}? _currentEntity;
       private bool _disposed;

       public {EntityName}Presenter(
           I{EntityName}View view,
           I{Entity}Service service,
           ILogger<{EntityName}Presenter> logger)
       {
           _view = view ?? throw new ArgumentNullException(nameof(view));
           _service = service ?? throw new ArgumentNullException(nameof(service));
           _logger = logger ?? throw new ArgumentNullException(nameof(logger));

           SubscribeToViewEvents();
       }

       private void SubscribeToViewEvents()
       {
           _view.LoadRequested += OnLoadRequested;
           _view.SaveRequested += OnSaveRequested;
           _view.CancelRequested += OnCancelRequested;
           _view.DeleteRequested += OnDeleteRequested;
       }

       private void UnsubscribeFromViewEvents()
       {
           _view.LoadRequested -= OnLoadRequested;
           _view.SaveRequested -= OnSaveRequested;
           _view.CancelRequested -= OnCancelRequested;
           _view.DeleteRequested -= OnDeleteRequested;
       }

       // Event handlers with async void pattern
       private async void OnLoadRequested(object? sender, EventArgs e)
       {
           await LoadEntityAsync();
       }

       private async Task LoadEntityAsync()
       {
           try
           {
               _view.IsLoading = true;
               // Load entity from service
               // Populate view properties
           }
           catch (Exception ex)
           {
               _logger.LogError(ex, "Error loading entity");
               _view.ShowError($"Failed to load: {ex.Message}");
           }
           finally
           {
               _view.IsLoading = false;
           }
       }

       // Validation helper
       private Dictionary<string, string> ValidateEntity({Entity} entity)
       {
           var errors = new Dictionary<string, string>();
           // Add validation rules
           return errors;
       }

       public void Dispose()
       {
           if (!_disposed)
           {
               UnsubscribeFromViewEvents();
               _disposed = true;
           }
       }
   }
   ```

4. **Generate Complete Code**

   Based on user requirements, generate:
   - Complete view interface with all properties/events
   - Complete presenter with all event handlers
   - Proper error handling with logging
   - Validation logic

5. **Register in DI Container**

   Show the user how to register in `Program.cs`:
   ```csharp
   // Register presenter
   services.AddTransient<{EntityName}Presenter>();

   // Register form
   services.AddTransient<{EntityName}Form>();
   ```

6. **Suggest Unit Tests**

   Ask: "Would you like me to generate unit tests for this presenter using /add:test?"

## Best Practices Checklist

Before finishing, verify:
- ✅ View interface created with all properties/events
- ✅ Presenter has NO Windows.Forms references
- ✅ All business logic in Presenter
- ✅ Events subscribed in constructor
- ✅ Events unsubscribed in Dispose
- ✅ Async/await for all I/O operations
- ✅ Try-catch with logging in all handlers
- ✅ Validation before save operations
- ✅ Field name mapping for ErrorProvider
- ✅ XML documentation on all public members

## Presenter Types

### Edit Presenter (Single Entity)
```csharp
// Events
LoadRequested, SaveRequested, CancelRequested, DeleteRequested

// Key Methods
LoadEntityAsync(), SaveEntityAsync(), DeleteEntityAsync()
```

### List Presenter (Multiple Entities)
```csharp
// Events
LoadRequested, RefreshRequested, SearchRequested
AddRequested, EditRequested, DeleteRequested

// Key Methods
LoadEntitiesAsync(), SearchAsync(), DeleteSelectedAsync()
```

## Example Output Structure

```
/Views
    └── I{EntityName}View.cs          (Interface)
/Presenters
    └── {EntityName}Presenter.cs      (Implementation)
```

## Common Mistakes to Avoid

❌ Don't reference Windows.Forms in Presenter
❌ Don't access controls directly - use IView properties
❌ Don't forget to unsubscribe from events in Dispose
❌ Don't forget async/await for I/O operations
❌ Don't skip validation in save operations
❌ Don't return raw exception messages to users

## Field Name Mapping

When view properties have different names than model properties:

```csharp
// Model: Name, Email, Phone
// View: CustomerName, CustomerEmail, CustomerPhone

// Map in presenter for ErrorProvider:
var fieldNameMap = new Dictionary<string, string>
{
    { "Name", nameof(_view.CustomerName) },
    { "Email", nameof(_view.CustomerEmail) },
    { "Phone", nameof(_view.CustomerPhone) }
};

foreach (var error in validationErrors)
{
    var viewFieldName = fieldNameMap.TryGetValue(error.Key, out var mapped)
        ? mapped
        : error.Key;
    _view.SetFieldError(viewFieldName, error.Value);
}
```

## After Creation

1. Show the user the created files
2. Explain the MVP architecture
3. Suggest creating the Form that implements the view interface
4. Suggest creating unit tests for the presenter
