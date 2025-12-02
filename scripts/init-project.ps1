# WinForms Project Initialization Script (Interactive)
# Creates a new WinForms project following coding standards with interactive prompts

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  WinForms Project Initialization" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# ============================================================================
# Interactive Questions
# ============================================================================

# Question 1: Project Name
Write-Host "1. Project Name" -ForegroundColor Yellow
$ProjectName = Read-Host "   Enter project name (e.g., CustomerManagement)"
if ([string]::IsNullOrWhiteSpace($ProjectName)) {
    Write-Host "[ERROR] Project name cannot be empty!" -ForegroundColor Red
    exit 1
}

# Check if directory already exists
if (Test-Path $ProjectName) {
    Write-Host "[ERROR] Directory '$ProjectName' already exists!" -ForegroundColor Red
    Write-Host "   Please choose a different name or delete the existing directory." -ForegroundColor Yellow
    exit 1
}

Write-Host "   Project name: $ProjectName" -ForegroundColor Green
Write-Host ""

# Question 2: Framework
Write-Host "2. Target Framework" -ForegroundColor Yellow
Write-Host "   [1] .NET 8.0 (recommended)" -ForegroundColor Gray
Write-Host "   [2] .NET 6.0" -ForegroundColor Gray
Write-Host "   [3] .NET Framework 4.8" -ForegroundColor Gray
$frameworkChoice = Read-Host "   Select framework (1-3)"
$Framework = switch ($frameworkChoice) {
    "1" { "net8.0" }
    "2" { "net6.0" }
    "3" { "net48" }
    default { "net8.0" }
}
Write-Host "   Selected: $Framework" -ForegroundColor Green
Write-Host ""

# Question 3: Database
Write-Host "3. Database Provider" -ForegroundColor Yellow
Write-Host "   [1] SQLite (recommended for dev/testing)" -ForegroundColor Gray
Write-Host "   [2] SQL Server" -ForegroundColor Gray
Write-Host "   [3] PostgreSQL" -ForegroundColor Gray
Write-Host "   [4] MySQL" -ForegroundColor Gray
Write-Host "   [5] None (no database)" -ForegroundColor Gray
$dbChoice = Read-Host "   Select database (1-5)"
$Database = switch ($dbChoice) {
    "1" { "SQLite" }
    "2" { "SQLServer" }
    "3" { "PostgreSQL" }
    "4" { "MySQL" }
    "5" { "None" }
    default { "SQLite" }
}
Write-Host "   Selected: $Database" -ForegroundColor Green
Write-Host ""

# Question 4: UI Framework
Write-Host "4. UI Framework" -ForegroundColor Yellow
Write-Host "   [1] Standard WinForms (default controls)" -ForegroundColor Gray
Write-Host "   [2] DevExpress (commercial, advanced features)" -ForegroundColor Gray
Write-Host "   [3] ReaLTaiizor (free, modern themes)" -ForegroundColor Gray
$uiChoice = Read-Host "   Select UI framework (1-3)"
$UIFramework = switch ($uiChoice) {
    "1" { "Standard" }
    "2" { "DevExpress" }
    "3" { "ReaLTaiizor" }
    default { "Standard" }
}
Write-Host "   Selected: $UIFramework" -ForegroundColor Green
Write-Host ""

# Question 5: Architecture Pattern
Write-Host "5. Architecture Pattern" -ForegroundColor Yellow
Write-Host ""

# Smart recommendation based on previous choices
$recommendedPattern = "MVP"  # Default safe choice
$patternRecommendation = ""

if ($Database -eq "None" -and ($Framework -eq "net48" -or $Framework -eq "net6.0")) {
    $recommendedPattern = "Simple"
    $patternRecommendation = "Recommended: Simple (no database, lightweight app)"
}
elseif ($Framework -eq "net8.0" -and $Database -ne "None") {
    $recommendedPattern = "MVP"
    $patternRecommendation = "Recommended: MVP (best balance of testability and simplicity)"
}
elseif ($Framework -eq "net48") {
    $recommendedPattern = "MVP"
    $patternRecommendation = "Recommended: MVP (.NET Framework works best with MVP)"
}
else {
    $recommendedPattern = "MVP"
    $patternRecommendation = "Recommended: MVP (works well in most scenarios)"
}

# Display options with descriptions
Write-Host "   [1] MVP (Model-View-Presenter)" -ForegroundColor $(if ($recommendedPattern -eq "MVP") { "Green" } else { "Gray" })
Write-Host "       - Best for: Most WinForms apps" -ForegroundColor DarkGray
Write-Host "       - Easy to test, clear separation" -ForegroundColor DarkGray
Write-Host "       - Works with all .NET versions" -ForegroundColor DarkGray
Write-Host ""

# MVVM availability based on framework
$mvvmAvailable = $Framework -eq "net8.0" -or $Framework -eq "net6.0"
$mvvmColor = if ($mvvmAvailable) {
    if ($recommendedPattern -eq "MVVM") { "Green" } else { "Gray" }
} else {
    "DarkGray"
}

Write-Host "   [2] MVVM (Model-View-ViewModel)" -ForegroundColor $mvvmColor
if ($mvvmAvailable) {
    Write-Host "       - Best for: Complex UI with data binding" -ForegroundColor DarkGray
    Write-Host "       - Two-way binding, INotifyPropertyChanged" -ForegroundColor DarkGray
    Write-Host "       - More complex than MVP" -ForegroundColor DarkGray
} else {
    Write-Host "       - Not recommended for $Framework" -ForegroundColor DarkGray
    Write-Host "       - Use .NET 6+ for better MVVM support" -ForegroundColor DarkGray
}
Write-Host ""

Write-Host "   [3] Simple (no pattern)" -ForegroundColor $(if ($recommendedPattern -eq "Simple") { "Green" } else { "Gray" })
Write-Host "       - Best for: Quick prototypes, demos" -ForegroundColor DarkGray
Write-Host "       - All code in Forms (harder to test)" -ForegroundColor DarkGray
Write-Host "       - Not recommended for production" -ForegroundColor DarkGray
Write-Host ""

# Show recommendation
Write-Host "   $patternRecommendation" -ForegroundColor Cyan
Write-Host ""

$patternChoice = Read-Host "   Select pattern (1-3)"
$Pattern = switch ($patternChoice) {
    "1" { "MVP" }
    "2" {
        if (-not $mvvmAvailable) {
            Write-Host "   [WARN] MVVM not recommended for $Framework, using MVP instead" -ForegroundColor Yellow
            "MVP"
        } else {
            "MVVM"
        }
    }
    "3" { "Simple" }
    default { $recommendedPattern }  # Use smart default
}
Write-Host "   Selected: $Pattern" -ForegroundColor Green
Write-Host ""

# Question 6: Project Structure
Write-Host "6. Project Structure" -ForegroundColor Yellow
Write-Host ""

# Smart recommendation based on previous choices
$recommendedStructure = "Single"  # Default for most apps
$structureRecommendation = ""

# Recommend Multi-Project for large apps or when reuse is likely
if ($Database -ne "None" -and $Pattern -eq "MVP") {
    $recommendedStructure = "Single"
    $structureRecommendation = "Recommended: Single (best for most WinForms apps)"
}

Write-Host "   [1] Single Project (Monolith)" -ForegroundColor $(if ($recommendedStructure -eq "Single") { "Green" } else { "Gray" })
Write-Host "       - All code in one project (.UI, Services, Repos in folders)" -ForegroundColor DarkGray
Write-Host "       - Simple, fast to build, easy to manage" -ForegroundColor DarkGray
Write-Host "       - Best for: Small/medium apps, 1-3 developers" -ForegroundColor DarkGray
Write-Host ""

Write-Host "   [2] Multi-Project (Separate Assemblies)" -ForegroundColor $(if ($recommendedStructure -eq "Multi") { "Green" } else { "Gray" })
Write-Host "       - Separate projects: UI, Core, Business, Data" -ForegroundColor DarkGray
Write-Host "       - Strict separation, compiler-enforced architecture" -ForegroundColor DarkGray
Write-Host "       - Best for: Large apps (20+ forms), 3+ developers, code reuse" -ForegroundColor DarkGray
Write-Host ""

Write-Host "   $structureRecommendation" -ForegroundColor Cyan
Write-Host ""

$structureChoice = Read-Host "   Select structure (1-2)"
$ProjectStructure = switch ($structureChoice) {
    "1" { "Single" }
    "2" { "Multi" }
    default { $recommendedStructure }
}
Write-Host "   Selected: $ProjectStructure" -ForegroundColor Green
Write-Host ""

# Question 7: Include Tests
Write-Host "7. Unit & Integration Tests" -ForegroundColor Yellow
$includeTestsInput = Read-Host "   Include test projects? (Y/n)"
$IncludeTests = $includeTestsInput -ne "n" -and $includeTestsInput -ne "N"
Write-Host "   Tests: $(if ($IncludeTests) { 'Yes' } else { 'No' })" -ForegroundColor Green
Write-Host ""

# Question 8: Include Example Code
Write-Host "8. Example Code" -ForegroundColor Yellow
$includeExampleInput = Read-Host "   Include example code? (y/N)"
$IncludeExampleCode = $includeExampleInput -eq "y" -or $includeExampleInput -eq "Y"
Write-Host "   Example code: $(if ($IncludeExampleCode) { 'Yes' } else { 'No' })" -ForegroundColor Green
Write-Host ""

# Question 9: Integrate Standards
Write-Host "9. Coding Standards Integration" -ForegroundColor Yellow
$integrateStandardsInput = Read-Host "   Integrate coding standards? (Y/n)"
$IntegrateStandards = $integrateStandardsInput -ne "n" -and $integrateStandardsInput -ne "N"
Write-Host "   Standards: $(if ($IntegrateStandards) { 'Yes' } else { 'No' })" -ForegroundColor Green
Write-Host ""

# ============================================================================
# Confirmation
# ============================================================================
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Configuration Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Project Name    : $ProjectName"
Write-Host "Framework       : $Framework"
Write-Host "Database        : $Database"
Write-Host "UI Framework    : $UIFramework"
Write-Host "Pattern         : $Pattern"
Write-Host "Structure       : $ProjectStructure"
Write-Host "Tests           : $(if ($IncludeTests) { 'Yes' } else { 'No' })"
Write-Host "Example Code    : $(if ($IncludeExampleCode) { 'Yes' } else { 'No' })"
Write-Host "Standards       : $(if ($IntegrateStandards) { 'Yes' } else { 'No' })"
Write-Host ""
$confirm = Read-Host "Proceed with these settings? (Y/n)"
if ($confirm -eq "n" -or $confirm -eq "N") {
    Write-Host "[CANCELLED] Project initialization cancelled" -ForegroundColor Yellow
    exit 0
}
Write-Host ""

