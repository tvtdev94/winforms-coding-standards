# Start Here - Quick Navigation Guide

> **New to this project?** This guide helps you find the right resource quickly.

---

## 🎯 "I want to..."

### Create Something

| I want to create... | Command | Guide |
|---------------------|---------|-------|
| New WinForms project | Run `scripts/init-project.ps1` | [USAGE_GUIDE.md](../USAGE_GUIDE.md) |
| New Form | `/create:form` | [form-template.cs](../templates/form-template.cs) |
| New Service | `/create:service` | [service-template.cs](../templates/service-template.cs) |
| New Repository | `/create:repository` | [repository-template.cs](../templates/repository-template.cs) |
| New Dialog | `/create:dialog` | [Templates](../templates/) |
| Custom UserControl | `/create:custom-control` | [Templates](../templates/) |

### Implement a Feature

| I want to... | Command | What it does |
|--------------|---------|--------------|
| **Full feature with workflow** | `/cook "feature name"` | Research → Plan → Implement → Test → Review |
| Create implementation plan | `/plan "feature name"` | Creates plan in `plans/` folder |
| Plan with 2 approaches | `/plan:two "feature name"` | Compare approaches before implementing |

### Fix or Improve

| I want to... | Command | Guide |
|--------------|---------|-------|
| Debug an error | `/debug "error description"` | [debugger agent](agents/debugger.md) |
| Fix threading issues | `/fix:threading` | [Threading Guide](guides/threading-guide.md) |
| Fix performance | `/fix:performance` | [Performance Guide](../docs/best-practices/) |
| Refactor to MVP | `/refactor:to-mvp` | [MVP Pattern](../docs/architecture/mvp-pattern.md) |

### Add Features

| I want to add... | Command |
|------------------|---------|
| Data validation | `/add:validation` |
| Error handling | `/add:error-handling` |
| Logging (Serilog/NLog) | `/add:logging` |
| Settings management | `/add:settings` |
| Data binding | `/add:data-binding` |

### Review & Test

| I want to... | Command | Output |
|--------------|---------|--------|
| Run tests | `/test` | Test results with pass/fail |
| Check work status | `/watzup` | Summary of changes, build status |
| Review code quality | Use `code-reviewer` agent | Quality report |

---

## 📚 "I need to learn about..."

### Architecture & Patterns

| Topic | Read This |
|-------|-----------|
| MVP Pattern | [docs/architecture/mvp-pattern.md](../docs/architecture/mvp-pattern.md) |
| Project Structure | [docs/architecture/project-structure.md](../docs/architecture/project-structure.md) |
| Dependency Injection | [docs/architecture/dependency-injection.md](../docs/architecture/dependency-injection.md) |
| Factory Pattern | [docs/architecture/factory-pattern.md](../docs/architecture/factory-pattern.md) |
| Unit of Work | [docs/data-access/unit-of-work.md](../docs/data-access/unit-of-work.md) |

### UI Framework

| Framework | Getting Started | Controls | Templates |
|-----------|-----------------|----------|-----------|
| **ReaLTaiizor** (default) | [Material Theme](../docs/realtaiizor/material-theme.md) | [Controls](../docs/realtaiizor/controls-reference.md) | [rt-material-form-template.cs](../templates/rt-material-form-template.cs) |
| DevExpress | [Quick Start](../docs/devexpress/dx-quick-start.md) | [Grid](../docs/devexpress/dx-grid-patterns.md) | [dx-form-template.cs](../templates/dx-form-template.cs) |
| Standard WinForms | [Best Practices](../docs/ui-ux/form-design.md) | [Controls](../docs/ui-ux/) | [form-template.cs](../templates/form-template.cs) |

### Best Practices

| Topic | Read This |
|-------|-----------|
| Coding Standards | [guides/coding-standards.md](guides/coding-standards.md) |
| Production UI Standards | [guides/production-ui-standards.md](guides/production-ui-standards.md) |
| Async/Await | [docs/best-practices/async-patterns.md](../docs/best-practices/async-patterns.md) |
| Error Handling | [docs/best-practices/error-handling.md](../docs/best-practices/error-handling.md) |
| Testing | [guides/testing-guide.md](guides/testing-guide.md) |

---

## 🛠️ Quick References

### Commands by Category

```
/cook          - Full feature workflow (research → implement → test)
/plan          - Create implementation plan only
/debug         - Debug issues
/test          - Run tests
/watzup        - Check work status

/create:*      - Create new components (form, service, repository...)
/add:*         - Add features (validation, logging, error-handling...)
/fix:*         - Fix issues (threading, performance, bug)
/refactor:*    - Refactor code (to-mvp)
/setup:*       - Setup features (di)
```

### Key Files

| File | Purpose |
|------|---------|
| `CLAUDE.md` | AI quick reference, rules, conventions |
| `.claude/INDEX.md` | Task → Resource mapping |
| `.claude/project-context.md` | Project-specific settings |
| `templates/` | Code templates to start from |
| `plans/templates/` | Planning templates |

### Agents Available

| Agent | Use For |
|-------|---------|
| `planner` | Creating implementation plans |
| `researcher` | Researching technologies |
| `tester` | Generating and running tests |
| `debugger` | Finding root causes |
| `code-reviewer` | Reviewing code quality |
| `git-manager` | Git operations |
| `winforms-reviewer` | WinForms-specific review |
| `mvp-validator` | Validating MVP pattern |
| `test-generator` | Generating WinForms tests |
| `docs-manager` | Updating documentation |

---

## 🚀 Getting Started Paths

### Path 1: New Project (5 minutes)
```
1. Run: scripts/init-project.ps1 -ProjectName "MyApp"
2. Open project in Visual Studio
3. Start with: /cook "implement main feature"
```

### Path 2: Learn the Standards (30 minutes)
```
1. Read: CLAUDE.md (quick rules)
2. Read: docs/architecture/mvp-pattern.md
3. Review: example-project/ (working example)
```

### Path 3: Add to Existing Project (10 minutes)
```
1. Add as Git submodule (see INTEGRATION_GUIDE.md)
2. Check: .claude/project-context.md is created
3. Use commands: /create:*, /add:*, etc.
```

---

## 📖 Full Documentation

For complete documentation index, see: [INDEX.md](INDEX.md)

For practical examples, see: [USAGE_GUIDE.md](../USAGE_GUIDE.md)

For troubleshooting, see: [TROUBLESHOOTING.md](../TROUBLESHOOTING.md)
