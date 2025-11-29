# Slash Commands Reference

> Complete reference for all 32 slash commands available in WinForms Coding Standards.

---

## 🍳 Orchestration Commands

### `/cook`
**Full feature implementation workflow**

```bash
/cook "implement customer search functionality"
```

**What it does:**
1. Load project context
2. Research best practices (researcher agent)
3. Find related files (Explore agent)
4. Create implementation plan (planner agent)
5. Implement code following plan
6. Build and verify (`dotnet build`)
7. Generate and run tests (tester agent)
8. Code review (code-reviewer agent)
9. Update docs if needed (docs-manager agent)
10. Commit with approval (git-manager agent)

**Best for:** Complete feature implementation with quality assurance.

---

### `/plan`
**Create implementation plan only**

```bash
/plan "add order management module"
```

**What it does:**
- Research approaches
- Explore codebase for patterns
- Create detailed plan in `plans/YYMMDD-feature-name-plan.md`
- Does NOT implement

**Output:** Plan file with TODO checklist, files to create/modify.

---

### `/plan:two`
**Create plan with 2 alternative approaches**

```bash
/plan:two "implement caching strategy"
```

**What it does:**
- Research multiple approaches
- Compare pros/cons
- Recommend best approach
- Ask for confirmation before implementing

**Best for:** Decisions requiring trade-off analysis.

---

## 🔨 Create Commands

### `/create:form`
**Create new WinForms form with MVP pattern**

```bash
/create:form CustomerEditForm
```

**Creates:**
- `UI/Forms/CustomerEditForm.cs` (Designer-compatible)
- `UI/Forms/CustomerEditForm.Designer.cs`
- `UI/Views/ICustomerEditView.cs`
- `UI/Presenters/CustomerEditPresenter.cs`

---

### `/create:service`
**Create new service class**

```bash
/create:service OrderService
```

**Creates:**
- `Application/Services/IOrderService.cs`
- `Application/Services/OrderService.cs`

**Includes:** DI registration instructions.

---

### `/create:repository`
**Create new repository with Entity Framework**

```bash
/create:repository ProductRepository
```

**Creates:**
- `Domain/Interfaces/IProductRepository.cs`
- `Infrastructure/Persistence/Repositories/ProductRepository.cs`

**Updates:** `IUnitOfWork` interface.

---

### `/create:dialog`
**Create modal dialog form**

```bash
/create:dialog ConfirmDeleteDialog
```

**Creates:** Dialog with OK/Cancel, result handling, validation.

---

### `/create:custom-control`
**Create reusable UserControl**

```bash
/create:custom-control SearchBox
```

**Creates:** Custom UserControl with properties, events, Designer support.

---

## ➕ Add Commands

### `/add:validation`
**Add input validation to a form**

```bash
/add:validation CustomerForm
```

**Adds:**
- Validation logic in Presenter
- Error provider setup
- Validation messages

---

### `/add:error-handling`
**Add comprehensive error handling**

```bash
/add:error-handling OrderService
```

**Adds:**
- Try-catch blocks
- Logging
- User-friendly error messages
- Exception types

---

### `/add:logging`
**Setup logging with Serilog or NLog**

```bash
/add:logging
```

**Configures:**
- Logger setup in `Program.cs`
- Log levels
- File/console sinks
- Structured logging

---

### `/add:settings`
**Setup application configuration**

```bash
/add:settings
```

**Creates:**
- `appsettings.json`
- Settings class
- DI configuration
- User settings management

---

### `/add:data-binding`
**Setup data binding for controls**

```bash
/add:data-binding CustomerForm
```

**Configures:**
- BindingSource
- DataGridView binding
- Two-way binding for edit forms

---

### `/add:test`
**Add tests for a component**

```bash
/add:test CustomerService
```

**Creates:** Unit tests with Moq, covers main scenarios.

---

## 🔧 Fix Commands

### `/fix:bug`
**Smart bug fixer**

