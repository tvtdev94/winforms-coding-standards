# Review Code Files

You are asked to review specific code file(s) following WinForms coding standards. This is a focused review on specific files rather than a full PR review.

## Prerequisites

Before starting, load essential context:
- Read `.claude/workflows/code-review-checklist.md` - Full review checklist
- Read `.claude/workflows/expert-behavior-guide.md` - Evaluation criteria
- Read `templates/review-comment-templates.md` - Comment templates

## Steps

### 1. **Identify Files to Review**

Ask user for:
- File path(s) to review
- Context: What was changed? Why?
- Specific concerns (optional)

If not provided by user, ask: "Which file(s) would you like me to review?"

### 2. **Read and Understand Files**

For each file:
- Read the complete file content
- Identify file type (Form, Service, Repository, Test, etc.)
- Understand the purpose and responsibilities
- Note dependencies (what services/repositories it uses)

### 3. **Categorize File Type and Apply Specific Checks**

#### **If Form (*.cs in /Forms/)**

Check:
- [ ] Implements IView interface (MVP pattern)
- [ ] No business logic (only UI handling)
- [ ] Constructor injection of Presenter
- [ ] Proper event wiring
- [ ] Dispose() implemented correctly
- [ ] Control naming conventions (btnSave, txtName, etc.)
- [ ] Thread-safe UI updates (Invoke/BeginInvoke)
- [ ] Async event handlers for I/O operations

#### **If Service (*.cs in /Services/)**

Check:
- [ ] Interface defined (ICustomerService)
- [ ] Constructor injection of dependencies
- [ ] Input validation (ArgumentNullException, ArgumentException)
- [ ] All I/O methods are async (with Async suffix)
- [ ] Proper error handling and logging
- [ ] No direct UI dependencies
- [ ] No direct data access (uses Repositories)
- [ ] XML documentation on public methods
- [ ] Unit testable (interfaces, DI)

#### **If Repository (*.cs in /Repositories/)**

Check:
- [ ] Interface defined (ICustomerRepository)
- [ ] Generic repository pattern (if applicable)
- [ ] EF Core async methods (ToListAsync, FirstOrDefaultAsync)
- [ ] Proper DbContext disposal
- [ ] No business logic (pure data access)
- [ ] Include related data when needed (.Include())
- [ ] Proper error handling
- [ ] No N+1 query problems

#### **If Presenter (*.cs in /Presenters/)**

Check:
- [ ] References IView interface (not concrete Form)
- [ ] Constructor injection of Services
- [ ] AttachView and DetachView methods
- [ ] All business logic here (not in Form)
- [ ] Validation logic
- [ ] Proper error handling
- [ ] Unit testable with mocked IView

#### **If Test (*.cs in /Tests/)**

Check:
- [ ] Test class naming: ClassNameTests
- [ ] Test method naming: MethodName_Scenario_ExpectedResult
- [ ] AAA pattern (Arrange, Act, Assert)
- [ ] Mocking with Moq (for dependencies)
- [ ] Tests both success and failure scenarios
- [ ] Tests edge cases
- [ ] No test interdependencies
- [ ] Async tests use async/await

#### **If Model/Entity (*.cs in /Models/)**

Check:
- [ ] Data annotations for validation
- [ ] Required properties marked
- [ ] Proper property types
- [ ] Navigation properties (if entity)
- [ ] No business logic (pure data)
- [ ] XML documentation

### 4. **Review Against General Checklist**

For ALL files, check:

#### **Code Quality**
- [ ] Follows naming conventions (docs/conventions/naming-conventions.md)
- [ ] No magic numbers/strings (use constants)
- [ ] XML documentation on public APIs
- [ ] No commented-out code
- [ ] Clear, readable code structure
- [ ] Proper indentation and formatting

#### **Resource Management**
- [ ] IDisposable resources disposed (using statement)
- [ ] Event subscriptions unsubscribed
- [ ] No memory leaks

#### **Async/Await**
- [ ] Async methods for I/O operations
- [ ] Methods named with Async suffix
- [ ] No blocking (.Result, .Wait())
- [ ] CancellationToken support (for long operations)

#### **Error Handling**
- [ ] Try-catch around risky operations
- [ ] Exceptions logged
- [ ] User-friendly error messages
- [ ] Specific exceptions caught

