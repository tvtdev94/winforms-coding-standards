# Shared Context & Rules Loader

> **⚠️ INCLUDE THIS AT THE START OF EVERY COMMAND THAT WRITES CODE**

---

## 🔥 STEP 0: Load Rules (MANDATORY - ALWAYS FIRST!)

**Before ANY code generation or modification, spawn `rules-loader` agent:**

```
Use Task tool with subagent_type="rules-loader" to:
- Load .claude/project-context.md
- Load ALL coding rules from CLAUDE.md, guides/, workflows/
- Generate rules summary for current task
```

**Why**: Rules are scattered across ~85,000+ lines. Agent must understand rules BEFORE coding.

---

## Quick Reference (After rules-loader completes)

### Project Configuration (from project-context.md)

| Setting | Values |
|---------|--------|
| `UI_FRAMEWORK` | Standard / DevExpress / ReaLTaiizor |
| `DATABASE` | SQLite / SQL Server / PostgreSQL |
| `PATTERN` | MVP / MVVM |
| `FRAMEWORK` | .NET 8 / .NET Framework 4.8 |

### Templates by UI Framework

| UI Framework | Form Template | Grid Template | Additional |
|--------------|---------------|---------------|------------|
| **Standard** | `form-template.cs` | N/A | `service-template.cs` |
| **DevExpress** | `dx-form-template.cs` | `dx-grid-template.cs` | `dx-lookup-template.cs` |
| **ReaLTaiizor** | `rt-material-form-template.cs` | N/A | `rt-controls-patterns.cs` |

### Critical Rules (MUST FOLLOW)

```
┌─────────────────────────────────────────────────────────┐
│ 🚫 NEVER DO                     │ ✅ ALWAYS DO          │
├─────────────────────────────────┼───────────────────────┤
│ Inject IServiceProvider         │ Use IFormFactory      │
│ Inject IRepository directly     │ Use IUnitOfWork       │
│ SaveChanges in Repository       │ SaveChanges in UoW    │
│ Business logic in Forms         │ Logic in Presenter    │
│ Separate Label + TextBox        │ Floating Label/Hint   │
│ Generate code without template  │ Start from template   │
│ Skip validation                 │ Validate all inputs   │
│ Ignore async/await              │ Async for all I/O     │
│ Helper methods in Designer      │ Inline all UI code    │
└─────────────────────────────────┴───────────────────────┘
```

### If project-context.md doesn't exist

Ask user for UI framework preference. Do NOT assume any default.

---

## Commands That MUST Load Rules First

| Category | Commands | Reason |
|----------|----------|--------|
| **Create** | `/create:form`, `/create:service`, `/create:repository`, `/create:presenter`, `/create:dialog`, `/create:custom-control` | Writing new code |
| **Add** | `/add:validation`, `/add:data-binding`, `/add:logging`, `/add:settings`, `/add:error-handling` | Modifying code |
| **Fix** | `/fix:bug`, `/fix:threading`, `/fix:performance` | Need rules to fix correctly |
| **Refactor** | `/refactor:to-mvp` | Must follow MVP rules |
| **Setup** | `/setup:di` | DI has specific patterns |
| **Review** | `/review:code`, `/review:pr` | Need rules to review against |
| **Plan** | `/plan`, `/plan:two`, `/cook` | Plans must follow rules |

## Commands That DON'T Need Rules

| Command | Reason |
|---------|--------|
| `/watzup` | Only shows git status |
| `/debug` | Investigates issues (but benefits from context) |
| `/test` | Runs existing tests |

---

## How to Include in Commands

Add this at the start of any command that writes code:

```markdown
## Step 0: Load Rules (MANDATORY)

Use **`rules-loader` subagent** to load all coding rules before proceeding.

Wait for rules summary before continuing to next step.
```
