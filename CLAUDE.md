# C# WinForms Coding Standards - Claude Code Guide

> Quick reference for AI assistants building WinForms applications
> **Version**: 5.8.0 | **Updated**: 2025-12-02

---

## Step 0: ALWAYS Read First

**Before ANY task**, you **MUST**:

1. **Read `.claude/project-context.md`** - Project config (UI framework, database, patterns)
2. **Read `.claude/INDEX.md`** - Find relevant guides/docs/templates for task

> **CRITICAL**: NEVER generate code from scratch - ALWAYS start with templates from INDEX.md!

---

## Tech Stack

| Component | Default |
|-----------|---------|
| .NET | 8.0 (or 4.8 Framework) |
| C# | 12.0 |
| ORM | Entity Framework Core 8.0 |
| Testing | xUnit + Moq |
| DI | Microsoft.Extensions.DependencyInjection |
| Logging | Serilog |

**UI Framework**: Configured in `project-context.md` (Standard / DevExpress / ReaLTaiizor)

---

## Critical Rules

### ALWAYS DO

1. **Use `IFormFactory`** - NOT `IServiceProvider`
2. **Use `IUnitOfWork`** - NOT `IRepository` directly
3. **`SaveChangesAsync` in UoW only** - NEVER in repositories
4. **Async/await** for all I/O
5. **Validate inputs** before processing
6. **Use templates** from `/templates/` folder
7. **Follow MVP pattern** - Separate UI from business logic
8. **Write tests** - Unit tests for services
9. **Floating labels** - NOT separate Label + TextBox
10. **Designer-compatible code** - All UI in `InitializeComponent()`

### NEVER DO

1. Business logic in Forms
2. `SaveChangesAsync` in repositories
3. Inject `IServiceProvider` directly
4. Synchronous I/O
5. Magic strings/numbers
6. UI updates from background threads
7. Skip validation
8. Helper methods in `InitializeComponent()` (Designer can't parse them)
9. Separate Label + TextBox (use Floating Label)
10. Generate code without using templates

---

## Architecture Quick Reference

### Patterns (Required)
- **MVP** - View + Presenter + Service
- **Factory** - `IFormFactory` for form creation
- **Unit of Work** - `IUnitOfWork` for transactions
- **DI** - Constructor injection everywhere

### Project Structure
```
/Domain/        → Models, Interfaces, Enums
/Application/   → Services, Validators
/Infrastructure/Persistence/ → Repositories, DbContext, UnitOfWork
/UI/            → Forms, Presenters, Views, Factories
```
> Full docs: [project-structure.md](docs/architecture/project-structure.md)

### Naming Conventions
```
Classes/Methods: PascalCase    Variables: camelCase
Constants: UPPER_SNAKE_CASE    Async: MethodNameAsync
Controls: prefix+Name (btnSave, txtEmail, dgvCustomers)
```

---

## UI Rules

### Floating Labels (Required)

```csharp
// CORRECT - Use Hint/Placeholder
var txtEmail = new MaterialTextBoxEdit { Hint = "Email Address" };

// WRONG - Separate label wastes space
var lblEmail = new Label { Text = "Email:" };
var txtEmail = new TextBox();
```

| Framework | Control | Property |
|-----------|---------|----------|
| ReaLTaiizor | `MaterialTextBoxEdit` | `Hint` |
| DevExpress | `TextEdit` | `NullValuePrompt` |
| Standard | `FloatingLabelTextBox` | Custom |

### Designer Compatibility

All UI code MUST be in `InitializeComponent()` for Visual Studio Designer:
- NO helper methods (`CreateLabel()`, `CreateButton()`)
- Use fully qualified names (`System.Windows.Forms.Panel`)
- Follow SuspendLayout/ResumeLayout pattern

> Full guide: [.claude/guides/code-generation-guide.md](.claude/guides/code-generation-guide.md)

---

## Documentation Rule

**ASK before creating docs** (spec.md, tasks.md, etc.)
- Does NOT apply to: `.cs`, `.csproj`, test files

---

## Commands & Agents

### Commands (6 Core)
| Command | Purpose |
|---------|---------|
| `/cook [feature]` | Full workflow: plan → implement → test → review |
| `/plan [feature]` | Create implementation plan only |
| `/fix [bug]` | Fix bugs with streamlined workflow |
| `/test` | Run tests and report results |
| `/debug [issue]` | Debug issues and find root cause |
| `/watzup` | Review recent changes and status |

> **Tip**: `/cook` for features, `/fix` for bugs

### Key Agents
| Agent | Purpose |
|-------|---------|
| `reviewer` | Code review, MVP validation, WinForms best practices |
| `tester` | Generate and run tests |
| `planner` | Create implementation plans |
| `debugger` | Debug issues |

> Full list: [.claude/INDEX.md](.claude/INDEX.md)

---

## Quick Commands

```bash
dotnet build              # Build
dotnet test               # Run tests
dotnet clean && dotnet build  # Clean rebuild
```

---

## Resources

| Need | Resource |
|------|----------|
| Find what to read | [.claude/INDEX.md](.claude/INDEX.md) |
| Architecture guide | [.claude/guides/architecture-guide.md](.claude/guides/architecture-guide.md) |
| UI standards | [.claude/guides/production-ui/](.claude/guides/production-ui/) |
| Full docs | [docs/00-overview.md](docs/00-overview.md) |
| Examples | [example-project/](example-project/) |
