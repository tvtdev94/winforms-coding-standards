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

### Option 1: Interactive Mode (Recommended for Beginners) ⭐

**New!** Interactive prompts guide you through the setup:

```powershell
# Windows (PowerShell)
.\scripts\init-project-interactive.ps1

# Linux/Mac (Bash) - Coming soon
# ./scripts/init-project-interactive.sh
```

**You will be asked**:
1. Project name? (e.g., CustomerManagement)
2. Framework? (.NET 8.0 / 6.0 / Framework 4.8)
3. Database? (SQLite / SQL Server / PostgreSQL / MySQL / None)
4. Pattern? (MVP / MVVM / Simple)
5. Include tests? (Y/n)
6. Include example code? (y/N)
7. Integrate standards? (Y/n)

**Then confirms** your choices before creating the project!

---

### Option 2: Command-Line Mode (For Automation/Scripts)

Quick project creation with command-line parameters:

```powershell
# Windows (PowerShell) - Basic
.\scripts\init-project.ps1 -ProjectName "MyWinFormsApp"

# With options
.\scripts\init-project.ps1 -ProjectName "MyApp" -Framework "net8.0" -IncludeTests

# Advanced
.\scripts\init-project.ps1 `
    -ProjectName "CustomerManagement" `
    -Framework "net8.0" `
    -IncludeTests `
    -IncludeExampleCode
```

```bash
# Linux/Mac (Bash)
chmod +x scripts/init-project.sh
./scripts/init-project.sh MyWinFormsApp net8.0
```

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

## 📋 Parameters (Command-Line Mode)

### PowerShell Script

| Parameter | Required | Default | Description |
|-----------|----------|---------|-------------|
| `-ProjectName` | ✅ Yes | - | Name of the project |
| `-Framework` | ❌ No | `net8.0` | Target framework (net8.0, net6.0, net48) |
| `-IncludeTests` | ❌ No | `$true` | Include test projects |
| `-IncludeExampleCode` | ❌ No | `$false` | Include example MVP code |
| `-IntegrateStandards` | ❌ No | `$true` | Integrate coding standards as submodule |

### Examples

```powershell
# Basic project
.\scripts\init-project.ps1 -ProjectName "SimpleApp"

# .NET 6.0 project
.\scripts\init-project.ps1 -ProjectName "LegacyApp" -Framework "net6.0"

# Without tests
.\scripts\init-project.ps1 -ProjectName "QuickPrototype" -IncludeTests:$false

# With example code
.\scripts\init-project.ps1 -ProjectName "LearningProject" -IncludeExampleCode
```

**Note**: Command-line mode uses SQLite by default. For other databases, use interactive mode.

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
PS> .\scripts\init-project.ps1 -ProjectName "CustomerApp"

🚀 WinForms Project Initialization
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Project Name: CustomerApp
Framework: net8.0
Include Tests: True
Include Example Code: False

1️⃣  Creating solution...
  ✓ Solution created

2️⃣  Creating WinForms project...
  ✓ WinForms project created

3️⃣  Creating folder structure...
  ✓ Created Models/
  ✓ Created Services/
  ✓ Created Repositories/
  ✓ Created Forms/
  ✓ Created Views/
  ✓ Created Presenters/
  ✓ Created Data/
  ✓ Created Utils/
  ✓ Created Resources/

4️⃣  Adding NuGet packages...
  Adding Microsoft.Extensions.DependencyInjection... ✓
  Adding Microsoft.Extensions.Configuration.Json... ✓
  Adding Serilog.Extensions.Logging... ✓
  [... more packages ...]
  Restoring packages... ✓

5️⃣  Creating appsettings.json...
  ✓ appsettings.json created

6️⃣  Creating Program.cs with DI...
  ✓ Program.cs created with DI

7️⃣  Creating test projects...
  ✓ Unit test project created
  ✓ Integration test project created
  Restoring test packages... ✓

8️⃣  Copying configuration files...
  ✓ .editorconfig copied
  ✓ .gitignore copied

9️⃣  Initializing git repository...
  ✓ Git repository initialized

🔟 Installing git hooks...
  ✓ Git hooks installed

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
✅ Project created successfully!
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

📂 Location: C:\Projects\CustomerApp

📋 Next steps:
  1. cd CustomerApp
  2. Open CustomerApp.sln in Visual Studio
  3. Start coding with MVP pattern!

💡 Useful commands:
  dotnet build              # Build project
  dotnet run --project CustomerApp  # Run application
  dotnet test               # Run all tests

📚 Documentation:
  See USAGE_GUIDE.md for practical examples
  See docs/ folder for detailed guidelines
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

### 1. Create Multiple Projects Quickly

```powershell
# Create batch of projects
$projects = @("CustomerApp", "OrderApp", "InventoryApp")
foreach ($proj in $projects) {
    .\scripts\init-project.ps1 -ProjectName $proj
}
```

### 2. Use Templates for Different Project Types

```powershell
# Simple CRUD app
.\scripts\init-project.ps1 -ProjectName "SimpleCRUD" -IncludeExampleCode

# Complex enterprise app
.\scripts\init-project.ps1 -ProjectName "EnterpriseApp" -Framework "net8.0" -IncludeTests
```

### 3. Integrate with CI/CD

```yaml
# GitHub Actions example
- name: Create test project
  run: |
    ./scripts/init-project.ps1 -ProjectName "TestApp" -IncludeTests
    dotnet build TestApp
    dotnet test TestApp
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
powershell -ExecutionPolicy Bypass -File .\scripts\init-project.ps1 -ProjectName "MyApp"
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

**Last Updated**: 2025-11-07
**Version**: 1.0
