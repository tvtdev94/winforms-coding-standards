# WinForms Project Initialization Script
# Creates a new WinForms project following coding standards

param(
    [Parameter(Mandatory=$true)]
    [string]$ProjectName,

    [Parameter(Mandatory=$false)]
    [ValidateSet('net8.0', 'net6.0', 'net48')]
    [string]$Framework = 'net8.0',

    [Parameter(Mandatory=$false)]
    [switch]$IncludeTests = $true,

    [Parameter(Mandatory=$false)]
    [switch]$IncludeExampleCode = $false,

    [Parameter(Mandatory=$false)]
    [switch]$IntegrateStandards = $true,

    [Parameter(Mandatory=$false)]
    [string]$StandardsRepo = ""
)

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "WinForms Project Initialization" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Green
Write-Host ""
Write-Host "Project Name: $ProjectName"
Write-Host "Framework: $Framework"
Write-Host "Include Tests: $IncludeTests"
Write-Host "Include Example Code: $IncludeExampleCode"
Write-Host "Integrate Standards: $IntegrateStandards"
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

$folders = @("Models", "Services", "Repositories", "Forms", "Views", "Presenters", "Data", "Utils", "Resources")
foreach ($folder in $folders) {
    New-Item -ItemType Directory -Path "$ProjectName/$folder" -Force | Out-Null
    Write-Host "  [OK] Created $folder/" -ForegroundColor Green
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
    "Serilog.Sinks.File",
    "Microsoft.EntityFrameworkCore.Sqlite",
    "Microsoft.EntityFrameworkCore.Design"
)

foreach ($package in $packages) {
    Write-Host "  Adding $package..." -NoNewline
    dotnet add $ProjectName package $package --no-restore | Out-Null
    Write-Host " [OK]" -ForegroundColor Green
}

Write-Host "  Restoring packages..." -NoNewline
dotnet restore $ProjectName | Out-Null
Write-Host " [OK]" -ForegroundColor Green

# ============================================================================
# Step 5: Create appsettings.json
# ============================================================================
Write-Host ""
Write-Host "[5] Creating appsettings.json..." -ForegroundColor Cyan