# ============================================================================
# Step 1: Create Solution
# ============================================================================
Write-Host "[1] Creating solution..." -ForegroundColor Cyan

dotnet new sln -n $ProjectName -o $ProjectName
Set-Location $ProjectName

Write-Host "  [OK] Solution created" -ForegroundColor Green

# ============================================================================
# Step 2: Create Projects
# ============================================================================
Write-Host ""
if ($ProjectStructure -eq "Single") {
    Write-Host "[2] Creating WinForms project (Single Project)..." -ForegroundColor Cyan

    dotnet new winforms -n $ProjectName -f $Framework
    dotnet sln add "$ProjectName/$ProjectName.csproj"

    Write-Host "  [OK] WinForms project created" -ForegroundColor Green
}
else {
    Write-Host "[2] Creating multi-project structure..." -ForegroundColor Cyan

    # Create UI project (WinForms)
    dotnet new winforms -n "$ProjectName.UI" -f $Framework
    dotnet sln add "$ProjectName.UI/$ProjectName.UI.csproj"
    Write-Host "  [OK] $ProjectName.UI (Presentation Layer) created" -ForegroundColor Green

    # Create Domain project (Models, Interfaces)
    dotnet new classlib -n "$ProjectName.Domain" -f $Framework
    dotnet sln add "$ProjectName.Domain/$ProjectName.Domain.csproj"
    Write-Host "  [OK] $ProjectName.Domain (Domain Layer) created" -ForegroundColor Green

    # Create Application project (Services, Use Cases)
    dotnet new classlib -n "$ProjectName.Application" -f $Framework
    dotnet sln add "$ProjectName.Application/$ProjectName.Application.csproj"
    Write-Host "  [OK] $ProjectName.Application (Application Layer) created" -ForegroundColor Green

    # Create Infrastructure project (Data Access, External Services) - only if database is selected
    if ($Database -ne "None") {
        dotnet new classlib -n "$ProjectName.Infrastructure" -f $Framework
        dotnet sln add "$ProjectName.Infrastructure/$ProjectName.Infrastructure.csproj"
        Write-Host "  [OK] $ProjectName.Infrastructure (Infrastructure Layer) created" -ForegroundColor Green
    }

    # Add project references
    Write-Host "  Adding project references..." -NoNewline
    if ($Database -ne "None") {
        dotnet add "$ProjectName.UI" reference "$ProjectName.Domain" "$ProjectName.Application" "$ProjectName.Infrastructure" | Out-Null
        dotnet add "$ProjectName.Application" reference "$ProjectName.Domain" | Out-Null
        dotnet add "$ProjectName.Infrastructure" reference "$ProjectName.Domain" | Out-Null
    }
    else {
        dotnet add "$ProjectName.UI" reference "$ProjectName.Domain" "$ProjectName.Application" | Out-Null
        dotnet add "$ProjectName.Application" reference "$ProjectName.Domain" | Out-Null
    }
    Write-Host " [OK]" -ForegroundColor Green
}

# ============================================================================
# Step 3: Create Folder Structure
# ============================================================================
Write-Host ""
Write-Host "[3] Creating folder structure..." -ForegroundColor Cyan

