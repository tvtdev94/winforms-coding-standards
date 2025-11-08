# Project Initialization Scripts

Automated scripts to create new WinForms projects with all best practices pre-configured.

## 🎯 Purpose

**Manually setting up a WinForms project**: 30-60 minutes
**Using init script**: 2 minutes ⚡

**What Gets Automated**:
- ✅ Solution and project creation
- ✅ Folder structure (MVP pattern)
- ✅ NuGet packages (EF Core, Serilog, DI)
- ✅ DI container setup in Program.cs
- ✅ appsettings.json configuration
- ✅ Test projects (unit + integration)
- ✅ .editorconfig and .gitignore
- ✅ Git initialization
- ✅ Git hooks installation

---

## 🚀 Quick Start

### Windows (PowerShell) ⭐

Simply run the interactive script - it guides you through setup step-by-step:

```powershell
.\scripts\init-project.ps1
```

**The script will ask you 7 questions**:
1. **Project name?** → e.g., CustomerManagement
2. **Framework?** → .NET 8.0 / 6.0 / Framework 4.8
3. **Database?** → SQLite / SQL Server / PostgreSQL / MySQL / None
4. **Pattern?** → MVP / MVVM / Simple
5. **Include tests?** → Y/n
6. **Include example code?** → y/N
7. **Integrate standards?** → Y/n

**Then shows confirmation** with all your choices before creating the project!

### Linux/Mac (Bash)

Bash version coming soon - use PowerShell on macOS for now (`brew install powershell`)

---

## 🗄️ Database Support

The interactive script supports multiple databases out of the box:

| Database | Use Case | NuGet Package |
|----------|----------|---------------|
| **SQLite** ⭐ | Development, testing, demos | Microsoft.EntityFrameworkCore.Sqlite |
| **SQL Server** | Windows enterprise, production | Microsoft.EntityFrameworkCore.SqlServer |
| **PostgreSQL** | Cross-platform, open source | Npgsql.EntityFrameworkCore.PostgreSQL |
| **MySQL** | Cross-platform, popular | Pomelo.EntityFrameworkCore.MySql |
| **None** | No database needed | - |

**The script automatically**:
- ✅ Installs correct NuGet packages
- ✅ Generates appropriate connection string
- ✅ Adds DbContext registration to Program.cs
- ✅ Creates Data folder for EF Core code

---

## 📁 Generated Project Structure

```
MyWinFormsApp/
├── MyWinFormsApp.sln
├── .editorconfig               # Code style rules
├── .gitignore                  # Git ignore patterns
├── appsettings.json            # Configuration
│
├── .githooks/                  # Pre-commit hooks
│   ├── pre-commit
│   └── install.sh
│
├── MyWinFormsApp/              # Main project
│   ├── MyWinFormsApp.csproj
│   ├── Program.cs              # DI container setup
│   ├── appsettings.json
│   │
│   ├── Models/                 # Domain entities
│   ├── Services/               # Business logic
│   ├── Repositories/           # Data access
│   ├── Forms/                  # UI forms
│   ├── Views/                  # MVP view interfaces
│   ├── Presenters/             # MVP presenters
│   ├── Data/                   # EF Core DbContext
│   ├── Utils/                  # Helper classes
│   └── Resources/              # Icons, strings
│
├── MyWinFormsApp.Tests/        # Unit tests
│   └── MyWinFormsApp.Tests.csproj
│
└── MyWinFormsApp.IntegrationTests/  # Integration tests
    └── MyWinFormsApp.IntegrationTests.csproj
```

---

## 📦 Pre-installed NuGet Packages

### Main Project

| Package | Purpose |
|---------|---------|
| `Microsoft.Extensions.DependencyInjection` | DI container |
| `Microsoft.Extensions.Configuration.Json` | JSON configuration |
| `Microsoft.Extensions.Logging` | Logging abstraction |
| `Serilog.Extensions.Logging` | Serilog integration |
| `Serilog.Sinks.File` | File logging |
| `Microsoft.EntityFrameworkCore.Sqlite` | EF Core + SQLite |
| `Microsoft.EntityFrameworkCore.Design` | EF Core tools |

### Test Projects

| Package | Purpose |
|---------|---------|
| `xUnit` | Testing framework |
| `Moq` | Mocking library |
| `FluentAssertions` | Assertion library |
| `Microsoft.EntityFrameworkCore.Sqlite` | In-memory DB for tests |

---

