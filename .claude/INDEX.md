# Resource Index - When to Read What

> **Purpose**: Help AI decide which resource to read based on task type
> **Goal**: Minimize context usage while ensuring quality

---

## ⚠️ MANDATORY: Read Before ANY UI Task

**Before doing ANY UI-related task**, you **MUST** read:

| Task Type | MUST Read | Section |
|-----------|-----------|---------|
| **Any UI task** | [production-ui-standards.md](guides/production-ui-standards.md) | Relevant section + Section 14 (Checklist) |
| Create Form | production-ui-standards.md | Section 5 (Layout) + Section 14 |
| Create Grid/DataGridView | production-ui-standards.md | Section 2 (Data Display) + Section 14 |
| Create Input controls | production-ui-standards.md | Section 3 (Input) + Section 14 |
| Create Buttons | production-ui-standards.md | Section 4 (Buttons) + Section 14 |

**⚠️ CRITICAL Rules from production-ui-standards.md:**
- ✅ **Grid must use Dock.Fill** - No empty gap below grid
- ✅ **Form starts Maximized** - WindowState = Maximized
- ✅ **NEVER mix UI libraries** - Use only one framework

**If you don't read the guide before doing UI work → code will be rejected!**

---

## Quick Decision Tree

```
Task received?
    ↓
Simple task (fix typo, small change)?
    → CLAUDE.md is enough ✅
    ↓
Complex task?
    → Check tables below → Read relevant sections
    → Need detailed examples? → Read from docs/
```

---

## 📁 Resource Overview

### 1. Guides (`.claude/guides/`) - AI Rules & Patterns

| Guide | Lines | Purpose |
|-------|-------|---------|
| [ai-instructions.md](guides/ai-instructions.md) | 690 | Core DO/DON'T rules, Expert behavior |
| [architecture-guide.md](guides/architecture-guide.md) | 665 | MVP, MVVM, DI, Factory, UoW patterns |
| [code-generation-guide.md](guides/code-generation-guide.md) | 820 | How to generate Forms, Services, Repos, Tests |
| [coding-standards.md](guides/coding-standards.md) | 375 | Naming, style, formatting |
| [testing-guide.md](guides/testing-guide.md) | 572 | Unit tests, Integration tests, Moq |
| [production-ui-standards.md](guides/production-ui-standards.md) | 1800+ | ⭐ **CRITICAL** Production-level UI requirements |

**Total**: ~4,900+ lines

### 2. Agents (`.claude/agents/`) - Specialized AI Tasks

| Agent | Lines | Purpose |
|-------|-------|---------|
| [rules-loader.md](agents/rules-loader.md) | 180 | **Phase 0** - Load ALL coding rules before implementation |
| [winforms-reviewer.md](agents/winforms-reviewer.md) | 585 | Code quality review, PR review |
| [test-generator.md](agents/test-generator.md) | 443 | Auto-generate unit/integration tests |
| [mvp-validator.md](agents/mvp-validator.md) | 495 | Validate MVP/MVVM architecture |
| [docs-manager.md](agents/docs-manager.md) | 382 | Keep docs in sync with code |

**Total**: ~2,085 lines

### 3. Workflows (`.claude/workflows/`) - Process Guides

| Workflow | Lines | Purpose |
|----------|-------|---------|
| [winforms-development-workflow.md](workflows/winforms-development-workflow.md) | 314 | Complete development process |
| [testing-workflow.md](workflows/testing-workflow.md) | 355 | TDD approach |
| [code-review-checklist.md](workflows/code-review-checklist.md) | 278 | Pre-commit checks |
| [pr-review-workflow.md](workflows/pr-review-workflow.md) | 723 | Team collaboration, PR review |
| [expert-behavior-guide.md](workflows/expert-behavior-guide.md) | 412 | How to evaluate requests |

**Total**: ~2,082 lines

### 4. Commands (`.claude/commands/`) - Slash Commands

| Category | Commands | Purpose |
|----------|----------|---------|
| `/create:*` | form, service, repository, dialog, custom-control, **presenter** | Create new components |
| `/add:*` | validation, data-binding, logging, settings, error-handling | Add features |
| `/fix:*` | threading, performance, bug | Fix common issues |
| `/refactor:*` | to-mvp | Refactor existing code |
| `/setup:*` | di | Setup infrastructure |
| `/review:*` | code, pr | Code review |

### 5. Docs (`docs/`) - Detailed Documentation

