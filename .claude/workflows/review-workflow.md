# Code Review Workflow

Complete guide for reviewing WinForms code - self-review, file review, and PR review.

---

## Pre-Commit Checklist

### 1. Build & Test
```bash
dotnet build    # No errors, no warnings
dotnet test     # All tests pass
```

### 2. Architecture
- [ ] No business logic in Forms (use Services/Presenters)
- [ ] MVP/MVVM pattern followed
- [ ] Services use dependency injection
- [ ] Repositories use EF Core correctly
- [ ] No direct database access from Forms

### 3. Resource Management
- [ ] All IDisposable disposed (`using` or Dispose())
- [ ] Form.Dispose() releases timers, subscriptions
- [ ] Database connections closed
- [ ] No memory leaks (events unsubscribed)

### 4. Async/Await
- [ ] Async for all I/O (database, file, network)
- [ ] Async methods named with `Async` suffix
- [ ] No blocking: `.Result`, `.Wait()`
- [ ] CancellationToken for long operations

### 5. Input Validation
- [ ] User input validated before processing
- [ ] ErrorProvider for WinForms validation
- [ ] Null checks with ArgumentNullException
- [ ] Business rules in Service layer

### 6. Error Handling
- [ ] Try-catch around risky operations
- [ ] Exceptions logged with ILogger
- [ ] User-friendly error messages
- [ ] Specific exceptions caught (not just `Exception`)

### 7. Thread Safety
- [ ] UI updates on UI thread (Invoke/BeginInvoke)
- [ ] No UI controls from background threads
- [ ] Async preferred over BackgroundWorker

### 8. Code Quality
- [ ] No magic numbers/strings (use constants)
- [ ] Proper naming conventions
- [ ] No commented-out code
- [ ] XML documentation on public APIs

### 9. Security
- [ ] No hardcoded credentials
- [ ] Parameterized queries (EF Core)
- [ ] Input sanitized
- [ ] No secrets in source control

### 10. Performance
- [ ] No N+1 queries (use Include())
- [ ] Large lists virtualized
- [ ] No unnecessary database calls

---

## Issue Severity

| Level | Description | Examples | Action |
|-------|-------------|----------|--------|
| **Critical** | Security, data loss, crash | SQL injection, memory leak, business logic in Form | Must fix |
| **Major** | Pattern violation, bugs | Missing async, no validation, thread issues | Should fix |
| **Minor** | Style, improvements | Missing docs, formatting, refactor opportunity | Nice to fix |

---

## PR Review Process

### Phase 1: Preparation (2 min)

```bash
git fetch origin
git diff origin/main...origin/feature-branch --stat
gh pr view <PR-number>  # If GitHub
```

**Understand:**
- PR title & description (what/why)
- Files changed & LOC
- Related issues

### Phase 2: Initial Assessment (3 min)

**Quick Sanity Checks:**
- Code compiles? `dotnet build`
- Tests pass? `dotnet test`
- PR size reasonable? (< 500 LOC preferred)
- Files in correct folders?

**Red Flags** (immediate feedback):
- Build failures
- Test failures
- Merge conflicts
- Hardcoded credentials
- Missing tests for new features

### Phase 3: Detailed Review (15-20 min)

Review by file type:

**Forms:**
- [ ] Implements IView interface
- [ ] No business logic
- [ ] Async event handlers for I/O
- [ ] Thread-safe UI updates
- [ ] Proper Dispose()

**Services:**
- [ ] Constructor injection
- [ ] Input validation
- [ ] Async methods
- [ ] Error handling with logging
- [ ] No UI dependencies

**Repositories:**
- [ ] EF Core async methods
- [ ] No business logic
- [ ] Interface defined
- [ ] No SaveChangesAsync (only in services)

**Tests:**
- [ ] AAA pattern
- [ ] Good naming: `Method_Scenario_Expected`
- [ ] Mocking with Moq
- [ ] Tests success + failure paths

### Phase 4: Feedback (5 min)

**Categorize issues:**
- Critical (must fix)
- Major (should fix)
- Minor (nice to have)

**Provide positive feedback too:**
- Good test coverage
- Clean implementation
- Proper error handling

