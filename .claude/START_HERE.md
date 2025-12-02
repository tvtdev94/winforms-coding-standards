# Start Here - Quick Navigation Guide

> **New to this project?** This guide helps you find the right resource quickly.

---

## 🎯 "I want to..."

### Create Something

| I want to create... | How | Template |
|---------------------|-----|----------|
| New WinForms project | Run `scripts/init-project.ps1` | - |
| New Form | `/cook "create CustomerForm"` | [form-template.cs](../templates/form-template.cs) |
| New Service | `/cook "create OrderService"` | [service-template.cs](../templates/service-template.cs) |
| New Repository | `/cook "create ProductRepository"` | [repository-template.cs](../templates/repository-template.cs) |

### Implement a Feature

| I want to... | Command | What it does |
|--------------|---------|--------------|
| **Full feature with workflow** | `/cook "feature name"` | Research → Plan → Implement → Test → Review |
| Create implementation plan | `/plan "feature name"` | Creates plan in `plans/` folder |

### Fix or Improve

| I want to... | Command |
|--------------|---------|
| Debug an error | `/debug "error description"` |
| Fix any issue | `/cook "fix threading issue in CustomerForm"` |

### Add Features

| I want to add... | Command |
|------------------|---------|
| Validation | `/cook "add validation to CustomerForm"` |
| Error handling | `/cook "add error handling to OrderService"` |
| Logging | `/cook "add Serilog logging"` |

### Review & Test

| I want to... | Command | Output |
|--------------|---------|--------|
| Run tests | `/test` | Test results with pass/fail |
| Check work status | `/watzup` | Summary of changes, build status |

---

## 📚 "I need to learn about..."

### Architecture & Patterns

| Topic | Read This |
|-------|-----------|
| MVP Pattern | [docs/architecture/mvp-pattern.md](../docs/architecture/mvp-pattern.md) |
| Project Structure | [docs/architecture/project-structure.md](../docs/architecture/project-structure.md) |
| Dependency Injection | [docs/architecture/dependency-injection.md](../docs/architecture/dependency-injection.md) |
| Unit of Work | [docs/data-access/unit-of-work-pattern.md](../docs/data-access/unit-of-work-pattern.md) |

### UI Framework

| Framework | Getting Started | Templates |
|-----------|-----------------|-----------|
| **Standard WinForms** | [Best Practices](../docs/ui-ux/responsive-design.md) | [form-template.cs](../templates/form-template.cs) |
| **ReaLTaiizor** | [Themes Guide](../docs/realtaiizor/realtaiizor-themes.md) | [rt-templates.cs](../templates/rt-templates.cs) |
| **DevExpress** | [Overview](../docs/devexpress/devexpress-overview.md) | [dx-form-templates.cs](../templates/dx-form-templates.cs) |

### Best Practices

| Topic | Read This |
|-------|-----------|
| Coding Standards | [guides/coding-standards.md](guides/coding-standards.md) |
| Production UI Standards | [guides/production-ui/](guides/production-ui/) |
| Async/Await | [docs/best-practices/async-await.md](../docs/best-practices/async-await.md) |
| Testing | [guides/testing-guide.md](guides/testing-guide.md) |

---

## 🛠️ Quick References

### 5 Core Commands

```
/cook          - Full feature workflow (research → implement → test → review)
/plan          - Create implementation plan only
/test          - Run tests
/debug         - Debug issues
/watzup        - Check work status
```

> **Tip**: Use `/cook` for most tasks (create, add, fix, refactor)

### Key Files

| File | Purpose |
|------|---------|
| `CLAUDE.md` | AI quick reference, rules, conventions |
| `.claude/INDEX.md` | Task → Resource mapping |
| `.claude/project-context.md` | Project-specific settings |
| `templates/` | Code templates to start from |

### Key Agents

| Agent | Use For |
|-------|---------|
| `reviewer` | Code review, MVP validation, WinForms best practices |
| `tester` | Generating and running tests |
| `planner` | Creating implementation plans |
| `debugger` | Finding root causes |

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
3. Use: /cook "implement feature"
```

---

## 📖 Full Documentation

For complete documentation index, see: [INDEX.md](INDEX.md)

For practical examples, see: [USAGE_GUIDE.md](../USAGE_GUIDE.md)

For troubleshooting, see: [TROUBLESHOOTING.md](../TROUBLESHOOTING.md)