## 🎬 Example Session

```powershell
PS> .\scripts\init-project.ps1

========================================
  WinForms Project Initialization
========================================

1. Project Name
   Enter project name (e.g., CustomerManagement): CustomerApp

2. Target Framework
   [1] .NET 8.0 (recommended)
   [2] .NET 6.0
   [3] .NET Framework 4.8
   Select framework (1-3): 1
   Selected: net8.0

3. Database Provider
   [1] SQLite (recommended for dev/testing)
   [2] SQL Server
   [3] PostgreSQL
   [4] MySQL
   [5] None (no database)
   Select database (1-5): 2
   Selected: SQLServer

4. Architecture Pattern
   [1] MVP (Model-View-Presenter) - recommended
   [2] MVVM (Model-View-ViewModel) - .NET 8+ only
   [3] Simple (no pattern)
   Select pattern (1-3): 1
   Selected: MVP

5. Unit & Integration Tests
   Include test projects? (Y/n): Y
   Tests: Yes

6. Example Code
   Include example code? (y/N): N
   Example code: No

7. Coding Standards Integration
   Integrate coding standards? (Y/n): Y
   Standards: Yes

========================================
Configuration Summary
========================================
Project Name    : CustomerApp
Framework       : net8.0
Database        : SQLServer
Pattern         : MVP
Tests           : Yes
Example Code    : No
Standards       : Yes

Proceed with these settings? (Y/n): Y

[1] Creating solution...
  [OK] Solution created

[2] Creating WinForms project...
  [OK] WinForms project created

[3] Creating folder structure...
  [OK] Created Models/
  [OK] Created Services/
  [OK] Created Repositories/
  [OK] Created Forms/
  [OK] Created Views/
  [OK] Created Presenters/
  [OK] Created Data/
  [OK] Created Utils/
  [OK] Created Resources/

[4] Adding NuGet packages...
  Adding Microsoft.Extensions.DependencyInjection... [OK]
  Adding Microsoft.EntityFrameworkCore.SqlServer... [OK]
  Restoring packages... [OK]

[5] Creating appsettings.json...
  [OK] appsettings.json created
  [INFO] Database: SQLServer
  [INFO] Connection string: Server=localhost;Database=CustomerApp;...

[6] Creating Program.cs with DI...
  [OK] Program.cs created with DI

[7] Creating test projects...
  [OK] Unit test project created
  [OK] Integration test project created

[8] Setting up configuration files...
  [OK] .editorconfig copied
  [OK] .gitignore copied

[9] Creating VS Code tasks and launch config...
  [OK] .vscode/tasks.json created
  [OK] .vscode/launch.json created

[10] Initializing git repository...
  [OK] Git repository initialized

[11] Integrating coding standards...
  [OK] Standards added as submodule
  [OK] Standards integration complete

========================================
Project created successfully!
========================================

[Location] C:\Projects\CustomerApp

[Configuration]
  Project Name : CustomerApp
  Framework    : net8.0
  Database     : SQLServer
  Pattern      : MVP
  Tests        : Yes
  Standards    : Yes

[Next steps]
  1. cd CustomerApp
  2. Open CustomerApp.sln in Visual Studio
  3. Update connection string in appsettings.json
  4. Create your DbContext and models
  5. Run: dotnet ef migrations add InitialCreate
  6. Run: dotnet ef database update
  7. Start coding with MVP pattern!

[Useful commands]
  dotnet build              # Build project
  dotnet run --project CustomerApp  # Run application
  dotnet test               # Run all tests
  dotnet ef migrations add <name>   # Create migration
  dotnet ef database update         # Apply migrations

[Coding Standards]
  .standards/USAGE_GUIDE.md     # Practical examples
  .standards/CLAUDE.md          # AI assistant guide
  .standards/docs/              # Full documentation
  Type / in Claude Code         # See slash commands

Happy coding! 🚀
```

---

## 🎓 What to Do After Project Creation

### 1. Open in IDE

**Visual Studio**:
```powershell
start CustomerApp.sln
```

**VS Code**:
```bash
code CustomerApp
```

**JetBrains Rider**:
```bash
rider CustomerApp.sln
```

### 2. Create Your First Form

Use code snippets:
```csharp
// In CustomerForm.cs
mvpform [Tab]
// Enter "Customer" → Full MVP form generated!
```

Or use slash command (if using Claude Code):
```
/create-form Customer
```

### 3. Add Your Database Models

