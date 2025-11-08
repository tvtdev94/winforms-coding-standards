# Review Pull Request

You are asked to perform a comprehensive code review of a Pull Request following WinForms coding standards. Follow these steps systematically:

## Prerequisites

Before starting, load essential context:
- Read `.claude/workflows/code-review-checklist.md` - Full review checklist
- Read `.claude/workflows/expert-behavior-guide.md` - Evaluation criteria
- Read `.claude/workflows/pr-review-workflow.md` - PR review process
- Read `templates/review-comment-templates.md` - Comment templates

## Steps

### 1. **Gather PR Information**

Ask the user for:
- Branch name or commit range (e.g., `feature/customer-form` or `main...feature/customer-form`)
- PR description/context (what feature/fix is being implemented)
- Files changed (or use git diff to discover)

If not provided, discover automatically:
```bash
# Get current branch name
git branch --show-current

# Get changed files
git diff --name-only main...HEAD

# Get commit messages
git log main..HEAD --oneline
```

### 2. **Analyze Changed Files**

For each changed file:
- Read the full file content
- Understand the purpose and context
- Identify the type of change (new feature, bug fix, refactor)

Categorize files by type:
- **Forms** (*.cs in /Forms) - Check MVP pattern, UI logic separation
- **Services** (*.cs in /Services) - Check business logic, async/await, validation
- **Repositories** (*.cs in /Repositories) - Check EF Core usage, async patterns
- **Tests** (*.cs in /Tests) - Check coverage, naming, AAA pattern
- **Configuration** - Check appsettings, .editorconfig, etc.

### 3. **Perform Architecture Review**

Check overall architecture:
- [ ] **Separation of Concerns** - UI vs Business vs Data properly separated
- [ ] **MVP/MVVM Pattern** - Correctly implemented
- [ ] **Dependency Injection** - Services injected via constructor
- [ ] **No business logic in Forms** - Forms only handle UI
- [ ] **Repository pattern** - Data access abstracted
- [ ] **Service layer** - Business logic centralized

### 4. **Review Each File Against Checklist**

For each file, check:

#### **4.1. Code Quality**
- [ ] Naming conventions followed (PascalCase, camelCase)
- [ ] No magic numbers/strings (use constants)
- [ ] XML documentation on public APIs
- [ ] No commented-out code
- [ ] Clear, readable code structure

#### **4.2. Architecture & Patterns**
- [ ] No business logic in Forms
- [ ] MVP/MVVM pattern followed
- [ ] Services use dependency injection
- [ ] Repositories use EF Core correctly
- [ ] No direct database access from Forms

#### **4.3. Resource Management**
- [ ] All IDisposable resources disposed
- [ ] Form.Dispose() implemented correctly
- [ ] No memory leaks (event unsubscription)
- [ ] Database connections closed

#### **4.4. Async/Await**
- [ ] Async/await used for all I/O operations
- [ ] Methods named with Async suffix
- [ ] No blocking on async (.Result or .Wait())
- [ ] CancellationToken support (for long operations)

#### **4.5. Input Validation**
- [ ] User input validated before processing
- [ ] Null checks (ArgumentNullException)
- [ ] Business rule validation in Service layer
- [ ] ErrorProvider used (WinForms)

#### **4.6. Error Handling**
- [ ] Try-catch blocks around risky operations
- [ ] Exceptions logged with ILogger
- [ ] User-friendly error messages
- [ ] Exceptions not swallowed
- [ ] Specific exceptions caught (not generic Exception)

#### **4.7. Thread Safety**
- [ ] UI updates on UI thread (Invoke/BeginInvoke)
- [ ] No UI controls accessed from background threads
- [ ] Async methods used instead of BackgroundWorker

#### **4.8. Security**
- [ ] No hardcoded credentials
- [ ] SQL injection prevented (parameterized queries)
- [ ] User input sanitized
- [ ] Sensitive data not logged
- [ ] No secrets in source control

#### **4.9. Performance**
- [ ] No N+1 queries (use .Include())
- [ ] Large lists virtualized
- [ ] Images optimized
- [ ] No unnecessary database calls

