# Orchestration Protocol

Guidelines for coordinating multiple subagents to complete complex tasks efficiently.

## Sequential Chaining

Chain subagents when tasks have dependencies:

```
Research → Plan → Implement → Test → Review → Document
```

**When to use:**
- Feature development (full workflow)
- Bug fixes requiring investigation
- Refactoring with testing

**Example Flow:**
```
1. researcher → Investigate approaches
2. planner → Create implementation plan
3. [main agent] → Implement code
4. tester → Generate and run tests
5. code-reviewer → Review changes
6. docs-manager → Update documentation
7. git-manager → Commit changes
```

## Parallel Execution

Spawn multiple subagents for independent tasks:

**When to use:**
- Multiple research topics
- Independent file searches
- Non-conflicting implementations

**Example:**
```
┌─────────────────────────────────────┐
│           /cook command             │
└──────────────┬──────────────────────┘
               │
    ┌──────────┴──────────┐
    ↓                     ↓
researcher            Explore
(best practices)    (find files)
    │                     │
    └──────────┬──────────┘
               ↓
           planner
               │
               ↓
        Implementation
               │
    ┌──────────┴──────────┐
    ↓                     ↓
 tester            code-reviewer
    │                     │
    └──────────┬──────────┘
               ↓
          docs-manager
               │
               ↓
          git-manager
```

## Context Management

### Passing Context Between Agents

Agents communicate through:

1. **File System Reports** (`./plans/reports/`):
   ```
   YYMMDD-from-agent-to-agent-task-report.md
   ```

2. **Plan Files** (`./plans/`):
   ```
   YYMMDD-feature-name-plan.md
   ```

3. **Research Files** (`./plans/research/`):
   ```
   YYMMDD-research-topic.md
   ```

### Context Loading Priority

Every agent should load context in this order:

1. `.claude/project-context.md` - Project settings
2. `.claude/INDEX.md` - Find relevant resources
3. Task-specific files from previous agents

## Agent Selection Guide

| Task | Primary Agent | Supporting Agents |
|------|---------------|-------------------|
| Research technology | `researcher` | - |
| Find files in codebase | `Explore` | - |
| Create implementation plan | `planner` | `researcher`, `Explore` |
| Generate tests | `tester` | - |
| Review code quality | `code-reviewer` | `winforms-reviewer` |
| Debug issues | `debugger` | - |
| Update documentation | `docs-manager` | - |
| Git operations | `git-manager` | - |
| Validate MVP pattern | `mvp-validator` | - |
| WinForms-specific review | `winforms-reviewer` | - |

## Error Handling

### When Agent Fails

1. Capture the error
2. Attempt to fix (if simple)
3. If complex, spawn `debugger` agent
4. Report findings and fix
5. Resume original workflow

### When Tests Fail

```
tester reports failure
        ↓
debugger investigates
        ↓
[main agent] fixes
        ↓
tester re-runs
        ↓
(repeat until pass)
```

## Workflow Templates

### Feature Development (Full)
```
/cook "implement customer search"
  ├─ Load context (project-context.md, INDEX.md)
  ├─ [parallel] researcher + Explore
  ├─ planner → creates plan
  ├─ Implementation (using templates)
  ├─ dotnet build (verify)
  ├─ tester → generate + run tests
  ├─ [if fail] debugger → fix → tester (repeat)
  ├─ code-reviewer → review
  ├─ [if issues] fix → tester → code-reviewer (repeat)
  ├─ docs-manager → update docs
  └─ git-manager → commit (with user approval)
```

### Bug Fix
```
/cook "fix null reference in OrderForm"
  ├─ debugger → investigate root cause
  ├─ [main agent] → implement fix
  ├─ dotnet build (verify)
  ├─ tester → run affected tests
  ├─ code-reviewer → quick review
  └─ git-manager → commit
```

### Quick Feature (No Research Needed)
```
/cook "add email field to CustomerForm"
  ├─ Load context
  ├─ Explore → find relevant files
  ├─ Implementation
  ├─ dotnet build
  ├─ tester → quick test
  └─ git-manager → commit
```

## Performance Guidelines

1. **Minimize Agent Spawns**: Only spawn when needed
2. **Parallel When Possible**: Independent tasks run together
3. **Early Failure**: Stop if critical issues found
4. **Incremental Progress**: Build/test after each significant change
5. **Focused Scope**: Each agent has one clear responsibility

## Quality Gates

Each phase must pass before proceeding:

| Phase | Gate |
|-------|------|
| Research | Findings documented |
| Plan | Plan file created with TODOs |
| Implementation | `dotnet build` succeeds |
| Testing | All tests pass |
| Review | No critical issues |
| Documentation | Relevant docs updated |
| Git | Clean commit created |
