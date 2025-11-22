# Shared Context Loader

> **⚠️ INCLUDE THIS AT THE START OF EVERY COMMAND**

---

## 🔥 STEP 0: MANDATORY Context Loading (DO THIS FIRST!)

**Before ANY analysis or code generation, you MUST load context:**

### 1. Read Project Configuration
```
READ: .claude/project-context.md
```
Extract:
- `UI_FRAMEWORK` → Standard / DevExpress / ReaLTaiizor
- `DATABASE` → SQLite / SQL Server / PostgreSQL
- `PATTERN` → MVP / MVVM
- `FRAMEWORK` → .NET 8 / .NET Framework 4.8

### 2. Load Templates Based on UI Framework

| UI Framework | Form Template | Grid Template | Additional |
|--------------|---------------|---------------|------------|
| **Standard** | `form-template.cs` | N/A | `service-template.cs` |
| **DevExpress** | `dx-form-template.cs` | `dx-grid-template.cs` | `dx-lookup-template.cs` |
| **ReaLTaiizor** | `rt-material-form-template.cs` | N/A | `rt-controls-patterns.cs` |

### 3. Critical Rules Summary (MUST FOLLOW)

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
└─────────────────────────────────┴───────────────────────┘
```

### 4. If project-context.md doesn't exist

Ask user for UI framework preference, default to **ReaLTaiizor Material**.

---