#### **4.10. Testing**
- [ ] Unit tests for new Services
- [ ] Integration tests for Repositories
- [ ] Tests follow AAA pattern
- [ ] Test coverage adequate (80%+ for Services)
- [ ] Edge cases tested

### 5. **Run Automated Checks** (if possible)

If the code is available locally:
```bash
# Build check
dotnet build

# Run tests
dotnet test

# Check test coverage (if configured)
dotnet test /p:CollectCoverage=true
```

### 6. **Generate Review Report**

Create a structured review report with:

#### **Summary**
- Overall assessment (Approve / Request Changes / Needs Discussion)
- Number of critical/major/minor issues found
- Key strengths of the PR

#### **Critical Issues** 🔴 (Must Fix)
List issues that MUST be fixed:
- Code doesn't compile
- Tests failing
- Security vulnerabilities
- Data loss risk
- Business logic in Forms
- Memory leaks

For each issue:
- **Location**: File path and line number
- **Issue**: Clear description
- **Why**: Explain the problem
- **Fix**: Specific solution
- **Template reference**: Use appropriate comment template

#### **Major Issues** 🟡 (Should Fix)
List issues that should be fixed:
- Missing async/await
- Missing input validation
- Missing error handling
- Thread safety issues
- Poor naming conventions

#### **Minor Issues** 🟢 (Nice to Have)
List issues that can be addressed later:
- Missing XML documentation
- Code formatting
- Performance optimizations
- Refactoring opportunities

#### **Positive Feedback** ✅
Highlight what was done well:
- Good pattern implementation
- Clean code structure
- Good test coverage
- Excellent error handling

#### **Recommendations**
- Architectural suggestions
- Alternative approaches
- Performance improvements
- Learning resources

### 7. **Use Review Comment Templates**

For common issues, use templates from `templates/review-comment-templates.md`:
- Business logic in Form
- Missing async/await
- Missing input validation
- Resource disposal issues
- Thread safety violations
- Security issues

### 8. **Provide Actionable Feedback**

For each issue:
- ✅ **DO**: Be specific with file:line references
- ✅ **DO**: Explain WHY it's an issue
- ✅ **DO**: Provide concrete solution/example
- ✅ **DO**: Reference documentation
- ❌ **DON'T**: Be vague ("this is bad")
- ❌ **DON'T**: Just point out problems without solutions
- ❌ **DON'T**: Be condescending or harsh

### 9. **Final Recommendation**

Provide clear recommendation:

**✅ APPROVE** - When:
- No critical issues
- 0-2 minor issues
- Good test coverage
- Follows all patterns correctly

**⚠️ REQUEST CHANGES** - When:
- 1+ critical issues
- 3+ major issues
- Missing tests
- Violates core patterns

**💬 COMMENT** - When:
- No blocking issues
- Some suggestions for improvement
- Educational feedback

## Output Format

```markdown
# Pull Request Review: [PR Title/Branch]

## Summary
- **Status**: ✅ Approve / ⚠️ Request Changes / 💬 Comment
- **Files Reviewed**: X files
- **Issues Found**: X critical, X major, X minor
- **Overall**: [1-2 sentence summary]

---

## Critical Issues 🔴 (Must Fix Before Merge)

### 1. [Issue Title]
- **File**: `path/to/file.cs:123`
- **Issue**: [Description]
- **Why**: [Explanation]
- **Fix**:
  ```csharp
  // Example code
  ```
- **Reference**: [docs/path/to/doc.md]

---

## Major Issues 🟡 (Should Fix)

[Same format as critical]

---

## Minor Issues 🟢 (Nice to Have)

[Same format]

---

## Positive Feedback ✅

- ✅ [What was done well]
- ✅ [Another good thing]

---

## Recommendations

1. [Suggestion 1]
2. [Suggestion 2]

---

## Final Recommendation

**[APPROVE/REQUEST CHANGES/COMMENT]**

[Explanation of decision]
```

## Example Usage

User: "Review my customer form implementation on branch feature/customer-form"