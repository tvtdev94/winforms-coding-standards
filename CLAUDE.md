# C# WinForms Coding Standards - Claude Code Guide

> **Project**: C# WinForms Coding Standards and Best Practices Documentation
> **Purpose**: Guidelines for building maintainable, scalable WinForms applications

---

## 📊 Project Status

**Repository Completion**: **100%** (62/62 files) 🎉🎉🎉
**Last Updated**: 2025-11-17
**Version**: 5.2.0 (Documentation Confirmation Rule Added!)

### Recent Enhancements ✨
- ✅ **Phase 1**: Workflows extracted, commands organized into categories
- ✅ **Phase 2**: 4 specialized AI agents (code review, testing, docs, MVP validation)
- ✅ **Phase 3**: 6 comprehensive plan templates for feature implementation
- ⭐ **NEW! PR Review System**: Complete team code review workflow with templates
- 📋 **Phase 4 & 5**: Optional enhancements documented in [PHASE_4_5_IMPLEMENTATION_GUIDE.md](PHASE_4_5_IMPLEMENTATION_GUIDE.md)

### What's Complete ✅
- ✅ **Configuration files** (4/4) - .gitignore, .editorconfig, LICENSE, pre-commit hooks
- ✅ **Architecture documentation** (5/5) - MVP, MVVM, DI, **Factory Pattern** ⭐, project structure
- ✅ **Coding conventions** (3/3) - Naming, style, comments
- ✅ **Templates** (7/7) - Form, service, repository, **Unit of Work**, **Factory** ⭐, test, review comments
- ✅ **UI/UX documentation** (6/6) - ~6,800 lines 🎉
- ✅ **Best practices documentation** (8/8) - ~6,200 lines 🎉
- ✅ **Data access documentation** (4/4) - Repository, Connection, EF Core, **Unit of Work** ⭐ ~6,100 lines 🎉
- ✅ **Advanced topics** (5/5) - ~5,700 lines 🎉
- ✅ **Examples documentation** (3/3) - ~2,200 lines 🎉
- ✅ **Testing documentation** (5/5) - ~3,700 lines 🎉
- ✅ **Slash commands** (13/13) - Complete command suite + **PR review commands** ⭐ 🎉
- ✅ **Working example project** - Complete Customer Management app with tests! 🎉
- ✅ **Support docs** (5/5) - USAGE_GUIDE, TROUBLESHOOTING, README, etc.

### Documentation Stats 📊
- **Total lines created**: **~48,000+ lines** (+6,000 new lines!)
- **Total files created**: **78 files** (38 docs + 13 commands + 5 workflows + 4 agents + 6 plan templates + 12 others)
- **Code examples**: **250+ working examples**
- **Review templates**: **25+ reusable comment templates** ⭐
- **Test coverage**: **65+ unit & integration tests** in example project
- **Coverage**: **100% - ALL** core WinForms topics! 🏆
- **Workflows**: 5 specialized development workflows (+ **PR Review**) ⭐
- **AI Agents**: 4 specialized agents (**WinForms Reviewer enhanced for PR review**) ⭐
- **Plan Templates**: 6 comprehensive project planning templates

---

## 📦 Tech Stack

- **.NET**: 8.0 (recommended) / .NET Framework 4.8
- **Language**: C# 12.0 / C# 10.0
- **UI Framework**: Windows Forms
- **ORM**: Entity Framework Core 8.0
- **Testing**: xUnit / NUnit
- **DI Container**: Microsoft.Extensions.DependencyInjection
- **Logging**: Serilog / NLog

---

## 🏗️ Project Structure

Standard WinForms project structure:

```
/ProjectName
    ├── /Forms              # UI Layer (minimal logic)
    ├── /Controls           # Custom user controls
    ├── /Models             # Business/data models
    ├── /Services           # Business logic
    ├── /Repositories       # Data access layer
    ├── /Utils              # Helpers, extensions
    ├── /Resources          # Icons, strings, localization
    ├── Program.cs
    └── App.config
```

**Key Principles**:
- ✅ Forms contain **UI handling only**, no business logic
- ✅ Business logic lives in **Services**
- ✅ Use **Dependency Injection** for loose coupling
- ✅ Follow **MVP** or **MVVM** pattern for larger apps

