# C# WinForms Coding Standards - Complete Documentation

> **Navigation Hub**: This document provides a complete index of all coding standards and best practices documentation.

---

## 📖 Documentation Map

### 🏗️ Architecture & Design Patterns

Master the architectural patterns and project organization for scalable WinForms applications.

- **[Project Structure](architecture/project-structure.md)**
  - Standard folder organization
  - Layer separation (UI, Services, Repositories)
  - File naming conventions

- **[MVP Pattern](architecture/mvp-pattern.md)** ⭐ RECOMMENDED
  - Model-View-Presenter explained
  - Implementation guide
  - When to use MVP vs MVVM

- **[MVVM Pattern](architecture/mvvm-pattern.md)**
  - Model-View-ViewModel for .NET 8+
  - Data binding with ViewModels
  - INotifyPropertyChanged implementation

- **[Dependency Injection](architecture/dependency-injection.md)**
  - Microsoft.Extensions.DependencyInjection setup
  - Service lifetime (Singleton, Scoped, Transient)
  - DI in WinForms applications

- **[Factory Pattern](architecture/factory-pattern.md)** ⭐ RECOMMENDED
  - Form creation with dependency injection
  - Replaces Service Locator anti-pattern
  - IFormFactory implementation and testing

---

### 🎨 Conventions & Code Style

Consistent naming and style guidelines for readable, maintainable code.

- **[Naming Conventions](conventions/naming-conventions.md)**
  - Class, method, variable naming
  - Control prefix conventions (btn, txt, dgv, etc.)
  - Namespace organization

- **[Code Style](conventions/code-style.md)**
  - Formatting rules (braces, indentation)
  - Spacing and line breaks
  - Code organization within files

- **[Comments & Docstrings](conventions/comments-docstrings.md)**
  - XML documentation comments
  - When to comment (and when not to)
  - TODO/FIXME conventions

---

### 🖥️ UI & UX Best Practices

Build responsive, user-friendly WinForms interfaces.

- **[Responsive Design & Layout](ui-ux/responsive-design.md)**
  - Anchor and Dock properties
  - TableLayoutPanel and FlowLayoutPanel
  - DPI awareness and scaling

- **[Form Communication](ui-ux/form-communication.md)**
  - Parent-child form communication
  - Event-driven communication
  - Using interfaces for loose coupling

- **[Data Binding](ui-ux/data-binding.md)**
  - BindingSource pattern
  - Two-way binding with controls
  - INotifyPropertyChanged integration

- **[Input Validation](ui-ux/input-validation.md)**
  - ErrorProvider usage
  - Validation patterns
  - Real-time vs on-submit validation

- **[Keyboard Navigation & Accessibility](ui-ux/keyboard-navigation.md)**
  - Tab order management
  - Keyboard shortcuts
  - Screen reader support

- **[DataGridView Best Practices](ui-ux/datagridview-practices.md)**
  - Virtual mode for large datasets
  - Custom cell formatting
  - Performance optimization

---

### 💾 Data Access

Patterns for efficient, maintainable database interactions.

- **[Entity Framework Core](data-access/entity-framework.md)**
  - DbContext configuration
  - Migrations
  - Query optimization

- **[Repository Pattern](data-access/repository-pattern.md)**
  - Generic repository implementation
  - Repository best practices
  - Testing with repositories

- **[Unit of Work Pattern](data-access/unit-of-work-pattern.md)** ⭐ **NEW!**
  - Centralized transaction management
  - Multiple repository coordination
  - Explicit transaction control
  - Testing with Unit of Work

- **[Connection Management](data-access/connection-management.md)**
  - Connection pooling
  - Connection string security
  - Retry policies

---

### ⚡ Best Practices

Essential patterns and practices for robust WinForms applications.

- **[Async/Await Pattern](best-practices/async-await.md)**
  - Non-blocking UI operations
  - Task-based asynchronous programming
  - Common async pitfalls

- **[Resource Management](best-practices/resource-management.md)**
  - IDisposable pattern
  - using statements and declarations
  - Memory leak prevention