```bash
/fix:bug "NullReferenceException in OrderForm line 45"
```

**What it does:**
- Analyze error logs
- Find root cause
- Suggest fixes
- Optionally implement fix

---

### `/fix:threading`
**Fix cross-thread UI access issues**

```bash
/fix:threading CustomerForm
```

**Fixes:**
- Cross-thread operation errors
- Adds proper `Invoke` calls
- Async/await patterns

---

### `/fix:performance`
**Optimize performance**

```bash
/fix:performance ProductListForm
```

**Analyzes:**
- N+1 queries
- Memory leaks
- UI responsiveness
- Suggests optimizations

---

## 🔄 Refactor Commands

### `/refactor:to-mvp`
**Refactor existing code to MVP pattern**

```bash
/refactor:to-mvp CustomerForm
```

**What it does:**
- Extract business logic from Form
- Create View interface
- Create Presenter
- Setup DI

---

## ⚙️ Setup Commands

### `/setup:di`
**Setup Dependency Injection**

```bash
/setup:di
```

**Configures:**
- `Program.cs` with DI container
- Service registration
- Form factory
- DbContext registration

---

## 🔍 Review Commands

### `/review:code`
**Review code for best practices**

```bash
/review:code CustomerService
```

**Checks:**
- MVP pattern compliance
- Error handling
- Async/await usage
- Security issues
- Performance

**Uses:** `code-reviewer` and `winforms-reviewer` agents.

---

### `/review:pr`
**Review pull request**

```bash
/review:pr 123
```

**Reviews:**
- All changed files
- Pattern compliance
- Test coverage
- Breaking changes

---

## 🧪 Quality Commands

### `/test`
**Run tests and analyze results**

```bash
/test                        # All tests
/test CustomerServiceTests   # Specific class
```

**What it does:**
- Run `dotnet test`
- Analyze results
- Report pass/fail
- Suggest fixes for failures

---

### `/debug`
**Debug issues and find root cause**

```bash
/debug "Cross-thread operation error in DataGridView"
```

**What it does:**
- Analyze error
- Find root cause
- Explain why it happens
- Suggest fixes (doesn't auto-implement)

---

### `/watzup`
**Check current work status**

```bash
/watzup
```

**Reports:**
- Current branch status
- Recent commits
- Uncommitted changes
- Build status
- Test status
- Suggested next steps

---

## 📊 Command Categories Summary

| Category | Commands | Purpose |
|----------|----------|---------|
| **Orchestration** | `/cook`, `/plan`, `/plan:two` | Full workflows |
| **Create** | `/create:form`, `/create:service`, `/create:repository`, `/create:dialog`, `/create:custom-control` | Generate new components |
| **Add** | `/add:validation`, `/add:error-handling`, `/add:logging`, `/add:settings`, `/add:data-binding`, `/add:test` | Add features to existing code |
| **Fix** | `/fix:bug`, `/fix:threading`, `/fix:performance` | Fix issues |
| **Refactor** | `/refactor:to-mvp` | Improve code structure |
| **Setup** | `/setup:di` | Configure project |
| **Review** | `/review:code`, `/review:pr` | Quality assurance |
| **Quality** | `/test`, `/debug`, `/watzup` | Testing and status |

---

## 💡 Tips

### Chain Commands
```bash
# Plan first, then implement
/plan "customer search"
# Review plan, then:
/cook "implement customer search following the plan"
```

### Use Specific Commands
```bash
# Instead of /cook for small tasks:
/create:service OrderService
/add:validation OrderForm
/add:test OrderService
```

### Check Status Often
```bash
/watzup  # Before starting work
/test    # After making changes
/watzup  # Before committing
```

---

## 📖 See Also

- [Agents Reference](AGENTS_REFERENCE.md) - All available agents
- [INDEX.md](INDEX.md) - Full resource index
- [START_HERE.md](START_HERE.md) - Quick navigation
