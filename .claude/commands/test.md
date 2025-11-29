---
description: Run tests and analyze results
argument-hint: [optional: specific test class or method]
---

Use the `tester` subagent to run tests and analyze results.

## Workflow

1. **Run Tests**:
   ```bash
   # Run all tests
   dotnet test

   # Or specific tests if argument provided
   dotnet test --filter "FullyQualifiedName~$ARGUMENTS"
   ```

2. **Analyze Results**:
   - Count passed/failed/skipped
   - Identify failure reasons
   - Check for common issues

3. **Report**:
   ```
   ## Test Results

   **Summary:** X passed, Y failed, Z skipped

   ### Passed ✅
   - Test1
   - Test2

   ### Failed ❌
   - Test3: [reason]
     - Fix suggestion: [suggestion]
   ```

## If Tests Fail

- Use `debugger` subagent to investigate root cause
- Provide specific fix suggestions
- Show which code needs to change

**IMPORTANT:** Do NOT auto-fix failing tests.
**IMPORTANT:** Report results and wait for user decision.