### Phase 5: Decision

| Decision | When |
|----------|------|
| **APPROVE** | No critical, 0-2 minor |
| **REQUEST CHANGES** | 1+ critical or 3+ major |
| **COMMENT** | Suggestions only |

---

## Common Anti-Patterns

### 1. Business Logic in Forms

```csharp
// BAD
private void btnSave_Click(object sender, EventArgs e)
{
    using var db = new AppDbContext();
    db.Customers.Add(new Customer { Name = txtName.Text });
    db.SaveChanges();
}

// GOOD
private async void btnSave_Click(object sender, EventArgs e)
{
    await _presenter.SaveCustomerAsync();
}
```

### 2. Synchronous I/O

```csharp
// BAD
var data = File.ReadAllText(path);
var customers = _context.Customers.ToList();

// GOOD
var data = await File.ReadAllTextAsync(path);
var customers = await _context.Customers.ToListAsync();
```

### 3. Missing Dispose

```csharp
// BAD
private Timer _timer = new Timer();
// Never disposed

// GOOD
protected override void Dispose(bool disposing)
{
    if (disposing) _timer?.Dispose();
    base.Dispose(disposing);
}
```

### 4. Cross-Thread UI Access

```csharp
// BAD
Task.Run(() => lblStatus.Text = "Done"); // Exception!

// GOOD
if (InvokeRequired)
    Invoke(() => lblStatus.Text = "Done");
else
    lblStatus.Text = "Done";
```

### 5. Blocking Async

```csharp
// BAD
var data = _service.GetDataAsync().Result;

// GOOD
var data = await _service.GetDataAsync();
```

### 6. Swallowed Exceptions

```csharp
// BAD
try { /* ... */ }
catch { }

// GOOD
catch (Exception ex)
{
    _logger.LogError(ex, "Operation failed");
    throw;
}
```

### 7. Magic Numbers

```csharp
// BAD
if (customer.Age < 18) { }

// GOOD
private const int MINIMUM_AGE = 18;
if (customer.Age < MINIMUM_AGE) { }
```

---

## Expert Evaluation Framework

### When Request Follows Best Practices

```
"Great approach! This follows [pattern] because..."
[Implement the request]
```

### When Request Has Issues

```
"I notice [X]. While this works, it may cause [issue]:
- [Reason with explanation]

Better approach:
[Code example]

Should I implement the recommended approach?"
```

### When Request Violates Standards

```
"This approach has significant issues:

Problems:
1. [Critical issue] - violates [principle]
2. [Issue] - Microsoft docs warn against

Recommended:
[Better solution with code]

References:
- docs/architecture/mvp-pattern.md

Implement recommended approach?"
```

---

## Key Principles to Enforce

1. **Separation of Concerns** - UI / Business / Data layers
2. **SOLID** - Single Responsibility, Dependency Inversion, etc.
3. **Async/Await** - All I/O must be async
4. **Dependency Injection** - Constructor injection, interfaces
5. **Testability** - Code must be unit testable
6. **Resource Management** - Dispose all IDisposable
7. **Error Handling** - Never swallow, always log
8. **Security** - Validate input, no hardcoded secrets

---

## Balancing Pragmatism

**Allow shortcuts for:**
- Prototypes/POCs
- Small utility apps (< 5 forms)
- Learning projects
- Time-constrained demos

**Enforce standards for:**
- Production applications
- Team projects
- Long-term maintained apps
- Enterprise software

---

## Review Comment Format

```markdown
### [Issue Title]

**Severity**: Critical / Major / Minor
**Location**: `path/to/file.cs:123`

**Issue**: [Description]
**Impact**: [Why it matters]
**Fix**:
```csharp
// Recommended code
```

**Reference**: [docs/path/to/guide.md]
```

---

## Review Best Practices

**DO:**
- Be specific with file:line references
- Explain WHY something is an issue
- Provide code examples for fixes
- Balance criticism with positive feedback
- Reference documentation
- Respond quickly (< 24h)

**DON'T:**
- Be vague ("this is bad")
- Only point out problems
- Be harsh or condescending
- Block on personal preferences
- Nitpick formatting (use linter)