if ($ProjectStructure -eq "Single") {
    # Single Project: Create folders inside main project
    $projectPath = $ProjectName

    # Domain folder (Models, Interfaces, Enums, Exceptions)
    New-Item -ItemType Directory -Path "$projectPath/Domain/Models" -Force | Out-Null
    New-Item -ItemType Directory -Path "$projectPath/Domain/Interfaces" -Force | Out-Null
    New-Item -ItemType Directory -Path "$projectPath/Domain/Enums" -Force | Out-Null
    New-Item -ItemType Directory -Path "$projectPath/Domain/Exceptions" -Force | Out-Null
    Write-Host "  [OK] Created Domain/ (Models, Interfaces, Enums, Exceptions)" -ForegroundColor Green

    # Application folder (Services, Validators)
    New-Item -ItemType Directory -Path "$projectPath/Application/Services" -Force | Out-Null
    New-Item -ItemType Directory -Path "$projectPath/Application/Validators" -Force | Out-Null
    Write-Host "  [OK] Created Application/ (Services, Validators)" -ForegroundColor Green

    # Infrastructure folder (only if database selected)
    if ($Database -ne "None") {
        New-Item -ItemType Directory -Path "$projectPath/Infrastructure/Persistence/Repositories" -Force | Out-Null
        New-Item -ItemType Directory -Path "$projectPath/Infrastructure/Persistence/Context" -Force | Out-Null
        New-Item -ItemType Directory -Path "$projectPath/Infrastructure/Persistence/Configurations" -Force | Out-Null
        New-Item -ItemType Directory -Path "$projectPath/Infrastructure/Persistence/UnitOfWork" -Force | Out-Null
        Write-Host "  [OK] Created Infrastructure/Persistence/ (Repositories, Context, Configurations, UnitOfWork)" -ForegroundColor Green
    }

    # UI folder (Forms, Presenters/Views, Factories)
    New-Item -ItemType Directory -Path "$projectPath/UI/Forms" -Force | Out-Null
    New-Item -ItemType Directory -Path "$projectPath/UI/Factories" -Force | Out-Null

    # Add pattern-specific folders
    if ($Pattern -eq "MVP") {
        New-Item -ItemType Directory -Path "$projectPath/UI/Views" -Force | Out-Null
        New-Item -ItemType Directory -Path "$projectPath/UI/Presenters" -Force | Out-Null
        Write-Host "  [OK] Created UI/ (Forms, Views, Presenters, Factories)" -ForegroundColor Green
    }
    elseif ($Pattern -eq "MVVM") {
        New-Item -ItemType Directory -Path "$projectPath/UI/ViewModels" -Force | Out-Null
        Write-Host "  [OK] Created UI/ (Forms, ViewModels, Factories)" -ForegroundColor Green
    }
    else {
        Write-Host "  [OK] Created UI/ (Forms, Factories)" -ForegroundColor Green
    }

    # Additional utility folders
    $folders = @(
        @{Name="Utils"; Template="// Place your utility classes and extensions here`n// Example: StringExtensions.cs, DateHelper.cs`n"},
        @{Name="Resources"; Template="// Place your resources here`n// Example: Icons/, Images/, Strings.resx`n"}
    )

    foreach ($folder in $folders) {
        New-Item -ItemType Directory -Path "$projectPath/$($folder.Name)" -Force | Out-Null

        # Add README.md to help Rider/VS show the folder
        if ($folder.Template) {
            $readmeContent = "# $($folder.Name)`n`n$($folder.Template)"
            $readmeContent | Out-File -FilePath "$projectPath/$($folder.Name)/README.md" -Encoding UTF8 -Force
        }

        Write-Host "  [OK] Created $($folder.Name)/" -ForegroundColor Green
    }

    # Move Form1 to UI/Forms folder and rename to MainForm
    Move-Item -Path "$projectPath/Form1.cs" -Destination "$projectPath/UI/Forms/MainForm.cs" -Force
    Move-Item -Path "$projectPath/Form1.Designer.cs" -Destination "$projectPath/UI/Forms/MainForm.Designer.cs" -Force
    if (Test-Path "$projectPath/Form1.resx") {
        Move-Item -Path "$projectPath/Form1.resx" -Destination "$projectPath/UI/Forms/MainForm.resx" -Force
    }

    # Update MainForm.cs namespace
    $mainFormContent = Get-Content "$projectPath/UI/Forms/MainForm.cs" -Raw
    $mainFormContent = $mainFormContent -replace "namespace $ProjectName", "namespace $ProjectName.UI.Forms"
    $mainFormContent = $mainFormContent -replace "partial class Form1", "partial class MainForm"
    $mainFormContent = $mainFormContent -replace "public Form1\(\)", "public MainForm()"
    $mainFormContent | Out-File -FilePath "$projectPath/UI/Forms/MainForm.cs" -Encoding UTF8 -Force

    # Update MainForm.Designer.cs
    $designerContent = Get-Content "$projectPath/UI/Forms/MainForm.Designer.cs" -Raw
    $designerContent = $designerContent -replace "namespace $ProjectName", "namespace $ProjectName.UI.Forms"
    $designerContent = $designerContent -replace "partial class Form1", "partial class MainForm"
    $designerContent = $designerContent -replace "Form1", "MainForm"
    $designerContent | Out-File -FilePath "$projectPath/UI/Forms/MainForm.Designer.cs" -Encoding UTF8 -Force

    Write-Host "  [OK] Moved and renamed Form1 to MainForm in UI/Forms/" -ForegroundColor Green
}
else {
    # Multi-Project: Create folders in appropriate projects

    # Domain project folders
    New-Item -ItemType Directory -Path "$ProjectName.Domain/Models" -Force | Out-Null
    New-Item -ItemType Directory -Path "$ProjectName.Domain/Interfaces" -Force | Out-Null
    New-Item -ItemType Directory -Path "$ProjectName.Domain/Enums" -Force | Out-Null
    New-Item -ItemType Directory -Path "$ProjectName.Domain/Exceptions" -Force | Out-Null
    Write-Host "  [OK] Created Domain project folders (Models, Interfaces, Enums, Exceptions)" -ForegroundColor Green

    # Application project folders
    New-Item -ItemType Directory -Path "$ProjectName.Application/Services" -Force | Out-Null
    New-Item -ItemType Directory -Path "$ProjectName.Application/Validators" -Force | Out-Null
    Write-Host "  [OK] Created Application project folders (Services, Validators)" -ForegroundColor Green

    # Infrastructure project folders (only if database is selected)
    if ($Database -ne "None") {
        New-Item -ItemType Directory -Path "$ProjectName.Infrastructure/Persistence" -Force | Out-Null
        New-Item -ItemType Directory -Path "$ProjectName.Infrastructure/Persistence/Repositories" -Force | Out-Null
        New-Item -ItemType Directory -Path "$ProjectName.Infrastructure/Persistence/Context" -Force | Out-Null
        New-Item -ItemType Directory -Path "$ProjectName.Infrastructure/Persistence/Configurations" -Force | Out-Null
        New-Item -ItemType Directory -Path "$ProjectName.Infrastructure/Persistence/UnitOfWork" -Force | Out-Null
        Write-Host "  [OK] Created Infrastructure project folders (Persistence/{Repositories, Context, Configurations, UnitOfWork})" -ForegroundColor Green
    }

    # UI project folders
    $uiPath = "$ProjectName.UI"
    New-Item -ItemType Directory -Path "$uiPath/Forms" -Force | Out-Null
    New-Item -ItemType Directory -Path "$uiPath/Controls" -Force | Out-Null
    New-Item -ItemType Directory -Path "$uiPath/Factories" -Force | Out-Null

    if ($Pattern -eq "MVP") {
        New-Item -ItemType Directory -Path "$uiPath/Views" -Force | Out-Null
        New-Item -ItemType Directory -Path "$uiPath/Presenters" -Force | Out-Null
        Write-Host "  [OK] Created UI project folders (Forms, Controls, Views, Presenters, Factories)" -ForegroundColor Green
    }
    elseif ($Pattern -eq "MVVM") {
        New-Item -ItemType Directory -Path "$uiPath/ViewModels" -Force | Out-Null
        Write-Host "  [OK] Created UI project folders (Forms, Controls, ViewModels, Factories)" -ForegroundColor Green
    }
    else {
        Write-Host "  [OK] Created UI project folders (Forms, Controls, Factories)" -ForegroundColor Green
    }

    # Move Form1 to Forms folder and rename to MainForm
    Move-Item -Path "$uiPath/Form1.cs" -Destination "$uiPath/Forms/MainForm.cs" -Force
    Move-Item -Path "$uiPath/Form1.Designer.cs" -Destination "$uiPath/Forms/MainForm.Designer.cs" -Force
    if (Test-Path "$uiPath/Form1.resx") {
        Move-Item -Path "$uiPath/Form1.resx" -Destination "$uiPath/Forms/MainForm.resx" -Force
    }

    # Update MainForm.cs namespace
    $mainFormContent = Get-Content "$uiPath/Forms/MainForm.cs" -Raw
    $mainFormContent = $mainFormContent -replace "namespace $ProjectName\.UI", "namespace $ProjectName.UI.Forms"
    $mainFormContent = $mainFormContent -replace "partial class Form1", "partial class MainForm"
    $mainFormContent = $mainFormContent -replace "public Form1\(\)", "public MainForm()"
    $mainFormContent | Out-File -FilePath "$uiPath/Forms/MainForm.cs" -Encoding UTF8 -Force

    # Update MainForm.Designer.cs
    $designerContent = Get-Content "$uiPath/Forms/MainForm.Designer.cs" -Raw
    $designerContent = $designerContent -replace "namespace $ProjectName\.UI", "namespace $ProjectName.UI.Forms"
    $designerContent = $designerContent -replace "partial class Form1", "partial class MainForm"
    $designerContent = $designerContent -replace "Form1", "MainForm"
    $designerContent | Out-File -FilePath "$uiPath/Forms/MainForm.Designer.cs" -Encoding UTF8 -Force

    Write-Host "  [OK] Moved and renamed Form1 to MainForm in UI/Forms/" -ForegroundColor Green

    # ========================================================================
    # Create README.md for each project in Multi-Project structure
    # ========================================================================
    Write-Host "  Creating README.md files for each project..." -ForegroundColor Cyan

    # Domain Project README
    $domainReadme = @"
# $ProjectName.Domain

> **Layer**: Domain Layer (Core/Innermost Layer)
> **Dependencies**: None (should not reference other projects)

## Purpose
Contains the domain model and core business interfaces. This is the **innermost layer** in Clean Architecture.

## What Goes Here

### ✅ Models (Domain Entities)
- ``/Models/Customer.cs``
- ``/Models/Order.cs``
- ``/Models/Product.cs``
- Pure C# classes representing business entities
- No framework dependencies
- Should have validation attributes

### ✅ Interfaces
- ``/Interfaces/IRepository.cs``
- ``/Interfaces/IUnitOfWork.cs``
- ``/Interfaces/ICustomerService.cs``
- Contracts that other layers implement

### ✅ Enums
- ``/Enums/OrderStatus.cs``
- ``/Enums/CustomerType.cs``

### ✅ Custom Exceptions
- ``/Exceptions/ValidationException.cs``
- ``/Exceptions/NotFoundException.cs``

## Key Rules

### ❌ DO NOT Put Here:
- Database code (belongs in Data project)
- Business logic implementation (belongs in Business project)
- UI code (belongs in UI project)
- External framework dependencies

### ✅ DO Put Here:
- Domain models (entities)
- Interface definitions
- Enums
- Custom exceptions
- Value objects

## Dependencies
**NuGet Packages**:
- ``System.ComponentModel.Annotations`` (for validation attributes)

**Project References**:
- None (Core should be dependency-free!)

## Example Structure
\`\`\`
$ProjectName.Domain/
|-- Models/
|   |-- Customer.cs
|   |-- Order.cs
|   \-- Product.cs
|-- Interfaces/
|   |-- ICustomerRepository.cs
|   |-- ICustomerService.cs
|   \-- IUnitOfWork.cs
|-- Enums/
|   \-- OrderStatus.cs
\-- Exceptions/
    \-- ValidationException.cs
\`\`\`

## Notes
- This project defines **WHAT** the system does (domain model)
- Other projects define **HOW** it does it (implementation)
- Keep this layer clean and framework-independent
"@

    $domainReadme | Out-File -FilePath "$ProjectName.Domain/README.md" -Encoding UTF8 -Force

    # Application Project README
    $applicationReadme = @"
# $ProjectName.Application

> **Layer**: Business Logic Layer (Application Layer)
> **Dependencies**: Core

## Purpose
Contains business logic, validation rules, and service implementations. This is where the **application behavior** lives.

## What Goes Here

### ✅ Services (Business Logic)
- ``/Services/CustomerService.cs``
- ``/Services/OrderService.cs``
- Implements business rules and workflows
- Uses repositories via Unit of Work
- Contains complex business operations

### ✅ Validators
- ``/Validators/CustomerValidator.cs``
- Input validation logic
- Business rule validation

## Key Rules

### ❌ DO NOT Put Here:
- Database code (use IRepository from Core, implemented in Data)
- UI code (belongs in UI project)
- Direct EF Core DbContext usage (use IUnitOfWork)
- Domain models (belong in Core)

### ✅ DO Put Here:
- Service implementations (ICustomerService → CustomerService)
- Business logic and workflows
- Validation logic
- Business rules
- Complex calculations

## Service Pattern Example

\`\`\`csharp
public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(
        IUnitOfWork unitOfWork,
        ILogger<CustomerService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Customer> CreateCustomerAsync(Customer customer)
    {
        // 1. Validate
        if (string.IsNullOrWhiteSpace(customer.Name))
            throw new ValidationException("Customer name is required");

        // 2. Business logic
        customer.CreatedDate = DateTime.UtcNow;
        customer.IsActive = true;

        // 3. Repository operation
        await _unitOfWork.Customers.AddAsync(customer);

        // 4. Save via Unit of Work
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Customer created: {Id}", customer.Id);
        return customer;
    }
}
\`\`\`

## Dependencies

**NuGet Packages**:
- ``Microsoft.Extensions.Logging.Abstractions``

**Project References**:
- ``$ProjectName.Domain`` (for models and interfaces)

## Example Structure
\`\`\`
$ProjectName.Application/
|-- Services/
|   |-- CustomerService.cs
|   |-- OrderService.cs
|   \-- ProductService.cs
\-- Validators/
    |-- CustomerValidator.cs
    \-- OrderValidator.cs
\`\`\`

## Notes
- Services implement interfaces defined in Core
- Use constructor injection for dependencies
- Always inject IUnitOfWork, NOT IRepository directly
- Let Unit of Work manage SaveChanges()
"@

    $applicationReadme | Out-File -FilePath "$ProjectName.Application/README.md" -Encoding UTF8 -Force

    # Infrastructure Project README (only if database selected)
    if ($Database -ne "None") {
        $infrastructureReadme = @"
# $ProjectName.Infrastructure

> **Layer**: Infrastructure Layer (Data Access + External Services)
> **Dependencies**: Domain

## Purpose
Handles all **external concerns** - database, file system, external APIs, email, etc. This is the **outermost layer** in Clean Architecture.

## Folder Structure

### 📁 Persistence/ (Database)
All database-related code goes in the ``Persistence`` subfolder:

- ``/Persistence/Context/AppDbContext.cs`` - EF Core DbContext
- ``/Persistence/Repositories/`` - Repository implementations
- ``/Persistence/UnitOfWork/`` - Unit of Work implementation
- ``/Persistence/Configurations/`` - Entity configurations

### 📁 Future: Other Infrastructure Services
- ``/Email/`` - Email service implementations
- ``/Logging/`` - Logging implementations
- ``/ExternalApis/`` - External API clients

## What Goes Here

### ✅ DbContext (in Persistence/Context/)
- ``/Persistence/Context/AppDbContext.cs``
- Entity Framework DbContext
- DbSet definitions
- Model configuration

### ✅ Repositories (in Persistence/Repositories/)
- ``/Persistence/Repositories/CustomerRepository.cs``
- ``/Persistence/Repositories/OrderRepository.cs``
- Implements IRepository interfaces from Domain
- **NEVER** call SaveChangesAsync in repositories!

### ✅ Unit of Work (in Persistence/UnitOfWork/)
- ``/Persistence/UnitOfWork/UnitOfWork.cs``
- Manages transactions
- Coordinates multiple repositories
- **ONLY** place to call SaveChangesAsync

### ✅ Entity Configurations (in Persistence/Configurations/)
- ``/Persistence/Configurations/CustomerConfiguration.cs``
- Fluent API configuration
- Table mappings, relationships, indexes

## Key Rules

### ❌ DO NOT:
- Call SaveChangesAsync() in repositories
- Put business logic here
- Expose DbContext outside this project
- Reference UI or Business projects

### ✅ DO:
- Implement repository interfaces from Core
- Call SaveChangesAsync() ONLY in Unit of Work
- Use Fluent API for entity configuration
- Keep repositories simple (CRUD only)
- Inject IUnitOfWork in services, NOT IRepository

## Repository Pattern Example

\`\`\`csharp
public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        // ❌ NO SaveChangesAsync here!
        // ✅ Unit of Work will handle it
    }
}
\`\`\`

## Unit of Work Pattern Example

\`\`\`csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private ICustomerRepository? _customers;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public ICustomerRepository Customers =>
        _customers ??= new CustomerRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
\`\`\`

## Dependencies

**NuGet Packages**:
- ``Microsoft.EntityFrameworkCore`` (v$efCoreVersion)
- ``Microsoft.EntityFrameworkCore.$Database`` (v$efCoreVersion)
- ``Microsoft.EntityFrameworkCore.Design`` (v$efCoreVersion)

**Project References**:
- ``$ProjectName.Domain`` (for models and interfaces)

## Database Configuration
- **Provider**: $Database
- **Connection String**: See appsettings.json in UI project

## Example Structure
\`\`\`
$ProjectName.Infrastructure/
\-- Persistence/              # Database layer
    |-- Context/
    |   \-- AppDbContext.cs
    |-- Repositories/
    |   |-- CustomerRepository.cs
    |   \-- OrderRepository.cs
    |-- UnitOfWork/
    |   \-- UnitOfWork.cs
    \-- Configurations/
        |-- CustomerConfiguration.cs
        \-- OrderConfiguration.cs
\`\`\`

## Migrations

\`\`\`bash
# Add migration
dotnet ef migrations add InitialCreate --project $ProjectName.Infrastructure --startup-project $ProjectName.UI

# Update database
dotnet ef database update --project $ProjectName.Infrastructure --startup-project $ProjectName.UI
\`\`\`

## Notes
- Repositories = simple CRUD operations
- Unit of Work = transaction coordinator
- NEVER expose DbContext to other layers
- Use async/await for all database operations
"@

        $infrastructureReadme | Out-File -FilePath "$ProjectName.Infrastructure/README.md" -Encoding UTF8 -Force
    }

    # UI Project README
    $uiReadme = @"
# $ProjectName.UI

> **Layer**: Presentation Layer (UI Layer)
> **Dependencies**: Core, Business$(if ($Database -ne "None") { ", Data" })

## Purpose
Windows Forms user interface. This is the **presentation layer** and application entry point.

## What Goes Here

### ✅ Forms (UI)
- ``/Forms/MainForm.cs``
- ``/Forms/CustomerListForm.cs``
- ``/Forms/CustomerEditForm.cs``
- Windows Forms UI
- **Minimal logic** - delegate to Presenters/Services

### ✅ Presenters (MVP Pattern)
$(if ($Pattern -eq "MVP") {
    "- \`\`/Presenters/CustomerListPresenter.cs\`\``n- \`\`/Presenters/CustomerEditPresenter.cs\`\``n- Contains UI logic`n- Mediates between View and Service"
} else {
    "Not using MVP pattern in this project"
})

### ✅ View Interfaces (MVP Pattern)
$(if ($Pattern -eq "MVP") {
    "- \`\`/Views/ICustomerListView.cs\`\``n- \`\`/Views/ICustomerEditView.cs\`\``n- Defines what UI can do`n- Forms implement these interfaces"
} else {
    "Not using MVP pattern in this project"
})

### ✅ Custom Controls
- ``/Controls/CustomButton.cs``
- ``/Controls/SearchBox.cs``
- Reusable UI components

### ✅ Factories
- ``/Factories/FormFactory.cs``
- Creates forms via DI
- **Use IFormFactory, NOT IServiceProvider!**

### ✅ Program.cs
- Application entry point
- DI container setup
- Configuration loading

## Key Rules

### ❌ DO NOT:
- Put business logic in Forms
- Directly access database/DbContext
- Inject IServiceProvider (use IFormFactory)
- Create forms with ``new`` keyword (use factory)

### ✅ DO:
- Keep Forms thin (minimal logic)
- Use Presenters/Services for logic
- Inject IFormFactory for creating child forms
- Use async/await for I/O operations
- Handle exceptions and show user-friendly messages

## MVP Pattern Example

\`\`\`csharp
// 1. View Interface
public interface ICustomerListView
{
    void DisplayCustomers(List<Customer> customers);
    void ShowError(string message);
}

// 2. Form (implements ICustomerListView)
public partial class CustomerListForm : Form, ICustomerListView
{
    private readonly CustomerListPresenter _presenter;

    public CustomerListForm(CustomerListPresenter presenter)
    {
        InitializeComponent();
        _presenter = presenter;
        _presenter.SetView(this);
    }

    public void DisplayCustomers(List<Customer> customers)
    {
        dgvCustomers.DataSource = customers;
    }

    public void ShowError(string message)
    {
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private async void CustomerListForm_Load(object sender, EventArgs e)
    {
        await _presenter.LoadCustomersAsync();
    }
}

// 3. Presenter
public class CustomerListPresenter
{
    private readonly ICustomerService _customerService;
    private ICustomerListView? _view;

    public CustomerListPresenter(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public void SetView(ICustomerListView view)
    {
        _view = view;
    }

    public async Task LoadCustomersAsync()
    {
        try
        {
            var customers = await _customerService.GetAllAsync();
            _view?.DisplayCustomers(customers);
        }
        catch (Exception ex)
        {
            _view?.ShowError(ex.Message);
        }
    }
}
\`\`\`

## Dependencies

**NuGet Packages**:
- ``Microsoft.Extensions.DependencyInjection``
- ``Microsoft.Extensions.Configuration.Json``
- ``Microsoft.Extensions.Logging``
- ``Serilog.Extensions.Logging``
- ``Serilog.Sinks.File``
$(if ($UIFramework -eq "DevExpress") {
    '- ``DevExpress.WindowsDesktop.Win.Grid``' + "`n" + '- ``DevExpress.WindowsDesktop.Win.Editors``' + "`n" + '- ``DevExpress.WindowsDesktop.Win.Layout``'
} elseif ($UIFramework -eq "ReaLTaiizor") {
    '- ``ReaLTaiizor`` (free' + ', ' + 'open-source)'
})

**Project References**:
- ``$ProjectName.Domain`` (for models and interfaces)
- ``$ProjectName.Application`` (for services)
$(if ($Database -ne "None") { "- ``$ProjectName.Infrastructure`` (for Unit of Work, repositories)" })

## Configuration

### appsettings.json
$(if ($Database -ne "None") {
    '```json' + "`n{`n  " + '"ConnectionStrings": {' + "`n    " + '"DefaultConnection": ' + '"' + $connectionString + '"' + "`n  }`n}`n" + '```'
} else {
    'No database configuration'
})

## Example Structure
\`\`\`
$ProjectName.UI/
|-- Forms/
|   |-- MainForm.cs
|   |-- CustomerListForm.cs
|   +-- CustomerEditForm.cs
$(if ($Pattern -eq "MVP") {
    "|-- Views/`n|   |-- ICustomerListView.cs`n|   +-- ICustomerEditView.cs`n|-- Presenters/`n|   |-- CustomerListPresenter.cs`n|   +-- CustomerEditPresenter.cs"
})
|-- Controls/
|   +-- (custom controls)
|-- Factories/
|   +-- FormFactory.cs
|-- Program.cs
+-- appsettings.json
\`\`\`

## Running the Application

\`\`\`bash
dotnet run --project $ProjectName.UI
\`\`\`

## Notes
- This is the **startup project**
- Contains DI container setup in Program.cs
- Forms should be thin - delegate to Presenters/Services
- Use IFormFactory to create child forms
"@

    $uiReadme | Out-File -FilePath "$ProjectName.UI/README.md" -Encoding UTF8 -Force

    Write-Host "  [OK] Created README.md files for all projects" -ForegroundColor Green
}

# ============================================================================
# Step 4: Add NuGet Packages
# ============================================================================
Write-Host ""
Write-Host "[4] Adding NuGet packages..." -ForegroundColor Cyan

# Determine EF Core version based on .NET version
$efCoreVersion = switch ($Framework) {
    "net8.0" { "8.0.11" }
    "net6.0" { "6.0.36" }
    "net48"  { "6.0.36" }  # .NET Framework 4.8 uses EF Core 6.x (last version supporting .NET Framework)
    default  { "8.0.11" }
}

if ($ProjectStructure -eq "Single") {
    # Single Project: Add all packages to main project
    $packages = @(
        "Microsoft.Extensions.DependencyInjection",
        "Microsoft.Extensions.Configuration.Json",
        "Microsoft.Extensions.Logging",
        "Serilog.Extensions.Logging",
        "Serilog.Sinks.File"
    )

    # Add DevExpress packages if selected
    if ($UIFramework -eq "DevExpress") {
        $packages += "DevExpress.WindowsDesktop.Win.Grid"
        $packages += "DevExpress.WindowsDesktop.Win.Editors"
        $packages += "DevExpress.WindowsDesktop.Win.Layout"
        $packages += "DevExpress.WindowsDesktop.Win.Navigation"
        $packages += "DevExpress.WindowsDesktop.Win.Reporting"
    }

    # Add ReaLTaiizor package if selected
    if ($UIFramework -eq "ReaLTaiizor") {
        $packages += "ReaLTaiizor"
        # ReaLTaiizor may have dependencies that require System.Data.SqlClient for backward compatibility
        $packages += "System.Data.SqlClient:4.8.6"
    }

    # Add database-specific packages with version
    if ($Database -eq "SQLite") {
        $packages += "Microsoft.EntityFrameworkCore.Sqlite:$efCoreVersion"
        $packages += "Microsoft.EntityFrameworkCore.Design:$efCoreVersion"
    }
    elseif ($Database -eq "SQLServer") {
        $packages += "Microsoft.EntityFrameworkCore.SqlServer:$efCoreVersion"
        $packages += "Microsoft.EntityFrameworkCore.Design:$efCoreVersion"
    }
    elseif ($Database -eq "PostgreSQL") {
        $packages += "Npgsql.EntityFrameworkCore.PostgreSQL:$efCoreVersion"
        $packages += "Microsoft.EntityFrameworkCore.Design:$efCoreVersion"
    }
    elseif ($Database -eq "MySQL") {
        $mySqlVersion = switch ($Framework) {
            "net8.0" { "8.0.0" }
            "net6.0" { "6.0.2" }
            "net48"  { "6.0.2" }
            default  { "8.0.0" }
        }
        $packages += "Pomelo.EntityFrameworkCore.MySql:$mySqlVersion"
        $packages += "Microsoft.EntityFrameworkCore.Design:$efCoreVersion"
    }

    foreach ($package in $packages) {
        $packageParts = $package -split ":"
        $packageName = $packageParts[0]
        $packageVersion = if ($packageParts.Length -gt 1) { $packageParts[1] } else { $null }

        Write-Host "  Adding $packageName..." -NoNewline
        if ($packageVersion) {
            dotnet add $ProjectName package $packageName --version $packageVersion --no-restore | Out-Null
        } else {
            dotnet add $ProjectName package $packageName --no-restore | Out-Null
        }
        Write-Host " [OK]" -ForegroundColor Green
    }

    Write-Host "  Restoring packages..." -NoNewline
    dotnet restore $ProjectName | Out-Null
    Write-Host " [OK]" -ForegroundColor Green
}
else {
    # Multi-Project: Add packages to specific projects

    # UI Project packages
    Write-Host "  Adding packages to UI project..." -ForegroundColor Cyan
    $uiPackages = @(
        "Microsoft.Extensions.DependencyInjection",
        "Microsoft.Extensions.Configuration.Json",
        "Microsoft.Extensions.Logging",
        "Serilog.Extensions.Logging",
        "Serilog.Sinks.File"
    )

    # Add UI framework packages
    if ($UIFramework -eq "DevExpress") {
        $uiPackages += "DevExpress.WindowsDesktop.Win.Grid"
        $uiPackages += "DevExpress.WindowsDesktop.Win.Editors"
        $uiPackages += "DevExpress.WindowsDesktop.Win.Layout"
        $uiPackages += "DevExpress.WindowsDesktop.Win.Navigation"
        $uiPackages += "DevExpress.WindowsDesktop.Win.Reporting"
    }
    elseif ($UIFramework -eq "ReaLTaiizor") {
        $uiPackages += "ReaLTaiizor"
    }

    foreach ($package in $uiPackages) {
        Write-Host "    Adding $package..." -NoNewline
        dotnet add "$ProjectName.UI" package $package --no-restore | Out-Null
        Write-Host " [OK]" -ForegroundColor Green
    }

    # Domain Project packages (minimal - just annotations)
    Write-Host "  Adding packages to Domain project..." -ForegroundColor Cyan
    dotnet add "$ProjectName.Domain" package "System.ComponentModel.Annotations" --no-restore | Out-Null
    Write-Host "    Adding System.ComponentModel.Annotations... [OK]" -ForegroundColor Green

    # Application Project packages
    Write-Host "  Adding packages to Application project..." -ForegroundColor Cyan
    dotnet add "$ProjectName.Application" package "Microsoft.Extensions.Logging.Abstractions" --no-restore | Out-Null
    Write-Host "    Adding Microsoft.Extensions.Logging.Abstractions... [OK]" -ForegroundColor Green

    # Infrastructure Project packages (only if database is selected)
    if ($Database -ne "None") {
        Write-Host "  Adding packages to Infrastructure project..." -ForegroundColor Cyan

        if ($Database -eq "SQLite") {
            dotnet add "$ProjectName.Infrastructure" package "Microsoft.EntityFrameworkCore.Sqlite" --version $efCoreVersion --no-restore | Out-Null
            dotnet add "$ProjectName.Infrastructure" package "Microsoft.EntityFrameworkCore.Design" --version $efCoreVersion --no-restore | Out-Null
            Write-Host "    Adding EF Core SQLite... [OK]" -ForegroundColor Green
        }
        elseif ($Database -eq "SQLServer") {
            dotnet add "$ProjectName.Infrastructure" package "Microsoft.EntityFrameworkCore.SqlServer" --version $efCoreVersion --no-restore | Out-Null
            dotnet add "$ProjectName.Infrastructure" package "Microsoft.EntityFrameworkCore.Design" --version $efCoreVersion --no-restore | Out-Null
            Write-Host "    Adding EF Core SQL Server... [OK]" -ForegroundColor Green
        }
        elseif ($Database -eq "PostgreSQL") {
            dotnet add "$ProjectName.Infrastructure" package "Npgsql.EntityFrameworkCore.PostgreSQL" --version $efCoreVersion --no-restore | Out-Null
            dotnet add "$ProjectName.Infrastructure" package "Microsoft.EntityFrameworkCore.Design" --version $efCoreVersion --no-restore | Out-Null
            Write-Host "    Adding EF Core PostgreSQL... [OK]" -ForegroundColor Green
        }
        elseif ($Database -eq "MySQL") {
            $mySqlVersion = switch ($Framework) {
                "net8.0" { "8.0.0" }
                "net6.0" { "6.0.2" }
                "net48"  { "6.0.2" }
                default  { "8.0.0" }
            }
            dotnet add "$ProjectName.Infrastructure" package "Pomelo.EntityFrameworkCore.MySql" --version $mySqlVersion --no-restore | Out-Null
            dotnet add "$ProjectName.Infrastructure" package "Microsoft.EntityFrameworkCore.Design" --version $efCoreVersion --no-restore | Out-Null
            Write-Host "    Adding EF Core MySQL... [OK]" -ForegroundColor Green
        }
    }

    Write-Host "  Restoring all packages..." -NoNewline
    dotnet restore | Out-Null
    Write-Host " [OK]" -ForegroundColor Green
}

# Add README.md files to .csproj so they show in Rider/VS
if ($ProjectStructure -eq "Single") {
    $csprojPath = "$ProjectName/$ProjectName.csproj"
    $projectRootPath = $ProjectName
}
else {
    $csprojPath = "$ProjectName.UI/$ProjectName.UI.csproj"
    $projectRootPath = $ProjectName
}

$csprojContent = Get-Content $csprojPath -Raw

# Add ItemGroup for README files if not already present
if (-not $csprojContent.Contains("<None Include=")) {
    $searchPath = if ($ProjectStructure -eq "Single") { $ProjectName } else { "$ProjectName.UI" }
    $basePathForReplace = "$((Get-Location).Path)\$searchPath\"

    $readmeFiles = Get-ChildItem -Path $searchPath -Recurse -Filter "README.md" | ForEach-Object {
        $relativePath = $_.FullName.Replace($basePathForReplace, "").Replace("\", "/")
        "    <None Include=`"$relativePath`" />"
    }

    $readmeItemGroup = "`n  <ItemGroup>`n" + ($readmeFiles -join "`n") + "`n  </ItemGroup>"
    $endProjectTag = '</Project>'
    $csprojContent = $csprojContent.Replace($endProjectTag, $readmeItemGroup + [Environment]::NewLine + $endProjectTag)
    $csprojContent | Out-File -FilePath $csprojPath -Encoding UTF8 -Force
}