| Category | Files | Purpose |
|----------|-------|---------|
| `architecture/` | 5 | MVP, MVVM, DI, Factory, Project Structure |
| `best-practices/` | 8 | Async, Error Handling, Thread Safety, Performance, Security |
| `conventions/` | 3 | Naming, Code Style, Comments |
| `data-access/` | 4 | EF Core, Repository, UoW, Connection |
| `testing/` | 5 | Unit, Integration, UI, Coverage |
| `ui-ux/` | 6 | Responsive, Data Binding, Validation, Forms |
| `advanced/` | 5 | Nullable, LINQ, Localization, Profiling |
| `examples/` | 4 | MVP, DI, Async, Testing examples |
| `devexpress/` | 6 | DevExpress specific |
| `realtaiizor/` | 6 | ReaLTaiizor specific |

**Total**: ~53 files, ~15,000+ lines

### 6. Templates (`templates/`) - Code Templates

| Category | Files | Purpose |
|----------|-------|---------|
| Standard | form, service, repository, unitofwork, factory, test, **presenter**, **validator** | Standard WinForms |
| DevExpress | dx-form, dx-grid, dx-lookup, dx-report | DevExpress controls |
| ReaLTaiizor | rt-material-form, rt-metro-form, rt-controls | ReaLTaiizor themes |

### 7. Plans (`plans/templates/`) - Planning Templates

| Template | Purpose |
|----------|---------|
| form-implementation-plan.md | Plan for implementing forms |
| service-implementation-plan.md | Plan for implementing services |
| repository-implementation-plan.md | Plan for implementing repositories |
| testing-plan.md | Plan for testing strategy |
| refactoring-plan.md | Plan for refactoring code |
| template-usage-guide.md | How to use plan templates |

### 8. Scripts (`scripts/`) - Automation Scripts

| Script | Purpose |
|--------|---------|
| init-project.ps1 | Initialize new WinForms project with standards |
| setup-standards.ps1/.sh | Setup standards as Git Submodule |
| update-standards.ps1 | Update standards to latest version |
| fix-symlinks.ps1 | Fix broken symlinks |

### 9. Root Guides - Getting Started

| Guide | Lines | Purpose |
|-------|-------|---------|
| [USAGE_GUIDE.md](../USAGE_GUIDE.md) | 1935 | ⭐ Practical step-by-step examples |
| [QUICK_START.md](../QUICK_START.md) | 146 | 2-minute quick start |
| [INIT_PROJECT_GUIDE.md](../INIT_PROJECT_GUIDE.md) | 388 | How to use init-project.ps1 |
| [INTEGRATION_GUIDE.md](../INTEGRATION_GUIDE.md) | 453 | Integrate standards into project |
| [MIGRATION_GUIDE.md](../MIGRATION_GUIDE.md) | 940 | Migrate existing project |
| [TROUBLESHOOTING.md](../TROUBLESHOOTING.md) | 1089 | Common issues & solutions |

**Total**: ~4,951 lines

### 10. Example Project (`example-project/`)

Complete working Customer Management app demonstrating:
- MVP pattern implementation
- Dependency Injection setup
- Unit of Work pattern
- Unit & Integration tests
- All coding standards applied

---

## 🎯 Task → Resource Mapping

### Creating Components

| Task | Guide | Docs | Template |
|------|-------|------|----------|
| Create Form | code-generation-guide.md | [mvp-pattern.md](../docs/architecture/mvp-pattern.md) | form-template.cs |
| Create Presenter | code-generation-guide.md | [mvp-pattern.md](../docs/architecture/mvp-pattern.md) | presenter-template.cs |
| Create Service | code-generation-guide.md | - | service-template.cs |
| Create Repository | code-generation-guide.md | [repository-pattern.md](../docs/data-access/repository-pattern.md) | repository-template.cs |
| Create Unit of Work | code-generation-guide.md | [unit-of-work-pattern.md](../docs/data-access/unit-of-work-pattern.md) | unitofwork-template.cs |
| Create Validator | code-generation-guide.md | [input-validation.md](../docs/ui-ux/input-validation.md) | validator-template.cs |
| Create Tests | testing-guide.md | [unit-testing.md](../docs/testing/unit-testing.md) | test-template.cs |

### UI Development

| Task | Guide Section to Read | Docs |
|------|----------------------|------|
| DataGridView/Grid | production-ui-standards.md → **Section 2** (~150 lines) | [datagridview-practices.md](../docs/ui-ux/datagridview-practices.md) |
| TextBox/ComboBox/Input | production-ui-standards.md → **Section 3** (~100 lines) | - |
| Buttons | production-ui-standards.md → **Section 4** (~80 lines) | - |
| Form layout | production-ui-standards.md → **Section 5** (~60 lines) | [responsive-design.md](../docs/ui-ux/responsive-design.md) |
| Color palette & theming | production-ui-standards.md → **Section 1** (~150 lines) | - |
| Loading/Toast/Feedback | production-ui-standards.md → **Section 6** (~80 lines) | - |
| Validation & errors | production-ui-standards.md → **Section 7** (~100 lines) | [input-validation.md](../docs/ui-ux/input-validation.md) |
| Accessibility | production-ui-standards.md → **Section 8** (~80 lines) | - |
| Performance (virtual mode) | production-ui-standards.md → **Section 9** (~60 lines) | - |
| Data binding | code-generation-guide.md | [data-binding.md](../docs/ui-ux/data-binding.md) |
| Form communication | - | [form-communication.md](../docs/ui-ux/form-communication.md) |
| **Final check** | production-ui-standards.md → **Section 14 Checklist** (~50 lines) | - |

