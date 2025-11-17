# C# WinForms Coding Standards - Claude Code Guide

> **Project**: C# WinForms Coding Standards and Best Practices Documentation
> **Purpose**: Quick reference guide for AI assistants building WinForms applications
> **Repository Type**: Submodule reference - Link this repo into C# projects for Claude Code integration

---

## 📊 Project Status

**Repository Completion**: **100%** (62/62 files) 🎉
**Last Updated**: 2025-11-17
**Version**: 5.3.0 (Modular Guide Structure!)

### What's New ✨
- ⭐ **NEW! Modular Guides**: CLAUDE.md refactored into focused guides
- ✅ **5 Specialized Guides**: Coding standards, architecture, code generation, testing, AI instructions
- ✅ **Context Loading Map**: Clear guide for when to read which documentation
- ✅ **Compact Main File**: 200 lines vs. 546 lines (63% reduction!)

### What's Complete ✅
- ✅ **Documentation** (62/62 files) - 100% complete! 🎉
- ✅ **Templates** (7/7) - Form, Service, Repository, Unit of Work, Factory, Test
- ✅ **Slash Commands** (13/13) - Complete command suite
- ✅ **AI Agents** (4/4) - WinForms Reviewer, Test Generator, Docs Manager, MVP Validator
- ✅ **Workflows** (5/5) - Development, Testing, Code Review, PR Review, Expert Behavior
- ✅ **Plan Templates** (6/6) - Feature planning templates
- ✅ **Example Project** - Complete Customer Management app with tests

📊 **Stats**: **~48,000+ lines** of documentation | **250+ code examples** | **65+ tests**

---

## 📦 Tech Stack

- **.NET**: 8.0 (recommended) / .NET Framework 4.8
- **Language**: C# 12.0 / C# 10.0
- **UI Framework**: Windows Forms
- **ORM**: Entity Framework Core 8.0
- **Testing**: xUnit / NUnit + Moq
- **DI Container**: Microsoft.Extensions.DependencyInjection
- **Logging**: Serilog / NLog

---

## 🏗️ Quick Reference

### Project Structure
```
/ProjectName
├── /Forms              # UI Layer (minimal logic)
├── /Presenters         # MVP Presenters
├── /Services           # Business logic
├── /Repositories       # Data access layer
├── /Data               # DbContext, Unit of Work
├── /Factories          # Form factories
└── Program.cs
```

### Architecture Patterns
- **MVP Pattern** ⭐ (recommended) - View + Presenter + Service
- **Factory Pattern** ⭐ (required) - Use `IFormFactory`, NOT `IServiceProvider`
- **Unit of Work** ⭐ (required) - Inject `IUnitOfWork`, NOT `IRepository`
- **Dependency Injection** ⭐ (mandatory)

### Quick Conventions
```
Classes: PascalCase          Methods: PascalCase
Variables: camelCase         Constants: UPPER_SNAKE_CASE
Controls: prefix+PascalCase  (btn, txt, dgv, etc.)
Async methods: MethodNameAsync
```

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

**This does NOT apply to**:
- Code files (`.cs`, `.csproj`, etc.)
- Code templates
- Test files
- Configuration files

**Example**:
```
❌ WRONG: "I'll create a spec.md file for this feature..." [proceeds to create file]
✅ CORRECT: "Should I create a spec.md file to document this feature's requirements?"
```

---

## 🚀 AI Assistant Quick Rules

### ✅ ALWAYS DO:
1. **Use Factory Pattern** - Inject `IFormFactory`, NOT `IServiceProvider`
2. **Use Unit of Work** - Inject `IUnitOfWork`, NOT `IRepository`
3. **Call SaveChangesAsync** - In Unit of Work only, NEVER in repositories
4. **Use async/await** - For all I/O operations
5. **Validate input** - Before processing
6. **Handle errors** - Try-catch with logging
7. **Use templates** - Start with templates from `/templates/` folder
8. **Write tests** - Unit tests for services, integration tests for repositories
9. **Follow MVP** - Separate UI from business logic
10. **Dispose resources** - Use `using` statements

### ❌ NEVER DO:
1. ❌ Business logic in Forms
2. ❌ Inject `IServiceProvider` (use `IFormFactory`)
3. ❌ Call `SaveChangesAsync` in repositories
4. ❌ Inject `IRepository` directly (use `IUnitOfWork`)
5. ❌ Synchronous I/O
6. ❌ Ignore exceptions silently
7. ❌ Magic numbers/strings
8. ❌ Create UI from background threads
9. ❌ Skip validation
10. ❌ Write code without tests

📖 **Full rules**: [.claude/guides/ai-instructions.md](.claude/guides/ai-instructions.md)

