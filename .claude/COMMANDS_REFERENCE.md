# Slash Commands Reference

> Reference for the 5 core slash commands available in WinForms Coding Standards.

---

## Core Commands

### `/cook`
**Full feature implementation workflow**

```bash
/cook "implement customer search functionality"
/cook "create OrderForm with CRUD"
/cook "add validation to CustomerForm"
/cook "fix threading issue in DataGridView"
```

**What it does:**
1. Load project context
2. Research best practices
3. Find related files in codebase
4. Create implementation plan
5. Implement code following plan
6. Build and verify (`dotnet build`)
7. Generate and run tests
8. Code review
9. Commit with approval

**Best for:** Any feature, fix, or enhancement that involves writing code.

---

### `/plan`
**Create implementation plan only**

```bash
/plan "add order management module"
/plan "refactor to MVP pattern"
```

**What it does:**
- Research approaches
- Explore codebase for patterns
- Create detailed plan in `plans/YYMMDD-feature-name-plan.md`
- Does NOT implement

**Output:** Plan file with TODO checklist, files to create/modify.

**Best for:** Complex features that need discussion before implementing.

---

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
/debug "NullReferenceException in OrderForm line 45"
```

**What it does:**
- Analyze error
- Find root cause
- Explain why it happens
- Suggest fixes (doesn't auto-implement unless requested)

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

## Usage Examples

### Creating Components
```bash
# Use /cook for any creation task
/cook "create CustomerForm with search and grid"
/cook "create OrderService with CRUD operations"
/cook "create ProductRepository"
```

### Adding Features
```bash
# Use /cook for adding features
/cook "add validation to CustomerForm"
/cook "add logging to the application"
/cook "add error handling to OrderService"
```

### Fixing Issues
```bash
# Use /cook or /debug
/cook "fix threading issue in ProductForm"
/debug "why is the grid not refreshing?"
```

### Workflow
```bash
/watzup              # Check status
/plan "new feature"  # Plan first
/cook "implement..." # Implement
/test                # Verify
/watzup              # Check before commit
```

---

## Tips

1. **Use `/cook` for most tasks** - It handles create, add, fix, refactor
2. **Use `/plan` for complex features** - Get a plan first, then `/cook`
3. **Use `/watzup` often** - Check status before/after work
4. **Use `/debug` for investigation** - When you need to understand an issue

---

## See Also

- [INDEX.md](INDEX.md) - Full resource index
- [AGENTS_REFERENCE.md](AGENTS_REFERENCE.md) - All agents
- [CLAUDE.md](../CLAUDE.md) - Main rules reference