# ============================================================================
# Step 5: Create appsettings.json
# ============================================================================
Write-Host ""
Write-Host "[5] Creating appsettings.json..." -ForegroundColor Cyan

# Generate connection string based on database choice
$connectionString = switch ($Database) {
    "SQLite" { "Data Source=$ProjectName.db" }
    "SQLServer" { "Server=localhost;Database=$ProjectName;Integrated Security=true;TrustServerCertificate=true;" }
    "PostgreSQL" { "Host=localhost;Database=$ProjectName;Username=postgres;Password=your_password" }
    "MySQL" { "Server=localhost;Database=$ProjectName;User=root;Password=your_password;" }
    "None" { "" }
    default { "Data Source=$ProjectName.db" }
}

$appsettingsHash = @{
    Logging = @{
        LogLevel = @{
            Default = "Information"
            Microsoft = "Warning"
            "Microsoft.EntityFrameworkCore" = "Information"
        }
    }
}

# Add connection string if database is selected
if ($Database -ne "None") {
    $appsettingsHash["ConnectionStrings"] = @{
        DefaultConnection = $connectionString
    }
}

$appsettingsPath = if ($ProjectStructure -eq "Single") { "$ProjectName/appsettings.json" } else { "$ProjectName.UI/appsettings.json" }
$appsettingsHash | ConvertTo-Json -Depth 10 | Out-File -FilePath $appsettingsPath -Encoding UTF8