---

## 🧠 Context Loading Map

**⚡ CRITICAL**: This repository is designed to be used as a **Git Submodule** in C# WinForms projects.
When AI (Claude Code) starts working on a task, it should **load the appropriate guide** based on the task type.

### When to Read Which Guide

| Task Type | Required Reading | Purpose |
|-----------|------------------|---------|
| **Any WinForms task** | [AI Instructions](.claude/guides/ai-instructions.md) | ⭐ Core DO/DON'T rules (READ FIRST!) |
| **Creating Forms** | [Code Generation Guide](.claude/guides/code-generation-guide.md) + `templates/form-template.cs` | MVP pattern, presenters, view interfaces |
| **Creating Services** | [Code Generation Guide](.claude/guides/code-generation-guide.md) + [Architecture Guide](.claude/guides/architecture-guide.md) | Unit of Work pattern, validation, error handling |
| **Creating Repositories** | [Code Generation Guide](.claude/guides/code-generation-guide.md) | Repository pattern (NO SaveChanges!) |
| **Understanding Architecture** | [Architecture Guide](.claude/guides/architecture-guide.md) | MVP, MVVM, DI, Factory, Unit of Work |
| **Writing Tests** | [Testing Guide](.claude/guides/testing-guide.md) + `templates/test-template.cs` | Unit tests, integration tests, Moq |
| **Coding Standards** | [Coding Standards Guide](.claude/guides/coding-standards.md) | Naming, style, formatting |
| **Code Review** | [Code Review Checklist](.claude/workflows/code-review-checklist.md) | Pre-commit checks |
| **Pull Request Review** | [PR Review Workflow](.claude/workflows/pr-review-workflow.md) | Team collaboration |

### How to Use This Repository

**For AI Assistants (Claude Code)**:
1. **ALWAYS start** by reading [AI Instructions](.claude/guides/ai-instructions.md)
2. **Load the appropriate guide** based on the task (see table above)
3. **Use templates** from `/templates/` folder
4. **Follow the patterns** exactly as documented
5. **Verify** against the Code Review Checklist before committing

**For Developers**:
1. Add this repository as a Git Submodule to your project
2. Point Claude Code to this repository's CLAUDE.md
3. Claude will automatically load the appropriate documentation
4. Use slash commands (type `/` in Claude Code) for common tasks

---

## 📚 Detailed Guides

### Core Guides

| Guide | Description | When to Read |
|-------|-------------|--------------|
| [🤖 AI Instructions](.claude/guides/ai-instructions.md) | ⭐ **START HERE!** Complete DO/DON'T rules for AI | Every task |
| [🏛️ Architecture Guide](.claude/guides/architecture-guide.md) | MVP, MVVM, DI, Factory, Unit of Work patterns | Understanding architecture |
| [⚙️ Code Generation Guide](.claude/guides/code-generation-guide.md) | How to generate Forms, Services, Repositories, Tests | Creating code |
| [✅ Testing Guide](.claude/guides/testing-guide.md) | Unit testing, integration testing, Moq patterns | Writing tests |
| [📝 Coding Standards](.claude/guides/coding-standards.md) | Naming, style, formatting conventions | Code style questions |

### Workflows

- [Development Workflow](.claude/workflows/winforms-development-workflow.md) - Complete dev process
- [Testing Workflow](.claude/workflows/testing-workflow.md) - TDD approach
- [Code Review Checklist](.claude/workflows/code-review-checklist.md) - Pre-commit checks
- [PR Review Workflow](.claude/workflows/pr-review-workflow.md) - Team collaboration
- [Expert Behavior Guide](.claude/workflows/expert-behavior-guide.md) - How to evaluate requests

### Templates (Production-Ready!)

All templates in `/templates/` folder:
- `form-template.cs` - MVP pattern form with presenter
- `service-template.cs` - Business logic with Unit of Work
- `repository-template.cs` - Data access (NO SaveChanges)
- `unitofwork-template.cs` - Transaction coordinator
- `factory-template.cs` - Form factory for DI
- `test-template.cs` - Unit test with Moq

**⚠️ CRITICAL**: NEVER generate code from scratch - ALWAYS start with templates!

---

## 🔧 Common Commands

```bash
# Build project
dotnet build

# Run tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true

# Clean and rebuild
dotnet clean && dotnet build
```

---

## 🎯 Complete Documentation

### Architecture & Patterns
- [Project Structure](docs/architecture/project-structure.md)
- [MVP Pattern](docs/architecture/mvp-pattern.md) ⭐
- [MVVM Pattern](docs/architecture/mvvm-pattern.md)
- [Dependency Injection](docs/architecture/dependency-injection.md) ⭐
- [Factory Pattern](docs/architecture/factory-pattern.md) ⭐