📖 **Detailed docs**: [docs/architecture/project-structure.md](docs/architecture/project-structure.md)

---

## 🎯 Coding Standards Quick Reference

### Architecture
- **Pattern**: MVP (recommended) or MVVM (.NET 8+)
- **Data Access**: **Unit of Work pattern** (manages repositories & transactions)
- **Form Creation**: **Factory Pattern** (replaces Service Locator anti-pattern)
- **Separation**: UI → Presenter/ViewModel → Service → **Unit of Work** → Repository → Database
- 📖 [MVP Pattern](docs/architecture/mvp-pattern.md) | [MVVM Pattern](docs/architecture/mvvm-pattern.md) | [Unit of Work](docs/data-access/unit-of-work-pattern.md) | [Factory Pattern](docs/architecture/factory-pattern.md)

### Naming Conventions
| Type | Convention | Example |
|------|-----------|---------|
| Class | PascalCase | `CustomerService`, `MainForm` |
| Method | PascalCase | `LoadCustomers()`, `SaveData()` |
| Variable | camelCase | `customerList`, `isActive` |
| Constant | UPPER_SNAKE_CASE | `MAX_RETRY_COUNT` |
| Control | prefix + PascalCase | `btnSave`, `txtName`, `dgvCustomers` |

📖 **Full conventions**: [docs/conventions/naming-conventions.md](docs/conventions/naming-conventions.md)

### Control Prefixes
```
btn → Button        lbl → Label         txt → TextBox
cbx → ComboBox      chk → CheckBox      dgv → DataGridView
grp → GroupBox      tab → TabControl    pnl → Panel
```

---

## ⚙️ Common Commands

```bash
# Build project
dotnet build

# Run tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true

# Run specific test
dotnet test --filter "FullyQualifiedName~ServiceTests"

# Clean and rebuild
dotnet clean && dotnet build
```

---

## 📋 Workflows

Detailed workflows for common development tasks:

- **[Development Workflow](.claude/workflows/winforms-development-workflow.md)** - Complete guide for creating Forms, Services, Repositories
- **[Testing Workflow](.claude/workflows/testing-workflow.md)** - TDD approach, test patterns, coverage goals
- **[Code Review Checklist](.claude/workflows/code-review-checklist.md)** - Pre-commit checks for quality assurance
- **[PR Review Workflow](.claude/workflows/pr-review-workflow.md)** - ⭐ **NEW!** Complete Pull Request review process for team collaboration
- **[Expert Behavior Guide](.claude/workflows/expert-behavior-guide.md)** - How to evaluate and guide user requests

📖 **Pro tip**: Load the appropriate workflow file before starting complex tasks.

---

## 🔍 Code Review (NEW!)

Complete code review system for team collaboration:

### Slash Commands
- **`/review-pr <branch>`** - Comprehensive Pull Request review with team feedback
- **`/review-code <files>`** - Detailed review of specific file(s)

### Resources
- **[PR Review Workflow](.claude/workflows/pr-review-workflow.md)** - Step-by-step PR review process (15-30 min)
- **[Review Comment Templates](templates/review-comment-templates.md)** - Reusable templates for common issues
- **[Code Review Checklist](.claude/workflows/code-review-checklist.md)** - Complete checklist (12 categories)

### Review Modes
1. **Self Review** (2-5 min) - Quick check before committing
2. **File Review** (5-10 min) - Detailed review of specific files
3. **Pull Request Review** (15-30 min) - Full PR review for team members

### Key Features
✅ Categorizes issues by severity (Critical/Major/Minor)
✅ Uses standard comment templates
✅ Provides positive feedback
✅ References documentation
✅ Makes clear recommendations (Approve/Request Changes/Comment)

---

## 🤖 AI Agents

Specialized agents for automating common tasks:

- **[WinForms Reviewer](.claude/agents/winforms-reviewer.md)** - ⭐ **ENHANCED!** Code quality checks, PR review, team collaboration
- **[Test Generator](.claude/agents/test-generator.md)** - Auto-generate unit and integration tests with proper mocking
- **[Docs Manager](.claude/agents/docs-manager.md)** - Keep documentation in sync with code changes
- **[MVP Validator](.claude/agents/mvp-validator.md)** - Validate MVP/MVVM pattern implementation and architecture