# Update .csproj to copy appsettings.json
if ($ProjectStructure -eq "Single") {
    $csprojPath = "$ProjectName/$ProjectName.csproj"
}
else {
    $csprojPath = "$ProjectName.UI/$ProjectName.UI.csproj"
}
$csprojContent = Get-Content $csprojPath -Raw
$itemGroupXml = @'
  <ItemGroup>
    <None Update="appsettings.json">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>
'@

$endProjectTag = '</Project>'
$csprojContent = $csprojContent.Replace($endProjectTag, $itemGroupXml + [Environment]::NewLine + $endProjectTag)
$csprojContent | Out-File -FilePath $csprojPath -Encoding UTF8

Write-Host "  [OK] appsettings.json created" -ForegroundColor Green
if ($Database -ne "None") {
    Write-Host "  [INFO] Database: $Database" -ForegroundColor Cyan
    Write-Host "  [INFO] Connection string: $connectionString" -ForegroundColor Gray
}

# ============================================================================
# Step 6: Create Program.cs with DI
# ============================================================================
Write-Host ""
Write-Host "[6] Creating Program.cs with DI..." -ForegroundColor Cyan

# Generate using statements based on database (both Single and Multi use same namespace now)
$usingStatements = if ($Database -ne "None") {
    switch ($Database) {
        "SQLite" { "using Microsoft.EntityFrameworkCore;`nusing $ProjectName.Infrastructure.Persistence;" }
        "SQLServer" { "using Microsoft.EntityFrameworkCore;`nusing $ProjectName.Infrastructure.Persistence;" }
        "PostgreSQL" { "using Microsoft.EntityFrameworkCore;`nusing $ProjectName.Infrastructure.Persistence;" }
        "MySQL" { "using Microsoft.EntityFrameworkCore;`nusing $ProjectName.Infrastructure.Persistence;" }
        default { "" }
    }
} else {
    ""
}

# Generate DbContext registration code based on database
$dbContextCode = if ($Database -ne "None") {
    switch ($Database) {
        "SQLite" {
            @"
            // Database
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
"@
        }
        "SQLServer" {
            @"
            // Database
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
"@
        }
        "PostgreSQL" {
            @"
            // Database
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
"@
        }
        "MySQL" {
            @"
            // Database
            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    configuration.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection"))));
"@
        }
    }
} else {
    "            // No database configured"
}

# Both Single and Multi use UI.Forms namespace now
$programNamespace = $ProjectName
$programFormsUsing = "$ProjectName.UI.Forms"

$programCs = @"
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using $programFormsUsing;
$usingStatements

