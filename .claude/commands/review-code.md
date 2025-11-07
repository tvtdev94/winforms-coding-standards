# Review Code Against Standards

You are asked to review code for compliance with coding standards. Follow this systematic review:

## Review Checklist

### 1. Architecture (docs/architecture/)
- [ ] Business logic separated from UI (in Services, not Forms)
- [ ] Follows MVP or MVVM pattern (if applicable)
- [ ] Dependencies injected via constructor
- [ ] Repository pattern used for data access
- [ ] Layering: Forms → Services → Repositories

### 2. Naming Conventions (docs/conventions/naming-conventions.md)
- [ ] Classes use PascalCase
- [ ] Methods use PascalCase
- [ ] Variables use camelCase
- [ ] Private fields use _camelCase
- [ ] Constants use UPPER_SNAKE_CASE
- [ ] Controls have prefixes (btn, txt, dgv, etc.)
- [ ] No default designer names (button1, textBox1)
- [ ] Interfaces start with I (ICustomerService)

### 3. Code Style (docs/conventions/code-style.md)
- [ ] Braces on new line (Allman style)
- [ ] 4-space indentation
- [ ] Methods < 30 lines
- [ ] Classes < 500 lines
- [ ] var used when type is obvious
- [ ] No deep nesting (max 3 levels)

### 4. Async/Await (docs/best-practices/async-await.md)
- [ ] All I/O operations use async/await
- [ ] No .Result or .Wait() calls (deadlock risk)
- [ ] Async methods have Async suffix
- [ ] Event handlers can be async void
- [ ] Other methods return Task, not async void
- [ ] CancellationToken used for long operations

### 5. Error Handling (docs/best-practices/error-handling.md)
- [ ] Try-catch blocks present for I/O operations
- [ ] Specific exceptions caught first
- [ ] Exceptions logged with context
- [ ] User-friendly error messages
- [ ] Finally blocks clean up resources
- [ ] No silent exception swallowing

### 6. Resource Management
- [ ] IDisposable resources disposed
- [ ] using statements used
- [ ] Event handlers unsubscribed
- [ ] No memory leaks

### 7. Testing
- [ ] Unit tests exist for Services
- [ ] Unit tests exist for Presenters (if MVP)
- [ ] Tests follow AAA pattern (Arrange-Act-Assert)
- [ ] Tests use mocks for dependencies

### 8. Documentation
- [ ] XML comments on public APIs
- [ ] Complex logic commented
- [ ] No commented-out code
- [ ] TODO comments for future work

## Review Process

1. **Identify files to review**
   - Ask user which files to review, or review recently changed files

2. **Read each file**
   - Check against all checklist items above

3. **Categorize issues**
   - **Critical**: Deadlock risk, memory leaks, security issues
   - **Major**: Violates core patterns (business logic in UI)
   - **Minor**: Naming conventions, style issues

4. **Provide feedback**
   - List issues by severity
   - Reference specific docs for each issue
   - Suggest fixes with code examples
   - Praise good practices found

5. **Offer to fix**
   - Ask if user wants you to fix issues
   - Fix critical and major issues first

## Output Format

```markdown
# Code Review Results

## ✅ Good Practices Found
- [List positive findings]

## 🔴 Critical Issues
1. [Issue with file:line reference]
   - **Problem**: [Description]
   - **Reference**: [Link to docs]
   - **Fix**: [Suggested code]

## 🟡 Major Issues
[Same format as critical]

## 🟢 Minor Issues
[Same format as critical]

## 📊 Summary
- Total files reviewed: X
- Critical issues: X
- Major issues: X
- Minor issues: X

## 🎯 Next Steps
[Prioritized list of recommended fixes]
```

## References

- [Architecture](../docs/architecture/)
- [Conventions](../docs/conventions/)
- [Best Practices](../docs/best-practices/)
- [Testing](../docs/testing/)
