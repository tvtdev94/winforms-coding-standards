# WinForms Development Workflow

This workflow guides the development of WinForms applications following best practices.

---

## 1. Planning Phase

### Context Loading Strategy

**Before starting any task**, load the appropriate documentation:

| Task Type | Files to Read |
|-----------|--------------|
| **Creating a new Form** | `templates/form-template.cs`, `docs/architecture/mvp-pattern.md`, `docs/ui-ux/input-validation.md` |
| **Creating a Service** | `templates/service-template.cs`, `docs/best-practices/async-await.md`, `docs/best-practices/error-handling.md` |
| **Creating a Repository** | `templates/repository-template.cs`, `docs/data-access/entity-framework.md`, `docs/data-access/repository-pattern.md` |
| **Writing Tests** | `templates/test-template.cs`, `docs/testing/unit-testing.md`, `docs/testing/integration-testing.md` |
| **Data Binding** | `docs/ui-ux/data-binding.md`, `docs/ui-ux/datagridview-practices.md` |
| **Form Communication** | `docs/ui-ux/form-communication.md`, `docs/architecture/mvp-pattern.md` |
| **Error Handling** | `docs/best-practices/error-handling.md`, `docs/best-practices/resource-management.md` |
| **Thread Safety** | `docs/best-practices/thread-safety.md`, `docs/best-practices/async-await.md` |
| **Performance** | `docs/best-practices/performance.md`, `docs/advanced/performance-profiling.md` |
| **Security** | `docs/best-practices/security.md`, `docs/data-access/connection-management.md` |

### Create Implementation Plan

**For complex tasks** (3+ steps), create a plan in `plans/`:

1. Use appropriate template from `plans/templates/`
2. Fill in requirements, architecture, checklist
3. Save as `plans/YYMMDD-feature-name-plan.md`
4. Reference plan throughout implementation

---

## 2. Implementation Phase

### Always Use Templates

**NEVER generate code from scratch**. Always start with templates:

- `/templates/form-template.cs` - MVP pattern form
- `/templates/service-template.cs` - Business logic service
- `/templates/repository-template.cs` - Data access repository
- `/templates/test-template.cs` - Unit test structure

### Follow MVP/MVVM Pattern

**MVP Pattern** (Recommended):
```
Form (View) → IView interface → Presenter → Service → Repository
```

**MVVM Pattern** (.NET 8+):
```
Form (View) → ViewModel → Service → Repository
```

### Code Generation Patterns

#### Pattern 1: Creating a New Form

```
1. Read: templates/form-template.cs
2. Read: docs/architecture/mvp-pattern.md (if complex logic)
3. Generate:
   - CustomerForm.cs (Form implementation)
   - ICustomerView.cs (View interface)
   - CustomerPresenter.cs (Presentation logic)
4. Ensure:
   - MVP pattern properly implemented
   - Async/await for data loading
   - Proper error handling
   - Input validation
   - Resource disposal
5. Offer: Generate unit tests for the presenter
```

#### Pattern 2: Creating a Service

```
1. Read: templates/service-template.cs
2. Generate: CustomerService.cs with:
   - Constructor injection for dependencies (ICustomerRepository, ILogger)
   - Async methods (LoadCustomersAsync, SaveCustomerAsync, etc.)
   - Input validation with ArgumentNullException
   - Try-catch with logging
   - XML documentation comments
3. Generate: ICustomerService interface
4. Offer: Generate unit tests with mocked dependencies
```

#### Pattern 3: Creating a Repository

```
1. Read: templates/repository-template.cs
2. Generate:
   - CustomerRepository.cs implementing ICustomerRepository
   - Use EF Core DbContext
   - Async CRUD operations
   - Proper using/disposal
   - Error handling
3. Note: Remind user about DbContext configuration
4. Offer: Generate integration tests
```

### Implementation Rules

**When generating Forms:**
1. ✅ Start with `form-template.cs`
2. ✅ Implement MVP pattern (Form + IView + Presenter)
3. ✅ Async event handlers for data operations
4. ✅ Try-catch with user-friendly error messages
5. ✅ Dispose resources in Dispose() method
6. ✅ Set TabIndex for proper keyboard navigation
7. ✅ Use meaningful control names (not button1, textBox1)

