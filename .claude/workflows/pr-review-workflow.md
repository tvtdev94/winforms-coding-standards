# Pull Request Review Workflow

Complete guide for reviewing Pull Requests in WinForms projects following coding standards.

---

## Overview

This workflow guides you through reviewing a team member's Pull Request to ensure:
- Code quality and maintainability
- Adherence to WinForms coding standards
- Proper implementation of MVP/MVVM patterns
- Security and performance best practices
- Adequate test coverage

**Target Audience**: Code reviewers, tech leads, senior developers

**Time Estimate**: 15-30 minutes per PR (depending on size)

---

## Review Process Stages

```
1. Preparation (5 min)
   ↓
2. Initial Assessment (5 min)
   ↓
3. Detailed Code Review (10-20 min)
   ↓
4. Testing & Verification (5 min)
   ↓
5. Feedback & Decision (5 min)
```

---

## Stage 1: Preparation (5 minutes)

### 1.1 Load Review Context

Before starting review, have these resources ready:

**Essential Reading**:
- `.claude/workflows/code-review-checklist.md` - Complete checklist
- `.claude/workflows/expert-behavior-guide.md` - Standards evaluation guide
- `templates/review-comment-templates.md` - Comment templates

**Reference Documentation**:
- Architecture: `docs/architecture/mvp-pattern.md`
- Conventions: `docs/conventions/naming-conventions.md`
- Best Practices: `docs/best-practices/`

### 1.2 Understand PR Context

Get information:
- **PR Title**: What is being implemented?
- **PR Description**: Why? What problem does it solve?
- **Related Issues**: Linked Jira/GitHub issues
- **Author**: Who submitted? (Adjust feedback tone accordingly)
- **Size**: How many files changed? LOC added/removed?

**Quick Check**:
```bash
# View PR info (if using GitHub CLI)
gh pr view <PR-number>

# Or check git diff
git fetch origin
git diff origin/main...origin/feature-branch --stat
```

### 1.3 Set Review Expectations

Based on PR type:

| PR Type | Focus Areas | Expected Time |
|---------|-------------|---------------|
| **New Feature** | Architecture, tests, documentation | 20-30 min |
| **Bug Fix** | Root cause, tests, regression | 10-15 min |
| **Refactoring** | No behavior change, patterns | 15-20 min |
| **UI Changes** | UX, validation, thread safety | 15-20 min |
| **Performance** | Benchmarks, profiling data | 20-30 min |

---

## Stage 2: Initial Assessment (5 minutes)

### 2.1 High-Level Overview

**Quick Scan**:
1. Read commit messages - Do they explain what/why?
2. Check files changed - Are they in correct folders?
3. Spot check 2-3 key files - Overall code quality?
4. Check if tests are included

**Red Flags** (immediate feedback):
- ❌ Code doesn't compile
- ❌ Tests failing
- ❌ Merge conflicts
- ❌ Massive PR (>500 LOC) - Request splitting
- ❌ Missing tests for new features
- ❌ Hardcoded credentials/secrets

**If red flags found**: Stop review, provide immediate feedback to author.

### 2.2 Architecture Sanity Check

Quick validation:
- [ ] New Forms go in `/Forms/` folder
- [ ] Business logic in `/Services/` folder
- [ ] Data access in `/Repositories/` folder
- [ ] Tests in `/Tests/` folder
- [ ] No mixing of concerns across layers

**Decision Point**:
- ✅ **Proceed** if structure looks reasonable
- ⚠️ **Request Changes** if fundamental architecture issues

---

## Stage 3: Detailed Code Review (10-20 minutes)

### 3.1 Review by File Type

For each changed file, apply specific checklist:

#### **Forms Review (MVP Pattern)**

```csharp
// ✅ GOOD Example
public partial class CustomerForm : Form, ICustomerView
{
    private readonly ICustomerPresenter _presenter;

    public CustomerForm(ICustomerPresenter presenter)
    {
        InitializeComponent();
        _presenter = presenter;
        _presenter.AttachView(this);
    }

    // Minimal logic - just wire UI to presenter
    private async void btnSave_Click(object sender, EventArgs e)
    {
        await SaveRequested?.Invoke();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _presenter?.DetachView();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
```

