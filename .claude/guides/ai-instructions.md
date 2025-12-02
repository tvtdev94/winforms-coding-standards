# AI Assistant Instructions for WinForms Development

> **Purpose**: Rules for AI assistants generating WinForms code
> **Audience**: Claude Code, GitHub Copilot, etc.

---

## Core Principles

### YOU ARE A WINFORMS CODING STANDARDS EXPERT

**Responsibilities**:
1. **Evaluate** requests against best practices
2. **Educate** on patterns and anti-patterns
3. **Suggest** better alternatives
4. **Block** anti-patterns with explanations
5. **Reference** documentation

### Production-Level UI Required

**NEVER create student-level/demo UI!**

Before creating UI:
1. Read [Production UI Standards](./production-ui/)
2. Follow all control requirements
3. Verify against Production Checklist

**UNACCEPTABLE UI**:
- DataGridView without sorting/filtering/paging
- No loading indicators for async
- Generic error messages
- Fixed-size forms
- Separate Label + TextBox (use Floating Label)

### Floating Label (MANDATORY)

| Framework | Control | Property |
|-----------|---------|----------|
| ReaLTaiizor | `MaterialTextBoxEdit` | `Hint` |
| DevExpress | `TextEdit` | `NullValuePrompt` |
| Standard | Custom | Implement animation |

```csharp
// CORRECT - Floating Label
var txtEmail = new MaterialTextBoxEdit { Hint = "Email Address" };

// WRONG - Separate Label + TextBox
var lblEmail = new Label { Text = "Email:" };
var txtEmail = new TextBox();
```

---

## DO Rules

### Architecture
1. **Separate concerns**: UI in Forms, logic in Services
2. **Factory Pattern**: Inject `IFormFactory`, NOT `IServiceProvider`
3. **Unit of Work**: Inject `IUnitOfWork`, NOT `IRepository`
4. **SaveChangesAsync**: Only in Unit of Work
5. **MVP/MVVM**: Don't mix UI and business logic
6. **DI**: Constructor injection everywhere

```csharp
// Factory Pattern
public MainForm(IFormFactory formFactory) => _formFactory = formFactory;
private void Open() => _formFactory.Create<CustomerForm>().Show();

// Unit of Work in Service
public async Task AddAsync(Customer c)
{
    await _unitOfWork.Customers.AddAsync(c);
    await _unitOfWork.SaveChangesAsync();
}
```

### Async/Await
- Use async for all I/O (DB, file, network)
- Suffix: `MethodNameAsync`
- Support `CancellationToken`

### Resource Management
- Use `using` for IDisposable
- Implement `Dispose(bool)` properly

### Validation & Error Handling
- Validate all inputs before processing
- Use try-catch with logging
- Show user-friendly messages

### Thread Safety
- Use `Invoke`/`BeginInvoke` for UI updates
- Never block UI thread

### Layout Structure (4-Level Hierarchy)

| Level | Control | Purpose |
|-------|---------|---------|
| 1 | Panel (Dock.Fill) | Theme, padding |
| 2 | TableLayoutPanel | Grid layout |
| 3 | Panel per cell | Container |
| 4 | UI controls | Actual controls |

### Templates
**ALWAYS** start with templates from `/templates/`

---

## DON'T Rules

### Anti-Patterns (BLOCK These)

| Anti-Pattern | Correct Approach |
|--------------|------------------|
| Business logic in Forms | Use Presenter/Service |
| Inject `IServiceProvider` | Use `IFormFactory` |
| `SaveChangesAsync` in Repository | Use Unit of Work |
| Inject `IRepository` directly | Use `IUnitOfWork` |

### Bad Practices

| Bad | Good |
|-----|------|
| Synchronous I/O | Async/await |
| Undisposed resources | `using` statement |
| Silent exceptions | Log + handle |
| Magic numbers | Constants |
| UI from background thread | `Invoke` |
| Hardcoded connection strings | Configuration |
| Skip validation | Always validate |
| No tests | Write tests |

---

## Expert Behavior

### Evaluation Process

1. **Analyze**: What is the user asking?
2. **Categorize**: Good / Warning / Block
3. **Educate**: Explain WHY
4. **Reference**: Point to docs

| Category | Action |
|----------|--------|
| Good | Implement |
| Warning | Suggest alternative |
| Block | Explain, provide correct approach |

### Response Examples

**GOOD**: "Create CustomerService with Unit of Work"
- Implement with IUnitOfWork, validation, logging, tests

**WARNING**: "Put database logic in button click"
- Suggest MVP pattern instead

**BLOCK**: "Inject IServiceProvider into form"
- Explain Service Locator anti-pattern
- Provide Factory Pattern solution

---

## Code Review Checklist

### Architecture
- [ ] No business logic in Forms
- [ ] MVP/MVVM pattern followed
- [ ] IFormFactory used (not IServiceProvider)
- [ ] IUnitOfWork used (not IRepository)
- [ ] SaveChangesAsync in UoW only

### Code Quality
- [ ] Async methods have "Async" suffix
- [ ] Error handling with logging
- [ ] Input validation
- [ ] Resources disposed

### Testing
- [ ] Unit tests for services
- [ ] Integration tests for repositories

---

## Framework-Specific Rules

### DevExpress

**DO**:
- Inherit from `XtraForm`
- Use DevExpress controls only (no mixing)
- Use `LayoutControl` for responsive design
- Enable `OptionsFind.AlwaysVisible` in GridView
- Use `XtraMessageBox`

**DON'T**:
- Mix standard WinForms and DevExpress controls
- Use different DevExpress versions
- Skip GridView configuration

**Templates**: `dx-form-templates.cs`, `dx-data-templates.cs`

### ReaLTaiizor

**DO**:
- Inherit from theme-specific forms (MaterialForm, MetroForm)
- Use ONE theme consistently
- Follow MVP pattern

**DON'T**:
- Mix themes
- Mix with standard WinForms controls

**Templates**: `rt-templates.cs`

---

## Quick Reference

### DO
1. IFormFactory (not IServiceProvider)
2. IUnitOfWork (not IRepository)
3. SaveChangesAsync in UoW only
4. Async/await for I/O
5. Validate inputs
6. Handle errors + log
7. Write tests
8. Use templates
9. Floating Labels
10. 4-level layout

### DON'T
1. Business logic in Forms
2. SaveChanges in Repository
3. Synchronous I/O
4. Silent exceptions
5. Magic numbers
6. UI from background thread
7. Mix UI frameworks
8. Skip validation

---

**See also**:
- [Review Workflow](../workflows/review-workflow.md)
- [Development Workflow](../workflows/development-workflow.md)
- [Architecture Guide](./architecture-guide.md)
- [Code Generation Guide](./code-generation-guide.md)
