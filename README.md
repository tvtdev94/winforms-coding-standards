# 📘 C# WinForms Coding Standards & Best Practices

> Comprehensive coding standards and best practices for building maintainable, scalable C# WinForms applications. Optimized for both human developers and AI assistants (Claude Code).

[![.NET Version](https://img.shields.io/badge/.NET-8.0%20%7C%204.8-blue)](https://dotnet.microsoft.com/)
[![C# Version](https://img.shields.io/badge/C%23-12.0-brightgreen)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

---

## 🎯 What's This?

This repository contains **coding standards, architectural guidelines, and best practices** for C# WinForms development. It's designed to:

- ✅ Ensure code **consistency** across team members
- ✅ Provide **clear guidelines** for architectural decisions (MVP, MVVM, etc.)
- ✅ Help **AI assistants** (like Claude Code) write better WinForms code
- ✅ Serve as **reference documentation** for common patterns
- ✅ Include **code examples** and templates for quick start

---

## 🚀 Quick Start

### 📘 **[USAGE GUIDE - Start Here!](USAGE_GUIDE.md)** ⭐

**New to this repository?** Check out the [**Usage Guide**](USAGE_GUIDE.md) with **practical, step-by-step examples**:
- 📝 Creating a Login Form from scratch
- 📝 Creating a Customer Management Form
- 📝 Adding validation to existing forms
- 📝 Refactoring to MVP pattern
- 📝 Setting up Dependency Injection
- 📝 And more real-world scenarios!

### ⚡ **Productivity Tools** - Speed Up Development

Boost your productivity with our automation tools:

- **[Code Snippets](snippets/)** 🎨
  - 7 ready-to-use snippets for Visual Studio & VS Code
  - Generate complete MVP forms in 10 seconds
  - Shortcuts: `mvpform`, `mvpservice`, `asyncevent`, `invokeui`
  - [Installation Guide](snippets/README.md)

- **[Pre-commit Hooks](.githooks/)** 🔒
  - Automatic quality checks before commits
  - Prevents broken builds, failing tests, debug code
  - 9 automated validations
  - [Setup Instructions](.githooks/README.md)

- **[Project Init Scripts](scripts/)** 🚀
  - Create new projects in 2 minutes (vs 30-60 min manual)
  - Pre-configured with DI, EF Core, Serilog, tests
  - Command: `.\scripts\init-project.ps1 -ProjectName "MyApp"`
  - [Usage Guide](scripts/README.md)

### For Developers

1. **Start with practical examples**: [**USAGE_GUIDE.md**](USAGE_GUIDE.md) ⭐
2. **Install productivity tools**: [Snippets](snippets/) + [Hooks](.githooks/) + [Scripts](scripts/)
3. **Read the overview**: [docs/00-overview.md](docs/00-overview.md)
4. **Understand architecture**: [MVP Pattern](docs/architecture/mvp-pattern.md)
5. **Follow conventions**: [Naming Conventions](docs/conventions/naming-conventions.md)
6. **Review examples**: [Code Examples](docs/examples/) and [Example Project](example-project/)

### For AI Assistants (Claude Code)

The [CLAUDE.md](CLAUDE.md) file is automatically loaded by Claude Code and contains:
- Quick reference for coding standards
- Links to detailed documentation
- Pre-commit checklist
- AI-specific rules

---

## 📚 Documentation Structure

```
/winforms-coding-standards
├── CLAUDE.md                  # Auto-loaded by Claude Code
├── README.md                  # This file
├── USAGE_GUIDE.md             # ⭐ Practical step-by-step examples
│
├── /snippets/                 # ⚡ Code snippets for rapid development
│   ├── /visual-studio/        # VS .snippet files
│   ├── /vscode/               # VS Code JSON snippets
│   └── README.md              # Installation guide
│
├── /.githooks/                # 🔒 Pre-commit quality checks
│   ├── pre-commit             # Main hook script
│   ├── install.sh             # Installation script
│   └── README.md              # Hook documentation
│
├── /scripts/                  # 🚀 Project automation scripts
│   ├── init-project.ps1       # PowerShell (Windows)
│   ├── init-project.sh        # Bash (Linux/Mac)
│   └── README.md              # Usage guide
│
├── /docs/
│   ├── 00-overview.md         # Complete documentation index
│   │
│   ├── /architecture/         # Architecture patterns & design
│   │   ├── project-structure.md
│   │   ├── mvp-pattern.md
│   │   ├── mvvm-pattern.md
│   │   └── dependency-injection.md
│   │
│   ├── /conventions/          # Coding conventions
│   │   ├── naming-conventions.md
│   │   ├── code-style.md
│   │   └── comments-docstrings.md
│   │
│   ├── /ui-ux/               # UI & UX best practices
│   │   ├── responsive-design.md
│   │   ├── form-communication.md
│   │   ├── data-binding.md
│   │   ├── input-validation.md
│   │   └── datagridview-practices.md
│   │
│   ├── /best-practices/      # General best practices
│   │   ├── async-await.md
│   │   ├── resource-management.md
│   │   ├── error-handling.md
│   │   ├── thread-safety.md
│   │   ├── performance.md
│   │   ├── security.md
│   │   └── configuration.md
│   │
│   ├── /testing/             # Testing guidelines
│   │   ├── testing-overview.md
│   │   ├── unit-testing.md
│   │   ├── integration-testing.md
│   │   └── ui-testing.md
│   │
│   ├── /advanced/            # Advanced topics
│   │   ├── nullable-reference-types.md
│   │   ├── linq-practices.md
│   │   └── localization-i18n.md
│   │
│   ├── /deployment/          # Deployment & packaging
│   │   └── packaging.md
│   │
│   └── /examples/            # Working code examples
│       ├── mvp-example.md
│       ├── di-example.md
│       └── async-ui-example.md
│
├── /.claude/
│   └── /commands/            # Claude Code slash commands
│       ├── create-form.md
│       ├── review-code.md
│       └── add-test.md
│
└── /templates/               # Code templates
    ├── form-template.cs
    ├── service-template.cs
    ├── repository-template.cs
    └── test-template.cs
```

---

## 📖 Key Topics

### Architecture
- [**Project Structure**](docs/architecture/project-structure.md) - Standard folder organization
- [**MVP Pattern**](docs/architecture/mvp-pattern.md) - Recommended for WinForms ⭐
- [**MVVM Pattern**](docs/architecture/mvvm-pattern.md) - For .NET 8+ with data binding
- [**Dependency Injection**](docs/architecture/dependency-injection.md) - Loose coupling

### Best Practices
- [**Async/Await**](docs/best-practices/async-await.md) - Non-blocking UI operations
- [**Resource Management**](docs/best-practices/resource-management.md) - Prevent memory leaks
- [**Error Handling**](docs/best-practices/error-handling.md) - Logging and exception handling
- [**Thread Safety**](docs/best-practices/thread-safety.md) - Cross-thread UI updates
- [**Performance**](docs/best-practices/performance.md) - Optimization techniques
- [**Security**](docs/best-practices/security.md) - Secure coding practices

### UI & UX
- [**Responsive Design**](docs/ui-ux/responsive-design.md) - Adaptive layouts
- [**Data Binding**](docs/ui-ux/data-binding.md) - BindingSource pattern
- [**Input Validation**](docs/ui-ux/input-validation.md) - ErrorProvider usage
- [**DataGridView**](docs/ui-ux/datagridview-practices.md) - Advanced grid techniques

### Testing
- [**Testing Overview**](docs/testing/testing-overview.md) - Testing strategy
- [**Unit Testing**](docs/testing/unit-testing.md) - xUnit with Services
- [**Integration Testing**](docs/testing/integration-testing.md) - Database testing
- [**UI Testing**](docs/testing/ui-testing.md) - FlaUI automation

---

## 💡 Core Principles

### 1. Separation of Concerns
```
UI (Forms) → Presenter/ViewModel → Service → Repository → Database
```
- ✅ Forms handle **UI only** (no business logic)
- ✅ Services contain **business logic**
- ✅ Repositories handle **data access**

### 2. Modern C# Features
- Use **async/await** for all I/O operations
- Enable **nullable reference types** (C# 8.0+)
- Use **pattern matching** where appropriate
- Leverage **LINQ** for data manipulation

### 3. Resource Management
- Always **dispose** `IDisposable` resources
- Use **using statements** or **using declarations**
- Handle **unmanaged resources** properly

### 4. Testing
- **Unit tests** for Services (business logic)
- **Integration tests** for Repositories (data access)
- **UI tests** for critical user flows (optional)
- Aim for **80%+ code coverage**

---

## 🎓 Learning Path

### Beginner
1. [Project Structure](docs/architecture/project-structure.md) - Understand folder organization
2. [Naming Conventions](docs/conventions/naming-conventions.md) - Learn naming rules
3. [Code Style](docs/conventions/code-style.md) - Follow style guidelines

### Intermediate
4. [MVP Pattern](docs/architecture/mvp-pattern.md) - Master architecture pattern
5. [Async/Await](docs/best-practices/async-await.md) - Non-blocking operations
6. [Data Binding](docs/ui-ux/data-binding.md) - BindingSource usage
7. [Unit Testing](docs/testing/unit-testing.md) - Write effective tests

### Advanced
8. [Thread Safety](docs/best-practices/thread-safety.md) - Multi-threaded UI
9. [Performance Optimization](docs/best-practices/performance.md) - Profiling & tuning
10. [Localization](docs/advanced/localization-i18n.md) - Multi-language support

---

## 🛠️ Tools & Technologies

- **.NET 8.0** / .NET Framework 4.8
- **C# 12.0** / C# 10.0
- **Entity Framework Core 8.0**
- **xUnit** / NUnit for testing
- **Serilog** / NLog for logging
- **Microsoft.Extensions.DependencyInjection** for DI
- **FlaUI** for UI automation testing (optional)

---

## 🤝 Contributing

Contributions are welcome! To contribute:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/new-guideline`)
3. Add or update documentation
4. Ensure examples are tested and working
5. Submit a pull request

### Guidelines for Contributors
- Keep documentation **concise** (200-500 lines per file)
- Include **code examples** with explanations
- Use **markdown best practices** (headings, lists, code blocks)
- Cross-reference related topics with links
- Test all code examples before committing

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- Microsoft's [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [.NET Framework Design Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/)
- Community best practices from WinForms developers

---

## 📞 Support

- **Documentation Issues**: Open an issue on GitHub
- **Questions**: Check [docs/00-overview.md](docs/00-overview.md) first
- **AI Assistant Help**: See [CLAUDE.md](CLAUDE.md)

---

**Last Updated**: 2025-11-07
**Version**: 2.0 (Modular structure)