- **[Error Handling & Logging](best-practices/error-handling.md)**
  - Try-catch best practices
  - Logging with Serilog/NLog
  - Global exception handling

- **[Thread Safety & Cross-Thread UI](best-practices/thread-safety.md)**
  - Invoke and BeginInvoke
  - SynchronizationContext
  - BackgroundWorker vs Task

- **[Performance Optimization](best-practices/performance.md)**
  - Profiling tools
  - Common performance bottlenecks
  - Lazy loading and caching

- **[Security Best Practices](best-practices/security.md)**
  - Input sanitization
  - SQL injection prevention
  - Secure credential storage

- **[Configuration Management](best-practices/configuration.md)**
  - App.config vs appsettings.json
  - User Secrets
  - Environment-specific configuration

- **[Memory Management](best-practices/memory-management.md)**
  - GC optimization
  - Memory profiling
  - Detecting memory leaks

---

### 🧪 Testing

Comprehensive testing strategies for WinForms applications.

- **[Testing Overview](testing/testing-overview.md)**
  - Testing pyramid
  - What to test (and what not to)
  - Testing tools and frameworks

- **[Unit Testing](testing/unit-testing.md)**
  - Testing Services with xUnit
  - Mocking dependencies
  - Test patterns (AAA, Given-When-Then)

- **[Integration Testing](testing/integration-testing.md)**
  - In-memory database testing
  - Testing with real databases
  - Test data management

- **[UI Testing](testing/ui-testing.md)**
  - FlaUI automation
  - Testing user interactions
  - Screenshot testing

- **[Test Coverage](testing/test-coverage.md)**
  - Coverlet setup
  - Coverage reports
  - Coverage thresholds

---

### 🚀 Advanced Topics

Advanced patterns and techniques for experienced developers.

- **[Nullable Reference Types](advanced/nullable-reference-types.md)**
  - C# 8.0+ nullable annotations
  - Null-checking patterns
  - Migration strategies

- **[String Handling](advanced/string-handling.md)**
  - StringBuilder usage
  - String interpolation vs concatenation
  - Globalization considerations

- **[LINQ Best Practices](advanced/linq-practices.md)**
  - Query syntax vs method syntax
  - Deferred execution
  - Performance considerations

- **[Localization & i18n](advanced/localization-i18n.md)**
  - Resource files (.resx)
  - CultureInfo management
  - Right-to-left (RTL) support

- **[Performance Profiling](advanced/performance-profiling.md)**
  - dotTrace, dotMemory
  - PerfView
  - Analyzing bottlenecks

---

### 📦 Deployment & Packaging

Strategies for distributing WinForms applications.

- **[Packaging & Deployment](deployment/packaging.md)**
  - Single-file executables
  - Framework-dependent vs self-contained
  - MSIX packaging
  - ClickOnce deployment

---

### 📚 Code Examples

Working code samples demonstrating key patterns.

- **[MVP Pattern Example](examples/mvp-example.md)**
  - Complete MVP implementation
  - Form, Presenter, and Model interaction
  - Testing the Presenter

- **[Dependency Injection Example](examples/di-example.md)**
  - Setting up DI container
  - Constructor injection
  - Service registration

- **[Async UI Example](examples/async-ui-example.md)**
  - Loading data asynchronously
  - Progress reporting
  - Cancellation support

- **[Testing Example](examples/testing-example.md)**
  - Unit test structure
  - Mocking with Moq
  - Assert patterns

---

## 🗂️ Documentation by Category

### By Skill Level

#### Beginner
- [Project Structure](architecture/project-structure.md)
- [Naming Conventions](conventions/naming-conventions.md)
- [Code Style](conventions/code-style.md)
- [Comments & Docstrings](conventions/comments-docstrings.md)

#### Intermediate
- [MVP Pattern](architecture/mvp-pattern.md)
- [Dependency Injection](architecture/dependency-injection.md)
- [Async/Await Pattern](best-practices/async-await.md)
- [Data Binding](ui-ux/data-binding.md)
- [Unit Testing](testing/unit-testing.md)

