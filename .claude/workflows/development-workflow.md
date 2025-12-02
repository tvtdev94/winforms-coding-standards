# WinForms Development Workflow

Complete guide for developing WinForms applications following best practices.

---

## General Principles

- **YAGNI**: You Aren't Gonna Need It - don't over-engineer
- **KISS**: Keep It Simple, Stupid - prefer simple solutions
- **DRY**: Don't Repeat Yourself - extract common code
- **Files < 500 lines** when practical

---

## Context Loading (ALWAYS FIRST)

Before ANY implementation:

1. **Read `.claude/project-context.md`**
   - UI Framework: Standard / DevExpress / ReaLTaiizor
   - Database: SQLite / SQL Server / PostgreSQL
   - Framework: .NET 8 / .NET Framework 4.8
   - Pattern: MVP / MVVM

2. **Read `.claude/INDEX.md`**
   - Find relevant templates
   - Find applicable guides

3. **Never guess** - always check context first

---

## Context Loading by Task

| Task Type | Files to Read |
|-----------|--------------|
| **New Form** | `templates/form-template.cs`, `docs/architecture/mvp-pattern.md` |
| **New Service** | `templates/service-template.cs`, `docs/best-practices/async-await.md` |
| **New Repository** | `templates/repository-template.cs`, `docs/data-access/entity-framework.md` |
| **Writing Tests** | `templates/test-template.cs`, `docs/testing/unit-testing.md` |
| **Data Binding** | `docs/ui-ux/data-binding.md` |
| **Error Handling** | `docs/best-practices/error-handling.md` |
| **Thread Safety** | `docs/best-practices/thread-safety.md` |

---

## Subagents

| Agent | When to Use |
|-------|-------------|
| `researcher` | Research technologies, packages, best practices |
| `Explore` | Find files in codebase, understand structure |
| `planner` | Create implementation plans before complex features |
| `tester` | Generate tests, run tests, report results |
| `debugger` | Investigate errors, find root causes |
| `reviewer` | Review code quality and patterns |
| `docs-manager` | Update documentation |
| `git-manager` | Create commits, handle git operations |

**Agent Communication** via file system:
```
./plans/reports/YYMMDD-from-agent-to-agent-task-report.md
```

---

## Implementation Patterns

### Pattern 1: Creating a Form

```
1. Read: templates/form-template.cs
2. Generate:
   - CustomerForm.cs (Form)
   - ICustomerView.cs (Interface)
   - CustomerPresenter.cs (Presenter)
3. Ensure:
   - MVP pattern
   - Async event handlers
   - Proper Dispose()
```

### Pattern 2: Creating a Service

```
1. Read: templates/service-template.cs
2. Generate:
   - CustomerService.cs
   - ICustomerService.cs
3. Include:
   - Constructor injection
   - Async methods with Async suffix
   - Input validation
   - Try-catch with logging
```

### Pattern 3: Creating a Repository

```
1. Read: templates/repository-template.cs
2. Generate:
   - CustomerRepository.cs
   - ICustomerRepository.cs
3. Use:
   - EF Core DbContext
   - Async CRUD operations
   - No SaveChangesAsync (only in services)
```

---

## Code Quality Rules

### Architecture

```csharp
// CORRECT - MVP Pattern
Form (View) -> Presenter -> Service -> UnitOfWork -> Repository

// CORRECT - Factory Pattern
_formFactory.CreateCustomerForm(id)

// CORRECT - Unit of Work
await _unitOfWork.SaveChangesAsync() // Only in services

// WRONG
_serviceProvider.GetService<Form>() // Never!
await _repository.SaveChangesAsync() // Never in repository!
```

### Error Handling

```csharp
try
{
    await _service.SaveAsync(customer);
    _view.ShowSuccess("Customer saved");
}
catch (ValidationException ex)
{
    _view.ShowError(ex.Message);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Failed to save customer");
    _view.ShowError("An error occurred. Please try again.");
}
```

### Async/Await

```csharp
// CORRECT
public async Task LoadAsync()
{
    var data = await _service.GetAllAsync();
    _view.BindData(data);
}

// WRONG - Blocking call
var data = _service.GetAllAsync().Result; // Never!
```