namespace $programNamespace
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                ApplicationConfiguration.Initialize();

                // Build configuration
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                // Build DI container
                var services = new ServiceCollection();
                ConfigureServices(services, configuration);
                var serviceProvider = services.BuildServiceProvider();

                // Run application
                var mainForm = serviceProvider.GetRequiredService<MainForm>();
                System.Windows.Forms.Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
                MessageBox.Show(
                    `$"Fatal error: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Configuration
            services.AddSingleton(configuration);

            // Logging
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog();
            });

$dbContextCode

            // TODO: Add your services here
            // services.AddScoped<IYourRepository, YourRepository>();
            // services.AddScoped<IYourService, YourService>();

            // Forms
            services.AddTransient<MainForm>();
        }
    }
}
"@

$programCsPath = if ($ProjectStructure -eq "Single") { "$ProjectName/Program.cs" } else { "$ProjectName.UI/Program.cs" }
$programCs | Out-File -FilePath $programCsPath -Encoding UTF8 -Force

Write-Host "  [OK] Program.cs created with DI" -ForegroundColor Green

# ============================================================================
# Step 6.5: Create AppDbContext (if database is selected)
# ============================================================================
if ($Database -ne "None") {
    Write-Host ""
    Write-Host "[6.5] Creating AppDbContext..." -ForegroundColor Cyan

    # Both Single and Multi now use same namespace structure
    $dbContextNamespace = "$ProjectName.Infrastructure.Persistence"

    $appDbContextCs = @"
using Microsoft.EntityFrameworkCore;

namespace $dbContextNamespace
{
    /// <summary>
    /// Application database context.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // TODO: Add your DbSets here
        // Example:
        // public DbSet<Customer> Customers { get; set; } = null!;
        // public DbSet<Order> Orders { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TODO: Add your entity configurations here
            // Example:
            // modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        }
    }
}
"@

    $dbContextPath = if ($ProjectStructure -eq "Single") { "$ProjectName/Infrastructure/Persistence/Context/AppDbContext.cs" } else { "$ProjectName.Infrastructure/Persistence/Context/AppDbContext.cs" }
    $appDbContextCs | Out-File -FilePath $dbContextPath -Encoding UTF8 -Force
    Write-Host "  [OK] AppDbContext.cs created" -ForegroundColor Green
}

# ============================================================================
# Step 7: Create Test Project (if requested)
# ============================================================================
if ($IncludeTests) {
    Write-Host ""
    Write-Host "[7] Creating test projects..." -ForegroundColor Cyan

    # Unit tests
    # Create test project with base framework first (xunit template doesn't support -windows suffix)
    dotnet new xunit -n "$ProjectName.Tests" -f $Framework
    dotnet sln add "$ProjectName.Tests/$ProjectName.Tests.csproj"

    # Update test project to use -windows framework for WinForms compatibility
    $testCsprojPath = "$ProjectName.Tests/$ProjectName.Tests.csproj"
    $testCsprojContent = Get-Content $testCsprojPath -Raw
    $testCsprojContent = $testCsprojContent -replace "<TargetFramework>$Framework</TargetFramework>", "<TargetFramework>$Framework-windows</TargetFramework>"
    $testCsprojContent | Out-File -FilePath $testCsprojPath -Encoding UTF8 -Force

    # Add references based on structure
    if ($ProjectStructure -eq "Single") {
        dotnet add "$ProjectName.Tests" reference $ProjectName
    }
    else {
        # Multi-Project: Reference all projects for comprehensive testing
        dotnet add "$ProjectName.Tests" reference "$ProjectName.Domain" "$ProjectName.Application" | Out-Null
        if ($Database -ne "None") {
            dotnet add "$ProjectName.Tests" reference "$ProjectName.Infrastructure" | Out-Null
        }
    }

    dotnet add "$ProjectName.Tests" package Moq --no-restore
    dotnet add "$ProjectName.Tests" package FluentAssertions --no-restore

    Write-Host "  [OK] Unit test project created" -ForegroundColor Green

    # Integration tests (only if database is selected)
    if ($Database -ne "None") {
        dotnet new xunit -n "$ProjectName.IntegrationTests" -f $Framework
        dotnet sln add "$ProjectName.IntegrationTests/$ProjectName.IntegrationTests.csproj"

        # Update integration test project to use -windows framework
        $integrationTestCsprojPath = "$ProjectName.IntegrationTests/$ProjectName.IntegrationTests.csproj"
        $integrationTestCsprojContent = Get-Content $integrationTestCsprojPath -Raw
        $integrationTestCsprojContent = $integrationTestCsprojContent -replace "<TargetFramework>$Framework</TargetFramework>", "<TargetFramework>$Framework-windows</TargetFramework>"
        $integrationTestCsprojContent | Out-File -FilePath $integrationTestCsprojPath -Encoding UTF8 -Force

        # Add references based on structure
        if ($ProjectStructure -eq "Single") {
            dotnet add "$ProjectName.IntegrationTests" reference $ProjectName
        }
        else {
            # Multi-Project: Integration tests need Infrastructure and Domain projects
            dotnet add "$ProjectName.IntegrationTests" reference "$ProjectName.Domain" "$ProjectName.Infrastructure" | Out-Null
        }

        # Add database package to integration tests (with correct version)
        if ($Database -eq "SQLite") {
            dotnet add "$ProjectName.IntegrationTests" package Microsoft.EntityFrameworkCore.Sqlite --version $efCoreVersion --no-restore
        }
        elseif ($Database -eq "SQLServer") {
            dotnet add "$ProjectName.IntegrationTests" package Microsoft.EntityFrameworkCore.SqlServer --version $efCoreVersion --no-restore
        }
        elseif ($Database -eq "PostgreSQL") {
            dotnet add "$ProjectName.IntegrationTests" package Npgsql.EntityFrameworkCore.PostgreSQL --version $efCoreVersion --no-restore
        }
        elseif ($Database -eq "MySQL") {
            dotnet add "$ProjectName.IntegrationTests" package Pomelo.EntityFrameworkCore.MySql --version $mySqlVersion --no-restore
        }

        Write-Host "  [OK] Integration test project created" -ForegroundColor Green
    }

    # Restore packages
    Write-Host "  Restoring test packages..." -NoNewline
    dotnet restore | Out-Null
    Write-Host " [OK]" -ForegroundColor Green
}

# ============================================================================
# Step 8: Setup configuration files (.editorconfig and .gitignore)
# ============================================================================
Write-Host ""
Write-Host "[8] Setting up configuration files..." -ForegroundColor Cyan

$scriptPath = Split-Path -Parent $PSCommandPath
$repoRoot = Split-Path -Parent $scriptPath

# These files will be symlinked if standards are integrated (auto-update)
# Otherwise they will be copied (one-time)
$configFiles = @(".editorconfig", ".gitignore")

foreach ($file in $configFiles) {
    if (Test-Path "$repoRoot/$file") {
        # Just copy for now - will be converted to symlink in step 10 if integrating standards
        Copy-Item "$repoRoot/$file" -Destination "." -Force
        Write-Host "  [OK] $file copied" -ForegroundColor Green
    }
}

# ============================================================================
# Step 9: Create VS Code configuration
# ============================================================================
Write-Host ""
Write-Host "[9] Creating VS Code tasks and launch config..." -ForegroundColor Cyan

New-Item -ItemType Directory -Path ".vscode" -Force | Out-Null

# Create tasks.json
$tasksJson = @"
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "build",
      "command": "dotnet",
      "type": "process",
      "args": [
        "build",
        "`${workspaceFolder}/$ProjectName.sln",
        "/property:GenerateFullPaths=true",
        "/consoleloggerparameters:NoSummary"
      ],
      "problemMatcher": "`$msCompile",
      "group": {
        "kind": "build",
        "isDefault": true
      }
    },
    {
      "label": "test",
      "command": "dotnet",
      "type": "process",
      "args": ["test", "`${workspaceFolder}/$ProjectName.sln"],
      "problemMatcher": "`$msCompile",
      "group": {
        "kind": "test",
        "isDefault": true
      }
    }
  ]
}
"@

$tasksJson | Out-File -FilePath ".vscode/tasks.json" -Encoding UTF8 -Force
Write-Host "  [OK] .vscode/tasks.json created" -ForegroundColor Green

# Create launch.json
$launchProjectName = if ($ProjectStructure -eq "Single") { $ProjectName } else { "$ProjectName.UI" }
$launchDllName = if ($ProjectStructure -eq "Single") { $ProjectName } else { "$ProjectName.UI" }

$launchJson = @"
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Launch (WinForms)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "`${workspaceFolder}/$launchProjectName/bin/Debug/$Framework-windows/$launchDllName.dll",
      "args": [],
      "cwd": "`${workspaceFolder}/$launchProjectName",
      "console": "internalConsole",
      "stopAtEntry": false
    }
  ]
}
"@

$launchJson | Out-File -FilePath ".vscode/launch.json" -Encoding UTF8 -Force
Write-Host "  [OK] .vscode/launch.json created" -ForegroundColor Green

# ============================================================================
# Step 10: Initialize Git
# ============================================================================
Write-Host ""
Write-Host "[10] Initializing git repository..." -ForegroundColor Cyan

git init 2>&1 | Out-Null
git -c core.autocrlf=false add . 2>&1 | Out-Null
git commit -m "Initial commit: Project structure created by init-project-interactive.ps1" 2>&1 | Out-Null

Write-Host "  [OK] Git repository initialized" -ForegroundColor Green

# ============================================================================
# Step 11: Integrate Coding Standards (if requested)
# ============================================================================
if ($IntegrateStandards) {
    Write-Host ""
    Write-Host "[11] Integrating coding standards..." -ForegroundColor Cyan

    # Auto-detect standards repo URL
    $StandardsRepo = ""
    Push-Location $repoRoot
    $StandardsRepo = git remote get-url origin 2>$null
    Pop-Location

    if ($StandardsRepo) {
        Write-Host "  Using standards repo: $StandardsRepo"

        # Add as submodule
        Start-Process git -ArgumentList "submodule","add",$StandardsRepo,".standards" -NoNewWindow -Wait -RedirectStandardError "$env:TEMP\git_stderr.txt" -RedirectStandardOutput "$env:TEMP\git_stdout.txt"

        # Check if submodule was actually added
        if (Test-Path ".standards/.git") {
            git submodule update --init --recursive *>&1 | Out-Null
            Write-Host "  [OK] Standards added as submodule" -ForegroundColor Green

            # Copy standards files (portable - works on any machine)
            Write-Host ""
            Write-Host "  Copying standards files..." -ForegroundColor Cyan

            # Create .claude folder
            if (-not (Test-Path ".claude")) {
                New-Item -ItemType Directory -Path ".claude" -Force | Out-Null
            }

            # Copy .claude subdirectories (agents, commands, guides, workflows)
            if (Test-Path ".standards\.claude") {
                $claudeSubdirs = @("agents", "commands", "guides", "workflows")
                foreach ($subdir in $claudeSubdirs) {
                    if (Test-Path ".standards\.claude\$subdir") {
                        Copy-Item -Recurse ".standards\.claude\$subdir" -Destination ".claude\$subdir" -Force
                    }
                }
                Write-Host "  [OK] Copied .claude directory" -ForegroundColor Green
            }

            # Copy templates
            if (Test-Path ".standards\templates") {
                Copy-Item -Recurse ".standards\templates" -Destination "templates" -Force
                Write-Host "  [OK] Copied templates directory" -ForegroundColor Green
            }

            # Copy CLAUDE.md
            if (Test-Path ".standards\CLAUDE.md") {
                Copy-Item ".standards\CLAUDE.md" -Destination "CLAUDE.md" -Force
                Write-Host "  [OK] Copied CLAUDE.md" -ForegroundColor Green
            }

            # Copy INDEX.md
            if (Test-Path ".standards\.claude\INDEX.md") {
                Copy-Item ".standards\.claude\INDEX.md" -Destination ".claude\INDEX.md" -Force
                Write-Host "  [OK] Copied .claude\INDEX.md" -ForegroundColor Green
            }

            # Create plans directory structure
            if (-not (Test-Path "plans")) {
                New-Item -ItemType Directory -Path "plans" -Force | Out-Null
            }
            if (Test-Path ".standards\plans\templates") {
                Copy-Item -Recurse ".standards\plans\templates" -Destination "plans\templates" -Force
                Write-Host "  [OK] Copied plans\templates" -ForegroundColor Green
            }
            if (-not (Test-Path "plans\research")) {
                New-Item -ItemType Directory -Path "plans\research" -Force | Out-Null
            }
            if (-not (Test-Path "plans\reports")) {
                New-Item -ItemType Directory -Path "plans\reports" -Force | Out-Null
            }
            Write-Host "  [OK] Created plans directory structure" -ForegroundColor Green

            # Update .gitignore to exclude copied standards files
            Write-Host ""
            Write-Host "  Updating .gitignore..." -ForegroundColor Cyan

            $gitignoreEntries = @"

# ============================================================================
# Coding Standards (copied from .standards - do not commit)
# ============================================================================
# These files are copied from the standards repository and should not be
# committed to your project. Run update-standards.ps1 to update them.

# Claude Code standards (copied)
.claude/agents/
.claude/commands/
.claude/guides/
.claude/workflows/
.claude/INDEX.md
CLAUDE.md

# Templates (copied)
templates/

# Plan templates (copied)
plans/templates/

# Plan working files (generated)
plans/research/
plans/reports/

# Keep project-specific config (NOT ignored):
# .claude/project-context.md
"@

            if (Test-Path ".gitignore") {
                $gitignoreContent = Get-Content ".gitignore" -Raw
                if ($gitignoreContent -notmatch "Coding Standards \(copied") {
                    Add-Content ".gitignore" $gitignoreEntries
                }
            }
            Write-Host "  [OK] Updated .gitignore with standards exclusions" -ForegroundColor Green

            # Commit submodule and gitignore only (copied files are excluded)
            git -c core.autocrlf=false add .gitmodules .standards .gitignore 2>&1 | Out-Null
            git commit -m "Add coding standards as submodule" 2>&1 | Out-Null
            Write-Host "  [OK] Standards integration complete" -ForegroundColor Green
        } else {
            Write-Host "  [WARN]  Could not add standards submodule" -ForegroundColor Yellow
        }
    } else {
        Write-Host "  [WARN]  Standards repo URL not detected, skipping" -ForegroundColor Yellow
    }
}

