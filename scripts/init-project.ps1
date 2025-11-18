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

# Question 6: Include Tests
Write-Host "6. Unit & Integration Tests" -ForegroundColor Yellow
$includeTestsInput = Read-Host "   Include test projects? (Y/n)"
$IncludeTests = $includeTestsInput -ne "n" -and $includeTestsInput -ne "N"
Write-Host "   Tests: $(if ($IncludeTests) { 'Yes' } else { 'No' })" -ForegroundColor Green
Write-Host ""

# Question 7: Include Example Code
Write-Host "7. Example Code" -ForegroundColor Yellow
$includeExampleInput = Read-Host "   Include example code? (y/N)"
$IncludeExampleCode = $includeExampleInput -eq "y" -or $includeExampleInput -eq "Y"
Write-Host "   Example code: $(if ($IncludeExampleCode) { 'Yes' } else { 'No' })" -ForegroundColor Green
Write-Host ""

# Question 8: Integrate Standards
Write-Host "8. Coding Standards Integration" -ForegroundColor Yellow
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
}

# Determine EF Core version based on .NET version
$efCoreVersion = switch ($Framework) {
    "net8.0" { "8.0.11" }
    "net6.0" { "6.0.36" }
    "net48"  { "6.0.36" }  # .NET Framework 4.8 uses EF Core 6.x (last version supporting .NET Framework)
    default  { "8.0.11" }
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
    # Npgsql follows EF Core versioning
    $packages += "Npgsql.EntityFrameworkCore.PostgreSQL:$efCoreVersion"
    $packages += "Microsoft.EntityFrameworkCore.Design:$efCoreVersion"
}
elseif ($Database -eq "MySQL") {
    # Pomelo.EntityFrameworkCore.MySql versioning
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
    # Split package name and version
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

# Generate using statements based on database
$usingStatements = if ($Database -ne "None") {
    switch ($Database) {
        "SQLite" { "using Microsoft.EntityFrameworkCore;`nusing $ProjectName.Data;" }
        "SQLServer" { "using Microsoft.EntityFrameworkCore;`nusing $ProjectName.Data;" }
        "PostgreSQL" { "using Microsoft.EntityFrameworkCore;`nusing $ProjectName.Data;" }
        "MySQL" { "using Microsoft.EntityFrameworkCore;`nusing $ProjectName.Data;" }
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

$programCs = @"
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using $ProjectName.Forms;
$usingStatements

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
# Step 6.5: Create AppDbContext (if database is selected)
# ============================================================================
if ($Database -ne "None") {
    Write-Host ""
    Write-Host "[6.5] Creating AppDbContext..." -ForegroundColor Cyan

    $appDbContextCs = @"
using Microsoft.EntityFrameworkCore;

namespace $ProjectName.Data
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

    $appDbContextCs | Out-File -FilePath "$ProjectName/Data/AppDbContext.cs" -Encoding UTF8 -Force
    Write-Host "  [OK] AppDbContext.cs created" -ForegroundColor Green
}

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

            # Create symlinks for .claude and templates (requires Admin on Windows)
            Write-Host ""
            Write-Host "  Creating symlinks for Claude Code integration..." -ForegroundColor Cyan

            # Check if running as Administrator
            $isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

            if ($isAdmin) {
                try {
                    # Create symlink for .claude directory
                    if (Test-Path ".standards\.claude") {
                        New-Item -ItemType SymbolicLink -Path ".claude" -Target ".standards\.claude" -Force -ErrorAction Stop | Out-Null
                        Write-Host "  [OK] Created symlink: .claude -> .standards\.claude" -ForegroundColor Green
                    }

                    # Create symlink for templates directory
                    if (Test-Path ".standards\templates") {
                        New-Item -ItemType SymbolicLink -Path "templates" -Target ".standards\templates" -Force -ErrorAction Stop | Out-Null
                        Write-Host "  [OK] Created symlink: templates -> .standards\templates" -ForegroundColor Green
                    }

                    # Create symlink for CLAUDE.md (important for Claude Code context)
                    if (Test-Path ".standards\CLAUDE.md") {
                        New-Item -ItemType SymbolicLink -Path "CLAUDE.md" -Target ".standards\CLAUDE.md" -Force -ErrorAction Stop | Out-Null
                        Write-Host "  [OK] Created symlink: CLAUDE.md -> .standards\CLAUDE.md" -ForegroundColor Green
                    }

                    Write-Host "  [OK] Symlinks created successfully" -ForegroundColor Green
                    Write-Host "  [INFO] Claude Code will now see all slash commands!" -ForegroundColor Cyan
                } catch {
                    Write-Host "  [WARN] Could not create symlinks: $($_.Exception.Message)" -ForegroundColor Yellow
                    Write-Host "  [INFO] Falling back to copying files..." -ForegroundColor Cyan

                    # Fallback: Copy instead of symlink
                    if (Test-Path ".standards\.claude") {
                        Copy-Item -Recurse ".standards\.claude" -Destination ".claude" -Force
                        Write-Host "  [OK] Copied .claude directory" -ForegroundColor Green
                    }
                    if (Test-Path ".standards\templates") {
                        Copy-Item -Recurse ".standards\templates" -Destination "templates" -Force
                        Write-Host "  [OK] Copied templates directory" -ForegroundColor Green
                    }
                    if (Test-Path ".standards\CLAUDE.md") {
                        Copy-Item ".standards\CLAUDE.md" -Destination "CLAUDE.md" -Force
                        Write-Host "  [OK] Copied CLAUDE.md" -ForegroundColor Green
                    }
                }
            } else {
                # Not running as Admin - copy instead
                Write-Host "  [WARN] Not running as Administrator - cannot create symlinks" -ForegroundColor Yellow
                Write-Host "  [INFO] Copying files instead (will not auto-update with standards)" -ForegroundColor Cyan

                if (Test-Path ".standards\.claude") {
                    Copy-Item -Recurse ".standards\.claude" -Destination ".claude" -Force
                    Write-Host "  [OK] Copied .claude directory" -ForegroundColor Green
                }
                if (Test-Path ".standards\templates") {
                    Copy-Item -Recurse ".standards\templates" -Destination "templates" -Force
                    Write-Host "  [OK] Copied templates directory" -ForegroundColor Green
                }
                if (Test-Path ".standards\CLAUDE.md") {
                    Copy-Item ".standards\CLAUDE.md" -Destination "CLAUDE.md" -Force
                    Write-Host "  [OK] Copied CLAUDE.md" -ForegroundColor Green
                }

                Write-Host ""
                Write-Host "  [TIP] To get auto-updating standards, re-run this script as Administrator:" -ForegroundColor Yellow
                Write-Host "        Right-click PowerShell -> Run as Administrator" -ForegroundColor Gray
            }

            # Commit submodule and symlinks/copies
            git -c core.autocrlf=false add .gitmodules .standards 2>&1 | Out-Null
            if (Test-Path ".claude") {
                git -c core.autocrlf=false add .claude 2>&1 | Out-Null
            }
            if (Test-Path "templates") {
                git -c core.autocrlf=false add templates 2>&1 | Out-Null
            }
            if (Test-Path "CLAUDE.md") {
                git -c core.autocrlf=false add CLAUDE.md 2>&1 | Out-Null
            }
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

# Project-specific context should NOT go in submodule
# Create context/ for project-specific files
$contextPath = "context"
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

# Template list
$templateList = switch ($UIFramework) {
    "Standard" { @"
- Form: [$($standardsPath.Replace('\', '/'))/templates/form-template.cs]($($standardsPath.Replace('\', '/'))/templates/form-template.cs)
- Service: [$($standardsPath.Replace('\', '/'))/templates/service-template.cs]($($standardsPath.Replace('\', '/'))/templates/service-template.cs)
- Repository: [$($standardsPath.Replace('\', '/'))/templates/repository-template.cs]($($standardsPath.Replace('\', '/'))/templates/repository-template.cs)
- Test: [$($standardsPath.Replace('\', '/'))/templates/test-template.cs]($($standardsPath.Replace('\', '/'))/templates/test-template.cs)
"@ }
    "DevExpress" { @"
- Form: [$($standardsPath.Replace('\', '/'))/templates/dx-form-template.cs]($($standardsPath.Replace('\', '/'))/templates/dx-form-template.cs)
- Grid: [$($standardsPath.Replace('\', '/'))/templates/dx-grid-template.cs]($($standardsPath.Replace('\', '/'))/templates/dx-grid-template.cs)
- Service: [$($standardsPath.Replace('\', '/'))/templates/service-template.cs]($($standardsPath.Replace('\', '/'))/templates/service-template.cs)
- Repository: [$($standardsPath.Replace('\', '/'))/templates/repository-template.cs]($($standardsPath.Replace('\', '/'))/templates/repository-template.cs)
- Test: [$($standardsPath.Replace('\', '/'))/templates/test-template.cs]($($standardsPath.Replace('\', '/'))/templates/test-template.cs)
"@ }
    "ReaLTaiizor" { @"
- Material Form: [$($standardsPath.Replace('\', '/'))/templates/rt-material-form-template.cs]($($standardsPath.Replace('\', '/'))/templates/rt-material-form-template.cs)
- Metro Form: [$($standardsPath.Replace('\', '/'))/templates/rt-metro-form-template.cs]($($standardsPath.Replace('\', '/'))/templates/rt-metro-form-template.cs)
- Controls: [$($standardsPath.Replace('\', '/'))/templates/rt-controls-patterns.cs]($($standardsPath.Replace('\', '/'))/templates/rt-controls-patterns.cs)
- Service: [$($standardsPath.Replace('\', '/'))/templates/service-template.cs]($($standardsPath.Replace('\', '/'))/templates/service-template.cs)
- Repository: [$($standardsPath.Replace('\', '/'))/templates/repository-template.cs]($($standardsPath.Replace('\', '/'))/templates/repository-template.cs)
- Test: [$($standardsPath.Replace('\', '/'))/templates/test-template.cs]($($standardsPath.Replace('\', '/'))/templates/test-template.cs)
"@ }
    default { "See /templates/ folder" }
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

---

## AI Instructions - READ THIS!

### 🎯 When Creating Forms

$formInstructions

### 🎯 When Creating Services/Repositories

$serviceInstructions

### 🎯 When Writing Tests

$testInstructions

### 🎯 Available Templates

Based on this project configuration, use these templates:

$templateList

---

## Project Structure

``````
/$ProjectName
|-- /Forms              # UI Layer (minimal logic)
$additionalFolders
|-- /Services           # Business logic
|-- /Repositories       # Data access layer
|-- /Data               # DbContext, Unit of Work
+-- Program.cs
``````

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

# Write to both locations initially
$projectContextContent | Out-File -FilePath $outputPath -Encoding UTF8 -Force
Write-Host "  [OK] Created context/project-context.md" -ForegroundColor Green

# Also write to .claude/ directory (if it exists and is writable)
$claudeDest = ".claude/project-context.md"
$projectContextContent | Out-File -FilePath $claudeDest -Encoding UTF8 -Force -ErrorAction SilentlyContinue
if (Test-Path $claudeDest) {
    Write-Host "  [OK] Created .claude/project-context.md (for AI access)" -ForegroundColor Green
}

# Cleanup: Decide which file to keep based on whether .claude is a symlink
Write-Host ""
Write-Host "  [INFO] Checking .claude directory type..." -ForegroundColor Cyan

# Check if .claude is a symbolic link
$claudeIsSymlink = $false
if (Test-Path ".claude") {
    $claudeItem = Get-Item ".claude" -Force -ErrorAction SilentlyContinue
    if ($claudeItem -and $claudeItem.LinkType -eq "SymbolicLink") {
        $claudeIsSymlink = $true
        Write-Host "  [INFO] .claude is a symlink (points to submodule)" -ForegroundColor Cyan
    }
}

if ($claudeIsSymlink) {
    # .claude is symlink → Keep context/project-context.md, remove .claude/project-context.md
    Write-Host "  [INFO] Keeping context/project-context.md (cannot track files in symlinked directory)" -ForegroundColor Cyan
    if (Test-Path $claudeDest) {
        Remove-Item $claudeDest -Force -ErrorAction SilentlyContinue
        Write-Host "  [OK] Removed .claude/project-context.md (symlink to submodule)" -ForegroundColor Green
    }
    # Add context/project-context.md to git
    git -c core.autocrlf=false add context/project-context.md 2>&1 | Out-Null
    git commit -m "Add project context for AI assistants" 2>&1 | Out-Null
    Write-Host "  [OK] Project context committed to git (context/project-context.md)" -ForegroundColor Green
    Write-Host ""
    Write-Host "  [TIP] AI assistants will find project-context.md in:" -ForegroundColor Yellow
    Write-Host "        - context/project-context.md (project-specific)" -ForegroundColor Gray
    Write-Host "        - .claude/ (from standards submodule via symlink)" -ForegroundColor Gray
} else {
    # .claude is NOT symlink → Keep .claude/project-context.md, remove context/project-context.md
    if (Test-Path $claudeDest) {
        Write-Host "  [INFO] Keeping .claude/project-context.md (AI can access directly)" -ForegroundColor Cyan
        if (Test-Path $outputPath) {
            Remove-Item $outputPath -Force
            Write-Host "  [OK] Removed context/project-context.md (keeping only .claude version)" -ForegroundColor Green
        }
        # Add .claude/project-context.md to git
        git -c core.autocrlf=false add .claude/project-context.md 2>&1 | Out-Null
        git commit -m "Add project context for AI assistants" 2>&1 | Out-Null
        Write-Host "  [OK] Project context committed to git (.claude/project-context.md)" -ForegroundColor Green
    } else {
        Write-Host "  [WARN] .claude/project-context.md not found, keeping context/project-context.md" -ForegroundColor Yellow
        # Add context/project-context.md to git as fallback
        git -c core.autocrlf=false add context/project-context.md 2>&1 | Out-Null
        git commit -m "Add project context for AI assistants" 2>&1 | Out-Null
        Write-Host "  [OK] Project context committed to git (context/project-context.md)" -ForegroundColor Green
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
        Write-Host "  [OK] Slash commands available!" -ForegroundColor Green
        Write-Host "  Type / in Claude Code to see 19 commands:" -ForegroundColor Cyan
        Write-Host "    /create:form, /create:service, /create:repository" -ForegroundColor Gray
        Write-Host "    /add:validation, /add:logging, /add:error-handling" -ForegroundColor Gray
        Write-Host "    /fix:bug, /fix:threading, /fix:performance" -ForegroundColor Gray
        Write-Host "    /auto-implement - Auto-create complete features!" -ForegroundColor Gray
    } else {
        Write-Host "  [WARN] Slash commands not available" -ForegroundColor Yellow
        Write-Host "  Run as Administrator to enable symlinks" -ForegroundColor Gray
    }
    Write-Host ""
    Write-Host "[Update standards]" -ForegroundColor Yellow
    Write-Host "  cd .standards && git pull && cd .."
    if (Test-Path ".claude") {
        $claudeLinkType = (Get-Item ".claude").LinkType
        if ($claudeLinkType -eq "SymbolicLink") {
            Write-Host "  (Symlinks will auto-update slash commands)" -ForegroundColor Gray
        } else {
            Write-Host "  (Re-copy .claude after updating: Copy-Item .standards\.claude .claude -Recurse -Force)" -ForegroundColor Gray
        }
    }
}
Write-Host ""
Write-Host "Happy coding!" -ForegroundColor Green
Write-Host ""
