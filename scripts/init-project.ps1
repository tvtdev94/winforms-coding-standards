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

# Question 4: Architecture Pattern
Write-Host "4. Architecture Pattern" -ForegroundColor Yellow
Write-Host ""

# Smart recommendation based on previous choices
$recommendedPattern = "MVP"  # Default safe choice
$patternRecommendation = ""

if ($Database -eq "None" -and ($Framework -eq "net48" -or $Framework -eq "net6.0")) {
    $recommendedPattern = "Simple"
    $patternRecommendation = "💡 Recommended: Simple (no database, lightweight app)"
}
elseif ($Framework -eq "net8.0" -and $Database -ne "None") {
    $recommendedPattern = "MVP"
    $patternRecommendation = "💡 Recommended: MVP (best balance of testability and simplicity)"
}
elseif ($Framework -eq "net48") {
    $recommendedPattern = "MVP"
    $patternRecommendation = "💡 Recommended: MVP (.NET Framework works best with MVP)"
}
else {
    $recommendedPattern = "MVP"
    $patternRecommendation = "💡 Recommended: MVP (works well in most scenarios)"
}

# Display options with descriptions
Write-Host "   [1] MVP (Model-View-Presenter)" -ForegroundColor $(if ($recommendedPattern -eq "MVP") { "Green" } else { "Gray" })
Write-Host "       ✅ Best for: Most WinForms apps" -ForegroundColor DarkGray
Write-Host "       ✅ Easy to test, clear separation" -ForegroundColor DarkGray
Write-Host "       ✅ Works with all .NET versions" -ForegroundColor DarkGray
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
    Write-Host "       ✅ Best for: Complex UI with data binding" -ForegroundColor DarkGray
    Write-Host "       ✅ Two-way binding, INotifyPropertyChanged" -ForegroundColor DarkGray
    Write-Host "       ⚠️  More complex than MVP" -ForegroundColor DarkGray
} else {
    Write-Host "       ❌ Not recommended for $Framework" -ForegroundColor DarkGray
    Write-Host "       ℹ️  Use .NET 6+ for better MVVM support" -ForegroundColor DarkGray
}
Write-Host ""

Write-Host "   [3] Simple (no pattern)" -ForegroundColor $(if ($recommendedPattern -eq "Simple") { "Green" } else { "Gray" })
Write-Host "       ✅ Best for: Quick prototypes, demos" -ForegroundColor DarkGray
Write-Host "       ⚠️  All code in Forms (harder to test)" -ForegroundColor DarkGray
Write-Host "       ⚠️  Not recommended for production" -ForegroundColor DarkGray
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

# Question 5: Include Tests
Write-Host "5. Unit & Integration Tests" -ForegroundColor Yellow
$includeTestsInput = Read-Host "   Include test projects? (Y/n)"
$IncludeTests = $includeTestsInput -ne "n" -and $includeTestsInput -ne "N"
Write-Host "   Tests: $(if ($IncludeTests) { 'Yes' } else { 'No' })" -ForegroundColor Green
Write-Host ""

# Question 6: Include Example Code
Write-Host "6. Example Code" -ForegroundColor Yellow
$includeExampleInput = Read-Host "   Include example code? (y/N)"
$IncludeExampleCode = $includeExampleInput -eq "y" -or $includeExampleInput -eq "Y"
Write-Host "   Example code: $(if ($IncludeExampleCode) { 'Yes' } else { 'No' })" -ForegroundColor Green
Write-Host ""

# Question 7: Integrate Standards
Write-Host "7. Coding Standards Integration" -ForegroundColor Yellow
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
Write-Host "Pattern         : $Pattern"
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

if (Test-Path $ProjectName) {
    Write-Host "[ERROR] Directory '$ProjectName' already exists!" -ForegroundColor Red
    exit 1
}

dotnet new sln -n $ProjectName -o $ProjectName
Set-Location $ProjectName

Write-Host "  [OK] Solution created" -ForegroundColor Green

# ============================================================================
# Step 2: Create Main Project
# ============================================================================
Write-Host ""
Write-Host "[2] Creating WinForms project..." -ForegroundColor Cyan

dotnet new winforms -n $ProjectName -f $Framework
dotnet sln add "$ProjectName/$ProjectName.csproj"

Write-Host "  [OK] WinForms project created" -ForegroundColor Green

# ============================================================================
# Step 3: Create Folder Structure
# ============================================================================
Write-Host ""
Write-Host "[3] Creating folder structure..." -ForegroundColor Cyan