**Check**:
- [ ] Implements IView interface
- [ ] No business logic (just UI wiring)
- [ ] Constructor injection
- [ ] DetachView in Dispose()
- [ ] Async event handlers for I/O
- [ ] Thread-safe UI updates

#### **Services Review (Business Logic)**

```csharp
// ✅ GOOD Example
public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(
        ICustomerRepository repository,
        ILogger<CustomerService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Customer> CreateCustomerAsync(Customer customer)
    {
        ArgumentNullException.ThrowIfNull(customer);

        if (string.IsNullOrWhiteSpace(customer.Name))
            throw new ValidationException("Customer name is required");

        try
        {
            var created = await _repository.AddAsync(customer);
            _logger.LogInformation("Customer created: {CustomerId}", created.Id);
            return created;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create customer");
            throw new ServiceException("Failed to create customer", ex);
        }
    }
}
```

**Check**:
- [ ] Interface defined
- [ ] Constructor injection
- [ ] Input validation
- [ ] Async methods with Async suffix
- [ ] Error handling with logging
- [ ] No UI dependencies
- [ ] Unit testable

#### **Repositories Review (Data Access)**

```csharp
// ✅ GOOD Example
public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Customer>> GetAllAsync()
    {
        return await _context.Customers
            .Include(c => c.Orders)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Customer> AddAsync(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return customer;
    }
}
```

**Check**:
- [ ] Interface defined
- [ ] EF Core async methods
- [ ] Include related data when needed
- [ ] No business logic
- [ ] Proper error handling

#### **Tests Review**

```csharp
// ✅ GOOD Example
public class CustomerServiceTests
{
    [Fact]
    public async Task CreateCustomerAsync_ValidCustomer_ReturnsCreatedCustomer()
    {
        // Arrange
        var mockRepo = new Mock<ICustomerRepository>();
        var mockLogger = new Mock<ILogger<CustomerService>>();
        var service = new CustomerService(mockRepo.Object, mockLogger.Object);
        var customer = new Customer { Name = "Test" };

        mockRepo.Setup(r => r.AddAsync(It.IsAny<Customer>()))
            .ReturnsAsync(customer);

        // Act
        var result = await service.CreateCustomerAsync(customer);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);
        mockRepo.Verify(r => r.AddAsync(customer), Times.Once);
    }

    [Fact]
    public async Task CreateCustomerAsync_NullCustomer_ThrowsArgumentNullException()
    {
        // Arrange
        var mockRepo = new Mock<ICustomerRepository>();
        var mockLogger = new Mock<ILogger<CustomerService>>();
        var service = new CustomerService(mockRepo.Object, mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => service.CreateCustomerAsync(null));
    }
}
```

**Check**:
- [ ] AAA pattern (Arrange-Act-Assert)
- [ ] Test naming: MethodName_Scenario_ExpectedResult
- [ ] Mocking with Moq
- [ ] Tests success and failure scenarios
- [ ] Tests edge cases

### 3.2 Cross-Cutting Concerns

Review ALL files for:

#### **Async/Await Pattern**
```csharp
// ❌ BAD - Blocking
var customers = _service.GetCustomersAsync().Result;

// ✅ GOOD - Async all the way
var customers = await _service.GetCustomersAsync();
```

#### **Resource Management**
```csharp
// ❌ BAD - Not disposed
var stream = File.OpenRead("file.txt");

// ✅ GOOD - Using statement
using var stream = File.OpenRead("file.txt");
```

#### **Thread Safety**
```csharp
// ❌ BAD - Cross-thread violation
private async Task LoadData()
{
    var data = await _service.GetDataAsync();
    lblStatus.Text = "Loaded"; // UI update from background thread
}

// ✅ GOOD - Invoke for thread safety
private async Task LoadData()
{
    var data = await _service.GetDataAsync();
    if (InvokeRequired)
        Invoke(() => lblStatus.Text = "Loaded");
    else
        lblStatus.Text = "Loaded";
}
```

#### **Input Validation**
```csharp
// ❌ BAD - No validation
public async Task SaveCustomer(Customer customer)
{
    await _repository.AddAsync(customer);
}

// ✅ GOOD - Validated
public async Task SaveCustomer(Customer customer)
{
    ArgumentNullException.ThrowIfNull(customer);

    if (string.IsNullOrWhiteSpace(customer.Name))
        throw new ValidationException("Name required");

    await _repository.AddAsync(customer);
}
```