# ============================================================================
# Step 12: Create Project Context for AI (Claude Code)
# ============================================================================
Write-Host ""
Write-Host "[12] Creating project context for AI assistants..." -ForegroundColor Cyan

# Determine paths
$standardsPath = if ($IntegrateStandards -and (Test-Path ".standards")) { ".standards" } else { $repoRoot }
$templatePath = Join-Path $standardsPath ".claude/project-context-template.md"

# Project-specific context should go in .claude/ (real folder, not symlink)
# .claude/ folder should already exist from integration step
$contextPath = ".claude"
if (-not (Test-Path $contextPath)) {
    New-Item -ItemType Directory -Path $contextPath -Force | Out-Null
}

$outputPath = Join-Path $contextPath "project-context.md"

# Generate framework description
$frameworkDesc = switch ($Framework) {
    "net8.0" { "Use **C# 12.0** features, latest async/await patterns, required properties, file-scoped types" }
    "net6.0" { "Use **C# 10.0** features, global using, file-scoped namespaces, async/await" }
    "net48"  { "Use **C# 7.3** features, avoid C# 8+ features (nullable reference types, ranges, etc.)" }
    default  { "Use latest C# features available for this framework" }
}

# Generate UI framework description and instructions
$uiFrameworkDesc = switch ($UIFramework) {
    "Standard" { "Use **standard WinForms controls**: Button, TextBox, DataGridView, Label, etc." }
    "DevExpress" { "Use **DevExpress controls**: XtraGrid, LookUpEdit, LayoutControl, SimpleButton, etc. See [DevExpress Overview](docs/devexpress/devexpress-overview.md)" }
    "ReaLTaiizor" { "Use **ReaLTaiizor controls**: MaterialButton, MaterialTextBox, MaterialListView, MetroGrid, etc. See [ReaLTaiizor Overview](docs/realtaiizor/realtaiizor-overview.md)" }
    default { "Use standard WinForms controls" }
}