$folders = @(
    @{Name="Models"; Template="// Place your data models here`n// Example: Customer.cs, Order.cs`n"},
    @{Name="Services"; Template="// Place your business logic services here`n// Example: CustomerService.cs, OrderService.cs`n"},
    @{Name="Repositories"; Template="// Place your data access repositories here`n// Example: CustomerRepository.cs, OrderRepository.cs`n"},
    @{Name="Forms"; Template=$null},  # MainForm will be moved here
    @{Name="Utils"; Template="// Place your utility classes and extensions here`n// Example: StringExtensions.cs, DateHelper.cs`n"},
    @{Name="Resources"; Template="// Place your resources here`n// Example: Icons/, Images/, Strings.resx`n"}
)

# Add pattern-specific folders
if ($Pattern -eq "MVP") {
    $folders += @{Name="Views"; Template="// Place your view interfaces here (for MVP pattern)`n// Example: ICustomerView.cs, IOrderView.cs`n"}
    $folders += @{Name="Presenters"; Template="// Place your presenters here (for MVP pattern)`n// Example: CustomerPresenter.cs, OrderPresenter.cs`n"}
}
elseif ($Pattern -eq "MVVM") {
    $folders += @{Name="ViewModels"; Template="// Place your view models here (for MVVM pattern)`n// Example: CustomerViewModel.cs, OrderViewModel.cs`n"}
}

# Add Data folder if database is selected
if ($Database -ne "None") {
    $folders += @{Name="Data"; Template="// Place your DbContext and configurations here`n// Example: AppDbContext.cs, EntityConfigurations/`n"}
}

foreach ($folder in $folders) {
    New-Item -ItemType Directory -Path "$ProjectName/$($folder.Name)" -Force | Out-Null

    # Add README.md to help Rider/VS show the folder
    if ($folder.Template) {
        $readmeContent = "# $($folder.Name)`n`n$($folder.Template)"
        $readmeContent | Out-File -FilePath "$ProjectName/$($folder.Name)/README.md" -Encoding UTF8 -Force
    }

    Write-Host "  [OK] Created $($folder.Name)/" -ForegroundColor Green
}

# Move Form1 to Forms folder and rename to MainForm
Move-Item -Path "$ProjectName/Form1.cs" -Destination "$ProjectName/Forms/MainForm.cs" -Force
Move-Item -Path "$ProjectName/Form1.Designer.cs" -Destination "$ProjectName/Forms/MainForm.Designer.cs" -Force
if (Test-Path "$ProjectName/Form1.resx") {
    Move-Item -Path "$ProjectName/Form1.resx" -Destination "$ProjectName/Forms/MainForm.resx" -Force
}

# Update MainForm.cs namespace
$mainFormContent = Get-Content "$ProjectName/Forms/MainForm.cs" -Raw
$mainFormContent = $mainFormContent -replace "namespace $ProjectName", "namespace $ProjectName.Forms"
$mainFormContent = $mainFormContent -replace "partial class Form1", "partial class MainForm"
$mainFormContent = $mainFormContent -replace "public Form1\(\)", "public MainForm()"
$mainFormContent | Out-File -FilePath "$ProjectName/Forms/MainForm.cs" -Encoding UTF8 -Force

# Update MainForm.Designer.cs
$designerContent = Get-Content "$ProjectName/Forms/MainForm.Designer.cs" -Raw
$designerContent = $designerContent -replace "namespace $ProjectName", "namespace $ProjectName.Forms"
$designerContent = $designerContent -replace "partial class Form1", "partial class MainForm"
$designerContent = $designerContent -replace "Form1", "MainForm"
$designerContent | Out-File -FilePath "$ProjectName/Forms/MainForm.Designer.cs" -Encoding UTF8 -Force

Write-Host "  [OK] Moved and renamed Form1 to MainForm in Forms/" -ForegroundColor Green

# ============================================================================
# Step 4: Add NuGet Packages
# ============================================================================
Write-Host ""
Write-Host "[4] Adding NuGet packages..." -ForegroundColor Cyan

$packages = @(
    "Microsoft.Extensions.DependencyInjection",
    "Microsoft.Extensions.Configuration.Json",
    "Microsoft.Extensions.Logging",
    "Serilog.Extensions.Logging",
    "Serilog.Sinks.File"
)

# Add database-specific packages
if ($Database -eq "SQLite") {
    $packages += "Microsoft.EntityFrameworkCore.Sqlite"
    $packages += "Microsoft.EntityFrameworkCore.Design"
}
elseif ($Database -eq "SQLServer") {
    $packages += "Microsoft.EntityFrameworkCore.SqlServer"
    $packages += "Microsoft.EntityFrameworkCore.Design"
}
elseif ($Database -eq "PostgreSQL") {
    $packages += "Npgsql.EntityFrameworkCore.PostgreSQL"
    $packages += "Microsoft.EntityFrameworkCore.Design"
}
elseif ($Database -eq "MySQL") {
    $packages += "Pomelo.EntityFrameworkCore.MySql"
    $packages += "Microsoft.EntityFrameworkCore.Design"
}