### Designer Compatibility

```csharp
// CORRECT - All in InitializeComponent
private void InitializeComponent()
{
    this.txtName = new System.Windows.Forms.TextBox();
    this.txtName.Location = new System.Drawing.Point(100, 50);
}

// WRONG - Helper methods
this.Controls.Add(CreateTextBox("Name")); // Designer can't read!
```

---

## Orchestration Protocol

### Sequential Chaining (Dependencies)

```
Research -> Plan -> Implement -> Test -> Review -> Document
```

### Parallel Execution (Independent)

```
       /cook command
            |
    +-------+-------+
    |               |
researcher       Explore
    |               |
    +-------+-------+
            |
        planner
            |
     Implementation
            |
    +-------+-------+
    |               |
 tester         reviewer
    |               |
    +-------+-------+
            |
      docs-manager
            |
      git-manager
```

### Workflow Templates

**Full Feature:**
```
/cook "implement customer search"
├─ Load context
├─ [parallel] researcher + Explore
├─ planner -> creates plan
├─ Implementation (using templates)
├─ dotnet build
├─ tester -> generate + run tests
├─ [if fail] debugger -> fix -> tester (repeat)
├─ reviewer -> review
├─ docs-manager -> update docs
└─ git-manager -> commit
```

**Bug Fix:**
```
/cook "fix null reference in OrderForm"
├─ debugger -> investigate root cause
├─ [main agent] -> implement fix
├─ dotnet build
├─ tester -> run affected tests
├─ reviewer -> quick review
└─ git-manager -> commit
```

---

## Quality Gates

| Phase | Gate |
|-------|------|
| Research | Findings documented |
| Plan | Plan file created |
| Implementation | `dotnet build` succeeds |
| Testing | All tests pass |
| Review | No critical issues |
| Documentation | Relevant docs updated |
| Git | Clean commit created |

---

## Build & Test

### After Every Change
```bash
dotnet build  # Fix errors before continuing
```

### Before Commit
```bash
dotnet build && dotnet test  # Both must pass
```

---

## Commit Rules

### DO
- Run `dotnet build` before commit
- Run `dotnet test` before push
- Write focused, atomic commits
- Use conventional commit format

### DON'T
- Commit with failing tests
- Commit secrets or credentials
- Force push to main/master
- Skip pre-commit hooks

### Commit Format
```
<type>(<scope>): <description>

Types: feat, fix, refactor, docs, test, chore

Examples:
feat(customer): add customer search
fix(order): resolve null reference in calculation
refactor(presenter): extract validation logic
```

### Commit Footer
```
Feature description here

Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>
```

---

## Common Code Snippets

### Async Button Click

```csharp
private async void btnLoad_Click(object sender, EventArgs e)
{
    try
    {
        btnLoad.Enabled = false;
        Cursor = Cursors.WaitCursor;
        var data = await _presenter.LoadDataAsync();
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
private void UpdateUI(string message)
{
    if (InvokeRequired)
    {
        Invoke(new Action(() => UpdateUI(message)));
        return;
    }
    lblStatus.Text = message;
}
```

### Proper Dispose

```csharp
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        _timer?.Dispose();
        _service.DataChanged -= OnDataChanged;
        components?.Dispose();
    }
    base.Dispose(disposing);
}
```

---

## Security Standards

- Parameterized queries (EF Core handles this)
- Validate all user input
- Never log sensitive data
- Don't expose stack traces to users
- Use HTTPS for external APIs

---

## Summary Checklist

For every implementation:

- [ ] Read `.claude/project-context.md`
- [ ] Read `.claude/INDEX.md`
- [ ] Use templates from `./templates/`
- [ ] Follow MVP pattern
- [ ] Use Factory pattern for forms
- [ ] Use Unit of Work for data
- [ ] Write Designer-compatible code
- [ ] Run `dotnet build`
- [ ] Write tests
- [ ] Run `dotnet test`
- [ ] Code review
- [ ] Commit with conventional format