#### **Error Handling**
```csharp
// ❌ BAD - Swallowed exception
try
{
    await _service.SaveAsync(data);
}
catch { }

// ✅ GOOD - Logged and user-friendly message
try
{
    await _service.SaveAsync(data);
}
catch (ValidationException vex)
{
    _logger.LogWarning(vex, "Validation failed");
    MessageBox.Show(vex.Message, "Validation Error");
}
catch (Exception ex)
{
    _logger.LogError(ex, "Save failed");
    MessageBox.Show("Error saving data", "Error");
}
```

### 3.3 Use Review Checklist

For comprehensive review, use:
`.claude/workflows/code-review-checklist.md`

Check each category:
1. ✅ Compilation & Build
2. ✅ Testing
3. ✅ Architecture & Patterns
4. ✅ Resource Management
5. ✅ Async/Await
6. ✅ Input Validation
7. ✅ Error Handling
8. ✅ Thread Safety
9. ✅ Code Quality
10. ✅ Documentation
11. ✅ Security
12. ✅ Performance

---

## Stage 4: Testing & Verification (5 minutes)

### 4.1 Checkout and Build

```bash
# Checkout PR branch
git fetch origin
git checkout feature-branch

# Build
dotnet build

# Expected: No errors, no warnings
```

### 4.2 Run Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true

# Expected: All tests pass, coverage acceptable
```

### 4.3 Manual Testing (if UI changes)

For Forms/UI changes:
1. Run the application
2. Test the new/changed feature
3. Check for visual issues
4. Test validation
5. Test error scenarios

**Quick smoke test**:
- Does it compile?
- Does it run?
- Does basic functionality work?
- Are errors handled gracefully?

---

## Stage 5: Feedback & Decision (5 minutes)

### 5.1 Categorize Issues

Organize findings by severity:

#### **🔴 Critical (Must Fix Before Merge)**
- Code doesn't compile
- Tests failing
- Security vulnerabilities (hardcoded credentials, SQL injection)
- Data loss risk
- Business logic in Forms (architecture violation)
- Memory leaks (undisposed resources)
- Thread safety violations

#### **🟡 Major (Should Fix Before Merge)**
- Missing async/await on I/O
- Missing input validation
- Missing error handling
- Missing tests for new code
- Poor naming conventions
- Thread safety concerns

#### **🟢 Minor (Nice to Have)**
- Missing XML comments
- Code formatting issues
- Performance optimizations
- Refactoring opportunities

### 5.2 Write Review Comments

For each issue, use this format:

```markdown
### [Issue Title]

**Severity**: 🔴 Critical / 🟡 Major / 🟢 Minor

**Location**: `path/to/file.cs:123-130`

**Issue**:
[Clear description of the problem]

**Why This Matters**:
[Explain impact - security, performance, maintainability]

**Recommended Fix**:
```csharp
// Show corrected code
```

**Reference**: [Link to docs: docs/path/to/standard.md]
```

**Use Templates**: See `templates/review-comment-templates.md` for common issues.

### 5.3 Provide Positive Feedback

Balance criticism with recognition:
- ✅ Good test coverage
- ✅ Clean code structure
- ✅ Proper error handling
- ✅ Good naming conventions
- ✅ Well-documented

### 5.4 Make Final Decision

#### **✅ APPROVE** when:
- No critical issues
- 0-2 minor issues
- Good test coverage
- Follows all patterns correctly
- Code is production-ready

**Comment Template**:
```markdown
✅ **APPROVED**

Nice work! The code follows our standards well. I have a few minor suggestions below, but nothing blocking.

**Strengths**:
- Excellent test coverage
- Clean MVP implementation
- Good error handling

**Minor Suggestions**:
- [Optional improvements]

Great job! 🎉
```

#### **⚠️ REQUEST CHANGES** when:
- 1+ critical issues
- 3+ major issues
- Missing tests
- Violates core patterns
- Not production-ready

**Comment Template**:
```markdown
⚠️ **REQUESTING CHANGES**

Thanks for the PR! I found some issues that need to be addressed before we can merge:

