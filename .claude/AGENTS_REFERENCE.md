# AI Agents Reference

> Complete reference for all 10 AI agents available in WinForms Coding Standards.

---

## Overview

Agents are specialized AI assistants that handle specific tasks. They can be invoked by commands (like `/cook`) or directly through the Task tool.

| Category | Agents | Purpose |
|----------|--------|---------|
| **WinForms Specific** | winforms-reviewer, mvp-validator, test-generator, docs-manager | Domain expertise |
| **General Development** | planner, researcher, tester, debugger, code-reviewer, git-manager | Development workflow |

---

## 🎯 WinForms-Specific Agents

### `winforms-reviewer`
**WinForms code quality specialist**

**Expertise:**
- WinForms best practices
- Control usage patterns
- Designer compatibility
- UI/UX for desktop apps

**Checks:**
- Form lifecycle management
- Control naming conventions
- Resource disposal
- Threading issues
- Data binding patterns

**When to use:**
- After creating new forms
- Reviewing UI-heavy code
- Checking Designer compatibility

**Example invocation:**
```
Use winforms-reviewer agent to review CustomerForm.cs
```

---

### `mvp-validator`
**MVP pattern compliance validator**

**Expertise:**
- MVP (Model-View-Presenter) pattern
- Separation of concerns
- View interface design
- Presenter logic

**Validates:**
- No business logic in Forms
- Proper View interface usage
- Presenter handles UI logic
- Service layer for business logic

**When to use:**
- After refactoring to MVP
- Reviewing new forms
- Checking architecture compliance

**Output:**
```markdown
## MVP Validation Report

### View (CustomerForm)
✅ Implements ICustomerView
✅ No business logic
✅ Delegates to Presenter

### Presenter (CustomerPresenter)
✅ Handles UI logic
✅ Uses Service for business logic
⚠️ Line 45: Direct database access - move to Service
```

---

### `test-generator`
**Automated test generation for WinForms**

**Expertise:**
- xUnit/NUnit testing
- Moq for mocking
- WinForms testing patterns
- Presenter/Service testing

**Generates:**
- Service unit tests
- Presenter unit tests
- Integration tests
- Test data builders

**When to use:**
- After creating new services
- After creating new presenters
- When test coverage is low

**Output:** Complete test files with multiple test cases.

---

### `docs-manager`
**Documentation synchronization specialist**

**Expertise:**
- Markdown documentation
- Code-to-docs sync
- API documentation
- README updates

**Tasks:**
- Update docs after code changes
- Sync code examples in docs
- Update codebase-summary.md
- Maintain consistency

**When to use:**
- After significant feature additions
- After architectural changes
- Before releases

---

## 🔧 General Development Agents

### `planner`
**Implementation planning specialist**

**Expertise:**
- C# WinForms architecture
- MVP pattern design
- Task breakdown
- Risk assessment

**Process:**
1. Load project context
2. Research approaches (spawns researcher agents)
3. Explore codebase (uses Explore agent)
4. Create detailed plan

**Output:** Plan file in `plans/YYMMDD-feature-name-plan.md`

**Plan includes:**
- Overview and requirements
- Architecture diagram
- Implementation steps
- Files to create/modify
- Testing strategy
- TODO checklist

**When to use:**
- Before implementing complex features
- When unsure about approach
- For features spanning multiple layers

---

### `researcher`
**Technology research specialist**

**Expertise:**
- .NET technologies
- NuGet packages
- Best practices
- Security/performance

**Research process:**
1. Web search for official docs
2. Evaluate packages/libraries
3. Compare approaches
4. Compile recommendations

**Output:** Research report in `plans/research/YYMMDD-topic.md`

**When to use:**
- Evaluating new technologies
- Finding best libraries
- Understanding best practices
- Comparing approaches

**Example:**
```
Use researcher agent to find best authentication libraries for WinForms
```

---

### `tester`
**Test execution and analysis specialist**

**Expertise:**
- xUnit/NUnit
- Moq mocking
- Test patterns
- Coverage analysis

**Tasks:**
1. Generate tests for code
2. Run `dotnet test`
3. Analyze results
4. Report pass/fail with details
5. Suggest fixes for failures

**Test patterns:**
```csharp
// Service tests
MethodName_Scenario_ExpectedResult

// Examples:
GetByIdAsync_WhenExists_ReturnsCustomer
SaveAsync_WhenInvalid_ThrowsValidationException
```