$appsettingsHash = @{
    ConnectionStrings = @{
        DefaultConnection = "Data Source=$ProjectName.db"
    }
    Logging = @{
        LogLevel = @{
            Default = "Information"
            Microsoft = "Warning"
            "Microsoft.EntityFrameworkCore" = "Information"
        }
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

# ============================================================================
# Step 6: Create Program.cs with DI
# ============================================================================
Write-Host ""
Write-Host "[6] Creating Program.cs with DI..." -ForegroundColor Cyan

$programCs = @'
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using {0}.Forms;

namespace {0}
{{
    internal static class Program
    {{
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {{
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {{
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
            }}
            catch (Exception ex)
            {{
                Log.Fatal(ex, "Application terminated unexpectedly");
                MessageBox.Show(
                    $"Fatal error: {{ex.Message}}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }}
            finally
            {{
                Log.CloseAndFlush();
            }}
        }}

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {{
            // Configuration
            services.AddSingleton(configuration);

            // Logging
            services.AddLogging(builder =>
            {{
                builder.ClearProviders();
                builder.AddSerilog();
            }});

            // TODO: Add your services here
            // services.AddDbContext<AppDbContext>(options =>
            //     options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            // services.AddScoped<IYourRepository, YourRepository>();
            // services.AddScoped<IYourService, YourService>();

            // Forms
            services.AddTransient<MainForm>();
        }}
    }}
}}
'@

$programCsFinal = $programCs -f $ProjectName
$programCsFinal | Out-File -FilePath "$ProjectName/Program.cs" -Encoding UTF8 -Force

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

    # Integration tests
    dotnet new xunit -n "$ProjectName.IntegrationTests" -f $Framework
    dotnet sln add "$ProjectName.IntegrationTests/$ProjectName.IntegrationTests.csproj"
    dotnet add "$ProjectName.IntegrationTests" reference $ProjectName
    dotnet add "$ProjectName.IntegrationTests" package Microsoft.EntityFrameworkCore.Sqlite --no-restore

    Write-Host "  [OK] Integration test project created" -ForegroundColor Green

    # Restore packages
    Write-Host "  Restoring test packages..." -NoNewline
    dotnet restore | Out-Null
    Write-Host " [OK]" -ForegroundColor Green
}

# ============================================================================
# Step 8: Copy .editorconfig and .gitignore
# ============================================================================
Write-Host ""
Write-Host "[8] Copying configuration files..." -ForegroundColor Cyan

$scriptPath = Split-Path -Parent $PSCommandPath
$repoRoot = Split-Path -Parent $scriptPath

if (Test-Path "$repoRoot/.editorconfig") {
    Copy-Item "$repoRoot/.editorconfig" -Destination "." -Force
    Write-Host "  [OK] .editorconfig copied" -ForegroundColor Green
}

if (Test-Path "$repoRoot/.gitignore") {
    Copy-Item "$repoRoot/.gitignore" -Destination "." -Force
    Write-Host "  [OK] .gitignore copied" -ForegroundColor Green
}

# ============================================================================
# Step 9: Initialize Git
# ============================================================================
Write-Host ""
Write-Host "[9] Initializing git repository..." -ForegroundColor Cyan

git init | Out-Null
git add . | Out-Null
git commit -m "Initial commit: Project structure created by init-project.ps1" | Out-Null

Write-Host "  [OK] Git repository initialized" -ForegroundColor Green

# ============================================================================
# Step 10: Integrate Coding Standards (if requested)
# ============================================================================
if ($IntegrateStandards) {
    Write-Host ""
    Write-Host "[10] Integrating coding standards..." -ForegroundColor Cyan

    # Auto-detect standards repo URL
    if (-not $StandardsRepo) {
        Push-Location $repoRoot
        $StandardsRepo = git remote get-url origin 2>$null
        Pop-Location
    }

    if ($StandardsRepo) {
        Write-Host "  Using standards repo: $StandardsRepo"

        # Add as submodule (suppress all output)
        Start-Process git -ArgumentList "submodule","add",$StandardsRepo,".standards" -NoNewWindow -Wait -RedirectStandardError "$env:TEMP\git_stderr.txt" -RedirectStandardOutput "$env:TEMP\git_stdout.txt"

        # Check if submodule was actually added
        if (Test-Path ".standards/.git") {
            git submodule update --init --recursive *>&1 | Out-Null
            Write-Host "  [OK] Standards added as submodule" -ForegroundColor Green

            # Try to create symlinks (requires Admin on Windows)
            $isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

            if ($isAdmin) {
                try {
                    New-Item -ItemType SymbolicLink -Path ".claude" -Target ".standards\.claude" -ErrorAction Stop | Out-Null
                    Write-Host "  [OK] Symlink created: .claude" -ForegroundColor Green
                } catch {
                    Write-Host "  [WARN]  Could not create .claude symlink (not critical)" -ForegroundColor Yellow
                }

                try {
                    New-Item -ItemType SymbolicLink -Path "templates" -Target ".standards\templates" -ErrorAction Stop | Out-Null
                    Write-Host "  [OK] Symlink created: templates" -ForegroundColor Green
                } catch {
                    Write-Host "  [WARN]  Could not create templates symlink (not critical)" -ForegroundColor Yellow
                }
            } else {
                Write-Host "  [WARN]  Run as Admin to create symlinks (optional)" -ForegroundColor Yellow
            }

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
# Step 11: Install Git Hooks (if available)
# ============================================================================
if (Test-Path "$repoRoot/.githooks") {
    Write-Host ""
    Write-Host "[11]  Installing git hooks..." -ForegroundColor Cyan

    Copy-Item "$repoRoot/.githooks" -Destination ".githooks" -Recurse -Force
    git config core.hooksPath .githooks

    Write-Host "  [OK] Git hooks installed" -ForegroundColor Green
}

# ============================================================================
# Summary
# ============================================================================
Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Green
Write-Host "Project created successfully!" -ForegroundColor Green
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Green
Write-Host ""
Write-Host "[Location] $(Get-Location)" -ForegroundColor Cyan
Write-Host ""
Write-Host "[Next steps]" -ForegroundColor Yellow
Write-Host "  1. cd $ProjectName"
Write-Host "  2. Open $ProjectName.sln in Visual Studio"
Write-Host "  3. Start coding with MVP pattern!"
Write-Host ""
Write-Host "[Useful commands]" -ForegroundColor Yellow
Write-Host "  dotnet build              # Build project"
Write-Host "  dotnet run --project $ProjectName  # Run application"
if ($IncludeTests) {
    Write-Host "  dotnet test               # Run all tests"
}
Write-Host ""

if ($IntegrateStandards -and (Test-Path ".standards")) {
    Write-Host "[Coding Standards]" -ForegroundColor Yellow
    Write-Host "  .standards/USAGE_GUIDE.md     # Practical examples"
    Write-Host "  .standards/CLAUDE.md          # AI assistant guide"
    Write-Host "  .standards/docs/              # Full documentation"
    if (Test-Path ".claude") {
        Write-Host "  Type / in Claude Code         # See slash commands"
    } else {
        Write-Host "  .standards/.claude/commands/  # Slash commands"
    }
    Write-Host ""
    Write-Host "[Update standards]" -ForegroundColor Yellow
    Write-Host "  cd .standards && git pull && cd .."
} else {
    Write-Host "[Documentation]" -ForegroundColor Yellow
    Write-Host "  See USAGE_GUIDE.md for practical examples"
    Write-Host "  See docs/ folder for detailed guidelines"
}
Write-Host ""