📖 **Usage**: Invoke agents when you need automated help with reviews, testing, or documentation.

---

## 📋 Plan Templates

Comprehensive templates for planning feature implementations:

- **[Form Implementation Plan](plans/templates/form-implementation-plan.md)** - Complete plan for creating Forms with MVP
- **[Service Implementation Plan](plans/templates/service-implementation-plan.md)** - Business logic layer planning
- **[Repository Implementation Plan](plans/templates/repository-implementation-plan.md)** - Data access layer planning
- **[Refactoring Plan](plans/templates/refactoring-plan.md)** - Structured approach to code refactoring
- **[Testing Plan](plans/templates/testing-plan.md)** - Comprehensive testing strategy template
- **[Template Usage Guide](plans/templates/template-usage-guide.md)** - How to use all plan templates effectively

📖 **Usage**: Copy a template to `plans/` folder, replace `{{PLACEHOLDERS}}`, and track your progress.

---

## ⚠️ CRITICAL RULE: Documentation Confirmation Required

**🛑 ALWAYS ASK FOR CONFIRMATION BEFORE CREATING DOCUMENTATION FILES**

Before writing ANY specification, planning document, or feature documentation, you **MUST**:
1. Ask the user for explicit confirmation
2. Wait for a clear "yes" or approval from the user
3. Only proceed after receiving confirmation

**This applies to**:
- Creating new specs in `specs/` folder
- Writing `spec.md`, `tasks.md`, `data-model.md`, or similar planning documents
- Creating feature development documentation
- Writing implementation guides or quickstart documents
- Any markdown files related to planning or specifications

**This does NOT apply to**:
- Code files (`.cs`, `.csproj`, etc.)
- Code templates
- Test files
- Configuration files (`.editorconfig`, `.gitignore`, etc.)
- Updating existing documentation when explicitly requested

**Example**:
```
❌ WRONG: "I'll create a spec.md file for this feature..." [proceeds to create file]
✅ CORRECT: "Should I create a spec.md file to document this feature's requirements?"
```

**Why this rule exists**: The user wants to maintain control over what documentation is created in the repository and avoid unnecessary or unwanted documentation files.

---

## 🚀 AI Assistant Rules (IMPORTANT!)

When writing code, **ALWAYS follow these rules**:

### ✅ DO:
1. **Separate concerns**: UI logic in Forms, business logic in Services
2. **Use Factory Pattern**: Inject `IFormFactory` into forms, NOT `IServiceProvider`
3. **Use Unit of Work**: Inject `IUnitOfWork` into services, NOT `IRepository`
4. **Call SaveChangesAsync**: Always call `_unitOfWork.SaveChangesAsync()` after modifications
5. **Use async/await**: For all I/O operations (DB, file, network)
6. **Dispose resources**: Use `using` statements for IDisposable
7. **Validate input**: Always validate user input before processing
8. **Handle errors**: Use try-catch with proper logging
9. **Add XML comments**: For all public APIs
10. **Follow MVP/MVVM**: Don't mix UI and business logic
11. **Use DI**: Constructor injection for dependencies
12. **Write tests**: Unit tests for Services, integration tests for Repositories
13. **Thread-safe UI**: Use `Invoke`/`BeginInvoke` for cross-thread UI updates

### ❌ DON'T:
1. ❌ Put business logic in Forms
2. ❌ **Inject IServiceProvider into forms** (use IFormFactory instead - Service Locator is anti-pattern!)
3. ❌ **Call SaveChangesAsync in repositories** (use Unit of Work instead)
4. ❌ **Inject IRepository directly** (inject IUnitOfWork into services)
5. ❌ Use synchronous I/O (use async instead)
6. ❌ Leave resources undisposed (memory leaks)
7. ❌ Ignore exceptions silently
8. ❌ Use magic numbers/strings (use constants)
9. ❌ Create UI controls from background threads
10. ❌ Hardcode connection strings (use configuration)
11. ❌ Skip input validation
12. ❌ Write code without tests
13. ❌ Use Hungarian notation excessively

---