**⚠️ NOTE**: Only read the relevant section in production-ui-standards.md, do NOT read the entire file!

### Architecture & Patterns

| Task | Guide | Docs |
|------|-------|------|
| MVP pattern | architecture-guide.md | [mvp-pattern.md](../docs/architecture/mvp-pattern.md) |
| MVVM pattern | architecture-guide.md | [mvvm-pattern.md](../docs/architecture/mvvm-pattern.md) |
| Dependency Injection | architecture-guide.md | [dependency-injection.md](../docs/architecture/dependency-injection.md) |
| Factory pattern | architecture-guide.md | [factory-pattern.md](../docs/architecture/factory-pattern.md) |
| Project structure | CLAUDE.md | [project-structure.md](../docs/architecture/project-structure.md) |

### Best Practices

| Task | Guide | Docs |
|------|-------|------|
| Async/await | ai-instructions.md | [async-await.md](../docs/best-practices/async-await.md) |
| Thread safety | ai-instructions.md | [thread-safety.md](../docs/best-practices/thread-safety.md) |
| Error handling | ai-instructions.md | [error-handling.md](../docs/best-practices/error-handling.md) |
| Performance | - | [performance.md](../docs/best-practices/performance.md) |
| Security | - | [security.md](../docs/best-practices/security.md) |

### Testing

| Task | Guide | Docs |
|------|-------|------|
| Unit tests | testing-guide.md | [unit-testing.md](../docs/testing/unit-testing.md) |
| Integration tests | testing-guide.md | [integration-testing.md](../docs/testing/integration-testing.md) |
| UI tests | - | [ui-testing.md](../docs/testing/ui-testing.md) |
| Test coverage | - | [test-coverage.md](../docs/testing/test-coverage.md) |

### DevExpress Projects

| Task | Guide | Docs | Template |
|------|-------|------|----------|
| Setup | ai-instructions.md | [devexpress-overview.md](../docs/devexpress/devexpress-overview.md) | - |
| Grid patterns | - | [devexpress-grid-patterns.md](../docs/devexpress/devexpress-grid-patterns.md) | dx-grid-template.cs |
| Data binding | - | [devexpress-data-binding.md](../docs/devexpress/devexpress-data-binding.md) | - |
| Responsive | - | [devexpress-responsive-design.md](../docs/devexpress/devexpress-responsive-design.md) | dx-form-template.cs |

### ReaLTaiizor Projects

| Task | Guide | Docs | Template |
|------|-------|------|----------|
| Setup | ai-instructions.md | [realtaiizor-overview.md](../docs/realtaiizor/realtaiizor-overview.md) | - |
| Material theme | - | [realtaiizor-forms.md](../docs/realtaiizor/realtaiizor-forms.md) | rt-material-form-template.cs |
| Metro theme | - | [realtaiizor-forms.md](../docs/realtaiizor/realtaiizor-forms.md) | rt-metro-form-template.cs |
| Controls | - | [realtaiizor-controls.md](../docs/realtaiizor/realtaiizor-controls.md) | rt-controls-patterns.cs |

---

## 🤖 Agent Usage

| Task | Agent | When to Use |
|------|-------|-------------|
| **Load rules** | rules-loader.md | **FIRST** - Before any implementation |
| Code review | winforms-reviewer.md | After completing significant code |
| Generate tests | test-generator.md | After creating service/repository |
| Validate architecture | mvp-validator.md | Before PR, architecture questions |
| Sync documentation | docs-manager.md | After code changes |

---

## 📋 Workflow Usage

| Task | Workflow | When to Use |
|------|----------|-------------|
| New feature development | winforms-development-workflow.md | Starting new feature |
| Writing tests | testing-workflow.md | TDD approach |
| Pre-commit check | code-review-checklist.md | Before committing |
| PR review | pr-review-workflow.md | Reviewing PRs |
| Evaluate requests | expert-behavior-guide.md | Complex decisions |

---

## 📝 Planning Templates

| Task | Template |
|------|----------|
| Plan form implementation | plans/templates/form-implementation-plan.md |
| Plan service implementation | plans/templates/service-implementation-plan.md |
| Plan repository implementation | plans/templates/repository-implementation-plan.md |
| Plan testing strategy | plans/templates/testing-plan.md |
| Plan refactoring | plans/templates/refactoring-plan.md |

---

## 🚀 Getting Started Resources