**When to use:**
- After implementing features
- Before committing code
- When investigating failures

---

### `debugger`
**Issue diagnosis specialist**

**Expertise:**
- .NET debugging
- WinForms common issues
- Stack trace analysis
- Root cause analysis

**Common issues handled:**
- NullReferenceException
- Cross-thread operation
- ObjectDisposedException
- EF Core issues
- DI registration problems

**Process:**
1. Analyze error message
2. Examine stack trace
3. Identify root cause
4. Explain why it happens
5. Suggest fixes

**Output:**
```markdown
## Debug Report

### Error
NullReferenceException at CustomerPresenter.cs:45

### Root Cause
`_service` is null because `ICustomerService` not registered in DI

### Fix
Add to Program.cs:
services.AddScoped<ICustomerService, CustomerService>();
```

**When to use:**
- When errors occur
- Investigating failures
- Understanding exceptions

---

### `code-reviewer`
**Code quality specialist**

**Expertise:**
- C# best practices
- SOLID principles
- Security review
- Performance review

**Checks:**
| Area | What it checks |
|------|----------------|
| Architecture | MVP compliance, DI usage, Factory pattern |
| Code Quality | Naming, async/await, error handling |
| Security | SQL injection, input validation |
| Performance | N+1 queries, blocking calls |

**Severity levels:**
- 🔴 Critical - Must fix
- 🟠 Major - Should fix
- 🟡 Minor - Nice to fix
- 🔵 Info - Suggestion

**When to use:**
- After implementing features
- Before creating PRs
- Regular code reviews

---

### `git-manager`
**Version control specialist**

**Expertise:**
- Git workflow
- Conventional commits
- Branch management
- PR creation

**Tasks:**
- Stage and commit changes
- Create clean commit messages
- Create pull requests
- Check branch status

**Commit format:**
```
<type>(<scope>): <description>

Types: feat, fix, refactor, docs, test, chore
```

**Safety rules:**
- Never force push to main
- Never commit secrets
- Never skip hooks
- Always verify build before commit

**When to use:**
- After completing features
- Creating commits
- Creating PRs

---

## 🔄 Agent Orchestration

### How Agents Work Together

```
/cook command
     │
     ├─→ researcher (parallel) ─→ Research report
     │
     ├─→ Explore (parallel) ───→ Related files
     │
     └─→ planner ──────────────→ Implementation plan
              │
              ↓
         Implementation
              │
              ├─→ tester ──────→ Test results
              │
              ├─→ code-reviewer → Quality report
              │
              └─→ git-manager ─→ Commit
```

### Sequential Chaining
For tasks with dependencies:
```
planner → implementation → tester → code-reviewer
```

### Parallel Execution
For independent tasks:
```
researcher (approach A) ─┐
researcher (approach B) ─┼→ planner combines results
Explore (find files) ────┘
```

---

## 📊 Agent Selection Guide

| I want to... | Use Agent |
|--------------|-----------|
| Plan a feature | `planner` |
| Research technology | `researcher` |
| Run tests | `tester` |
| Debug an error | `debugger` |
| Review code quality | `code-reviewer` |
| Check MVP compliance | `mvp-validator` |
| Review WinForms code | `winforms-reviewer` |
| Generate tests | `test-generator` |
| Update docs | `docs-manager` |
| Commit changes | `git-manager` |

---

## 💡 Tips

### Direct Agent Invocation
```
Use the planner agent to create a plan for customer management feature
```

### Let Commands Handle It
Most commands automatically invoke the right agents:
- `/cook` → planner, researcher, tester, code-reviewer, git-manager
- `/plan` → planner, researcher
- `/test` → tester
- `/debug` → debugger

### Agent Reports
Agents save reports to:
- Plans: `plans/YYMMDD-feature-plan.md`
- Research: `plans/research/YYMMDD-topic.md`
- Agent-to-agent: `plans/reports/YYMMDD-from-to-task.md`

---

## 📖 See Also

- [Commands Reference](COMMANDS_REFERENCE.md) - All slash commands
- [Orchestration Protocol](workflows/orchestration-protocol.md) - How agents coordinate
- [Development Rules](workflows/development-rules.md) - C# WinForms rules