#### **Security**
- [ ] No hardcoded credentials
- [ ] No SQL injection vulnerability
- [ ] Input sanitized
- [ ] No sensitive data in logs

#### **Performance**
- [ ] No unnecessary database calls
- [ ] Proper use of async
- [ ] No N+1 queries
- [ ] Efficient LINQ queries

### 5. **Generate Review Report**

Create structured review with:

#### **File Summary**
- File path
- File type (Form/Service/Repository/etc.)
- Purpose
- Lines of code
- Overall quality score (1-5 stars)

#### **Issues Found**

Categorize by severity:

**🔴 Critical Issues** (Must Fix):
- Security vulnerabilities
- Memory leaks
- Business logic in Forms
- Blocking async operations
- Data loss risks

**🟡 Major Issues** (Should Fix):
- Missing validation
- Missing error handling
- Incorrect pattern implementation
- Missing tests
- Thread safety issues

**🟢 Minor Issues** (Nice to Have):
- Missing XML comments
- Naming convention violations
- Code formatting
- Refactoring opportunities

#### **Positive Feedback**
Highlight what was done well.

#### **Recommendations**
Specific suggestions for improvement.

### 6. **Use Review Comment Templates**

For common issues, use templates from `templates/review-comment-templates.md`.

### 7. **Provide Code Examples**

For each issue:
- Show the problematic code (with line numbers)
- Explain WHY it's an issue
- Provide corrected code example
- Reference relevant documentation

Format:
```markdown
### Issue: [Title]

**Location**: `path/to/file.cs:123-130`

**Current Code**:
```csharp
// Problematic code
```

**Issue**: [Explanation]

**Why This Matters**: [Impact/consequences]

**Recommended Fix**:
```csharp
// Corrected code
```

**Reference**: [docs/path/to/doc.md]
```

### 8. **Final Assessment**

For each file:
- ⭐⭐⭐⭐⭐ Excellent - No issues, best practices followed
- ⭐⭐⭐⭐ Good - Minor issues only
- ⭐⭐⭐ Acceptable - Some major issues to address
- ⭐⭐ Needs Work - Multiple major issues
- ⭐ Poor - Critical issues, requires significant rework

## Output Format

```markdown
# Code Review: [File Name]

## File Information
- **Path**: `path/to/file.cs`
- **Type**: Form / Service / Repository / Test / Other
- **Purpose**: [Brief description]
- **Lines**: XXX
- **Rating**: ⭐⭐⭐⭐⭐

---

## Critical Issues 🔴 (Must Fix)

### 1. [Issue Title]
- **Lines**: 123-130
- **Issue**: [Description]
- **Why**: [Explanation]
- **Fix**:
  ```csharp
  // Example
  ```
- **Reference**: [Link to docs]

---

## Major Issues 🟡 (Should Fix)

[Same format]

---

## Minor Issues 🟢 (Nice to Have)

[Same format]

---

## Positive Feedback ✅

- ✅ [Good practice used]
- ✅ [Another strength]

---

## Recommendations

1. [Specific suggestion]
2. [Another suggestion]

---

## Summary

[Overall assessment and next steps]
```

## Examples

**Example 1**: Review a single file
```
User: "Review CustomerForm.cs"
Assistant: [Performs review following steps above]
```

**Example 2**: Review multiple files
```
User: "Review CustomerService.cs and CustomerRepository.cs"
Assistant: [Reviews each file separately, provides combined summary]
```

**Example 3**: Focused review
```
User: "Review the error handling in OrderService.cs"
Assistant: [Focuses specifically on error handling patterns]
```

## Tips for Effective Review

1. **Be specific** - Always provide line numbers and concrete examples
2. **Explain why** - Don't just point out issues, explain the impact
3. **Provide solutions** - Show corrected code, not just problems
4. **Be constructive** - Balance criticism with positive feedback
5. **Reference docs** - Link to relevant standards documentation
6. **Consider context** - Understand the purpose before criticizing
7. **Prioritize** - Focus on critical issues first
8. **Be consistent** - Apply standards uniformly

## After Review

Optionally offer to:
- Fix the issues automatically
- Create a refactoring plan
- Write missing tests
- Generate documentation
