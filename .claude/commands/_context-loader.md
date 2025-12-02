# Shared Context & Rules Loader

> **Include this at the start of commands that write code**

---

## Step 0: Load Context (MANDATORY)

**Before ANY code generation:**

1. **Read `.claude/project-context.md`** - Project config
2. **Read `.claude/INDEX.md`** - Find relevant resources

---

## Quick Reference

### Project Configuration (from project-context.md)

| Setting | Values |
|---------|--------|
| `UI_FRAMEWORK` | Standard / DevExpress / ReaLTaiizor |
| `DATABASE` | SQLite / SQL Server / PostgreSQL |
| `PATTERN` | MVP / MVVM |
| `FRAMEWORK` | .NET 8 / .NET Framework 4.8 |

### Templates by UI Framework

| UI Framework | Form Template | Additional |
|--------------|---------------|------------|
| **Standard** | `form-template.cs` | `service-template.cs` |
| **DevExpress** | `dx-form-templates.cs` | `dx-data-templates.cs` |
| **ReaLTaiizor** | `rt-templates.cs` | - |

### Critical Rules

| NEVER | ALWAYS |
|-------|--------|
| Inject IServiceProvider | Use IFormFactory |
| Inject IRepository directly | Use IUnitOfWork |
| SaveChanges in Repository | SaveChanges in UoW |
| Business logic in Forms | Logic in Presenter |
| Separate Label + TextBox | Floating Label/Hint |
| Generate without template | Start from template |
| Skip validation | Validate all inputs |
| Helper methods in Designer | Inline all UI code |

### If project-context.md doesn't exist

Ask user for UI framework preference. Do NOT assume any default.

---

## Commands That Load Context First

| Command | Reason |
|---------|--------|
| `/cook` | Full workflow - needs all context |
| `/plan` | Plans must follow rules |

## Commands That DON'T Need Full Context

| Command | Reason |
|---------|--------|
| `/watzup` | Only shows git status |
| `/test` | Runs existing tests |
| `/debug` | Investigates issues |