foreach ($package in $packages) {
    Write-Host "  Adding $package..." -NoNewline
    dotnet add $ProjectName package $package --no-restore | Out-Null
    Write-Host " [OK]" -ForegroundColor Green
}

Write-Host "  Restoring packages..." -NoNewline
dotnet restore $ProjectName | Out-Null
Write-Host " [OK]" -ForegroundColor Green

# Add README.md files to .csproj so they show in Rider/VS
$csprojPath = "$ProjectName/$ProjectName.csproj"
$csprojContent = Get-Content $csprojPath -Raw

# Add ItemGroup for README files if not already present
if (-not $csprojContent.Contains("<None Include=")) {
    $readmeFiles = Get-ChildItem -Path "$ProjectName" -Recurse -Filter "README.md" | ForEach-Object {
        $relativePath = $_.FullName.Replace("$((Get-Location).Path)\$ProjectName\", "").Replace("\", "/")
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

$appsettingsHash | ConvertTo-Json -Depth 10 | Out-File -FilePath "$ProjectName/appsettings.json" -Encoding UTF8

# Update .csproj to copy appsettings.json
$csprojPath = "$ProjectName/$ProjectName.csproj"
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

$programCs = @"
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using $ProjectName.Forms;

namespace $ProjectName
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
                Application.Run(mainForm);
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

$programCs | Out-File -FilePath "$ProjectName/Program.cs" -Encoding UTF8 -Force

Write-Host "  [OK] Program.cs created with DI" -ForegroundColor Green

# ============================================================================
# Step 7: Create Test Project (if requested)
# ============================================================================
if ($IncludeTests) {
    Write-Host ""
    Write-Host "[7] Creating test projects..." -ForegroundColor Cyan

    # Unit tests
    dotnet new xunit -n "$ProjectName.Tests" -f $Framework
    dotnet sln add "$ProjectName.Tests/$ProjectName.Tests.csproj"
    dotnet add "$ProjectName.Tests" reference $ProjectName
    dotnet add "$ProjectName.Tests" package Moq --no-restore
    dotnet add "$ProjectName.Tests" package FluentAssertions --no-restore

    Write-Host "  [OK] Unit test project created" -ForegroundColor Green

    # Integration tests (only if database is selected)
    if ($Database -ne "None") {
        dotnet new xunit -n "$ProjectName.IntegrationTests" -f $Framework
        dotnet sln add "$ProjectName.IntegrationTests/$ProjectName.IntegrationTests.csproj"
        dotnet add "$ProjectName.IntegrationTests" reference $ProjectName

        # Add database package to integration tests
        if ($Database -eq "SQLite") {
            dotnet add "$ProjectName.IntegrationTests" package Microsoft.EntityFrameworkCore.Sqlite --no-restore
        }
        elseif ($Database -eq "SQLServer") {
            dotnet add "$ProjectName.IntegrationTests" package Microsoft.EntityFrameworkCore.SqlServer --no-restore
        }
        elseif ($Database -eq "PostgreSQL") {
            dotnet add "$ProjectName.IntegrationTests" package Npgsql.EntityFrameworkCore.PostgreSQL --no-restore
        }
        elseif ($Database -eq "MySQL") {
            dotnet add "$ProjectName.IntegrationTests" package Pomelo.EntityFrameworkCore.MySql --no-restore
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
$launchJson = @"
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Launch (WinForms)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "`${workspaceFolder}/$ProjectName/bin/Debug/$Framework-windows/$ProjectName.dll",
      "args": [],
      "cwd": "`${workspaceFolder}/$ProjectName",
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

git init | Out-Null
git add . | Out-Null
git commit -m "Initial commit: Project structure created by init-project-interactive.ps1" | Out-Null

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

            # Commit submodule
            git add .gitmodules .standards
            git commit -m "Add coding standards as submodule" | Out-Null
            Write-Host "  [OK] Standards integration complete" -ForegroundColor Green
        } else {
            Write-Host "  [WARN]  Could not add standards submodule" -ForegroundColor Yellow
        }
    } else {
        Write-Host "  [WARN]  Standards repo URL not detected, skipping" -ForegroundColor Yellow
    }
}

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
    Write-Host "  Type / in Claude Code         # See slash commands"
    Write-Host ""
    Write-Host "[Update standards]" -ForegroundColor Yellow
    Write-Host "  cd .standards && git pull && cd .."
}
Write-Host ""
Write-Host "Happy coding! 🚀" -ForegroundColor Green
Write-Host ""