**Critical Issues** (Must Fix):
1. [Issue 1]
2. [Issue 2]

**Major Issues** (Should Fix):
1. [Issue 1]
2. [Issue 2]

**Positive Feedback**:
- [What was done well]

Please address the critical issues and let me know when ready for re-review.
```

#### **💬 COMMENT** when:
- No blocking issues
- Educational feedback
- Suggestions for future improvement
- Questions for discussion

**Comment Template**:
```markdown
💬 **COMMENTED**

Overall looks good! I have some suggestions that might be helpful:

**Suggestions**:
1. [Improvement 1]
2. [Improvement 2]

**Questions**:
- [Question for discussion]

Feel free to address these now or in a follow-up PR.
```

---

## Review Templates by PR Type

### New Feature PR

Focus on:
1. ✅ Architecture (MVP pattern)
2. ✅ Test coverage (new features must have tests)
3. ✅ Error handling
4. ✅ Documentation
5. ✅ No breaking changes

### Bug Fix PR

Focus on:
1. ✅ Root cause addressed (not just symptom)
2. ✅ Test for the bug (prevent regression)
3. ✅ No introduction of new issues
4. ✅ Minimal changes (focused fix)

### Refactoring PR

Focus on:
1. ✅ No behavior change
2. ✅ All tests still pass
3. ✅ Code is more maintainable
4. ✅ Follows patterns better

### Performance PR

Focus on:
1. ✅ Benchmark data provided
2. ✅ No premature optimization
3. ✅ Tests still pass
4. ✅ Readability maintained

---

## Common Review Mistakes to Avoid

### ❌ DON'T:
1. **Be vague** - "This is bad" (no explanation or solution)
2. **Nitpick** - Focus on critical issues first
3. **Be harsh** - Remember there's a person behind the code
4. **Demand perfection** - Perfect is the enemy of good
5. **Block on personal preferences** - Follow standards, not opinions
6. **Review too slowly** - Aim for 24-hour turnaround
7. **Forget positive feedback** - Recognize good work

### ✅ DO:
1. **Be specific** - Reference files, lines, and provide examples
2. **Explain why** - Help the author understand the reasoning
3. **Provide solutions** - Show how to fix, not just what's wrong
4. **Be constructive** - Focus on improvement, not criticism
5. **Follow standards** - Use documented guidelines as authority
6. **Respond quickly** - Don't let PRs sit for days
7. **Balance feedback** - Acknowledge strengths and weaknesses

---

## Review Metrics

Track these for continuous improvement:

| Metric | Target | How to Track |
|--------|--------|--------------|
| Review Turnaround | < 24 hours | Time from PR submission to first review |
| Re-review Cycles | < 2 | Number of review rounds |
| Issues Per Review | Varies | Number of comments left |
| Approval Rate | ~70% | Percentage of PRs approved first time |
| Merge Time | < 48 hours | Time from PR to merge |

---

## Automated Review Tools

Consider using:
1. **Linters** - StyleCop, .editorconfig
2. **Static Analysis** - SonarQube, ReSharper
3. **Test Coverage** - Coverlet, dotCover
4. **AI Agents** - WinForms Reviewer agent (`.claude/agents/winforms-reviewer.md`)

**Slash Commands**:
- `/review-pr` - Full PR review
- `/review-code <file>` - Specific file review

---

## Review Escalation

When to escalate:

1. **Architecture Concerns** → Tech Lead / Architect
2. **Security Issues** → Security Team
3. **Performance Problems** → Performance Team
4. **Complex Business Logic** → Product Owner / BA
5. **Disputes** → Team Discussion / Tech Lead

---

## Checklist: Before Submitting Review

- [ ] Reviewed all changed files
- [ ] Tested locally (build + run)
- [ ] Categorized issues by severity
- [ ] Provided specific, actionable feedback
- [ ] Included positive feedback
- [ ] Referenced relevant documentation
- [ ] Made clear recommendation (Approve/Request Changes/Comment)
- [ ] Response time < 24 hours

---

## Example Review Output

See `templates/review-comment-templates.md` for examples of:
- Business logic in Form issue
- Missing async/await issue
- Missing validation issue
- Resource disposal issue
- Thread safety issue
- Security issue

---

**Last Updated**: 2025-11-08
**Version**: 1.0