```csharp
// In Models/Customer.cs
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
```

### 4. Create DbContext

```csharp
// In Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
}
```

### 5. Register in DI (Program.cs)

```csharp
// In Program.cs ConfigureServices method
services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

services.AddScoped<ICustomerRepository, CustomerRepository>();
services.AddScoped<ICustomerService, CustomerService>();
services.AddTransient<CustomerForm>();
```

### 6. Run Migrations

```bash
dotnet ef migrations add InitialCreate --project CustomerApp
dotnet ef database update --project CustomerApp
```

### 7. Build and Run

```bash
dotnet build
dotnet run --project CustomerApp
```

---

## 🔧 Customization

### Modify Script for Your Team

Edit `init-project.ps1` to:

**1. Add More NuGet Packages**:
```powershell
$packages = @(
    # ... existing packages ...
    "AutoMapper.Extensions.Microsoft.DependencyInjection",
    "YourCompany.SharedLibrary"
)
```

**2. Add Custom Folders**:
```powershell
$folders = @(
    # ... existing folders ...
    "ViewModels",
    "Helpers",
    "Constants"
)
```

**3. Create Boilerplate Files**:
```powershell
# Add after Step 6
Write-Host "Creating base classes..." -ForegroundColor Cyan

$baseFormContent = @"
public abstract class BaseForm : Form
{
    protected ILogger Logger { get; }

    protected BaseForm(ILogger logger)
    {
        Logger = logger;
    }
}
"@

$baseFormContent | Out-File "$ProjectName/Forms/BaseForm.cs" -Encoding UTF8
```

---

## 💡 Tips & Tricks

### 1. Quick Setup for Different Project Types

**Simple CRUD app**:
- Choose SQLite for database
- Choose MVP pattern
- Include tests: Yes

**Enterprise app**:
- Choose SQL Server for database
- Choose MVP pattern
- Include tests: Yes
- Integrate standards: Yes

**Prototype/Demo**:
- Choose None for database
- Choose Simple pattern
- Include tests: No

### 2. After Creation

Always verify the setup works:
```powershell
cd YourProject
dotnet build
dotnet test
```

### 3. Team Collaboration

The standards submodule ensures everyone uses the same coding standards:
```powershell
# Update standards in existing project
cd .standards
git pull
cd ..
```

---

## 🆘 Troubleshooting

### Script Execution Error (PowerShell)

**Error**: `cannot be loaded because running scripts is disabled`

**Solution**:
```powershell
# Allow script execution (one-time)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# Or run with bypass
powershell -ExecutionPolicy Bypass -File .\scripts\init-project.ps1
```

### "dotnet: command not found"

**Solution**:
- Install .NET SDK from https://dotnet.microsoft.com/download
- Restart terminal after installation

### Project Already Exists

**Error**: `Directory 'MyApp' already exists!`

**Solutions**:
```powershell
# Option 1: Delete and recreate
Remove-Item -Recurse -Force MyApp
.\scripts\init-project.ps1 -ProjectName "MyApp"

# Option 2: Use different name
.\scripts\init-project.ps1 -ProjectName "MyApp_v2"
```

### Git Not Initialized

**Issue**: No .git folder created

**Solution**:
```bash
# Ensure git is installed
git --version

# If git is missing, install it
# Then re-run the script
```

---

## 📊 Time Savings

| Task | Manual | With Script | Saved |
|------|--------|-------------|-------|
| Create solution & project | 3 min | 10 sec | **94%** |
| Setup folder structure | 5 min | 10 sec | **97%** |
| Add NuGet packages | 10 min | 30 sec | **95%** |
| Configure DI in Program.cs | 15 min | 10 sec | **99%** |
| Create test projects | 10 min | 20 sec | **97%** |
| Copy config files | 5 min | 5 sec | **98%** |
| Git setup | 2 min | 10 sec | **92%** |
| **TOTAL** | **50 min** | **~2 min** | **96%** ⚡ |

**For 10 projects/year**: Save **8 hours** = **$600+ value** (at $75/hour)

---

## 📚 Related Tools

- **[Code Snippets](../snippets/)** - Speed up coding
- **[Git Hooks](../.githooks/)** - Enforce quality
- **[Templates](../templates/)** - Boilerplate code
- **[USAGE_GUIDE.md](../USAGE_GUIDE.md)** - Practical examples

---

**Last Updated**: 2025-11-08
**Version**: 2.0 - Interactive mode with multi-database support