| Task | Resource |
|------|----------|
| Quick start (2 min) | [QUICK_START.md](../QUICK_START.md) |
| Practical examples | [USAGE_GUIDE.md](../USAGE_GUIDE.md) |
| Initialize new project | [INIT_PROJECT_GUIDE.md](../INIT_PROJECT_GUIDE.md) + `scripts/init-project.ps1` |
| Integrate standards | [INTEGRATION_GUIDE.md](../INTEGRATION_GUIDE.md) |
| Migrate existing project | [MIGRATION_GUIDE.md](../MIGRATION_GUIDE.md) |
| Troubleshoot issues | [TROUBLESHOOTING.md](../TROUBLESHOOTING.md) |
| See working example | `example-project/` |

---

## 🔍 Common Issues → What to Read

| Issue | Read This |
|-------|-----------|
| **Frozen UI** | [async-await.md](../docs/best-practices/async-await.md) |
| **Memory leaks** | [resource-management.md](../docs/best-practices/resource-management.md) |
| **Cross-thread exception** | [thread-safety.md](../docs/best-practices/thread-safety.md) |
| **Slow DataGridView** | [datagridview-practices.md](../docs/ui-ux/datagridview-practices.md) |
| **How to test Forms** | [ui-testing.md](../docs/testing/ui-testing.md) |
| **Form not responsive** | [responsive-design.md](../docs/ui-ux/responsive-design.md) |

---

## 📊 Reading Strategy

### Level 1: CLAUDE.md Only (Default)
**When**: Simple tasks, quick fixes, already familiar with patterns
**Context**: ~400 lines

### Level 2: + Guide Sections
**When**: Creating new components, need quick reference
**Context**: +100-200 lines per section

### Level 3: + Docs
**When**: Need detailed examples, first time with pattern, complex implementation
**Context**: +300-500 lines per doc

### Level 4: + Agent/Workflow
**When**: Automated tasks, team processes
**Context**: +300-700 lines per agent/workflow

---

## 📝 Examples

### Example 1: "Create a CustomerForm"
```
1. CLAUDE.md ✅ (already loaded)
2. Check project-context.md for UI framework
3. Read: code-generation-guide.md → "Generating Forms"
4. If need detail: docs/architecture/mvp-pattern.md
5. Use template: form-template.cs (or dx-/rt- variant)
6. After done: Use winforms-reviewer agent
```

### Example 2: "Fix cross-thread UI exception"
```
1. CLAUDE.md ✅ (already loaded)
2. Read: docs/best-practices/thread-safety.md
```

### Example 3: "Review this PR"
```
1. CLAUDE.md ✅ (already loaded)
2. Read: workflows/pr-review-workflow.md
3. Use: winforms-reviewer agent
```

### Example 4: "Create DevExpress grid with CRUD"
```
1. CLAUDE.md ✅ (already loaded)
2. Check project-context.md confirms DevExpress
3. Read: ai-instructions.md → "DevExpress DO/DON'T"
4. Read: docs/devexpress/devexpress-grid-patterns.md
5. Use template: dx-grid-template.cs
```

---

## 📂 File Locations Summary

```
Root/
├── CLAUDE.md             ← Main entry point (always loaded)
├── USAGE_GUIDE.md        ← Practical examples (~1,935 lines)
├── QUICK_START.md        ← 2-minute start
├── TROUBLESHOOTING.md    ← Common issues
├── INTEGRATION_GUIDE.md  ← Setup standards
├── MIGRATION_GUIDE.md    ← Migrate projects
└── INIT_PROJECT_GUIDE.md ← Init script guide

.claude/
├── INDEX.md              ← You are here
├── project-context-template.md
├── guides/               ← AI rules & patterns (~3,100 lines)
├── agents/               ← Specialized tasks (~1,905 lines)
├── workflows/            ← Process guides (~2,082 lines)
└── commands/             ← Slash commands

docs/                     ← Detailed documentation (~15,000+ lines)
├── 00-overview.md        ← Full docs index
├── architecture/
├── best-practices/
├── data-access/
├── testing/
├── ui-ux/
├── devexpress/
└── realtaiizor/

templates/                ← Code templates
├── *-template.cs         ← Standard
├── dx-*.cs               ← DevExpress
└── rt-*.cs               ← ReaLTaiizor

plans/templates/          ← Planning templates
scripts/                  ← Automation scripts
example-project/          ← Complete working app
```

---

## ⚡ Quick Reference

**Remember**:
- ✅ CLAUDE.md has 80% of essential rules (always loaded)
- ✅ Guides have summaries (~3,100 lines)
- ✅ Agents for automated tasks (~1,905 lines)
- ✅ Workflows for processes (~2,082 lines)
- ✅ Docs have detailed examples (~15,000+ lines)
- ✅ Only read what you need for the task
- ✅ Read specific sections, not entire files