### Data Access
- [Repository Pattern](docs/data-access/repository-pattern.md)
- [Unit of Work Pattern](docs/data-access/unit-of-work-pattern.md) ⭐
- [Entity Framework Core](docs/data-access/entity-framework.md)
- [Connection Management](docs/data-access/connection-management.md)

### Best Practices
- [Async/Await Pattern](docs/best-practices/async-await.md)
- [Error Handling & Logging](docs/best-practices/error-handling.md)
- [Thread Safety](docs/best-practices/thread-safety.md)
- [Resource Management](docs/best-practices/resource-management.md)
- [Performance Optimization](docs/best-practices/performance.md)
- [Security](docs/best-practices/security.md)

### UI & UX
- [Responsive Design](docs/ui-ux/responsive-design.md)
- [Data Binding](docs/ui-ux/data-binding.md)
- [Input Validation](docs/ui-ux/input-validation.md)
- [Form Communication](docs/ui-ux/form-communication.md)

### Testing
- [Testing Overview](docs/testing/testing-overview.md)
- [Unit Testing](docs/testing/unit-testing.md)
- [Integration Testing](docs/testing/integration-testing.md)
- [UI Testing](docs/testing/ui-testing.md)

---

## 🔗 Quick Links

- **[📘 USAGE GUIDE](USAGE_GUIDE.md)** - ⭐ **Start here!** Practical step-by-step examples
- **[🤖 AI Instructions](.claude/guides/ai-instructions.md)** - ⭐ **AI must read!** Core rules
- **[Full Documentation Index](docs/00-overview.md)** - Complete docs
- **[Example Project](example-project/)** - Complete working app
- **[Troubleshooting](TROUBLESHOOTING.md)** - Common issues

---

## 🎓 Learning Path

**For AI Assistants**:
1. ✅ Read [AI Instructions](.claude/guides/ai-instructions.md) - **REQUIRED**
2. ✅ Load appropriate guide based on task (see Context Loading Map)
3. ✅ Use templates - NEVER generate from scratch
4. ✅ Follow patterns exactly
5. ✅ Verify against Code Review Checklist

**For Developers**:
1. Read [USAGE_GUIDE.md](USAGE_GUIDE.md)
2. Understand [MVP Pattern](docs/architecture/mvp-pattern.md)
3. Explore [Example Project](example-project/)
4. Practice with templates

---

## 🤖 AI Agents

Specialized agents for automating tasks:

- [WinForms Reviewer](.claude/agents/winforms-reviewer.md) - Code quality checks, PR review
- [Test Generator](.claude/agents/test-generator.md) - Auto-generate tests
- [Docs Manager](.claude/agents/docs-manager.md) - Keep docs in sync
- [MVP Validator](.claude/agents/mvp-validator.md) - Validate architecture

---

## 📋 Slash Commands

Common commands (type `/` in Claude Code):

**Create**:
- `/create:form` - Create new form with MVP
- `/create:service` - Create service class
- `/create:repository` - Create repository class

**Add Features**:
- `/add:validation` - Add input validation
- `/add:data-binding` - Setup data binding
- `/add:logging` - Setup logging

**Fix Issues**:
- `/fix:threading` - Fix cross-thread UI issues
- `/fix:performance` - Optimize performance
- `/fix:bug` - Smart bug fixing

**Review**:
- `/review-pr <branch>` - Comprehensive PR review
- `/review-code <files>` - Detailed file review

---

## 📞 Need Help?

1. **Quick reference** - This file (you are here!)
2. **Practical examples** - [USAGE_GUIDE.md](USAGE_GUIDE.md)
3. **Detailed guides** - [.claude/guides/](.claude/guides/)
4. **Full documentation** - [docs/00-overview.md](docs/00-overview.md)
5. **Working example** - [example-project/](example-project/)
6. **Issues?** - [TROUBLESHOOTING.md](TROUBLESHOOTING.md)

---

**Last Updated**: 2025-11-17
**Version**: 5.3.0 (Modular Guide Structure!)

**Major Changes in 5.3.0**:
- ✅ Refactored CLAUDE.md into 5 focused guides
- ✅ Created Context Loading Map for AI assistants
- ✅ Reduced main file from 546 to ~200 lines (63% smaller!)
- ✅ Improved organization for submodule use case
- ✅ Clear "when to read which guide" instructions

**Previous Versions**:
- v5.2.0: Added CRITICAL documentation confirmation rule
- v5.1.0: Added complete PR review system
- v5.0.0: Complete repository with 100% documentation coverage
