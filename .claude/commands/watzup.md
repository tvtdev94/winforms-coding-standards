---
description: Review recent changes and summarize current work status
---

Review my current branch and recent commits. Provide a detailed summary of the work done.

## Workflow

1. **Check Git Status**:
   ```bash
   git status
   git log --oneline -10
   git diff --stat HEAD~5
   ```

2. **Analyze Changes**:
   - What files were modified/added/deleted?
   - What features were implemented?
   - What bugs were fixed?
   - Are there any uncommitted changes?

3. **Quality Check**:
   - Run `dotnet build` - any errors?
   - Run `dotnet test` - tests passing?
   - Any TODO items left?

## Output Format

```markdown
## Work Summary

### Branch: [branch-name]
**Status:** [ahead/behind main by X commits]

### Recent Changes
| Commit | Description |
|--------|-------------|
| abc123 | feat: added customer search |
| def456 | fix: resolved null reference |

### Files Changed
- **Added:** 5 files
- **Modified:** 8 files
- **Deleted:** 1 file

### Features Completed
- ✅ Customer search functionality
- ✅ Order validation

### Pending Items
- ⏳ Uncommitted changes in CustomerForm.cs
- ⏳ 2 failing tests

### Build Status
- Build: ✅ Success
- Tests: ⚠️ 15 passed, 2 failed

### Suggested Next Steps
1. Fix failing tests
2. Commit pending changes
3. Create PR to main
```

**IMPORTANT:** Do NOT make any changes - only report status.
