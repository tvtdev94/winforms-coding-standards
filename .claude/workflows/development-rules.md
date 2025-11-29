# Development Rules for C# WinForms

## General Principles

### Holy Trinity
- **YAGNI**: You Aren't Gonna Need It - don't over-engineer
- **KISS**: Keep It Simple, Stupid - prefer simple solutions
- **DRY**: Don't Repeat Yourself - extract common code

### File Size Management
- Keep files under 500 lines when practical
- Split large forms into UserControls
- Extract utility functions to separate classes
- Create dedicated service classes for complex logic

## Context Loading (ALWAYS FIRST)

Before ANY implementation:

1. **Read `.claude/project-context.md`**:
   - UI Framework: Standard / DevExpress / ReaLTaiizor
   - Database: SQLite / SQL Server / PostgreSQL
   - Framework: .NET 8 / .NET Framework 4.8
   - Pattern: MVP / MVVM

2. **Read `.claude/INDEX.md`**:
   - Find relevant templates
   - Find applicable guides
   - Find documentation

3. **Never guess** - always check context first

## Subagents

Delegate tasks to appropriate subagents:

| Agent | When to Use |
|-------|-------------|
| `researcher` | Research technologies, packages, best practices |
| `Explore` | Find files in codebase, understand structure |
| `planner` | Create implementation plans before complex features |
| `tester` | Generate tests, run tests, report results |
| `debugger` | Investigate errors, find root causes |
| `code-reviewer` | Review code quality and patterns |
| `winforms-reviewer` | WinForms-specific review |
| `mvp-validator` | Validate MVP pattern compliance |
| `docs-manager` | Update documentation |
| `git-manager` | Create commits, handle git operations |

### Agent Communication

Use file system for agent-to-agent reports:
```
./plans/reports/YYMMDD-from-agent-to-agent-task-report.md
```

## Code Quality Guidelines

### Architecture

```csharp
// ✅ CORRECT - MVP Pattern
Form (View) → Presenter → Service → UnitOfWork → Repository

// ✅ CORRECT - Factory Pattern
_formFactory.CreateCustomerForm(id)

// ✅ CORRECT - Unit of Work
await _unitOfWork.SaveChangesAsync() // Only in services

// ❌ WRONG
_serviceProvider.GetService<Form>() // Never!
await _repository.SaveChangesAsync() // Never in repository!
```

### Error Handling

```csharp
// ✅ CORRECT
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
// ✅ CORRECT
public async Task LoadAsync()
{
    var data = await _service.GetAllAsync();
    _view.BindData(data);
}

// ❌ WRONG - Blocking call
public void Load()
{
    var data = _service.GetAllAsync().Result; // Never!
}
```

### Designer Compatibility

```csharp
// ✅ CORRECT - All in InitializeComponent
private void InitializeComponent()
{
    this.txtName = new System.Windows.Forms.TextBox();
    this.txtName.Location = new System.Drawing.Point(100, 50);
    this.Controls.Add(this.txtName);
}

// ❌ WRONG - Helper methods
private void InitializeComponent()
{
    this.Controls.Add(CreateTextBox("Name")); // Designer can't read!
}
```

## Build & Test

### After Every Change

```bash
# 1. Build to check for errors
dotnet build

# 2. If build fails, fix before continuing
```

### Before Commit

```bash
# 1. Build
dotnet build

# 2. Run tests
dotnet test

# 3. All must pass
```

## Pre-Commit Rules

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

## Security Standards

- Use parameterized queries (EF Core handles this)
- Validate all user input
- Never log sensitive data
- Don't expose stack traces to users
- Use HTTPS for external APIs

## Commit Message Format

```
<type>(<scope>): <description>

Types: feat, fix, refactor, docs, test, chore

Examples:
feat(customer): add customer search
fix(order): resolve null reference in calculation
refactor(presenter): extract validation logic
```

## Documentation

### When to Update

- After significant feature additions
- After architectural changes
- When adding new patterns or components
- After major bug fixes

### Files to Consider

- `./docs/codebase-summary.md` - Overall structure
- Feature-specific docs if they exist
- README.md for setup changes

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
- [ ] Code review (use agents)
- [ ] Commit with conventional format
