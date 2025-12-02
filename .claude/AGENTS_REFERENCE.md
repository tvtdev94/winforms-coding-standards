# AI Agents Reference

> Reference for the 7 AI agents available in WinForms Coding Standards.
> **Updated**: 2025-12-02

---

## Overview

Agents are specialized AI assistants that handle specific tasks. They can be invoked by commands (like `/cook`) or directly through the Task tool.

| Category | Agents | Purpose |
|----------|--------|---------|
| **WinForms Specific** | reviewer, tester, docs-manager | Domain expertise |
| **General Development** | planner, researcher, debugger, git-manager | Development workflow |

---

## Agent Quick Reference

| Agent | Use For |
|-------|---------|
| `reviewer` | Code review, MVP validation, WinForms best practices |
| `tester` | Generate tests, run tests, coverage analysis |
| `planner` | Create implementation plans |
| `debugger` | Debug issues, root cause analysis |
| `researcher` | Research technologies, packages |
| `git-manager` | Commits, PRs, branch management |
| `docs-manager` | Documentation sync |

---

## Detailed Agent Descriptions

### `reviewer`
**Code quality & architecture specialist**

Combines: Code review + WinForms best practices + MVP validation

**Checks:**
- MVP/MVVM pattern compliance
- WinForms best practices
- Security (SQL injection, input validation)
- Performance (N+1 queries, blocking calls)
- Resource management (Dispose, memory leaks)
- Thread safety (Invoke/BeginInvoke)
- Designer compatibility

**Severity levels:**
- Critical - Must fix (security, data loss)
- Major - Should fix (pattern violations)
- Minor - Nice to fix (style, suggestions)

**When to use:**
- After implementing features
- Before creating PRs
- Reviewing code quality

---

### `tester`
**Test generation and execution specialist**

Combines: Test generation + test running + coverage

**Capabilities:**
- Generate unit tests for Services, Presenters
- Generate integration tests for Repositories
- Run `dotnet test`
- Analyze results and suggest fixes
- Coverage analysis

**Test patterns:**
```csharp
// Naming: MethodName_Scenario_ExpectedResult
GetByIdAsync_WhenExists_ReturnsCustomer
SaveAsync_WhenInvalid_ThrowsValidationException
```

**When to use:**
- After implementing features
- Before committing code
- When test coverage is low

---

### `planner`
**Implementation planning specialist**

**Process:**
1. Load project context
2. Research approaches (spawns researcher)
3. Explore codebase
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

---

### `debugger`
**Issue diagnosis specialist**

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

**When to use:**
- When errors occur
- Investigating failures

---

### `researcher`
**Technology research specialist**

**Research process:**
1. Web search for official docs
2. Evaluate packages/libraries
3. Compare approaches
4. Compile recommendations

**Output:** Research report in `plans/research/YYMMDD-topic.md`

**When to use:**
- Evaluating new technologies
- Finding best libraries
- Comparing approaches

---

### `git-manager`
**Version control specialist**

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
- Creating commits/PRs

---

### `docs-manager`
**Documentation synchronization specialist**

**Tasks:**
- Update docs after code changes
- Sync code examples in docs
- Update codebase-summary.md
- Maintain consistency

**When to use:**
- After significant feature additions
- After architectural changes

---

## Agent Orchestration

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
              ├─→ reviewer ────→ Quality report
              │
              └─→ git-manager ─→ Commit
```

### Sequential vs Parallel

**Sequential** (dependencies):
```
planner → implementation → tester → reviewer
```

**Parallel** (independent):
```
researcher + Explore → planner combines results
```

---

## Agent Selection Guide

| I want to... | Use Agent |
|--------------|-----------|
| Plan a feature | `planner` |
| Research technology | `researcher` |
| Run tests | `tester` |
| Debug an error | `debugger` |
| Review code quality | `reviewer` |
| Update docs | `docs-manager` |
| Commit changes | `git-manager` |

---

## Tips

### Direct Agent Invocation
```
Use the planner agent to create a plan for customer management feature
```

### Let Commands Handle It
Most commands automatically invoke the right agents:
- `/cook` → planner, researcher, tester, reviewer, git-manager
- `/plan` → planner, researcher
- `/test` → tester
- `/debug` → debugger

### Agent Reports
Agents save reports to:
- Plans: `plans/YYMMDD-feature-plan.md`
- Research: `plans/research/YYMMDD-topic.md`
- Agent-to-agent: `plans/reports/YYMMDD-from-to-task.md`

---

## See Also

- [Commands Reference](COMMANDS_REFERENCE.md) - All slash commands
- [Development Workflow](workflows/development-workflow.md) - How agents coordinate
- [Review Workflow](workflows/review-workflow.md) - Review process
