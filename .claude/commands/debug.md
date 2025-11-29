---
description: Debug issues and find root cause
argument-hint: [error description or issue]
---

**Reported Issues:**
$ARGUMENTS

Use the `debugger` subagent to:
1. Analyze the error/issue
2. Find the root cause
3. Explain the problem clearly
4. Suggest fix approaches

## Common WinForms Issues to Check

- **NullReferenceException**: Missing DI registration? Null check needed?
- **Cross-thread operation**: UI accessed from background thread?
- **ObjectDisposedException**: Form/DbContext disposed too early?
- **InvalidOperationException**: DbContext concurrent access?
- **Build errors**: Missing reference? Wrong namespace?

## Output

Provide:
1. **Root Cause**: What's causing the issue
2. **Explanation**: Why it happens
3. **Fix Options**: 1-2 approaches to fix
4. **Code Sample**: Example fix code

**IMPORTANT:** Do NOT implement the fix automatically.
**IMPORTANT:** Ask user for confirmation before making changes.
