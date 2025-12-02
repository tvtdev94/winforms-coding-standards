# Resource Index - Smart Task Router

> **Purpose**: Help AI decide which resource to read based on task type
> **Goal**: Minimize context usage while ensuring quality
> **Version**: 2.1.0 | **Updated**: 2025-12-02

---

## Quick Router (Read This First!)

### What to Load by Task Type

| Task Type | Load | DON'T Load |
|-----------|------|------------|
| **Simple fix/typo** | CLAUDE.md only | Nothing else |
| **Create Form** | + code-generation-guide.md (Form section) + template | Full docs |
| **Create Service** | + service-template.cs | Guides |
| **Fix bug** | + relevant docs section | Full guides |
| **UI work** | + production-ui/ (relevant section) | All sections |
| **Architecture question** | + architecture-guide.md | Templates |
| **Code review** | + reviewer agent | Docs |

### Framework-Specific Loading

**Check `project-context.md` first, then load ONLY relevant framework:**

| Framework | Templates | Docs |
|-----------|-----------|------|
| **Standard** | form-template.cs, service-template.cs | docs/ui-ux/* |
| **DevExpress** | dx-*.cs templates | docs/devexpress/* |
| **ReaLTaiizor** | rt-*.cs templates | docs/realtaiizor/* |

> **NEVER load** DevExpress docs for Standard project or vice versa!

---

## Token Budget Guide

| Context Level | When to Use | Est. Tokens |
|---------------|-------------|-------------|
| **Minimal** | Simple tasks | ~2K |
| **Standard** | Most tasks | ~5-8K |
| **Full** | Complex features | ~15-20K |

### Minimal Context (Simple tasks)
```
CLAUDE.md (~170 lines)
```

### Standard Context (Most tasks)
```
CLAUDE.md
+ 1 guide section (~100-200 lines)
+ 1 template (~100-300 lines)
```

### Full Context (Complex features)
```
CLAUDE.md
+ 1-2 full guides (~500-800 lines each)
+ 2-3 templates
+ relevant docs sections
```

---

## Mandatory Reading by Task

### UI Tasks - MUST Read Before Coding

| Task | MUST Read | Section Only |
|------|-----------|--------------|
| Any Form | production-ui/ | 05-layout-responsive.md + CHECKLIST.md |
| DataGridView | production-ui/ | 02-data-display.md |
| Input controls | production-ui/ | 03-input-controls.md |
| Buttons | production-ui/ | 04-buttons-actions.md |

**Critical UI Rules:**
- Grid must use `Dock.Fill` - No empty gap
- Form starts `Maximized`
- NEVER mix UI frameworks
- Use Floating Labels, NOT Label+TextBox

---

## Task → Resource Quick Map

### Creating Components

| Create | Template | Guide Section |
|--------|----------|---------------|
| Form | form-template.cs | code-generation-guide.md → Forms |
| Presenter | presenter-template.cs | code-generation-guide.md → Presenters |
| Service | service-template.cs | code-generation-guide.md → Services |
| Repository | repository-template.cs | code-generation-guide.md → Repositories |
| Validator | validator-template.cs | code-generation-guide.md → Validators |
| Tests | test-template.cs | testing-guide.md |

### Fixing Issues

| Issue | Read This |
|-------|-----------|
| Frozen UI | docs/best-practices/async-await.md |
| Memory leak | docs/best-practices/resource-management.md |
| Cross-thread error | docs/best-practices/thread-safety.md |
| Slow DataGridView | docs/ui-ux/datagridview-practices.md |
| Form not responsive | docs/ui-ux/responsive-design.md |

### Architecture Questions

| Question | Read This |
|----------|-----------|
| MVP pattern | architecture-guide.md or docs/architecture/mvp-pattern.md |
| DI setup | architecture-guide.md or docs/architecture/dependency-injection.md |
| Factory pattern | architecture-guide.md → Factory section |
| Unit of Work | architecture-guide.md → UoW section |

---

## Available Resources

### Guides (`.claude/guides/`)

| Guide | Lines | Use For |
|-------|-------|---------|
| ai-instructions.md | 249 | DO/DON'T rules |
| architecture-guide.md | 665 | MVP, DI, Factory, UoW |
| code-generation-guide.md | 388 | Creating components |
| coding-standards.md | 375 | Naming, style |
| testing-guide.md | 572 | Writing tests |
| production-ui/ | modular | UI quality (read sections!) |

### Agents (`.claude/agents/`) - 8 Agents

| Agent | Use For |
|-------|---------|
| reviewer | Code review, MVP validation, WinForms best practices |
| tester | Generate tests, run tests, coverage |
| planner | Create implementation plans |
| debugger | Debug issues |
| researcher | Research technologies |
| git-manager | Commits, PRs |
| docs-manager | Documentation sync |
| rules-loader | Load rules before implementation |

### Workflows (`.claude/workflows/`) - 3 Core Workflows

| Workflow | Use For |
|----------|---------|
| review-workflow.md | Code review process, checklists |
| development-workflow.md | Development rules, orchestration |
| testing-workflow.md | Testing guidelines |

### Templates (`templates/`)

**Standard:**
- form-template.cs
- presenter-template.cs
- service-template.cs
- repository-template.cs
- unitofwork-template.cs
- validator-template.cs
- test-template.cs
- form-factory-template.cs

**DevExpress (only if project uses DX):**
- dx-form-templates.cs (Edit Form + List Form + Grid)
- dx-data-templates.cs (LookUpEdit + Reports)

**ReaLTaiizor (only if project uses RT):**
- rt-templates.cs (Material Form + Metro List + Patterns)

### Commands (`.claude/commands/`) - 6 Core Commands

| Command | Purpose |
|---------|---------|
| `/cook [feature]` | Full workflow: plan → implement → test → review |
| `/plan [feature]` | Create implementation plan only |
| `/fix [bug]` | Fix bugs with streamlined workflow |
| `/test` | Run tests and report results |
| `/debug [issue]` | Debug issues and find root cause |
| `/watzup` | Review recent changes and status |

> `/cook` for features, `/fix` for bugs, `/debug` to analyze only

### Docs (`docs/`)

| Folder | Contents |
|--------|----------|
| architecture/ | MVP, MVVM, DI, Factory, Project Structure |
| best-practices/ | Async, Error, Thread, Performance, Security |
| data-access/ | EF Core, Repository, UoW |
| testing/ | Unit, Integration, UI tests |
| ui-ux/ | Forms, DataBinding, Validation |
| devexpress/ | DX-specific (load only for DX projects) |
| realtaiizor/ | RT-specific (load only for RT projects) |

---

## Example Workflows

### Example 1: "Create CustomerForm"
```
1. CLAUDE.md (already loaded)
2. Check project-context.md → UI Framework
3. Load: code-generation-guide.md → "Forms" section (~100 lines)
4. Load: form-template.cs (or dx-/rt- variant)
5. After: reviewer agent
Total: ~500-800 tokens added
```

### Example 2: "Fix slow DataGridView"
```
1. CLAUDE.md (already loaded)
2. Load: docs/ui-ux/datagridview-practices.md (~300 lines)
Total: ~400 tokens added
```

### Example 3: "Setup DI for new project"
```
1. CLAUDE.md (already loaded)
2. Load: architecture-guide.md → DI section (~150 lines)
3. If need more: docs/architecture/dependency-injection.md
Total: ~300-600 tokens added
```

---

## File Structure

```
.claude/
├── INDEX.md              ← You are here (Smart Router)
├── project-context.md    ← Project config (check first!)
├── guides/               ← AI rules (load sections)
│   └── production-ui/    ← UI standards (modular)
├── agents/               ← 6 specialized agents
├── workflows/            ← 2 core workflows
└── commands/             ← 6 slash commands

templates/                ← Code templates (load by framework)
docs/                     ← Detailed docs (load on-demand)
example-project/          ← Working example
```

---

## Key Principles

1. **CLAUDE.md has core rules** - Always sufficient for simple tasks
2. **Load sections, not files** - Read only relevant parts of guides
3. **Framework-aware** - Only load templates/docs for your framework
4. **On-demand** - Load detailed docs only when needed
5. **Templates first** - Never generate from scratch