## 👨‍🏫 Expert Behavior

**YOU ARE A WINFORMS CODING STANDARDS EXPERT** - Not just a code generator!

📖 **Full guide**: [.claude/workflows/expert-behavior-guide.md](.claude/workflows/expert-behavior-guide.md)

**Core Principle**: Evaluate requests, educate on best practices, suggest better alternatives.

**Key actions**:
- ✅ Approve requests that follow best practices
- ⚠️ Warn about potential issues with explanations
- ❌ Block anti-patterns, suggest proper solutions
- 📚 Reference Microsoft docs and industry standards

---

## 🧠 Claude Code Context Loading

When starting a new coding task, follow this context loading strategy:

### 1. **This File is Auto-Loaded**
CLAUDE.md is automatically loaded at the start of every session. You already have the core guidelines.

### 2. **Load Context Based on Task Type**

| Task Type | Files to Read |
|-----------|--------------|
| **Creating a new Form** | `templates/form-template.cs`, `docs/architecture/mvp-pattern.md`, `docs/ui-ux/input-validation.md` |
| **Creating a Service** | `templates/service-template.cs`, `docs/best-practices/async-await.md`, `docs/best-practices/error-handling.md` |
| **Creating a Repository** | `templates/repository-template.cs`, `docs/data-access/entity-framework.md`, `docs/data-access/repository-pattern.md` |
| **Writing Tests** | `templates/test-template.cs`, `docs/testing/unit-testing.md`, `docs/testing/integration-testing.md` |
| **Data Binding** | `docs/ui-ux/data-binding.md`, `docs/ui-ux/datagridview-practices.md` |
| **Form Communication** | `docs/ui-ux/form-communication.md`, `docs/architecture/mvp-pattern.md` |
| **Error Handling** | `docs/best-practices/error-handling.md`, `docs/best-practices/resource-management.md` |
| **Thread Safety** | `docs/best-practices/thread-safety.md`, `docs/best-practices/async-await.md` |
| **Performance** | `docs/best-practices/performance.md`, `docs/advanced/performance-profiling.md` |
| **Security** | `docs/best-practices/security.md`, `docs/data-access/connection-management.md` |
| **General Questions** | `CODE_REVIEW_REPORT.md`, `USAGE_GUIDE.md` for practical examples |

### 3. **Documentation Availability** ✅

✅ **ALL DOCS AVAILABLE**: Complete documentation coverage for all WinForms topics!

**Quick Access**:
- **UI/UX**: All 6 docs complete (responsive design, data binding, validation, etc.)
- **Best Practices**: All 8 docs complete (async/await, threading, security, etc.)
- **Data Access**: All 3 docs complete (EF Core, repositories, connections)
- **Testing**: All 5 docs complete (unit, integration, UI, coverage)
- **Advanced**: All 5 docs complete (nullable types, LINQ, i18n, profiling)
- **Examples**: 3 complete working examples with full code

See [COMPLETION_STATUS.md](COMPLETION_STATUS.md) for full file list.

### 4. **Always Use Templates**

Templates are **production-ready** and follow all standards:
- `/templates/form-template.cs` - MVP pattern form
- `/templates/service-template.cs` - Business logic service with Unit of Work
- `/templates/repository-template.cs` - Data access repository (NO SaveChanges)
- `/templates/unitofwork-template.cs` - Unit of Work pattern implementation
- `/templates/test-template.cs` - Unit test structure

**Never generate code from scratch** - always start with templates!

---

## 🎨 Code Generation Patterns