$formInstructions = switch ($UIFramework) {
    "Standard" { @"
- **ALWAYS use** [$($standardsPath.Replace('\', '/'))/templates/form-template.cs]($($standardsPath.Replace('\', '/'))/templates/form-template.cs)
- Use standard controls: Button, TextBox, DataGridView
- Follow MVP pattern with Presenter and View interface
- Prefix controls: btn, txt, dgv, lbl, etc.
"@ }
    "DevExpress" { @"
- **ALWAYS use** [$($standardsPath.Replace('\', '/'))/templates/dx-form-template.cs]($($standardsPath.Replace('\', '/'))/templates/dx-form-template.cs)
- Use DevExpress controls: SimpleButton, TextEdit, GridControl, LayoutControl
- Follow MVP pattern with Presenter and View interface
- Prefix controls: btn, txt, grid, lbl, lookup, etc.
- **READ FIRST**: [$($standardsPath.Replace('\', '/'))/docs/devexpress/devexpress-overview.md]($($standardsPath.Replace('\', '/'))/docs/devexpress/devexpress-overview.md)
"@ }
    "ReaLTaiizor" { @"
- **ALWAYS use**: [$($standardsPath.Replace('\', '/'))/templates/rt-material-form-template.cs]($($standardsPath.Replace('\', '/'))/templates/rt-material-form-template.cs) OR [$($standardsPath.Replace('\', '/'))/templates/rt-metro-form-template.cs]($($standardsPath.Replace('\', '/'))/templates/rt-metro-form-template.cs)
- Use ReaLTaiizor controls: MaterialButton, MaterialTextBox, MaterialListView, MetroGrid
- Follow MVP pattern with Presenter and View interface
- Prefix controls: btn, txt, lst, grid, etc.
- **READ FIRST**: [$($standardsPath.Replace('\', '/'))/docs/realtaiizor/realtaiizor-overview.md]($($standardsPath.Replace('\', '/'))/docs/realtaiizor/realtaiizor-overview.md)
"@ }
    default { "Use templates from /templates/ folder" }
}

$serviceInstructions = switch ($Database) {
    "None" { "- No database configured, skip repository creation`n- Create services without data access" }
    default { @"
- **ALWAYS use** [$($standardsPath.Replace('\', '/'))/templates/repository-template.cs]($($standardsPath.Replace('\', '/'))/templates/repository-template.cs) for repositories
- **ALWAYS use** [$($standardsPath.Replace('\', '/'))/templates/service-template.cs]($($standardsPath.Replace('\', '/'))/templates/service-template.cs) for services
- **ALWAYS use** [$($standardsPath.Replace('\', '/'))/templates/unitofwork-template.cs]($($standardsPath.Replace('\', '/'))/templates/unitofwork-template.cs) for Unit of Work
- Database: **$Database** - Use appropriate Entity Framework provider
"@ }
}

$testInstructions = if ($IncludeTests) {
    "- **ALWAYS use** [$($standardsPath.Replace('\', '/'))/templates/test-template.cs]($($standardsPath.Replace('\', '/'))/templates/test-template.cs)`n- Use xUnit + Moq + FluentAssertions`n- Test projects already created: $ProjectName.Tests, $ProjectName.IntegrationTests"
} else {
    "- No test projects created`n- Consider adding tests later"
}

$patternDesc = switch ($Pattern) {
    "MVP" { "**Model-View-Presenter**: Forms implement View interfaces, Presenters contain UI logic, Services contain business logic. See [$($standardsPath.Replace('\', '/'))/docs/architecture/mvp-pattern.md]($($standardsPath.Replace('\', '/'))/docs/architecture/mvp-pattern.md)" }
    "MVVM" { "**Model-View-ViewModel**: Use ViewModels with INotifyPropertyChanged, data binding. See [$($standardsPath.Replace('\', '/'))/docs/architecture/mvvm-pattern.md]($($standardsPath.Replace('\', '/'))/docs/architecture/mvvm-pattern.md)" }
    "Simple" { "**Simple/Code-behind**: All logic in Form classes. Not recommended for production." }
    default { "Follow standard WinForms patterns" }
}

# Template list (for lazy loading - only load relevant templates)
$templateList = switch ($UIFramework) {
    "Standard" { @"
- Form: ``form-template.cs``
- Presenter: ``presenter-template.cs``
- Service: ``service-template.cs``
- Repository: ``repository-template.cs``
- UnitOfWork: ``unitofwork-template.cs``
- Validator: ``validator-template.cs``
- Test: ``test-template.cs``
- FormFactory: ``form-factory-template.cs``
"@ }
    "DevExpress" { @"
- Form: ``dx-form-template.cs``
- Grid: ``dx-grid-template.cs``
- Lookup: ``dx-lookup-template.cs``
- Report: ``dx-report-template.cs``
- Presenter: ``presenter-template.cs``
- Service: ``service-template.cs``
- Repository: ``repository-template.cs``
- UnitOfWork: ``unitofwork-template.cs``
- Validator: ``validator-template.cs``
- Test: ``test-template.cs``
"@ }
    "ReaLTaiizor" { @"
- Material Form: ``rt-material-form-template.cs``
- Metro Form: ``rt-metro-form-template.cs``
- Controls: ``rt-controls-patterns.cs``
- Presenter: ``presenter-template.cs``
- Service: ``service-template.cs``
- Repository: ``repository-template.cs``
- UnitOfWork: ``unitofwork-template.cs``
- Validator: ``validator-template.cs``
- Test: ``test-template.cs``
"@ }
    default { "See /templates/ folder" }
}

# Excluded templates (DO NOT load these - saves tokens)
$excludedTemplates = switch ($UIFramework) {
    "Standard" { @"
- ``dx-*.cs`` (DevExpress templates)
- ``rt-*.cs`` (ReaLTaiizor templates)
- ``docs/devexpress/*`` (DevExpress docs)
- ``docs/realtaiizor/*`` (ReaLTaiizor docs)
"@ }
    "DevExpress" { @"
- ``form-template.cs`` (Standard form - use dx-form instead)
- ``rt-*.cs`` (ReaLTaiizor templates)
- ``docs/realtaiizor/*`` (ReaLTaiizor docs)
"@ }
    "ReaLTaiizor" { @"
- ``form-template.cs`` (Standard form - use rt-* instead)
- ``dx-*.cs`` (DevExpress templates)
- ``docs/devexpress/*`` (DevExpress docs)
"@ }
    default { "N/A" }
}

# DO/DON'T lists
$doList = @"
- [DO] Use **$UIFramework** controls (NOT standard controls)
- [DO] Follow **$Pattern** pattern
- [DO] Use templates from **.standards/templates/** or **templates/**
- [DO] Inject **IUnitOfWork**, NOT IRepository
- [DO] Inject **IFormFactory**, NOT IServiceProvider
- [DO] Call **SaveChangesAsync()** in Unit of Work ONLY
- [DO] Use **async/await** for all I/O operations
- [DO] Validate input before processing
- [DO] Handle errors with try-catch + logging
"@

$dontList = @"
- [DON'T] DON'T use standard controls when $UIFramework is selected
- [DON'T] DON'T inject IRepository directly (use IUnitOfWork)
- [DON'T] DON'T inject IServiceProvider (use IFormFactory)
- [DON'T] DON'T call SaveChangesAsync in repositories
- [DON'T] DON'T put business logic in Forms
- [DON'T] DON'T use synchronous I/O
- [DON'T] DON'T ignore exceptions
- [DON'T] DON'T create UI from background threads
"@

# Additional folders based on pattern
$additionalFolders = if ($Pattern -eq "MVP") {
    "|-- /Views              # View interfaces`n|-- /Presenters         # Presenters"
} elseif ($Pattern -eq "MVVM") {
    "|-- /ViewModels         # ViewModels"
} else {
    ""
}

# Get NuGet packages list
$nugetPackages = @"
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Configuration.Json
- Microsoft.Extensions.Logging
- Serilog.Extensions.Logging
- Serilog.Sinks.File
"@

if ($Database -ne "None") {
    $nugetPackages += "`n- Microsoft.EntityFrameworkCore (for $Database)"
}

if ($UIFramework -eq "DevExpress") {
    $nugetPackages += "`n- DevExpress.WindowsDesktop.Win.Grid`n- DevExpress.WindowsDesktop.Win.Editors`n- DevExpress.WindowsDesktop.Win.Layout"
}

if ($UIFramework -eq "ReaLTaiizor") {
    $nugetPackages += "`n- ReaLTaiizor (free, open-source)"
}

# Connection string
$connectionStringDisplay = if ($Database -ne "None") { $connectionString } else { "N/A (no database)" }

# Generate project-context.md content
$projectContextContent = @"
# Project Context

> **IMPORTANT**: This file is **auto-generated** by ``init-project.ps1`` and contains project-specific configuration.
> AI assistants (Claude Code) should **ALWAYS read this file** to understand the project setup.

---

## Project Information

- **Project Name**: ``$ProjectName``
- **Created Date**: ``$(Get-Date -Format "yyyy-MM-dd")``
- **Framework**: ``$Framework``
- **Pattern**: ``$Pattern``
- **Structure**: ``$ProjectStructure`` ($(if ($ProjectStructure -eq "Single") { "Monolith - all code in one project" } else { "Multi-Project - separate assemblies" }))

---

## Configuration

### Target Framework
``````
$Framework
``````

**What this means**:
$frameworkDesc

### Database Provider
``````
$Database
``````

**Connection String** (from appsettings.json):
``````
$connectionStringDisplay
``````

### UI Framework
``````
$UIFramework
``````

**What this means**:
$uiFrameworkDesc

### Architecture Pattern
``````
$Pattern
``````

**What this means**:
$patternDesc

### Testing
``````
$(if ($IncludeTests) { "Unit tests + Integration tests included" } else { "No tests included" })
``````

### Project Structure
``````
$ProjectStructure
``````

**What this means**:
$(if ($ProjectStructure -eq "Single") {
"**Single Project (Monolith)** - All code in one project with folders for organization:
- Simpler, faster to build
- Convention-based separation (folders)
- Best for small/medium apps (< 20 forms)
- See: [$($standardsPath.Replace('\', '/'))/docs/architecture/project-structure.md]($($standardsPath.Replace('\', '/'))/docs/architecture/project-structure.md)"
} else {
"**Multi-Project** - Separate assemblies for each layer:
- $ProjectName.UI (WinForms)
- $ProjectName.Domain (Models, Interfaces)
- $ProjectName.Application (Services)
$(if ($Database -ne 'None') { "- $ProjectName.Infrastructure (Repositories, DbContext)" })
- Compiler-enforced architecture
- Best for large apps (20+ forms), code reuse
- See: [$($standardsPath.Replace('\', '/'))/docs/architecture/multi-project-structure.md]($($standardsPath.Replace('\', '/'))/docs/architecture/multi-project-structure.md)"
})

---

## AI Instructions - READ THIS!

### 🎯 When Creating Forms

$formInstructions

### 🎯 When Creating Services/Repositories

$serviceInstructions

### 🎯 When Writing Tests

$testInstructions

### 🎯 Templates to Use (Lazy Loading)

**ONLY load these templates for this project:**

$templateList

**DO NOT load (saves tokens):**
$excludedTemplates

> This ensures minimal token usage by loading only relevant framework templates.

---

## Project Structure Diagram

$(if ($ProjectStructure -eq "Single") {
@"
``````
/$ProjectName (Single Project - Clean Architecture)
|-- /Domain/                   # Domain Layer
|   |-- /Models                # Entities
|   |-- /Interfaces            # Contracts
|   |-- /Enums                 # Enumerations
|   \-- /Exceptions            # Custom exceptions
|-- /Application/              # Application Layer
|   |-- /Services              # Business logic
|   \-- /Validators            # Validation rules
$(if ($Database -ne 'None') {
@"
|-- /Infrastructure/           # Infrastructure Layer
|   \-- /Persistence/          # Database
|       |-- /Repositories      # Data access
|       |-- /Context           # DbContext
|       |-- /Configurations    # Entity configs
|       \-- /UnitOfWork        # Transaction coordinator
"@
})
|-- /UI/                       # Presentation Layer
|   |-- /Forms                 # WinForms
$additionalFolders
|   \-- /Factories             # Form factories
|-- /Utils                     # Helpers, extensions
\-- Program.cs                 # Entry point with DI
``````
"@
} else {
@"
``````
/$ProjectName.sln (Multi-Project Solution)
|-- $ProjectName.UI/            # Presentation Layer
|   |-- /Forms
|   |-- /Controls
$(if ($Pattern -eq "MVP") { "|   |-- /Views`n|   |-- /Presenters" } elseif ($Pattern -eq "MVVM") { "|   |-- /ViewModels" })
|   |-- /Factories
|   \-- Program.cs
|
|-- $ProjectName.Domain/          # Domain & Interfaces
|   |-- /Models
|   |-- /Interfaces
|   |-- /Enums
|   \-- /Exceptions
|
|-- $ProjectName.Application/      # Business Logic
|   |-- /Services
|   \-- /Validators
|
$(if ($Database -ne 'None') {
@"
\-- $ProjectName.Infrastructure/            # Infrastructure Layer
    \-- /Persistence/                      # Database
        |-- /Repositories
        |-- /Context
        |-- /Configurations
        \-- /UnitOfWork
"@
})
``````
"@
})

---

## Quick Reference for AI

### DO for this project:
$doList

### DON'T for this project:
$dontList

---

## NuGet Packages Installed

$nugetPackages

---

**Last Updated**: ``$(Get-Date -Format "yyyy-MM-dd")``
**Generated by**: ``init-project.ps1`` v2.0
"@

# Write project-context.md to .claude/ (always a real folder now)
$projectContextContent | Out-File -FilePath $outputPath -Encoding UTF8 -Force
Write-Host "  [OK] Created .claude/project-context.md" -ForegroundColor Green

# Note: .claude/ is now always a real folder with symlinked subdirectories
Write-Host "  [INFO] .claude/ structure:" -ForegroundColor Cyan
Write-Host "        - project-context.md (real file, project-specific)" -ForegroundColor Gray
Write-Host "        - agents/ (symlink -> .standards/.claude/agents/)" -ForegroundColor Gray
Write-Host "        - commands/ (symlink -> .standards/.claude/commands/)" -ForegroundColor Gray
Write-Host "        - guides/ (symlink -> .standards/.claude/guides/)" -ForegroundColor Gray
Write-Host "        - workflows/ (symlink -> .standards/.claude/workflows/)" -ForegroundColor Gray

# Cleanup old .context/ folder if exists
if (Test-Path ".context") {
    Write-Host "  [INFO] Removing old .context/ folder..." -ForegroundColor Cyan
    Remove-Item ".context" -Recurse -Force -ErrorAction SilentlyContinue
    Write-Host "  [OK] Removed .context/ (deprecated)" -ForegroundColor Green
}

# Add project-context.md to git
git -c core.autocrlf=false add .claude/project-context.md 2>&1 | Out-Null
git commit -m "Add project context for AI assistants" 2>&1 | Out-Null
Write-Host "  [OK] Project context committed to git" -ForegroundColor Green


# ============================================================================
# Summary
# ============================================================================
Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "Project created successfully!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "[Location] $(Get-Location)" -ForegroundColor Cyan
Write-Host ""
Write-Host "[Configuration]" -ForegroundColor Yellow
Write-Host "  Project Name : $ProjectName"
Write-Host "  Framework    : $Framework"
Write-Host "  Database     : $Database"
Write-Host "  UI Framework : $UIFramework"
Write-Host "  Pattern      : $Pattern"
Write-Host "  Tests        : $(if ($IncludeTests) { 'Yes' } else { 'No' })"
Write-Host "  Standards    : $(if ($IntegrateStandards) { 'Yes' } else { 'No' })"
Write-Host ""
Write-Host "[Next steps]" -ForegroundColor Yellow
Write-Host "  1. cd $ProjectName"
Write-Host "  2. Open $ProjectName.sln in Visual Studio"
if ($Database -ne "None") {
    Write-Host "  3. Update connection string in appsettings.json"
    Write-Host "  4. Create your DbContext and models"
    Write-Host "  5. Run: dotnet ef migrations add InitialCreate"
    Write-Host "  6. Run: dotnet ef database update"
    Write-Host "  7. Start coding with $Pattern pattern!"
} else {
    Write-Host "  3. Start coding with $Pattern pattern!"
}
Write-Host ""
Write-Host "[Useful commands]" -ForegroundColor Yellow
Write-Host "  dotnet build              # Build project"
Write-Host "  dotnet run --project $ProjectName  # Run application"
if ($IncludeTests) {
    Write-Host "  dotnet test               # Run all tests"
}
if ($Database -ne "None") {
    Write-Host "  dotnet ef migrations add <name>   # Create migration"
    Write-Host "  dotnet ef database update         # Apply migrations"
}
Write-Host ""

if ($IntegrateStandards -and (Test-Path ".standards")) {
    Write-Host "[Coding Standards]" -ForegroundColor Yellow
    Write-Host "  .standards/USAGE_GUIDE.md     # Practical examples"
    Write-Host "  .standards/CLAUDE.md          # AI assistant guide"
    Write-Host "  .standards/docs/              # Full documentation"
    Write-Host ""
    Write-Host "[Claude Code Integration]" -ForegroundColor Yellow
    if (Test-Path ".claude") {
        Write-Host "  [OK] Slash commands available! Type / to see all commands" -ForegroundColor Green
    } else {
        Write-Host "  [WARN] .claude folder not found" -ForegroundColor Yellow
    }
    Write-Host ""
    Write-Host "[Update standards]" -ForegroundColor Yellow
    Write-Host "  cd .standards && git pull"
    Write-Host "  Copy-Item .standards\.claude\* .claude -Recurse -Force" -ForegroundColor Gray
    Write-Host "  Copy-Item .standards\templates\* templates -Recurse -Force" -ForegroundColor Gray
}
Write-Host ""
Write-Host "Happy coding!" -ForegroundColor Green
Write-Host ""