#### Advanced
- [MVVM Pattern](architecture/mvvm-pattern.md)
- [Thread Safety](best-practices/thread-safety.md)
- [Performance Optimization](best-practices/performance.md)
- [Nullable Reference Types](advanced/nullable-reference-types.md)
- [Performance Profiling](advanced/performance-profiling.md)

### By Task

#### Starting a New Project
1. [Project Structure](architecture/project-structure.md)
2. [Dependency Injection](architecture/dependency-injection.md)
3. [MVP Pattern](architecture/mvp-pattern.md) or [MVVM Pattern](architecture/mvvm-pattern.md)
4. [Configuration Management](best-practices/configuration.md)

#### Building UI
1. [Responsive Design](ui-ux/responsive-design.md)
2. [Data Binding](ui-ux/data-binding.md)
3. [Input Validation](ui-ux/input-validation.md)
4. [Keyboard Navigation](ui-ux/keyboard-navigation.md)

#### Working with Data
1. [Entity Framework Core](data-access/entity-framework.md)
2. [Repository Pattern](data-access/repository-pattern.md)
3. [Unit of Work Pattern](data-access/unit-of-work-pattern.md) ⭐
4. [Async/Await Pattern](best-practices/async-await.md)

#### Ensuring Quality
1. [Testing Overview](testing/testing-overview.md)
2. [Unit Testing](testing/unit-testing.md)
3. [Integration Testing](testing/integration-testing.md)
4. [Test Coverage](testing/test-coverage.md)

#### Optimizing Performance
1. [Performance Optimization](best-practices/performance.md)
2. [Memory Management](best-practices/memory-management.md)
3. [Performance Profiling](advanced/performance-profiling.md)
4. [DataGridView Best Practices](ui-ux/datagridview-practices.md)

#### Deploying
1. [Packaging & Deployment](deployment/packaging.md)
2. [Configuration Management](best-practices/configuration.md)
3. [Security Best Practices](best-practices/security.md)

---

## 🎯 Quick Reference

### Most Important Documents
1. [MVP Pattern](architecture/mvp-pattern.md) - Core architecture
2. [Naming Conventions](conventions/naming-conventions.md) - Consistent naming
3. [Async/Await Pattern](best-practices/async-await.md) - Non-blocking UI
4. [Resource Management](best-practices/resource-management.md) - Prevent leaks
5. [Unit Testing](testing/unit-testing.md) - Quality assurance

### Common Issues
- **Frozen UI?** → [Async/Await Pattern](best-practices/async-await.md)
- **Memory leaks?** → [Resource Management](best-practices/resource-management.md)
- **Cross-thread exception?** → [Thread Safety](best-practices/thread-safety.md)
- **Slow DataGridView?** → [DataGridView Best Practices](ui-ux/datagridview-practices.md)
- **How to test Forms?** → [UI Testing](testing/ui-testing.md)

---

## 🔧 Additional Resources

### Code Templates
Located in `/templates/` folder:
- `form-template.cs` - Standard Form with MVP
- `service-template.cs` - Service layer template
- `repository-template.cs` - Repository pattern
- `test-template.cs` - Unit test template

### Slash Commands
Located in `/.claude/commands/` folder (for Claude Code):
- `/create-form` - Create new Form with MVP pattern
- `/review-code` - Review code against standards
- `/add-test` - Add unit test for a class
- `/check-standards` - Verify standards compliance

---

## 📝 Contributing to Documentation

When adding or updating documentation:

1. **Keep files focused** - One topic per file (200-500 lines)
2. **Include examples** - Code samples with explanations
3. **Cross-reference** - Link to related topics
4. **Use structure** - Follow the template format
5. **Test code** - Ensure examples compile and run

---

**Need Help?**
- Start with [CLAUDE.md](../CLAUDE.md) for quick reference
- Use this overview to navigate to specific topics
- Check [examples](examples/) for working code
- Review [templates](../templates/) for starting points

---

**Last Updated**: 2025-11-07
**Version**: 2.0
