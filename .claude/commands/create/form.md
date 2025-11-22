# Create WinForms Form with MVP Pattern

You are asked to create a new WinForm following MVP pattern. Follow these steps systematically:

---

## 🔥 STEP 0: MANDATORY Context Loading (DO THIS FIRST!)

**Before ANY code generation, you MUST:**

### 1. Read Project Configuration
```
READ: .claude/project-context.md
```
Extract: `UI_FRAMEWORK`, `DATABASE`, `PATTERN`, `FRAMEWORK`

### 2. Load Correct Form Template

| UI Framework | Template to Use |
|--------------|-----------------|
| **Standard** | `templates/form-template.cs` |
| **DevExpress** | `templates/dx-form-template.cs` |
| **ReaLTaiizor** | `templates/rt-material-form-template.cs` |

### 3. Load Required Guides
- `docs/patterns/mvp-pattern.md` → MVP rules
- `.claude/guides/production-ui-standards.md` → UI quality
- `docs/ui/responsive-layout.md` → Layout patterns

### 4. Critical Rules

| 🚫 NEVER | ✅ ALWAYS |
|----------|----------|
| Inject IServiceProvider | Use IFormFactory |
| Business logic in Form | Logic in Presenter |
| Separate Label + TextBox | Floating Label/Hint |
| Generate without template | Start from template |

**⚠️ If project-context.md doesn't exist**: Default to ReaLTaiizor Material.

---

## Steps

1. **Check architecture documentation**
   - Read docs/architecture/mvp-pattern.md
   - Review docs/architecture/dependency-injection.md

2. **Confirm form details with user**
   - Form name and purpose
   - Data entities involved
   - Required fields and controls

3. **Create the View Interface** (IYourFormView.cs)
   - Properties for all form fields
   - Properties for UI state (IsLoading, IsButtonEnabled, etc.)
   - Events (LoadRequested, SaveRequested, etc.)
   - Methods (ShowError, ShowSuccess, Close)

4. **Create the Presenter** (YourFormPresenter.cs)
   - Constructor with IService dependencies
   - AttachView and DetachView methods
   - Event handlers for View events
   - Validation logic
   - Call services for business operations

5. **Create the Form** (YourForm.cs)
   - Implement IYourFormView interface
   - Constructor injection of Presenter
   - Wire UI controls to View properties
   - Wire UI events to View events
   - Minimal code - delegate to Presenter

6. **Register with DI Container**
   - Add Presenter to services
   - Add Form to services
   - Update FormFactory if needed

7. **Add XML Comments**
   - Document all public APIs
   - Follow docs/conventions/comments-docstrings.md

8. **Create Unit Tests**
   - Test Presenter with mocked IView
   - Test validation logic
   - Test error handling

## Checklist

Before marking complete, verify:

- [ ] IView interface created with all properties/events
- [ ] Presenter has no UI dependencies (only IView reference)
- [ ] Form implements IView interface
- [ ] Form code-behind is minimal (< 50 lines)
- [ ] All business logic in Presenter
- [ ] Presenter uses Services (not direct data access)
- [ ] XML comments on public APIs
- [ ] Unit tests for Presenter created
- [ ] Registered in DI container
- [ ] Follows naming conventions (docs/conventions/naming-conventions.md)
- [ ] Async/await used for I/O operations

## Example Reference

See docs/examples/mvp-example.md for complete working example.

## Common Mistakes to Avoid

❌ Don't put business logic in Form
❌ Don't reference concrete Form in Presenter
❌ Don't forget to DetachView in Form.Dispose
❌ Don't forget async/await for I/O operations
❌ Don't skip validation in Presenter
❌ Don't skip unit tests

## After Creation

1. Show the user the created files
2. Explain the architecture
3. Suggest next steps (e.g., add to main form, create tests)