**When generating Services:**
1. ✅ Start with `service-template.cs`
2. ✅ Constructor injection for all dependencies
3. ✅ Validate all inputs (ArgumentNullException, ArgumentException)
4. ✅ Async methods with proper cancellation token support
5. ✅ Log all operations (info, errors, warnings)
6. ✅ Wrap exceptions with meaningful messages
7. ✅ XML documentation on all public methods

**When generating Repositories:**
1. ✅ Start with `repository-template.cs`
2. ✅ Implement generic repository pattern
3. ✅ Use EF Core async methods (ToListAsync, FirstOrDefaultAsync, etc.)
4. ✅ Proper disposal of DbContext
5. ✅ Include soft-delete support if applicable
6. ✅ Error handling with data access exceptions

---

## 3. Testing Phase

### Test-Driven Approach

1. **Generate tests** using `templates/test-template.cs`
2. **Run tests**: `dotnet test`
3. **Fix failures** before proceeding
4. **Verify coverage** meets standards (80%+ for services)

### Testing Strategy

**For Services:**
- Unit tests with mocked dependencies (Moq)
- Test success paths, error cases, edge cases
- Naming: `MethodName_Scenario_ExpectedResult`

**For Repositories:**
- Integration tests with EF Core InMemory database
- Test CRUD operations
- Test queries and filters

**For Presenters:**
- Unit tests with mocked View and Service
- Test user interaction flows
- Test error handling

---

## 4. Code Review Phase

### Self-Review Checklist

Before committing, verify against [code-review-checklist.md](./code-review-checklist.md):

- [ ] Code compiles without warnings
- [ ] All tests pass
- [ ] No business logic in Forms
- [ ] Resources properly disposed
- [ ] Async/await used for I/O
- [ ] Input validated
- [ ] Errors handled with try-catch
- [ ] XML comments added
- [ ] No magic numbers
- [ ] Thread-safe UI updates
- [ ] Tests cover new code
- [ ] Follows naming conventions

### Agent-Assisted Review (Phase 2+)

When agents are available:
```
/review-code [files]
```

This will spawn `winforms-reviewer` agent to check:
- MVP/MVVM pattern adherence
- WinForms best practices
- Threading issues
- Performance concerns

---

## 5. Documentation Phase

### Update Documentation

**For new features:**
- Update XML comments in code
- Update relevant docs in `docs/`
- Add examples if applicable
- Update CHANGELOG.md

**For example project:**
- Keep `example-project/` in sync with standards
- Update if new pattern introduced

---

## 6. Commit Phase

### Git Workflow

**Only commit when user explicitly requests it.**

**When creating commits:**

1. Run `git status` and `git diff` to see changes
2. Draft commit message following repository style
3. Add relevant files to staging area
4. Create commit with co-authored-by footer:

```
Feature description here

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>
```

5. Run `git status` after commit to verify

**DO NOT:**
- ❌ Push unless explicitly requested
- ❌ Use `--no-verify` flag
- ❌ Force push to main/master
- ❌ Amend other developers' commits
- ❌ Commit secrets (.env, credentials, etc.)

---

## Common Code Snippets

### Async Button Click Handler

```csharp
private async void btnLoad_Click(object sender, EventArgs e)
{
    try
    {
        btnLoad.Enabled = false;
        Cursor = Cursors.WaitCursor;

        var data = await _presenter.LoadDataAsync();
        // Update UI with data
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}", "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    finally
    {
        btnLoad.Enabled = true;
        Cursor = Cursors.Default;
    }
}
```

### Thread-Safe UI Update

```csharp
private void UpdateUIFromBackgroundThread(string message)
{
    if (InvokeRequired)
    {
        Invoke(new Action(() => UpdateUIFromBackgroundThread(message)));
        return;
    }

    // Safe to update UI here
    lblStatus.Text = message;
}
```

### Proper Resource Disposal

```csharp
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        // Dispose managed resources
        _timer?.Dispose();
        _backgroundWorker?.Dispose();
        components?.Dispose();

        // Unsubscribe from events
        _service.DataChanged -= OnDataChanged;
    }
    base.Dispose(disposing);
}
```

---

**Last Updated**: 2025-11-08 (Phase 1)
**Version**: 1.0