📖 **Detailed patterns**: [.claude/workflows/winforms-development-workflow.md](.claude/workflows/winforms-development-workflow.md#code-generation-patterns)

**Key patterns**:
1. **Creating Forms** - Use form-template.cs, implement MVP, async handlers
2. **Creating Services** - Use service-template.cs, DI, async methods, validation
3. **Creating Repositories** - Use repository-template.cs, EF Core async, proper disposal
4. **Writing Tests** - Use test-template.cs, Moq, AAA pattern, proper naming
5. **Code Review** - Check against checklist, DO/DON'T rules, templates

---

## 🎯 Code Generation Rules Summary

### When generating Forms:
1. ✅ Start with `form-template.cs`
2. ✅ Implement MVP pattern (Form + IView + Presenter)
3. ✅ Async event handlers for data operations
4. ✅ Try-catch with user-friendly error messages
5. ✅ Dispose resources in Dispose() method
6. ✅ Set TabIndex for proper keyboard navigation
7. ✅ Use meaningful control names (not button1, textBox1)

### When generating Services:
1. ✅ Start with `service-template.cs`
2. ✅ **Inject `IUnitOfWork`, NOT `IRepository`** (Unit of Work pattern)
3. ✅ Access repositories via `_unitOfWork.EntityName` (e.g., `_unitOfWork.Customers`)
4. ✅ **Call `await _unitOfWork.SaveChangesAsync()` after Add/Update/Delete operations**
5. ✅ Validate all inputs (ArgumentNullException, ArgumentException)
6. ✅ Async methods with proper cancellation token support
7. ✅ Log all operations (info, errors, warnings)
8. ✅ Wrap exceptions with meaningful messages
9. ✅ XML documentation on all public methods

### When generating Repositories:
1. ✅ Start with `repository-template.cs`
2. ✅ Implement generic repository pattern with entity-specific interface
3. ✅ **NEVER call `SaveChangesAsync()` in repositories** (handled by Unit of Work)
4. ✅ Use EF Core async methods (ToListAsync, FirstOrDefaultAsync, etc.)
5. ✅ Use `AsNoTracking()` for read-only queries
6. ✅ Return `Task.CompletedTask` for Update/Delete (no SaveChanges)
7. ✅ Include soft-delete support if applicable

### When creating Unit of Work:
1. ✅ Use `unitofwork-template.cs` as starting point
2. ✅ Add repository properties for each entity (lazy-loaded)
3. ✅ Implement `SaveChangesAsync()` method
4. ✅ Implement transaction methods (Begin/Commit/Rollback)
5. ✅ Proper disposal pattern
6. ✅ Register as `Scoped` in DI (one instance per scope)

### When generating Tests:
1. ✅ Start with `test-template.cs`
2. ✅ One test class per class under test
3. ✅ Use Moq for mocking dependencies
4. ✅ Arrange-Act-Assert structure
5. ✅ Test naming: `MethodName_Scenario_ExpectedResult`
6. ✅ Test both success and failure scenarios
7. ✅ Use Assert.Throws for exception testing

---

## 📚 Documentation Structure

### Core Documentation
- **[Overview](docs/00-overview.md)** - Full documentation index

### Architecture & Design
- [Project Structure](docs/architecture/project-structure.md)
- [MVP Pattern](docs/architecture/mvp-pattern.md)
- [MVVM Pattern](docs/architecture/mvvm-pattern.md)
- [Dependency Injection](docs/architecture/dependency-injection.md)

### Conventions
- [Naming Conventions](docs/conventions/naming-conventions.md)
- [Code Style](docs/conventions/code-style.md)
- [Comments & Docstrings](docs/conventions/comments-docstrings.md)

### UI & UX
- [Responsive Design](docs/ui-ux/responsive-design.md)
- [Form Communication](docs/ui-ux/form-communication.md)
- [Data Binding](docs/ui-ux/data-binding.md)
- [Input Validation](docs/ui-ux/input-validation.md)
- [DataGridView Best Practices](docs/ui-ux/datagridview-practices.md)

### Best Practices
- [Async/Await Pattern](docs/best-practices/async-await.md)
- [Resource Management](docs/best-practices/resource-management.md)
- [Error Handling & Logging](docs/best-practices/error-handling.md)
- [Thread Safety](docs/best-practices/thread-safety.md)
- [Performance Optimization](docs/best-practices/performance.md)
- [Security](docs/best-practices/security.md)
- [Configuration Management](docs/best-practices/configuration.md)

### Testing
- [Testing Overview](docs/testing/testing-overview.md)
- [Unit Testing](docs/testing/unit-testing.md)
- [Integration Testing](docs/testing/integration-testing.md)
- [UI Testing](docs/testing/ui-testing.md)

### Advanced Topics
- [Nullable Reference Types](docs/advanced/nullable-reference-types.md)
- [LINQ Best Practices](docs/advanced/linq-practices.md)
- [Localization (i18n)](docs/advanced/localization-i18n.md)

### Examples
- [MVP Example](docs/examples/mvp-example.md)
- [DI Example](docs/examples/di-example.md)
- [Async UI Example](docs/examples/async-ui-example.md)

---

## 🔧 Code Templates

Use templates from `/templates/` folder:
- `form-template.cs` - Standard Form with MVP pattern
- `service-template.cs` - Service layer template
- `repository-template.cs` - Repository pattern template
- `test-template.cs` - Unit test template

---

## ✅ Pre-Commit Checklist

📖 **Full checklist**: [.claude/workflows/code-review-checklist.md](.claude/workflows/code-review-checklist.md)

**Quick check**:
- [ ] Code compiles, tests pass
- [ ] No business logic in Forms
- [ ] Resources disposed, async/await used
- [ ] Input validated, errors handled
- [ ] Code follows naming conventions

---

## 🔗 Quick Links

- **[📘 USAGE GUIDE](USAGE_GUIDE.md)** - ⭐ **Start here!** Practical step-by-step examples
- **[Full Overview](docs/00-overview.md)** - Complete documentation index
- **[MVP Pattern Guide](docs/architecture/mvp-pattern.md)** - Recommended architecture
- **[Testing Guide](docs/testing/testing-overview.md)** - How to test WinForms apps
- **[Code Examples](docs/examples/)** - Working code samples
- **[Working Example Project](example-project/)** - Complete Customer Management app 🎉

---

## 📞 Need Help?

1. **[USAGE_GUIDE.md](USAGE_GUIDE.md)** - ⭐ Practical examples (Login form, Customer form, etc.)
2. Check **[docs/00-overview.md](docs/00-overview.md)** for full documentation
3. Search for specific topic in `/docs/` folders
4. Review **[examples](docs/examples/)** and **[example-project](example-project/)** for working code
5. Use slash commands (type `/` in Claude Code) for common tasks

---

## 🎓 Learning Path

**For new developers**:
1. Read [Project Structure](docs/architecture/project-structure.md)
2. Understand [MVP Pattern](docs/architecture/mvp-pattern.md)
3. Review [Naming Conventions](docs/conventions/naming-conventions.md)
4. Study [Code Examples](docs/examples/)
5. **Explore [Working Example Project](example-project/)** - Complete app demonstrating all patterns
6. Practice with templates from `/templates/`

**For AI assistants**:
1. Load this file first (automatic)
2. **Check project status** - Know what docs exist vs missing
3. Reference specific docs as needed for deep dives
4. **Always use templates** - Never generate code from scratch
5. Follow code generation patterns above
6. Follow pre-commit checklist before suggesting commits
7. Validate against DO/DON'T rules before responding

---

## 📝 Common Code Snippets

📖 **Full snippets**: [.claude/workflows/winforms-development-workflow.md](.claude/workflows/winforms-development-workflow.md#common-code-snippets)

- Async button click handler
- Thread-safe UI updates
- Proper resource disposal

---

**Last Updated**: 2025-11-17
**Version**: 5.2.0 (Documentation Confirmation Rule Added!)
**Changes**:
- Phase 1: Extracted 4 workflows, organized commands into categories, added metadata.json
- Phase 2: Added 4 specialized AI agents (reviewer, test-generator, docs-manager, mvp-validator)
- Phase 3: Created 6 comprehensive plan templates with placeholder system
- Phase 4 & 5: Documented optional enhancements in PHASE_4_5_IMPLEMENTATION_GUIDE.md
- **Version 5.1.0**: Added complete PR review system for team collaboration
  - New slash commands: /review-pr, /review-code
  - New workflow: pr-review-workflow.md (comprehensive 5-phase process)
  - New templates: review-comment-templates.md (25+ reusable templates)
  - Enhanced: winforms-reviewer agent (v2.0 with PR review mode)
  - Updated: CLAUDE.md with Code Review section
- **Version 5.2.0**: Added CRITICAL documentation confirmation rule
  - AI must always ask for explicit confirmation before creating any documentation files
  - Prevents unwanted spec.md, tasks.md, and planning documents
  - Gives user full control over documentation creation
