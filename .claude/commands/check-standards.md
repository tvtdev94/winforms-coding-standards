# Check Coding Standards Compliance

Quick check if code follows WinForms coding standards.

## Quick Checks

Run through this checklist:

### 1. Architecture
- [ ] No business logic in Forms?
- [ ] Services used for business logic?
- [ ] Dependency Injection used?

### 2. Naming
- [ ] Classes PascalCase?
- [ ] Methods PascalCase?
- [ ] Variables camelCase?
- [ ] Private fields _camelCase?
- [ ] Controls have prefixes (btn, txt, etc.)?

### 3. Async/Await
- [ ] I/O operations async?
- [ ] No .Result or .Wait()?
- [ ] Async suffix on methods?

### 4. Error Handling
- [ ] Try-catch on I/O operations?
- [ ] Exceptions logged?
- [ ] User-friendly messages?

### 5. Resources
- [ ] using statements for IDisposable?
- [ ] Events unsubscribed?
- [ ] No memory leaks?

### 6. Documentation
- [ ] XML comments on public APIs?
- [ ] Complex logic commented?

## Output Format

```markdown
# Standards Compliance Check

## ✅ Pass: X/Y checks

## ❌ Failed Checks:
1. [Description with file:line]
2. [Description with file:line]

## 💡 Recommendations:
- [Fix suggestion 1]
- [Fix suggestion 2]
```

## Fix Offer

After showing results, ask:
"Would you like me to fix these issues?"
