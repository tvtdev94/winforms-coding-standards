# C# WinForms Coding Standards - Claude Code Guide

> **Project**: C# WinForms Coding Standards and Best Practices Documentation
> **Purpose**: Guidelines for building maintainable, scalable WinForms applications

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
- **Separation**: UI → Presenter/ViewModel → Service → Repository → Database
- 📖 [MVP Pattern](docs/architecture/mvp-pattern.md) | [MVVM Pattern](docs/architecture/mvvm-pattern.md)

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

## 🤖 AI Assistant Rules (IMPORTANT!)

When writing code, **ALWAYS follow these rules**:

### ✅ DO:
1. **Separate concerns**: UI logic in Forms, business logic in Services
2. **Use async/await**: For all I/O operations (DB, file, network)
3. **Dispose resources**: Use `using` statements for IDisposable
4. **Validate input**: Always validate user input before processing
5. **Handle errors**: Use try-catch with proper logging
6. **Add XML comments**: For all public APIs
7. **Follow MVP/MVVM**: Don't mix UI and business logic
8. **Use DI**: Constructor injection for dependencies
9. **Write tests**: Unit tests for Services, integration tests for Repositories
10. **Thread-safe UI**: Use `Invoke`/`BeginInvoke` for cross-thread UI updates

### ❌ DON'T:
1. ❌ Put business logic in Forms
2. ❌ Use synchronous I/O (use async instead)
3. ❌ Leave resources undisposed (memory leaks)
4. ❌ Ignore exceptions silently
5. ❌ Use magic numbers/strings (use constants)
6. ❌ Create UI controls from background threads
7. ❌ Hardcode connection strings (use configuration)
8. ❌ Skip input validation
9. ❌ Write code without tests
10. ❌ Use Hungarian notation excessively

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

Before committing code, verify:

- [ ] **Code compiles** without warnings
- [ ] **All tests pass** (`dotnet test`)
- [ ] **No business logic in Forms** - moved to Services
- [ ] **Resources properly disposed** - using statements
- [ ] **Async/await used** for I/O operations
- [ ] **Input validated** with ErrorProvider or validation logic
- [ ] **Errors handled** with try-catch and logging
- [ ] **XML comments added** for public APIs
- [ ] **No magic numbers** - constants defined
- [ ] **Thread-safe UI updates** - Invoke/BeginInvoke used
- [ ] **Tests cover new code** - adequate test coverage
- [ ] **Code follows naming conventions**

---

## 🔗 Quick Links

- **[Full Overview](docs/00-overview.md)** - Complete documentation index
- **[MVP Pattern Guide](docs/architecture/mvp-pattern.md)** - Recommended architecture
- **[Testing Guide](docs/testing/testing-overview.md)** - How to test WinForms apps
- **[Code Examples](docs/examples/)** - Working code samples

---

## 📞 Need Help?

1. Check **[docs/00-overview.md](docs/00-overview.md)** for full documentation
2. Search for specific topic in `/docs/` folders
3. Review **[examples](docs/examples/)** for working code
4. Use slash commands (type `/` in Claude Code) for common tasks

---

## 🎓 Learning Path

**For new developers**:
1. Read [Project Structure](docs/architecture/project-structure.md)
2. Understand [MVP Pattern](docs/architecture/mvp-pattern.md)
3. Review [Naming Conventions](docs/conventions/naming-conventions.md)
4. Study [Code Examples](docs/examples/)
5. Practice with templates from `/templates/`

**For AI assistants**:
1. Load this file first (automatic)
2. Reference specific docs as needed for deep dives
3. Follow checklist before suggesting commits
4. Use templates for consistent code generation
5. Always validate against standards before responding

---

**Last Updated**: 2025-11-07
**Version**: 2.0 (Modular structure)
