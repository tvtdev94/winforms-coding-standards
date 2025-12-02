---
description: Fix a bug or issue with streamlined workflow
argument-hint: [bug description or error message]
---

**Issue to Fix:**
$ARGUMENTS

---

## Workflow

### Step 1: Analyze Issue

Use **`debugger` subagent** to:
1. Analyze the error/issue
2. Find the root cause
3. Identify affected files

### Step 2: Plan Fix

Based on debugger output:
- Identify minimal changes needed
- List files to modify
- Check for side effects

### Step 3: Implement Fix

1. **Read affected files first**
2. **Apply minimal fix** - YAGNI principle
3. **Run `dotnet build`** to verify compile
4. **Fix any build errors**

### Step 4: Test

1. **Run `dotnet test`**
2. If tests fail, iterate on fix
3. Ensure no regressions

### Step 5: Review (if significant)

For non-trivial fixes, use **`reviewer` subagent** to:
- Verify fix doesn't introduce new issues
- Check error handling

### Step 6: Report

**Output:**
- Root cause explanation
- Fix applied (file:line)
- Tests status

---

## Critical Rules

| DO | DON'T |
|----|-------|
| Minimal fix | Over-engineer solution |
| Read files before editing | Generate code from scratch |
| Run build after changes | Skip testing |
| Fix root cause | Apply band-aid fix |

---

## Subagents Used

| Agent | When |
|-------|------|
| `debugger` | Step 1 - Find root cause |
| `reviewer` | Step 5 - For significant fixes |
| `tester` | Step 4 - If new tests needed |
